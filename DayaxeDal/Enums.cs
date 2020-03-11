using System.ComponentModel;

namespace DayaxeDal
{
    public class Enums
    {
        public enum Hoteltype
        {
            FESTIVE = 0,
            TRANQUIL = 1,
            FAMILY = 2,
            BASIC = 3
        }

        public enum PassStatus
        {
            [Description("Expired")]
            Expired = 1,
            [Description("Redeemed")]
            Redeemed = 2,
            [Description("Not Redeemed")]
            NotRedeemed = 3
        }

        public enum AmentyType
        {
            Pool = 0,
            Gym = 1,
            Spa = 2,
            BusinessCenter = 3,
            Other = 4,
            Dining = 5,
            Event = 6
        }

        public enum BookingStatus
        {
            Active = 0,
            Expired = 1,
            Redeemed = 2,
            Refunded = 3,
            Capture = 4
        }

        public enum SubscriptionBookingStatus
        {
            Active = 0,
            End = 1,
            Suspended = 2
        }

        public enum TargetGroups
        {
            [Description("Urban <br/>Professionals")]
            UrbanProfessionals = 0,
            [Description("Twenty <br/>Somethings")]
            TwentySomethings = 1,
            [Description("Savvy <br/>Singles")]
            SavvySingles = 2,
            [Description("Parents")]
            Parents = 3,
            [Description("Couples")]
            Couples = 4,
            [Description("Creatives")]
            Creatives = 5
        }

        public enum Gender
        {
            [Description("Female")]
            Female = 0,
            [Description("Male")]
            Male = 1
        }

        public enum Education
        {
            [Description("PhD+")]
            PhD = 0,
            [Description("Master's <br/>Degree")]
            MasterDegree = 1,
            [Description("Bachelor's <br/>Degree")]
            BachelorDegree = 2,
            [Description("Some <br/>College")]
            SomeColledge = 3,
            [Description("High <br/>School Diploma")]
            HighSchoolDiploma = 4,
            [Description("Below <br/>High School")]
            BelowHighSchool = 5
        }

        public enum ScheduleType
        {
            NotRun = 0,
            Complete = 1
        }

        public enum ScheduleSendType
        {
            IsMailConfirm = 1,
            IsRecalculateExpiredDate = 2,
            IsBookingAlert = 3,
            IsEmailRefund = 4,
            IsEmailSurvey = 5,
            IsAddOnNotification = 6,
            IsAddOnNotificationAfterRedemption = 7,
            IsEmailWaitingList = 10,
            IsNotifiedSurvey = 11,
            IsEmailMonthlySalesReport = 12,
            IsEmailConfirmSubscription = 13,
            IsEmailGoldPassNoShow = 14,
            IsEmailSubscriptionCancellation = 15,
            IsEmailGiftCardConfirmation = 16
        }

        public enum DiscountStatus
        {
            Scheduled = 0,
            Active = 1,
            Ended = 2
        }

        public enum ProductType
        {
            [Description("Day Pass")]
            DayPass = 0,
            [Description("Cabana")]
            Cabana = 1,
            [Description("Spa Pass")]
            SpaPass = 2,
            [Description("Daybed")]
            Daybed = 3,
            [Description("Add-On")]
            AddOns = 4
        }

        public enum SubscriptionType
        {
            [Description("Subscription")]
            Subscription = 0,
            [Description("Gift Card")]
            GiftCard = 1
        }

        public enum BookingsTempStatus
        {
            Active = 0,
            Upgrade = 1,
            NotUpgrade = 2,
            ExitBrowser = 3
        }

        public enum SelectedCheckedInDateType
        {
            Later = 0,
            Now = 1
        }

        public enum InvoiceStatus
        {
            Charge = 0,
            PartialRefund = 1,
            FullRefund = 2
        }

        public enum PromoType
        {
            Percent = 0,
            Fixed = 1,
            SubscriptionPromo = 2
        }

        public enum KidAllowType
        {
            [Description("Not Allowed")]
            NotAllow = 0,
            [Description("Allowed")]
            Allowed = 1,
            [Description("Allowed, at full price")]
            AllowedFullPrice = 2
        }

        public enum CreditType
        {
            [Description("Referral")]
            Referral = 0,
            [Description("Partial Refund")]
            PartialPuchaseRefund = 1,
            [Description("Full Refund")]
            FullPurchaseRefund = 2,
            [Description("Gift Card")]
            GiftCard = 3,
            [Description("Charge")]
            Charge = 4
        }

        public enum PaymentType
        {
            NotSelected = 0,
            [Description("Dayaxe Credit")]
            DayAxeCredit = 1,
            [Description("Stripe")]
            Stripe = 2,
            [Description("Credit And Stripe")]
            CreditAndStripe
        }

        public enum CreditStatusType
        {
            Pending = 0,
            Completed = 1
        }

        public enum GiftCardType
        {
            Available = 0,
            Used = 1
        }

        public enum MonthType
        {
            January = 1,
            February = 2,
            March = 3,
            April = 4,
            May = 5,
            June = 6,
            July = 7,
            August = 8,
            September = 9,
            October = 10,
            November = 11,
            December = 12
        }

        public enum DayType
        {
            Monday = 1,
            Tueday = 2,
            Wednesday = 3,
            Thursday = 4,
            Friday = 5,
            Saturday = 6,
            Sunday = 7
        }

        public enum RoleType
        {
            NormalUser = 0,
            IsSuperAdmin = 1,
            IsAdmin = 2,
            IsCheckInOnly = 3
        }
    }
}
