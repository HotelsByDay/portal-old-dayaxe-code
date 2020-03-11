using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Dayaxe.SendEmail;
using DayaxeDal;
using DayaxeDal.Repositories;
using DayaxeDal.Ultility;

namespace h.dayaxe.com
{
    public partial class UserHotel : BasePage
    {
        private HotelRepository _hotelRepository = new HotelRepository();
        private readonly UserRepository _userRepository = new UserRepository();
        private CustomerInfoHotelRepository _userHotelRepository = new CustomerInfoHotelRepository();
        readonly CustomerInfoRepository _customerInfoRepository = new CustomerInfoRepository();
        private CustomerInfos _users;

        protected void Page_Init(object sender, EventArgs e)
        {
            Session["Active"] = "UserHotel";

            if (Request.Params["userId"] != null)
            {
                int userId = int.Parse(Request.Params["userId"]);
                _users = _userRepository.GetById(userId);
            }
            if (!IsPostBack)
            {
                Session["UserHotel"] = null;
                Session["CurrentPage"] = 1;
                if (Request.Params["userId"] == null)
                {
                    MVUserHotel.ActiveViewIndex = 0;
                    RptUserHotel.DataSource = _userRepository.GetAll().Take(Constant.ItemPerPage);
                    RptUserHotel.DataBind();
                }
                else
                {
                    MVUserHotel.ActiveViewIndex = 1;
                    int userId = int.Parse(Request.Params["userId"]);
                    if (userId == 0) // Add new
                    {
                        DdlHotels.Visible = false;
                        BtnAddUserHotel.Visible = false;
                        Deactivebutton.Visible = false;
                        ActiveButton.Visible = false;
                    }
                    else
                    {
                        _users = _userRepository.GetById(userId);
                        if (_users == null)
                        {
                            Response.Redirect(Constant.UserHotelPage);
                        }
                        TxtFirstName.Text = _users.FirstName;
                        TxtLastName.Text = _users.LastName;
                        TxtUsername.Text = _users.EmailAddress;
                        if (_users.IsActive)
                        {
                            Deactivebutton.Visible = true;
                            ActiveButton.Visible = false;
                            btnSendActivationLink.Visible = true;
                        }
                        else
                        {
                            Deactivebutton.Visible = false;
                            ActiveButton.Visible = true;
                        }
                        RebindHotelsByuser();
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void RebindHotelsByuser(bool isReload = false)
        {
            if (isReload)
            {
                _hotelRepository = new HotelRepository();
                _userHotelRepository = new CustomerInfoHotelRepository();
            }
            var allHotel = _hotelRepository.SearchHotelsByCode();
            int userId = int.Parse(Request.Params["userId"]);

            var userHotel = _hotelRepository.SearchHotelsByUserId(userId);
            DdlHotels.DataSource = allHotel.Except(userHotel);
            DdlHotels.DataTextField = "HotelName";
            DdlHotels.DataValueField = "HotelId";
            DdlHotels.DataBind();

            var userses = _userHotelRepository.GetByUserId(userId);
            RptHotelListings.DataSource = userses;
            RptHotelListings.DataBind();

            DdlRole.SelectedIndex = _users.IsAdmin && _users.IsCheckInOnly ? 1 : 0;
        }

        private void RebindHotelsFromSession()
        {
            var userHotelses = (List<CustomerInfosHotels>)Session["UserHotel"];
            RptHotelListings.DataSource = userHotelses;
            RptHotelListings.DataBind();
        }

        protected void RptUserHotel_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var rowHistory = (HtmlTableRow)e.Item.FindControl("rowHotel");
                rowHistory.Attributes.Add("class", "alternative");
            }

            if (e.Item.ItemType == ListItemType.Footer)
            {
                var litPage = (Literal)e.Item.FindControl("LitPage");
                var litTotal = (Literal)e.Item.FindControl("LitTotal");
                var totaluser = _userRepository.GetAll().Count();
                var totalPage = totaluser / Constant.ItemPerPage + (totaluser % Constant.ItemPerPage != 0 ? 1 : 0);
                litPage.Text = string.Format("Page {0} of {1}", Session["CurrentPage"], totalPage);
                litTotal.Text = totaluser + " Users";
            }
        }

        protected void RptHotelListings_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var rowHistory = (HtmlGenericControl)e.Item.FindControl("liAlternatie");
                rowHistory.Attributes.Add("class", "alternative");
            }
        }

        protected void BtnAddUserHotel_OnClick(object sender, EventArgs e)
        {
            int hotelId;
            int.TryParse(DdlHotels.SelectedValue, out hotelId);
            if (hotelId != 0)
            {
                int userId = int.Parse(Request.Params["userId"]);
                _users = _userRepository.GetById(userId);
                var hotel = _hotelRepository.GetHotel(hotelId, _users.EmailAddress);
                if (hotel == null)
                {
                    var userHotels = new CustomerInfosHotels
                    {
                        HotelId = hotelId,
                        CustomerId = _users.CustomerId
                    };
                    _userHotelRepository.Add(userHotels);

                    _userHotelRepository.ResetCache();
                }
                RebindHotelsByuser(true);
            }
        }

        protected void CancelClick(object sender, EventArgs e)
        {
            Response.Redirect(Constant.UserHotelPage);
        }

        protected void DeleteClick(object sender, EventArgs e)
        {
            int userId = int.Parse(Request.Params["userId"]);
            if (userId != 0)
            {
                _userRepository.Delete(userId);

                _userHotelRepository.ResetCache();
            }
            Response.Redirect(Constant.UserHotelPage);
        }

        protected void DeactiveClick(object sender, EventArgs e)
        {
            int userId = int.Parse(Request.Params["userId"]);
            if (userId != 0)
            {
                _users.IsActive = false;
                _userRepository.Update(_users);

                _userHotelRepository.ResetCache();
            }

            Deactivebutton.Visible = false;
            ActiveButton.Visible = true;
        }

        protected void ActiveClick(object sender, EventArgs e)
        {
            int userId = int.Parse(Request.Params["userId"]);
            if (userId != 0)
            {
                _users.IsActive = true;
                _userRepository.Update(_users);

                _userHotelRepository.ResetCache();
            }

            Deactivebutton.Visible = true;
            ActiveButton.Visible = false;
        }

        protected void SaveUserClick(object sender, EventArgs e)
        {
            try
            {
                int userId = int.Parse(Request.Params["userId"]);
                string userName = TxtUsername.Text.Trim();
                if (userId == 0)
                {
                    if (string.IsNullOrEmpty(userName))
                    {
                        LblMessage.Visible = true;
                        LblMessage.Text = "Username is required";
                        return;
                    }

                    if (!Helper.IsValidEmail(userName))
                    {
                        LblMessage.Visible = true;
                        LblMessage.Text = "Please use valid email address";
                        return;
                    }

                    _users = _userRepository.GetCustomerByEmailAddress(userName);
                    if (_users == null)
                    {
                        string salt = Algoritma.GenerateSalt();
                        string password = Algoritma.EncryptHMACSHA512("~new_dayaxe-user!", salt);

                        _users = new CustomerInfos
                        {
                            FirstName = TxtFirstName.Text,
                            LastName = TxtLastName.Text,
                            EmailAddress = userName,
                            Password = password,
                            Salt = salt,
                            CreatedDate = DateTime.UtcNow,
                            IsActive = true,
                            IsAdmin = true,
                            IsCheckInOnly = DdlRole.SelectedValue == "3" // Checkin Only
                        };
                        userId = _userRepository.Add(_users);
                    }
                    else
                    {
                        _users.FirstName = TxtFirstName.Text;
                        _users.LastName = TxtLastName.Text;
                        _users.EmailAddress = userName;
                        _users.IsActive = true;
                        _users.IsAdmin = true;
                        _users.IsDelete = false;
                        _users.IsCheckInOnly = DdlRole.SelectedValue == "3"; // Checkin Only

                        _userRepository.Update(_users);
                        userId = _users.CustomerId;
                    }
                }
                else
                {
                    _users = _userRepository.GetById(userId);
                    _users.FirstName = TxtFirstName.Text;
                    _users.LastName = TxtLastName.Text;
                    _users.EmailAddress = userName;
                    _users.IsCheckInOnly = DdlRole.SelectedValue == "3"; // Checkin Only

                    _userRepository.Update(_users);
                }

                _userHotelRepository.ResetCache();

                Response.Redirect(Constant.UserHotelPage + "?userId=" + userId);
            }
            catch (Exception ex)
            {
                LblMessage.Visible = true;
                LblMessage.Text = ex.Message;
                return;
            }
        }

        protected void UnAssignHotelClick(object sender, EventArgs e)
        {
            if (Request.Params["userId"] != "0")
            {
                List<CustomerInfosHotels> userHotelses = new List<CustomerInfosHotels>();
                foreach (RepeaterItem item in RptHotelListings.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkRemove");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidHotelId = (HiddenField)item.FindControl("HidHotelId");
                        HiddenField hidUserId = (HiddenField)item.FindControl("HidUserId");
                        userHotelses.Add(new CustomerInfosHotels()
                        {
                            CustomerId = int.Parse(hidUserId.Value),
                            HotelId = int.Parse(hidHotelId.Value)
                        });
                    }
                }
                _userHotelRepository.Delete(userHotelses);
                CacheLayer.Clear("CustomerInfosCacheKey");
                _userHotelRepository.ResetCache();
                RebindHotelsByuser(true);
            }
            else
            {
                List<CustomerInfosHotels> userHotelses = (List<CustomerInfosHotels>)Session["UserHotel"];
                //AmentiesName
                foreach (RepeaterItem item in RptHotelListings.Items)
                {
                    //to get the dropdown of each line
                    CheckBox removeUserHotel = (CheckBox)item.FindControl("ChkRemove");

                    if (removeUserHotel.Checked)
                    {
                        HiddenField hidHotelId = (HiddenField)item.FindControl("HidHotelId");
                        HiddenField hidUserId = (HiddenField)item.FindControl("HidUserId");
                        userHotelses.RemoveAll(x => x.CustomerId == int.Parse(hidUserId.Value) && x.HotelId == int.Parse(hidHotelId.Value));
                    }
                }
                Session["UserHotel"] = userHotelses;
                RebindHotelsFromSession();
            }
        }

        protected void Previous_OnClick(object sender, EventArgs e)
        {
            int currentPage = int.Parse(Session["CurrentPage"].ToString());
            var hotels = _userRepository.GetAll().Skip((currentPage - 2) * Constant.ItemPerPage).Take(Constant.ItemPerPage).ToList();
            if (hotels.Any() && currentPage - 2 >= 0)
            {
                Session["CurrentPage"] = currentPage - 1;
                RptUserHotel.DataSource = hotels;
                RptUserHotel.DataBind();
            }
        }

        protected void Next_OnClick(object sender, EventArgs e)
        {
            int currentPage = int.Parse(Session["CurrentPage"].ToString());
            var hotels = _userRepository.GetAll().Skip(currentPage * Constant.ItemPerPage).Take(Constant.ItemPerPage).ToList();
            if (hotels.Any())
            {
                Session["CurrentPage"] = currentPage + 1;
                RptUserHotel.DataSource = hotels;
                RptUserHotel.DataBind();
            }
        }

        protected void btnSendActivationLink_Click(object sender, EventArgs e)
        {

            using (var client = new WebClient())
            {
                var values = new NameValueCollection();
                values["email"] = TxtUsername.Text;

                string newBackend = ConfigurationManager.AppSettings["newBackend"];
                var response = client.UploadValues(newBackend + "/api/v1/users/reset_password", values);

                var responseString = Encoding.Default.GetString(response);
            }


        }
    }
}