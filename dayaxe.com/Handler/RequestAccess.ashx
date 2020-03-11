<%@ WebHandler Language="C#" Class="RequestAccess" %>

using System;
using System.Web;
using DayaxeDal;

public class RequestAccess : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var _hotelId = context.Request.Params["hotelId"];        
        int hotelId;
        int.TryParse(_hotelId, out hotelId);

        var _customerId = context.Request.Params["customerId"];        
        int customerId;
        int.TryParse(_customerId, out customerId);

        var email = context.Request.Params["email"] ?? string.Empty;
            AccessRequests request = null;
            try
        {
           var helper = new DalHelper();
        request = new AccessRequests
        {
            HotelId = hotelId,
            Email = email,
            CustomerId = customerId,
            RequestedDateTime = DateTime.UtcNow
        };
        helper.AddAccessRequest(request);
        }
        catch (Exception) { }
        context.Response.ContentType = "application/json";
        context.Response.Write(request.RequestId);


        
    }

    public bool IsReusable
    {
        get { return false; }
    }
}