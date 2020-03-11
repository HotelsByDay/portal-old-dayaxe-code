<%@ WebHandler Language="C#" Class="ValidatePin" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using DayaxeDal;
using DayaxeDal.Repositories;

public class ValidatePin : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        var bookingsRepository = new BookingRepository();
        var id = context.Request.Params["bookId"];
        var pinCode = context.Request.Params["code"] ?? string.Empty;
        var expire = context.Request.Params["exp"] ?? String.Empty;
        int bookingId;
        int.TryParse(id, out bookingId);
        string response = "{}";

        try
        {
            if (!string.IsNullOrEmpty(expire) && expire == "0")
            {
                response = bookingsRepository.ValidatePinWithoutExpired(bookingId, pinCode);
            }
            else
            {
                response = bookingsRepository.ValidatePinCode(bookingId, pinCode);
            }

            var javaScriptSerializer = new JavaScriptSerializer();
            var responseData = javaScriptSerializer.Deserialize<Response>(response);

            if (responseData.IsSuccess)
            {
                var schedules = new Schedules
                {
                    BookingId = bookingId,
                    Name = "EmailSurvey",
                    ScheduleSendType = (int) Enums.ScheduleSendType.IsEmailSurvey,
                    Status = (int) Enums.ScheduleType.NotRun,
                    SendAt = DateTime.UtcNow.AddMinutes(AppConfiguration.SendEmailSurveyAfterMinutes)
                };
                bookingsRepository.AddSchedule(schedules);

                // Insert Schedule Send Add On Notification
                var schedulesAddOn = new Schedules
                {
                    ScheduleSendType = (int)Enums.ScheduleSendType.IsAddOnNotificationAfterRedemption,
                    Name = "Send Add-On Notification Redemption",
                    Status = (int)Enums.ScheduleType.NotRun,
                    BookingId = bookingId,
                    SendAt = DateTime.UtcNow.AddMinutes(AppConfiguration.SendEmailSurveyAfterMinutes)
                };
                bookingsRepository.AddSchedule(schedulesAddOn);
            }
        }
        catch (Exception ex)
        {
            using (var logRepository = new LogRepository())
            {
                var log = new Logs
                {
                    LogKey = "RedeemedError",
                    UpdatedContent = String.Format("{0}-{1}-{2}-{3}", bookingId, ex.Source, ex.Message, ex.StackTrace),
                    UpdatedDate = DateTime.UtcNow
                };
                logRepository.Add(log);
            }
        }
        context.Response.ContentType = "application/json";
        context.Response.Write(response);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}