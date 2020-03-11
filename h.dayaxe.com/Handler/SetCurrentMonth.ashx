<%@ WebHandler Language="C#" Class="SetCurrentMonth" %>

using System;
using System.Web;

public class SetCurrentMonth : IHttpHandler , System.Web.SessionState.IRequiresSessionState{

    public void ProcessRequest (HttpContext context)
    {
        var date = context.Request.Params["date"];
        try
        {
            context.Session["CurrentMonth_Calendar"] = DateTime.Parse(date);
        }catch(Exception) { }

        context.Response.ContentType = "application/json";
        context.Response.Write("{success:true}");
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}