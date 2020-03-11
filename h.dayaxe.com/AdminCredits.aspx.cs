using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DayaxeDal;
using DayaxeDal.Extensions;
using DayaxeDal.Repositories;
using Newtonsoft.Json;
using Stripe;

namespace h.dayaxe.com
{
    public partial class AdminCredits : BasePage
    {
        private CustomerInfos PublicUser { get; set; }
        private CustomerInfos CustomerInfosDetails { get; set; }
        private int CurrentCustomerId { get; set; }

        private readonly DalHelper _helper = new DalHelper();
        private readonly CustomerInfoRepository _customerInfoRepository = new CustomerInfoRepository();
        private readonly CustomerCreditRepository _customerCreditRepository = new CustomerCreditRepository();
        private List<CustomerCreditLogs> _currentData;
        private CustomerCredits _customerCredits;

        protected void Page_Init(object sender, EventArgs e)
        {
            Session["Active"] = "SearchBookingsList";
            string sessionUser = Session["CurrentUser"] != null ? Session["CurrentUser"].ToString() : string.Empty;
            PublicUser = JsonConvert.DeserializeObject<CustomerInfos>(sessionUser);
            if (PublicUser == null)
            {
                Response.Redirect(Constant.DefaultPage);
            }

            int customerId;
            int.TryParse(Request.Params["id"], out customerId);
            CurrentCustomerId = customerId;
            _currentData = _customerCreditRepository.GetAllLogsByCustomerId(CurrentCustomerId).ToList();

            _customerCredits = _customerCreditRepository.GetById(CurrentCustomerId);

            if (!IsPostBack)
            {
                Session["CurrentPage"] = 1;
                CustomerInfosDetails = _customerInfoRepository.GetById(CurrentCustomerId);

                string json = JsonConvert.SerializeObject(CustomerInfosDetails, CustomSettings.SerializerSettings());
                Session["CustomerInfosDetails"] = json;
                BindRepeater(_currentData.Take(10));
            }
            else
            {
                string session = Session["CustomerInfosDetails"] != null ? Session["CustomerInfosDetails"].ToString() : string.Empty;
                CustomerInfosDetails = JsonConvert.DeserializeObject<CustomerInfos>(session);
            }

            if (CustomerInfosDetails == null)
            {
                Response.Redirect(Constant.SearchBookingsAdminpage);
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
            PasswordText.Text = CustomerInfosDetails.Password;
            StripeCustomerIdText.Text = CustomerInfosDetails.StripeCustomerId;
            CreatedDateText.Text = string.Format("{0:MMM dd, yyyy hh:mm tt} PDT", CustomerInfosDetails.CreatedDate.ToLosAngerlesTime());

            TotalCreditLabel.Text = _currentData.Count.ToString();
            AccountTotalLabel.Text = Helper.FormatPrice(_customerCredits != null ? _customerCredits.Amount : 0);

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

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    var amountLit = (Literal)e.Item.FindControl("Amount");
                    var credits = (CustomerCreditLogs)e.Item.DataItem;

                    if (credits.CreditType == (byte)Enums.CreditType.Charge)
                    {
                        amountLit.Text = string.Format("-${0:0.00}", credits.Amount);
                    }
                    else
                    {
                        amountLit.Text = string.Format("${0:0.00}", credits.Amount);
                    }
                }
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
            var dataObjects = _currentData.Skip(currentPage * Constant.ItemPerPage).Take(Constant.ItemPerPage).ToList();
            if (dataObjects.Any())
            {
                Session["CurrentPage"] = currentPage + 1;
                SearchRepeater.DataSource = dataObjects;
                SearchRepeater.DataBind();
            }
        }

        private void BindRepeater(IEnumerable<CustomerCreditLogs> bookings)
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
            string password = PasswordText.Text.Trim();
            if (string.IsNullOrEmpty(password))
            {
                ErrorLabel.Text = ErrorMessage.PasswordCanNotEmpty;
                ErrorLabel.Visible = true;
                return;
            }

            if (password.Contains(" "))
            {
                ErrorLabel.Text = ErrorMessage.PasswordCanNotContainSpace;
                ErrorLabel.Visible = true;
                return;
            }

            if (password.Length < 7)
            {
                ErrorLabel.Text = ErrorMessage.PasswordNotValid;
                ErrorLabel.Visible = true;
                return;
            }

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
            CustomerInfosDetails.Password = PasswordText.Text;

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
                    UpdatedBy = PublicUser.CustomerId,
                    Result = stripeUpdate.StripeResponse.ResponseJson
                };

                _helper.AddCustomerInfoLogs(log);
            }
            catch (Exception ex)
            {
                var logError = new Logs
                {
                    LogKey = "UpdateCustomerError",
                    UpdatedBy = PublicUser.CustomerId,
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
    }
}