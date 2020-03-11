<%@ WebHandler Language="C#" Class="Refresh" %>

using System.Web;
using DayaxeDal;

public class Refresh : IHttpHandler {
    
    public void ProcessRequest (HttpContext context)
    {
        CacheLayer.ClearAll();

        context.Response.ContentType = "application/json";
        context.Response.Write("{}");
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}