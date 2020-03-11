using System;
using System.Configuration;
using System.Globalization;

namespace Dayaxe.SendEmail
{
    public static class EmailConfig
    {
        public static string FullStar
        {
            get
            {
                return ConfigurationManager.AppSettings["cdnImageUrlDefault"] + "/images/star_f_full.png";
            }
        }

        public static string HalfStar
        {
            get
            {
                return ConfigurationManager.AppSettings["cdnImageUrlDefault"] + "/images/star_f_half.png";
            }
        }

        public static string EmptyStar
        {
            get
            {
                return ConfigurationManager.AppSettings["cdnImageUrlDefault"] + "/images/star_f_empty.png";
            }
        }

        public static string ImageLogo
        {
            get
            {
                return ConfigurationManager.AppSettings["cdnImageUrlDefault"] + "/images/logo.png";
            }
        }

        //public static string SendEmailApikeys = "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";
        //Stage 
        //public static string SendEmailApikeys = "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";
        public static string SendEmailApikeys { get { return ConfigurationManager.AppSettings["sendgridApiKey"]; } }

        public static string DefaultImageUrlSendEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["defaultImageUrlSendEmail"];
            }
        }
        

        public static DateTime ReminderAlertTime
        {
            get
            {
                DateTime val;
                DateTime.TryParseExact(ConfigurationManager.AppSettings["reminderAlertTime"], "H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out val);
                return val;
            }
        }

        public static DateTime SalesReportSendDateTime
        {
            get
            {
                DateTime val;
                DateTime.TryParseExact(ConfigurationManager.AppSettings["salesReportSendDateTime"], "dd H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out val);
                return val;
            }
        }

        public static long SendEmailSurveyAfterMinutes
        {
            get
            {
                long val = 0;
                long.TryParse(ConfigurationManager.AppSettings["sendEmailSurveyAfterMinutes"], out val);
                return val;
            }
        }

        public static string DefaultSendSurveyEmail
        {
            get { return ConfigurationManager.AppSettings["defaultSendSurveyEmail"]; }
        }

        //public const string EmailNewAccount = "fc6d8bc9-2cc9-407d-9d08-58fb675f4a54";
        //Stage
        //public const string EmailNewAccount = "dd8c7dd4-8b97-4f1a-947b-80d92a849047";
        public static string EmailNewAccount { get { return ConfigurationManager.AppSettings["sendgridEmailNewAccount"]; } }


        //emailConfirmation.html
        //Guest: Booking Confirmation
        //public const string EmailBookingConfirmation = "cf481d67-3049-4b95-a15a-7d4ab93671ad";
        //Stage
        //public const string EmailBookingConfirmation = "3b72904b-d8a3-4742-830e-acc6ed96a949";
        public static string EmailBookingConfirmation { get { return ConfigurationManager.AppSettings["sendgridEmailBookingConfirmation"]; } }


        //hostEmailConfirmation.html
        //Host Alert: Redemption
        //public const string EmailBookingconfirmationOfHotel = "7b281ded-2910-4cc9-b248-de9e6b42d515";
        //Stage
        //public const string EmailBookingconfirmationOfHotel = "c97780bf-0732-43f3-a18b-7e493bfc6f1e";
        public static string EmailBookingconfirmationOfHotel { get { return ConfigurationManager.AppSettings["sendgridEmailBookingconfirmationOfHotel"]; } }


        //hostEmailConfirmationAlert.html
        //Host Alert: Redemption (check-in)
        //public const string EmailBookingconfirmationAlertOfHotel = "";
        //Stage
        //public const string EmailBookingconfirmationAlertOfHotel = "8da6e98e-22c7-4b9b-936f-471fc414a8db";

        //hostBookingAlert.html
        //Host Alert: Booking
        //public const string EmailBookingAlertOfHotel = "f8b7e99c-eff6-439d-88d0-d5fd3fdc1058";
        //Stage
        //public const string EmailBookingAlertOfHotel = "536ce054-908f-4ec4-9548-85cc96980f25";
        public static string EmailBookingAlertOfHotel { get { return ConfigurationManager.AppSettings["sendgridEmailBookingAlertOfHotel"]; } }

        //public const string EmailPostPurchaseSurvey = "933dfb53-ab82-4b8e-a188-51f8583041b1";
        //Stage
        //public const string EmailPostPurchaseSurvey = "b6891dc0-bd04-4996-9647-214552fb9d80";
        public static string EmailPostPurchaseSurvey { get { return ConfigurationManager.AppSettings["sendgridEmailPostPurchaseSurvey"]; } }

        //public const string EmailPasswordRecovery = "8f584079-82dc-4e7a-8bfe-85f905275e3b";
        //Stage
        //public const string EmailPasswordRecovery = "739dc30e-88cf-4029-bfc7-76dd796ca674";
        public static string EmailPasswordRecovery { get { return ConfigurationManager.AppSettings["sendgridEmailPasswordRecovery"]; } }

        //public const string EmailPasswordCreated = "0a459be6-28d1-48e3-929d-82f2a3ead40a";
        //Stage
        //public const string EmailPasswordCreated = "e5c9f810-2528-459b-bf5a-d0600ac61df3";
        public static string EmailPasswordCreated { get { return ConfigurationManager.AppSettings["sendgridEmailPasswordCreated"]; } }

        //public const string EmailRefund = "b837577b-5291-4b7d-a339-fa60e5005c66";
        //Stage
        //public const string EmailRefund = "d3a8443d-4be2-4ae9-abc3-32612a2ded14";
        public static string EmailRefund { get { return ConfigurationManager.AppSettings["sendgridEmailRefund"]; } }

        //guestalertCheckInReminder.html
        //public const string CheckInReminder = "3bb45302-a233-4b8d-8a9f-7610c88b707b";
        //Stage
        //public const string CheckInReminder = "8499e14e-0e22-4b3f-9638-02de069b00c1";
        public static string CheckInReminder { get { return ConfigurationManager.AppSettings["sendgridCheckInReminder"]; } }

        //AvailableAddOn.html
        //public const string AvailableAddOn = "f52e5e66-ed1c-423b-afb2-cc02e413b134";
        //Stage
        //public const string AvailableAddOn = "8cf35f89-3e98-41bc-901b-f6d0a180310d";
        public static string AvailableAddOn { get { return ConfigurationManager.AppSettings["sendgridAvailableAddOn"]; } }

        //AvailableAddOn_redemption.html
        //public const string AvailableAddOnRedemption = "0097ebba-6163-419e-813d-02b1e3fce2ac";
        //Stage
        //public const string AvailableAddOnRedemption = "75a08a4e-1e76-481f-866e-daf1efb41c14";
        public static string AvailableAddOnRedemption { get { return ConfigurationManager.AppSettings["sendgridAvailableAddOnRedemption"]; } }

        //booking_change_guest.html
        //public const string GuestBookingChange = "cf481d67-3049-4b95-a15a-7d4ab93671ad";
        //Stage
        //public const string GuestBookingChange = "b8aebe0b-7c8d-4bc0-aa0d-b9da38ebd670";

        //booking_change_hotel_alert.html
        //public const string HotelAlertBookingChange = "7b281ded-2910-4cc9-b248-de9e6b42d515";
        //Stage
        //public const string HotelAlertBookingChange = "6005dac0-c080-4ffe-8fb2-3b603b42731c";

        //ticket_open_up.html
        //public const string TicketOpenedUp = "2d049a9d-6835-4f42-a62a-6971ca8cf337";
        //Stage
        //public const string TicketOpenedUp = "f09ecf37-82d3-408b-adfc-b44821e18e5c";
        public static string TicketOpenedUp { get { return ConfigurationManager.AppSettings["sendgridTicketOpenedUp"]; } }

        // 
        //public const string SendNotifiedSurveyHotel = "63a7933b-be7a-4059-a6a3-cc611b9defec";
        // Stage
        //public const string SendNotifiedSurveyHotel = "4d24e61c-b395-471b-8bdd-ad1e03ab476d";
        public static string SendNotifiedSurveyHotel { get { return ConfigurationManager.AppSettings["sendgridSendNotifiedSurveyHotel"]; } }

        // 
        //public const string SendNotifiedSurvey = "9e55468d-3592-4d5b-a91f-3a664bac91c8";
        // Stage
        //public const string SendNotifiedSurvey = "26e5d45c-b170-45a7-9262-4f77f9a4b8fc";
        public static string SendNotifiedSurvey { get { return ConfigurationManager.AppSettings["sendgridSendNotifiedSurvey"]; } }

        //public const string HotelReportMonthlySummary = "c1ed05aa-1804-431f-baa9-765225da1f1b";
        // Stage
        //public const string HotelReportMonthlySummary = "782cac08-85ad-4b06-b3ee-e4af79fa943b";
        public static string HotelReportMonthlySummary { get { return ConfigurationManager.AppSettings["sendgridHotelReportMonthlySummary"]; } }

        //public const string EmailSubscriptionConfirmation = "78de7f6d-5dcb-44a1-89d4-0156e08518e9";
        // Stage
        //public const string EmailSubscriptionConfirmation = "25831a43-8a81-42d9-9805-7e6d1a5476b7";
        public static string EmailSubscriptionConfirmation { get { return ConfigurationManager.AppSettings["sendgridEmailSubscriptionConfirmation"]; } }

        //public const string EmailGoldPassNoShow = "75a21ab1-97da-4bb5-8d9b-3ed588a5786e";
        // Stage
        //public const string EmailGoldPassNoShow = "22c4e7d4-4683-46f5-8bb7-c42b5a16b02a";
        public static string EmailGoldPassNoShow { get { return ConfigurationManager.AppSettings["sendgridEmailGoldPassNoShow"]; } }

        //public const string EmailSubscriptionCancellation = "c0bf3930-bf8e-4487-b844-38efabd9c10d";
        // Stage
        //public const string EmailSubscriptionCancellation = "d7e13a5f-74ca-4bae-a252-168f1c748e42";
        public static string EmailSubscriptionCancellation { get { return ConfigurationManager.AppSettings["sendgridEmailSubscriptionCancellation"]; } }

        //public const string EmaileGiftCardConfirmation = "eedaa0e6-ffd8-4c95-ae1b-f8db209786bf";
        // Stage
        //public const string EmaileGiftCardConfirmation = "59b1595a-5d1f-498b-acde-3a5dc091407c";
        public static string EmaileGiftCardConfirmation { get { return ConfigurationManager.AppSettings["sendgridEmaileGiftCardConfirmation"]; } }

        //public const string EmaileGiftCardDelivery = "a0bf1a3f-a31a-4778-aa92-66a7949cb898";
        // Stage
        //public const string EmaileGiftCardDelivery = "ea5ffff9-a3e5-496e-9431-17c035575cc0";
        public static string EmaileGiftCardDelivery { get { return ConfigurationManager.AppSettings["sendgridEmaileGiftCardDelivery"]; } }

        public static string DefaultSendFromEmail
        {
            get { return ConfigurationManager.AppSettings["defaultSendFromEmail"]; }
        }

        public static string TermsUrl
        {
            get { return ConfigurationManager.AppSettings["termsUrl"]; }
        }

        public static string HelpEmail = "help@dayaxe.com";
    }
}
