using Com.Qazima.Libraries.Server.Http.Actions.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

namespace Com.Qazima.Libraries.Server.Http.Actions
{
  public class JSonActionReadOnly<ListObjectType, ObjectType> : IAction where ListObjectType : IEnumerable<ObjectType> where ObjectType : IHasId
  {
    /// <summary>
    /// Event handler for the Get action
    /// </summary>
    public event EventHandler<ActionGetEventArgs> OnJSonActionGet;

    /// <summary>
    /// Object to parse
    /// </summary>
    protected ListObjectType Item { get; set; }

    /// <summary>
    /// Constructor of a json action read only
    /// </summary>
    /// <param name="item">item to parse</param>
    public JSonActionReadOnly(ListObjectType item)
    {
      Item = item;
    }

    /// <summary>
    /// Process the current context
    /// </summary>
    /// <param name="context">Context to process</param>
    /// <returns>True if handled, else false</returns>
    public virtual bool Process(HttpListenerContext context)
    {
      bool result = false;
      switch (context.Request.HttpMethod.ToUpper())
      {
        case "GET":
          result = ProcessGet(context);
          break;
        case "HEAD":
          result = ProcessHead(context);
          break;
      }

      return result;
    }

    /// <summary>
    /// Fire event on an action
    /// </summary>
    /// <param name="e">Event arguments</param>
    protected virtual void OnGetAction(ActionGetEventArgs e)
    {
      OnJSonActionGet?.Invoke(this, e);
    }

    /// <summary>
    /// Generate Json flux from the collection
    /// </summary>
    /// <param name="context">Context to process</param>
    /// <returns>True if handled, else false</returns>
    protected bool ProcessGet(HttpListenerContext context)
    {
      ActionGetEventArgs eventArgs = new ActionGetEventArgs() { AskedDate = DateTime.Now, AskedUrl = context.Request.Url };
      bool result = true;
      try
      {
        PrepareResponse(context);
      }
      catch
      {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        result = false;
      }

      eventArgs.EndDate = DateTime.Now;
      eventArgs.ResponseHttpStatusCode = (HttpStatusCode)context.Response.StatusCode;
      OnGetAction(eventArgs);

      return result;
    }

    /// <summary>
    /// Generate head flux
    /// </summary>
    /// <param name="context">Context to process</param>
    /// <returns>True if handled, else false</returns>
    protected bool ProcessHead(HttpListenerContext context)
    {
      try
      {
        DateTime currDate = DateTime.Now;
        //Adding permanent http response headers
        context.Response.ContentType = "application/javascript";
        context.Response.ContentLength64 = 0;
        context.Response.AddHeader("Date", currDate.ToString("r"));
        context.Response.AddHeader("Last-Modified", currDate.ToString("r"));

        byte[] buffer = new byte[0];
        Buffer.BlockCopy(string.Empty.ToCharArray(), 0, buffer, 0, 0);
        context.Response.OutputStream.Write(buffer, 0, 0);

        context.Response.StatusCode = (int)HttpStatusCode.OK;
        context.Response.OutputStream.Flush();
      }
      catch
      {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        return false;
      }

      return true;
    }

    /// <summary>
    /// Prepare reponse
    /// </summary>
    /// <param name="context">Context to process</param>
    protected void PrepareResponse(HttpListenerContext context)
    {
      IEnumerable<ObjectType> filteredItems = Item;
      List<string> keys = context.Request.QueryString.AllKeys.ToList();

      foreach(string key in keys)
      {
        PropertyInfo property = Item.First().GetType().GetProperty(key);
        if(property != null)
        {
          filteredItems = filteredItems.Where(item => property.GetValue(item, null).ToString().Equals(Encoding.UTF8.GetString(Encoding.Default.GetBytes(context.Request.QueryString[key])), StringComparison.InvariantCultureIgnoreCase));
        }
      }

      string strItem = new JavaScriptSerializer().Serialize(filteredItems);
      byte[] buffer = Encoding.UTF8.GetBytes(strItem.ToCharArray());
      int bytesCount = buffer.Length;
      DateTime currDate = DateTime.Now;
      //Adding permanent http response headers
      context.Response.ContentType = "application/javascript";
      context.Response.ContentLength64 = bytesCount;
      context.Response.AddHeader("Date", currDate.ToString("r"));
      context.Response.AddHeader("Last-Modified", currDate.ToString("r"));

      context.Response.OutputStream.Write(buffer, 0, bytesCount);

      context.Response.StatusCode = (int)HttpStatusCode.OK;
      context.Response.OutputStream.Flush();
    }
  }
}
