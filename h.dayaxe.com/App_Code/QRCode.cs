using System;
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using ZXing.Common;

/// <summary>
/// Summary description for QRCode
/// </summary>
public static class QRCode
{
    public static string GetImageSource(string url)
    {
        var barcodeWriter = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new EncodingOptions
            {
                Height = 150,
                Width = 150,
                Margin = 0
            }
        };

        using (var bitmap = barcodeWriter.Write(url))
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Gif);
                return String.Format("data:image/gif;base64,{0}",
                    Convert.ToBase64String(stream.ToArray()));
            }
        }
    }
}