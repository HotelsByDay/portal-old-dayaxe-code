using System;
using AutoMapper;
using DayaxeDal;
using DayaxeDal.Repositories;

namespace dayaxe.com.GiftCards
{
    public partial class ConfirmGiftCardPage : BasePage
    {
        protected GiftCardBookings PublicGiftCardBooking { get; set; }

        protected DayaxeDal.GiftCards PublicGiftCards { get; set; }

        private readonly CustomerInfoRepository _customerInfoRepository = new CustomerInfoRepository();

        private readonly GiftCardBookingRepository _giftCardBookingRepository = new GiftCardBookingRepository();


        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                if (Page.RouteData.Values["GiftCardBookingId"] != null)
                {
                    int giftCardBookingId;
                    if (int.TryParse(Page.RouteData.Values["GiftCardBookingId"].ToString(), out giftCardBookingId))
                    {
                        PublicGiftCardBooking = _giftCardBookingRepository.GetById(giftCardBookingId);
                        PublicGiftCards = _giftCardBookingRepository.GetGiftCard(PublicGiftCardBooking.GiftCardId);

                        if (PublicGiftCardBooking.PayByCredit.Equals(0))
                        {
                            creditRow.Visible = false;
                        }
                        CreditPrice.Text = Helper.FormatPrice(PublicGiftCardBooking.PayByCredit * -1);
                        var totalPrice = PublicGiftCardBooking.TotalPrice - PublicGiftCardBooking.PayByCredit;
                        TotalPriceLit.Text = Helper.FormatPrice(totalPrice);
                    }
                }

                if (PublicCustomerInfos == null ||
                    PublicGiftCardBooking != null &&
                    PublicCustomerInfos.CustomerId != PublicGiftCardBooking.CustomerId)
                {
                    Response.Redirect(Constant.InvalidTicketPage, false);
                }

                if (PublicGiftCardBooking == null)
                {
                    if (PublicCustomerInfos != null && !string.IsNullOrEmpty(PublicCustomerInfos.BrowsePassUrl))
                    {
                        Response.Redirect(PublicCustomerInfos.BrowsePassUrl, true);
                    }
                    Response.Redirect(!string.IsNullOrEmpty((string)Session["SearchPage"])
                        ? Session["SearchPage"].ToString()
                        : Constant.SearchPageDefault, true);
                }

                if (PublicCustomerInfos == null || string.IsNullOrEmpty(PublicCustomerInfos.EmailAddress))
                {
                    PublicCustomerInfos = _customerInfoRepository.GetById(PublicGiftCardBooking.CustomerId);
                }
                if (!PublicCustomerInfos.IsConfirmed)
                {
                    PasswordErrorMessage.Visible = false;
                    CustomerMV.ActiveViewIndex = 0;
                }
            }
            catch (Exception ex)
            {
                var logs = new Logs
                {
                    LogKey = "Subscription-Confirmation-Error",
                    UpdatedDate = DateTime.UtcNow,
                    UpdatedContent = string.Format("Subscription Confirmation Error - {0} - {1} - {2}", ex.Message, ex.StackTrace, ex.Source),
                    UpdatedBy = PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 0
                };
                _giftCardBookingRepository.AddLog(logs);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            Session["IsRegister"] = null;
        }

        protected void ConfirmButtonClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Password.Text))
            {
                PasswordErrorMessage.Visible = true;
                PasswordErrorMessage.Text = ErrorMessage.EnterPassword;
                return;
            }

            if (Password.Text.Length < 7)
            {
                PasswordErrorMessage.Visible = true;
                PasswordErrorMessage.Text = ErrorMessage.PasswordNotValid;
                return;
            }

            if (Password.Text != PasswordConfirm.Text)
            {
                PasswordErrorMessage.Visible = true;
                PasswordErrorMessage.Text = ErrorMessage.ConfirmPasswordNotValid;
                return;
            }

            var customerInfo = Mapper.Map<CustomerInfos>(PublicCustomerInfos);

            customerInfo.Password = Password.Text;
            customerInfo.IsConfirmed = true;
            _customerInfoRepository.Update(customerInfo);

            CustomerMV.ActiveViewIndex = 1;
        }
    }
}