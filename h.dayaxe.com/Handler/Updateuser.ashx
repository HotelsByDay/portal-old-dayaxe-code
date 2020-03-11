<%@ WebHandler Language="C#" Class="UpdateUser" %>

using System.Web;
using DayaxeDal.Repositories;

public class UpdateUser : IHttpHandler {

    public void ProcessRequest (HttpContext context)
    {
        var helper = new UserRepository();
        var userId = context.Request.Params["id"];
        int id = 0;
        int.TryParse(userId, out id);
        var isActive = context.Request.Params["isActive"] != null ? true : false;
        var isDelete = context.Request.Params["isDelete"] != null ? true : false;
        var users = helper.GetById(id);
        if (users != null && isActive)
        {
            users.IsActive = true;
            helper.Update(users);
        }
        if (isDelete)
        {
            helper.Delete(id);
        }

        helper.ResetCache();

        context.Response.ContentType = "application/json";
        context.Response.Write("{success:true}");
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}