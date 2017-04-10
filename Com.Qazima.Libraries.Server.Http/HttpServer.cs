using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Com.Qazima.Libraries.Server.Http
{
  public class HttpServer
  {
    /// <summary>
    /// Differents actions
    /// </summary>
    private Dictionary<string, IAction> Actions { get; set; }

    /// <summary>
    /// Server thread
    /// </summary>
    private Thread ServerThread;

    /// <summary>
    /// Http Listener
    /// </summary>
    private HttpListener Listener { get; set; }

    /// <summary>
    /// Listening port
    /// </summary>
    public int ListeningPort { get; set; }

    /// <summary>
    /// Listening url or ip address
    /// </summary>
    public string ListeningUrl { get; set; }

    /// <summary>
    /// Construct server with given port.
    /// </summary>
    /// <param name="url">Url of the server</param>
    /// <param name="port">Port of the server</param>
    public HttpServer(string url, int port)
    {
      Initialize(url, port);
    }

    /// <summary>
    /// Construct server with suitable port.
    /// </summary>
    /// <param name="url">Url of the server</param>
    public HttpServer(string url)
    {
      //get an empty port
      TcpListener l = new TcpListener(IPAddress.Loopback, 0);
      l.Start();
      int port = ((IPEndPoint)l.LocalEndpoint).Port;
      l.Stop();
      Initialize(url, port);
    }

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="url">Url of the server</param>
    /// <param name="port">Port of the server</param>
    private void Initialize(string url, int port)
    {
      ListeningUrl = url;
      ListeningPort = port;
      Actions = new Dictionary<string, IAction>();
      ServerThread = new Thread(Listen);
    }

    /// <summary>
    /// Add an action
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="action"></param>
    public void AddAction(string directory, IAction action)
    {
      if (!directory.EndsWith("/"))
      {
        directory += "/";
      }

      Actions.Add(directory, action);
    }

    /// <summary>
    /// Start server
    /// </summary>
    public void Start()
    {
      ServerThread.Start();
    }

    /// <summary>
    /// Stop server and dispose all functions.
    /// </summary>
    public void Stop()
    {
      ServerThread.Abort();
      Listener.Stop();
    }

    /// <summary>
    /// Start the web server
    /// </summary>
    private void Listen()
    {
      Listener = new HttpListener();
      Listener.Prefixes.Add("http://" + ListeningUrl + ":" + ListeningPort.ToString() + "/");
      Listener.Start();
      while (true)
      {
        try
        {
          HttpListenerContext context = Listener.GetContext();
          Process(context);
        }
        catch
        {
        }
      }
    }

    /// <summary>
    /// Process the current context with actions
    /// </summary>
    /// <param name="context">Context to process</param>
    private void Process(HttpListenerContext context)
    {
      try
      {
        if (!ProcessRawUrl(context.Request.RawUrl, context))
        {
          context.Response.StatusCode = (int)HttpStatusCode.NotFound;
          context.Response.OutputStream.Close();
        }
      }
      catch
      {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.OutputStream.Close();
      }
    }

    /// <summary>
    /// Process the current context with actions
    /// </summary>
    /// <param name="path">Requested path</param>
    /// <param name="context">Context to process</param>
    /// <returns>True if handled, else false</returns>
    private bool ProcessRawUrl(string path, HttpListenerContext context)
    {
      if (!path.EndsWith("/"))
      {
        path += "/";
      }

      if (Actions.ContainsKey(path))
      {
        return Actions.First(item => item.Key == path).Value.Process(context);
      }
      else
      {
        if (path.EndsWith("/"))
        {
          path = path.Substring(0, path.Length - 1);
        }

        if (!string.IsNullOrWhiteSpace(path))
        {
          List<string> pathExplode = path.Split('/').ToList();
          string parentPath = string.Join("/", pathExplode.Take(pathExplode.Count - 1));

          if (!parentPath.EndsWith("/"))
          {
            parentPath += "/";
          }

          if (!string.IsNullOrWhiteSpace(parentPath))
          {
            return ProcessRawUrl(parentPath, context);
          }
        }
      }

      return false;
    }
  }
}
