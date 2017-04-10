using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Com.Qazima.Libraries.Server.Http.Actions
{
  public enum KnownMimeType
  {
    Javascript,
    Css,
  }

  public class BundleTextFileAction : IAction
  {
    public List<KeyValuePair<KnownMimeType, string>> Bundle { get; set; }

    //    public bool MinifyText { get; set; }

    public BundleTextFileAction()
    { }

    public BundleTextFileAction(params KeyValuePair<KnownMimeType, string>[] files)
    {
      Bundle = files.ToList();
    }

    public bool Process(HttpListenerContext context)
    {
      context.Response.ContentType = "application/octet-stream";
      context.Response.ContentLength64 = 0;
      context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
      context.Response.AddHeader("Last-Modified", DateTime.Now.ToString("r"));

      foreach (KeyValuePair<KnownMimeType, string> file in Bundle)
      {
        string filename = file.Value;
        if (File.Exists(filename))
        {
          try
          {
            Stream input = new FileStream(filename, FileMode.Open);

            //Adding permanent http response headers
            context.Response.ContentLength64 += input.Length;
            byte[] buffer = new byte[1024 * 16];
            int nbytes;
            while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
            {
              context.Response.OutputStream.Write(buffer, 0, nbytes);
            }
            input.Close();

            context.Response.OutputStream.Flush();
          }
          catch
          {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
          }
        }
      }
      context.Response.OutputStream.Close();
      return context.Response.StatusCode == (int)HttpStatusCode.OK;
    }
  }
}
