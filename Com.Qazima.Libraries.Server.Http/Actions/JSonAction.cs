using Com.Qazima.Libraries.Server.Http.Actions.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;

namespace Com.Qazima.Libraries.Server.Http.Actions
{
  public class JSonAction<ListObjectType, ObjectType> : JSonActionReadOnly<ListObjectType, ObjectType> where ListObjectType : IList<ObjectType> where ObjectType : IHasId
  {
    /// <summary>
    /// Event handler for the Delete action
    /// </summary>
    public event EventHandler<JsonActionDeleteEventArgs<ObjectType>> OnJSonActionDelete;

    /// <summary>
    /// Event handler for the Post action
    /// </summary>
    public event EventHandler<JsonActionPostEventArgs<ObjectType>> OnJSonActionPost;

    /// <summary>
    /// Event handler for the Put action
    /// </summary>
    public event EventHandler<JsonActionPutEventArgs<ObjectType>> OnJSonActionPut;

    /// <summary>
    /// Constructor of a json action
    /// </summary>
    /// <param name="item">item to parse</param>
    public JSonAction(ListObjectType item) : base(item)
    { }

    /// <summary>
    /// Process the current context
    /// </summary>
    /// <param name="context">Context to process</param>
    /// <returns>True if handled, else false</returns>
    public override bool Process(HttpListenerContext context)
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
        case "POST":
          result = ProcessPost(context);
          break;
        case "PUT":
          result = ProcessPut(context);
          break;
        case "DELETE":
          result = ProcessDelete(context);
          break;
      }

      return result;
    }

    /// <summary>
    /// Fire event on an action
    /// </summary>
    /// <param name="e">Event arguments</param>
    protected virtual void OnDeleteAction(JsonActionDeleteEventArgs<ObjectType> e)
    {
      OnJSonActionDelete?.Invoke(this, e);
    }

    /// <summary>
    /// Fire event on an action
    /// </summary>
    /// <param name="e">Event arguments</param>
    protected virtual void OnPostAction(JsonActionPostEventArgs<ObjectType> e)
    {
      OnJSonActionPost?.Invoke(this, e);
    }

    /// <summary>
    /// Fire event on an action
    /// </summary>
    /// <param name="e">Event arguments</param>
    protected virtual void OnPutAction(JsonActionPutEventArgs<ObjectType> e)
    {
      OnJSonActionPut?.Invoke(this, e);
    }

    /// <summary>
    /// Generate object from Json flux and add it to the collection
    /// </summary>
    /// <param name="context">Context to process</param>
    /// <returns>True if handled, else false</returns>
    protected bool ProcessPost(HttpListenerContext context)
    {
      JsonActionPostEventArgs<ObjectType> eventArgs = new JsonActionPostEventArgs<ObjectType>() { AskedDate = DateTime.Now, AskedUrl = context.Request.Url };
      bool result = true;
      try
      {
        if (!context.Request.HasEntityBody)
        {
          result = false;
        }
        else
        {
          string parameters = null;
          using (System.IO.Stream body = context.Request.InputStream)
          {
            using (System.IO.StreamReader reader = new System.IO.StreamReader(body, context.Request.ContentEncoding))
            {
              parameters = reader.ReadToEnd();
            }
          }

          if (string.IsNullOrWhiteSpace(parameters))
          {
            result = false;
          }
          else
          {
            ObjectType objFromParameters = new JavaScriptSerializer().Deserialize<ObjectType>(parameters);

            if (!Item.Any(item => item.Id.Equals(objFromParameters.Id)))
            {
              Item.Add(objFromParameters);
              eventArgs.New = objFromParameters;
              PrepareResponse(context);
            }
            else
            {
              result = false;
            }
          }
        }
      }
      catch
      {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        result = false;
      }

      eventArgs.EndDate = DateTime.Now;
      eventArgs.ResponseHttpStatusCode = (HttpStatusCode)context.Response.StatusCode;
      OnPostAction(eventArgs);

      result = true;
      return result;
    }

    /// <summary>
    /// Generate object from Json flux and replace the old one (based on id) with the new one
    /// </summary>
    /// <param name="context">Context to process</param>
    /// <returns>True if handled, else false</returns>
    protected bool ProcessPut(HttpListenerContext context)
    {
      JsonActionPutEventArgs<ObjectType> eventArgs = new JsonActionPutEventArgs<ObjectType>() { AskedDate = DateTime.Now, AskedUrl = context.Request.Url };
      bool result = true;
      try
      {
        if (!context.Request.HasEntityBody)
        {
          result = false;
        }
        else
        {
          string parameters = null;
          using (System.IO.Stream body = context.Request.InputStream)
          {
            using (System.IO.StreamReader reader = new System.IO.StreamReader(body, context.Request.ContentEncoding))
            {
              parameters = reader.ReadToEnd();
            }
          }

          if (string.IsNullOrWhiteSpace(parameters))
          {
            result = false;
          }
          else
          {
            ObjectType objFromParameters = new JavaScriptSerializer().Deserialize<ObjectType>(parameters);
            if (objFromParameters == null)
            {
              result = false;
            }
            else
            {
              eventArgs.New = objFromParameters;
              ObjectType objFromCollection = Item.FirstOrDefault(item => item.Id.Equals(objFromParameters.Id));
              if (objFromCollection == null)
              {
                result = false;
              }
              else
              {
                eventArgs.Old = objFromCollection;
                int index = Item.IndexOf(objFromCollection);
                Item[index] = objFromParameters;

                PrepareResponse(context);
              }
            }
          }
        }
      }
      catch
      {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        result = false;
      }

      eventArgs.EndDate = DateTime.Now;
      eventArgs.ResponseHttpStatusCode = (HttpStatusCode)context.Response.StatusCode;
      OnPutAction(eventArgs);

      return result;
    }

    /// <summary>
    /// Delete object from collection (based on id)
    /// </summary>
    /// <param name="context">Context to process</param>
    /// <returns>True if handled, else false</returns>
    protected bool ProcessDelete(HttpListenerContext context)
    {
      JsonActionDeleteEventArgs<ObjectType> eventArgs = new JsonActionDeleteEventArgs<ObjectType>() { AskedDate = DateTime.Now, AskedUrl = context.Request.Url };
      bool result = true;
      try
      {
        if (!context.Request.HasEntityBody)
        {
          result = false;
        }
        else
        {
          string parameters = null;
          using (System.IO.Stream body = context.Request.InputStream)
          {
            using (System.IO.StreamReader reader = new System.IO.StreamReader(body, context.Request.ContentEncoding))
            {
              parameters = reader.ReadToEnd();
            }
          }

          if (string.IsNullOrWhiteSpace(parameters))
          {
            result = false;
          }
          else
          {
            ObjectType objFromParameters = new JavaScriptSerializer().Deserialize<ObjectType>(parameters);
            if (objFromParameters == null)
            {
              result = false;
            }
            else
            {
              ObjectType objFromCollection = Item.FirstOrDefault(item => item.Id.Equals(objFromParameters.Id));
              if (objFromCollection == null)
              {
                result = false;
              }
              else
              {
                eventArgs.Old = objFromCollection;
                Item.Remove(objFromCollection);

                PrepareResponse(context);
              }
            }
          }
        }
      }
      catch
      {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        result = false;
      }

      eventArgs.EndDate = DateTime.Now;
      eventArgs.ResponseHttpStatusCode = (HttpStatusCode)context.Response.StatusCode;
      OnDeleteAction(eventArgs);

      return result;
    }
  }
}
