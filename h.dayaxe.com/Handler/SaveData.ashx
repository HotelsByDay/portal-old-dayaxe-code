<%@ WebHandler Language="C#" Class="SaveData" %>

using System.Web;
using System.Web.SessionState;
using DayaxeDal;
using DayaxeDal.Repositories;

public class SaveData : IHttpHandler, IRequiresSessionState
{

    public void ProcessRequest (HttpContext context)
    {
        var dalHelper = new HotelRepository();
        var hotelId = context.Request.Form["id"];
        var userName = context.Request.Params["userName"] ?? string.Empty;
        var targets = context.Request.Form["target"];
        var genders = context.Request.Form["gender"];
        var educations = context.Request.Form["education"];
        var income = context.Request.Form["incomCurrent"];
        var distance = context.Request.Form["distanceCurrent"];
        var agefrom = context.Request.Form["ageFromCurrent"];
        var ageto = context.Request.Form["ageToCurrent"];
        int id = int.Parse(hotelId);
        var hotel = dalHelper.GetHotel(id, userName);
        if (hotel != null)
        {
            hotel.TargetGroups = targets;
            hotel.Gender = genders;
            hotel.Education = educations;
            hotel.Income = income;
            hotel.Distance = distance;
            hotel.AgeFrom = int.Parse(agefrom);
            hotel.AgeTo = int.Parse(ageto);
            dalHelper.Update(hotel);
        }
        
        context.Response.ContentType = "text/plain";
        context.Response.Write("Done");
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}