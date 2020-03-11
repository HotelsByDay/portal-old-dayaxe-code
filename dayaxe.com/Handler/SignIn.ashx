<%@ WebHandler Language="C#" Class="SignIn" %>

using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;
using DayaxeDal;

public class SignIn : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        var response = new ResponseData();
        context.Response.ContentType = "application/json";
        if (context.Request.HttpMethod == "GET")
        {
            response.Message = new List<string>
            {
                "Method is not allowed."
            };
        }
        if (context.Request.HttpMethod == "POST")
        {
            var username = context.Request["u"];
            var pass = context.Request["p"];
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(pass))
            {
                var helper = new DalHelper();
                //response = helper.SignIn(username, pass);
            }
            else
            {
                response.Message = new List<string>
                {
                    "Please provide valid username and password"
                };
            }
        }
        var serialize = new JavaScriptSerializer();
        var result = serialize.Serialize(response);
        context.Response.Write(result);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}