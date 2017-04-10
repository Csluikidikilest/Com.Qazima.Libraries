using Com.Qazima.Libraries.Server.Http.Actions.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Com.Qazima.Libraries.Server.Http.Actions
{
  public class DirectoryAction : IAction
  {
    /// <summary>
    /// List of index files
    /// </summary>
    public List<string> IndexFiles { get; set; }

    /// <summary>
    /// Directory to render
    /// </summary>
    public string Directory { get; set; }

    /// <summary>
    /// Event handler for the Get action
    /// </summary>
    public event EventHandler<ActionGetEventArgs> OnDirectoryActionGet;

    /// <summary>
    /// Mimes types
    /// </summary>
    public Dictionary<string, string> MimeTypeMappings { get; private set; }

    /// <summary>
    /// Constructor of a directory action
    /// </summary>
    /// <param name="directory">Mapped directory</param>
    /// <param name="indexFiles">Index files</param>
    public DirectoryAction(string directory, params string[] indexFiles)
    {
      Directory = directory;
      IndexFiles = indexFiles.ToList();
      if (IndexFiles == null)
      {
        IndexFiles = new List<string>();
      }

      if (!IndexFiles.Any())
      {
        IndexFiles.Add("index.html");
        IndexFiles.Add("index.htm");
      }

      MimeTypeMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Process the current context and rendering a directory
    /// </summary>
    /// <param name="context">Context to process</param>
    /// <returns>True if handled, else false</returns>
    public bool Process(HttpListenerContext context)
    {
      ActionGetEventArgs eventArgs = new ActionGetEventArgs() { AskedDate = DateTime.Now, AskedUrl = context.Request.Url };
      string absolutePath = context.Request.Url.AbsolutePath;

      string filename = Path.GetFileName(absolutePath);
      string directory = context.Request.Url.AbsolutePath;

      if (!string.IsNullOrEmpty(filename))
      {
        directory = directory.Replace(filename, string.Empty);
      }

      if (filename.StartsWith("/"))
      {
        filename = filename.Substring(1);
      }

      if (directory.StartsWith("/"))
      {
        directory = directory.Substring(1);
      }

      if (filename.Equals(directory))
      {
        filename = string.Empty;
      }

      if (directory.Contains("/") && !directory.EndsWith("/"))
      {
        List<string> directoryExplode = directory.Split('/').ToList();
        directory = string.Join("/", directoryExplode.Take(directoryExplode.Count - 1));
      }

      directory = directory.Replace('/', Path.DirectorySeparatorChar);

      if (string.IsNullOrEmpty(filename))
      {
        foreach (string indexFile in IndexFiles)
        {
          if (File.Exists(Path.Combine(Directory, directory, indexFile)))
          {
            filename = indexFile;
            break;
          }
        }
      }

      filename = Path.Combine(Directory, directory, filename);

      if (File.Exists(filename))
      {
        try
        {
          Stream input = new FileStream(filename, FileMode.Open, FileAccess.Read);

          //Adding permanent http response headers
          string mime;
          context.Response.ContentType = MimeTypeMappings.TryGetValue(Path.GetExtension(filename), out mime) ? mime : "application/octet-stream";
          context.Response.ContentLength64 = input.Length;
          context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
          context.Response.AddHeader("Last-Modified", File.GetLastWriteTime(filename).ToString("r"));

          byte[] buffer = new byte[2048 * 16];
          int nbytes;
          while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
          {
            context.Response.OutputStream.Write(buffer, 0, nbytes);
          }
          input.Close();

          context.Response.StatusCode = (int)HttpStatusCode.OK;
          context.Response.OutputStream.Flush();
        }
        catch
        {
          context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
      }
      else
      {
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
      }

      context.Response.OutputStream.Close();

      eventArgs.EndDate = DateTime.Now;
      eventArgs.ResponseHttpStatusCode = (HttpStatusCode)context.Response.StatusCode;
      OnGetAction(eventArgs);

      return context.Response.StatusCode == (int)HttpStatusCode.OK;
    }

    /// <summary>
    /// Fire event on an action
    /// </summary>
    /// <param name="e">Event arguments</param>
    protected virtual void OnGetAction(ActionGetEventArgs e)
    {
      OnDirectoryActionGet?.Invoke(this, e);
    }
  }
}
