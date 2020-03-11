using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Script.Serialization;
using DayaxeDal.Custom;
using DayaxeDal.Extensions;
using DayaxeDal.Parameters;
using Stripe;

namespace DayaxeDal.Repositories
{
    public class BookingRepository : BaseRepository, IRepository<Bookings>
    {
        public IEnumerable<Bookings> Get(Func<Bookings, bool> criteria)
        {
            return GetAll().Where(criteria);
        }

        public int Add(Bookings entity)
        {
            DayaxeDbContext.Bookings.InsertOnSubmit(entity);
            Commit();

            return entity.BookingId;
        }

        public int Add(AddBookingParams param, bool sendEmail = true)
        {
            try
            {
                using (var transaction = new TransactionScope())
                {
                    // Insert New Bookings
                    DayaxeDbContext.Bookings.InsertOnSubmit(param.BookingObject);
                    Commit();

                    var cusCredits = DayaxeDbContext.CustomerCredits
                        .SingleOrDefault(cc => cc.CustomerId == param.CustomerCreditObject.CustomerId);

                    // Add Logs when pay by DayAxe Credit
                    if (param.BookingObject.PayByCredit > 0)
                    {
                        var creditLogs = new CustomerCreditLogs
                        {
                            Amount = param.BookingObject.PayByCredit,
                            ProductId = param.BookingObject.ProductId,
                            CreatedBy = param.CustomerCreditObject.CustomerId,
                            CreatedDate = DateTime.UtcNow,
                            CreditType = (byte)Enums.CreditType.Charge,
                            Description = param.Description,
                            CustomerId = param.CustomerCreditObject.CustomerId,
                            ReferralId = param.CustomerCreditObject.ReferralCustomerId,
                            BookingId = param.BookingObject.BookingId,
                            Status = true,
                            GiftCardId = 0
                        };

                        DayaxeDbContext.CustomerCreditLogs.InsertOnSubmit(creditLogs);

                        if (cusCredits != null)
                        {
                            cusCredits.Amount -= param.BookingObject.PayByCredit;
                        }
                    }

                    // First Buy of referral
                    if (param.CustomerCreditObject.ReferralCustomerId > 0 && (
                            DayaxeDbContext.Bookings.Count(x => x.CustomerId == param.BookingObject.CustomerId) == 1 ||
                            DayaxeDbContext.SubscriptionBookings.Count(x => x.CustomerId == param.BookingObject.CustomerId) == 1))
                    {
                        var logs = DayaxeDbContext.CustomerCreditLogs
                            .Where(ccl => ccl.CustomerId == param.CustomerCreditObject.ReferralCustomerId && ccl.ReferralId == param.BookingObject.CustomerId && !ccl.Status)
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

                    //
                    var logsReferred = DayaxeDbContext.CustomerCreditLogs
                        .Where(ccl => ccl.ReferralId == param.BookingObject.CustomerId && !ccl.Status)
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

                    if (param.SubscriptionDiscountId > 0)
                    {
                        var discounts = new DiscountBookings
                        {
                            DiscountId = param.SubscriptionDiscountId,
                            ProductId = param.BookingObject.ProductId,
                            BookingId = param.BookingObject.BookingId
                        };
                        param.BookingObject.DiscountBookings.Add(discounts);
                    }

                    // Discount Of Bookings
                    if (param.DiscountId > 0)
                    {
                        var discounts = new DiscountBookings
                        {
                            DiscountId = param.DiscountId,
                            ProductId = param.BookingObject.ProductId,
                            BookingId = param.BookingObject.BookingId
                        };
                        param.BookingObject.DiscountBookings.Add(discounts);
                    }

                    if (sendEmail)
                    {
                        // Insert Schedule
                        var schedules = new Schedules
                        {
                            ScheduleSendType = (int)Enums.ScheduleSendType.IsMailConfirm,
                            Name = "Send Email confirm",
                            Status = (int)Enums.ScheduleType.NotRun,
                            BookingId = param.BookingObject.BookingId
                        };
                        DayaxeDbContext.Schedules.InsertOnSubmit(schedules);
                    }

                    // Insert Survey
                    var code = Helper.RandomString(20).ToLower();
                    while (IsSurveyCodeExists(code))
                    {
                        code = Helper.RandomString(20).ToLower();
                    }

                    var surveys = new Surveys
                    {
                        BookingId = param.BookingObject.BookingId,
                        Code = code,
                        IsFinish = false,
                        LastStep = 0
                    };
                    DayaxeDbContext.Surveys.InsertOnSubmit(surveys);

                    if (sendEmail)
                    {
                        // Insert Schedule Send Add On Notification
                        var schedulesAddOn = new Schedules
                        {
                            ScheduleSendType = (int)Enums.ScheduleSendType.IsAddOnNotification,
                            Name = "Send Add-On Notification",
                            Status = (int)Enums.ScheduleType.NotRun,
                            BookingId = param.BookingObject.BookingId
                        };
                        DayaxeDbContext.Schedules.InsertOnSubmit(schedulesAddOn);
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
                    UpdatedContent = string.Format("{0} - {1} - {2} - {3}", param.BookingObject.CustomerId, ex.Message, ex.StackTrace, ex.Source),
                    UpdatedBy = param.BookingObject.CustomerId,
                    UpdatedDate = DateTime.UtcNow
                };
                AddLog(logs);

                throw new Exception(ex.Message, ex);
            }

            return param.BookingObject.BookingId;
        }

        public void Update(Bookings bookings)
        {
            using (var transaction = new TransactionScope())
            {
                var bookingUpdate = DayaxeDbContext.Bookings.FirstOrDefault(x => x.BookingId == bookings.BookingId);
                if (bookingUpdate != null)
                {
                    var discount = (from d in DayaxeDbContext.Discounts
                                    join db in DayaxeDbContext.DiscountBookings on d.Id equals db.DiscountId
                                    join b in DayaxeDbContext.Bookings on db.BookingId equals b.BookingId
                                    where b.BookingId == bookings.BookingId &&
                                          d.PromoType != (int)Enums.PromoType.SubscriptionPromo
                                    select d).FirstOrDefault();

                    var bookingHistories = new BookingHistories
                    {
                        BookingId = bookingUpdate.BookingId,
                        CheckinDate = bookingUpdate.CheckinDate,
                        PassStatus = bookingUpdate.PassStatus,
                        ExpiredDate = bookingUpdate.ExpiredDate,
                        RedeemedDate = bookingUpdate.RedeemedDate,
                        UpdatedDate = DateTime.UtcNow,
                        UpdatedBy = bookings.UpdatedBy,
                        DiscountCode = discount != null ? discount.Code : string.Empty,
                        DiscountId = discount != null ? discount.Id : 0,
                        HotelPrice = bookingUpdate.HotelPrice,
                        MerchantPrice = bookingUpdate.MerchantPrice,
                        TotalPrice = bookingUpdate.TotalPrice,
                        Quantity = bookingUpdate.Quantity,
                        PaymentType = bookings.PaymentType,
                        PayByCredit = bookings.PayByCredit
                    };
                    DayaxeDbContext.BookingHistories.InsertOnSubmit(bookingHistories);

                    Commit();

                    if (!bookings.IsMaintainInvoices)
                    {
                        // Insert Schedule Expired Bookings
                        var schedules = new Schedules
                        {
                            ScheduleSendType = (int)Enums.ScheduleSendType.IsMailConfirm,
                            Name = "Send Email confirm - Updated",
                            Status = (int)Enums.ScheduleType.NotRun,
                            BookingId = bookings.BookingId,
                            IsUpdated = true,
                            BookingHistoryId = bookingHistories.Id
                        };
                        DayaxeDbContext.Schedules.InsertOnSubmit(schedules);
                    }

                    bookingUpdate.PassStatus = bookings.PassStatus;
                    bookingUpdate.CheckinDate = bookings.CheckinDate;
                    bookingUpdate.ExpiredDate = bookings.ExpiredDate;
                    bookingUpdate.RedeemedDate = bookings.RedeemedDate;
                    bookingUpdate.CancelDated = bookings.CancelDated;

                    bookingUpdate.StripeChargeId = bookings.StripeChargeId;

                    bookingUpdate.StripeRefundStatus = bookings.StripeRefundStatus;
                    bookingUpdate.StripeRefundAmount = bookings.StripeRefundAmount;
                    bookingUpdate.StripeRefundTransactionId = bookings.StripeRefundTransactionId;

                    bookingUpdate.HasInvoice = bookings.HasInvoice;

                    bookingUpdate.IsEmailConfirmSend = bookings.IsEmailConfirmSend;

                    bookingUpdate.TotalPrice = bookings.TotalPrice;
                    bookingUpdate.HotelPrice = bookings.HotelPrice;
                    bookingUpdate.MerchantPrice = bookings.MerchantPrice;

                    bookingUpdate.IsActiveSubscription = bookings.IsActiveSubscription;

                    Commit();
                }

                transaction.Complete();
            }
        }

        public void UpdateConfirmSend(Bookings bookings)
        {
            using (var transaction = new TransactionScope())
            {
                var bookingUpdate = DayaxeDbContext.Bookings.FirstOrDefault(x => x.BookingId == bookings.BookingId);
                if (bookingUpdate != null)
                {
                    bookingUpdate.PassStatus = bookings.PassStatus;
                    bookingUpdate.IsEmailConfirmSend = bookings.IsEmailConfirmSend;
                    bookingUpdate.IsActiveSubscription = bookings.IsActiveSubscription;

                    Commit();
                }

                transaction.Complete();
            }
        }

        public void Update(Bookings bookings, int discountId, CustomerCreditLogs logs)
        {
            using (var transaction = new TransactionScope())
            {
                var bookingUpdate = DayaxeDbContext.Bookings.SingleOrDefault(x => x.BookingId == bookings.BookingId);
                if (bookingUpdate != null)
                {
                    var discountUsed = GetDiscountUsedByBookingId(bookingUpdate.BookingId);

                    var bookingHistories = new BookingHistories
                    {
                        BookingId = bookingUpdate.BookingId,
                        CheckinDate = bookingUpdate.CheckinDate,
                        PassStatus = bookingUpdate.PassStatus,
                        ExpiredDate = bookingUpdate.ExpiredDate,
                        RedeemedDate = bookingUpdate.RedeemedDate,
                        UpdatedDate = DateTime.UtcNow,
                        UpdatedBy = bookings.UpdatedBy,
                        DiscountCode = discountUsed != null ? discountUsed.Code : string.Empty,
                        DiscountId = discountUsed != null ? discountUsed.Id : 0,
                        HotelPrice = bookingUpdate.HotelPrice,
                        MerchantPrice = bookingUpdate.MerchantPrice,
                        TotalPrice = bookingUpdate.TotalPrice,
                        Quantity = bookingUpdate.Quantity,
                        StripeCardString = bookings.StripeCardString,
                        PaymentType = bookings.PaymentType,
                        PayByCredit = bookings.PayByCredit
                    };
                    DayaxeDbContext.BookingHistories.InsertOnSubmit(bookingHistories);

                    Commit();

                    var discountBookings = (from db in DayaxeDbContext.DiscountBookings
                                            join d in DayaxeDbContext.Discounts on db.DiscountId equals d.Id
                                            join b in DayaxeDbContext.Bookings on db.BookingId equals b.BookingId
                                            where db.BookingId == bookingUpdate.BookingId &&
                                                  d.PromoType != (byte)Enums.PromoType.SubscriptionPromo
                                            select db);
                    DayaxeDbContext.DiscountBookings.DeleteAllOnSubmit(discountBookings);

                    // Discount Of Bookings
                    if (discountId > 0)
                    {
                        var discounts = new DiscountBookings
                        {
                            DiscountId = discountId,
                            ProductId = bookingUpdate.ProductId,
                            BookingId = bookingUpdate.BookingId
                        };
                        bookingUpdate.DiscountBookings.Add(discounts);
                    }

                    // Insert to Customer Credit if Refund to DayAxe Credit
                    if (logs != null && logs.CustomerId > 0)
                    {
                        var customerCredit = DayaxeDbContext.CustomerCredits
                            .FirstOrDefault(ci => ci.CustomerId == logs.CustomerId);
                        if (customerCredit != null)
                        {
                            switch (logs.CreditType)
                            {
                                case (int)Enums.CreditType.PartialPuchaseRefund:
                                case (int)Enums.CreditType.FullPurchaseRefund:
                                    customerCredit.Amount += logs.Amount;
                                    break;
                                //case (int)Enums.CreditType.Referral:
                                //    break;
                                //case (int)Enums.CreditType.GiftCard:
                                //    break;
                                case (int)Enums.CreditType.Charge:
                                    customerCredit.Amount -= logs.Amount;
                                    break;
                            }
                            customerCredit.LastUpdatedDate = DateTime.UtcNow;

                            DayaxeDbContext.CustomerCreditLogs.InsertOnSubmit(logs);

                            //// Insert Schedule
                            //var schedules = new Schedules
                            //{
                            //    ScheduleSendType = (int)Enums.ScheduleSendType.IsGuestBookingChange,
                            //    Name = "Send Email Guest Booking Change",
                            //    Status = (int)Enums.ScheduleType.NotRun,
                            //    BookingId = bookingUpdate.BookingId,
                            //    BookingHistoryId = bookingHistories.Id
                            //};
                            //DayaxeDbContext.Schedules.InsertOnSubmit(schedules);

                            //// Insert Schedule
                            //var hotelSchedules = new Schedules
                            //{
                            //    ScheduleSendType = (int)Enums.ScheduleSendType.IsHotelAlertBookingChange,
                            //    Name = "Send Email Hotel Booking Change",
                            //    Status = (int)Enums.ScheduleType.NotRun,
                            //    BookingId = bookingUpdate.BookingId,
                            //    BookingHistoryId = bookingHistories.Id
                            //};
                            //DayaxeDbContext.Schedules.InsertOnSubmit(hotelSchedules);
                        }
                        else
                        {
                            throw new Exception("Can not Refund with current Customer.");
                        }
                    }

                    // Refund
                    if (bookingHistories.TotalPrice > bookings.TotalPrice)
                    {
                        var schedulesRefund = new Schedules
                        {
                            BookingId = bookings.BookingId,
                            Name = "Refund - updated",
                            Status = (int)Enums.ScheduleType.NotRun,
                            ScheduleSendType = (int)Enums.ScheduleSendType.IsEmailRefund,
                            BookingHistoryId = bookingHistories.Id,
                            CustomerCreditId = logs != null ? logs.Id : 0
                        };
                        DayaxeDbContext.Schedules.InsertOnSubmit(schedulesRefund);
                    }

                    // Insert Schedule
                    var schedules = new Schedules
                    {
                        ScheduleSendType = (int)Enums.ScheduleSendType.IsMailConfirm,
                        Name = "Send Email confirm - Updated",
                        Status = (int)Enums.ScheduleType.NotRun,
                        BookingId = bookings.BookingId,
                        IsUpdated = true,
                        BookingHistoryId = bookingHistories.Id
                    };
                    DayaxeDbContext.Schedules.InsertOnSubmit(schedules);

                    bookingUpdate.PassStatus = bookings.PassStatus;
                    bookingUpdate.CheckinDate = bookings.CheckinDate;
                    bookingUpdate.ExpiredDate = bookings.ExpiredDate;
                    bookingUpdate.RedeemedDate = bookings.RedeemedDate;
                    bookingUpdate.CancelDated = bookings.CancelDated;

                    bookingUpdate.StripeChargeId = bookings.StripeChargeId;

                    bookingUpdate.StripeRefundStatus = bookings.StripeRefundStatus;
                    bookingUpdate.StripeRefundAmount = bookings.StripeRefundAmount;
                    bookingUpdate.StripeRefundTransactionId = bookings.StripeRefundTransactionId;

                    bookingUpdate.HasInvoice = bookings.HasInvoice;

                    bookingUpdate.IsEmailConfirmSend = bookings.IsEmailConfirmSend;

                    bookingUpdate.TotalPrice = bookings.TotalPrice;
                    bookingUpdate.HotelPrice = bookings.HotelPrice;
                    bookingUpdate.MerchantPrice = bookings.MerchantPrice;

                    bookingUpdate.Quantity = bookings.Quantity;

                    Commit();
                }

                transaction.Complete();
            }
        }

        public void RefundBooking(Bookings bookings, CustomerCreditLogs logs)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                var bookingUpdate = DayaxeDbContext.Bookings.SingleOrDefault(x => x.BookingId == bookings.BookingId);
                if (bookingUpdate != null)
                {
                    bookingUpdate.StripeRefundAmount = bookings.StripeRefundAmount;
                    bookingUpdate.RefundCreditAmount = bookings.RefundCreditAmount;
                    bookingUpdate.TotalRefundAmount = bookings.TotalRefundAmount;

                    var discountUsed = GetDiscountUsedByBookingId(bookingUpdate.BookingId);

                    var bookingHistories = new BookingHistories
                    {
                        BookingId = bookingUpdate.BookingId,
                        CheckinDate = bookingUpdate.CheckinDate,
                        PassStatus = bookingUpdate.PassStatus,
                        ExpiredDate = bookingUpdate.ExpiredDate,
                        RedeemedDate = bookingUpdate.RedeemedDate,
                        UpdatedDate = DateTime.UtcNow,
                        UpdatedBy = bookings.UpdatedBy,
                        DiscountCode = discountUsed != null ? discountUsed.Code : string.Empty,
                        DiscountId = discountUsed != null ? discountUsed.Id : 0,
                        HotelPrice = bookingUpdate.HotelPrice,
                        MerchantPrice = bookingUpdate.MerchantPrice,
                        TotalPrice = bookingUpdate.TotalPrice,
                        Quantity = bookingUpdate.Quantity,
                        PaymentType = bookings.PaymentType,
                        PayByCredit = bookings.PayByCredit,
                        StripeCardString = bookings.StripeCardString
                    };
                    DayaxeDbContext.BookingHistories.InsertOnSubmit(bookingHistories);

                    Commit();

                    var discountBookings = DayaxeDbContext.DiscountBookings
                        .Where(db => db.BookingId == bookingUpdate.BookingId);
                    DayaxeDbContext.DiscountBookings.DeleteAllOnSubmit(discountBookings);

                    // Insert to Customer Credit if Refund to DayAxe Credit
                    if (logs != null && logs.CustomerId > 0)
                    {
                        var customerCredit = DayaxeDbContext.CustomerCredits
                            .FirstOrDefault(ci => ci.CustomerId == logs.CustomerId);
                        if (customerCredit != null)
                        {
                            switch (logs.CreditType)
                            {
                                case (int)Enums.CreditType.PartialPuchaseRefund:
                                case (int)Enums.CreditType.FullPurchaseRefund:
                                    customerCredit.Amount += logs.Amount;
                                    break;
                                    //case (int)Enums.CreditType.Referral:
                                    //    break;
                                    //case (int)Enums.CreditType.GiftCard:
                                    //    break;
                                    //case (int)Enums.CreditType.Charge:
                                    //    break;
                            }
                            customerCredit.LastUpdatedDate = DateTime.UtcNow;

                            DayaxeDbContext.CustomerCreditLogs.InsertOnSubmit(logs);

                            Commit();
                        }
                        else
                        {
                            throw new Exception("Can not Refund with current Customer.");
                        }
                    }

                    // Insert Schedule Refund
                    var schedules = new Schedules
                    {
                        BookingId = bookings.BookingId,
                        Name = "Refund",
                        Status = (int)Enums.ScheduleType.NotRun,
                        ScheduleSendType = (int)Enums.ScheduleSendType.IsEmailRefund,
                        BookingHistoryId = bookingHistories.Id,
                        CustomerCreditId = logs != null ? logs.Id : 0
                    };
                    DayaxeDbContext.Schedules.InsertOnSubmit(schedules);

                    // Insert Schedule Booking Confirmation - Updated
                    var schedulesConfirm = new Schedules
                    {
                        ScheduleSendType = (int)Enums.ScheduleSendType.IsMailConfirm,
                        Name = "Send Email confirm - Updated",
                        Status = (int)Enums.ScheduleType.NotRun,
                        BookingId = bookings.BookingId,
                        IsUpdated = true
                    };
                    DayaxeDbContext.Schedules.InsertOnSubmit(schedulesConfirm);

                    // Add Mail ProductWaitingList
                    if (bookings.CheckinDate.HasValue)
                    {
                        AddEmailToProductWaittingLists(new List<KeyValuePair<int, DateTime>>
                        {
                            new KeyValuePair<int, DateTime>(bookings.ProductId, bookings.CheckinDate.Value)
                        });
                    }

                    bookingUpdate.PassStatus = bookings.PassStatus;
                    bookingUpdate.CancelDated = bookings.CancelDated;
                    bookingUpdate.TotalPrice = bookings.TotalPrice;

                    Commit();
                }

                transaction.Complete();
            }
        }

        public void Delete(Bookings entity)
        {
            //DayaxeDbContext.Bookings.DeleteOnSubmit(entity);
            //Commit();
        }

        public void Delete(Func<Bookings, bool> predicate)
        {
            //IEnumerable<Bookings> bookings = DayaxeDbContext.Bookings.Where(predicate).AsEnumerable();
            //DayaxeDbContext.Bookings.DeleteAllOnSubmit(bookings);
            //Commit();
        }

        public Bookings GetById(long id)
        {
            Bookings res = GetAll().FirstOrDefault(h => h.BookingId == id);
            return res;
        }

        public IEnumerable<Bookings> GetAll()
        {
            /*var entities = CacheLayer.Get<List<Bookings>>(CacheKeys.BookingsCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }*/
            var entities = DayaxeDbContext.Bookings.ToList();
            //CacheLayer.Add(entities, CacheKeys.BookingsCacheKey);
            return entities.AsEnumerable();
        }

        public Bookings Refresh(Bookings entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateStripeChargeId(Bookings bookings, bool sendEmail)
        {
            using (var transaction = new TransactionScope())
            {
                var bookingUpdate = DayaxeDbContext.Bookings.FirstOrDefault(x => x.BookingId == bookings.BookingId);
                if (bookingUpdate != null)
                {
                    bookingUpdate.StripeChargeId = bookings.StripeChargeId;

                    if (sendEmail)
                    {
                        var schedules = new Schedules
                        {
                            ScheduleSendType = (int)Enums.ScheduleSendType.IsMailConfirm,
                            Name = "Send Email confirm",
                            Status = (int)Enums.ScheduleType.NotRun,
                            BookingId = bookingUpdate.BookingId
                        };
                        DayaxeDbContext.Schedules.InsertOnSubmit(schedules);

                        // Insert Schedule Send Add On Notification
                        var schedulesAddOn = new Schedules
                        {
                            ScheduleSendType = (int)Enums.ScheduleSendType.IsAddOnNotification,
                            Name = "Send Add-On Notification",
                            Status = (int)Enums.ScheduleType.NotRun,
                            BookingId = bookingUpdate.BookingId
                        };
                        DayaxeDbContext.Schedules.InsertOnSubmit(schedulesAddOn);
                    }

                    Commit();
                }

                transaction.Complete();
            }
        }

        #region Invoices

        public int AddInvoices(Invoices entity, string message)
        {
            using (var transaction = new TransactionScope())
            {
                DayaxeDbContext.Invoices.InsertOnSubmit(entity);

                if (!string.IsNullOrEmpty(message))
                {
                    var logs = new Logs
                    {
                        LogKey = "StripeRefundResponse",
                        UpdatedDate = DateTime.UtcNow,
                        UpdatedBy = 1,
                        UpdatedContent = message
                    };
                    DayaxeDbContext.Logs.InsertOnSubmit(logs);
                }

                Commit();
                transaction.Complete();

                return entity.Id;
            }
        }

        public IEnumerable<Invoices> GetInvoicesByBookingId(int bookingId)
        {
            return DayaxeDbContext.Invoices.Where(x => x.BookingId == bookingId);
        }

        #endregion

        #region Custom

        public IEnumerable<Bookings> GetBookingsReminder(DateTime dateNow)
        {
            return (from b in DayaxeDbContext.Bookings
                    join p in DayaxeDbContext.Products on b.ProductId equals p.ProductId
                    join h in DayaxeDbContext.Hotels on p.HotelId equals h.HotelId
                    let checkInDate = b.CheckinDate.Value.AddHours(h.TimeZoneOffset).AddDays(-1)
                    let dateRun = DateTime.UtcNow.AddHours(h.TimeZoneOffset)
                    where b.CheckinDate.HasValue &&
                        b.PassStatus == (int)Enums.BookingStatus.Active &&
                        dateRun.Date == checkInDate.Date &&
                        dateRun.Hour == dateNow.Hour
                    select b);
        }

        public void Update(List<Bookings> entity)
        {
            entity.ForEach(Update);
        }
        public void UpdateStatus(List<Bookings> entities)
        {
            using (var transaction = new TransactionScope())
            {
                if (entities.Any())
                {
                    entities.ForEach(bookings =>
                    {
                        var bookingUpdate = DayaxeDbContext.Bookings.FirstOrDefault(x => x.BookingId == bookings.BookingId);
                        if (bookingUpdate != null)
                        {
                            var discountUsed = GetDiscountUsedByBookingId(bookingUpdate.BookingId);

                            var bookingHistories = new BookingHistories
                            {
                                BookingId = bookingUpdate.BookingId,
                                CheckinDate = bookingUpdate.CheckinDate,
                                PassStatus = bookingUpdate.PassStatus,
                                ExpiredDate = bookingUpdate.ExpiredDate,
                                RedeemedDate = bookingUpdate.RedeemedDate,
                                UpdatedDate = DateTime.UtcNow,
                                UpdatedBy = bookings.UpdatedBy,
                                DiscountCode = discountUsed != null ? discountUsed.Code : string.Empty,
                                DiscountId = discountUsed != null ? discountUsed.Id : 0,
                                HotelPrice = bookingUpdate.HotelPrice,
                                MerchantPrice = bookingUpdate.MerchantPrice,
                                TotalPrice = bookingUpdate.TotalPrice,
                                Quantity = bookingUpdate.Quantity,
                                PaymentType = bookings.PaymentType,
                                PayByCredit = bookings.PayByCredit,
                                StripeCardString = bookings.StripeCardString
                            };
                            DayaxeDbContext.BookingHistories.InsertOnSubmit(bookingHistories);

                            bookingUpdate.PassStatus = bookings.PassStatus;
                            bookingUpdate.ExpiredDate = bookings.ExpiredDate;
                            bookingUpdate.RedeemedDate = bookings.RedeemedDate;

                            bookingUpdate.StripeChargeId = bookingUpdate.StripeChargeId;
                            bookingUpdate.StripeRefundAmount = bookingUpdate.StripeRefundAmount;
                            bookingUpdate.StripeRefundStatus = bookingUpdate.StripeRefundStatus;
                            bookingUpdate.StripeRefundTransactionId = bookingUpdate.StripeRefundTransactionId;
                            bookingUpdate.HasInvoice = bookingUpdate.HasInvoice;
                        }
                    });

                    Commit();
                }

                transaction.Complete();
            }
        }

        public IEnumerable<Bookings> GetAllBookingsActiveOfHotel(int hotelId, bool isGetDb = false)
        {
            var bookings = isGetDb
                ? (from p in DayaxeDbContext.Bookings
                   join p1 in DayaxeDbContext.Products on p.ProductId equals p1.ProductId
                   join p2 in DayaxeDbContext.Hotels on p1.HotelId equals p2.HotelId
                   where p2.HotelId == hotelId &&
                         p.PassStatus == (int)Enums.BookingStatus.Active
                   select p).ToList()
                : (from p in BookingList
                   join p1 in ProductList
                       on p.ProductId equals p1.ProductId
                   join p2 in HotelList
                       on p1.HotelId equals p2.HotelId
                   where p2.HotelId == hotelId &&
                         p.PassStatus == (int)Enums.BookingStatus.Active
                   select p).ToList();

            return bookings;
        }

        public List<Bookings> GetBookingsExpired()
        {
            var dateNow = DateTime.UtcNow.AddMinutes(5);
            return (from b in DayaxeDbContext.Bookings
                    join p in DayaxeDbContext.Products on b.ProductId equals p.ProductId
                    join h in DayaxeDbContext.Hotels on p.HotelId equals h.HotelId
                    where b.ExpiredDate.HasValue
                            && b.PassStatus == (int)Enums.BookingStatus.Active
                            && dateNow >= b.ExpiredDate.Value
                    select b)
                .ToList();
        }

        public bool IsBookingCodeExists(string bookingCode)
        {
            var booking = DbContext.Bookings.FirstOrDefault(x => x.BookingCode == bookingCode.Trim().Replace(" ", "")); //BookingList.FirstOrDefault(x => x.BookingCode == bookingCode.Trim().Replace(" ", ""));
            if (booking != null)
            {
                return true;
            }
            return false;
        }

        public IEnumerable<Bookings> GetBookingsByCustomerId(int customerId)
        {
            return (from p in BookingList
                    join p1 in CustomerInfoList
                        on p.CustomerId equals p1.CustomerId
                    where p1.CustomerId == customerId
                    orderby p.BookedDate descending
                    select p);
        }

        public string ValidatePinCode(int bookingId, string pinCode, bool isAdmin = false)
        {
            var response = new Response();
            var javaScriptSerializer = new JavaScriptSerializer();
            var booking = GetById(bookingId);
            var currentProduct = DayaxeDbContext.Products.FirstOrDefault(p => p.ProductId == booking.ProductId);
            if (booking == null || currentProduct == null)
            {
                response.Message = "Booking is not valid!";
                return javaScriptSerializer.Serialize(response);
            }

            var hotel = DayaxeDbContext.Hotels.FirstOrDefault(h => h.HotelId == currentProduct.HotelId);
            if (hotel != null && hotel.HotelPinCode != pinCode && !isAdmin)
            {
                response.Message = "Incorrect pin code";
                return javaScriptSerializer.Serialize(response);
            }

            var dateNow = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(hotel != null ? hotel.TimeZoneId : Constant.UsDefaultTime);
            switch (booking.PassStatus)
            {
                case (int)Enums.BookingStatus.Active:
                    if (BlockedDatesCustomPriceList.FirstOrDefault(
                            x => x.ProductId == booking.ProductId &&
                            x.Date.Date == dateNow.Date &&
                            x.Capacity == 0) != null)
                    {
                        // Set booking expired
                        if (booking.ExpiredDate.HasValue && booking.ExpiredDate.Value.Date <= dateNow.Date)
                        {
                            booking.PassStatus = (int)Enums.BookingStatus.Expired;
                            booking.ExpiredDate = DateTime.UtcNow;
                            UpdateStatus(new List<Bookings> { booking });
                        }
                        response.Message = "Ticket is within blackout days!";
                    }
                    else if (currentProduct.IsCheckedInRequired && booking.CheckinDate.HasValue && booking.CheckinDate.Value.Date != dateNow.Date)
                    {
                        response.Message = "Incorrect check-in date. Please check-in on the date you selected when booking.";
                    }
                    else
                    {
                        booking.PassStatus = (int)Enums.BookingStatus.Redeemed;
                        booking.RedeemedDate = DateTime.UtcNow;
                        UpdateStatus(new List<Bookings> { booking });
                        response.Message = "Success! This ticket is valid! You may issue this customer an access card";
                        response.IsSuccess = true;
                    }
                    break;
                case (int)Enums.BookingStatus.Expired:
                    response.Message = "Ticket Expired!";
                    break;
                case (int)Enums.BookingStatus.Redeemed:
                    response.Message = "Ticket has already been redeemed!";
                    break;
                case (int)Enums.BookingStatus.Refunded:
                    response.Message = "Failed! Ticket has already been refunded";
                    break;
            }
            return javaScriptSerializer.Serialize(response);
        }

        public string ValidatePinWithoutExpired(int bookingId, string pinCode)
        {
            var response = new Response();
            var javaScriptSerializer = new JavaScriptSerializer();
            var booking = DayaxeDbContext.Bookings.FirstOrDefault(x => x.BookingId == bookingId);
            if (booking == null)
            {
                response.Message = "Booking is not valid!";
                return javaScriptSerializer.Serialize(response);
            }
            if (booking.Products.Hotels.HotelPinCode != pinCode)
            {
                response.Message = "Incorrect pin code";
                return javaScriptSerializer.Serialize(response);
            }
            booking.PassStatus = (int)Enums.BookingStatus.Redeemed;
            booking.RedeemedDate = DateTime.UtcNow;
            Update(booking);
            response.Message = "Success! This ticket is valid! You may issue this customer an access card";
            response.IsSuccess = true;
            return javaScriptSerializer.Serialize(response);
        }

        public Bookings GetBookingInLast3Minutes(int productId, string emailAddress)
        {
            var booking = (from p in BookingList
                           join p1 in CustomerInfoList
                               on p.CustomerId equals p1.CustomerId
                           where p.ProductId == productId
                                 && p1.EmailAddress == emailAddress.Trim()
                                 && DateTime.UtcNow.AddMinutes(-3) <= p.BookedDate
                                 && p.StripeChargeId != null
                                 && p.IsEmailConfirmSend
                           select p);
            return booking.FirstOrDefault();
        }

        public List<Bookings> GetAllBookingsToday(SearchAllBookingsTodayParams param)
        {
            var dateNow = DateTime.UtcNow;
            var bookings = (from p in BookingList
                            join p1 in ProductList on p.ProductId equals p1.ProductId
                            join p2 in HotelList on p1.HotelId equals p2.HotelId
                            join ci in CustomerInfoList on p.CustomerId equals ci.CustomerId
                            let checkInDate = p.RedeemedDate.HasValue ? p.RedeemedDate : p.CheckinDate
                            let fullName = string.Format("{0} {1}", ci.FirstName, ci.LastName)
                            where p2.HotelId == param.HotelId &&
                                checkInDate.HasValue &&
                                checkInDate.Value.ToLosAngerlesTimeWithTimeZone(p2.TimeZoneId).Date == dateNow.ToLosAngerlesTimeWithTimeZone(p2.TimeZoneId).Date &&
                                  (string.IsNullOrEmpty(param.FilterText) ||
                                   (ci.FirstName.ToUpper().Contains(param.FilterText.ToUpper()) ||
                                    ci.LastName.ToUpper().Contains(param.FilterText.ToUpper()) ||
                                    p.BookingCode.ToUpper().Contains(param.FilterText.ToUpper()) ||
                                    fullName.ToUpper().Contains(param.FilterText.ToUpper())))
                            orderby checkInDate descending
                            select p).ToList();

            if (param.IsForRevenue)
            {
                bookings = FilterStatusForRevenue(bookings);
            }
            else
            {
                FilterBookings(ref bookings);
            }

            bookings.ForEach(booking =>
            {
                var product = ProductList.FirstOrDefault(x => x.ProductId == booking.ProductId);
                var hotel = HotelList.FirstOrDefault(h => h.HotelId == (product != null ? product.ProductId : 0));
                booking.ProductName = product != null ? product.ProductName : string.Empty;
                booking.TimeZoneId = hotel != null ? hotel.TimeZoneId : string.Empty;
            });

            return bookings.ToList();
        }

        public List<Bookings> GetAllbookingsByRange(SearchBookingsParams param)
        {
            var bookings = (from p in BookingList
                            join p1 in ProductList on p.ProductId equals p1.ProductId
                            join p2 in HotelList on p1.HotelId equals p2.HotelId
                            join ci in CustomerInfoList on p.CustomerId equals ci.CustomerId
                            let checkInDate = p.RedeemedDate.HasValue ? p.RedeemedDate : p.CheckinDate
                            let fullName = string.Format("{0} {1}", ci.FirstName, ci.LastName)
                            where p2.HotelId == param.HotelId && checkInDate.HasValue &&
                                  checkInDate.Value.ToLosAngerlesTimeWithTimeZone(p2.TimeZoneId).Date >= param.StartDate.Date &&
                                  checkInDate.Value.ToLosAngerlesTimeWithTimeZone(p2.TimeZoneId).Date <= param.EndDate.Date &&
                                  (string.IsNullOrEmpty(param.FilterText) ||
                                   (ci.FirstName.ToUpper().Contains(param.FilterText.ToUpper()) ||
                                    ci.LastName.ToUpper().Contains(param.FilterText.ToUpper()) ||
                                    p.BookingCode.ToUpper().Contains(param.FilterText.ToUpper()) ||
                                    fullName.ToUpper().Contains(param.FilterText.ToUpper())))
                            orderby checkInDate descending
                            select p).ToList();

            if (param.IsBookingForRevenue)
            {
                bookings = FilterStatusForRevenue(bookings);
            }
            else
            {
                FilterBookings(ref bookings);
            }

            bookings.ForEach(booking =>
            {
                var product = ProductList.FirstOrDefault(x => x.ProductId == booking.ProductId);
                var hotel = HotelList.FirstOrDefault(h => h.HotelId == (product != null ? product.ProductId : 0));
                booking.ProductName = product != null ? product.ProductName : string.Empty;
                booking.TimeZoneId = hotel != null ? hotel.TimeZoneId : string.Empty;
            });

            return bookings.ToList();
        }

        public List<Bookings> GetAllBookingsOfHotel(SearchAllBookingsParams param)
        {
            var bookings = (from p in BookingList
                            join p1 in ProductList on p.ProductId equals p1.ProductId
                            join p2 in HotelList on p1.HotelId equals p2.HotelId
                            join ci in CustomerInfoList on p.CustomerId equals ci.CustomerId
                            let checkInDate = p.RedeemedDate.HasValue ? p.RedeemedDate : p.CheckinDate
                            let fullName = string.Format("{0} {1}", ci.FirstName, ci.LastName)
                            where p2.HotelId == param.HotelId &&
                                  (string.IsNullOrEmpty(param.FilterText) ||
                                   (ci.FirstName.ToUpper().Contains(param.FilterText.ToUpper()) ||
                                    ci.LastName.ToUpper().Contains(param.FilterText.ToUpper()) ||
                                    p.BookingCode.ToUpper().Contains(param.FilterText.ToUpper()) ||
                                    fullName.ToUpper().Contains(param.FilterText.ToUpper())))
                            orderby checkInDate descending
                            select p).ToList();

            if (param.IsForRevenue)
            {
                bookings = FilterStatusForRevenue(bookings);
            }
            else
            {
                FilterBookings(ref bookings);
            }

            bookings.ForEach(booking =>
            {
                var product = ProductList.FirstOrDefault(x => x.ProductId == booking.ProductId);
                var hotel = HotelList.FirstOrDefault(h => h.HotelId == (product != null ? product.ProductId : 0));
                booking.ProductName = product != null ? product.ProductName : string.Empty;
                booking.TimeZoneId = hotel != null ? hotel.TimeZoneId : string.Empty;
            });

            return bookings.ToList();
        }

        public List<Bookings> GetAllBookingsOfProduct(int productId)
        {
            var bookings = (from p in BookingList
                            join p1 in ProductList on p.ProductId equals p1.ProductId
                            join ci in CustomerInfoList on p.CustomerId equals ci.CustomerId
                            let checkInDate = p.RedeemedDate.HasValue ? p.RedeemedDate : p.CheckinDate
                            where p1.ProductId == productId
                            orderby checkInDate descending
                            select p).ToList();

            bookings.ForEach(booking =>
            {
                var product = ProductList.FirstOrDefault(x => x.ProductId == booking.ProductId);
                var hotel = HotelList.FirstOrDefault(h => h.HotelId == (product != null ? product.ProductId : 0));
                booking.ProductName = product != null ? product.ProductName : string.Empty;
                booking.TimeZoneId = hotel != null ? hotel.TimeZoneId : string.Empty;
            });

            return bookings.ToList();
        }

        public IEnumerable<DataObject> GetHotelVisited(int hotelId)
        {
            var hotelVisited = (from p in BookingList
                                join p1 in ProductList on p.ProductId equals p1.ProductId
                                join p2 in HotelList on p1.HotelId equals p2.HotelId
                                where p2.HotelId != hotelId &&
                                    p.PassStatus == (int)Enums.BookingStatus.Redeemed &&
                                    p.RedeemedDate.HasValue
                                group p by p2.HotelName
                                    into g
                                select new DataObject
                                {
                                    Name = g.Key,
                                    Count = g.Count()
                                })
                .OrderByDescending(h => h.Count);
            return hotelVisited;
        }

        public Hotels GetHotelsByBookingId(int bookingId)
        {
            return (from h in HotelList
                    join p in ProductList on h.HotelId equals p.HotelId
                    join b in BookingList on p.ProductId equals b.ProductId
                    where b.BookingId == bookingId
                    select h).FirstOrDefault();
        }

        public Discounts GetDiscountsByBookingId(int bookingId)
        {
            return (from d in DiscountList
                    join db in DiscountBookingList on d.Id equals db.DiscountId
                    where db.BookingId == bookingId &&
                        d.PromoType != (int)Enums.PromoType.SubscriptionPromo
                    select d).FirstOrDefault();
        }

        #endregion

        #region Surveys

        private bool IsSurveyCodeExists(string surveyCode)
        {
            var booking = DayaxeDbContext.Surveys.FirstOrDefault(x => x.Code == surveyCode);
            if (booking != null)
            {
                return true;
            }
            return false;
        }

        public void AddSurveys(List<Surveys> surveys)
        {
            if (surveys.Any())
            {
                DbContext.Surveys.InsertAllOnSubmit(surveys);
                Commit();
            }
        }

        #endregion

        #region Discounts

        public Discounts GetDiscountUsedByBookingId(long bookingId)
        {
            return (from d in DiscountList
                    join db in DiscountBookingList on d.Id equals db.DiscountId
                    where db.BookingId == bookingId
                    select d).FirstOrDefault();
        }

        public Discounts GetGoldPassDiscountByBookingId(long bookingId)
        {
            return (from d in DayaxeDbContext.Discounts
                    join db in DayaxeDbContext.DiscountBookings on d.Id equals db.DiscountId
                    where db.BookingId == bookingId && d.PromoType == (int)Enums.PromoType.SubscriptionPromo
                    select d).FirstOrDefault();
        }

        public Discounts GetDiscountByBookingId(long bookingId)
        {
            return (from d in DayaxeDbContext.Discounts
                    join db in DayaxeDbContext.DiscountBookings on d.Id equals db.DiscountId
                    where db.BookingId == bookingId && d.PromoType != (int)Enums.PromoType.SubscriptionPromo
                    select d).FirstOrDefault();
        }

        #endregion

        #region AddCustomerCredits

        public void AddCustomerCredits(List<CustomerCredits> entities)
        {
            if (entities.Any())
            {
                DbContext.CustomerCredits.InsertAllOnSubmit(entities);
                Commit();
            }
        }

        #endregion

        #region Sales Report

        public List<ProductSalesReportObject> GetSalesReportByHotelId(int hotelId, string timezoneId, int year = 2017)
        {
            var result = new List<ProductSalesReportObject>();
            var products = ProductList.Where(p => p.HotelId == hotelId && !p.IsDelete && p.IsActive).ToList();
            products.ForEach(productItem =>
            {
                var bookings = BookingList
                    .Where(p => p.ProductId == productItem.ProductId &&
                        p.CheckinDate.HasValue &&
                        p.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(timezoneId).Year == year)
                    .ToList();

                // Current Calculate Avg By All Ticket Redeemed
                var bookingsCalculateSpend = BookingList
                    .Where(p => p.ProductId == productItem.ProductId &&
                                p.CheckinDate.HasValue &&
                                p.PassStatus == (int)Enums.BookingStatus.Redeemed)
                    .ToList();

                double avgPerTicketSpend = GetAvgPerTicketSpend(bookingsCalculateSpend);

                var productSalesReportObject = new ProductSalesReportObject
                {
                    ProductObject = productItem,
                    SalesReportObject = new List<SalesReportObject>()
                };

                foreach (Enums.MonthType type in Enum.GetValues(typeof(Enums.MonthType)))
                {
                    var param = new GetIventoryByMonthParams
                    {
                        Month = (int)type,
                        Year = year,
                        HotelId = productItem.HotelId,
                        ProductId = productItem.ProductId,
                        ProductTypeId = productItem.ProductType
                    };

                    var ticketRedeemedInCurrentMonth = bookings.Where(x =>
                        x.PassStatus == (int)Enums.BookingStatus.Redeemed &&
                        x.CheckinDate.HasValue &&
                        x.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(timezoneId).Month == (int)type)
                        .ToList();

                    var saleReportObj = new SalesReportObject
                    {
                        Month = type,
                        Inventory = GetIventoryByMonth(param),
                        TicketsSold = bookings.Where(b => b.CheckinDate.HasValue &&
                                                            b.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(timezoneId).Month == (int)type &&
                                                            b.PassStatus != (int)Enums.BookingStatus.Refunded)
                                    .Sum(x => x.Quantity),
                        TicketsRedeemed = ticketRedeemedInCurrentMonth
                                    .Sum(x => x.Quantity),
                        TicketsExpired = bookings.Where(b => b.CheckinDate.HasValue &&
                                                            b.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(timezoneId).Month == (int)type &&
                                                            b.PassStatus == (int)Enums.BookingStatus.Expired)
                                    .Sum(x => x.Quantity),
                        TicketsRefunded = bookings.Where(b => b.CheckinDate.HasValue &&
                                                            b.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(timezoneId).Month == (int)type &&
                                                            b.PassStatus == (int)Enums.BookingStatus.Refunded)
                                    .Sum(x => x.Quantity),
                        GrossSales = bookings.Where(b => b.CheckinDate.HasValue &&
                                                            b.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(timezoneId).Month == (int)type)
                                    .Sum(x => x.MerchantPrice * x.Quantity),
                        NetSales = ticketRedeemedInCurrentMonth
                                    .Sum(x => x.MerchantPrice * x.Quantity)
                    };

                    saleReportObj.Utilization = saleReportObj.Inventory != 0 ? Math.Round((double)saleReportObj.TicketsRedeemed * 100 / saleReportObj.Inventory) : 0;
                    saleReportObj.NetRevenue = CalculateNetRevenue(ticketRedeemedInCurrentMonth, timezoneId);
                    saleReportObj.AvgIncrementalRevenue = saleReportObj.TicketsRedeemed * avgPerTicketSpend;

                    saleReportObj.PercentSold = saleReportObj.Inventory != 0 ? saleReportObj.TicketsSold * 100 / saleReportObj.Inventory : 0;
                    saleReportObj.PercentRedeemed = saleReportObj.TicketsSold != 0 ? saleReportObj.TicketsRedeemed * 100 / saleReportObj.TicketsSold : 0;
                    saleReportObj.PercentExpired = saleReportObj.TicketsSold != 0 ? saleReportObj.TicketsExpired * 100 / saleReportObj.TicketsSold : 0;
                    saleReportObj.PercentRefunded = saleReportObj.TicketsSold != 0 ? saleReportObj.TicketsRefunded * 100 / saleReportObj.TicketsSold : 0;

                    productSalesReportObject.SalesReportObject.Add(saleReportObj);
                }

                result.Add(productSalesReportObject);
            });

            return result;
        }

        public int GetIventoryByMonth(GetIventoryByMonthParams param)
        {
            var customPrices = BlockedDatesCustomPriceList
                .Where(x => x.Date.Month == param.Month &&
                            x.Date.Year == param.Year &&
                            x.ProductId == param.ProductId)
                .ToList();

            var block = customPrices.Where(x => x.Capacity == 0).ToList();

            var openDates = customPrices.Except(block).ToList();

            var firstDate = new DateTime(param.Year, param.Month, 1);
            var dates = firstDate.GetDates()
                .Except(block.Select(x => x.Date))
                .Except(openDates.Select(x => x.Date))
                .ToList();

            int result = 0;

            openDates.ForEach(date =>
            {
                if (date.Capacity.HasValue && date.Capacity.Value > 0)
                {
                    result += date.Capacity.Value;
                }
                else
                {
                    dates.Add(date.Date);
                }
            });

            var products = ProductList.FirstOrDefault(p => p.ProductId == param.ProductId);

            dates.ForEach(date =>
            {
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        result += products != null ? products.PassCapacityMon : 0;
                        break;
                    case DayOfWeek.Tuesday:
                        result += products != null ? products.PassCapacityTue : 0;
                        break;
                    case DayOfWeek.Wednesday:
                        result += products != null ? products.PassCapacityWed : 0;
                        break;
                    case DayOfWeek.Thursday:
                        result += products != null ? products.PassCapacityThu : 0;
                        break;
                    case DayOfWeek.Friday:
                        result += products != null ? products.PassCapacityFri : 0;
                        break;
                    case DayOfWeek.Saturday:
                        result += products != null ? products.PassCapacitySat : 0;
                        break;
                    case DayOfWeek.Sunday:
                        result += products != null ? products.PassCapacitySun : 0;
                        break;
                }
            });

            return result;
        }

        /// <summary>
        /// Return Weekdays - Weekens Value
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public KeyValuePair<int, int> GetIventoryByMonthWeekendWeekday(GetIventoryByMonthParams param)
        {
            var customPrices = BlockedDatesCustomPriceList
                .Where(x => x.Date.ToLosAngerlesTimeWithTimeZone(param.TimezoneId).Month == param.Month &&
                            x.Date.ToLosAngerlesTimeWithTimeZone(param.TimezoneId).Year == param.Year &&
                            x.ProductId == param.ProductId)
                .ToList();

            var block = customPrices.Where(x => x.Capacity == 0).ToList();

            var openDates = customPrices.Except(block).ToList();

            var firstDate = new DateTime(param.Year, param.Month, 1);
            var dates = firstDate.GetDates()
                .Except(block.Select(x => x.Date))
                .Except(openDates.Select(x => x.Date))
                .ToList();

            int iventoryWeekend = 0;
            int iventoryWeekdays = 0;

            openDates.ForEach(date =>
            {
                switch (date.Date.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                    case DayOfWeek.Tuesday:
                    case DayOfWeek.Wednesday:
                    case DayOfWeek.Thursday:
                    case DayOfWeek.Friday:
                        if (date.Capacity.HasValue && date.Capacity.Value > 0)
                        {
                            iventoryWeekdays += date.Capacity.Value;
                        }
                        break;
                    case DayOfWeek.Saturday:
                    case DayOfWeek.Sunday:
                        if (date.Capacity.HasValue && date.Capacity.Value > 0)
                        {
                            iventoryWeekend += date.Capacity.Value;
                        }
                        break;
                }
            });

            var products = ProductList.FirstOrDefault(p => p.ProductId == param.ProductId);

            dates.ForEach(date =>
            {
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        iventoryWeekdays += products != null ? products.PassCapacityMon : 0;
                        break;
                    case DayOfWeek.Tuesday:
                        iventoryWeekdays += products != null ? products.PassCapacityTue : 0;
                        break;
                    case DayOfWeek.Wednesday:
                        iventoryWeekdays += products != null ? products.PassCapacityWed : 0;
                        break;
                    case DayOfWeek.Thursday:
                        iventoryWeekdays += products != null ? products.PassCapacityThu : 0;
                        break;
                    case DayOfWeek.Friday:
                        iventoryWeekdays += products != null ? products.PassCapacityFri : 0;
                        break;
                    case DayOfWeek.Saturday:
                        iventoryWeekend += products != null ? products.PassCapacitySat : 0;
                        break;
                    case DayOfWeek.Sunday:
                        iventoryWeekend += products != null ? products.PassCapacitySun : 0;
                        break;
                }
            });

            return new KeyValuePair<int, int>(iventoryWeekdays, iventoryWeekend);
        }

        private double GetAvgPerTicketSpend(List<Bookings> bookings)
        {
            var surveyFinish = bookings
                .Where(x => SurveyList.Where(sl => sl.BookingId == x.BookingId).Any(s => s.IsFinish))
                .Select(x => SurveyList.FirstOrDefault(sl => sl.BookingId == x.BookingId))
                .ToList();

            if (surveyFinish.Any())
            {
                surveyFinish.ForEach(item =>
                {
                    double spend = 0;
                    if (item.IsBuyFoodAndDrink && item.FoodAndDrinkPrice.HasValue)
                    {
                        spend += item.FoodAndDrinkPrice.Value;
                    }

                    if (item.IsPayForParking)
                    {
                        spend += Constant.ParkingPrice;
                    }

                    if (item.IsBuySpaService && item.SpaServicePrice.HasValue)
                    {
                        spend += item.SpaServicePrice.Value;
                    }

                    if (item.IsBuyAdditionalService && item.AdditionalServicePrice.HasValue)
                    {
                        spend += item.AdditionalServicePrice.Value;
                    }

                    item.EstSpend = spend;
                    item.RedeemedDate = item.Bookings.RedeemedDate;
                });
            }

            var totalResponsed = bookings.Where(x => surveyFinish.Select(s => s.BookingId).Contains(x.BookingId))
                .Sum(b => b.Quantity);

            return surveyFinish.Count > 0
                ? surveyFinish.Sum(s => s.EstSpend) / totalResponsed
                : 0;
        }

        private double CalculateNetRevenue(List<Bookings> bookings, string timezoneId)
        {
            double result = 0;

            if (bookings.Any())
            {
                var booking = bookings.First();
                int productId = booking.ProductId;
                var checkInDate = booking.CheckinDate.HasValue
                    ? booking.CheckinDate.Value
                    : DateTime.UtcNow;
                var startDate = new DateTime(checkInDate.Year, checkInDate.Month, 1);
                var endDate = new DateTime(checkInDate.Year, checkInDate.Month, DateTime.DaysInMonth(checkInDate.Year, checkInDate.Month));
                var revShares = RevShareList.Where(rs =>
                        rs.ProductId == productId &&
                        ((rs.EndDate.Date <= endDate && rs.EndDate.Date >= startDate) ||
                            (rs.StartDate.Date >= startDate && rs.StartDate.Date <= endDate) ||
                            (rs.StartDate.Date <= startDate && rs.EndDate.Date >= endDate)))
                    .OrderBy(rs => rs.StartDate)
                    .ToList();

                var nStartDte = startDate;
                revShares.ForEach(rs =>
                {
                    var nEndDate = rs.EndDate;
                    if (rs.StartDate < startDate)
                    {
                        nStartDte = startDate;
                    }
                    if (nEndDate > endDate)
                    {
                        nEndDate = endDate;
                    }
                    var nBookings = bookings.Where(b =>
                            b.CheckinDate.HasValue && b.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(timezoneId).Date >= nStartDte &&
                            b.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(timezoneId).Date <= nEndDate)
                        .ToList();
                    result += nBookings.Sum(b => b.Quantity * b.MerchantPrice * (1 - rs.RevShareAmount / 100));
                });
            }

            return result > 0 ? result : bookings.Sum(b => b.Quantity * b.MerchantPrice);
        }

        #endregion

        #region Private Function

        private List<Bookings> FilterStatusForRevenue(List<Bookings> bookingses)
        {
            return
                bookingses.Where(
                    x =>
                        x.PassStatus == (int)Enums.BookingStatus.Redeemed)
                        .DistinctBy(x => x.BookingId)
                        .ToList();
        }

        private void FilterBookings(ref List<Bookings> bookings)
        {
            bookings.ForEach(item =>
            {
                var surveys = SurveyList.FirstOrDefault(sl => sl.BookingId == item.BookingId);
                if (surveys != null)
                {
                    item.EstSpend = 0;
                    if (surveys.IsBuyFoodAndDrink && surveys.FoodAndDrinkPrice.HasValue)
                    {
                        item.EstSpend += surveys.FoodAndDrinkPrice.Value;
                    }
                    if (surveys.IsBuySpaService && surveys.SpaServicePrice.HasValue)
                    {
                        item.EstSpend += surveys.SpaServicePrice.Value;
                    }
                    if (surveys.IsPayForParking && surveys.ParkingPrice.HasValue)
                    {
                        item.EstSpend += surveys.ParkingPrice.Value;
                    }
                    if (surveys.IsBuyAdditionalService && surveys.AdditionalServicePrice.HasValue)
                    {
                        item.EstSpend += surveys.AdditionalServicePrice.Value;
                    }

                    item.UserRating = surveys.Rating.HasValue ? surveys.Rating.Value : 0;

                    item.SurveyFeedback = surveys.RateCommend;
                }
            });
        }

        #endregion


        #region BookingTemp

        public int AddBookingsTemp(BookingsTemps bookings, int discountId)
        {
            using (var transaction = new TransactionScope())
            {
                try
                {
                    // Insert New BookingsTemps
                    DayaxeDbContext.BookingsTemps.InsertOnSubmit(bookings);
                    Commit();

                    // Insert DiscountBookingsTemps
                    if (discountId > 0)
                    {
                        var discountBookingsTemps = new DiscountBookingsTemps
                        {
                            DiscountId = discountId,
                            ProductId = bookings.ProductId,
                            BookingId = bookings.BookingId
                        };
                        DayaxeDbContext.DiscountBookingsTemps.InsertOnSubmit(discountBookingsTemps);

                        Commit();
                    }

                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }

            return bookings.BookingId;
        }

        public Tuple<BookingsTemps, DiscountBookingsTemps> GetBookingsTempById(int bookingsTempId)
        {
            var bookingsTemp = DayaxeDbContext.BookingsTemps.FirstOrDefault(b => b.BookingId == bookingsTempId);
            var discounts = DayaxeDbContext.DiscountBookingsTemps.FirstOrDefault(x => x.BookingId == bookingsTempId);
            return new Tuple<BookingsTemps, DiscountBookingsTemps>(bookingsTemp, discounts);
        }

        #endregion

        #region Product Available Upgrade

        public bool IsAvailableUpgrade(int productId)
        {
            return (from p in ProductList
                    join p1 in ProductUpgradeList
                    on p.ProductId equals p1.ProductId
                    where p1.ProductId == productId &&
                          p1.UpgradeId != productId &&
                          p.IsActive &&
                          !p.IsDelete &&
                          p.ProductType != (int)Enums.ProductType.DayPass
                    select p1.UpgradeId).Any();
        }

        #endregion

        #region Subscription Bookings

        public void ExpiredSubscriptions()
        {
            using (var transaction = new TransactionScope())
            {
                //var subBookingsTest = (from sb in DayaxeDbContext.SubscriptionBookings
                //    join sc in DayaxeDbContext.SubscriptionCycles on sb.Id equals sc.SubscriptionBookingId
                //    where sc.Id == DayaxeDbContext.SubscriptionCycles
                //              .Where(scChild => scChild.SubscriptionBookingId == sb.Id).Max(scChild => scChild.Id) &&
                //          sc.EndDate.HasValue &&
                //          sc.EndDate.Value <= DateTime.UtcNow &&
                //          sb.Status == (int) Enums.SubscriptionBookingStatus.Active
                //    select new { sb, sc}).ToList();

                var subBookings = DayaxeDbContext.SubscriptionBookings.Where(sb =>
                    sb.EndDate.HasValue &&
                    sb.EndDate.Value <= DateTime.UtcNow &&
                    sb.Status == (int)Enums.SubscriptionBookingStatus.Active).ToList();

                if (subBookings.Any())
                {
                    subBookings.ForEach(subBooking =>
                    {
                        var subscriptionService = new StripeSubscriptionService();
                        Stripe.StripeSubscription subscription = subscriptionService.Get(subBooking.StripeSubscriptionId);

                        var subscriptionCycle = DayaxeDbContext.SubscriptionCycles
                            .Where(sc => sc.SubscriptionBookingId == subBooking.Id)
                            .OrderByDescending(sc => sc.CycleNumber)
                            .FirstOrDefault();

                        // Update status of Cycle too
                        if (subscriptionCycle != null)
                        {
                            var subDiscounts = (from sdu in DayaxeDbContext.SubsciptionDiscountUseds
                                                join sb in DayaxeDbContext.SubscriptionBookings on sdu.SubscriptionBookingId equals sb.Id
                                                join d in DayaxeDbContext.Discounts on sdu.DiscountId equals d.Id
                                                where sb.Id == subBooking.Id
                                                select d).First();

                            switch (subscription.Status)
                            {
                                case "past_due":
                                case "unpaid":
                                    subBooking.Status = (int)Enums.SubscriptionBookingStatus.Suspended;
                                    subscriptionCycle.Status = (int)Enums.SubscriptionBookingStatus.Suspended;
                                    ExpiredBookingByCancelledSubscription(subDiscounts.Id);
                                    break;
                                case "active":
                                    // Set Date of Discounts
                                    subBooking.Status = (int)Enums.SubscriptionBookingStatus.Active;
                                    subscriptionCycle.Status = (int)Enums.SubscriptionBookingStatus.Active;
                                    subDiscounts.EndDate = subscription.CurrentPeriodEnd;

                                    // Set Date of Subscription Bookings
                                    subBooking.StartDate = subscription.CurrentPeriodStart;
                                    subBooking.EndDate = subscription.CurrentPeriodEnd;
                                    subBooking.LastUpdatedDate = DateTime.UtcNow;
                                    subBooking.LastUpdatedBy = 0;
                                    break;
                                case "canceled":
                                    subBooking.Status = (int)Enums.SubscriptionBookingStatus.End;
                                    subscriptionCycle.Status = (int)Enums.SubscriptionBookingStatus.End;
                                    ExpiredBookingByCancelledSubscription(subDiscounts.Id);

                                    // Insert Schedule Send Email Cancellation
                                    var schedules = new Schedules
                                    {
                                        ScheduleSendType = (int)Enums.ScheduleSendType.IsEmailSubscriptionCancellation,
                                        Name = "Send Email Subscription Cancellation Confirmation",
                                        Status = (int)Enums.ScheduleType.NotRun,
                                        SubscriptionBookingId = subBooking.Id
                                    };
                                    DayaxeDbContext.Schedules.InsertOnSubmit(schedules);
                                    break;
                                default: //trialing
                                    break;
                            }
                        }
                    });
                }

                Commit();
                transaction.Complete();
            }
        }

        public void ExpiredBookingByCancelledSubscription(int discountId)
        {
            using (var transaction = new TransactionScope())
            {
                var bookingsExpired = (from b in DayaxeDbContext.Bookings
                                       join p in DayaxeDbContext.Products on b.ProductId equals p.ProductId
                                       join db in DayaxeDbContext.DiscountBookings on b.BookingId equals db.BookingId
                                       join d in DayaxeDbContext.Discounts on db.DiscountId equals d.Id
                                       where b.ExpiredDate.HasValue &&
                                             b.PassStatus == (int)Enums.BookingStatus.Active &&
                                             d.Id == discountId
                                       select b)
                    .ToList();

                if (bookingsExpired.Any())
                {
                    bookingsExpired.ForEach(booking =>
                    {
                        booking.PassStatus = (int)Enums.BookingStatus.Expired;
                    });

                    Update(bookingsExpired);
                }

                transaction.Complete();
            }
        }

        #endregion

        #region Custom Price

        public void SaveCalendar(List<SaveCalendarObject> items, List<DateTime> dates)
        {
            using (var transaction = new TransactionScope())
            {
                var newCustomPrice = new List<BlockedDatesCustomPrice>();
                dates.ForEach(dateItem =>
                {
                    items.ForEach(saveObject =>
                    {
                        if (!(saveObject.Quantity == -1 && saveObject.Price.Equals(0)))
                        {
                            var customPrice = DayaxeDbContext.BlockedDatesCustomPrice.FirstOrDefault(bd =>
                                bd.ProductId == saveObject.ProductId && bd.Date.Date == dateItem.Date);
                            if (customPrice == null)
                            {
                                var obj = new BlockedDatesCustomPrice
                                {
                                    BlockedDateId = 0,
                                    Capacity = saveObject.Quantity != -1 ? saveObject.Quantity : -1,
                                    RegularPrice = !saveObject.Price.Equals(0) ? saveObject.Price : 0,
                                    DiscountPrice = 0,
                                    ProductId = saveObject.ProductId,
                                    Date = dateItem.Date
                                };

                                if (saveObject.Quantity == -1)
                                {
                                    var products = ProductList.FirstOrDefault(p => p.ProductId == saveObject.ProductId);
                                    var actualPrice = GetProductDailyPriceByDate(products, dateItem);
                                    obj.Capacity = actualPrice.Capacity;
                                }

                                if (saveObject.Price.Equals(0))
                                {
                                    var products = ProductList.FirstOrDefault(p => p.ProductId == saveObject.ProductId);
                                    var actualPrice = GetProductDailyPriceByDate(products, dateItem);
                                    obj.RegularPrice = actualPrice.Price;
                                }
                                newCustomPrice.Add(obj);
                            }
                            else
                            {
                                customPrice.Capacity = saveObject.Quantity != -1 ? saveObject.Quantity : customPrice.Capacity;
                                customPrice.RegularPrice = !saveObject.Price.Equals(0) ? saveObject.Price : customPrice.RegularPrice;
                                customPrice.ProductId = saveObject.ProductId;
                                customPrice.Date = dateItem.Date;

                                if (customPrice.Capacity == -1)
                                {
                                    var products = ProductList.FirstOrDefault(p => p.ProductId == saveObject.ProductId);
                                    var actualPrice = GetProductDailyPriceByDate(products, dateItem);
                                    customPrice.Capacity = actualPrice.Capacity;
                                }

                                if (customPrice.RegularPrice.Equals(0))
                                {
                                    var products = ProductList.FirstOrDefault(p => p.ProductId == saveObject.ProductId);
                                    var actualPrice = GetProductDailyPriceByDate(products, dateItem);
                                    customPrice.RegularPrice = actualPrice.Price;
                                }
                            }
                        }
                    });
                });

                if (newCustomPrice.Any())
                {
                    DayaxeDbContext.BlockedDatesCustomPrice.InsertAllOnSubmit(newCustomPrice);
                }

                Commit();
                transaction.Complete();
            }
        }

        private ActualPriceObject GetProductDailyPriceByDate(Products products, DateTime date)
        {
            if (BlockedDatesCustomPriceList.Any(bdcpchild => bdcpchild.ProductId == products.ProductId && bdcpchild.Date == date.Date))
            {
                var result = GetCustomPriceByDate(products.ProductId, date);

                if (result.Price.Equals(0))
                {
                    var defaultPrice = GetRegularPriceByDate(products, date);
                    result.Price = defaultPrice.Price;
                }

                if (result.Capacity == -1)
                {
                    var defaultPrice = GetRegularPriceByDate(products, date);
                    result.Capacity = defaultPrice.Capacity;
                }

                return result;
            }

            return GetRegularPriceByDate(products, date);
        }

        private ActualPriceObject GetCustomPriceByDate(long productId, DateTime date)
        {
            var customPrice = BlockedDatesCustomPriceList
                .First(bdcpchild => bdcpchild.ProductId == productId && bdcpchild.Date.Date == date.Date.Date);
            return new ActualPriceObject
            {
                Price = customPrice.RegularPrice,
                DiscountPrice = customPrice.DiscountPrice,
                Capacity = customPrice.Capacity ?? 0
            };
        }

        private ActualPriceObject GetRegularPriceByDate(Products product, DateTime date)
        {
            var defaultPrice = DefaultPriceList.Where(d => d.EffectiveDate.Date < date.Date && d.ProductId == product.ProductId)
                .OrderByDescending(d => d.EffectiveDate).FirstOrDefault();
            if (defaultPrice != null)
            {
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        return new ActualPriceObject
                        {
                            Price = defaultPrice.PriceMon,
                            DiscountPrice = defaultPrice.UpgradeDiscountMon,
                            Capacity = defaultPrice.PassCapacityMon
                        };
                    case DayOfWeek.Tuesday:
                        return new ActualPriceObject
                        {
                            Price = defaultPrice.PriceTue,
                            DiscountPrice = defaultPrice.UpgradeDiscountTue,
                            Capacity = defaultPrice.PassCapacityTue
                        };
                    case DayOfWeek.Wednesday:
                        return new ActualPriceObject
                        {
                            Price = defaultPrice.PriceWed,
                            DiscountPrice = defaultPrice.UpgradeDiscountWed,
                            Capacity = defaultPrice.PassCapacityWed
                        };
                    case DayOfWeek.Thursday:
                        return new ActualPriceObject
                        {
                            Price = defaultPrice.PriceThu,
                            DiscountPrice = defaultPrice.UpgradeDiscountThu,
                            Capacity = defaultPrice.PassCapacityThu
                        };
                    case DayOfWeek.Friday:
                        return new ActualPriceObject
                        {
                            Price = defaultPrice.PriceFri,
                            DiscountPrice = defaultPrice.UpgradeDiscountFri,
                            Capacity = defaultPrice.PassCapacityFri
                        };
                    case DayOfWeek.Saturday:
                        return new ActualPriceObject
                        {
                            Price = defaultPrice.PriceSat,
                            DiscountPrice = defaultPrice.UpgradeDiscountSat,
                            Capacity = defaultPrice.PassCapacitySat
                        };
                    default:
                        return new ActualPriceObject
                        {
                            Price = defaultPrice.PriceSun,
                            DiscountPrice = defaultPrice.UpgradeDiscountSun,
                            Capacity = defaultPrice.PassCapacitySun
                        };
                }
            }

            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return new ActualPriceObject
                    {
                        Price = product.PriceMon,
                        DiscountPrice = product.UpgradeDiscountMon,
                        Capacity = product.PassCapacityMon
                    };
                case DayOfWeek.Tuesday:
                    return new ActualPriceObject
                    {
                        Price = product.PriceTue,
                        DiscountPrice = product.UpgradeDiscountTue,
                        Capacity = product.PassCapacityTue
                    };
                case DayOfWeek.Wednesday:
                    return new ActualPriceObject
                    {
                        Price = product.PriceWed,
                        DiscountPrice = product.UpgradeDiscountWed,
                        Capacity = product.PassCapacityWed
                    };
                case DayOfWeek.Thursday:
                    return new ActualPriceObject
                    {
                        Price = product.PriceThu,
                        DiscountPrice = product.UpgradeDiscountThu,
                        Capacity = product.PassCapacityThu
                    };
                case DayOfWeek.Friday:
                    return new ActualPriceObject
                    {
                        Price = product.PriceFri,
                        DiscountPrice = product.UpgradeDiscountFri,
                        Capacity = product.PassCapacityFri
                    };
                case DayOfWeek.Saturday:
                    return new ActualPriceObject
                    {
                        Price = product.PriceSat,
                        DiscountPrice = product.UpgradeDiscountSat,
                        Capacity = product.PassCapacitySat
                    };
                default:
                    return new ActualPriceObject
                    {
                        Price = product.PriceSun,
                        DiscountPrice = product.UpgradeDiscountSun,
                        Capacity = product.PassCapacitySun
                    };
            }
        }

        #endregion

        #region Tax Region

        public Taxes GetTaxByDate(DateTime date, int hotelId)
        {
            return TaxList.FirstOrDefault(t => t.Date.Date == date.Date && t.HotelId == hotelId);
        }

        public void AddTaxes(Taxes entity)
        {
            DayaxeDbContext.Taxes.InsertOnSubmit(entity);
            Commit();
        }

        #endregion

        #region Distances

        public void AddDistance(Distances entity)
        {
            DayaxeDbContext.Distances.InsertOnSubmit(entity);
            Commit();
        }

        #endregion
    }
}
