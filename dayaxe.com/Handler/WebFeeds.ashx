//<%@ WebHandler Language="C#" Class="WebFeeds" %>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using DayaxeDal.Repositories;
using System.Linq;
using DayaxeDal.Custom;
using DayaxeDal.Extensions;
using Newtonsoft.Json;

public class WebFeeds : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        using (var repository = new BookingRepository())
        {
            var ps = context.Request.Params["pageSize"] ?? string.Empty;
            int pageSize;
            if (!int.TryParse(ps, NumberStyles.Any, null, out pageSize))
            {
                pageSize = 5;
            }

            var surveys = (from h in repository.HotelList
                           join p in repository.ProductList on h.HotelId equals p.HotelId
                           join b in repository.BookingList on p.ProductId equals b.ProductId
                           join s in repository.SurveyList on b.BookingId equals s.BookingId
                           join ci in repository.CustomerInfoList on b.CustomerId equals ci.CustomerId
                           where p.IsActive && h.IsActive && h.IsPublished && s.IsFinish && s.Rating.Equals(5.0)
                           orderby b.RedeemedDate descending
                           select new SurveyResponseObject
                           {
                               HotelId = h.HotelId,
                               BookingId = b.BookingId,
                               SurveyCode = s.Code,
                               CustomerId = b.CustomerId,
                               Rating = s.Rating ?? 5,
                               RateCommend = s.RateCommend,
                               UpdatedDate = (s.LastUpdatedDate ?? DateTime.UtcNow)
                                    .ToLosAngerlesTimeWithTimeZone(h.TimeZoneId),
                               UpdatedDateWithFormat = (s.LastUpdatedDate ?? DateTime.UtcNow)
                                    .ToLosAngerlesTimeWithTimeZone(h.TimeZoneId)
                                    .ToString("ddd, MMM dd. yyyy"),
                               ByUser = string.Format("{0} {1}.",
                                   !string.IsNullOrEmpty(ci.FirstName) ? ci.FirstName : string.Empty,
                                   (!string.IsNullOrEmpty(ci.LastName) ? ci.LastName[0] : ' ')),
                               FirstName = ci.FirstName,
                               LastName = ci.LastName
                           }).ToList();

            var surveysGroupByHotel = surveys.GroupBy(g => g.HotelId)
                .Select(group => new {HotelId = group.Key, SurveyList = group.OrderByDescending(g => g.UpdatedDate).ToList()})
                .ToList();

            var responseObject = new List<SurveyResponseObject>();

            surveysGroupByHotel.ForEach(survey =>
            {
                responseObject.AddRange(survey.SurveyList.Take(pageSize));
            });

            var response = JsonConvert.SerializeObject(responseObject, CustomSettings.SerializerSettingsWithFullDateTime());

            context.Response.ContentType = "application/json";
            context.Response.Write(response);
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}