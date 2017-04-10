using System.Net;

namespace Com.Qazima.Libraries.Server.Http
{
  /// <summary>
  /// Interface of actions
  /// </summary>
  public interface IAction
  {
    /// <summary>
    /// Process method
    /// </summary>
    /// <param name="context">Context to process</param>
    /// <returns>True if handled, else false</returns>
    bool Process(HttpListenerContext context);
  }
}
