using System;
using System.Collections.Generic;
using System.Linq;
using DayaxeDal;
using DayaxeDal.Repositories;

namespace h.dayaxe.com
{
    public partial class Setting : BasePageProduct
    {
        readonly HotelRepository _hotelRepository = new HotelRepository();
        protected void Page_Init(object sender, EventArgs e)
        {
            Session["Active"] = "Settings";
            if (!IsPostBack && PublicHotel != null)
            {
                BookingConfirmEmailText.Text = PublicHotel.BookingConfirmationEmail;
                BookingConfirmEmailCabanasText.Text = PublicHotel.BookingConfirmationCabanaEmail;
                BookingConfirmEmailSpaText.Text = PublicHotel.BookingConfirmationSpaEmail;
                PinCode.Text = PublicHotel.HotelPinCode;
                ReportSubscribersText.Text = PublicHotel.ReportSubscribers;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SaveSettingClick(object sender, EventArgs e)
        {
            bool isValidEmail = true;
            MessageLabel.Text = string.Empty;

            PublicHotel.HotelPinCode = PinCode.Text;
            List<string> subcribers = ReportSubscribersText.Text.Replace(" ", "").Split(';').ToList();

            subcribers.ForEach(email =>
            {
                if (!Helper.IsValidEmail(email))
                {
                    MessageLabel.Text = "Please provide valid email for Subcribers";
                    isValidEmail = false;
                }
            });
            PublicHotel.ReportSubscribers = ReportSubscribersText.Text;

            List<string> emailList = BookingConfirmEmailText.Text.Replace(" ", "").Split(';').ToList();
            emailList.AddRange(BookingConfirmEmailCabanasText.Text.Replace(" ", "").Split(';').ToList());
            emailList.AddRange(BookingConfirmEmailSpaText.Text.Replace(" ", "").Split(';').ToList());
            emailList.ForEach(email =>
            {
                if (!Helper.IsValidEmail(email))
                {
                    MessageLabel.Text = "Please provide valid email";
                    isValidEmail = false;
                }
            });

            if (!isValidEmail)
            {
                return;
            }

            PublicHotel.BookingConfirmationEmail = BookingConfirmEmailText.Text.Replace(" ", "");
            PublicHotel.BookingConfirmationCabanaEmail = BookingConfirmEmailCabanasText.Text.Replace(" ", "");
            PublicHotel.BookingConfirmationSpaEmail = BookingConfirmEmailSpaText.Text.Replace(" ", "");

            _hotelRepository.Update(PublicHotel);

            _hotelRepository.ResetCache();
        }
    }
}