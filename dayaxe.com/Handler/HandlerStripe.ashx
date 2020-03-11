<%@ WebHandler Language="C#" Class="dayaxe.com.Handler.HandlerStripe" %>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using DayaxeDal;
using DayaxeDal.Custom;
using DayaxeDal.Extensions;
using DayaxeDal.Parameters;
using DayaxeDal.Repositories;
using Newtonsoft.Json;
using Stripe;

namespace dayaxe.com.Handler
{
    public class HandlerStripe : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {

            var json = new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();
            var stripeEvent = StripeEventUtility.ParseEvent(json);
            using (var subscriptionRepository = new SubscriptionRepository())
            {
                try
                {
                    var logs = new Logs
                    {
                        LogKey = "WebHooksHandlerStripeInvoice",
                        UpdatedBy = 1,
                        UpdatedContent = string.Format("{0}", json),
                        UpdatedDate = DateTime.UtcNow
                    };
                    subscriptionRepository.AddLog(logs);

                    ReturnSubscriptionInvoiceObject invoices = JsonConvert.DeserializeObject<ReturnSubscriptionInvoiceObject>(stripeEvent.Data.Object.ToString());
                    string chargeId = invoices.Charge;
                    var invoiceList = invoices.Lines.Data.FirstOrDefault();
                    var customerId = invoices.Customer;
                    var invoiceId = invoices.Id;
                    var subscriptionId = invoices.Subscription;
                    switch (stripeEvent.Type)
                    {
                        case "invoice.created":
                            #region invoice.created

                            var customerCredits = subscriptionRepository.GetCustomerCreditsByStripeCustomerId(customerId);
                            if (customerCredits != null && customerCredits.Amount > 0)
                            {
                                var stripeSubscriptionId = invoiceList != null ? invoiceList.Plan.Id : String.Empty;
                                var subscriptions = subscriptionRepository.SubscriptionsList
                                    .FirstOrDefault(s => s.StripePlanId.Equals(stripeSubscriptionId, StringComparison.OrdinalIgnoreCase));

                                if (subscriptions != null)
                                {
                                    var stripeCouponId = string.Format("{0}SC{1}",
                                        customerCredits.CustomerId,
                                        Helper.RandomString(Constant.BookingCodeLength));
                                    var couponPrice = customerCredits.Amount >= subscriptions.Price ? subscriptions.Price : customerCredits.Amount;
                                    var couponOptions = new StripeCouponCreateOptions
                                    {
                                        Id = stripeCouponId,
                                        AmountOff = Convert.ToInt32(couponPrice * 100), // USD
                                        Duration = "once",
                                        Currency = "USD"
                                    };

                                    // Create Coupon
                                    var couponService = new StripeCouponService();
                                    var coupon = couponService.Create(couponOptions);
                                    var invoiceOptions = new StripeInvoiceUpdateOptions
                                    {
                                        CouponId = coupon.Id
                                    };

                                    var invoiceService = new StripeInvoiceService();
                                    var invoice = invoiceService.Update(invoices.Id, invoiceOptions);

                                    using (var customerCreditRepository = new CustomerCreditRepository())
                                    {
                                        customerCredits.Amount -= couponPrice;
                                        customerCredits.LogDescriptionCredits = string.Format("{0} – {1}",
                                            subscriptions.Name,
                                            stripeCouponId);
                                        customerCredits.SubscriptionId = subscriptions.Id;
                                        customerCredits.LogsItem = new Logs
                                        {
                                            LogKey = "Invoices_Created_Event_Renewal",
                                            UpdatedContent = string.Format("Stripe Post Data: {0} - Response: {1}", json, invoice.StripeResponse.ObjectJson),
                                            UpdatedBy = 0,
                                            UpdatedDate = DateTime.UtcNow
                                        };
                                        customerCreditRepository.Update(customerCredits);

                                        CacheLayer.Clear(CacheKeys.CustomerCreditsCacheKey);
                                        CacheLayer.Clear(CacheKeys.CustomerCreditLogsCacheKey);
                                    }
                                }
                            }

                            #endregion
                            break;
                        case "invoice.payment_succeeded":
                            #region invoice.payment_succeeded

                            if (invoiceList != null)
                            {
                                var subscription = subscriptionRepository.GetById(invoiceList.Plan.Id);

                                if (subscription != null)
                                {
                                    var chargeOptions = new StripeChargeUpdateOptions()
                                    {
                                        Description = String.Format("Subscription - {0} - {1}", subscription.Name,
                                            subscription.ProductHighlight)
                                    };
                                    var chargeService = new StripeChargeService();
                                    chargeService.Update(chargeId, chargeOptions);

                                    var subscriptionBooking = subscriptionRepository.SubscriptionBookingsList
                                        .FirstOrDefault(sb => sb.StripeSubscriptionId == subscriptionId);

                                    if (subscriptionBooking != null)
                                    {
                                        double periodStart = invoiceList.Period.Start,
                                            periodEnd = invoiceList.Period.End;

                                        var currentSubscriptionCycle = (from sb in subscriptionRepository.SubscriptionBookingsList
                                                                        join sc in subscriptionRepository.SubscriptionCyclesList on sb.Id equals sc.SubscriptionBookingId
                                                                        where sb.StripeSubscriptionId.Equals(subscriptionId, StringComparison.OrdinalIgnoreCase)
                                                                        orderby sc.CycleNumber descending
                                                                        select sc).FirstOrDefault();

                                        var isCurrentCycle = currentSubscriptionCycle != null &&
                                                             currentSubscriptionCycle.StartDate.HasValue &&
                                                             currentSubscriptionCycle.EndDate.HasValue &&
                                                             currentSubscriptionCycle.StartDate.Value.Date == periodStart.ToDateTime().Date &&
                                                             currentSubscriptionCycle.EndDate.Value.Date == periodEnd.ToDateTime().Date;

                                        var invoiceService = new StripeInvoiceService();
                                        var invoiceItems = invoiceService.Get(invoiceId);
                                        double chargeAmount = 0;
                                        string stripeCouponId = string.Empty;
                                        StripeCharge charge = new StripeCharge();

                                        if (!string.IsNullOrEmpty(invoiceItems.ChargeId))
                                        {
                                            charge = chargeService.Get(invoiceItems.ChargeId);
                                            chargeAmount = (double)charge.Amount / 100;
                                            stripeCouponId = invoiceItems.StripeDiscount != null && invoiceItems.StripeDiscount.StripeCoupon != null ? invoiceItems.StripeDiscount.StripeCoupon.Id : string.Empty;
                                        }

                                        var param = new AddSubscriptionCycleParams
                                        {
                                            CanceledDate = null,
                                            SubscriptionInvoices = new List<SubscriptionInvoices>()
                                        };

                                        if (isCurrentCycle)
                                        {
                                            currentSubscriptionCycle.StartDate = periodStart.ToDateTime();
                                            currentSubscriptionCycle.EndDate = periodEnd.ToDateTime();
                                            currentSubscriptionCycle.LastUpdatedDate = DateTime.UtcNow;
                                            currentSubscriptionCycle.Price = chargeAmount;
                                            currentSubscriptionCycle.StripeInvoiceId = invoiceId;
                                            currentSubscriptionCycle.StripeChargeId = invoiceItems.ChargeId;
                                            currentSubscriptionCycle.StripeCouponId = stripeCouponId;

                                            param.SubscriptionCyclesObject = currentSubscriptionCycle;

                                            if (invoices.Paid)
                                            {
                                                var subscriptionInvoice = new SubscriptionInvoices
                                                {
                                                    SubscriptionCyclesId = currentSubscriptionCycle.Id,
                                                    BookingStatus = currentSubscriptionCycle.Status,
                                                    Quantity = 1,
                                                    Price = chargeAmount,
                                                    MerchantPrice = 50,
                                                    PayByCredit = 0,
                                                    TotalPrice = chargeAmount,
                                                    InvoiceStatus = (int)Enums.InvoiceStatus.Charge,
                                                    StripeChargeId = charge.Id,
                                                    ChargeAmount = chargeAmount,
                                                    StripeRefundId = string.Empty,
                                                    RefundAmount = 0,
                                                    RefundCreditAmount = 0,
                                                    StripeCouponId = stripeCouponId,
                                                    CreatedDate = DateTime.UtcNow,
                                                    CreatedBy = 1
                                                };

                                                param.SubscriptionInvoices.Add(subscriptionInvoice);

                                                subscriptionRepository.UpdateSubscriptionCycle(param);
                                            }
                                        }
                                        else // Not Current Cycle
                                        {
                                            if (!string.IsNullOrEmpty(invoiceItems.ChargeId))
                                            {
                                                var subscriptionCycle = new SubscriptionCycles
                                                {
                                                    SubscriptionBookingId = subscriptionBooking.Id,
                                                    StartDate = periodStart.ToDateTime(),
                                                    EndDate = periodEnd.ToDateTime(),
                                                    CancelDate = null,
                                                    Status = subscriptionBooking.Status,
                                                    LastUpdatedDate = DateTime.UtcNow,
                                                    LastUpdatedBy = 1,
                                                    Price = chargeAmount,
                                                    MerchantPrice = 50,
                                                    PayByCredit = 0,
                                                    TotalPrice = chargeAmount,
                                                    Quantity = 1,
                                                    StripeInvoiceId = invoiceId,
                                                    StripeChargeId = invoiceItems.ChargeId,
                                                    StripeCouponId = stripeCouponId,
                                                    CycleNumber = currentSubscriptionCycle != null ? ++currentSubscriptionCycle.CycleNumber : (short)1
                                                };

                                                param.SubscriptionCyclesObject = subscriptionCycle;

                                                if (invoices.Paid)
                                                {
                                                    var subscriptionInvoice = new SubscriptionInvoices
                                                    {
                                                        SubscriptionCyclesId = subscriptionCycle.Id,
                                                        BookingStatus = subscriptionCycle.Status,
                                                        Quantity = 1,
                                                        Price = chargeAmount,
                                                        MerchantPrice = 50,
                                                        PayByCredit = 0,
                                                        TotalPrice = chargeAmount,
                                                        InvoiceStatus = (int)Enums.InvoiceStatus.Charge,
                                                        StripeChargeId = charge.Id,
                                                        ChargeAmount = chargeAmount,
                                                        StripeRefundId = string.Empty,
                                                        RefundAmount = 0,
                                                        RefundCreditAmount = 0,
                                                        StripeCouponId = stripeCouponId,
                                                        CreatedDate = DateTime.UtcNow,
                                                        CreatedBy = 1
                                                    };

                                                    param.SubscriptionInvoices.Add(subscriptionInvoice);
                                                }

                                                subscriptionRepository.AddSubscriptionCycle(param);
                                            }
                                        }

                                        CacheLayer.Clear(CacheKeys.SubscriptionCyclesCacheKey);
                                    }
                                }
                            }

                            #endregion
                            break;
                        case "invoice.payment_failed":
                            #region invoice.payment_failed

                            if (invoiceList != null && !invoices.Paid)
                            {
                                var subscriptionBooking = subscriptionRepository.SubscriptionBookingsList
                                    .FirstOrDefault(sb => sb.StripeSubscriptionId == subscriptionId);

                                if (subscriptionBooking != null)
                                {
                                    double periodStart = invoiceList.Period.Start,
                                        periodEnd = invoiceList.Period.End;

                                    var currentSubscriptionCycle =
                                    (from sb in subscriptionRepository.SubscriptionBookingsList
                                     join sc in subscriptionRepository.SubscriptionCyclesList on sb.Id equals sc
                                     .SubscriptionBookingId
                                     where sb.StripeSubscriptionId.Equals(subscriptionId,
                                     StringComparison.OrdinalIgnoreCase)
                                     orderby sc.CycleNumber descending
                                     select sc).FirstOrDefault();

                                    var isCurrentCycle = currentSubscriptionCycle != null &&
                                                         currentSubscriptionCycle.StartDate.HasValue &&
                                                         currentSubscriptionCycle.EndDate.HasValue &&
                                                         currentSubscriptionCycle.StartDate.Value.Date ==
                                                         periodStart.ToDateTime().Date &&
                                                         currentSubscriptionCycle.EndDate.Value.Date ==
                                                         periodEnd.ToDateTime().Date;
                                    var param = new StripePaymentFailedInsertOrUpdate();

                                    subscriptionBooking.Status = (int)Enums.SubscriptionBookingStatus.Suspended;
                                    subscriptionBooking.LastUpdatedDate = DateTime.UtcNow;
                                    subscriptionBooking.LastUpdatedBy = 1;

                                    param.CurrentSubscriptionBookings = subscriptionBooking;
                                    if (isCurrentCycle)
                                    {
                                        param.CurrentSubscriptionCycles = currentSubscriptionCycle;
                                    }
                                    else
                                    {
                                        var subscriptionCycle = new SubscriptionCycles
                                        {
                                            SubscriptionBookingId = subscriptionBooking.Id,
                                            StartDate = periodStart.ToDateTime(),
                                            EndDate = periodEnd.ToDateTime(),
                                            CancelDate = null,
                                            Status = subscriptionBooking.Status,
                                            LastUpdatedDate = DateTime.UtcNow,
                                            LastUpdatedBy = 1,
                                            Price = invoiceList.Amount / 100,
                                            MerchantPrice = 50,
                                            PayByCredit = 0,
                                            TotalPrice = invoiceList.Amount / 100,
                                            Quantity = 1,
                                            StripeInvoiceId = invoiceId,
                                            StripeChargeId = string.Empty,
                                            StripeCouponId = string.Empty,
                                            CycleNumber = currentSubscriptionCycle != null ? ++currentSubscriptionCycle.CycleNumber : (short)1
                                        };
                                        param.CurrentSubscriptionCycles = subscriptionCycle;
                                    }

                                    subscriptionRepository.UpdateSubscriptionCycleFailed(param);

                                    using (var bookingRepository = new BookingRepository())
                                    {
                                        bookingRepository.ExpiredSubscriptions();
                                    }

                                    CacheLayer.Clear(CacheKeys.SubscriptionCyclesCacheKey);
                                    CacheLayer.Clear(CacheKeys.SubscriptionBookingsCacheKey);
                                    CacheLayer.Clear(CacheKeys.SubscriptionDiscountsCacheKey);
                                    CacheLayer.Clear(CacheKeys.SubsciptionDiscountUsedCacheKey);
                                }
                            }

                            #endregion
                            break;
                    }
                }
                catch (Exception ex)
                {
                    var logs = new Logs
                    {
                        LogKey = "SupscriptionInvoiceSuccess_Error",
                        UpdatedBy = 1,
                        UpdatedContent = string.Format("{0} {1} {2}", json, ex.Message, ex.StackTrace),
                        UpdatedDate = DateTime.UtcNow
                    };
                    subscriptionRepository.AddLog(logs);
                }
            }
            context.Response.ContentType = "text/plain";
            context.Response.Write("{}");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

    }
}
