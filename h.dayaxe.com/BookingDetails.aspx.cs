using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dayaxe.SendEmail;
using DayaxeDal;
using DayaxeDal.Custom;
using DayaxeDal.Extensions;
using DayaxeDal.Parameters;
using DayaxeDal.Repositories;
using Newtonsoft.Json;
using Stripe;

namespace h.dayaxe.com
{
    public partial class BookingDetails : BasePage
    {
        private Bookings CurrentBookingsDetails { get; set; }
        private Bookings BookingsDetails { get; set; }
        private CustomerInfos CustomerInfosDetails { get; set; }
        private Discounts CurrentDiscountUsed { get; set; }
        private Discounts DiscountUsed { get; set; }
        private Products CurrentProduct { get; set; }
        protected Hotels CurrentHotel { get; set; }

        private Discounts PublicDiscountSubscription { get; set; }

        private int CurrentBookingId { get; set; }
        protected string PublicTickets { get; set; }
        protected string ProductBlockoutDate { get; set; }
        private bool IsNeedCalculate { get; set; }

        private BookingRepository _bookingRepository = new BookingRepository();
        private readonly ProductRepository _productRepository = new ProductRepository();
        private readonly DiscountRepository _discountRepository = new DiscountRepository();

        protected void Page_Init(object sender, EventArgs e)
        {
            Session["Active"] = "SearchBookingsList";

            int bookingId;
            int.TryParse(Request.Params["id"], out bookingId);
            if (!IsPostBack)
            {
                Session["CurrentPage"] = 1;
            }

            CurrentBookingId = bookingId;
            AssignBlackOutDate();
            BindBookingStatus();
            BindBookingsData();
            BindCustomerData();
            BindBookingHistories();

            CurrentDiscountUsed = _bookingRepository.GetDiscountUsedByBookingId(BookingsDetails.BookingId);
            var serialize = JsonConvert.SerializeObject(CurrentDiscountUsed, CustomSettings.SerializerSettingsWithFullDateTime());
            DiscountUsed = JsonConvert.DeserializeObject<Discounts>(serialize);
            alternativeTimezone.InnerHtml = CurrentHotel.TimeZoneId.GetTimeZoneInfo();
            alternativeRedemptionTimezone.InnerHtml = CurrentHotel.TimeZoneId.GetTimeZoneInfo();

            // If exists Subscription Promo use on this bookings
            PublicDiscountSubscription = (from d in _bookingRepository.DiscountOfSubscriptionList
                join db in _bookingRepository.DiscountBookingList on d.Id equals db.DiscountId
                where db.BookingId == BookingsDetails.BookingId &&
                      d.PromoType == (byte) Enums.PromoType.SubscriptionPromo
                select d).FirstOrDefault();
            if (!IsPostBack)
            {
                BookingStatusDdl.SelectedValue = ((Enums.BookingStatus)CurrentBookingsDetails.PassStatus).ToString();

                if (CurrentProduct.IsCheckedInRequired)
                {
                    CheckInDateText.Text = string.Format("{0:MMM dd, yyyy}",
                        BookingsDetails.CheckinDate);
                }
                else if (CurrentBookingsDetails.CheckinDate.HasValue)
                {
                    CheckInDateText.Text = string.Format("{0:MMM dd, yyyy}",
                        BookingsDetails.CheckinDate);
                }

                if (CurrentBookingsDetails.ExpiredDate.HasValue)
                {
                    ExpiredDateText.Text = string.Format("{0:MMM dd, yyyy hh:mm tt}",
                        CurrentBookingsDetails.ExpiredDate.Value.ToLosAngerlesTimeWithTimeZone(CurrentHotel.TimeZoneId));
                    alternativeTimezone.Attributes["class"] = alternativeTimezone.Attributes["class"].Replace("hidden", "");
                }
                if (CurrentBookingsDetails.RedeemedDate.HasValue)
                {
                    RedemptionDateText.Text = string.Format("{0:MMM dd, yyyy hh:mm tt}",
                        CurrentBookingsDetails.RedeemedDate.Value.ToLosAngerlesTimeWithTimeZone(CurrentHotel.TimeZoneId));
                    alternativeRedemptionTimezone.Attributes["class"] = alternativeTimezone.Attributes["class"].Replace("hidden", "");
                }

                TicketQtyText.Text = BookingsDetails.Quantity.ToString();
                if (DiscountUsed != null)
                {
                    DiscountUsedText.Text = DiscountUsed.Code;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UpdateSuccess"] != null)
            {
                ChangeCheckInDateMessage.Visible = true;
                ChangeCheckInDateMessage.ForeColor = Color.Red;
                ChangeCheckInDateMessage.Text = (string)Session["UpdateSuccess"];
                Session.Remove("UpdateSuccess");
            }
        }

        private void BindCustomerData()
        {
            StripeCustomerLabel.Text = string.Format("<a target=\"_blank\" class=\"blue\" href=\"{1}\">Stripe Account: {0}</a>",
                CustomerInfosDetails.StripeCustomerId,
                string.Format(Constant.StripeCustomerLink, CustomerInfosDetails.StripeCustomerId));
            FirstNameText.Text = CustomerInfosDetails.FirstName;
            LastNameText.Text = CustomerInfosDetails.LastName;
            CustomerIdText.Text = CustomerInfosDetails.CustomerId.ToString();
            EmailText.Text = CustomerInfosDetails.EmailAddress;
            ZipCodeText.Text = CustomerInfosDetails.ZipCode;
            //PasswordText.Text = CustomerInfosDetails.Password;
            CreatedDateText.Text = string.Format("{0:MMM dd, YYYY hh:mm tt} {1}", 
                CustomerInfosDetails.CreatedDate.ToLosAngerlesTimeWithTimeZone(CurrentHotel.TimeZoneId),
                CurrentHotel.TimeZoneId.GetTimeZoneInfo());
        }

        private void BindBookingsData(bool isReload = false)
        {
            if (isReload)
            {
                _bookingRepository = new BookingRepository();
                CurrentBookingsDetails = _bookingRepository.GetBookings(CurrentBookingId);
                var serialize = JsonConvert.SerializeObject(CurrentBookingsDetails, CustomSettings.SerializerSettingsWithFullDateTime());
                BookingsDetails = JsonConvert.DeserializeObject<Bookings>(serialize);
            }
            StripeTransactionLabel.Text = string.Format("<a target=\"_blank\" class=\"blue\" href=\"{1}\">Stripe Trans: {0}</a>",
                BookingsDetails.StripeChargeId,
                string.Format(Constant.StripePaymentLink, BookingsDetails.StripeChargeId));

            BookingIdText.Text = BookingsDetails.BookingId.ToString();
            BookingCodeText.Text = BookingsDetails.BookingIdString;
            BookingDateText.Text = string.Format("{0:MMM dd, yyyy hh:mmtt} {1}",
                CurrentBookingsDetails.BookedDate.ToLosAngerlesTimeWithTimeZone(CurrentHotel.TimeZoneId),
                CurrentHotel.TimeZoneId.GetTimeZoneInfo());
            ProductTypeText.Text = Helper.GetStringPassByProductType(CurrentProduct.ProductType);
            HotelText.Text = CurrentHotel.HotelName;
            ProductNameText.Text = BookingsDetails.ProductName;
            MerchantPriceText.Text = Helper.FormatPrice(BookingsDetails.MerchantPrice);
            PaidItemPriceText.Text = Helper.FormatPrice(BookingsDetails.HotelPrice);

            BookingTotalText.Text = Helper.FormatPrice(BookingsDetails.TotalPrice);

            if (CurrentBookingsDetails.CancelDated.HasValue)
            {
                RefundDateText.Text = string.Format("{0:MMM dd, yyyy hh:mm tt} {1}",
                    CurrentBookingsDetails.CancelDated.Value.ToLosAngerlesTimeWithTimeZone(CurrentHotel.TimeZoneId),
                    CurrentHotel.TimeZoneId.GetTimeZoneInfo());
                double refundPrice = BookingsDetails.TotalRefundAmount.HasValue ? BookingsDetails.TotalRefundAmount.Value : 0;
                RefundTotalText.Text = Helper.FormatPrice(refundPrice);
                double refundCredit = BookingsDetails.RefundCreditAmount.HasValue ? BookingsDetails.RefundCreditAmount.Value : 0;
                double refundStripe = BookingsDetails.StripeRefundAmount.HasValue ? BookingsDetails.StripeRefundAmount.Value : 0;
                RefundTotalLit.Text = string.Format("Refund Total<br/> (Credit: {0} + Stripe {1})",
                    Helper.FormatPrice(refundCredit), Helper.FormatPrice(refundStripe));
            }

            // Refund
            FullAmountPrice.Text = Helper.FormatPrice(CurrentBookingsDetails.TotalPrice);
        }

        private void BindBookingStatus()
        {
            var status = Helper.EnumToList<Enums.BookingStatus>();
            BookingStatusDdl.DataSource = status;
            BookingStatusDdl.DataTextField = "Key";
            BookingStatusDdl.DataValueField = "Value";
            BookingStatusDdl.DataBind();
        }

        protected void CancelClick(object sender, EventArgs e)
        {
            Response.Redirect(string.Format(Constant.CustomersDetailsAdminPage, CustomerInfosDetails.CustomerId));
        }

        protected void ReSendSurveyClick(object sender, EventArgs e)
        {
            ChangeCheckInDateMessage.Visible = false;
            var url = Helper.GetUrlSendSurvey(BookingsDetails);

            string hotelName = String.Format("{0} at {1}", CurrentProduct.ProductName, CurrentHotel.HotelName);

            var result = EmailHelper.EmailTemplateSurvey(CustomerInfosDetails.EmailAddress,
                hotelName,
                CurrentHotel.Neighborhood,
                CustomerInfosDetails.FirstName,
                url);

            var logs = new Logs
            {
                LogKey = "ResendEmailSurvey",
                UpdatedBy = 1,
                UpdatedDate = DateTime.UtcNow,
                UpdatedContent = string.Format("BookingId: {0} - Email: {1} - Result: {2}", BookingsDetails.BookingIdString, CustomerInfosDetails.EmailAddress, string.Empty)
            };
            _bookingRepository.AddLog(logs);

            Session["UpdateSuccess"] = Message.SendSurveySuccess;
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        protected void SaveBookingsClick(object sender, EventArgs e)
        {
            ChangeCheckInDateMessage.Visible = false;

            var hasChanged = DataChanged();

            if (hasChanged)
            {
                UpdateBookings();
                _bookingRepository.ResetCache();
                Session["UpdateSuccess"] = Message.BookingUpdateSuccessfully;
            }
            else
            {
                Session["UpdateSuccess"] = Message.NothingChange;
            }
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        private bool DataChanged()
        {
            DateTime checkInDate;
            DateTime expiredDate;
            DateTime redemptionDate;
            bool isChanged = BookingStatusDdl.SelectedValue != ((Enums.BookingStatus) BookingsDetails.PassStatus).ToString();

            if (!isChanged && DateTime.TryParseExact(CheckInDateText.Text, "MMM dd, yyyy", null, DateTimeStyles.None,
                out checkInDate))
            {
                IsNeedCalculate = true;
                isChanged = CurrentBookingsDetails.CheckinDate == null || (CurrentBookingsDetails.CheckinDate.Value.Date != checkInDate);
            }

            if (!isChanged && DateTime.TryParseExact(ExpiredDateText.Text, "MMM dd, yyyy h:mm tt", null, DateTimeStyles.None,
                out expiredDate))
            {
                isChanged = CurrentBookingsDetails.ExpiredDate == null || CurrentBookingsDetails.ExpiredDate.Value.ToLosAngerlesTimeWithTimeZone(CurrentHotel.TimeZoneId) != expiredDate;
            }

            if (!isChanged && DateTime.TryParseExact(RedemptionDateText.Text, "MMM dd, yyyy h:mm tt", null, DateTimeStyles.None,
                out redemptionDate))
            {
                isChanged = CurrentBookingsDetails.RedeemedDate == null || CurrentBookingsDetails.RedeemedDate.Value.ToLosAngerlesTimeWithTimeZone(CurrentHotel.TimeZoneId) != redemptionDate;
            }

            if (!isChanged && TicketQtyText.Text != CurrentBookingsDetails.Quantity.ToString())
            {
                IsNeedCalculate = true;
                isChanged = true;
            }

            if (!isChanged && DiscountUsed != null && DiscountUsedText.Text != CurrentDiscountUsed.Code)
            {
                IsNeedCalculate = true;
                isChanged = true;
            }

            return isChanged;
        }

        private void UpdateBookings()
        {
            var logs = new CustomerCreditLogs();
            double diffPrice = 0;
            DateTime checkInDate;
            bool isRedeemed = false;
            if (DateTime.TryParseExact(CheckInDateText.Text, "MMM dd, yyyy", null, DateTimeStyles.None,
                out checkInDate))
            {
                BookingsDetails.CheckinDate = checkInDate.AddHours(Math.Abs(CurrentHotel.TimeZoneOffset));
            }

            DateTime expiredDate;
            if (DateTime.TryParseExact(ExpiredDateText.Text, "MMM dd, yyyy h:mm tt", null, DateTimeStyles.None,
                out expiredDate))
            {
                BookingsDetails.ExpiredDate = expiredDate.AddHours(Math.Abs(CurrentHotel.TimeZoneOffset));
            }

            DateTime redemptionDate;
            if (DateTime.TryParseExact(RedemptionDateText.Text, "MMM dd, yyyy h:mm tt", null, DateTimeStyles.None,
                out redemptionDate))
            {
                BookingsDetails.RedeemedDate = redemptionDate.AddHours(Math.Abs(CurrentHotel.TimeZoneOffset));
            }

            if (BookingStatusDdl.SelectedValue != ((Enums.BookingStatus) BookingsDetails.PassStatus).ToString())
            {
                BookingsDetails.PassStatus = (int)((Enums.BookingStatus)Enum.Parse(typeof(Enums.BookingStatus), BookingStatusDdl.SelectedValue));
                if (BookingsDetails.PassStatus == (int) Enums.BookingStatus.Redeemed && !BookingsDetails.RedeemedDate.HasValue)
                {
                    BookingsDetails.RedeemedDate = DateTime.UtcNow;
                    isRedeemed = true;
                }
            }

            if (IsNeedCalculate)
            {
                int quantity;
                int ticketAvailable;
                int.TryParse(TicketQtyText.Text, out quantity);
                var availableProductParams = new CheckAvailableProductParams
                {
                    ProductId = BookingsDetails.ProductId,
                    CheckInDate = checkInDate,
                    TotalTicket = quantity,
                    BookingId = BookingsDetails.BookingId,
                    IsAdmin = true,
                    TimezoneId = CurrentHotel.TimeZoneId
                };
                var availableProduct = _productRepository.CheckAvailableProduct(availableProductParams, out ticketAvailable);
                if (!availableProduct)
                {
                    ChangeCheckInDateMessage.Text = Message.VerifyUpdate;
                    ChangeCheckInDateMessage.Visible = true;
                    return;
                }
                
                bool isLimit;
                var param = new GetDiscountValidByCodeParams
                {
                    Code = DiscountUsedText.Text.Trim(),
                    ProductId = BookingsDetails.ProductId,
                    CustomerId = BookingsDetails.CustomerId,
                    IsAdmin = true,
                    BookingId = BookingsDetails.BookingId,
                    TimezoneId = CurrentHotel.TimeZoneId
                };
                DiscountUsed = _discountRepository.GetDiscountValidByCode(param, out isLimit);

                if (DiscountUsed == null && isLimit)
                {
                    ChangeCheckInDateMessage.Text = Message.ExceedLimit;
                    ChangeCheckInDateMessage.Visible = true;
                    return;
                }

                if (!BookingsDetails.HasInvoice)
                {
                    MaintainOldInvoices();
                }

                // Exists current booking with Subscription Discount
                if (PublicDiscountSubscription != null)
                {
                    quantity -= 1;
                }

                BookingsDetails.Quantity = quantity;

                var newPrice = _productRepository.GetById(CurrentProduct.ProductId, checkInDate).ActualPriceWithDate;

                double actualPrice = GetActualPrice(newPrice, quantity);

                diffPrice = actualPrice * BookingsDetails.Quantity - BookingsDetails.TotalPrice;

                try
                {
                    // Charge
                    if (diffPrice > 0)
                    {
                        BookingsDetails.HotelPrice = actualPrice;
                        BookingsDetails.MerchantPrice = newPrice.Price;
                        BookingsDetails.TotalPrice = CurrentBookingsDetails.TotalPrice + diffPrice;

                        switch (PaymentMethodDdl.SelectedValue)
                        {
                            case "DayAxeCredit":
                                var market = (from mh in _productRepository.MarketHotelList
                                    join m in _productRepository.MarketList on mh.MarketId equals m.Id
                                    where mh.HotelId == CurrentHotel.HotelId
                                    select m).FirstOrDefault();

                                logs = new CustomerCreditLogs
                                {
                                    CustomerId = CurrentBookingsDetails.CustomerId,
                                    ProductId = CurrentBookingsDetails.ProductId,
                                    Description = string.Format("{0} – {1} – {2} – {3}",
                                        CurrentProduct.ProductName,
                                        CurrentHotel.HotelName,
                                        market != null ? market.LocationName : "",
                                        CurrentBookingsDetails.BookingIdString),
                                    Amount = Math.Abs(diffPrice),
                                    CreatedBy = PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 0,
                                    CreatedDate = DateTime.UtcNow,
                                    CreditType = (int)Enums.CreditType.Charge,
                                    ReferralId = 0,
                                    BookingId = CurrentBookingsDetails.BookingId,
                                    Status = true,
                                    GiftCardId = 0
                                };
                                break;
                            default:
                                Charges(diffPrice, newPrice, actualPrice, CurrentProduct, CurrentHotel, CustomerInfosDetails);
                                BookingsDetails.StripeCardString = GetStripeCardString();
                                break;
                        }
                    }
                    else if (diffPrice < 0) //Refund
                    {
                        BookingsDetails.HotelPrice = actualPrice;
                        BookingsDetails.MerchantPrice = newPrice.Price;
                        BookingsDetails.TotalPrice = CurrentBookingsDetails.TotalPrice - Math.Abs(diffPrice);
                        switch (PaymentMethodDdl.SelectedValue)
                        {
                            case "DayAxeCredit":
                                var market = (from mh in _productRepository.MarketHotelList
                                    join m in _productRepository.MarketList on mh.MarketId equals m.Id
                                    where mh.HotelId == CurrentHotel.HotelId
                                    select m).FirstOrDefault();

                                logs = new CustomerCreditLogs
                                {
                                    CustomerId = CurrentBookingsDetails.CustomerId,
                                    ProductId = CurrentBookingsDetails.ProductId,
                                    Description = string.Format("{0} – {1} – {2} – {3}",
                                        CurrentProduct.ProductName,
                                        CurrentHotel.HotelName,
                                        market != null ? market.LocationName : "",
                                        CurrentBookingsDetails.BookingIdString),
                                    Amount = Math.Abs(diffPrice),
                                    CreatedBy = PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 0,
                                    CreatedDate = DateTime.UtcNow,
                                    CreditType = (int) Enums.CreditType.PartialPuchaseRefund,
                                    ReferralId = 0,
                                    BookingId = CurrentBookingsDetails.BookingId,
                                    Status = true,
                                    GiftCardId = 0
                                };
                                BookingsDetails.PaymentType = (byte)Enums.PaymentType.DayAxeCredit;
                                break;
                            default:
                                Refund(Math.Abs(diffPrice), newPrice, actualPrice);
                                BookingsDetails.PaymentType = (byte)Enums.PaymentType.Stripe;
                                BookingsDetails.StripeCardString = GetStripeCardString();
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ChangeCheckInDateMessage.Text = ex.Message;
                    ChangeCheckInDateMessage.Visible = true;
                    return;
                }

                // Return Quantity after calculate with Discount
                if (PublicDiscountSubscription != null)
                {
                    BookingsDetails.Quantity += 1;
                }
            }

            _bookingRepository.Update(BookingsDetails, DiscountUsed != null ? DiscountUsed.Id : 0, logs);

            if (isRedeemed)
            {
                var schedules = new Schedules
                {
                    BookingId = BookingsDetails.BookingId,
                    Name = "EmailSurvey",
                    ScheduleSendType = (int)Enums.ScheduleSendType.IsEmailSurvey,
                    Status = (int)Enums.ScheduleType.NotRun,
                    SendAt = DateTime.UtcNow.AddMinutes(AppConfiguration.SendEmailSurveyAfterMinutes)
                };
                _bookingRepository.AddSchedule(schedules);

                // Insert Schedule Send Add On Notification
                var schedulesAddOn = new Schedules
                {
                    ScheduleSendType = (int)Enums.ScheduleSendType.IsAddOnNotificationAfterRedemption,
                    Name = "Send Add-On Notification Redemption",
                    Status = (int)Enums.ScheduleType.NotRun,
                    BookingId = BookingsDetails.BookingId,
                    SendAt = DateTime.UtcNow.AddMinutes(AppConfiguration.SendEmailSurveyAfterMinutes)
                };
                _bookingRepository.AddSchedule(schedulesAddOn);
            }

            Session["UpdateSuccess"] = string.Format("Booking was updated successfully: {0} {1}", 
                (diffPrice < 0 ? "Refund" : "Charge"), 
                Helper.FormatPrice(diffPrice));
            
            ChangeCheckInDateMessage.Visible = true;
            ChangeCheckInDateMessage.ForeColor = Color.Green;
        }

        private void ReCalculatePrice()
        {
            ErrorMessageLabel.Visible = false;
            DateTime checkInDate;
            if (DateTime.TryParseExact(CheckInDateText.Text, "MMM dd, yyyy", null, DateTimeStyles.None,
                out checkInDate))
            {
                BookingsDetails.CheckinDate = checkInDate.AddHours(Math.Abs(CurrentHotel.TimeZoneOffset));
            }
            if (IsNeedCalculate)
            {
                int quantity;
                int ticketAvailable;
                int.TryParse(TicketQtyText.Text, out quantity);
                var availableProductParams = new CheckAvailableProductParams
                {
                    ProductId = BookingsDetails.ProductId,
                    CheckInDate = checkInDate,
                    TotalTicket = quantity,
                    BookingId = BookingsDetails.BookingId,
                    IsAdmin = true,
                    TimezoneId = CurrentHotel.TimeZoneId
                };
                var availableProduct = _productRepository.CheckAvailableProduct(availableProductParams, out ticketAvailable);
                if (!availableProduct)
                {
                    ChangeCheckInDateMessage.Text = Message.VerifyUpdate;
                    ChangeCheckInDateMessage.Visible = true;
                    ErrorMessageLabel.Text = string.Format(Message.CheckQuantityOnDate, checkInDate);
                    ErrorMessageLabel.Visible = true;
                    return;
                }

                var products = _productRepository.GetById(BookingsDetails.ProductId);
                bool isLimit;
                var param = new GetDiscountValidByCodeParams
                {
                    Code = DiscountUsedText.Text.Trim(),
                    ProductId = BookingsDetails.ProductId,
                    CustomerId = BookingsDetails.CustomerId,
                    IsAdmin = true,
                    BookingId = BookingsDetails.BookingId,
                    TimezoneId = CurrentHotel.TimeZoneId
                };
                DiscountUsed = _discountRepository.GetDiscountValidByCode(param, out isLimit);

                if (DiscountUsed == null && isLimit)
                {
                    ChangeCheckInDateMessage.Text = Message.ExceedLimit;
                    ChangeCheckInDateMessage.Visible = true;

                    ErrorMessageLabel.Text = Message.ExceedLimit;
                    ErrorMessageLabel.Visible = true;
                    return;
                }

                // Exists current booking with Subscription Discount
                if (PublicDiscountSubscription != null)
                {
                    quantity -= 1;
                }

                BookingsDetails.Quantity = quantity;

                var newPrice = _productRepository.GetById(products.ProductId, checkInDate).ActualPriceWithDate;

                double actualPrice = GetActualPrice(newPrice, quantity);

                double diffPrice = actualPrice * BookingsDetails.Quantity - BookingsDetails.TotalPrice;

                // Charge
                if (diffPrice > 0)
                {
                    BookingsDetails.HotelPrice = actualPrice;
                    BookingsDetails.MerchantPrice = newPrice.Price;
                    BookingsDetails.TotalPrice = CurrentBookingsDetails.TotalPrice + diffPrice;
                }
                else if (diffPrice < 0) //Refund
                {
                    BookingsDetails.HotelPrice = actualPrice;
                    BookingsDetails.MerchantPrice = newPrice.Price;
                    BookingsDetails.TotalPrice = CurrentBookingsDetails.TotalPrice - Math.Abs(diffPrice);
                }
            }
        }

        private void AssignBlackOutDate()
        {
            CurrentBookingsDetails = _bookingRepository.GetById(CurrentBookingId);
            var serialize = JsonConvert.SerializeObject(CurrentBookingsDetails, CustomSettings.SerializerSettingsWithFullDateTime());
            BookingsDetails = JsonConvert.DeserializeObject<Bookings>(serialize);
            CustomerInfosDetails = _bookingRepository.CustomerInfoList.FirstOrDefault(x => x.CustomerId == BookingsDetails.CustomerId);
            CurrentProduct = _productRepository.ProductList.First(x => x.ProductId == BookingsDetails.ProductId);
            CurrentHotel = _productRepository.HotelList.First(h => h.HotelId == CurrentProduct.HotelId);
            var tickets = _productRepository.GetTicketsFuture(BookingsDetails.ProductId);
            string json1 = JsonConvert.SerializeObject(tickets, CustomSettings.SerializerSettings());
            PublicTickets = json1;

            var blockedDates = _productRepository.BlockedDatesCustomPriceList
                .Where(bd => bd.ProductId == CurrentBookingsDetails.ProductId && bd.Capacity == 0 && bd.Date > DateTime.UtcNow)
                .Select(b => "'" + b.Date.ToString("MM/dd/yyyy") + "'")
                .ToList();

            ProductBlockoutDate = string.Format("[{0}]", String.Join(",", blockedDates));
        }

        #region View Day Pass

        private void Charges(double diffPrice, ActualPriceObject newPrice, double actualPrice, Products products, Hotels hotels, CustomerInfos customerInfos)
        {
            string description = string.Format("Update Check-In Date {0} - {1} - {2} - {3:MMM dd, yyyy}",
                Helper.GetStringPassByProductType(products.ProductType),
                products.ProductName,
                hotels.HotelName, DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(CurrentHotel.TimeZoneId));
            StripeCharge stripeCharge = CreateCharges(diffPrice, customerInfos.StripeCustomerId,
                description);

            var chargeInvoice = new Invoices
            {
                BookingId = BookingsDetails.BookingId,
                PassStatus = BookingsDetails.PassStatus,
                StripeChargeId = stripeCharge.Id,
                ChargeAmount = diffPrice,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = PublicCustomerInfos.CustomerId,
                InvoiceStatus = (int)Enums.InvoiceStatus.Charge,
                HotelPrice = actualPrice,
                MerchantPrice = newPrice.Price,
                Quantity = BookingsDetails.Quantity,
                RefundAmount = 0,
                StripeRefundId = string.Empty,
                StripeRefundStatus = string.Empty,
                TotalPrice = BookingsDetails.TotalPrice + diffPrice
            };
            _bookingRepository.AddInvoices(chargeInvoice, string.Empty);
        }

        private void Refund(double diffPrice, ActualPriceObject newPrice, double actualPrice)
        {
            var bookingInvoices = _bookingRepository.GetInvoicesByBookingId(BookingsDetails.BookingId).ToList();
            var chargeInvoices = bookingInvoices
                .Where(x => x.InvoiceStatus == (int)Enums.InvoiceStatus.Charge)
                .OrderBy(x => x.TotalPrice)
                .ToList();

            var currentCharge = chargeInvoices.FirstOrDefault();
            while (diffPrice > 0 && currentCharge != null)
            {
                var refundOfCurrentCharge = bookingInvoices
                    .Where(x => (x.InvoiceStatus == (int) Enums.InvoiceStatus.PartialRefund ||
                                 x.InvoiceStatus == (int) Enums.InvoiceStatus.FullRefund) &&
                                x.StripeChargeId == currentCharge.StripeChargeId)
                    .ToList();

                var oldRefund = refundOfCurrentCharge.Sum(x => x.RefundAmount);
                var newDiffPrice = currentCharge.ChargeAmount - oldRefund;

                if (newDiffPrice > 0)
                {
                    double newRefundPrice = diffPrice;
                    diffPrice -= newDiffPrice;
                    var chargeInvoice = new Invoices
                    {
                        BookingId = BookingsDetails.BookingId,
                        PassStatus = BookingsDetails.PassStatus,
                        StripeChargeId = currentCharge.StripeChargeId,
                        ChargeAmount = currentCharge.ChargeAmount,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = PublicCustomerInfos.CustomerId,
                        HotelPrice = actualPrice,
                        MerchantPrice = newPrice.Price,
                        Quantity = BookingsDetails.Quantity,
                        TotalPrice = BookingsDetails.TotalPrice
                    };

                    if (diffPrice >= 0)
                    {
                        newRefundPrice = newDiffPrice;
                        chargeInvoice.InvoiceStatus = (int) Enums.InvoiceStatus.FullRefund;
                    }
                    else
                    {
                        chargeInvoice.InvoiceStatus = (int) Enums.InvoiceStatus.PartialRefund;
                    }
                    var refund = CallStripeRefund(currentCharge.StripeChargeId, newRefundPrice);
                    chargeInvoice.RefundAmount = (double) refund.Amount / 100;
                    chargeInvoice.StripeRefundId = refund.Id;
                    chargeInvoice.StripeRefundStatus = refund.Status;

                    string requestObj = JsonConvert.SerializeObject(refund);
                    string message = string.Format("{0} Refund Response: {1}, Refund Post Data: {2}",
                        BookingsDetails.BookingCode,
                        refund.StripeResponse.ResponseJson,
                        requestObj);
                    _bookingRepository.AddInvoices(chargeInvoice, message);
                }
                chargeInvoices.Remove(currentCharge);
            }
        }

        private void MaintainOldInvoices()
        {
            var chargeInvoice = new Invoices
            {
                BookingId = BookingsDetails.BookingId,
                PassStatus = BookingsDetails.PassStatus,
                StripeChargeId = BookingsDetails.StripeChargeId,
                ChargeAmount = BookingsDetails.TotalPrice,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = PublicCustomerInfos.CustomerId,
                HotelPrice = BookingsDetails.HotelPrice,
                InvoiceStatus = (int)Enums.InvoiceStatus.Charge,
                MerchantPrice = BookingsDetails.MerchantPrice,
                Quantity = BookingsDetails.Quantity,
                RefundAmount = 0,
                StripeRefundId = string.Empty,
                StripeRefundStatus = string.Empty,
                TotalPrice = BookingsDetails.TotalPrice
            };
            _bookingRepository.AddInvoices(chargeInvoice, string.Empty);

            if (!string.IsNullOrEmpty(BookingsDetails.StripeRefundTransactionId))
            {
                double refundAmount = BookingsDetails.StripeRefundAmount.HasValue
                    ? BookingsDetails.StripeRefundAmount.Value
                    : 0;
                var refundInvoice = new Invoices
                {
                    BookingId = BookingsDetails.BookingId,
                    PassStatus = BookingsDetails.PassStatus,
                    StripeChargeId = string.Empty,
                    ChargeAmount = 0,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = PublicCustomerInfos.CustomerId,
                    HotelPrice = BookingsDetails.HotelPrice,
                    InvoiceStatus = refundAmount >= BookingsDetails.TotalPrice * 100 ? (int)Enums.InvoiceStatus.FullRefund : (int)Enums.InvoiceStatus.PartialRefund,
                    MerchantPrice = BookingsDetails.MerchantPrice,
                    Quantity = BookingsDetails.Quantity,
                    RefundAmount = (double)refundAmount / 100,
                    StripeRefundId = BookingsDetails.StripeRefundTransactionId,
                    StripeRefundStatus = BookingsDetails.StripeRefundStatus,
                    TotalPrice = BookingsDetails.TotalPrice
                };
                _bookingRepository.AddInvoices(refundInvoice, string.Empty);
            }

            BookingsDetails.StripeChargeId = string.Empty;
            BookingsDetails.StripeRefundAmount = 0;
            BookingsDetails.StripeRefundStatus = string.Empty;
            BookingsDetails.StripeRefundTransactionId = string.Empty;
            BookingsDetails.HasInvoice = true;
            BookingsDetails.IsMaintainInvoices = true;
            _bookingRepository.Update(BookingsDetails);
        }

        private double GetActualPrice(ActualPriceObject newPrice, int quantity)
        {
            double actualPrice = newPrice.Price;
            if (DiscountUsed != null)
            {
                return Helper.CalculateDiscount(DiscountUsed, actualPrice, quantity);
            }
            return actualPrice;
        }

        private StripeRefund CallStripeRefund(string stripeChargeId, double diffPrice)
        {
            var refundService = new StripeRefundService();

            StripeRefund refund = refundService.Create(stripeChargeId, new StripeRefundCreateOptions()
            {
                Amount = Convert.ToInt32(diffPrice * 100),
                Reason = StripeRefundReasons.RequestedByCustomer
            });
            return refund;
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

        #endregion

        protected void UpdateBooking_OnClick(object sender, EventArgs e)
        {
            ChangeCheckInDateMessage.Visible = false;
            DataChanged();
            UpdateBookings();
            _bookingRepository.ResetCache();
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        private void BindBookingChange()
        {
            if (CurrentBookingsDetails.CheckinDate.HasValue)
            {
                CurrentCheckInDateLabel.Text = CurrentBookingsDetails.CheckinDate.Value.ToString(Constant.DateFormat);
            }
            CurrentTicketQuantityLabel.Text = CurrentBookingsDetails.Quantity.ToString();
            CurrentMsrpPriceLabel.Text = Helper.FormatPrice(CurrentBookingsDetails.MerchantPrice);
            CurrentPaidItemPriceLabel.Text = Helper.FormatPrice(CurrentBookingsDetails.HotelPrice);
            if (CurrentDiscountUsed != null)
            {
                switch (CurrentDiscountUsed.PromoType)
                {
                    case (int) Enums.PromoType.Fixed:
                        CurrentDiscountLabel.Text = string.Format("{0}$", CurrentDiscountUsed.PercentOff);
                        break;
                    case (int) Enums.PromoType.Percent:
                        CurrentDiscountLabel.Text = string.Format("{0}%", CurrentDiscountUsed.PercentOff);
                        break;
                }
                CurrentPromoCodeLabel.Text = CurrentDiscountUsed.Code;
            }
            else
            {
                CurrentDiscountLabel.Text = "0%";
            }
            CurrentTotalPriceLabel.Text = Helper.FormatPrice(CurrentBookingsDetails.TotalPrice);
            SaveButton.Attributes["onclick"] = "$('#confirmModal').modal(); return false;";

            //New
            NewCheckInDateLabel.Text = CheckInDateText.Text;
            NewTicketQuantityLabel.Text = TicketQtyText.Text;
            NewMsrpPriceLabel.Text = Helper.FormatPrice(BookingsDetails.MerchantPrice);
            NewPaidItemPriceLabel.Text = Helper.FormatPrice(BookingsDetails.HotelPrice);
            if (DiscountUsed != null)
            {
                switch (DiscountUsed.PromoType)
                {
                    case (int)Enums.PromoType.Fixed:
                        NewDiscountLabel.Text = string.Format("{0}$", DiscountUsed.PercentOff);
                        break;
                    case (int)Enums.PromoType.Percent:
                        NewDiscountLabel.Text = string.Format("{0}%", DiscountUsed.PercentOff);
                        break;
                }
                NewPromoCodeLabel.Text = DiscountUsed.Code;
            }
            else
            {
                NewDiscountLabel.Text = "0%";
            }
            NewTotalPriceLabel.Text = Helper.FormatPrice(BookingsDetails.TotalPrice);

            // Charge / refund Row
            double changePrice = CurrentBookingsDetails.TotalPrice - BookingsDetails.TotalPrice;
            NewBookingChangePriceLabel.Text = Helper.FormatPrice(changePrice).Replace("(", "").Replace(")", "");

            PaymentMethodDdl.Items.Clear();
            RefundPaymentMethodDdl.Items.Clear();
            var customerCredit = _productRepository.CustomerCreditList
                .FirstOrDefault(cc => cc.CustomerId == CurrentBookingsDetails.CustomerId);

            RefundPaymentMethodDdl.Items.Add(new ListItem(Message.DayAxeCredit, "DayAxeCredit"));

            if (changePrice < 0)
            {
                BookingChangeLabel.Text = "Charge:";
                NewBookingChangePriceLabel.CssClass = "success";
                // Check Enough Credit to Charge ??
                if (customerCredit != null && customerCredit.Amount >= Math.Abs(changePrice))
                {
                    PaymentMethodDdl.Items.Add(new ListItem(Message.DayAxeCredit, "DayAxeCredit"));
                }
            }
            else if (changePrice > 0)
            {
                BookingChangeLabel.Text = "Refund:";
                NewBookingChangePriceLabel.CssClass = "error";
                if (customerCredit != null)
                {
                    PaymentMethodDdl.Items.Add(new ListItem(Message.DayAxeCredit, "DayAxeCredit"));
                }
            }
            else
            {
                BookingChangeLabel.Text = "Change";
                NewBookingChangePriceLabel.CssClass = "success";
                NewBookingChangePriceLabel.Text = "$0";
            }

            if (CustomerInfosDetails != null)
            {
                var stripeCard = GetCardById(CustomerInfosDetails.StripeCustomerId,
                    CustomerInfosDetails.StripeCardId);
                if (stripeCard != null)
                {
                    PaymentMethodDdl.Items.Add(new ListItem(string.Format("{0} ending in {1} Exp {2}/{3}",
                        stripeCard.Brand,
                        stripeCard.Last4,
                        stripeCard.ExpirationMonth,
                        stripeCard.ExpirationYear % 100), "Card"));
                    RefundPaymentMethodDdl.Items.Add(new ListItem(string.Format("{0} ending in {1} Exp {2}/{3}",
                        stripeCard.Brand,
                        stripeCard.Last4,
                        stripeCard.ExpirationMonth,
                        stripeCard.ExpirationYear % 100), "Card"));
                }
            }

        }

        private StripeCard GetCardById(string customerId, string cardId)
        {
            var cardService = new StripeCardService();
            StripeCard stripeCard = cardService.Get(customerId, cardId);
            return stripeCard;
        }

        protected void DiscountUsedText_OnTextChanged(object sender, EventArgs e)
        {
            DataChanged();
            ReCalculatePrice();
            BindBookingChange();
        }

        protected void TicketQtyText_OnTextChanged(object sender, EventArgs e)
        {
            DataChanged();
            ReCalculatePrice();
            BindBookingChange();
        }

        protected void CheckInDateText_OnTextChanged(object sender, EventArgs e)
        {
            DataChanged();
            ReCalculatePrice();
            BindBookingChange();
        }

        protected void BookingStatusDdl_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            BindBookingChange();
            if (BookingStatusDdl.SelectedValue == "Refunded")
            {
                SaveButton.Attributes["onclick"] = "$('#refundModal').modal(); return false;";
            }
            else if (BookingStatusDdl.SelectedValue == "Active")
            {
                SaveButton.Attributes["onclick"] = "$('#confirmModal').modal(); return false;";
            }
            else
            {
                SaveButton.Attributes.Remove("onclick");
            }
            ConfirmPanel.Update();
        }

        protected void RefundButton_OnClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(BookingsDetails.StripeChargeId))
            {
                MaintainOldInvoices();
            }
            RefundMessageLabel.Text = string.Empty;
            double refundAmount;
            bool isRefundAll = FullRefundRadio.Checked;

            if (!isRefundAll && string.IsNullOrEmpty(RefundAmount.Text))
            {
                RefundMessageLabel.Text = Message.EnterRefundAmount;
                ScriptManager.RegisterStartupScript(RefundPanel, RefundPanel.GetType(), "RefundError",
                    "$(function(){$('.modal-backdrop').remove(); $('#refundModal').modal('show');});", true);
                return;
            }

            if (isRefundAll)
            {
                refundAmount = CurrentBookingsDetails.TotalPrice;
                BookingsDetails.CancelDated = DateTime.UtcNow;
                BookingsDetails.PassStatus = (int)Enums.BookingStatus.Refunded;
            }
            else
            {
                if (!double.TryParse(RefundAmount.Text, out refundAmount) ||
                    CurrentBookingsDetails.TotalPrice < refundAmount)
                {
                    RefundMessageLabel.Text = Message.EnterValidRefundAmount;
                    ScriptManager.RegisterStartupScript(RefundPanel, RefundPanel.GetType(), "RefundError",
                        "$(function(){$('.modal-backdrop').remove(); $('#refundModal').modal('show');});", true);
                    return;
                }
                BookingsDetails.TotalPrice -= refundAmount;
            }

            var products = _productRepository.GetById(BookingsDetails.ProductId);
            var hotels = _productRepository.HotelList.First(x => x.HotelId == products.HotelId);
            var logs = new CustomerCreditLogs();
            var market = (from mh in _productRepository.MarketHotelList
                join m in _productRepository.MarketList on mh.MarketId equals m.Id
                where mh.HotelId == hotels.HotelId
                select m).FirstOrDefault();
            switch (RefundPaymentMethodDdl.SelectedValue)
            {
                case "DayAxeCredit":
                    logs = new CustomerCreditLogs
                    {
                        CustomerId = CurrentBookingsDetails.CustomerId,
                        ProductId = CurrentBookingsDetails.ProductId,
                        Description = string.Format("{0} – {1} – {2} – {3}",
                            products.ProductName,
                            hotels.HotelName,
                            market != null ? market.LocationName : "",
                            CurrentBookingsDetails.BookingIdString),
                        Amount = refundAmount,
                        CreatedBy = PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 0,
                        CreatedDate = DateTime.UtcNow,
                        CreditType = isRefundAll
                            ? (byte) Enums.CreditType.FullPurchaseRefund
                            : (byte) Enums.CreditType.PartialPuchaseRefund,
                        ReferralId = 0,
                        BookingId = CurrentBookingsDetails.BookingId,
                        Status = true,
                        GiftCardId = 0
                    };
                    BookingsDetails.PaymentType = (byte) Enums.PaymentType.DayAxeCredit;

                    BookingsDetails.RefundCreditAmount = refundAmount;

                    // Refund an Amount
                    if (!isRefundAll)
                    {
                        if (refundAmount > BookingsDetails.PayByCredit)
                        {
                            BookingsDetails.PayByCredit = 0;
                        }
                        else
                        {
                            BookingsDetails.PayByCredit = BookingsDetails.PayByCredit - refundAmount;
                        }
                    }
                    break;
                default:
                    double payByCard = refundAmount - BookingsDetails.PayByCredit;
                    if (BookingsDetails.PayByCredit > 0)
                    {
                        logs = new CustomerCreditLogs
                        {
                            CustomerId = CurrentBookingsDetails.CustomerId,
                            ProductId = CurrentBookingsDetails.ProductId,
                            Description = string.Format("{0} – {1} – {2} – {3}",
                                products.ProductName,
                                hotels.HotelName,
                                market != null ? market.LocationName : "",
                                CurrentBookingsDetails.BookingIdString),
                            Amount = Math.Abs(BookingsDetails.PayByCredit),
                            CreatedBy = PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 0,
                            CreatedDate = DateTime.UtcNow,
                            CreditType = payByCard >= 0 ? (byte)Enums.CreditType.FullPurchaseRefund : (byte)Enums.CreditType.PartialPuchaseRefund,
                            ReferralId = 0,
                            BookingId = CurrentBookingsDetails.BookingId,
                            Status = true,
                            GiftCardId = 0
                        };
                        BookingsDetails.PaymentType = (byte)Enums.PaymentType.DayAxeCredit;

                        BookingsDetails.RefundCreditAmount = BookingsDetails.PayByCredit;
                    }

                    if (payByCard > 0)
                    {
                        Refund(payByCard, new ActualPriceObject { Price = CurrentBookingsDetails.MerchantPrice },
                            CurrentBookingsDetails.HotelPrice);
                        var stripeCard = GetCardById(CustomerInfosDetails.StripeCustomerId,
                            CustomerInfosDetails.StripeCardId);
                        BookingsDetails.StripeCardString = string.Format("{0} {1}", stripeCard.Brand, stripeCard.Last4);
                        BookingsDetails.PaymentType = (byte) Enums.PaymentType.Stripe;

                        BookingsDetails.StripeRefundAmount = payByCard;
                    }
                    break;
            }
            BookingsDetails.TotalRefundAmount = BookingsDetails.TotalPrice;
            _bookingRepository.RefundBooking(BookingsDetails, logs);

            _bookingRepository.ResetCache();
            Session["UpdateSuccess"] = Message.BookingRefundSuccessfully;
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        protected void FullRefundRadio_OnCheckedChanged(object sender, EventArgs e)
        {
            RefundPartialRow.Visible = false;
            ScriptManager.RegisterStartupScript(RefundPanel, 
                RefundPanel.GetType(), 
                "FullRefundChange",
                "$(function(){$('.modal-backdrop').remove(); $('#refundModal').modal('show');});", 
                true);
        }

        protected void PartialRefundRadio_OnCheckedChanged(object sender, EventArgs e)
        {
            RefundPartialRow.Visible = true;
            ScriptManager.RegisterStartupScript(RefundPanel, 
                RefundPanel.GetType(), 
                "PartialRefundChange",
                "$(function(){$('.modal-backdrop').remove(); $('#refundModal').modal('show');});", 
                true);
        }

        private void BindBookingHistories()
        {
            var bookingHistories = _bookingRepository.BookingHistoryList
                .Where(bh => bh.BookingId == BookingsDetails.BookingId)
                .ToList();

            bookingHistories.Add(new BookingHistories
            {
                BookingId = BookingsDetails.BookingId,
                CheckinDate = BookingsDetails.CheckinDate,
                DiscountCode = string.Empty,
                DiscountId = CurrentDiscountUsed != null ? CurrentDiscountUsed.Id : 0,
                ExpiredDate = BookingsDetails.ExpiredDate,
                HotelPrice = BookingsDetails.HotelPrice,
                MerchantPrice = BookingsDetails.MerchantPrice,
                PassStatus = BookingsDetails.PassStatus,
                TotalPrice = BookingsDetails.TotalPrice,
                Quantity = BookingsDetails.Quantity,
                PayByCredit = BookingsDetails.PayByCredit,
                RedeemedDate = BookingsDetails.RedeemedDate,
                StripeCardString = string.Empty,
                UpdatedDate = DateTime.UtcNow,
                UpdatedBy = 1,
                PaymentType = 0
            });

            bookingHistories = bookingHistories.OrderByDescending(bh => bh.UpdatedDate)
                .ToList();

            var result = new List<BookingHistoryCompareObject>();

            if (bookingHistories.Count > 1)
            {
                for (int i = 0; i < bookingHistories.Count - 1; i++)
                {
                    var compareObj = CompareBookingHistories(bookingHistories[i], bookingHistories[i + 1]);
                    if (!string.IsNullOrEmpty(compareObj.Description))
                    {
                        result.Add(compareObj);
                    }
                }
            }

            BookingHistoriesRpt.DataSource = result;
            BookingHistoriesRpt.DataBind();
        }

        private BookingHistoryCompareObject CompareBookingHistories(BookingHistories bookingNew, BookingHistories bookingOld)
        {
            StringBuilder sb = new StringBuilder();

            if (bookingNew.Quantity != bookingOld.Quantity)
            {
                sb.AppendFormat("Change Quantity from {0} to {1} - ",
                    bookingOld.Quantity, bookingNew.Quantity);
            }

            if (bookingNew.CheckinDate.HasValue && bookingOld.CheckinDate.HasValue && bookingNew.CheckinDate.Value.Date != bookingOld.CheckinDate.Value.Date)
            {
                sb.AppendLine();
                sb.AppendFormat("Change Check-In Date from {0:MMM dd, yyyy} to {1:MMM dd, yyyy} - ", 
                    bookingOld.CheckinDate.Value, 
                    bookingNew.CheckinDate.Value);
            }

            if (bookingNew.PassStatus != bookingOld.PassStatus)
            {
                sb.AppendLine();
                sb.AppendFormat("Change Status from {0} to {1} ",
                    ((Enums.BookingStatus) bookingOld.PassStatus).ToDescription(),
                    ((Enums.BookingStatus) bookingNew.PassStatus).ToDescription());

                if (bookingNew.PassStatus == (int) Enums.BookingStatus.Refunded)
                {
                    var customerCredits = _bookingRepository.CustomerCreditLogList
                        .FirstOrDefault(ccl => ccl.BookingId == bookingOld.BookingId && ccl.CreatedDate == bookingOld.UpdatedDate);
                    if (customerCredits != null)
                    {
                        double refundCredit = customerCredits.Amount;
                        double refundStripe = bookingOld.TotalPrice - customerCredits.Amount;
                        sb.AppendFormat(" - Refund Total (Credit: {0} + Stripe {1}) - ", Helper.FormatPrice(refundCredit), Helper.FormatPrice(refundStripe));
                    }
                    else if (bookingOld.PayByCredit > 0)
                    {
                        double refundCredit = bookingOld.PayByCredit;
                        double refundStripe = bookingOld.TotalPrice - bookingOld.PayByCredit;
                        sb.AppendFormat(" - Refund Total (Credit: {0} + Stripe {1}) - ",
                            Helper.FormatPrice(refundCredit),
                            Helper.FormatPrice(refundStripe));
                    }
                    else
                    {
                        sb.AppendFormat(" - Refund to Stripe {0} - ", Helper.FormatPrice(bookingNew.TotalPrice));
                    }
                }
            }

            if (bookingNew.DiscountId != bookingOld.DiscountId)
            {
                sb.AppendLine();
                if (bookingNew.DiscountId > 0)
                {
                    var discount = _discountRepository.GetById(bookingNew.DiscountId);
                    sb.AppendFormat("Apply new Discount {0} - ", discount != null ? discount.Code : string.Empty);
                }
                else
                {
                    var discount = _discountRepository.GetById(bookingOld.DiscountId);
                    sb.AppendFormat("Remove Discount from Bookings: {0} - ", discount != null ? discount.Code : string.Empty);
                }
            }

            if (!bookingNew.TotalPrice.Equals(bookingOld.TotalPrice))
            {
                var customerCredits = _bookingRepository.CustomerCreditLogList
                    .FirstOrDefault(ccl => ccl.BookingId == bookingOld.BookingId && 
                        ccl.CreatedDate == bookingOld.UpdatedDate);
                if (bookingNew.TotalPrice > bookingOld.TotalPrice)
                {
                    var totalCharge = bookingNew.TotalPrice - bookingOld.TotalPrice;
                    double creditCharge = totalCharge;
                    if (customerCredits != null)
                    {
                        creditCharge -= customerCredits.Amount;
                        sb.AppendFormat(" Charge Credit: {0} - ",
                            Helper.FormatPrice(customerCredits.Amount * -1));
                    }
                    if (creditCharge > 0)
                    {
                        sb.AppendFormat(" Charge Stripe: {0} - ", Helper.FormatPrice(creditCharge * -1));
                    }
                }
                else if (bookingNew.TotalPrice < bookingOld.TotalPrice)
                {
                    var totalRefund = Math.Abs(bookingNew.TotalPrice - bookingOld.TotalPrice);
                    double refundStripe = totalRefund;
                    if (customerCredits != null)
                    {
                        refundStripe -= customerCredits.Amount;
                        sb.AppendFormat("{0} Refund Credit: {1} - ", 
                            customerCredits.CreditType == (byte)Enums.CreditType.PartialPuchaseRefund ? "Partial" : string.Empty,
                            Helper.FormatPrice(customerCredits.Amount));
                    }
                    if (refundStripe > 0)
                    {
                        sb.AppendFormat(" Refund Stripe: {0} - ", Helper.FormatPrice(refundStripe));
                    }
                }
            }
            
            var result = new BookingHistoryCompareObject
            {
                Id = bookingOld.Id,
                UpdatedDate = bookingNew.UpdatedDate.AddMinutes(2) >= DateTime.UtcNow ? bookingOld.UpdatedDate : bookingNew.UpdatedDate,
                Description = Regex.Replace(sb.ToString(), "(\\s-\\s)$", "", RegexOptions.Compiled)
            };

            return result;
        }

        protected void BookingHistoriesRpt_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            
        }

        private string GetStripeCardString()
        {
            var stripeCard = GetCardById(CustomerInfosDetails.StripeCustomerId,
                CustomerInfosDetails.StripeCardId);
            return string.Format("{0} {1}", stripeCard.Brand, stripeCard.Last4);
        }
    }
}