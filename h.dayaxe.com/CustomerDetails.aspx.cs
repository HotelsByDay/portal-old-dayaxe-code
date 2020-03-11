using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DayaxeDal;
using DayaxeDal.Extensions;
using DayaxeDal.Repositories;
using Newtonsoft.Json;
using Stripe;

namespace h.dayaxe.com
{
    public partial class CustomerDetails : BasePage
    {
        private CustomerInfos CustomerInfosDetails { get; set; }
        private int CurrentCustomerId { get; set; }
        private Discounts PublicDiscounts { get; set; }
        private SubscriptionBookings _subscriptionBookings;

        private readonly DalHelper _helper = new DalHelper();
        private readonly CustomerInfoRepository _customerInfoRepository = new CustomerInfoRepository();
        private readonly SubscriptionBookingRepository _subscriptionBookingRepository = new SubscriptionBookingRepository();
        private SearchDataResponse _response;
        private List<SearchDataObject> _currentData;

        protected void Page_Init(object sender, EventArgs e)
        {
            Session["Active"] = "SearchBookingsList";

            int customerId;
            int.TryParse(Request.Params["id"], out customerId);
            CurrentCustomerId = customerId;
            if (!IsPostBack)
            {
                Session["CurrentPage"] = 1;
                CustomerInfosDetails = _customerInfoRepository.GetById(CurrentCustomerId);

                string json = JsonConvert.SerializeObject(CustomerInfosDetails, CustomSettings.SerializerSettings());
                Session["CustomerInfosDetails"] = json;

                _response = _helper.SearchDataByCustomerId(CurrentCustomerId);

                _currentData = _response.ListBookingsData.ToList();
                string jsonData = JsonConvert.SerializeObject(_currentData, CustomSettings.SerializerSettings());
                Session["SearchCustomerBookings"] = jsonData;
                BindRepeater(_currentData.Take(10));
            }
            else
            {
                string session = Session["CustomerInfosDetails"] != null ? Session["CustomerInfosDetails"].ToString() : string.Empty;
                CustomerInfosDetails = JsonConvert.DeserializeObject<CustomerInfos>(session);

                string sessionData = Session["SearchCustomerBookings"] != null ? Session["SearchCustomerBookings"].ToString() : string.Empty;
                _currentData = JsonConvert.DeserializeObject<List<SearchDataObject>>(sessionData);
            }

            if (CustomerInfosDetails == null)
            {
                Response.Redirect(Constant.SearchBookingsAdminpage);
            }

            // Get Membership Info
            PublicDiscounts = _customerInfoRepository.GetSubscriptionDiscount(CustomerInfosDetails.CustomerId);

            if (PublicDiscounts != null)
            {
                _subscriptionBookings = _subscriptionBookingRepository.GetByCustomerId(CustomerInfosDetails.CustomerId, PublicDiscounts.Id);
                if (_subscriptionBookings != null && _subscriptionBookings.Status == (byte)Enums.SubscriptionBookingStatus.Active)
                {
                    CancelMembershipLinkButton.Visible = true;
                }
            }

            BindCustomerData();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
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
            StripeCustomerIdText.Text = CustomerInfosDetails.StripeCustomerId;
            CreatedDateText.Text = string.Format("{0:MMM dd, yyyy hh:mm tt} PDT", 
                CustomerInfosDetails.CreatedDate.ToLosAngerlesTime());

            var discount = _customerInfoRepository.GetSubscriptionDiscount(CustomerInfosDetails.CustomerId);

            if (discount != null)
            {
                var subscription = _customerInfoRepository.GetSubscriptionActiveByDiscountId(discount.Id);
                if (subscription != null)
                {
                    MembershipNameText.Text = string.Format("{0} - {1} - {2}", subscription.Name, Helper.FormatPrice(subscription.Price), subscription.ProductHighlight);
                }
                MembershipIDText.Text = discount.Code;
                MembershipCreatedText.Text = discount.StartDate.HasValue ? string.Format("{0:MMM dd, yyyy hh:mm tt} PDT", 
                    discount.StartDate.Value.ToLosAngerlesTime()) : string.Empty;
            }

            if (CustomerInfosDetails.IsActive)
            {
                ActiveButton.Visible = false;
                Deactivebutton.Visible = true;
            }
            else
            {
                ActiveButton.Visible = true;
                Deactivebutton.Visible = false;
            }
        }

        protected void SearchRepeater_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var rowHistory = (HtmlTableRow)e.Item.FindControl("rowHistory");
                rowHistory.Attributes.Add("class", "alternative");
            }

            if (e.Item.ItemType == ListItemType.Header)
            {
                var totalBookings = (Label) e.Item.FindControl("TotalBookings");

                totalBookings.Text = String.Format("Bookings <span class=\"active\">{0}</span>", _currentData.Count);
            }

            if (e.Item.ItemType == ListItemType.Footer)
            {
                var totalPass = (Literal)e.Item.FindControl("TotalPass");
                var litPage = (Literal)e.Item.FindControl("LitPage");
                var totalData = _currentData.Count;
                var totalPage = totalData / Constant.ItemPerPage + (totalData % Constant.ItemPerPage != 0 ? 1 : 0);

                litPage.Text = string.Format("Page {0} of {1}", Session["CurrentPage"], totalPage);
                totalPass.Text = _currentData.Count + " records";
            }
        }

        protected void Previous_OnClick(object sender, EventArgs e)
        {
            int currentPage = int.Parse(Session["CurrentPage"].ToString());
            string sessionData = Session["SearchCustomerBookings"] != null ? Session["SearchCustomerBookings"].ToString() : string.Empty;
            _currentData = JsonConvert.DeserializeObject<List<SearchDataObject>>(sessionData);
            var dataObjects = _currentData.Skip((currentPage - 2) * Constant.ItemPerPage).Take(Constant.ItemPerPage).ToList();
            if (dataObjects.Any() && currentPage - 2 >= 0)
            {
                Session["CurrentPage"] = currentPage - 1;
                SearchRepeater.DataSource = dataObjects;
                SearchRepeater.DataBind();
            }
        }

        protected void Next_OnClick(object sender, EventArgs e)
        {
            int currentPage = int.Parse(Session["CurrentPage"].ToString());
            string sessionData = Session["SearchCustomerBookings"] != null ? Session["SearchCustomerBookings"].ToString() : string.Empty;
            _currentData = JsonConvert.DeserializeObject<List<SearchDataObject>>(sessionData);
            var dataObjects = _currentData.Skip(currentPage * Constant.ItemPerPage).Take(Constant.ItemPerPage).ToList();
            if (dataObjects.Any())
            {
                Session["CurrentPage"] = currentPage + 1;
                SearchRepeater.DataSource = dataObjects;
                SearchRepeater.DataBind();
            }
        }

        private void BindRepeater(IEnumerable<SearchDataObject> bookings)
        {
            SearchRepeater.DataSource = bookings;
            SearchRepeater.DataBind();
        }

        protected void CancelClick(object sender, EventArgs e)
        {
            Response.Redirect(Constant.SearchBookingsAdminpage);
        }

        protected void DeleteClick(object sender, EventArgs e)
        {
            CustomerInfosDetails.IsDelete = true;
            _customerInfoRepository.Update(CustomerInfosDetails);

            _customerInfoRepository.ResetCache();

            Response.Redirect(Constant.HotelList);
        }

        protected void DeactiveClick(object sender, EventArgs e)
        {
            CustomerInfosDetails.IsActive = false;
            _customerInfoRepository.Update(CustomerInfosDetails);

            _customerInfoRepository.ResetCache();

            Deactivebutton.Visible = false;
            ActiveButton.Visible = true;
        }

        protected void ActiveClick(object sender, EventArgs e)
        {
            CustomerInfosDetails.IsActive = true;
            _customerInfoRepository.Update(CustomerInfosDetails);

            _customerInfoRepository.ResetCache();

            Deactivebutton.Visible = true;
            ActiveButton.Visible = false;
        }

        protected void SaveCustomerInfosClick(object sender, EventArgs e)
        {
            ErrorLabel.Visible = false;
            //string password = PasswordText.Text.Trim();
            //if (string.IsNullOrEmpty(password))
            //{
            //    ErrorLabel.Text = ErrorMessage.PasswordCanNotEmpty;
            //    ErrorLabel.Visible = true;
            //    return;
            //}

            //if (password.Contains(" "))
            //{
            //    ErrorLabel.Text = ErrorMessage.PasswordCanNotContainSpace;
            //    ErrorLabel.Visible = true;
            //    return;
            //}

            //if (password.Length < 7)
            //{
            //    ErrorLabel.Text = ErrorMessage.PasswordNotValid;
            //    ErrorLabel.Visible = true;
            //    return;
            //}

            if (string.IsNullOrEmpty(FirstNameText.Text) || string.IsNullOrEmpty(LastNameText.Text))
            {
                ErrorLabel.Text = ErrorMessage.FirstLastNameCanNotEmpty;
                ErrorLabel.Visible = true;
                return;
            }

            if (string.IsNullOrEmpty(EmailText.Text) || !Helper.IsValidEmail(EmailText.Text.Trim()))
            {
                ErrorLabel.Text = ErrorMessage.EmailIsInvalid;
                ErrorLabel.Visible = true;
                return;
            }

            CustomerInfosDetails.FirstName = FirstNameText.Text;
            CustomerInfosDetails.LastName = LastNameText.Text;
            CustomerInfosDetails.EmailAddress = EmailText.Text.Trim().Replace(" ", "");
            CustomerInfosDetails.ZipCode = ZipCodeText.Text;
            //CustomerInfosDetails.Password = PasswordText.Text;

            _customerInfoRepository.Update(CustomerInfosDetails);
            try
            {
                var stripeUpdate = UpdateStripeCustomer(CustomerInfosDetails);

                var log = new CustomerInfoLogs
                {
                    FirstName = FirstNameText.Text,
                    LastName = LastNameText.Text,
                    CustomerEmail = EmailText.Text,
                    ChangedDate = DateTime.UtcNow,
                    CustomerId = CustomerInfosDetails.CustomerId,
                    UpdatedBy = PublicCustomerInfos.CustomerId,
                    Result = stripeUpdate.StripeResponse.ResponseJson
                };

                _helper.AddCustomerInfoLogs(log);
            }
            catch (Exception ex)
            {
                var logError = new Logs
                {
                    LogKey = "UpdateCustomerError",
                    UpdatedBy = PublicCustomerInfos.CustomerId,
                    UpdatedContent = string.Format("{0} - {1}", ex.Message, ex.StackTrace),
                    UpdatedDate = DateTime.UtcNow
                };
                _customerInfoRepository.AddLog(logError);
            }

            _customerInfoRepository.ResetCache();

            ErrorLabel.Text = ErrorMessage.UpdateSuccess;
            ErrorLabel.Visible = true;
        }

        private StripeCustomer UpdateStripeCustomer(CustomerInfos customerInfos)
        {
            var myCustomer = new StripeCustomerUpdateOptions
            {
                Email = customerInfos.EmailAddress,
                Description = string.Format("{0} {1} ({2})", 
                        customerInfos.FirstName, 
                        customerInfos.LastName,
                        customerInfos.EmailAddress)
            };

            var customerService = new StripeCustomerService();
            StripeCustomer stripeCustomer = customerService.Update(customerInfos.StripeCustomerId, myCustomer);
            return stripeCustomer;
        }

        protected void CancelMembershipLinkButton_OnClick(object sender, EventArgs e)
        {
            var subscriptionService = new StripeSubscriptionService();
            StripeSubscription subscription = subscriptionService.Cancel(_subscriptionBookings.StripeSubscriptionId, true);

            _subscriptionBookings.Status = (int)Enums.SubscriptionBookingStatus.End;
            _subscriptionBookings.CancelDate = DateTime.UtcNow;

            _subscriptionBookings.Description = subscription.StripeResponse.ObjectJson;

            _subscriptionBookingRepository.Update(_subscriptionBookings);

            CancelMembershipLinkButton.Visible = false;
            ErrorLabel.Visible = true;
            ErrorLabel.Text = "Your subscription successfully canceled. Your membership is still valid till the end of the period.";
        }
    }
}