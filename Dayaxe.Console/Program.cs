using System;
using System.Collections.Generic;
using System.Linq;
using DayaxeDal;
using DayaxeDal.Parameters;
using DayaxeDal.Repositories;
using Stripe;

namespace Dayaxe.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set Stripe Api Key
            StripeConfiguration.SetApiKey(AppConfiguration.StripeApiKey);

            using (var subscriptionBookingRepository = new SubscriptionBookingRepository())
            {
                var subscriptionBookingList = subscriptionBookingRepository.SubscriptionBookingsList.ToList();

                // Each Subscription Bookings
                subscriptionBookingList.ForEach(subscriptionBookings =>
                {
                    var customerInfos = subscriptionBookingRepository.CustomerInfoList
                        .FirstOrDefault(ci => ci.CustomerId == subscriptionBookings.CustomerId);

                    // Customer Infos
                    if (customerInfos != null)
                    {
                        var subscriptionService = new StripeSubscriptionService();
                        StripeSubscription subscription = subscriptionService.Get(subscriptionBookings.StripeSubscriptionId);
                        DateTime? canceledDate = subscription.CanceledAt;

                        var invoiceService = new StripeInvoiceService();

                        // All Invoices of Customer
                        var invoiceItems = invoiceService.List(new StripeInvoiceListOptions
                            {
                                CustomerId = customerInfos.StripeCustomerId
                            }
                        ).OrderBy(ic => ic.Date).ToList();

                        short cycleNumber = 0;
                        invoiceItems.ForEach(invoice =>
                        {
                            if (invoice.SubscriptionId.Equals(subscriptionBookings.StripeSubscriptionId, StringComparison.InvariantCulture))
                            {
                                cycleNumber++;
                                double totalCharge = (double)invoice.Total / 100;
                                if (!totalCharge.Equals(subscriptionBookings.TotalPrice))
                                {
                                    subscriptionBookings.Price = totalCharge;
                                }

                                DateTime? periodStart = invoice.PeriodStart;
                                DateTime? periodEnd = invoice.PeriodEnd;
                                try
                                {
                                    if (periodStart.Value.Date == periodEnd.Value.Date)
                                    {
                                        periodEnd = periodEnd.Value.AddDays(30);
                                    }
                                    periodStart = invoice.StripeInvoiceLineItems.Data[0].StripePeriod.Start;
                                    periodEnd = invoice.StripeInvoiceLineItems.Data[0].StripePeriod.End;
                                }
                                catch (Exception) { }
                                var subscriptionCycle = new SubscriptionCycles
                                {
                                    SubscriptionBookingId = subscriptionBookings.Id,
                                    StartDate = periodStart,
                                    EndDate = periodEnd,
                                    CancelDate = subscriptionBookings.CancelDate,
                                    Status = subscriptionBookings.Status,
                                    LastUpdatedDate = subscriptionBookings.LastUpdatedDate,
                                    LastUpdatedBy = subscriptionBookings.LastUpdatedBy,
                                    Price = totalCharge.Equals(0) && subscriptionBookings.PayByCredit > 0 ? subscriptionBookings.PayByCredit : totalCharge,
                                    MerchantPrice = subscriptionBookings.MerchantPrice,
                                    PayByCredit = subscriptionBookings.PayByCredit,
                                    TotalPrice = totalCharge,
                                    Quantity = subscriptionBookings.Quantity,
                                    StripeInvoiceId = invoice.Id,
                                    StripeChargeId = invoice.ChargeId,
                                    StripeCouponId = subscriptionBookings.StripeCouponId,
                                    CycleNumber = cycleNumber
                                };

                                var param = new AddSubscriptionCycleParams
                                {
                                    CanceledDate = canceledDate,
                                    SubscriptionCyclesObject = subscriptionCycle,
                                    SubscriptionInvoices = new List<SubscriptionInvoices>()
                                };

                                SubscriptionInvoices subscriptionInvoice;

                                // Paid Charge
                                if (invoice.Paid)
                                {
                                    if (!string.IsNullOrEmpty(invoice.ChargeId))
                                    {
                                        var chargeService = new StripeChargeService();
                                        StripeCharge charge = chargeService.Get(invoice.ChargeId);
                                        subscriptionInvoice = new SubscriptionInvoices
                                        {
                                            SubscriptionCyclesId = subscriptionCycle.Id,
                                            BookingStatus = subscriptionCycle.Status,
                                            Quantity = subscriptionBookings.Quantity,
                                            Price = (double) charge.Amount / 100,
                                            MerchantPrice = subscriptionBookings.MerchantPrice,
                                            PayByCredit = subscriptionBookings.PayByCredit,
                                            TotalPrice = (double) charge.Amount / 100,
                                            InvoiceStatus = (int) Enums.InvoiceStatus.Charge,
                                            StripeChargeId = charge.Id,
                                            ChargeAmount = (double) charge.Amount / 100,
                                            StripeRefundId = string.Empty,
                                            RefundAmount = 0,
                                            RefundCreditAmount = 0,
                                            StripeCouponId = subscriptionBookings.StripeCouponId,
                                            CreatedDate = DateTime.UtcNow,
                                            CreatedBy = 1
                                        };

                                        param.SubscriptionInvoices.Add(subscriptionInvoice);

                                        // Charge with Refunded
                                        if (charge.Refunded || charge.AmountRefunded > 0)
                                        {
                                            var refundList = charge.Refunds;
                                            for (int i = 0; i < refundList.Data.Count; i++)
                                            {
                                                var refundItem = charge.Refunds.Data[i];
                                                var subscriptionInvoiceRefunded = new SubscriptionInvoices
                                                {
                                                    SubscriptionCyclesId = subscriptionCycle.Id,
                                                    BookingStatus = subscriptionCycle.Status,
                                                    Quantity = subscriptionBookings.Quantity,
                                                    Price = (double)charge.Amount / 100 -
                                                            (double)refundItem.Amount / 100,
                                                    MerchantPrice = subscriptionBookings.MerchantPrice,
                                                    PayByCredit = subscriptionBookings.PayByCredit,
                                                    TotalPrice =
                                                        subscriptionBookings.MerchantPrice -
                                                        (double)refundItem.Amount / 100 -
                                                        subscriptionBookings.PayByCredit,
                                                    InvoiceStatus =
                                                        refundItem.Amount < charge.Amount
                                                            ? (int)Enums.InvoiceStatus.PartialRefund
                                                            : (int)Enums.InvoiceStatus.FullRefund,
                                                    StripeChargeId = charge.Id,
                                                    ChargeAmount = (double)charge.Amount / 100,
                                                    StripeRefundId = refundItem.Id,
                                                    RefundAmount = (double)refundItem.Amount / 100,
                                                    RefundCreditAmount = 0,
                                                    StripeCouponId = subscriptionBookings.StripeCouponId,
                                                    CreatedDate = DateTime.UtcNow,
                                                    CreatedBy = 1
                                                };

                                                param.SubscriptionInvoices.Add(subscriptionInvoiceRefunded);

                                                param.SubscriptionCyclesObject.Price = subscriptionInvoiceRefunded.Price;
                                                param.SubscriptionCyclesObject.TotalPrice = subscriptionInvoiceRefunded.TotalPrice;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // Charge but have Coupon and use DayAxe Credit so Amount = 0
                                        subscriptionInvoice = new SubscriptionInvoices
                                        {
                                            SubscriptionCyclesId = subscriptionCycle.Id,
                                            BookingStatus = subscriptionCycle.Status,
                                            Quantity = subscriptionBookings.Quantity,
                                            Price = 0,
                                            MerchantPrice = subscriptionBookings.MerchantPrice,
                                            PayByCredit = subscriptionBookings.PayByCredit,
                                            TotalPrice = 0,
                                            InvoiceStatus = (int)Enums.InvoiceStatus.Charge,
                                            StripeChargeId = string.Empty,
                                            ChargeAmount = 0,
                                            StripeRefundId = string.Empty,
                                            RefundAmount = 0,
                                            RefundCreditAmount = 0,
                                            StripeCouponId = subscriptionBookings.StripeCouponId,
                                            CreatedDate = DateTime.UtcNow,
                                            CreatedBy = 1
                                        };

                                        param.SubscriptionInvoices.Add(subscriptionInvoice);
                                    }
                                }
                                else
                                {
                                    // Closed Charge
                                    subscriptionInvoice = new SubscriptionInvoices
                                    {
                                        SubscriptionCyclesId = subscriptionCycle.Id,
                                        BookingStatus = subscriptionCycle.Status,
                                        Quantity = subscriptionBookings.Quantity,
                                        Price = totalCharge,
                                        MerchantPrice = subscriptionBookings.MerchantPrice,
                                        PayByCredit = subscriptionBookings.PayByCredit,
                                        TotalPrice = totalCharge,
                                        InvoiceStatus = (int)Enums.InvoiceStatus.Charge,
                                        StripeChargeId = invoice.ChargeId,
                                        ChargeAmount = totalCharge,
                                        StripeRefundId = string.Empty,
                                        RefundAmount = 0,
                                        RefundCreditAmount = 0,
                                        StripeCouponId = subscriptionBookings.StripeCouponId,
                                        CreatedDate = DateTime.UtcNow,
                                        CreatedBy = 1
                                    };

                                    param.SubscriptionInvoices.Add(subscriptionInvoice);
                                }

                                subscriptionBookingRepository.AddSubscriptionCycle(param);

                                Console.WriteLine("Update - " + invoice.SubscriptionId);
                            }
                        });
                    }
                });

                Console.WriteLine("Done!!!");
                Console.ReadLine();
            }
        }
    }
}
