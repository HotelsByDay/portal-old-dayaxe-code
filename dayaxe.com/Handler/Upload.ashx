<%@ WebHandler Language="C#" Class="Upload" %>

using System;
using System.IO;
using System.Net;
using System.Web;

public class Upload : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        try
        {
            var urlImage = context.Request.Params["imageUrl"];
            var hotelId = context.Request.Params["id"];
            int id = 0;
            int.TryParse(hotelId, out id);
            string pathString = context.Server.MapPath(string.Format("/HotelImage/{0}/", hotelId));
            string imagename = urlImage.Substring(urlImage.LastIndexOf('/') + 1,
                urlImage.Length - (urlImage.LastIndexOf('/') + 1));
            string localImageFile = context.Server.MapPath(string.Format("/HotelImage/{0}/{1}", hotelId, imagename));
            if (!Directory.Exists(pathString))
            {
                Directory.CreateDirectory(pathString);
            }
            try
            {
                SaveImageFromUrl(localImageFile, urlImage);
            }
            catch (Exception ex)
            {
                    throw new Exception(ex.Message);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

        context.Response.ContentType = "application/json";
        context.Response.Write("{}");
    }

        public void SaveImageFromUrl(string imageName, string url)
    {
        byte[] content;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        WebResponse response = request.GetResponse();

        Stream stream = response.GetResponseStream();

        using (BinaryReader br = new BinaryReader(stream))
        {
            content = br.ReadBytes(500000);
            br.Close();
        }
        response.Close();

        FileStream fs = new FileStream(imageName, FileMode.Create);
        BinaryWriter bw = new BinaryWriter(fs);
        try
        {
            bw.Write(content);
        }
        finally
        {
            fs.Close();
            bw.Close();
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