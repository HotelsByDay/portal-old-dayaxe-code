using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Dayaxe.SendEmail;
using DayaxeDal;
using DayaxeDal.Extensions;
using DayaxeDal.Parameters;
using DayaxeDal.Repositories;
using Newtonsoft.Json;
using Stripe;
using Timer = System.Timers.Timer;

namespace Dayaxe.Schedule
{
    public partial class AutoSendEmailServiceProduction : ServiceBase
    {
        internal static volatile bool IsRunning;

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
        public AutoSendEmailServiceProduction(string[] args)
        {
            InitializeComponent();
        }

        internal void TestStartupAndStop(string[] args)
        {
            OnStart(args);
            Console.ReadLine();
            OnStop();
        }

        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus
            {
                dwCurrentState = ServiceState.SERVICE_START_PENDING,
                dwWaitHint = 100000
            };
            SetServiceStatus(ServiceHandle, ref serviceStatus);

            StripeConfiguration.SetApiKey(AppConfiguration.StripeApiKey);

            //IUnityContainer myContainer = new UnityContainer();
            //myContainer.RegisterType<IHotelService, HotelService>();

            // Set up a timer to trigger every minute.
            var timer = new Timer
            {
                Interval = 5000
            };
            // 60 seconds
            timer.Elapsed += OnTimer;
            timer.Start();

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(ServiceHandle, ref serviceStatus);
        }

        protected override void OnStop()
        {
            // eventLog1.WriteEntry("In onStop.");
        }

        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            using (var bookingRepository = new BookingRepository())
            {
                if (IsRunning)
                {
                    //var log = new Logs
                    //{
                    //    LogKey = "Service Still Running",
                    //    UpdatedContent = string.Format("{0}", DateTime.UtcNow),
                    //    UpdatedDate = DateTime.UtcNow,
                    //    UpdatedBy = 1
                    //};
                    //bookingRepository.AddLog(log);
                    return;
                }
                IsRunning = true;
                try
                {
                    var schedules = bookingRepository.GetAllSchedule().ToList();
                    // MigrateOldCustomerCredits();

                    // MigrateOldBlockedDate();

                    // MigrateOldSurvey();

                    DeliveryGiftCard();

                    schedules.ForEach(item =>
                    {
                        switch (item.ScheduleSendType)
                        {
                            case (int)Enums.ScheduleSendType.IsMailConfirm:
                                SendEmailConfirm(item);
                                break;
                            case (int)Enums.ScheduleSendType.IsRecalculateExpiredDate:
                                // Run first time when Service Start 
                                // Run in midnight every day
                                if (!item.LastRun.HasValue ||
                                    DateTime.UtcNow.Hour != item.LastRun.Value.Hour)
                                {
                                    CacheLayer.ClearAll();

                                    ReCalculateSubscriptionExpired();

                                    ReCalculateExpiredDate(item);
                                }
                                break;
                            case (int)Enums.ScheduleSendType.IsBookingAlert:
                                //reminderTime.Hour
                                //reminderTime.Minute
                                if (!item.LastRun.HasValue ||
                                    DateTime.UtcNow.Hour != item.LastRun.Value.Hour)
                                {
                                    SendBookingAlert(item);
                                }
                                break;
                            case (int)Enums.ScheduleSendType.IsEmailRefund:
                                SendEmailRefund(item);
                                break;
                            case (int)Enums.ScheduleSendType.IsEmailSurvey:
                                SendEmailSurvey(item);
                                break;
                            case (int)Enums.ScheduleSendType.IsAddOnNotification:
                                SendAddOnNotification(item);
                                break;
                            case (int)Enums.ScheduleSendType.IsAddOnNotificationAfterRedemption:
                                SendAddOnNotificationRedmeption(item);
                                break;
                            case (int)Enums.ScheduleSendType.IsEmailWaitingList:
                                SendTicketOpenedUp(item);
                                break;
                            case (int)Enums.ScheduleSendType.IsNotifiedSurvey:
                                SendNotifiedSurvey(item);
                                break;
                            case (int)Enums.ScheduleSendType.IsEmailMonthlySalesReport:
                                //var saleReportSendDateTime = EmailConfig.SalesReportSendDateTime;
                                //int year = DateTime.UtcNow.Year;
                                //int month = DateTime.UtcNow.Month;
                                //if (month == 0)
                                //{
                                //    year -= 1;
                                //}
                                //dateNow = DateTime.UtcNow;
                                //dateRunSchedule =
                                //    new DateTime(year, month, saleReportSendDateTime.Day, saleReportSendDateTime.Hour,
                                //            saleReportSendDateTime.Minute, 0, DateTimeKind.Utc)
                                //        .AddHours(Math.Abs(Constant.LosAngelesTimeWithUtc));
                                ////reminderTime.Hour
                                ////reminderTime.Minute
                                //if (EqualDate(dateRunSchedule, dateNow))
                                //{
                                //    SendEmailMonthlySalesReport(item);
                                //}
                                break;
                            case (int)Enums.ScheduleSendType.IsEmailConfirmSubscription:
                                SendEmailConfirmSubscription(item);
                                break;
                            case (int)Enums.ScheduleSendType.IsEmailGoldPassNoShow:
                                SendEmailGoldPassNoShow(item);
                                break;
                            case (int)Enums.ScheduleSendType.IsEmailSubscriptionCancellation:
                                SendEmailSubscriptionCancellation(item);
                                break;
                            case (int)Enums.ScheduleSendType.IsEmailGiftCardConfirmation:
                                SendEmailGiftCardConfirmation(item);
                                break;
                        }
                    });
                }
                catch (Exception ex)
                {
                    string json = JsonConvert.SerializeObject(ex, CustomSettings.SerializerSettings());
                    var log = new Logs
                    {
                        LogKey = "Update Scheduler Error",
                        UpdatedContent = string.Format("{0} - {1} - {2}", json, ex.Message, ex.StackTrace),
                        UpdatedDate = DateTime.UtcNow,
                        UpdatedBy = 1
                    };
                    bookingRepository.AddLog(log);
                }
                finally
                {
                    IsRunning = false;
                }
            }
        }

        private void SendEmailConfirm(Schedules schedules)
        {
            using (var bookingRepository = new BookingRepository())
            {
                int bookingId = schedules.BookingId.HasValue ? schedules.BookingId.Value : 0;

                // Refresh Hotel Data
                var booking = bookingRepository.GetBookings(bookingId);

                // - Check If Current Booking is Capture 
                // - Wait 2 minute for user select Upgrade or not
                if (booking.PassStatus == (int) Enums.BookingStatus.Capture)
                {
                    if (booking.BookedDate.AddMinutes(3) > DateTime.UtcNow)
                    {
                        return;
                    }

                    string responseStripe = string.Empty;
                    if (!string.IsNullOrEmpty(booking.StripeChargeId))
                    {
                        var chargeService = new StripeChargeService();
                        StripeCharge charge = chargeService.Capture(booking.StripeChargeId);
                        responseStripe = charge.StripeResponse.ResponseJson;
                    }

                    var logs = new Logs
                    {
                        LogKey = "UpgradeSubscriptionCharge",
                        UpdatedContent = string.Format("ScheduleId: {0} - Response: {1}",
                            schedules.Id,
                            responseStripe),
                        UpdatedBy = 0,
                        UpdatedDate = DateTime.UtcNow
                    };

                    booking.PassStatus = (int)Enums.BookingStatus.Active;
                    booking.IsActiveSubscription = false;

                    bookingRepository.AddLog(logs);
                }
                var product = bookingRepository.GetProduct(booking.ProductId);
                var hotels = bookingRepository.GetHotel(product.HotelId);
                var customerInfos = bookingRepository.GetCustomer(booking.CustomerId);
                var finePrint = bookingRepository.GetDiscountByBookingId(bookingId);
                var goldPassDiscount = bookingRepository.GetGoldPassDiscountByBookingId(bookingId);

                if (booking.IsEmailConfirmSend && !schedules.IsUpdated)
                {
                    schedules.Status = (int)Enums.ScheduleType.Complete;
                    if (!schedules.CompleteDate.HasValue)
                    {
                        schedules.CompleteDate = DateTime.UtcNow;
                        schedules.LastRun = DateTime.UtcNow;
                    }
                }
                else
                {
                    var maxGuest = product.MaxGuest <= 0 ? Constant.DefaultMaxGuest : product.MaxGuest;
                    var reviews = bookingRepository.GetSurveyByHotelId(product.HotelId).ToList();

                    var emailInfo = new EmailInfo
                    {
                        UserEmail = customerInfos.EmailAddress,
                        Neighborhood = string.Format("{0}, {1}", hotels.Neighborhood, hotels.City),
                        Rating = hotels.TripAdvisorRating ?? 4,
                        CustomerRating = reviews.Any() ? reviews.Average(r => r.Rating ?? 5) : 0,
                        TotalCustomerReviews = reviews.Count,
                        FirstName = customerInfos.FirstName,
                        LastName = customerInfos.LastName,
                        Address = hotels.StreetAddress,
                        AddressInfo = string.Format(" {0}, {1} {2}", hotels.City, hotels.State, hotels.ZipCode),
                        UrlViewDayPass = string.Format("{0}/{1}/ViewDayPass.aspx",
                            EmailConfig.DefaultImageUrlSendEmail, bookingId),
                        PhoneNumber = hotels.PhoneNumber,
                        BookedDate = booking.BookedDate.ToLosAngerlesTimeWithTimeZone(hotels.TimeZoneId).ToString(Constant.FullDateFormat),
                        //Access = booking.Access,
                        //BlackoutDays = !string.IsNullOrEmpty(booking.BlackoutDays)
                        //    ? string.Format("<tr><td style=\"text-align: right;\">Blackout Days:</td><td style=\"text-align: left; padding-left:7px;\">{0}</td></tr>",
                        //        booking.BlackoutDays)
                        //    : "",
                        ExpiredDate = booking.ExpiredDate.HasValue ? booking.ExpiredDate.Value.ToLosAngerlesTimeWithTimeZone(hotels.TimeZoneId).ToString(Constant.FullDateFormat) : string.Empty,
                        BookingCode = booking.BookingIdString,
                        PerTicketPrice = Helper.FormatPriceWithFixed(booking.MerchantPrice),
                        BookingTotal = Helper.FormatPriceWithFixed((booking.TotalPrice - booking.PayByCredit)),
                        CheckInPlace = hotels.CheckInPlace,
                        Tickets = booking.Quantity.ToString(),
                        MaxPerTicket = product.MaxGuest.ToString(),
                        HotelName = string.Format("{0} at {1}", product.ProductName, hotels.HotelName),
                        ViewDayPassString = "VIEW TICKET",
                        // Kid Allowed ?
                        MaxGuest = string.Format(Constant.MaxGuestText,
                            maxGuest,
                            maxGuest > 1 ? "s" : string.Empty,
                            product.ProductTypeString,
                            product.KidAllowedString),
                        FinePrintVisible = product.IsCheckedInRequired ? "hidden" : "visible",
                        BookingStatus = ((Enums.BookingStatus)booking.PassStatus).ToDescription()
                    };

                    try
                    {
                        var imageName = Helper.ReplaceLastOccurrence(product.ImageUrl, ".", "-ovl.");
                        emailInfo.HotelImage = string.Format("{0}",
                            new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), imageName).AbsoluteUri);

                        if (finePrint != null)
                        {
                            var strFinePrintDiscount = @"<tr>" +
                                                       "    <td colspan=\"2\">" +
                                                       "        <h4 style=\"font-weight: bold; text-align: center; font-size: 14px;margin: 0 auto 15px; font-color: 4A4A4A;\">PROMO APPLIED</h4>" +
                                                       "    </td>" +
                                                       "</tr>" +
                                                       "<tr>" +
                                                       "    <td colspan=\"2\">" +
                                                       "        <div style=\"margin-left:40px;margin-right:20px;margin-top:0;margin-bottom:10px;font-size:10px;max-width:500px;margin: 0 auto;\">" +
                                                       "            <span style=\"font-weight: bold; font-size: 10px; color: #11A752; text-transform: none;\">{0}</span><br/>" +
                                                       "            <span style=\"font-size:10px; font-weight:bold;color:#666;\">{1}</span> - {2}<br/>" +
                                                       "            {3}" +
                                                       "        </div>" +
                                                       "    </td>" +
                                                       "</tr>";
                            emailInfo.DiscountFinePrint = string.Format(strFinePrintDiscount, 
                                finePrint.DiscountName,
                                finePrint.Code,
                                string.Format("{0} OFF",
                                    finePrint.PromoType == (int)Enums.PromoType.Fixed
                                        ? Helper.FormatPriceWithFixed(finePrint.PercentOff)
                                        : finePrint.PercentOff + "%"),
                                finePrint.FinePrint);
                        }
                    }
                    catch (Exception) { }

                    if (!booking.TotalPrice.Equals(booking.MerchantPrice * booking.Quantity))
                    {
                        var discountPrice = booking.MerchantPrice * booking.Quantity + booking.TaxPrice - booking.TotalPrice;
                        emailInfo.Discount = string.Format(
                            "<tr><td style=\"text-align: left;\">Promo</td><td style=\"text-align: right; padding-left:7px;\">-{0}</td></tr>",
                            Helper.FormatPriceWithFixed(discountPrice));
                    }

                    if (!booking.TaxPrice.Equals(0))
                    {
                        emailInfo.Discount += string.Format(
                            "<tr><td style=\"text-align: left;\">Tax</td><td style=\"text-align: right; padding-left:7px;\">{0}</td></tr>",
                            Helper.FormatPriceWithFixed(booking.TaxPrice));
                    }

                    var quantity = booking.IsActiveSubscription ? booking.Quantity - 1 : booking.Quantity;
                    emailInfo.TotalTicketInfo = string.Format("{0} x {1}",
                        Helper.FormatPriceWithFixed(booking.MerchantPrice),
                        quantity == 1 ? "1 ticket" : quantity + " tickets");
                    emailInfo.TotalPriceInfo = Helper.FormatPriceWithFixed(booking.MerchantPrice * quantity);

                    if (booking.PayByCredit > 0)
                    {
                        emailInfo.Discount += string.Format(
                            "<tr><td style=\"text-align: left;\">Credit</td><td style=\"text-align: right; padding-left:7px;\">{0}</td></tr>",
                            Helper.FormatPriceWithFixed(booking.PayByCredit * -1));
                    }

                    try
                    {
                        emailInfo.EmailBccList = bookingRepository.GetBccEmailOfUser(product.HotelId, product.ProductType);

                        if (product.IsCheckedInRequired || booking.CheckinDate.HasValue)
                        {
                            emailInfo.CheckInDate = string.Format("{0:MMM dd, yyyy}", booking.CheckinDate);
                            emailInfo.CheckInText = "CHANGE";
                        }
                        else
                        {
                            emailInfo.CheckInDate = "Select Date";
                            emailInfo.CheckInText = "SELECT";
                        }

                        var checkInDate = booking.CheckinDate.HasValue
                            ? booking.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(hotels.TimeZoneId).Date
                            : DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(hotels.TimeZoneId);
                        var addOnProducts = bookingRepository.GetProductsAdOns(product.ProductId, checkInDate).Take(6);

                        string title = "<div style=\"text-align: center;\"><h2>Add to Your Visit</h2></div>";
                        emailInfo.AddOnString = GetAddOnString(addOnProducts.ToList(), booking.BrowsePassUrl, title);

                        // Calculate With Updated Booking
                        emailInfo.Updated = "";
                        emailInfo.UpdatedReceipt = "a receipt";
                        if (schedules.IsUpdated)
                        {
                            emailInfo.Updated = "[Updated] ";
                            emailInfo.UpdatedReceipt = "an updated receipt";

                            var bookingHistories = bookingRepository.GetBookingHistories(schedules.BookingHistoryId);

                            if (bookingHistories != null)
                            {
                                double totalPrice = bookingHistories.TotalPrice - booking.TotalPrice;
                                if (totalPrice < 0)
                                {
                                    emailInfo.Discount += string.Format(
                                        "<tr><td style=\"text-align: left;\">Charge</td><td style=\"text-align: right; padding-left:7px;\">{0}</td></tr>",
                                        Helper.FormatPriceWithFixed(Math.Abs(totalPrice)));
                                }
                            }
                        }

                        if (goldPassDiscount != null)
                        {
                            emailInfo.SubscriptionReminder = "<div style=\"background-color: #ecf8fc; text-align: center; padding: 12px 0; font-size: 12px;\">" +
                                                             "<b>REMINDER:</b> You'll be charged $20 if you don't show up." +
                                                             "</div>";
                        }
                        
                        var result = EmailHelper.EmailTemplateBookingConfirmation(emailInfo);

                        // Add Bcc Email to Support
                        // Comment this on Stage
                        EmailHelper.AddBookingsToBccEmail(ref emailInfo);

                        if (emailInfo.EmailBccList != null && emailInfo.EmailBccList.Any())
                        {
                            emailInfo.UserEmail = emailInfo.EmailBccList.First();
                            emailInfo.EmailBccList.RemoveAt(0);

                            // Add this when Redeemed will get this info to send Email to Admin's Hotel
                            var bookingConfirmationEmails = new BookingConfirmationHotels
                            {
                                UserEmail = emailInfo.UserEmail,
                                //BlackoutDays = emailInfo.BlackoutDays,
                                BookedDate = emailInfo.BookedDate,
                                BookingCode = emailInfo.BookingCode,
                                BookingId = booking.BookingId,
                                EmailBccList = string.Join(";", emailInfo.EmailBccList),
                                ExpiredDate = emailInfo.ExpiredDate,
                                FullName = emailInfo.FullName,
                                HotelName = hotels.HotelName,
                                RedeemedDate = string.Empty,
                                MaxPerTicket = string.Format("{0} adult{1}", product.MaxGuest,
                                    product.MaxGuest == 1 ? string.Empty : "s"),
                                Tickets = booking.Quantity.ToString()
                            };
                            bookingRepository.AddBookingConfirm(bookingConfirmationEmails);

                            // Alert to Admin's Hotel with Hotel Check-In Required
                            if (product.IsCheckedInRequired)
                            {
                                emailInfo.ProductType = Helper.GetStringPassByProductType(product.ProductType).ToLower()
                                    .Replace(" pass", "");
                                emailInfo.CheckInDate = booking.CheckinDate.HasValue
                                    ? booking.CheckinDate.Value.ToString(Constant.DateFormat)
                                    : "";
                                emailInfo.ProductName = product.ProductName;
                                emailInfo.HotelName = hotels.HotelName;
                                emailInfo.MaxPerTicket = string.Format("{0} adult{1}", product.MaxGuest,
                                    product.MaxGuest == 1 ? string.Empty : "s");
                                emailInfo.Tickets = booking.Quantity.ToString();
                                emailInfo.PerTicketPrice = Helper.FormatPriceWithFixed(booking.MerchantPrice);

                                emailInfo.Updated = string.Empty;
                                emailInfo.UpdatedReceipt = "a new";
                                if (schedules.IsUpdated)
                                {
                                    emailInfo.Updated = "[Updated] ";
                                    emailInfo.UpdatedReceipt = "an updated";
                                }

                                var result2 = EmailHelper.EmailTemplateBookingAlertOfHotel(emailInfo);

                                var logSchedule = new Logs
                                {
                                    LogKey = string.Format("Send Email Alert from Schedule {0}", schedules.IsUpdated ? " - updated" : string.Empty),
                                    UpdatedContent = string.Format("{0} - {1} - {2} - {3}",
                                        emailInfo.UserEmail,
                                        booking.BookingIdString,
                                        result2.Result,
                                        JsonConvert.SerializeObject(emailInfo, CustomSettings.SerializerSettings())),
                                    UpdatedDate = DateTime.UtcNow,
                                    UpdatedBy = 1
                                };
                                bookingRepository.AddLog(logSchedule);
                            }
                        }

                        booking.IsEmailConfirmSend = true;
                        bookingRepository.UpdateConfirmSend(booking);

                        schedules.Status = (int)Enums.ScheduleType.Complete;
                        schedules.CompleteDate = DateTime.UtcNow;
                        schedules.LastRun = DateTime.UtcNow;
                        bookingRepository.UpdateSchedule(schedules);

                        if (!string.IsNullOrEmpty(result.Result))
                        {
                            string json = JsonConvert.SerializeObject(emailInfo, CustomSettings.SerializerSettings());
                            var log = new Logs
                            {
                                LogKey = string.Format("EmailConfirm{0}", schedules.IsUpdated ? " - updated" : string.Empty),
                                UpdatedContent = string.Format("{0} - {1} - {2} - {3}", emailInfo.UserEmail, booking.BookingIdString, result.Result, json),
                                UpdatedDate = DateTime.UtcNow,
                                UpdatedBy = 1
                            };
                            bookingRepository.AddLog(log);
                        }
                    }
                    catch (Exception ex)
                    {
                        string json = JsonConvert.SerializeObject(emailInfo, CustomSettings.SerializerSettings());
                        var log = new Logs
                        {
                            LogKey = "BccError",
                            UpdatedContent = string.Format("{0} - {1} - {2} - {3} - {4}", emailInfo.UserEmail, booking.BookingIdString, ex.Message, ex.StackTrace, json),
                            UpdatedDate = DateTime.UtcNow,
                            UpdatedBy = 1
                        };
                        bookingRepository.AddLog(log);
                    }
                }
            }
        }

        private void ReCalculateExpiredDate(Schedules schedules)
        {
            // Expired Bookings of Products
            using (var bookingRepository = new BookingRepository())
            {
                // Booking Has Expired Date but Status is <> Expired
                var bookingsFail = bookingRepository.GetBookingsExpired();

                try
                {
                    if (bookingsFail.Any())
                    {
                        bookingsFail.ForEach(booking =>
                        {
                            booking.PassStatus = (int)Enums.BookingStatus.Expired;

                            var bookingDiscount = bookingRepository.GetGoldPassDiscountByBookingId(booking.BookingId);
                            if (bookingDiscount != null)
                            {
                                try
                                {
                                    var products = bookingRepository.GetProduct(booking.ProductId);
                                    var hotels = bookingRepository.GetHotel(products.HotelId);
                                    var customerInfos = bookingRepository.GetCustomer(booking.CustomerId);
                                    string description = string.Format(
                                        "\"No show\" fee for {0} at {1} {2} {3}",
                                        products.ProductName,
                                        hotels.HotelName,
                                        booking.BookingIdString,
                                        booking.CheckinDate.HasValue
                                            ? booking.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(hotels.TimeZoneId).ToString(Constant.DateFormat)
                                            : string.Empty);
                                    try
                                    {
                                        // https://trello.com/c/o7ryHh3f/114-user-disable-20-no-show-charge-temporarily
                                        //var charge = CreateCharges(20, customerInfos.StripeCustomerId, description);
                                    }
                                    catch (Exception ex) { }

                                    var schedulesNoShow = new Schedules
                                    {
                                        BookingId = booking.BookingId,
                                        Name = "EmailGoldPassNoShow",
                                        ScheduleSendType = (int)Enums.ScheduleSendType.IsEmailGoldPassNoShow,
                                        Status = (int)Enums.ScheduleType.NotRun
                                    };
                                    bookingRepository.AddSchedule(schedulesNoShow);
                                }
                                catch (Exception ex)
                                {
                                    var chargeError = new Logs
                                    {
                                        LogKey = "AutoCharge-No-Shows-Error",
                                        UpdatedContent = string.Format("{0} - {1}", booking.BookingCode, ex.Message),
                                        UpdatedDate = DateTime.UtcNow,
                                        UpdatedBy = 1
                                    };
                                    bookingRepository.AddLog(chargeError);
                                }
                            }
                        });

                        bookingRepository.UpdateStatus(bookingsFail);

                        var log = new Logs
                        {
                            LogKey = "UpdateExpiredBooking",
                            UpdatedContent = string.Format("{0}",
                                string.Join(" - ", bookingsFail.Select(b => b.BookingCode))),
                            UpdatedDate = DateTime.UtcNow,
                            UpdatedBy = 1
                        };
                        bookingRepository.AddLog(log);
                    }

                    // Update Status of Schedules
                    schedules.CompleteDate = DateTime.UtcNow;
                    schedules.LastRun = DateTime.UtcNow;
                    bookingRepository.UpdateSchedule(schedules);
                }
                catch (Exception ex)
                {
                    var log = new Logs
                    {
                        LogKey = "UpdateExpiredBooking-Error",
                        UpdatedContent = string.Format("{0} - {1}",
                            string.Join(" - ", bookingsFail.Select(b => b.BookingCode)), ex.Message),
                        UpdatedDate = DateTime.UtcNow,
                        UpdatedBy = 1
                    };
                    bookingRepository.AddLog(log);
                }
            }
        }

        private void ReCalculateSubscriptionExpired()
        {
            using (var bookingRepository = new BookingRepository())
            {
                try
                {
                    bookingRepository.ExpiredSubscriptions();
                }
                catch (Exception ex)
                {
                    var log = new Logs
                    {
                        LogKey = "UpdateExpiredSubscriptionBooking-Error",
                        UpdatedContent = string.Format("{0}", ex.Message),
                        UpdatedDate = DateTime.UtcNow,
                        UpdatedBy = 1
                    };
                    bookingRepository.AddLog(log);
                }
            }
        }

        private void DeliveryGiftCard()
        {
            using (var giftCardBookingRepository = new GiftCardBookingRepository())
            {
                var giftCardBookingsList = giftCardBookingRepository.GetGiftCardBookingsDelivery().ToList();
                if (giftCardBookingsList.Any())
                {
                    giftCardBookingsList.ForEach(bookings =>
                    {
                        try
                        {
                            var customerInfos = giftCardBookingRepository.GetCustomer(bookings.CustomerId);
                            var giftCards = giftCardBookingRepository.GetGiftCard(bookings.GiftCardId);

                            var emailInfo = new EmailDeliveryGiftCardInfo
                            {
                                UserEmail = bookings.RecipientEmail,
                                FullName = string.Format("{0} {1}", customerInfos.FirstName, customerInfos.LastName),
                                UrlImage = AppConfiguration.CdnImageUrlDefault,
                                Message = bookings.Message,
                                Amount = Helper.FormatPrice(giftCards.Amount),
                                GiftCode = giftCards.Code,
                                UrlRedeem = string.Format("{0}/credits", EmailConfig.DefaultImageUrlSendEmail)
                            };

                            var result = EmailHelper.EmailDeliveryeGiftCard(emailInfo);

                            if (!string.IsNullOrEmpty(result.Result))
                            {
                                string json = JsonConvert.SerializeObject(emailInfo, CustomSettings.SerializerSettings());
                                var log = new Logs
                                {
                                    LogKey = "DeliveryGiftCard",
                                    UpdatedContent = string.Format("{0} - giftCardBookingId: {1} - {2} - {3}", emailInfo.UserEmail, bookings.Id, result.Result, json),
                                    UpdatedDate = DateTime.UtcNow,
                                    UpdatedBy = 1
                                };
                                giftCardBookingRepository.AddLog(log);
                            }

                            bookings.IsSend = true;
                            bookings.SendDate = DateTime.UtcNow;
                            bookings.LastUpdatedDate = DateTime.UtcNow;
                            bookings.LastUpdatedBy = 1;
                        }
                        catch (Exception ex)
                        {
                            var log = new Logs
                            {
                                LogKey = "DeliveryGiftCard-Error",
                                UpdatedContent = string.Format("giftCardBookingId: {0} - {1}", bookings.Id, ex.Message),
                                UpdatedDate = DateTime.UtcNow,
                                UpdatedBy = 1
                            };
                            giftCardBookingRepository.AddLog(log);
                        }

                        giftCardBookingRepository.Update(giftCardBookingsList);
                    });
                }
            }
        }

        private void SendEmailRefund(Schedules schedules)
        {
            using (var bookingRepository = new BookingRepository())
            {
                int bookingId = schedules.BookingId.HasValue ? schedules.BookingId.Value : 0;

                var booking = bookingRepository.GetBookings(bookingId);
                if (booking == null)
                {
                    var log = new Logs
                    {
                        LogKey = "EmailRefund - Error - BookingIdNull",
                        UpdatedContent = string.Format("BookingIdNull - {0}", schedules.BookingId),
                        UpdatedDate = DateTime.UtcNow,
                        UpdatedBy = 1
                    };
                    bookingRepository.AddLog(log);
                    return;
                }
                var products = bookingRepository.GetProduct(booking.ProductId);
                var hotels = bookingRepository.GetHotel(products.HotelId);
                var customerInfos = bookingRepository.GetCustomer(booking.CustomerId);

                var bookingHistories = bookingRepository.GetBookingHistories(schedules.BookingHistoryId);

                DateTime refundedDate = booking.PassStatus == (int) Enums.BookingStatus.Refunded
                    ? (booking.CancelDated.HasValue ? booking.CancelDated.Value : bookingHistories.UpdatedDate)
                    : bookingHistories.UpdatedDate;
                double refundedPrice = booking.PassStatus == (int) Enums.BookingStatus.Refunded
                    ? booking.TotalPrice
                    : bookingHistories.TotalPrice - booking.TotalPrice;

                var emailInfo = new EmailRefundInfo
                {
                    UserEmail = customerInfos.EmailAddress,
                    HotelName = hotels.HotelName,
                    Neightborhood = hotels.Neighborhood,
                    FirstName = customerInfos.FirstName,
                    LastName = customerInfos.LastName,
                    ProductName = products.ProductName,
                    BookedDate = booking.BookedDate.ToLosAngerlesTimeWithTimeZone(hotels.TimeZoneId).ToString(Constant.FullDateFormat),
                    BookingCode = booking.BookingIdString,
                    Tickets = booking.Quantity.ToString(),
                    RefundDate = refundedDate.ToLosAngerlesTimeWithTimeZone(hotels.TimeZoneId).ToString(Constant.FullDateFormat),
                    RefundAmount = Helper.FormatPrice(refundedPrice),
                    UrlViewDayPass = string.Format("{0}/{1}/ViewDayPass.aspx", 
                        EmailConfig.DefaultImageUrlSendEmail,
                        bookingId)
                };

                double refundCreditAmount = 0;
                if (schedules.CustomerCreditId > 0)
                {
                    var customerCredit = bookingRepository.GetCustomerCreditLogs(schedules.CustomerCreditId);
                    if (customerCredit != null)
                    {
                        refundCreditAmount = customerCredit.Amount;
                    }
                }

                // Refund to Dayaxe Credit 
                if (bookingHistories.PaymentType == (int) Enums.PaymentType.DayAxeCredit &&
                    string.IsNullOrEmpty(bookingHistories.StripeCardString))
                {
                    refundCreditAmount = refundedPrice;
                }

                if (!refundCreditAmount.Equals(0))
                {
                    emailInfo.RefundCreditAmount =
                        string.Format(
                            "<tr><td style=\"text-align: right;\">Pay To:</td><td style=\"text-align: left; padding-left:7px;\">{0} DayAxe Credit</td></tr>",
                            Helper.FormatPrice(refundCreditAmount));
                }

                // Refund to Stripe
                if (bookingHistories.PaymentType == (byte)Enums.PaymentType.Stripe ||
                    (!string.IsNullOrEmpty(bookingHistories.StripeCardString) && schedules.CustomerCreditId == 0))
                {
                    double refundStripeAmount = refundedPrice;
                    if (!string.IsNullOrEmpty(emailInfo.RefundCreditAmount))
                    {
                        refundStripeAmount = refundedPrice - refundCreditAmount;
                    }

                    if (refundStripeAmount > 0)
                    {
                        emailInfo.RefundStripeAmount =
                            string.Format(
                                "<tr><td style=\"text-align: right;\">Pay To:</td><td style=\"text-align: left; padding-left:7px;\">{0} {1}</td></tr>",
                                Helper.FormatPrice(refundStripeAmount),
                                bookingHistories.StripeCardString);
                    }
                }

                string json = JsonConvert.SerializeObject(emailInfo, CustomSettings.SerializerSettings());

                try
                {
                    var result = EmailHelper.EmailRefund(emailInfo);
                    schedules.Status = (int)Enums.ScheduleType.Complete;
                    schedules.CompleteDate = DateTime.UtcNow;
                    schedules.LastRun = DateTime.UtcNow;
                    bookingRepository.UpdateSchedule(schedules);

                    if (!string.IsNullOrEmpty(result.Result))
                    {
                        var log = new Logs
                        {
                            LogKey = "EmailRefund",
                            UpdatedContent = string.Format("{0} - {1} - {2} - {3}",
                                emailInfo.UserEmail,
                                booking.BookingIdString,
                                result.Result,
                                json),
                            UpdatedDate = DateTime.UtcNow,
                            UpdatedBy = 1
                        };
                        bookingRepository.AddLog(log);
                    }
                }
                catch (Exception ex)
                {
                    var log = new Logs
                    {
                        LogKey = "EmailRefund - Error",
                        UpdatedContent = string.Format("{0} - {1} - {2} - {3} - {4}", 
                            emailInfo.UserEmail, 
                            booking.BookingIdString, 
                            ex.Message, 
                            ex.StackTrace, 
                            json),
                        UpdatedDate = DateTime.UtcNow,
                        UpdatedBy = 1
                    };
                    bookingRepository.AddLog(log);
                }
            }
        }

        private void SendBookingAlert(Schedules schedules)
        {
            using (var bookingRepository = new BookingRepository())
            {
                var dateRun = EmailConfig.ReminderAlertTime;
                var bookings = bookingRepository.GetBookingsReminder(dateRun).ToList();
                if (bookings.Any())
                {
                    bookings.ForEach(booking =>
                    {
                        try
                        {
                            var products = bookingRepository.GetProduct(booking.ProductId);
                            var hotels = bookingRepository.GetHotel(products.HotelId);
                            var customerInfos = bookingRepository.GetCustomer(booking.CustomerId);
                            var goldPassDiscount = bookingRepository.GetGoldPassDiscountByBookingId(booking.BookingId);

                            var emailInfo = new EmailCheckInReminder
                            {
                                UserEmail = customerInfos.EmailAddress,
                                HotelName = hotels.HotelName,
                                ProductName = products.ProductName,
                                FirstName = customerInfos.FirstName,
                                CheckInDate = booking.CheckinDate.HasValue
                                    ? booking.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(hotels.TimeZoneId).ToString(Constant.DateFormat)
                                    : string.Empty,
                                Rating = hotels.TripAdvisorRating ?? 4,
                                MaxGuest = products.MaxGuest.ToString(),
                                Tickets = booking.Quantity.ToString(),
                                CheckInPlace = hotels.CheckInPlace,
                                Neighborhood = string.Format("{0}, {1}", hotels.Neighborhood, hotels.City),
                                UrlViewDayPass = string.Format("{0}/{1}/ViewDayPass.aspx",
                                    EmailConfig.DefaultImageUrlSendEmail, booking.BookingId)
                            };

                            try
                            {
                                var imageName = Helper.ReplaceLastOccurrence(products.ImageUrl, ".", "-ovl.");
                                emailInfo.ImageUrl = string.Format("{0}", new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), imageName).AbsoluteUri);
                            }
                            catch (Exception)
                            {
                            }

                            if (booking.CheckinDate.HasValue)
                            {
                                var addOnProducts = bookingRepository.GetProductsAdOns(products.ProductId, booking.CheckinDate.Value);

                                string title = "<div style=\"text-align: center;\"><h2>Add These to Your Trip</h2></div>";
                                emailInfo.AddOnString = GetAddOnString(addOnProducts.ToList(), booking.BrowsePassUrl, title);
                            }

                            if (goldPassDiscount != null)
                            {
                                emailInfo.SubscriptionReminder = "<div style=\"background-color: #ecf8fc; text-align: center; padding: 12px 0; font-size: 12px;\">" +
                                                                 "<b>REMINDER:</b> You'll be charged $20 if you don't show up." +
                                                                 "</div>";
                            }

                            var result = EmailHelper.EmailCheckInReminder(emailInfo);

                            var log = new Logs
                            {
                                LogKey = "EmailReminder",
                                UpdatedContent = string.Format("{0} - {1} - {2:MMM dd, hh:mm tt} - {3} - {4}", 
                                    emailInfo.UserEmail, 
                                    booking.BookingIdString,
                                    dateRun, 
                                    result.Result, 
                                    booking.BookingIdString),
                                UpdatedDate = DateTime.UtcNow,
                                UpdatedBy = 1
                            };
                            bookingRepository.AddLog(log);
                        }
                        catch (Exception ex)
                        {
                            string json = JsonConvert.SerializeObject(string.Join("|", bookings.Select(b => b.BookingId).ToList()), CustomSettings.SerializerSettings());
                            var log = new Logs
                            {
                                LogKey = "EmailReminderError",
                                UpdatedContent = string.Format("{0} - {1} - {2} - {3}", 
                                    json, 
                                    ex.Message, 
                                    ex.StackTrace, 
                                    booking.BookingIdString),
                                UpdatedDate = DateTime.UtcNow,
                                UpdatedBy = 1
                            };
                            bookingRepository.AddLog(log);
                        }
                    });
                }

                schedules.CompleteDate = DateTime.UtcNow;
                schedules.LastRun = DateTime.UtcNow;
                bookingRepository.UpdateSchedule(schedules);
            }

            Thread.Sleep(5000);
        }

        private void SendEmailSurvey(Schedules schedules)
        {
            using (var surveyRepository = new SurveyRepository())
            {
                try
                {
                    int bookingId = schedules.BookingId.HasValue ? schedules.BookingId.Value : 0;
                    var surveys = surveyRepository.GetSurvey(bookingId);
                    var booking = surveyRepository.GetBookings(surveys.BookingId);
                    if (booking.PassStatus == (int) Enums.BookingStatus.Redeemed)
                    {
                        var product = surveyRepository.GetProduct(booking.ProductId);
                        var hotel = surveyRepository.GetHotel(product.HotelId);
                        var customerInfo = surveyRepository.GetCustomer(booking.CustomerId);
                        var survey = surveyRepository.GetSurvey(bookingId);
                        string surveyCode = survey != null ? survey.Code : string.Empty;

                        var relativeUrl = string.Format("{0}/{1}",
                            booking.BrowsePassUrl.Substring(0, booking.BrowsePassUrl.LastIndexOf('/')),
                            surveyCode);
                        var url = new Uri(new Uri(EmailConfig.DefaultImageUrlSendEmail), relativeUrl).AbsoluteUri;

                        string hotelName = string.Format("{0} at {1}", product.ProductName, hotel.HotelName);

                        var result = EmailHelper.EmailTemplateSurvey(customerInfo.EmailAddress, hotelName,
                            hotel.Neighborhood,
                            customerInfo.FirstName, url);

                        var bookingConfirmationHotel = surveyRepository.GetBookingConfirmationHotels(booking.BookingId);

                        var emailInfo = new EmailInfo
                        {
                            UserEmail = bookingConfirmationHotel.UserEmail,
                            //BlackoutDays = bookingConfirmationHotel.BlackoutDays,
                            BookedDate = bookingConfirmationHotel.BookedDate,
                            BookingCode = bookingConfirmationHotel.BookingCode,
                            EmailBccList = bookingConfirmationHotel.EmailBccList.Split(';').ToList(),
                            ExpiredDate = bookingConfirmationHotel.ExpiredDate,
                            HotelName = bookingConfirmationHotel.HotelName,
                            RedeemedDate = booking.RedeemedDate.HasValue ? booking.RedeemedDate.Value.ToLosAngerlesTimeWithTimeZone(hotel.TimeZoneId)
                                .ToString(Constant.FullDateFormat) : string.Empty,
                            ProductType =
                                Helper.GetStringPassByProductType(product.ProductType)
                                    .ToLower()
                                    .Replace(" pass", ""),
                            MaxPerTicket = bookingConfirmationHotel.MaxPerTicket,
                            Tickets = bookingConfirmationHotel.Tickets,
                            ProductName = product.ProductName,
                            PerTicketPrice = Helper.FormatPriceWithFixed(booking.MerchantPrice)
                        };

                        var result1 = EmailHelper.EmailTemplateBookingConfirmationOfHotel(emailInfo,
                            bookingConfirmationHotel.FullName);

                        var log = new Logs
                        {
                            LogKey = "EmailSurvey",
                            UpdatedContent = string.Format("{0} - {1} - result: {2} - result1: {3}",
                                customerInfo.EmailAddress,
                                booking.BookingIdString,
                                result.Result,
                                result1.Result),
                            UpdatedDate = DateTime.UtcNow,
                            UpdatedBy = 1
                        };
                        surveyRepository.AddLog(log);
                    }
                }
                catch (Exception ex)
                {
                    string json = JsonConvert.SerializeObject(schedules, CustomSettings.SerializerSettings());
                    var log = new Logs
                    {
                        LogKey = "EmailSurveyError",
                        UpdatedContent = string.Format("{0} - {1} - {2} - {3}", schedules.BookingId, ex.Message, ex.StackTrace, json),
                        UpdatedDate = DateTime.UtcNow,
                        UpdatedBy = 1
                    };
                    surveyRepository.AddLog(log);
                }

                schedules.CompleteDate = DateTime.UtcNow;
                schedules.LastRun = DateTime.UtcNow;
                schedules.Status = (int) Enums.ScheduleType.Complete;
                surveyRepository.UpdateSchedule(schedules);
            }
        }

        private void SendAddOnNotification(Schedules schedules)
        {
            using (var bookingRepository = new BookingRepository())
            {
                int bookingId = schedules.BookingId.HasValue ? schedules.BookingId.Value : 0;
                bool isSend = true;

                // Refresh Hotel Data
                var booking = bookingRepository.GetBookings(bookingId);

                // - Check If Current Booking is Capture 
                // - Wait 2 minute for user select Upgrade or not
                if (booking.PassStatus == (int)Enums.BookingStatus.Capture &&
                    booking.BookedDate.AddMinutes(2) < DateTime.UtcNow)
                {
                    return;
                }

                var product = bookingRepository.GetProduct(booking.ProductId);
                var hotels = bookingRepository.GetHotel(product.HotelId);
                var customerInfos = bookingRepository.GetCustomer(booking.CustomerId);

                var emailInfo = new EmailAddOnNotificationInfo
                {
                    FirstName = customerInfos.FirstName,
                    UserEmail = customerInfos.EmailAddress,
                    HotelName = string.Format("{0} at {1}", product.ProductName, hotels.HotelName),
                    CheckInDate = string.Format("{0:MMM dd, yyyy}", booking.CheckinDate)
                };

                try
                {
                    if (booking.CheckinDate.HasValue &&
                        DateTime.UtcNow.AddDays(1).Date >= booking.CheckinDate.Value.Date)
                    {
                        isSend = false;
                    }

                    if (isSend)
                    {
                        var checkInDate = booking.CheckinDate.HasValue
                            ? booking.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(hotels.TimeZoneId).Date
                            : DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(hotels.TimeZoneId);
                        var addOnProducts = bookingRepository.GetProductsAdOns(product.ProductId, checkInDate).ToList();
                        var logString = string.Format("{0} - {1} - No Add-Ons Available On {2:MMM dd, yyyy}",
                            emailInfo.UserEmail,
                            booking.BookingIdString, 
                            checkInDate);

                        if (addOnProducts.Any())
                        {
                            string title = "<div style=\"text-align: center;\"><h2>Add These to Your Trip</h2></div>";
                            emailInfo.AddOnString = GetAddOnString(addOnProducts, booking.BrowsePassUrl, title);

                            var tomorrow = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(hotels.TimeZoneId).Date.AddDays(1);
                            // 9 - 9 AM PDT
                            // 7 - 7 GMT Time with PDT
                            tomorrow = tomorrow.AddHours(9 + 7);

                            emailInfo.SentAt = Convert.ToInt64(tomorrow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);

                            var result = EmailHelper.EmailAddOnNotification(emailInfo);

                            if (!string.IsNullOrEmpty(result.Result))
                            {
                                string json =
                                    JsonConvert.SerializeObject(emailInfo, CustomSettings.SerializerSettings());
                                logString = string.Format("{0} - {1} - {2} - {3}",
                                    emailInfo.UserEmail,
                                    booking.BookingIdString,
                                    result.Result,
                                    json);
                            }
                        }

                        var log = new Logs
                        {
                            LogKey = "AddOnNotification",
                            UpdatedContent = logString,
                            UpdatedDate = DateTime.UtcNow,
                            UpdatedBy = 1
                        };
                        bookingRepository.AddLog(log);
                    }

                    schedules.Status = (int)Enums.ScheduleType.Complete;
                    schedules.CompleteDate = DateTime.UtcNow;
                    schedules.LastRun = DateTime.UtcNow;
                    bookingRepository.UpdateSchedule(schedules);
                }
                catch (Exception ex)
                {
                    string json = JsonConvert.SerializeObject(emailInfo, CustomSettings.SerializerSettings());
                    var log = new Logs
                    {
                        LogKey = "AddOnNotificationError",
                        UpdatedContent = string.Format("{0} - {1} - {2} - {3} - {4}", 
                            emailInfo.UserEmail, 
                            booking.BookingId, 
                            ex.Message, 
                            ex.StackTrace, 
                            json),
                        UpdatedDate = DateTime.UtcNow,
                        UpdatedBy = 1
                    };
                    bookingRepository.AddLog(log);
                }
            }
        }

        private void SendAddOnNotificationRedmeption(Schedules schedules)
        {
            using (var bookingRepository = new BookingRepository())
            {
                int bookingId = schedules.BookingId.HasValue ? schedules.BookingId.Value : 0;

                // Refresh Hotel Data
                var booking = bookingRepository.GetBookings(bookingId);
                var product = bookingRepository.GetProduct(booking.ProductId);
                var hotels = bookingRepository.GetHotel(product.HotelId);
                var customerInfos = bookingRepository.GetCustomer(booking.CustomerId);

                var emailInfo = new EmailAddOnNotificationInfo
                {
                    FirstName = customerInfos.FirstName,
                    UserEmail = customerInfos.EmailAddress,
                    HotelName = string.Format("{0} at {1}", product.ProductName, hotels.HotelName),
                    CheckInDate = string.Format("{0:MMM dd, yyyy}", booking.CheckinDate)
                };

                try
                {
                    var checkInDate = booking.CheckinDate.HasValue
                        ? booking.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(hotels.TimeZoneId).Date
                        : DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(hotels.TimeZoneId);
                    var addOnProducts = bookingRepository.GetProductsAdOns(product.ProductId, checkInDate).ToList();

                    if (addOnProducts.Any())
                    {
                        string title = "<div style=\"text-align: center;\"><h2>Add These to Your Trip</h2></div>";
                        emailInfo.AddOnString = GetAddOnString(addOnProducts, booking.BrowsePassUrl, title);

                        var result = EmailHelper.EmailAddOnNotificationRedemption(emailInfo);

                        if (!string.IsNullOrEmpty(result.Result))
                        {
                            string json = JsonConvert.SerializeObject(emailInfo, CustomSettings.SerializerSettings());
                            var log = new Logs
                            {
                                LogKey = "AddOnNotificationRedmemption",
                                UpdatedContent = string.Format("{0} - {1} - {2} - {3}",
                                    emailInfo.UserEmail,
                                    booking.BookingIdString,
                                    result.Result,
                                    json),
                                UpdatedDate = DateTime.UtcNow,
                                UpdatedBy = 1
                            };
                            bookingRepository.AddLog(log);
                        }
                    }

                    schedules.Status = (int)Enums.ScheduleType.Complete;
                    schedules.CompleteDate = DateTime.UtcNow;
                    schedules.LastRun = DateTime.UtcNow;
                    bookingRepository.UpdateSchedule(schedules);
                }
                catch (Exception ex)
                {
                    string json = JsonConvert.SerializeObject(emailInfo, CustomSettings.SerializerSettings());
                    var log = new Logs
                    {
                        LogKey = "AddOnNotificationRedmemptionError",
                        UpdatedContent = string.Format("{0} - {1} - {2} - {3} - {4}", 
                            emailInfo.UserEmail, 
                            booking.BookingIdString, 
                            ex.Message, 
                            ex.StackTrace, 
                            json),
                        UpdatedDate = DateTime.UtcNow,
                        UpdatedBy = 1
                    };
                    bookingRepository.AddLog(log);
                }
            }
        }

        private void SendTicketOpenedUp(Schedules schedules)
        {
            using (var bookingRepository = new BookingRepository())
            {
                try
                {
                    var productWaitingList = bookingRepository.GetProductWaitingList(schedules.ProductWaitingListId);
                    var product = bookingRepository.GetProduct(productWaitingList.ProductId);
                    var hotel = bookingRepository.GetHotel(product.HotelId);
                    var market = bookingRepository.GetMarketByHotelId(product.HotelId);
                    var customerInfos = bookingRepository.GetCustomerByEmailAddress(productWaitingList.EmailAddress);

                    string url = string.Format("/{0}/{1}/{2}/{3}/{4}",
                        market != null ? market.MarketCode.ToLower() : "socal",
                        hotel.CityUrl,
                        hotel.HotelNameUrl,
                        product.ProductNameUrl,
                        product.ProductId);

                    var price = bookingRepository.GetDailyPriceByDate(product, productWaitingList.CheckInDate);

                    var emailInfo = new EmailTicketOpenedUp
                    {
                        UserEmail = productWaitingList.EmailAddress,
                        ProductName = product.ProductName,
                        FirstName = customerInfos != null ? customerInfos.FirstName : string.Empty,
                        CheckInDate = productWaitingList.CheckInDate.ToString(Constant.DateFormat),
                        HotelName = hotel.HotelName,
                        Location = string.Format("{0}, {1}", hotel.Neighborhood, hotel.City),
                        UrlProductUrl = new Uri(new Uri(AppConfiguration.DefaultImageUrlSendEmail), url).AbsoluteUri,
                        Price = Helper.FormatPrice(price)
                    };

                    var result = EmailHelper.EmailTicketOpenedUp(emailInfo);
                    string json = JsonConvert.SerializeObject(emailInfo, CustomSettings.SerializerSettings());
                    var log = new Logs
                    {
                        LogKey = "SendTicketOpenedUp",
                        UpdatedContent = string.Format("{0} - {1} - {2} - {3} - {4}",
                            emailInfo.UserEmail,
                            hotel.HotelName,
                            product.ProductName,
                            result.Result,
                            json),
                        UpdatedDate = DateTime.UtcNow,
                        UpdatedBy = 1
                    };
                    bookingRepository.AddLog(log);

                    schedules.Status = (int)Enums.ScheduleType.Complete;
                    schedules.CompleteDate = DateTime.UtcNow;
                    schedules.LastRun = DateTime.UtcNow;
                    bookingRepository.UpdateSchedule(schedules);
                }
                catch (Exception ex)
                {
                    var log = new Logs
                    {
                        LogKey = "SendTicketOpenedUpError",
                        UpdatedContent = string.Format("ProductWaitingListId: {0} - ScheduleId: {1} - {2} - {3}",
                            schedules.ProductWaitingListId,
                            schedules.Id,
                            ex.Message,
                            ex.StackTrace),
                        UpdatedDate = DateTime.UtcNow,
                        UpdatedBy = 1
                    };
                    bookingRepository.AddLog(log);
                }
            }
        }

        private void SendNotifiedSurvey(Schedules schedules)
        {
            using (var bookingRepository = new BookingRepository())
            {
                try
                {
                    long bookingId = schedules.BookingId.HasValue ? schedules.BookingId.Value : 0;
                    var surveys = bookingRepository.GetSurvey(bookingId);
                    var bookings = bookingRepository.GetBookings(bookingId);
                    var product = bookingRepository.GetProduct(bookings.ProductId);
                    var hotel = bookingRepository.GetHotel(product.HotelId);
                    var customerInfos = bookingRepository.GetCustomer(bookings.CustomerId);

                    var emailAddress = bookingRepository.GetEmailAdminOfHotel(hotel.HotelId);

                    double totalSpend = 0;
                    if (surveys.FoodAndDrinkPrice.HasValue)
                    {
                        totalSpend += surveys.FoodAndDrinkPrice.Value;
                    }

                    if (surveys.SpaServicePrice.HasValue)
                    {
                        totalSpend += surveys.SpaServicePrice.Value;
                    }

                    if (surveys.AdditionalServicePrice.HasValue)
                    {
                        totalSpend += surveys.AdditionalServicePrice.Value;
                    }

                    if (surveys.IsPayForParking)
                    {
                        totalSpend += 20;
                    }

                    var emailInfo = new EmailNotifiedSurvey
                    {
                        UserEmail = emailAddress.Key,
                        EmailBccList = emailAddress.Value,
                        HotelName = hotel.HotelName,
                        ProductName = product.ProductName,
                        Quantity = bookings.Quantity.ToString(),
                        CustomerFullName = customerInfos != null ? string.Format("{0} {1}", customerInfos.FirstName, customerInfos.LastName) : string.Empty,
                        CheckInDate = bookings.CheckinDate.HasValue ? bookings.CheckinDate.Value.ToString(Constant.DateFormat) : string.Empty,
                        Rating = surveys.Rating ?? 4,
                        RateCommend = surveys.RateCommend,
                        TotalSpend = Helper.FormatPrice(totalSpend)
                        //UrlSurveyAdmin = new Uri(new Uri(AppConfiguration.DefaultImageUrlSendEmail), url).AbsoluteUri
                    };

                    if (emailInfo.EmailBccList == null || emailInfo.EmailBccList.Count == 0)
                    {
                        emailInfo.EmailBccList = new List<string>
                        {
                            EmailConfig.DefaultSendFromEmail
                        };
                    }
                    else
                    {
                        emailInfo.EmailBccList.Add(EmailConfig.DefaultSendFromEmail);
                    }

                    emailInfo.EmailBccList = emailInfo.EmailBccList.Distinct().ToList();

                    var result = EmailHelper.EmailNotifiedSurveyHotel(emailInfo);

                    string json = JsonConvert.SerializeObject(emailInfo, CustomSettings.SerializerSettings());
                    var log = new Logs
                    {
                        LogKey = "SendNotifiedSurvey",
                        UpdatedContent = string.Format("{0} - {1} - {2} - {3} - {4}",
                            string.Join("-", emailInfo.UserEmail),
                            hotel.HotelName,
                            product.ProductName,
                            result.Result,
                            json),
                        UpdatedDate = DateTime.UtcNow,
                        UpdatedBy = 1
                    };
                    bookingRepository.AddLog(log);

                    try
                    {
                        if (surveys.Rating.HasValue && surveys.Rating.Value < 5)
                        {
                            emailInfo.UserEmail = new List<string> {EmailConfig.HelpEmail};
                            emailInfo.EmailBccList = new List<string>();
                            emailInfo.ReplyToEmail = customerInfos != null ? customerInfos.EmailAddress : string.Empty;
                            emailInfo.ReplyToName = customerInfos != null
                                ? string.Format("{0} {1}", customerInfos.FirstName, customerInfos.LastName)
                                : string.Empty;
                            var result2 = EmailHelper.EmailNotifiedSurvey(emailInfo);
                        }
                    }
                    catch (Exception) { }

                    schedules.Status = (int)Enums.ScheduleType.Complete;
                    schedules.CompleteDate = DateTime.UtcNow;
                    schedules.LastRun = DateTime.UtcNow;
                    bookingRepository.UpdateSchedule(schedules);
                }
                catch (Exception ex)
                {
                    var log = new Logs
                    {
                        LogKey = "SendTicketOpenedUpError",
                        UpdatedContent = string.Format("ProductWaitingListId: {0} - ScheduleId: {1} - {2} - {3}",
                            schedules.ProductWaitingListId,
                            schedules.Id,
                            ex.Message,
                            ex.StackTrace),
                        UpdatedDate = DateTime.UtcNow,
                        UpdatedBy = 1
                    };
                    bookingRepository.AddLog(log);
                }
            }
        }

        private void SendEmailMonthlySalesReport(Schedules schedules)
        {
            using (var bookingRepository = new BookingRepository())
            {
                try
                {
                    var hotels = bookingRepository.HotelList
                        .Where(h => h.IsActive && !h.IsDelete && h.IsPublished && !string.IsNullOrEmpty(h.ReportSubscribers))
                        .ToList();

                    hotels.ForEach(hotel =>
                    {
                        // Last Month
                        DateTime startDate = new DateTime(DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(hotel.TimeZoneId).Year,
                            DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(hotel.TimeZoneId).Month - 1, 1);

                        // Gross Sale
                        var searchBookingsParams = new SearchBookingsParams
                        {
                            HotelId = hotel.HotelId,
                            StartDate = startDate,
                            EndDate = startDate.AddDays(DateTime.DaysInMonth(startDate.Year, startDate.Month) - 1),
                            IsBookingForRevenue = false
                        };
                        var ticketSoldLastMonth = bookingRepository.GetAllbookingsByRange(searchBookingsParams);

                        // Total Bookings Last Month
                        searchBookingsParams.IsBookingForRevenue = true;
                        var ticketRedeemedLastMonth = bookingRepository.GetAllbookingsByRange(searchBookingsParams);

                        // Total Bookings Weekends LastMonth
                        var totalBookingsWeekendsLastMonth = (from p in ticketRedeemedLastMonth
                            let checkInDate = (p.RedeemedDate ?? p.CheckinDate).Value.ToLosAngerlesTimeWithTimeZone(hotel.TimeZoneId)
                            where checkInDate.DayOfWeek == DayOfWeek.Saturday ||
                                  checkInDate.DayOfWeek == DayOfWeek.Sunday
                            select p).ToList();

                        // Total Bookings Weekdays LastMonth
                        var totalBookingsWeekdaysLastMonth =
                            ticketRedeemedLastMonth.Except(totalBookingsWeekendsLastMonth).ToList();

                        // Survey Last Month
                        var surveysLastMonth = ticketRedeemedLastMonth
                            .Where(x => bookingRepository.SurveyList.Where(sl => sl.BookingId == x.BookingId).Any(s => s.IsFinish))
                            .Select(x => bookingRepository.SurveyList.FirstOrDefault(sl => sl.BookingId == x.BookingId))
                            .ToList();

                        CalculateEstSpend(ref surveysLastMonth);

                        // Survey DayPass Last Month
                        var surveyDayPassLastMonth = (from s in surveysLastMonth
                            join b in ticketRedeemedLastMonth on s.BookingId equals b.BookingId
                            join p in bookingRepository.ProductList on b.ProductId equals p.ProductId
                            where p.ProductType == (int)Enums.ProductType.DayPass
                            select s).ToList();
                        CalculateEstSpend(ref surveyDayPassLastMonth);

                        // Survey Cabana Last Month
                        var surveyCabanaLastMonth = (from s in surveysLastMonth
                            join b in ticketRedeemedLastMonth on s.BookingId equals b.BookingId
                            join p in bookingRepository.ProductList on b.ProductId equals p.ProductId
                            where p.ProductType == (int)Enums.ProductType.Cabana
                            select s).ToList();
                        CalculateEstSpend(ref surveyCabanaLastMonth);

                        // TotalBookingDayPassLastMonth
                        var totalBookingDayPassLastMonth = (from b in ticketSoldLastMonth
                            join p in bookingRepository.ProductList on b.ProductId equals p.ProductId
                            where p.ProductType == (int)Enums.ProductType.DayPass
                            select b).ToList();

                        // TotalBookingCabanaLastMonth
                        var totalBookingCabanaLastMonth = (from b in ticketSoldLastMonth
                            join p in bookingRepository.ProductList on b.ProductId equals p.ProductId
                            where p.ProductType == (int)Enums.ProductType.Cabana
                            select b).ToList();

                        var products = bookingRepository.ProductList
                            .Where(p => p.IsActive && !p.IsDelete && p.HotelId == hotel.HotelId &&
                                        (p.ProductType == (int)Enums.ProductType.DayPass ||
                                         p.ProductType == (int)Enums.ProductType.Cabana))
                            .ToList();

                        // BookingsDayPassLastMonth
                        var bookingsDayPassLastMonth = (from b in ticketRedeemedLastMonth
                            join p in products on b.ProductId equals p.ProductId
                            where p.ProductType == (int)Enums.ProductType.DayPass
                            select b).ToList();


                        // BookingsCabanasLastMonth
                        var bookingsCabanqaLastMonth = (from b in ticketRedeemedLastMonth
                            join p in products on b.ProductId equals p.ProductId
                            where p.ProductType == (int)Enums.ProductType.Cabana
                            select b).ToList();

                        // Iventory Last Month
                        int iventory = 0;
                        int iventoryDayPass = 0;
                        int iventoryCabana = 0;
                        int iventoryWeekdays = 0;
                        int iventoryWeekens = 0;
                        products.ForEach(product =>
                        {
                            var paramIventory = new GetIventoryByMonthParams
                            {
                                Month = startDate.Month,
                                Year = startDate.Year,
                                HotelId = hotel.HotelId,
                                TimezoneId = hotel.TimeZoneId,
                                ProductId = product.ProductId,
                                ProductTypeId = product.ProductType
                            };
                            switch (product.ProductType)
                            {
                                case (int)Enums.ProductType.DayPass:
                                    iventoryDayPass += bookingRepository.GetIventoryByMonth(paramIventory);
                                    break;
                                case (int)Enums.ProductType.Cabana:
                                    iventoryCabana += bookingRepository.GetIventoryByMonth(paramIventory);
                                    break;
                            }
                            iventory += bookingRepository.GetIventoryByMonth(paramIventory);
                            var parambyWeekdays = bookingRepository.GetIventoryByMonthWeekendWeekday(paramIventory);
                            iventoryWeekdays += parambyWeekdays.Key;
                            iventoryWeekens += parambyWeekdays.Value;
                        });

                        // Previous 2 month
                        startDate = new DateTime(DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(hotel.TimeZoneId).Year,
                            DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(hotel.TimeZoneId).AddMonths(-2).Month, 1);

                        searchBookingsParams.StartDate = startDate;
                        searchBookingsParams.EndDate = startDate.AddDays(DateTime.DaysInMonth(startDate.Year, startDate.Month) - 1);
                        searchBookingsParams.IsBookingForRevenue = false;
                        var bookingsPreviousMonth = bookingRepository.GetAllbookingsByRange(searchBookingsParams);

                        var param = new MonthlySalesReportParams
                        {
                            HotelId = hotel.HotelId,
                            TimeZoneId = hotel.TimeZoneId,
                            GrossSale = ticketSoldLastMonth.Sum(b => b.MerchantPrice * b.Quantity),
                            TicketSold = ticketSoldLastMonth.Sum(b => b.Quantity),
                            TicketRedeemed = ticketRedeemedLastMonth.Sum(b => b.Quantity),
                            BookingsLastMonth = ticketRedeemedLastMonth,
                            BookingsDayPassLastMonth = bookingsDayPassLastMonth,
                            BookingsCabanaLastMonth = bookingsCabanqaLastMonth,
                            SurveyLastMonth = surveysLastMonth,
                            SurveyDayPassLastMonth = surveyDayPassLastMonth,
                            SurveyCabanaLastMonth = surveyCabanaLastMonth,
                            TotalBookingDayPassLastMonth = totalBookingDayPassLastMonth.Sum(b => b.Quantity),
                            GrossSaleDayPass = totalBookingDayPassLastMonth.Sum(b => b.Quantity * b.MerchantPrice),
                            TotalBookingCabanaLastMonth = totalBookingCabanaLastMonth.Sum(b => b.Quantity),
                            GrossSaleCabana = totalBookingCabanaLastMonth.Sum(b => b.Quantity * b.MerchantPrice),
                            TotalBookingWeekensLastMonth = totalBookingsWeekendsLastMonth.Sum(b => b.Quantity),
                            TotalBookingWeekdaysLastMonth = totalBookingsWeekdaysLastMonth.Sum(b => b.Quantity),
                            IventoryLastMonth = iventory,
                            IventoryDayPassLastMonth = iventoryDayPass,
                            IventoryCabanalastMonth = iventoryCabana,
                            IventoryWeekdays = iventoryWeekdays,
                            IventoryWeekens = iventoryWeekens,
                            BookingsPreviousMonth = bookingsPreviousMonth,
                            Productses = products
                        };

                        var emailInfo = GetMonthlySalesReport(param);

                        emailInfo.UserEmail = hotel.ReportSubscribers.Split(';').Where(Helper.IsValidEmail).ToList();
                        emailInfo.HotelNameWithNeighborhood = string.Format("{0} {1}", hotel.HotelName, hotel.Neighborhood);

                        if (emailInfo.UserEmail.Any())
                        {
                            var result = EmailHelper.EmailEmailMonthlySalesReport(emailInfo);
                            string json = JsonConvert.SerializeObject(emailInfo, CustomSettings.SerializerSettings());
                            var log = new Logs
                            {
                                LogKey = "SendHotelReportMonthlySummary",
                                UpdatedContent = string.Format("{0} - {1} - {2}",
                                    string.Join("-", emailInfo.UserEmail),
                                    result.Result,
                                    json),
                                UpdatedDate = DateTime.UtcNow,
                                UpdatedBy = 1
                            };
                            bookingRepository.AddLog(log);
                        }
                    });

                    schedules.Status = (int)Enums.ScheduleType.Complete;
                    schedules.CompleteDate = DateTime.UtcNow;
                    schedules.LastRun = DateTime.UtcNow;
                    bookingRepository.UpdateSchedule(schedules);
                }
                catch (Exception ex)
                {
                    var log = new Logs
                    {
                        LogKey = "SendHotelReportMonthlySummaryError",
                        UpdatedContent = string.Format("{0} {1} - {2}",
                            ex.Message,
                            ex.StackTrace),
                        UpdatedDate = DateTime.UtcNow,
                        UpdatedBy = 1
                    };
                    bookingRepository.AddLog(log);
                }
            }
        }

        private void SendEmailConfirmSubscription(Schedules schedules)
        {
            using (var subscriptionBookingRepository = new SubscriptionBookingRepository())
            {
                int subscriptionBookingId = schedules.SubscriptionBookingId;
                
                var subscriptionBookings = subscriptionBookingRepository.GetSubscriptionBookings(subscriptionBookingId);
                var currentSubscriptionCycle = subscriptionBookingRepository.GetCurrentSubscriptionCycle(subscriptionBookings.Id);
                var subscriptions = subscriptionBookingRepository.GetSubscriptions(subscriptionBookings.SubscriptionId);
                var discountSubscription = subscriptionBookingRepository.GetMembershipId(subscriptionBookingId);
                var discounts = subscriptionBookingRepository.GetDiscountsSubscriptionBookings(subscriptionBookingId);
                var image = subscriptionBookingRepository.GetSubscriptionImageCover(subscriptions.Id);
                var customerInfos = subscriptionBookingRepository.GetCustomer(subscriptionBookings.CustomerId);

                var emailInfo = new EmailSubscriptionConfirmationInfo
                {
                    UserEmail = customerInfos.EmailAddress,
                    FirstName = customerInfos.FirstName,
                    LastName = customerInfos.LastName,
                    SubscriptionName = subscriptions.Name,
                    SubscriptionDescription = subscriptions.ProductHighlight,
                    UrlReserveDayPass = string.Format("{0}", new Uri(new Uri(AppConfiguration.DefaultImageUrlSendEmail), Constant.SearchPageDefault).AbsoluteUri),
                    PurchasedDate = subscriptionBookings.BookedDate.ToLosAngerlesTime().ToString(Constant.FullDateFormat),
                    MembershipId = discountSubscription.Code,
                    Price = Helper.FormatPrice(subscriptions.Price),
                    TotalPrice = Helper.FormatPrice(currentSubscriptionCycle.TotalPrice),
                    FinePrint = subscriptions.WhatYouGet
                };

                if (currentSubscriptionCycle.PayByCredit > 0)
                {
                    emailInfo.Credit =
                        string.Format(
                            "<tr><td style=\"text-align: right;\">Credit:</td><td style=\"text-align: left; padding-left:7px;\">{0}</td></tr>",
                            Helper.FormatPrice(currentSubscriptionCycle.PayByCredit * -1));
                }

                if (discounts != null)
                {
                    var strFinePrintDiscount = @"<tr>" +
                                               "    <td colspan=\"2\">" +
                                               "        <h4 style=\"font-weight: bold; text-align: center; font-size: 14px;margin: 0 auto 15px; font-color: 4A4A4A;\">PROMO APPLIED</h4>" +
                                               "    </td>" +
                                               "</tr>" +
                                               "<tr>" +
                                               "    <td colspan=\"2\">" +
                                               "        <div style=\"margin-left:40px;margin-right:20px;margin-top:0;margin-bottom:10px;font-size:10px;max-width:500px;margin:0 auto;\">" +
                                               "            <span style=\"font-weight: bold; font-size: 10px; color: #11A752; text-transform: none;\">{0}</span><br/>" +
                                               "            <span style=\"font-size:10px; font-weight:bold;color:#666;\">{1}</span> - {2}<br/>" +
                                               "            {3}" +
                                               "        </div>" +
                                               "    </td>" +
                                               "</tr>";
                    emailInfo.DiscountFinePrint = string.Format(strFinePrintDiscount,
                        discounts.DiscountName,
                        discounts.Code,
                        string.Format("{0} OFF",
                            discounts.PromoType == (int)Enums.PromoType.Fixed
                                ? Helper.FormatPrice(discounts.PercentOff)
                                : discounts.PercentOff + "%"),
                        discounts.FinePrint);
                }

                try
                {
                    string imageUrl = Constant.ImageDefault;
                    if (image != null)
                    {
                        imageUrl = image.Url;
                    }
                    //imageUrl = Helper.ReplaceLastOccurrence(imageUrl, ".", "-ovl.");
                    try
                    {
                        emailInfo.ImageUrl = string.Format("{0}", new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), imageUrl).AbsoluteUri);
                    }
                    catch (Exception)
                    {
                        emailInfo.ImageUrl = imageUrl;
                    }

                    var promoPrice = currentSubscriptionCycle.MerchantPrice * currentSubscriptionCycle.Quantity -
                                     currentSubscriptionCycle.TotalPrice - currentSubscriptionCycle.PayByCredit;
                    if (!promoPrice.Equals(0))
                    {
                        emailInfo.Discount = string.Format(
                            "<tr><td style=\"text-align: right;\">Promo:</td><td style=\"text-align: left; padding-left:7px;\">{0}</td></tr>",
                            Helper.FormatPrice(promoPrice));
                    }

                    var result = EmailHelper.EmailSubscriptionConfirmation(emailInfo);

                    schedules.Status = (int)Enums.ScheduleType.Complete;
                    schedules.CompleteDate = DateTime.UtcNow;
                    schedules.LastRun = DateTime.UtcNow;
                    subscriptionBookingRepository.UpdateSchedule(schedules);

                    if (!string.IsNullOrEmpty(result.Result))
                    {
                        string json = JsonConvert.SerializeObject(emailInfo, CustomSettings.SerializerSettings());
                        var log = new Logs
                        {
                            LogKey = "EmailConfirmSubscription",
                            UpdatedContent = string.Format("{0} - {1} - {2} - {3}", emailInfo.UserEmail, subscriptionBookings.BookingIdString, result.Result, json),
                            UpdatedDate = DateTime.UtcNow,
                            UpdatedBy = 1
                        };
                        subscriptionBookingRepository.AddLog(log);
                    }
                }
                catch (Exception ex)
                {
                    string json = JsonConvert.SerializeObject(emailInfo, CustomSettings.SerializerSettings());
                    var log = new Logs
                    {
                        LogKey = "EmailConfirmSubscriptionError",
                        UpdatedContent = string.Format("{0} - {1} - {2} - {3} - {4}", emailInfo.UserEmail, subscriptionBookings.BookingIdString, ex.Message, ex.StackTrace, json),
                        UpdatedDate = DateTime.UtcNow,
                        UpdatedBy = 1
                    };
                    subscriptionBookingRepository.AddLog(log);
                }
            }
        }

        private void SendEmailGoldPassNoShow(Schedules schedules)
        {
            using (var bookingRepository = new BookingRepository())
            {
                int bookingId = schedules.BookingId.HasValue ? schedules.BookingId.Value : 0;

                // Refresh Hotel Data
                var booking = bookingRepository.GetBookings(bookingId);
                var products = bookingRepository.GetProduct(booking.ProductId);
                var hotels = bookingRepository.GetHotel(products.HotelId);
                var customerInfos = bookingRepository.GetCustomer(booking.CustomerId);

                var emailInfo = new EmailGoldPassNoShowInfo
                {
                    UserEmail = customerInfos.EmailAddress,
                    FirstName = customerInfos.FirstName,
                    LastName = customerInfos.LastName,
                    ProductName = products.ProductName,
                    HotelName = hotels.HotelName,
                    Neighborhood = hotels.Neighborhood,
                    BookingCode = booking.BookingIdString,
                    CheckinDate = booking.CheckinDate.HasValue ? booking.CheckinDate.Value.ToLosAngerlesTimeWithTimeZone(hotels.TimeZoneId).ToString(Constant.DateFormat) : string.Empty,
                    ChargeDate = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(hotels.TimeZoneId).ToString(Constant.DiscountDateTimeFormat),
                    ChargeAmount = Helper.FormatPrice(20),
                    ChargeAccount = string.Format("{0} {1}", customerInfos.CardType, customerInfos.BankAccountLast4)
                };

                try
                {
                    //https://trello.com/c/o7ryHh3f/114-user-disable-20-no-show-charge-temporarily
                    //var result = EmailHelper.EmailGoldPassNoShow(emailInfo);

                    schedules.Status = (int)Enums.ScheduleType.Complete;
                    schedules.CompleteDate = DateTime.UtcNow;
                    schedules.LastRun = DateTime.UtcNow;
                    bookingRepository.UpdateSchedule(schedules);

                    //https://trello.com/c/o7ryHh3f/114-user-disable-20-no-show-charge-temporarily
                    //if (!string.IsNullOrEmpty(result.Result))
                    //{
                    //    string json = JsonConvert.SerializeObject(emailInfo, CustomSettings.SerializerSettings());
                    //    var log = new Logs
                    //    {
                    //        LogKey = "EmailChargeNoShow",
                    //        UpdatedContent = string.Format("{0} - {1} - {2} - {3}", emailInfo.UserEmail, booking.BookingIdString, result.Result, json),
                    //        UpdatedDate = DateTime.UtcNow,
                    //        UpdatedBy = 1
                    //    };
                    //    bookingRepository.AddLog(log);
                    //}
                }
                catch (Exception ex)
                {
                    string json = JsonConvert.SerializeObject(emailInfo, CustomSettings.SerializerSettings());
                    var log = new Logs
                    {
                        LogKey = "EmailChargeNoShowError",
                        UpdatedContent = string.Format("{0} - {1} - {2} - {3} - {4}", emailInfo.UserEmail, booking.BookingIdString, ex.Message, ex.StackTrace, json),
                        UpdatedDate = DateTime.UtcNow,
                        UpdatedBy = 1
                    };
                    bookingRepository.AddLog(log);
                }
            }
        }

        private void SendEmailSubscriptionCancellation(Schedules schedules)
        {
            using (var subscriptionBookingRepository = new SubscriptionBookingRepository())
            {
                int subscriptionBookingId = schedules.SubscriptionBookingId;
                
                var subscriptionBookings = subscriptionBookingRepository.GetSubscriptionBookings(subscriptionBookingId);
                var customerInfos = subscriptionBookingRepository.GetCustomer(subscriptionBookings.CustomerId);

                var emailInfo = new EmailSubscriptionCancellationInfo
                {
                    UserEmail = customerInfos.EmailAddress,
                    FirstName = customerInfos.FirstName,
                    EndCurrentCycle = subscriptionBookings.EndDate.HasValue ? subscriptionBookings.EndDate.Value.ToString(Constant.DateFormat) : string.Empty
                };

                try
                {
                    var result = EmailHelper.EmailSubscriptionCancellation(emailInfo);

                    schedules.Status = (int)Enums.ScheduleType.Complete;
                    schedules.CompleteDate = DateTime.UtcNow;
                    schedules.LastRun = DateTime.UtcNow;
                    subscriptionBookingRepository.UpdateSchedule(schedules);

                    if (!string.IsNullOrEmpty(result.Result))
                    {
                        string json = JsonConvert.SerializeObject(emailInfo, CustomSettings.SerializerSettings());
                        var log = new Logs
                        {
                            LogKey = "EmailSubscriptionCancellation",
                            UpdatedContent = string.Format("{0} - SubscriptionBookingId: {1} - {2} - {3}", emailInfo.UserEmail, schedules.SubscriptionBookingId, result.Result, json),
                            UpdatedDate = DateTime.UtcNow,
                            UpdatedBy = 1
                        };
                        subscriptionBookingRepository.AddLog(log);
                    }
                }
                catch (Exception ex)
                {
                    string json = JsonConvert.SerializeObject(emailInfo, CustomSettings.SerializerSettings());
                    var log = new Logs
                    {
                        LogKey = "EmailSubscriptionCancellationError",
                        UpdatedContent = string.Format("{0} - SubscriptionBookingId: {1} - {2} - {3} - {4}", emailInfo.UserEmail, schedules.SubscriptionBookingId, ex.Message, ex.StackTrace, json),
                        UpdatedDate = DateTime.UtcNow,
                        UpdatedBy = 1
                    };
                    subscriptionBookingRepository.AddLog(log);
                }
            }
        }

        private void SendEmailGiftCardConfirmation(Schedules schedules)
        {
            using (var subscriptionBookingRepository = new GiftCardBookingRepository())
            {
                try
                {
                    var giftCardBookings = subscriptionBookingRepository.GetGiftCardBookings(schedules.GiftCardBookingId);
                    var customerInfos = subscriptionBookingRepository.GetCustomer(giftCardBookings.CustomerId);
                    var giftCards = subscriptionBookingRepository.GetGiftCard(giftCardBookings.GiftCardId);

                    var emailInfo = new EmaileGiftCardConfirmation
                    {
                        UserEmail = customerInfos.EmailAddress,
                        FirstName = customerInfos.FirstName,
                        GiftAmount = Helper.FormatPrice(giftCardBookings.TotalPrice),
                        Recipient = giftCardBookings.RecipientName,
                        EmailTo = giftCardBookings.RecipientEmail,
                        DeliveryDate = giftCardBookings.DeliveryDate.ToString(Constant.DateFormat),
                        Message = giftCardBookings.Message,
                        Purchased = giftCardBookings.UserBookedDate.ToDateTime().ToString(Constant.FullDateFormat),
                        GiftCardId = giftCards.Code,
                        Total = Helper.FormatPrice(giftCardBookings.Price)
                    };

                    if (giftCardBookings.PayByCredit > 0)
                    {
                        emailInfo.Credit += string.Format(
                            "<tr><td style=\"text-align: right;\">Credit:</td><td style=\"text-align: left; padding-left:7px;\">{0}</td></tr>",
                            Helper.FormatPrice(giftCardBookings.PayByCredit * -1));
                    }

                    var result = EmailHelper.EmaileGiftCardConfirmation(emailInfo);

                    schedules.Status = (int)Enums.ScheduleType.Complete;
                    schedules.CompleteDate = DateTime.UtcNow;
                    schedules.LastRun = DateTime.UtcNow;
                    subscriptionBookingRepository.UpdateSchedule(schedules);

                    if (!string.IsNullOrEmpty(result.Result))
                    {
                        string json = JsonConvert.SerializeObject(emailInfo, CustomSettings.SerializerSettings());
                        var log = new Logs
                        {
                            LogKey = "EmailGiftCardConfirmation",
                            UpdatedContent = string.Format("{0} - GiftCardBookingId: {1} - {2} - {3}", emailInfo.UserEmail, schedules.GiftCardBookingId, result.Result, json),
                            UpdatedDate = DateTime.UtcNow,
                            UpdatedBy = 1
                        };
                        subscriptionBookingRepository.AddLog(log);
                    }
                }
                catch (Exception ex)
                {
                    var log = new Logs
                    {
                        LogKey = "EmailGiftCardConfirmationError",
                        UpdatedContent = string.Format("{0} - giftCardBookingId: {1} - {2} - {3} - {4}", schedules.GiftCardBookingId, ex.Message, ex.StackTrace, ex.Source),
                        UpdatedDate = DateTime.UtcNow,
                        UpdatedBy = 1
                    };
                    subscriptionBookingRepository.AddLog(log);
                }
            }
        }

        private void MigrateOldSurvey()
        {
            using (var bookingRepository = new BookingRepository())
            {
                var bookingId = bookingRepository.BookingList
                    .Where(b => b.PassStatus == (int)Enums.BookingStatus.Active)
                    .Select(b => b.BookingId)
                    .ToList();

                var bookingsHasSurveys = (from b in bookingRepository.BookingList
                                          join s in bookingRepository.SurveyList on b.BookingId equals s.BookingId
                                          select b.BookingId)
                    .ToList();

                var bookingNotHaveSurvey = bookingId.Except(bookingsHasSurveys);

                var bookings = bookingRepository.BookingList
                    .Where(b => bookingNotHaveSurvey.Contains(b.BookingId))
                    .ToList();

                if (bookings.Any())
                {
                    var surveysList = new List<Surveys>();
                    bookings.ForEach(booking =>
                    {
                        var surveys = new Surveys
                        {
                            BookingId = booking.BookingId,
                            Code = Helper.RandomString(20).ToLower(),
                            IsFinish = false,
                            LastStep = 0
                        };
                        surveysList.Add(surveys);
                    });

                    bookingRepository.AddSurveys(surveysList);

                    CacheLayer.Clear(CacheKeys.BookingsCacheKey);
                    CacheLayer.Clear(CacheKeys.SurveysCacheKey);
                }
            }
        }

        private void MigrateOldCustomerCredits()
        {
            using (var repository = new BookingRepository())
            {
                var customerInfos = repository.CustomerInfoList
                    .ToList();

                var customerHasCredit = (from cc in repository.CustomerCreditList
                        join ci in repository.CustomerInfoList on cc.CustomerId equals ci.CustomerId
                        select ci)
                    .ToList();

                var customers = customerInfos.Except(customerHasCredit).ToList();

                if (customers.Any())
                {
                    var customerCreditList = new List<CustomerCredits>();
                    customers.ForEach(customer =>
                    {
                        var surveys = new CustomerCredits
                        {
                            Amount = 0,
                            CreatedDate = DateTime.UtcNow,
                            CustomerId = customer.CustomerId,
                            FirstRewardForOwner = 5,
                            FirstRewardForReferral = 5,
                            IsActive = true,
                            IsDelete = false,
                            LastUpdatedDate = DateTime.UtcNow,
                            ReferralCode = Helper.RandomString(8)
                        };
                        customerCreditList.Add(surveys);
                    });

                    repository.AddCustomerCredits(customerCreditList);

                    CacheLayer.Clear(CacheKeys.CustomerCreditsCacheKey);
                    CacheLayer.Clear(CacheKeys.CustomerInfosCacheKey);
                }
            }
        }

        //private void MigrateOldBlockedDate()
        //{
        //    using (var repository = new BlockedDateRepository())
        //    {
        //        var products = repository.ProductList.Where(p => p.IsActive && !p.IsDelete).ToList();

        //        products.ForEach(product =>
        //        {
        //            var blockedDate = repository.BlockedDateList
        //                .Where(bd => bd.HotelId == product.HotelId && bd.ProductTypeId == product.ProductType && (bd.IsActive == null || !bd.IsActive.Value))
        //                .ToList();

        //            var blockedDateHasCustomPrice = (from bd in blockedDate
        //                join bdcp in repository.BlockedDatesCustomPriceList on bd.Id equals bdcp.BlockedDateId
        //                select bd)
        //                .ToList();

        //            var blockedDateWithoutCustomPrice = blockedDate.Except(blockedDateHasCustomPrice).ToList();

        //            if (blockedDateWithoutCustomPrice.Any())
        //            {
        //                var customPriceList = new List<BlockedDatesCustomPrice>();

        //                blockedDateWithoutCustomPrice.ForEach(bdcp =>
        //                {
        //                    if (bdcp.Date.HasValue)
        //                    {
        //                        var customPrice = new BlockedDatesCustomPrice
        //                        {
        //                            BlockedDateId = bdcp.Id,
        //                            Capacity = bdcp.Capacity,
        //                            Date = bdcp.Date.Value,
        //                            RegularPrice = bdcp.CustomPrice ?? 0,
        //                            DiscountPrice = 0,
        //                            ProductId = product.ProductId
        //                        };

        //                        customPriceList.Add(customPrice);
        //                    }
        //                });

        //                repository.AddCustomPrice(customPriceList);
        //            }
        //        });
        //    }
        //}

        private bool EqualDate(DateTime date1, DateTime date2)
        {
            if (date1.Date == date2.Date && date1.Hour == date2.Hour && date1.Minute == date2.Minute && date2.Second < 5)
            {
                return true;
            }
            return false;
        }

        private string GetAddOnString(List<Products> products, string browseDayPassUrl, string title)
        {
            var result = new StringBuilder();
            if (products.Any()) { 
                result.Append(title);
                result.Append("<table border=\"0\" cellpadding=\"1\" cellspacing=\"10\" style=\"width: 100 %; \"><tbody>");
                for (int i = 0; i < products.Count; i++)
                {
                    if (i % 2 == 0)
                    {
                        result.Append("<tr>");
                    }

                    result.Append(GetAddOnItem(products[i], browseDayPassUrl));

                    if (i % 2 == 0 && i == products.Count - 1)
                    {
                        result.Append("<td></td>");
                    }

                    if (i % 2 != 0 || (i + 1) == products.Count)
                    {
                        result.Append("</tr>");
                    }
                }
                result.Append("</tbody></table>");
            }
            return result.ToString();
        }

        private string GetAddOnItem(Products products, string bookingDayPassUrl)
        {
            string result = "<td style=\"width:50%;border:solid 1px #a6a6a6;box-shadow:0px 2px 4px #a6a6a6;\"><a href=\"{4}\" style=\"text-decoration:none;\">" +
                "    <img src=\"{0}\" style=\"display: block; max-width: 100%;\" alt=\"\"/>" +
                "    <div style=\"width: 100%; display: table; height: 35px;border-spacing:0;\">" +
                "        <div style=\"width: 65%; text-transform: uppercase; text-decoration:none; color: #13C1F4; font-weight: bold; padding-left: 5px; display: table-cell; vertical-align: middle; height: 20px; font-size: 10px;\">" +
                "            {1}" +
                "        </div>" +
                "        <div style=\"width: 35%; display: table-cell; text-align: right;\">" +
                "            <div style=\"font-size: 14px; font-weight: 700; line-height: 14px; position: relative; color: #4A4A4A; margin-top: 5px; margin-right: 3px;\">" +
                "                {2}" +
                "            </div>" +
                "            <div style=\"font-size: 10px; color: #4A4A4A; line-height: 14px; margin-right: 3px;\">" +
                "                {3}" +
                "            </div>" +
                "        </div>" +
                "    </div>" +
                "</a></td>";
            bookingDayPassUrl = bookingDayPassUrl.Substring(0, bookingDayPassUrl.LastIndexOf('/'));
            bookingDayPassUrl = bookingDayPassUrl.Substring(0, bookingDayPassUrl.LastIndexOf('/') + 1);
            var productUrl = bookingDayPassUrl.Substring(0, bookingDayPassUrl.LastIndexOf('/') + 1) +
                             products.ProductNameUrl + "/" + products.ProductId;
            productUrl = string.Format("{0}",
                new Uri(new Uri(AppConfiguration.DefaultImageUrlSendEmail), productUrl).AbsoluteUri);
            return string.Format(result,
                products.ImageAddOnUrl,
                products.ProductName,
                Helper.FormatPrice(products.ActualPriceWithDate.Price),
                products.MaxGuest > 1 ? products.MaxGuest +  " guests" : "per guest",
                productUrl);
        }

        private EmailMonthlySalesReportInfo GetMonthlySalesReport(MonthlySalesReportParams param)
        {
            var previous2MonthGrossSales = param.BookingsPreviousMonth.Sum(b => b.MerchantPrice * b.Quantity);

            // Incremental DayPass Revenue
            var totalDayPassResponsed = param.BookingsDayPassLastMonth.Where(x => param.SurveyDayPassLastMonth.Select(s => s.BookingId).Contains(x.BookingId))
                .Sum(b => b.Quantity);

            var avgDayPassPerTicketSpend = totalDayPassResponsed > 0
                ? param.SurveyDayPassLastMonth.Sum(s => s.EstSpend) / totalDayPassResponsed
                : 0;

            var totalDayPassBookings = param.BookingsDayPassLastMonth.Count > 0 ? param.BookingsDayPassLastMonth.Count : 0;
            var nonSpenderDayPass = param.SurveyDayPassLastMonth.Count > 0
                ? param.SurveyDayPassLastMonth.Count(s => s.EstSpend.Equals(0.0))
                : 0;
            var nonSpenderPercentDayPass = totalDayPassBookings > 0 ? nonSpenderDayPass * 100 / totalDayPassBookings : 0;
            var incrementalDayPassRevenue = param.BookingsDayPassLastMonth.Sum(x => x.Quantity) * avgDayPassPerTicketSpend * (100 - nonSpenderPercentDayPass) / 100;

            // Incremental Cabana Revenue
            var totalCabanaResponsed = param.BookingsCabanaLastMonth.Where(x => param.SurveyCabanaLastMonth.Select(s => s.BookingId).Contains(x.BookingId))
                .Sum(b => b.Quantity);

            var avgCabanaPerTicketSpend = totalCabanaResponsed > 0
                ? param.SurveyCabanaLastMonth.Sum(s => s.EstSpend) / totalCabanaResponsed
                : 0;

            var totalCabanaBookings = param.BookingsCabanaLastMonth.Count > 0 ? param.BookingsCabanaLastMonth.Count : 0;
            var nonSpenderCabana = param.SurveyCabanaLastMonth.Count > 0
                ? param.SurveyCabanaLastMonth.Count(s => s.EstSpend.Equals(0.0))
                : 0;
            var nonSpenderPercentCabana = totalCabanaBookings > 0 ? nonSpenderCabana * 100 / totalCabanaBookings : 0;
            var incrementalCabanaRevenue = param.BookingsCabanaLastMonth.Sum(x => x.Quantity) * avgCabanaPerTicketSpend * (100 - nonSpenderPercentCabana) / 100;

            // Utilization
            var utilization = param.IventoryLastMonth != 0 ? Math.Round((double)param.TicketRedeemed * 100 / param.IventoryLastMonth) : 0;

            // Utilization Weekdays
            var utilizationWeekdays = param.IventoryWeekdays != 0 ? Math.Round((double)param.TotalBookingWeekdaysLastMonth * 100 / param.IventoryWeekdays) : 0;

            // Utilization Weekens
            var utilizationWeekens = param.IventoryWeekens != 0 ? Math.Round((double)param.TotalBookingWeekensLastMonth * 100 / param.IventoryWeekens) : 0;

            // Avg Spend DayPass

            var avgSpendDayPass = param.SurveyDayPassLastMonth.Count > 0
                ? param.SurveyDayPassLastMonth.Sum(s => s.EstSpend) / param.TotalBookingDayPassLastMonth
                : 0;

            // Avg Spend Cabana

            var avgSpendCabana = param.SurveyCabanaLastMonth.Count > 0
                ? param.SurveyCabanaLastMonth.Sum(s => s.EstSpend) / param.TotalBookingCabanaLastMonth
                : 0;

            return new EmailMonthlySalesReportInfo
            {
                SelectedMonth = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(param.TimeZoneId).AddMonths(-1).ToString("MMMM yyyy"),
                GrossSales = Helper.FormatPrice(param.GrossSale),
                IncreaseFromMonth = string.Format("{0:0}%", ((param.GrossSale - previous2MonthGrossSales) / previous2MonthGrossSales) * 100),
                IncreaseFromPreviousMonth = string.Format("INCREASE FROM {0:MMMM}", DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(param.TimeZoneId).AddMonths(-2)).ToUpper(),
                GrossSalesDayPass = Helper.FormatPrice(param.GrossSaleDayPass),
                GrossSalesCabana = Helper.FormatPrice(param.GrossSaleCabana),
                Incremental = Helper.FormatPrice(incrementalDayPassRevenue + incrementalCabanaRevenue),
                TicketSold = param.TicketSold.ToString(),
                TicketSoldDayPass = param.TotalBookingDayPassLastMonth.ToString(),
                TicketSoldCabana = param.TotalBookingCabanaLastMonth.ToString(),
                Utilization = string.Format("{0}%", utilization),
                UtilizationWeedDays = string.Format("{0:0}%", utilizationWeekdays),
                UtilizationWeedkens = string.Format("{0:0}%", utilizationWeekens),
                AvgSpendGuest = Helper.FormatPrice(Math.Round(avgSpendDayPass)),
                AvgSpendCabana = Helper.FormatPrice(Math.Round(avgSpendCabana))
            };
        }

        private void CalculateEstSpend(ref List<Surveys> surveys)
        {
            if (surveys.Any())
            {
                surveys.ForEach(item =>
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
        }

        private StripeCharge CreateCharges(double amount, string customerId, string description)
        {
            var myCharge = new StripeChargeCreateOptions();

            // always set these properties
            myCharge.Amount = Convert.ToInt32(amount * 100); // cents
            myCharge.Currency = "USD";

            // set this if you want to
            myCharge.Description = description;

            // set this property if using a customer - this MUST be set if you are using an existing source!
            myCharge.CustomerId = customerId;

            // (not required) set this to false if you don't want to capture the charge yet - requires you call capture later
            myCharge.Capture = true;

            var chargeService = new StripeChargeService();
            StripeCharge stripeCharge = chargeService.Create(myCharge);

            return stripeCharge;
        }

        protected override void OnContinue()
        {

        }

        protected override void OnPause()
        {

        }

        protected override void OnShutdown()
        {

        }

        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public long dwServiceType;
            public ServiceState dwCurrentState;
            public long dwControlsAccepted;
            public long dwWin32ExitCode;
            public long dwServiceSpecificExitCode;
            public long dwCheckPoint;
            public long dwWaitHint;
        };
    }
}







