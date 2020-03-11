<%@ WebHandler Language="C#" Class="Redeemed" %>

using System;
using System.Web;
using DayaxeDal;
using DayaxeDal.Repositories;

public class Redeemed : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        string id = context.Request.Params["id"];
        BookingRepository bookingRepository = new BookingRepository();
        int bookingId;

        if (string.IsNullOrEmpty(id))
        {
            context.Response.Write("Incorrect redemption code");
            return;
        }
        if (!int.TryParse(id, out bookingId))
        {
            context.Response.Write("Incorrect redemption code");
            return;
        }

        var currentBooking = bookingRepository.GetById(bookingId);
        if (currentBooking == null)
        {
            context.Response.Write("Incorrect redemption code");
            return;
        }
        switch (currentBooking.PassStatus)
        {
            case (int)Enums.BookingStatus.Active:
                if (currentBooking.PassStatus == (int)Enums.BookingStatus.Expired)
                {
                    context.Response.Write("Pass expired with: ");
                    context.Response.Write(string.Format("{0} {1} - {2}", currentBooking.CustomerInfos.FirstName, currentBooking.CustomerInfos.LastName, currentBooking.ExpiredDate));
                    currentBooking.PassStatus = (int) Enums.BookingStatus.Expired;
                    currentBooking.ExpiredDate = DateTime.UtcNow;
                    bookingRepository.Update(currentBooking);
                }
                else
                {
                    context.Response.Write("Validated successfully with: ");
                    context.Response.Write(string.Format("{0} {1} - {2}", currentBooking.CustomerInfos.FirstName, currentBooking.CustomerInfos.LastName, DateTime.UtcNow));
                    currentBooking.PassStatus = (int) Enums.BookingStatus.Redeemed;
                    currentBooking.ExpiredDate = DateTime.UtcNow;
                    bookingRepository.Update(currentBooking);
                }
                break;
            case (int)Enums.BookingStatus.Redeemed:
                context.Response.Write("Code has already been redeemed with: ");
                context.Response.Write(string.Format("{0} {1} - {2}", currentBooking.CustomerInfos.FirstName, currentBooking.CustomerInfos.LastName, currentBooking.RedeemedDate));
                break;
            case (int)Enums.BookingStatus.Expired:
                context.Response.Write("Pass expired with: ");
                context.Response.Write(string.Format("{0} {1} - {2}", currentBooking.CustomerInfos.FirstName, currentBooking.CustomerInfos.LastName, currentBooking.ExpiredDate));
                break;
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}