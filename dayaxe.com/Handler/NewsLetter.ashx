<%@ WebHandler Language="C#" Class="NewsLetter" %>

using System;
using System.Web;
using DayaxeDal;
using DayaxeDal.Extensions;
using Newtonsoft.Json;

public class NewsLetter : IHttpHandler {

    public void ProcessRequest (HttpContext context) {
        var responseObject = new Response
        {
            IsSuccess = true,
            Message = "Thank you for subscribing! Please check your email, you should get your promo shortly."
        };
        using (var helper = new DalHelper())
        {
            var email = context.Request.Params["e"] ?? string.Empty;

            if (!Helper.IsValidEmail(email))
            {
                responseObject.Message = "Please provide a valid email.";
                responseObject.IsSuccess = false;
            }

            if (responseObject.IsSuccess)
            {
                var url = string.Format(Constant.KlaviyoListApiUrl, AppConfiguration.KlaviyoNewsletterListId);
                var addToListKlaviyoRes = Helper.Post(url, email);

                var newsletter = new Newsletters
                {
                    Email = email,
                    CreatedDate = DateTime.UtcNow
                };

                var id = helper.AddNewsletterEmail(newsletter);

                var logs = new Logs
                {
                    LogKey = "Klaviyo_Newsletter_Response",
                    UpdatedBy = (int) id,
                    UpdatedDate = DateTime.UtcNow,
                    UpdatedContent = string.Format("{0} - {1}", email, addToListKlaviyoRes)
                };

                helper.AddLog(logs);
            }
        }
        var response = JsonConvert.SerializeObject(responseObject, CustomSettings.SerializerSettingsWithFullDateTime());

        context.Response.ContentType = "application/json";
        context.Response.Write(response);
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}