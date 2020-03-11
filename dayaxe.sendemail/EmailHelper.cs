using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using SendGrid;
using Newtonsoft.Json;

namespace Dayaxe.SendEmail
{
    public static class EmailHelper
    {
        static SendGridClient sendgrid = new SendGridClient(EmailConfig.SendEmailApikeys);

        /// <summary>
        /// This Template send to User to confirm with user's booking
        /// </summary>
        /// <param name="emailInfo"></param>
        /// <returns></returns>
        public static async Task<string> EmailTemplateBookingConfirmation(EmailInfo emailInfo)
        {
            var mail = BuildMailInfo(EmailConfig.DefaultSendFromEmail, emailInfo.UserEmail);

            mail.TemplateId = EmailConfig.EmailBookingConfirmation;
            mail.AddSubstitution("[Hotel_Name]", emailInfo.HotelName);
            mail.AddSubstitution("[Neighborhood]", emailInfo.Neighborhood);
            mail.AddSubstitution("[rating]", emailInfo.TotalCustomerReviews > 0 ? emailInfo.CustomerRatingString : emailInfo.RatingString);
            mail.AddSubstitution("[FirstName]", emailInfo.FirstName);
            mail.AddSubstitution("[ImageUrl]", emailInfo.HotelImage);
            mail.AddSubstitution("[UrlViewDayPass]", emailInfo.UrlViewDayPass);
            mail.AddSubstitution("[Address]", emailInfo.Address);
            mail.AddSubstitution("[AddressInfo]", emailInfo.AddressInfo);
            mail.AddSubstitution("[PhoneNumber]", emailInfo.PhoneNumber);
            mail.AddSubstitution("[FullName]", emailInfo.FullName);
            mail.AddSubstitution("[BookedDate]", emailInfo.BookedDate);
            //mail.AddSubstitution("[BlackoutDays]", emailInfo.BlackoutDays);
            mail.AddSubstitution("[CheckInDate]", emailInfo.CheckInDate);
            mail.AddSubstitution("[CheckInText]", emailInfo.CheckInText);
            mail.AddSubstitution("[BookingCode]", emailInfo.BookingCode);
            mail.AddSubstitution("[BookingTotal]", emailInfo.BookingTotal);
            mail.AddSubstitution("[UrlLinkToTerms]", emailInfo.UrlLinkToTerms);
            mail.AddSubstitution("[MaxGuest]", emailInfo.MaxGuest);
            mail.AddSubstitution("[Discount]", emailInfo.Discount);
            mail.AddSubstitution("[CheckInPlace]", emailInfo.CheckInPlace);
            mail.AddSubstitution("[ViewDayPassString]", emailInfo.ViewDayPassString);
            mail.AddSubstitution("[PerTicketPrice]", emailInfo.PerTicketPrice);
            mail.AddSubstitution("[Tickets]", emailInfo.Tickets);
            mail.AddSubstitution("[FinePrintVisible]", emailInfo.FinePrintVisible);
            mail.AddSubstitution("[AddOnString]", emailInfo.AddOnString);
            mail.AddSubstitution("[Updated]", emailInfo.Updated);
            mail.AddSubstitution("[UpdatedReceipt]", emailInfo.UpdatedReceipt);
            mail.AddSubstitution("[BookingStatus]", emailInfo.BookingStatus);
            mail.AddSubstitution("[SubscriptionReminder]", emailInfo.SubscriptionReminder);
            mail.AddSubstitution("[DiscountFinePrint]", emailInfo.DiscountFinePrint);
            mail.AddSubstitution("[TotalTicketInfo]", emailInfo.TotalTicketInfo);
            mail.AddSubstitution("[TotalPriceInfo]", emailInfo.TotalPriceInfo);
            mail.AddBcc(new EmailAddress(EmailConfig.DefaultSendFromEmail));

            dynamic response = await sendgrid.SendEmailAsync(mail);
            HttpStatusCode statusCode = response.StatusCode;
            string result = response.Body.ReadAsStringAsync().Result;
            HttpResponseHeaders header = response.Headers;
            return statusCode + " - " + result + " - " + header;
        }

        /// <summary>
        /// This template send to Hotel's Admin With Product is not Check-In Required
        /// </summary>
        /// <param name="emailInfo"></param>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static async Task<string> EmailTemplateBookingConfirmationOfHotel(EmailInfo emailInfo, string fullName)
        {
            var mail = BuildMailInfo(EmailConfig.DefaultSendFromEmail, emailInfo.UserEmail);

            mail.TemplateId = EmailConfig.EmailBookingconfirmationOfHotel;
            mail.AddSubstitution("[Hotel_Name]", emailInfo.HotelName);
            mail.AddSubstitution("[FullName]", fullName);
            mail.AddSubstitution("[BookedDate]", emailInfo.BookedDate);
            //mail.AddSubstitution("[BlackoutDays]", emailInfo.BlackoutDays);
            mail.AddSubstitution("[RedeemedDate]", emailInfo.RedeemedDate);
            mail.AddSubstitution("[ExpiredDate]", emailInfo.ExpiredDate);
            mail.AddSubstitution("[BookingCode]", emailInfo.BookingCode);
            mail.AddSubstitution("[ProductType]", emailInfo.ProductType);
            mail.AddSubstitution("[ProductName]", emailInfo.ProductName);
            mail.AddSubstitution("[PerTicketPrice]", emailInfo.PerTicketPrice);
            mail.AddSubstitution("[MaxPerTicket]", emailInfo.MaxPerTicket);
            mail.AddSubstitution("[Tickets]", emailInfo.Tickets);

            if (emailInfo.EmailBccList.Any())
            {
                emailInfo.EmailBccList.ForEach(email =>
                {
                    if (!string.IsNullOrEmpty(email.Replace("\\s", "")))
                    {
                        mail.AddTo(new EmailAddress(email.Replace("\\s", "")));
                    }
                });
            }

            dynamic response = await sendgrid.SendEmailAsync(mail);
            HttpStatusCode statusCode = response.StatusCode;
            string result = response.Body.ReadAsStringAsync().Result;
            HttpResponseHeaders header = response.Headers;
            return statusCode + " - " + result + " - " + header;
        }

        /// <summary>
        /// This Template send to Hotel's Admin with Product Check-In Required
        /// </summary>
        /// <param name="emailInfo"></param>
        /// <returns></returns>
        public static async Task<string> EmailTemplateBookingAlertOfHotel(EmailInfo emailInfo)
        {
            //var mail = BuildMailInfo(EmailConfig.DefaultSendFromEmail, emailInfo.UserEmail);

            //mail.TemplateId = EmailConfig.EmailBookingAlertOfHotel;
            //mail.AddSubstitution("[Hotel_Name]", emailInfo.HotelName);
            //mail.AddSubstitution("[FullName]", emailInfo.FullName);
            //mail.AddSubstitution("[BookedDate]", emailInfo.BookedDate);
            //mail.AddSubstitution("[BookingCode]", emailInfo.BookingCode);
            //mail.AddSubstitution("[ProductName]", emailInfo.ProductName);
            //mail.AddSubstitution("[CheckInDate]", emailInfo.CheckInDate);
            //mail.AddSubstitution("[ProductType]", emailInfo.ProductType);
            //mail.AddSubstitution("[MaxPerTicket]", emailInfo.MaxPerTicket);
            //mail.AddSubstitution("[PerTicketPrice]", emailInfo.PerTicketPrice);
            //mail.AddSubstitution("[Tickets]", emailInfo.Tickets);
            //mail.AddSubstitution("[Updated]", emailInfo.Updated);
            //mail.AddSubstitution("[UpdatedReceipt]", emailInfo.UpdatedReceipt);
            //mail.AddSubstitution("[BookingStatus]", emailInfo.BookingStatus);

            //if (emailInfo.EmailBccList.Any())
            //{
            //    emailInfo.EmailBccList.ForEach(email =>
            //    {
            //        if (!string.IsNullOrEmpty(email.Replace("\\s", "")))
            //        {
            //            mail.AddTo(new EmailAddress(email.Replace("\\s", "")));
            //        }
            //    });
            //}

            //dynamic response = await sendgrid.SendEmailAsync(mail);
            //HttpStatusCode statusCode = response.StatusCode;
            //string result = response.Body.ReadAsStringAsync().Result;
            //HttpResponseHeaders header = response.Headers;
            //return statusCode + " - " + result + " - " + header;

            var mail = BuildMailInfo(EmailConfig.DefaultSendFromEmail, emailInfo.UserEmail);
            mail.SetTemplateId(EmailConfig.EmailBookingAlertOfHotel);

            if (emailInfo.EmailBccList.Any())
            {
                emailInfo.EmailBccList.ForEach(email =>
                {
                    if (!string.IsNullOrEmpty(email.Replace("\\s", "")))
                    {
                        mail.AddTo(new EmailAddress(email.Replace("\\s", "")));
                    }
                });
            }

            mail.SetTemplateData(emailInfo);

            dynamic response = await sendgrid.SendEmailAsync(mail);
            HttpStatusCode statusCode = response.StatusCode;
            string result = response.Body.ReadAsStringAsync().Result;
            HttpResponseHeaders header = response.Headers;
            return statusCode + " - " + result + " - " + header;
        }

        /// <summary>
        /// This Template Send Email Survey to User After booking has been Redeemed
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="hotelName"></param>
        /// <param name="neightborhood"></param>
        /// <param name="firstName"></param>
        /// <param name="ratingUrl"></param>
        /// <returns></returns>
        public static async Task<string> EmailTemplateSurvey(string toEmail, string hotelName, string neightborhood, string firstName, string ratingUrl)
        {
            var mail = BuildMailInfo(EmailConfig.DefaultSendSurveyEmail, toEmail);

            mail.TemplateId = EmailConfig.EmailPostPurchaseSurvey;
            mail.AddSubstitution("[Hotel_Name]", hotelName);
            mail.AddSubstitution("[Neighborhood]", neightborhood);
            mail.AddSubstitution("[First_Name]", firstName);
            mail.AddSubstitution("[DayAxe_Logo]", EmailConfig.ImageLogo);
            mail.AddSubstitution("[Rating_Url]", ratingUrl);
            if (EmailConfig.SendEmailSurveyAfterMinutes > 0)
            {
                mail.SendAt = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds + EmailConfig.SendEmailSurveyAfterMinutes * 60;
            }

            dynamic response = await sendgrid.SendEmailAsync(mail);
            string result = response.Body.ReadAsStringAsync().Result;
            return result;
        }

        /// <summary>
        /// User's request change password
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="firstName"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string> EmailForgotPassword(string toEmail, string firstName, string url)
        {
            var mail = BuildMailInfo(EmailConfig.DefaultSendFromEmail, toEmail);

            mail.TemplateId = EmailConfig.EmailPasswordRecovery;
            if (String.IsNullOrEmpty(firstName))
            {
                mail.AddSubstitution("[FirstName]", "Hi!");
            }
            else
            {
                mail.AddSubstitution("[FirstName]", string.Format("Hi, {0}!", firstName));
            }
            mail.AddSubstitution("[Reset_PasswordUrl]", url);

            dynamic response = await sendgrid.SendEmailAsync(mail);
            string result = response.Body.ReadAsStringAsync().Result;
            return result;
        }

        /// <summary>
        /// Email Password's change
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="firstName"></param>
        /// <returns></returns>
        public static async Task<string> EmailNewPasswordchange(string toEmail, string firstName)
        {
            var mail = BuildMailInfo(EmailConfig.DefaultSendFromEmail, toEmail);

            mail.TemplateId = EmailConfig.EmailPasswordCreated;
            if (String.IsNullOrEmpty(firstName))
            {
                mail.AddSubstitution("[FirstName]", "Hi!");
            }
            else
            {
                mail.AddSubstitution("[FirstName]", string.Format("Hi, {0}!", firstName));
            }

            dynamic response = await sendgrid.SendEmailAsync(mail);
            string result = response.Body.ReadAsStringAsync().Result;
            return result;
        }

        /// <summary>
        /// Send email when user create account on website
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="firstName"></param>
        /// <param name="password"></param>
        /// <param name="urlResetPassword"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string> EmailNewAccount(string toEmail, string firstName, string password, string urlResetPassword, string url)
        {
            var mail = BuildMailInfo(EmailConfig.DefaultSendFromEmail, toEmail);

            mail.TemplateId = EmailConfig.EmailNewAccount;
            if (String.IsNullOrEmpty(firstName))
            {
                mail.AddSubstitution("[FirstName]", "Welcome to DayAxe!");
            }
            else
            {
                mail.AddSubstitution("[FirstName]", string.Format("Welcome to DayAxe, {0}!", firstName));
            }
            mail.AddSubstitution("[NewPassword]", password);
            mail.AddSubstitution("[UrlResetPassword]", urlResetPassword);
            mail.AddSubstitution("[UrlBrowsePass]", url);

            dynamic response = await sendgrid.SendEmailAsync(mail);
            string result = response.Body.ReadAsStringAsync().Result;
            return result;
        }

        /// <summary>
        /// Send Email Refund for customer and Bcc to Support Dayaxe
        /// </summary>
        /// <param name="emailInfo"></param>
        /// <returns></returns>
        public static async Task<string> EmailRefund(EmailRefundInfo emailInfo)
        {
            var mail = BuildMailInfo(EmailConfig.DefaultSendFromEmail, emailInfo.UserEmail);

            mail.TemplateId = EmailConfig.EmailRefund;
            mail.AddSubstitution("[Hotel_Name]", emailInfo.HotelName);
            mail.AddSubstitution("[Neighborhood]", emailInfo.Neightborhood);
            mail.AddSubstitution("[ProductName]", emailInfo.ProductName);
            mail.AddSubstitution("[FirstName]", emailInfo.FirstName);
            mail.AddSubstitution("[UrlViewDayPass]", emailInfo.UrlViewDayPass);
            mail.AddSubstitution("[FullName]", emailInfo.FullName);
            mail.AddSubstitution("[BookedDate]", emailInfo.BookedDate);
            mail.AddSubstitution("[BookingCode]", emailInfo.BookingCode);
            mail.AddSubstitution("[RefundAmount]", emailInfo.RefundAmount);
            mail.AddSubstitution("[RefundCreditAmount]", emailInfo.RefundCreditAmount);
            mail.AddSubstitution("[RefundStripeAmount]", emailInfo.RefundStripeAmount);
            mail.AddSubstitution("[RefundDate]", emailInfo.RefundDate);
            mail.AddSubstitution("[NumberOfTickets]", emailInfo.Tickets);
            //mail.AddSubstitution("[PaymentType]", emailInfo.PaymentType);
            mail.AddBcc(new EmailAddress(EmailConfig.DefaultSendFromEmail));

            dynamic response = await sendgrid.SendEmailAsync(mail);
            HttpStatusCode statusCode = response.StatusCode;
            string result = response.Body.ReadAsStringAsync().Result;
            HttpResponseHeaders header = response.Headers;
            return statusCode + " - " + result + " - " + header;
        }

        public static async Task<string> EmailCheckInReminder(EmailCheckInReminder emailInfo)
        {
            var mail = BuildMailInfo(EmailConfig.DefaultSendFromEmail, emailInfo.UserEmail);

            mail.TemplateId = EmailConfig.CheckInReminder;
            mail.AddSubstitution("[Hotel_Name]", emailInfo.HotelName);
            mail.AddSubstitution("[Product_Name]", emailInfo.ProductName);
            mail.AddSubstitution("[Neighborhood]", emailInfo.Neighborhood);
            mail.AddSubstitution("[FirstName]", emailInfo.FirstName);
            mail.AddSubstitution("[CheckInDate]", emailInfo.CheckInDate);
            mail.AddSubstitution("[rating]", emailInfo.RatingString);
            mail.AddSubstitution("[MaxGuest]", emailInfo.MaxGuest);
            mail.AddSubstitution("[Tickets]", emailInfo.Tickets);
            mail.AddSubstitution("[ImageUrl]", emailInfo.ImageUrl);
            mail.AddSubstitution("[CheckInPlace]", emailInfo.CheckInPlace);
            mail.AddSubstitution("[UrlViewDayPass]", emailInfo.UrlViewDayPass);
            mail.AddSubstitution("[AddOnString]", emailInfo.AddOnString);
            mail.AddSubstitution("[SubscriptionReminder]", emailInfo.SubscriptionReminder);

            dynamic response = await sendgrid.SendEmailAsync(mail);
            HttpStatusCode statusCode = response.StatusCode;
            string result = response.Body.ReadAsStringAsync().Result;
            HttpResponseHeaders header = response.Headers;
            return statusCode + " - " + result + " - " + header;
        }

        public static async Task<string> EmailAddOnNotification(EmailAddOnNotificationInfo emailInfo)
        {
            var mail = BuildMailInfo(EmailConfig.DefaultSendFromEmail, emailInfo.UserEmail);

            mail.TemplateId = EmailConfig.AvailableAddOn;
            mail.AddSubstitution("[Hotel_Name]", emailInfo.HotelName);
            mail.AddSubstitution("[Product_Name]", emailInfo.ProductName);
            mail.AddSubstitution("[FirstName]", emailInfo.FirstName);
            mail.AddSubstitution("[AddOnString]", emailInfo.AddOnString);
            mail.AddSubstitution("[CheckInDate]", emailInfo.CheckInDate);
            mail.SendAt = (long)emailInfo.SentAt;

            dynamic response = await sendgrid.SendEmailAsync(mail);
            HttpStatusCode statusCode = response.StatusCode;
            string result = response.Body.ReadAsStringAsync().Result;
            HttpResponseHeaders header = response.Headers;
            return statusCode + " - " + result + " - " + header;
        }

        public static async Task<string> EmailAddOnNotificationRedemption(EmailAddOnNotificationInfo emailInfo)
        {
            var mail = BuildMailInfo(EmailConfig.DefaultSendFromEmail, emailInfo.UserEmail);

            mail.TemplateId = EmailConfig.AvailableAddOnRedemption;
            mail.AddSubstitution("[Hotel_Name]", emailInfo.HotelName);
            mail.AddSubstitution("[Product_Name]", emailInfo.ProductName);
            mail.AddSubstitution("[FirstName]", emailInfo.FirstName);
            mail.AddSubstitution("[AddOnString]", emailInfo.AddOnString);
            mail.AddSubstitution("[CheckInDate]", emailInfo.CheckInDate);
            mail.SendAt = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds + 15 * 60;

            dynamic response = await sendgrid.SendEmailAsync(mail);
            HttpStatusCode statusCode = response.StatusCode;
            string result = response.Body.ReadAsStringAsync().Result;
            HttpResponseHeaders header = response.Headers;
            return statusCode + " - " + result + " - " + header;
        }

        public static async Task<string> EmailTicketOpenedUp(EmailTicketOpenedUp emailInfo)
        {
            var mail = BuildMailInfo(EmailConfig.DefaultSendFromEmail, emailInfo.UserEmail);

            mail.TemplateId = EmailConfig.TicketOpenedUp;
            mail.AddSubstitution("[FirstName]", emailInfo.FirstName);
            mail.AddSubstitution("[ProductName]", emailInfo.ProductName);
            mail.AddSubstitution("[HotelName]", emailInfo.HotelName);
            mail.AddSubstitution("[Location]", emailInfo.Location);
            mail.AddSubstitution("[Price]", emailInfo.Price);
            mail.AddSubstitution("[CheckInDate]", emailInfo.CheckInDate);
            mail.AddSubstitution("[UrlProductUrl]", emailInfo.UrlProductUrl);

            dynamic response = await sendgrid.SendEmailAsync(mail);
            HttpStatusCode statusCode = response.StatusCode;
            string result = response.Body.ReadAsStringAsync().Result;
            HttpResponseHeaders header = response.Headers;
            return statusCode + " - " + result + " - " + header;
        }

        public static async Task<string> EmailNotifiedSurveyHotel(EmailNotifiedSurvey emailInfo)
        {
            var mail = BuildMailInfo(EmailConfig.DefaultSendFromEmail, emailInfo.UserEmail.FirstOrDefault());

            if (emailInfo.UserEmail.Any())
            {
                // Remove First Email
                emailInfo.UserEmail.RemoveAt(0);

                emailInfo.UserEmail.ForEach(emailAddress =>
                {
                    mail.AddTo(new EmailAddress(emailAddress));
                });
            }

            mail.TemplateId = EmailConfig.SendNotifiedSurveyHotel;
            mail.AddSubstitution("[HotelName]", emailInfo.HotelName);
            mail.AddSubstitution("[ProductName]", emailInfo.ProductName);
            mail.AddSubstitution("[Quantity]", emailInfo.Quantity);
            mail.AddSubstitution("[CustomerFullName]", emailInfo.CustomerFullName);
            mail.AddSubstitution("[CheckInDate]", emailInfo.CheckInDate);
            mail.AddSubstitution("[rating]", emailInfo.RatingString);
            mail.AddSubstitution("[RateCommend]", emailInfo.RateCommend);
            mail.AddSubstitution("[TotalSpend]", emailInfo.TotalSpend);
            //mail.AddSubstitution("[UrlSurveyAdmin]", emailInfo.UrlSurveyAdmin);

            if (emailInfo.EmailBccList.Any())
            {
                emailInfo.EmailBccList.ForEach(email =>
                {
                    mail.AddBcc(new EmailAddress(email));
                });
            }

            if (!string.IsNullOrEmpty(emailInfo.ReplyToEmail))
            {
                mail.ReplyTo = new EmailAddress(emailInfo.ReplyToEmail, emailInfo.ReplyToName);
            }

            dynamic response = await sendgrid.SendEmailAsync(mail);
            HttpStatusCode statusCode = response.StatusCode;
            string result = response.Body.ReadAsStringAsync().Result;
            HttpResponseHeaders header = response.Headers;
            return statusCode + " - " + result + " - " + header;
        }

        public static async Task<string> EmailNotifiedSurvey(EmailNotifiedSurvey emailInfo)
        {
            var mail = BuildMailInfo(EmailConfig.DefaultSendFromEmail, emailInfo.UserEmail.FirstOrDefault());

            if (emailInfo.UserEmail.Any())
            {
                // Remove First Email
                emailInfo.UserEmail.RemoveAt(0);

                emailInfo.UserEmail.ForEach(emailAddress =>
                {
                    mail.AddTo(new EmailAddress(emailAddress));
                });
            }

            mail.TemplateId = EmailConfig.SendNotifiedSurvey;
            mail.AddSubstitution("[HotelName]", emailInfo.HotelName);
            mail.AddSubstitution("[ProductName]", emailInfo.ProductName);
            mail.AddSubstitution("[Quantity]", emailInfo.Quantity);
            mail.AddSubstitution("[CustomerFullName]", emailInfo.CustomerFullName);
            mail.AddSubstitution("[CheckInDate]", emailInfo.CheckInDate);
            mail.AddSubstitution("[rating]", emailInfo.RatingString);
            mail.AddSubstitution("[RateCommend]", emailInfo.RateCommend);
            mail.AddSubstitution("[TotalSpend]", emailInfo.TotalSpend);
            //mail.AddSubstitution("[UrlSurveyAdmin]", emailInfo.UrlSurveyAdmin);

            if (emailInfo.EmailBccList.Any())
            {
                emailInfo.EmailBccList.ForEach(email =>
                {
                    mail.AddBcc(new EmailAddress(email));
                });
            }

            if (!string.IsNullOrEmpty(emailInfo.ReplyToEmail))
            {
                mail.ReplyTo = new EmailAddress(emailInfo.ReplyToEmail, emailInfo.ReplyToName);
            }

            dynamic response = await sendgrid.SendEmailAsync(mail);
            HttpStatusCode statusCode = response.StatusCode;
            string result = response.Body.ReadAsStringAsync().Result;
            HttpResponseHeaders header = response.Headers;
            return statusCode + " - " + result + " - " + header;
        }

        public static async Task<string> EmailEmailMonthlySalesReport(EmailMonthlySalesReportInfo emailInfo)
        {
            var mail = BuildMailInfo(EmailConfig.DefaultSendFromEmail, emailInfo.UserEmail.First());

            // Remove First Email
            emailInfo.UserEmail.RemoveAt(0);

            if (emailInfo.UserEmail.Any())
            {
                emailInfo.UserEmail.ForEach(emailAddress =>
                {
                    mail.AddTo(new EmailAddress(emailAddress));
                });
            }

            mail.TemplateId = EmailConfig.HotelReportMonthlySummary;
            mail.AddSubstitution("[HotelNameWithNeighborhood]", emailInfo.HotelNameWithNeighborhood);
            mail.AddSubstitution("[SelectedMonth]", emailInfo.SelectedMonth);
            mail.AddSubstitution("[GrossSales]", emailInfo.GrossSales);
            mail.AddSubstitution("[IncreaseFromMonth]", emailInfo.IncreaseFromMonth);
            mail.AddSubstitution("[IncreaseFromPreviousMonth]", emailInfo.IncreaseFromPreviousMonth);
            mail.AddSubstitution("[GrossSalesDayPass]", emailInfo.GrossSalesDayPass);
            mail.AddSubstitution("[GrossSalesCabana]", emailInfo.GrossSalesCabana);
            mail.AddSubstitution("[Incremental]", emailInfo.Incremental);
            mail.AddSubstitution("[TicketSold]", emailInfo.TicketSold);
            mail.AddSubstitution("[TicketSoldDayPass]", emailInfo.TicketSoldDayPass);
            mail.AddSubstitution("[TicketSoldCabana]", emailInfo.TicketSoldCabana);
            mail.AddSubstitution("[Utilization]", emailInfo.Utilization);
            mail.AddSubstitution("[UtilizationWeedDays]", emailInfo.UtilizationWeedDays);
            mail.AddSubstitution("[UtilizationWeedkens]", emailInfo.UtilizationWeedkens);
            mail.AddSubstitution("[AvgSpendGuest]", emailInfo.AvgSpendGuest);
            mail.AddSubstitution("[AvgSpendCabana]", emailInfo.AvgSpendCabana);

            dynamic response = await sendgrid.SendEmailAsync(mail);
            HttpStatusCode statusCode = response.StatusCode;
            string result = response.Body.ReadAsStringAsync().Result;
            HttpResponseHeaders header = response.Headers;
            return statusCode + " - " + result + " - " + header;
        }

        public static async Task<string> EmailSubscriptionConfirmation(EmailSubscriptionConfirmationInfo emailInfo)
        {
            var mail = BuildMailInfo(EmailConfig.DefaultSendFromEmail, emailInfo.UserEmail);

            mail.TemplateId = EmailConfig.EmailSubscriptionConfirmation;
            mail.AddSubstitution("[FirstName]", emailInfo.FirstName);
            mail.AddSubstitution("[FullName]", emailInfo.FullName);
            mail.AddSubstitution("[SubscriptionName]", emailInfo.SubscriptionName);
            mail.AddSubstitution("[ImageUrl]", emailInfo.ImageUrl);
            mail.AddSubstitution("[SubscriptionDescription]", emailInfo.SubscriptionDescription);
            mail.AddSubstitution("[UrlReserveDayPass]", emailInfo.UrlReserveDayPass);
            mail.AddSubstitution("[PurchasedDate]", emailInfo.PurchasedDate);
            mail.AddSubstitution("[MembershipId]", emailInfo.MembershipId);
            mail.AddSubstitution("[Price]", emailInfo.Price);
            mail.AddSubstitution("[Credit]", emailInfo.Credit);
            mail.AddSubstitution("[TotalPrice]", emailInfo.TotalPrice);
            mail.AddSubstitution("[FinePrint]", emailInfo.FinePrint);
            mail.AddSubstitution("[UrlLinkToTerms]", emailInfo.UrlLinkToTerms);
            mail.AddSubstitution("[Discount]", emailInfo.Discount);
            mail.AddSubstitution("[DiscountFinePrint]", emailInfo.DiscountFinePrint);

            // BCC Email For
            mail.AddBcc(new EmailAddress(EmailConfig.DefaultSendFromEmail));

            dynamic response = await sendgrid.SendEmailAsync(mail);
            HttpStatusCode statusCode = response.StatusCode;
            string result = response.Body.ReadAsStringAsync().Result;
            HttpResponseHeaders header = response.Headers;
            return statusCode + " - " + result + " - " + header;
        }

        public static async Task<string> EmailGoldPassNoShow(EmailGoldPassNoShowInfo emailInfo)
        {
            var mail = BuildMailInfo(EmailConfig.DefaultSendFromEmail, emailInfo.UserEmail);

            mail.TemplateId = EmailConfig.EmailGoldPassNoShow;
            mail.AddSubstitution("[FirstName]", emailInfo.FirstName);
            mail.AddSubstitution("[FullName]", emailInfo.FullName);
            mail.AddSubstitution("[CheckinDate]", emailInfo.CheckinDate);
            mail.AddSubstitution("[BookingCode]", emailInfo.BookingCode);
            mail.AddSubstitution("[ChargeDate]", emailInfo.ChargeDate);
            mail.AddSubstitution("[ChargeAmount]", emailInfo.ChargeAmount);
            mail.AddSubstitution("[ChargeAccount]", emailInfo.ChargeAccount);
            mail.AddSubstitution("[ProductName]", emailInfo.ProductName);
            mail.AddSubstitution("[Hotel_Name]", emailInfo.HotelName);
            mail.AddSubstitution("[Neighborhood]", emailInfo.Neighborhood);

            // BCC Email For
            mail.AddBcc(new EmailAddress(EmailConfig.DefaultSendFromEmail));

            dynamic response = await sendgrid.SendEmailAsync(mail);
            HttpStatusCode statusCode = response.StatusCode;
            string result = response.Body.ReadAsStringAsync().Result;
            HttpResponseHeaders header = response.Headers;
            return statusCode + " - " + result + " - " + header;
        }

        public static async Task<string> EmailSubscriptionCancellation(EmailSubscriptionCancellationInfo emailInfo)
        {
            var mail = BuildMailInfo(EmailConfig.DefaultSendFromEmail, emailInfo.UserEmail);

            mail.TemplateId = EmailConfig.EmailSubscriptionCancellation;
            mail.AddSubstitution("[FirstName]", emailInfo.FirstName);
            mail.AddSubstitution("[EndCurrentCycle]", emailInfo.EndCurrentCycle);

            // BCC Email For
            mail.AddBcc(new EmailAddress(EmailConfig.DefaultSendFromEmail));

            dynamic response = await sendgrid.SendEmailAsync(mail);
            HttpStatusCode statusCode = response.StatusCode;
            string result = response.Body.ReadAsStringAsync().Result;
            HttpResponseHeaders header = response.Headers;
            return statusCode + " - " + result + " - " + header;
        }

        public static async Task<string> EmaileGiftCardConfirmation(EmaileGiftCardConfirmation emailInfo)
        {
            var mail = BuildMailInfo(EmailConfig.DefaultSendFromEmail, emailInfo.UserEmail);

            mail.TemplateId = EmailConfig.EmaileGiftCardConfirmation;
            mail.AddSubstitution("[FirstName]", emailInfo.FirstName);
            mail.AddSubstitution("[GiftAmount]", emailInfo.GiftAmount);
            mail.AddSubstitution("[Recipient]", emailInfo.Recipient);
            mail.AddSubstitution("[EmailTo]", emailInfo.EmailTo);
            mail.AddSubstitution("[DeliveryDate]", emailInfo.DeliveryDate);
            mail.AddSubstitution("[Message]", emailInfo.Message);
            mail.AddSubstitution("[Purchased]", emailInfo.Purchased);
            mail.AddSubstitution("[GiftCardId]", emailInfo.GiftCardId);
            mail.AddSubstitution("[Credit]", emailInfo.Credit);
            mail.AddSubstitution("[BookingTotal]", emailInfo.Total);

            // BCC Email For
            mail.AddBcc(new EmailAddress(EmailConfig.DefaultSendFromEmail));

            dynamic response = await sendgrid.SendEmailAsync(mail);
            HttpStatusCode statusCode = response.StatusCode;
            string result = response.Body.ReadAsStringAsync().Result;
            HttpResponseHeaders header = response.Headers;
            return statusCode + " - " + result + " - " + header;
        }

        public static async Task<string> EmailDeliveryeGiftCard(EmailDeliveryGiftCardInfo emailInfo)
        {
            var mail = BuildMailInfo(EmailConfig.DefaultSendFromEmail, emailInfo.UserEmail);

            mail.TemplateId = EmailConfig.EmaileGiftCardDelivery;
            mail.AddSubstitution("[FullName]", emailInfo.FullName);
            mail.AddSubstitution("[UrlImage]", emailInfo.UrlImage);
            mail.AddSubstitution("[Amount]", emailInfo.Amount);
            mail.AddSubstitution("[GiftCode]", emailInfo.GiftCode);
            mail.AddSubstitution("[Message]", emailInfo.Message);
            mail.AddSubstitution("[UrlRedeem]", emailInfo.UrlRedeem);

            mail.AddBcc(new EmailAddress(EmailConfig.DefaultSendFromEmail));

            dynamic response = await sendgrid.SendEmailAsync(mail);
            HttpStatusCode statusCode = response.StatusCode;
            string result = response.Body.ReadAsStringAsync().Result;
            HttpResponseHeaders header = response.Headers;
            return statusCode + " - " + result + " - " + header;
        }

        private static SendGridMessage BuildMailInfo(string fromEmail, string toEmail)
        {
            SendGridMessage mail = new SendGridMessage
            {
                From = new EmailAddress(fromEmail, "DayAxe"),
                ReplyTo = new EmailAddress(EmailConfig.HelpEmail, "DayAxe")
            };
            if (!string.IsNullOrEmpty(toEmail))
            {
                mail.AddTo(new EmailAddress(toEmail));
            }

            return mail;
        }

        //public static void AddSupportToBccEmail(ref EmailInfo emailInfo)
        //{
        //    if (emailInfo.EmailBccList == null || emailInfo.EmailBccList.Count == 0)
        //    {
        //        emailInfo.EmailBccList = new List<string> { EmailConfig.SupportEmailDayaxe };
        //    }
        //    else
        //    {
        //        emailInfo.EmailBccList.Add(EmailConfig.SupportEmailDayaxe);
        //    }

        //    emailInfo.EmailBccList = emailInfo.EmailBccList.Distinct().ToList();
        //}

        public static void AddBookingsToBccEmail(ref EmailInfo emailInfo)
        {
            if (emailInfo.EmailBccList == null || emailInfo.EmailBccList.Count == 0)
            {
                emailInfo.EmailBccList = new List<string> { EmailConfig.DefaultSendFromEmail };
            }
            else
            {
                emailInfo.EmailBccList.Add(EmailConfig.DefaultSendFromEmail);
            }

            emailInfo.EmailBccList = emailInfo.EmailBccList.Distinct().ToList();
        }
    }
}
