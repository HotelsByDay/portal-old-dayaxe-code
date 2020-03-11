<%@ WebHandler Language="C#" Class="GetTickets" %>

using System.Web;
using DayaxeDal.Extensions;
using DayaxeDal.Repositories;
using Newtonsoft.Json;

public class GetTickets : IHttpHandler {

    public void ProcessRequest (HttpContext context)
    {
        context.Response.ContentType = "application/json";
        var p = context.Request.Params["p"];
        int productId;
        if (int.TryParse(p, out productId))
        {
            using (var repository = new ProductRepository())
            {
                var tickets = repository.GetTicketsFuture(productId);
                string json = JsonConvert.SerializeObject(tickets, CustomSettings.SerializerSettings());
                context.Response.Write(json);
            }
        }
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}