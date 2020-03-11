using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using DayaxeDal;
using DayaxeDal.Extensions;
using DayaxeDal.Repositories;
using DayaxeDal.Ultility;
using Newtonsoft.Json;

namespace h.dayaxe.com
{
    public partial class Default : Page
    {
        private UserRepository _userRepository = new UserRepository();
        protected void Page_Init(object sender, EventArgs e)
        {
            if (!Request.IsLocal && !Request.IsSecureConnection)
            {
                string redirectUrl = Request.Url.ToString().Replace("http:", "https:");
                Response.Redirect(redirectUrl, false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }

            if (Request.Params["e"] != null)
            {
                var email = HttpUtility.UrlDecode(Algoritma.Decrypt(Request.Params["e"].Replace(' ', '+'), Constant.EncryptPassword));
                var user = _userRepository.GetUsersByEmail(email);
                if (user != null)
                {
                    string json = JsonConvert.SerializeObject(user, CustomSettings.SerializerSettings());
                    Session["CurrentUser"] = json;
                    //var hotels = (from h in _userRepository.HotelList
                    //    join uh in _userRepository.UserHotelList on h.HotelId equals uh.HotelId
                    //    join ci in _userRepository.CustomerInfoList on uh.CustomerId equals ci.CustomerId
                    //    where !h.IsDelete && ci.CustomerId == user.CustomerId
                    //    select h).FirstOrDefault();
                    //if (hotels != null)
                    //{
                    //    Response.Redirect(String.Format("/Revenues.aspx?hotelId={0}", hotels.HotelId), true);
                    //}

                    Response.Redirect(Constant.HotelList, true);
                }
            }
            //if (Context.Session != null)
            //{
            //    string sessionUser = Session["CurrentUser"] != null ? Session["CurrentUser"].ToString() : string.Empty;
            //    var user = JsonConvert.DeserializeObject<CustomerInfos>(sessionUser);
            //    if (user != null)
            //    {
            //        Response.Redirect(Constant.HotelList);
            //    }
            //}
        }
        protected void Page_Load(object sender, EventArgs e)
        {
        
        }

        protected void LoginClick(object sender, EventArgs e)
        {
            var user = _userRepository.GetUsersByEmail(Email.Text.Trim().ToUpper());
            if (user != null)
            {
                if (user.Password == Algoritma.EncryptHMACSHA512(Password.Text, user.Salt))
                {
                    string json = JsonConvert.SerializeObject(user, CustomSettings.SerializerSettings());
                    Session["CurrentUser"] = json;
                    if (Request.Params["ReturnUrl"] != null)
                    {
                        Response.Redirect(HttpUtility.UrlDecode(Request.Params["ReturnUrl"]));
                    }
                    Response.Redirect(Constant.HotelList);
                }
            }
            LblMessage.Text = "Please provide valid email and password";
        }

        //Temporary method to get Salt from database
        //private string GetSaltByCustomerId(int CustomerId)
        //{
        //    string salt = string.Empty;
        //    using (SqlConnection connection = new SqlConnection(Constant.ConnectionString))
        //    {
        //        SqlCommand command = new SqlCommand("SELECT [Salt] FROM [CustomerInfos] where CustomerId=@CID", connection);
        //        command.Parameters.Add(new SqlParameter("CID", CustomerId)); 
        //        command.Connection.Open();
        //        using (var reader = command.ExecuteReader())
        //        {
        //            if (reader.Read() && reader["Salt"] != DBNull.Value)
        //            {
        //                salt = (string)reader["Salt"];
        //            }
        //        }
        //    }
        //    return salt;
        //}
    }
}