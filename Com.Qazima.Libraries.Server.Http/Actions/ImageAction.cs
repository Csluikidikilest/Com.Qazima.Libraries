//using Com.Qazima.Libraries.Server.Http.Actions.Parameters.ImageParameters;
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Drawing2D;
//using System.Drawing.Imaging;
//using System.Linq;
//using System.Net;
//using System.Web.Script.Serialization;

//namespace Com.Qazima.Libraries.Server.Http.Actions
//{
//  public class ImageAction : IAction
//  {
//    /// <summary>
//    /// Image to work with
//    /// </summary>
//    protected Bitmap Image { get; set; }

//    /// <summary>
//    /// Constructor of a image action
//    /// </summary>
//    /// <param name="imagePath">path of the image to work with</param>
//    public ImageAction(string imagePath)
//    {
//      Image = (Bitmap)System.Drawing.Image.FromFile(imagePath);
//    }

//    /// <summary>
//    /// Constructor of a image action
//    /// </summary>
//    /// <param name="image">Image to work with</param>
//    public ImageAction(Bitmap image)
//    {
//      Image = image;
//    }

//    /// <summary>
//    /// Process the current context
//    /// </summary>
//    /// <param name="context">Context to process</param>
//    /// <returns>True if handled, else false</returns>
//    public bool Process(HttpListenerContext context)
//    {
//      bool result = false;
//      switch (context.Request.HttpMethod.ToUpper())
//      {
//        case "GET":
//        case "POST":
//          result = ProcessActions(context);
//          break;
//      }

//      return result;
//    }

//    /// <summary>
//    /// Generate Json flux from the collection
//    /// </summary>
//    /// <param name="context">Context to process</param>
//    /// <returns>True if handled, else false</returns>
//    protected bool ProcessActions(HttpListenerContext context)
//    {
//      try
//      {
//        string parameters = null;
//        if (context.Request.HttpMethod.Equals("post", StringComparison.InvariantCultureIgnoreCase))
//        {
//          using (System.IO.Stream body = context.Request.InputStream)
//          {
//            using (System.IO.StreamReader reader = new System.IO.StreamReader(body, context.Request.ContentEncoding))
//            {
//              parameters = reader.ReadToEnd();
//            }
//          }
//        }
//        else if (context.Request.HttpMethod.Equals("get", StringComparison.InvariantCultureIgnoreCase))
//        {
//          parameters = context.Request.Url.Query.Replace("?", "{'").Replace("=", "':'").Replace("&", "', '") + "'}";
//        }

//        ImageParameter objFromParameters = new ImageParameter();

//        if (!string.IsNullOrWhiteSpace(parameters))
//        {
//          objFromParameters = new JavaScriptSerializer().Deserialize<ImageParameter>(parameters);
//        }

//        int destHeight = objFromParameters.Height;
//        int destWidth = objFromParameters.Width;
//        if (destHeight == 0)
//        {
//          destHeight = Image.Height;
//        }

//        if (destWidth == 0)
//        {
//          destWidth = Image.Width;
//        }

//        Bitmap result = new Bitmap(destWidth, destHeight);
//        result.SetResolution(Image.HorizontalResolution, Image.VerticalResolution);

//        Graphics g = Graphics.FromImage(result);

//        ImageAttributes attributes = new ImageAttributes();
//        if (objFromParameters.GrayScale)
//        {
//          attributes.SetColorMatrix(new ColorMatrix(
//            new float[][]
//            {
//              new float[] {.3f, .3f, .3f, 0, 0},
//              new float[] {.59f, .59f, .59f, 0, 0},
//              new float[] {.11f, .11f, .11f, 0, 0},
//              new float[] {0, 0, 0, 1, 0},
//              new float[] {0, 0, 0, 0, 1},
//            }));
//        }

//        g.CompositingMode = CompositingMode.SourceCopy;
//        g.CompositingQuality = CompositingQuality.HighQuality;
//        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
//        g.SmoothingMode = SmoothingMode.HighQuality;
//        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

//        g.DrawImage(Image, new Rectangle(0, 0, destWidth, destHeight), 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel, attributes);
//        g.Dispose();
//        GC.SuppressFinalize(g);

//        ImageConverter imgConverter = new ImageConverter();
//        byte[] buffer = (byte[])imgConverter.ConvertTo(result, typeof(byte[]));
//        int bytesCount = buffer.Length;
//        DateTime currDate = DateTime.Now;
//        //Adding permanent http response headers
//        context.Response.ContentType = "image/jpeg";
//        context.Response.ContentLength64 = bytesCount;
//        context.Response.AddHeader("Date", currDate.ToString("r"));
//        context.Response.AddHeader("Last-Modified", currDate.ToString("r"));

//        context.Response.OutputStream.Write(buffer, 0, bytesCount);

//        context.Response.StatusCode = (int)HttpStatusCode.OK;
//        context.Response.OutputStream.Flush();
//      }
//      catch
//      {
//        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
//        return false;
//      }

//      return true;
//    }
//  }
//}
