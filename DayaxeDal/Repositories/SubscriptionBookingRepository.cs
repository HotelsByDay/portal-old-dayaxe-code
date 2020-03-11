using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using DayaxeDal.Extensions;
using DayaxeDal.Parameters;
using Newtonsoft.Json;
using Stripe;

namespace DayaxeDal.Repositories
{
    public class SubscriptionBookingRepository : BaseRepository, IRepository<SubscriptionBookings>
    {
        public IEnumerable<SubscriptionBookings> Get(Func<SubscriptionBookings, bool> criteria)
        {
            throw new NotImplementedException();
        }

        public int Add(SubscriptionBookings entity)
        {
            throw new NotImplementedException();
        }

        public void Update(SubscriptionBookings entity)
        {
            using (var transaction = new TransactionScope())
            {
                try
                {
                    var subBooking = DayaxeDbContext.SubscriptionBookings.FirstOrDefault(sb => sb.Id == entity.Id);
                    if (subBooking != null)
                    {
                        var subscriptionCycle = DayaxeDbContext.SubscriptionCycles
                            .Where(sc => sc.SubscriptionBookingId == subBooking.Id)
                            .OrderByDescending(sc => sc.CycleNumber)
                            .FirstOrDefault();
                        if (subscriptionCycle != null)
                        {
                            subscriptionCycle.Status = entity.Status;
                            subscriptionCycle.LastUpdatedDate = DateTime.UtcNow;
                        }

                        var subDiscounts = (from sdu in DayaxeDbContext.SubsciptionDiscountUseds
                                            join sb in DayaxeDbContext.SubscriptionBookings on sdu.SubscriptionBookingId equals sb.Id
                                            join d in DayaxeDbContext.Discounts on sdu.DiscountId equals d.Id
                                            where sb.Id == subBooking.Id
                                            select d).First();

                        subBooking.Status = entity.Status;
                        subBooking.CancelDate = entity.CancelDate;
                        switch (subBooking.Status)
                        {
                            case (int)Enums.SubscriptionBookingStatus.Active:
                                // Update Date of Subscription
                                subBooking.StartDate = entity.StartDate;
                                subBooking.EndDate = entity.EndDate;
                                subBooking.LastUpdatedDate = entity.LastUpdatedDate;
                                subBooking.LastUpdatedBy = entity.LastUpdatedBy;

                                // Update End Date of Discounts
                                subDiscounts.EndDate = entity.EndDate;
                                break;
                            case (int)Enums.SubscriptionBookingStatus.End:
                                if (subBooking.Status == (int)Enums.SubscriptionBookingStatus.End)
                                {
                                    var schedules = new Schedules
                                    {
                                        ScheduleSendType = (int)Enums.ScheduleSendType.IsEmailSubscriptionCancellation,
                                        Name = "Send Email Subscription Cancellation Confirmation",
                                        Status = (int)Enums.ScheduleType.NotRun,
                                        SubscriptionBookingId = subBooking.Id
                                    };
                                    DayaxeDbContext.Schedules.InsertOnSubmit(schedules);
                                }
                                break;
                            case (int)Enums.SubscriptionBookingStatus.Suspended:
                                break;
                        }

                        var logs = new Logs
                        {
                            LogKey = "UpdateSubscription",
                            UpdatedContent = string.Format("{0} - Response: {1}", entity.CustomerId, entity.Description),
                            UpdatedBy = entity.CustomerId,
                            UpdatedDate = DateTime.UtcNow
                        };
                        AddLog(logs);
                    }
                }
                catch (Exception ex)
                {
                    var logs = new Logs
                    {
                        LogKey = "UpdateSubscriptionError",
                        UpdatedContent = string.Format("{0} - {1} - {2} - {3}", entity.CustomerId, ex.Message, ex.StackTrace, ex.Source),
                        UpdatedBy = entity.CustomerId,
                        UpdatedDate = DateTime.UtcNow
                    };
                    AddLog(logs);
                }

                transaction.Complete();
            }
        }

        public void Delete(SubscriptionBookings entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Func<SubscriptionBookings, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public SubscriptionBookings GetById(long id)
        {
            return GetAll().FirstOrDefault(h => h.Id == id);
        }

        public IEnumerable<SubscriptionBookings> GetAll()
        {
            var entities = CacheLayer.Get<List<SubscriptionBookings>>(CacheKeys.SubscriptionBookingsCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.SubscriptionBookings.ToList();
            CacheLayer.Add(entities, CacheKeys.SubscriptionBookingsCacheKey);
            return entities.AsEnumerable();
        }

        public SubscriptionBookings Refresh(SubscriptionBookings entity)
        {
            throw new NotImplementedException();
        }

        #region Custom

        public int Add(AddSubscriptionBookingParams param)
        {
            try
            {
                using (var transaction = new TransactionScope())
                {
                    var bookingCode = Helper.RandomString(Constant.BookingCodeLength);
                    while (IsBookingCodeExists(bookingCode))
                    {
                        bookingCode = Helper.RandomString(Constant.BookingCodeLength);
                    }
                    param.SubscriptionBookingsObject.BookingCode = bookingCode;

                    // Insert New Bookings
                    DayaxeDbContext.SubscriptionBookings.InsertOnSubmit(param.SubscriptionBookingsObject);
                    Commit();

                    if (param.SubscriptionBookingDiscounts != null && param.SubscriptionBookingDiscounts.Id > 0)
                    {
                        var subscriptionDiscount = new SubscriptionBookingDiscounts
                        {
                            DiscountId = param.SubscriptionBookingDiscounts.Id,
                            SubscriptionBookingId = param.SubscriptionBookingsObject.Id,
                            SubscriptionId = param.SubscriptionBookingsObject.SubscriptionId
                        };

                        DayaxeDbContext.SubscriptionBookingDiscounts.InsertOnSubmit(subscriptionDiscount);
                    }

                    // Insert to History for First Cycle
                    var cycle = new SubscriptionCycles
                    {
                        SubscriptionBookingId = param.SubscriptionBookingsObject.Id,
                        StartDate = param.SubscriptionBookingsObject.StartDate,
                        EndDate = param.SubscriptionBookingsObject.EndDate,
                        CancelDate = param.SubscriptionBookingsObject.CancelDate,
                        LastUpdatedBy = param.SubscriptionBookingsObject.LastUpdatedBy,
                        LastUpdatedDate = param.SubscriptionBookingsObject.LastUpdatedDate,
                        Status = param.SubscriptionBookingsObject.Status,
                        Price = param.ActualPrice,
                        MerchantPrice = param.MerchantPrice,
                        PayByCredit = param.PayByCredit,
                        TotalPrice = param.TotalPrice,
                        Quantity = param.SubscriptionBookingsObject.Quantity,
                        StripeChargeId = string.Empty,
                        StripeCouponId = param.SubscriptionBookingsObject.StripeCouponId,
                        StripeInvoiceId = string.Empty,
                        CycleNumber = 1
                    };

                    DayaxeDbContext.SubscriptionCycles.InsertOnSubmit(cycle);

                    // Insert Discount for Current Customer active Subscription
                    var discount = new Discounts
                    {
                        DiscountName = string.Format("{0} - {1} {2}", 
                            param.SubscriptionName, 
                            param.FirstName,
                            param.LastName),
                        Code = string.Format("SUP{0}", Helper.RandomString(8)),
                        StartDate = param.SubscriptionBookingsObject.StartDate,
                        EndDate = param.SubscriptionBookingsObject.EndDate,
                        CodeRequired = true,
                        PercentOff = 100,
                        PromoType = (byte) Enums.PromoType.SubscriptionPromo,
                        MinAmount = 0,
                        IsAllProducts = true,
                        MaxPurchases = (byte)param.MaxPurchases
                    };
                    DayaxeDbContext.Discounts.InsertOnSubmit(discount);
                    Commit();

                    // Add Invoices
                    var subscriptionInvoice = new SubscriptionInvoices
                    {
                        SubscriptionCyclesId = cycle.Id,
                        BookingStatus = cycle.Status,
                        Quantity = cycle.Quantity,
                        Price = cycle.Price,
                        MerchantPrice = cycle.MerchantPrice,
                        PayByCredit = cycle.PayByCredit,
                        TotalPrice = cycle.TotalPrice,
                        InvoiceStatus = (int)Enums.InvoiceStatus.Charge,
                        StripeChargeId = cycle.StripeChargeId,
                        ChargeAmount = cycle.Price,
                        StripeRefundId = string.Empty,
                        RefundAmount = 0,
                        RefundCreditAmount = 0,
                        StripeCouponId = cycle.StripeCouponId,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = 1
                    };
                    DayaxeDbContext.SubscriptionInvoices.InsertOnSubmit(subscriptionInvoice);

                    var discountUsed = new SubsciptionDiscountUseds
                    {
                        CustomerId = param.SubscriptionBookingsObject.CustomerId,
                        DiscountId = discount.Id,
                        SubscriptionBookingId = param.SubscriptionBookingsObject.Id
                    };
                    DayaxeDbContext.SubsciptionDiscountUseds.InsertOnSubmit(discountUsed);

                    var cusCredits = DayaxeDbContext.CustomerCredits
                        .SingleOrDefault(cc => cc.CustomerId == param.CustomerCreditsObject.CustomerId);

                    // Add Logs when refund by Upgrade to Subscription
                    if (param.RefundCreditByUpgrade > 0)
                    {
                        var creditLogs = new CustomerCreditLogs
                        {
                            Amount = param.RefundCreditByUpgrade,
                            ProductId = 0,
                            BookingId = 0,
                            SubscriptionId = param.SubscriptionBookingsObject.SubscriptionId,
                            CreatedBy = param.CustomerCreditsObject.CustomerId,
                            CreatedDate = DateTime.UtcNow,
                            CreditType = (byte)Enums.CreditType.PartialPuchaseRefund,
                            Description = param.RefundCreditDescription,
                            CustomerId = param.CustomerCreditsObject.CustomerId,
                            ReferralId = param.CustomerCreditsObject.ReferralCustomerId,
                            SubscriptionBookingId = param.SubscriptionBookingsObject.Id,
                            Status = true,
                            GiftCardId = 0
                        };

                        DayaxeDbContext.CustomerCreditLogs.InsertOnSubmit(creditLogs);

                        if (cusCredits != null)
                        {
                            cusCredits.Amount += param.RefundCreditByUpgrade;
                        }
                    }

                    // Add Logs when pay by DayAxe Credit
                    if (param.PayByCredit > 0)
                    {
                        var creditLogs = new CustomerCreditLogs
                        {
                            Amount = param.PayByCredit,
                            ProductId = 0,
                            BookingId = 0,
                            SubscriptionId = param.SubscriptionBookingsObject.SubscriptionId,
                            CreatedBy = param.CustomerCreditsObject.CustomerId,
                            CreatedDate = DateTime.UtcNow,
                            CreditType = (byte)Enums.CreditType.Charge,
                            Description = param.Description,
                            CustomerId = param.CustomerCreditsObject.CustomerId,
                            ReferralId = param.CustomerCreditsObject.ReferralCustomerId,
                            SubscriptionBookingId = param.SubscriptionBookingsObject.Id,
                            Status = true,
                            GiftCardId = 0
                        };

                        DayaxeDbContext.CustomerCreditLogs.InsertOnSubmit(creditLogs);

                        if (cusCredits != null)
                        {
                            cusCredits.Amount -= param.PayByCredit;
                        }
                    }

                    // First Buy of referral
                    if (param.CustomerCreditsObject != null && param.CustomerCreditsObject.ReferralCustomerId > 0 && (
                            DayaxeDbContext.Bookings.Count(x => x.CustomerId == param.SubscriptionBookingsObject.CustomerId) == 1 ||
                            DayaxeDbContext.SubscriptionBookings.Count(x => x.CustomerId == param.SubscriptionBookingsObject.CustomerId) == 1))
                    {
                        var logs = DayaxeDbContext.CustomerCreditLogs
                            .Where(ccl => ccl.CustomerId == param.CustomerCreditsObject.ReferralCustomerId && 
                                ccl.ReferralId == param.SubscriptionBookingsObject.CustomerId && 
                                !ccl.Status)
                            .ToList();

                        if (logs.Any())
                        {
                            logs.ForEach(log =>
                            {
                                var cus = DayaxeDbContext.CustomerCredits
                                    .FirstOrDefault(cc => cc.CustomerId == log.CustomerId);
                                if (cus != null)
                                {
                                    cus.Amount += log.Amount;
                                }

                                log.Status = true;
                            });
                            Commit();
                        }
                    }

                    // Add to Customer Credit Log
                    var logsReferred = DayaxeDbContext.CustomerCreditLogs
                        .Where(ccl => ccl.ReferralId == param.SubscriptionBookingsObject.CustomerId && !ccl.Status)
                        .ToList();
                    if (logsReferred.Any())
                    {
                        logsReferred.ForEach(log =>
                        {
                            var cus = DayaxeDbContext.CustomerCredits
                                .FirstOrDefault(cc => cc.CustomerId == log.CustomerId);
                            if (cus != null)
                            {
                                cus.Amount += log.Amount;
                            }

                            log.Status = true;
                        });
                    }

                    // Insert Schedule
                    var schedules = new Schedules
                    {
                        ScheduleSendType = (int)Enums.ScheduleSendType.IsEmailConfirmSubscription,
                        Name = "Send Email confirm Subscription",
                        Status = (int)Enums.ScheduleType.NotRun,
                        SubscriptionBookingId = param.SubscriptionBookingsObject.Id
                    };
                    DayaxeDbContext.Schedules.InsertOnSubmit(schedules);

                    // Maybe not use here because flow upgrade to subscription do not use this 
                    if (param.BookingId > 0)
                    {
                        var bookings = DayaxeDbContext.Bookings.FirstOrDefault(b => b.BookingId == param.BookingId);
                        if (bookings != null)
                        {
                            var chargePrice = (int)((bookings.TotalPrice - bookings.HotelPrice) * 100);
                            try
                            {
                                bookings.TotalPrice -= bookings.HotelPrice;
                                bookings.PassStatus = (int)Enums.BookingStatus.Active;

                                var discounts = new DiscountBookings
                                {
                                    DiscountId = discount.Id,
                                    ProductId = bookings.ProductId,
                                    BookingId = bookings.BookingId
                                };

                                DayaxeDbContext.DiscountBookings.InsertOnSubmit(discounts);
                                bookings.IsActiveSubscription = true;

                                string responseString;
                                if (chargePrice > 0)
                                {
                                    var chargeService = new StripeChargeService();
                                    StripeCharge charge = chargeService.Capture(bookings.StripeChargeId, chargePrice);
                                    responseString = charge.StripeResponse.ResponseJson;
                                }
                                else
                                {
                                    var refundOptions = new StripeRefundCreateOptions
                                    {
                                        Reason = StripeRefundReasons.RequestedByCustomer
                                    };
                                    var refundService = new StripeRefundService();
                                    StripeRefund refund = refundService.Create(bookings.StripeChargeId, refundOptions);
                                    responseString = refund.StripeResponse.ResponseJson;
                                }

                                var logs = new Logs
                                {
                                    LogKey = "UpgradeSubscriptionCharge",
                                    UpdatedContent = string.Format("Params: {0} - Response: {1}",
                                        JsonConvert.SerializeObject(param, CustomSettings.SerializerSettings()),
                                        responseString),
                                    UpdatedBy = param.SubscriptionBookingsObject.CustomerId,
                                    UpdatedDate = DateTime.UtcNow
                                };
                                AddLog(logs);
                            }
                            catch (Exception ex)
                            {
                                var logs = new Logs
                                {
                                    LogKey = "UpgradeSubscriptionChargeError",
                                    UpdatedContent = string.Format("Params: {0} - {1} - {2} - {3}",
                                        JsonConvert.SerializeObject(param, CustomSettings.SerializerSettings()), 
                                        ex.Message, 
                                        ex.StackTrace, 
                                        ex.Source),
                                    UpdatedBy = param.SubscriptionBookingsObject.CustomerId,
                                    UpdatedDate = DateTime.UtcNow
                                };
                                AddLog(logs);
                            }
                        }
                    }

                    Commit();

                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                var logs = new Logs
                {
                    LogKey = "AddBookingError",
                    UpdatedContent = string.Format("{0} - {1} - {2} - {3}", param.SubscriptionBookingsObject.CustomerId, ex.Message, ex.StackTrace, ex.Source),
                    UpdatedBy = param.SubscriptionBookingsObject.CustomerId,
                    UpdatedDate = DateTime.UtcNow
                };
                AddLog(logs);

                throw new Exception(ex.Message, ex);
            }

            return param.SubscriptionBookingsObject.Id;
        }

        public SubscriptionBookings GetBookingInLast3Minutes(int subscriptionId, string emailAddress)
        {
            var booking = (from sb in SubscriptionBookingsList
                join ci in CustomerInfoList on sb.CustomerId equals ci.CustomerId
                join sd in SubscriptionDiscountUsedList on ci.CustomerId equals sd.CustomerId
                join d in DiscountOfSubscriptionList on sd.DiscountId equals d.Id
                where sb.SubscriptionId == subscriptionId
                      && ci.EmailAddress == emailAddress.Trim()
                      && d.Status == Enums.DiscountStatus.Active
                select sb);
            return booking.FirstOrDefault();
        }

        private bool IsBookingCodeExists(string bookingCode)
        {
            var booking = SubscriptionBookingsList.FirstOrDefault(x => x.BookingCode == bookingCode.Trim().Replace(" ", ""));
            if (booking != null)
            {
                return true;
            }
            return false;
        }

        public IEnumerable<Discounts> GetAutoPromosBySubscriptionId(int subscriptionId)
        {
            var dateNow = DateTime.UtcNow.ToLosAngerlesTime();
            // x.IsAllProducts && 
            //var discounts = DiscountList.Where(d => 
            //    d.Status == Enums.DiscountStatus.Active && 
            //    !d.CodeRequired && 
            //    !d.IsDelete);

            var discountsProducts = (from p in DiscountList
                join p1 in DiscountSubscriptionList on p.Id equals p1.DiscountId
                where !p.IsDelete
                      && !p.CodeRequired
                      && p1.SubscriptionId == subscriptionId
                      && p.StartDate.HasValue
                      && p.EndDate.HasValue
                      && p.StartDate.Value.Date <= dateNow
                      && p.EndDate.Value.Date >= dateNow
                select p);

            //return discounts.Concat(discountsProducts).DistinctBy(x => x.Id);
            return discountsProducts.DistinctBy(x => x.Id);
        }

        public SubscriptionBookings GetByCustomerId(int customerId, int discountId)
        {
            return (from sb in SubscriptionBookingsList
                    join sdu in SubscriptionDiscountUsedList on sb.Id equals sdu.SubscriptionBookingId
                    join sd in DiscountOfSubscriptionList on sdu.DiscountId equals sd.Id
                    where sdu.CustomerId == customerId && sdu.DiscountId == discountId && sd.Status == Enums.DiscountStatus.Active
                    select sb).FirstOrDefault();
        }
        public SubscriptionBookings GetSuspendedByCustomerId(int customerId)
        {
            return SubscriptionBookingsList.FirstOrDefault(sb =>
                sb.Status == (int)Enums.SubscriptionBookingStatus.Suspended && sb.CustomerId == customerId);
        }

        public bool CanApplySubscriptionPromo(int customerId)
        {
            return (from sdb in SubscriptionBookingDiscountsList
                    join d in DiscountList on sdb.DiscountId equals d.Id
                    join sb in SubscriptionBookingsList on sdb.SubscriptionBookingId equals sb.Id
                    where sb.CustomerId == customerId && d.Code == "MFREESB18"
                    select sdb).FirstOrDefault() == null;
        }

        #endregion

        #region Migrate 

        public int AddSubscriptionCycle(SubscriptionCycles entity)
        {
            DayaxeDbContext.SubscriptionCycles.InsertOnSubmit(entity);
            Commit();
            return entity.Id;
        }

        public void AddSubscriptionCycle(AddSubscriptionCycleParams param)
        {
            using (var transaction = new TransactionScope())
            {
                var subscriptionBookings = DayaxeDbContext.SubscriptionBookings
                    .FirstOrDefault(sb => sb.Id == param.SubscriptionCyclesObject.SubscriptionBookingId);

                if (subscriptionBookings != null)
                {
                    subscriptionBookings.CancelDate = param.CanceledDate;
                }

                DayaxeDbContext.SubscriptionCycles.InsertOnSubmit(param.SubscriptionCyclesObject);
                Commit();

                if (param.SubscriptionInvoices.Any())
                {
                    param.SubscriptionInvoices.ForEach(subscriptionInvoice =>
                    {
                        subscriptionInvoice.SubscriptionCyclesId = param.SubscriptionCyclesObject.Id;
                    });
                }

                DayaxeDbContext.SubscriptionInvoices.InsertAllOnSubmit(param.SubscriptionInvoices);

                Commit();
                transaction.Complete();
            }
        }
        public void UpdateSubscriptionBookingCanceledDate(List<SubscriptionBookings> entities)
        {
            using (var transaction = new TransactionScope())
            {
                entities.ForEach(entity =>
                {
                    var subscriptionBookings = DayaxeDbContext.SubscriptionBookings
                        .FirstOrDefault(sb => sb.Id == entity.Id);
                    if (subscriptionBookings != null)
                    {
                        subscriptionBookings.CancelDate = entity.CancelDate;

                        var subscriptionCycle = DayaxeDbContext.SubscriptionCycles
                            .Where(sc => sc.SubscriptionBookingId == subscriptionBookings.Id)
                            .OrderByDescending(sc => sc.CycleNumber)
                            .FirstOrDefault();
                        if (subscriptionCycle != null)
                        {
                            subscriptionCycle.Status = entity.Status;
                            subscriptionCycle.LastUpdatedDate = DateTime.UtcNow;

                            switch (entity.Status)
                            {
                                case (int)Enums.SubscriptionBookingStatus.Suspended:
                                    var discount = (from d in DayaxeDbContext.Discounts
                                                    join sdu in DayaxeDbContext.SubsciptionDiscountUseds on d.Id equals sdu
                                                        .DiscountId
                                                    where sdu.SubscriptionBookingId == subscriptionBookings.Id
                                                    select d).FirstOrDefault();
                                    if (discount != null)
                                    {
                                        discount.EndDate = DateTime.UtcNow.AddMinutes(-5);
                                    }
                                    break;
                            }
                        }

                        subscriptionBookings.Status = entity.Status;
                    }
                });

                Commit();
                transaction.Complete();
            }
        }

        #endregion
    }
}
