<%@ WebHandler Language="C#" Class="UpdateHotel" %>

using System.Web;
using DayaxeDal.Repositories;

public class UpdateHotel : IHttpHandler {

    public void ProcessRequest (HttpContext context)
    {
        var helper = new HotelRepository();
        PhotoRepository photoRepository = new PhotoRepository();
        ProductImageRepository productImageRepository = new ProductImageRepository();
        var hotelId = context.Request.Params["id"];
        var userName = context.Request.Params["userName"] ?? string.Empty;
        int id;
        int.TryParse(hotelId, out id);
        var hotel = helper.GetHotel(id, userName);
        context.Response.ContentType = "application/json";
        if (hotel != null)
        {
            var isDeactive = context.Request.Params["deActive"] != null ? true : false;
            var isDelete = context.Request.Params["isActive"] != null ? true : false;
            if (isDelete)
            {
                helper.Delete(id);
            }
            if (isDeactive)
            {
                var hotels = helper.GetById(id);
                hotels.IsActive = false;
                helper.Update(hotels);
            }

        }

        var photoId = context.Request.Params["photoId"];
        if (photoId != null)
        {
            photoRepository.Delete(int.Parse(photoId));
        }

        var productPhotoId = context.Request.Params["productPhotoId"];
        if (productPhotoId != null)
        {
            productImageRepository.Delete(int.Parse(productPhotoId));
        }
        var sPhotoId = context.Request.Params["sPhotoId"];
        if (sPhotoId != null)
        {
            SubscriptionRepository subscriptionRepository = new SubscriptionRepository();
            subscriptionRepository.DeleteImage(int.Parse(sPhotoId));
        }

        productImageRepository.ResetCache();

        context.Response.Write("{success:true}");
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}