using System;
using System.IO;
using System.Web.UI;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DayaxeDal;
using DayaxeDal.Repositories;
using Page = System.Web.UI.Page;

namespace h.dayaxe.com.Helper
{
    public partial class HelperCloudinaryEx : Page
    {
        private readonly BookingRepository _bookingRepository = new BookingRepository();
        protected Cloudinary Cloudinary;
        protected string imageId = string.Empty;

        protected void Page_Init(object sender, EventArgs e)
        {
            Account account = new Account(
                "vietluyen",
                "385557456569739",
                "Si2Q2D3dxjqgya-Rl7-lZ0cy99Q");

            Cloudinary = new Cloudinary(account);

            ImageCloudinary.Visible = false;
            if (Session["PublicId"] != null)
            {
                ImageCloudinary.Visible = true;
                imageId = (string) Session["PublicId"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ImageCloudinary.ImageUrl = Cloudinary.Api.UrlImgUp.Secure()
                    .Transform(new Transformation().Width(960).Height(486).Crop("fill"))
                    .BuildUrl(imageId);
            }
        }

        protected void SaveButtonOnClick(object sender, EventArgs e)
        {
            if (ImageFileUpload.HasFile)
            {
                var file = ImageFileUpload.PostedFile;
                if (Request.Params["id"] != "0")
                {
                    string pathString = Server.MapPath("~/cloudinary/");
                    string localImageFile = string.Format("/cloudinary/{0}", file.FileName);

                    if (!Directory.Exists(pathString))
                    {
                        Directory.CreateDirectory(pathString);
                    }
                    try
                    {
                        file.SaveAs(Server.MapPath(localImageFile));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }

                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(string.Format("{0}{1}", pathString, file.FileName)),
                        PublicId = "Hotel_" + Guid.NewGuid()
                    };
                    var uploadResult = Cloudinary.Upload(uploadParams);
                    imageId = string.Format("{0}.{1}", uploadResult.PublicId, uploadResult.Format);
                    Session["PublicId"] = imageId;

                    ImageCloudinary.ImageUrl = ImageCloudinary.ImageUrl = Cloudinary.Api.UrlImgUp.Secure()
                        .Transform(new Transformation().Width(960).Height(486).Crop("fill"))
                        .BuildUrl(imageId);
                    ImageCloudinary.Visible = true;

                    var log = new Logs
                    {
                        LogKey = "cloudinary",
                        UpdatedContent = uploadResult.JsonObj.ToString(),
                        UpdatedBy = 0,
                        UpdatedDate = DateTime.UtcNow
                    };

                    _bookingRepository.AddLog(log);
                }
            }
        }

        protected void DropDownList1_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            switch (DropDownList1.SelectedValue)
            {
                case "Raw":
                    ImageCloudinary.ImageUrl = Cloudinary.Api.UrlImgUp.Secure()
                        .BuildUrl(imageId);
                    break;
                case "Thumbnail":
                    ImageCloudinary.ImageUrl = Cloudinary.Api.UrlImgUp.Secure()
                        .Transform(new Transformation().Width(560).Height(283).Crop("fill"))
                        .BuildUrl(imageId);
                    break;
                default:
                    ImageCloudinary.ImageUrl = Cloudinary.Api.UrlImgUp.Secure()
                        .Transform(new Transformation().Width(960).Height(486).Crop("fill"))
                        .BuildUrl(imageId);
                    break;
            }
        }
    }
}