namespace DayaxeDal
{
    public static class ErrorMessage
    {
        public const string InvalidCode = "Invalid Code";
        public const string CanNotReferralTwice = "You can only be referred once";
        public const string CreditHasBeenApplied = "Credit has been applied!";
        public const string EnjoyDayCations = "Refer a friend by giving them your personal code below. Your friend will get a {0} credit towards their first purchase and you will receive a {1} credit after they purchase!";
        public const string DoNotHavePurchase = "Owner of code does not have a purchase.";
        public const string YouAlreadyHaveBookings = "Cannot be referred. You already have bookings.";
        public const string GiftCardHasBeenUsed = "Gift card has been used";
        public const string GiftCardHasBeenApplied = "Gift card has been applied!";
        public const string MinAmountNotMatch = "Min amount is not met, code will not apply.";
        public const string YouMustLogIn = "To apply Gift Card or Credit you must have an active account. Please sign up and try again.";
        public const string PleaseCheckYourGiftCardWithCurrentAccount = "This gift card was meant for another customer. You cannot use this gift card.";

        public const string DoNotHaveAccessOnThisDate = "You don't have access to this hotel/resort on this date. Please check your bookings and correct your check-in date.";
        public const string RestrictBookingAfterHour = "Cannot book same day ticket after {0}.";
        public const string MembershipNotActive = "Your membership is not active.";

        public const string MasterNotFound = "Master Not found";
        public const string NotFound = "Not found";
        public const string EnterPassword = "Please enter your password";
        public const string PasswordNotValid = "Password should be minimum 7 characters.";
        public const string ConfirmPasswordNotValid = "Please enter valid Confirm Password";
        public const string BookingsExists = "Your selected days have bookings. " +
                                             "You cannot block those days. " +
                                             "Please contact DayAxe team if you need further assistance.";

        public const string PasswordCanNotEmpty = "Password can not empty";
        public const string PasswordCanNotContainSpace = "Password can not contains space characters";
        public const string FirstLastNameCanNotEmpty = "First name and Last name can not empty";
        public const string EmailIsInvalid = "Please enter valid your email";
        public const string UpdateSuccess = "Update success.";

        public const string FailedToUpdated = "Failed to update account details.";
        public const string SuccessfullyToUpdated = "Successfully updated your account details!";
        public const string OldPasswordNotValid = "Old password is not valid!";
    }
}
