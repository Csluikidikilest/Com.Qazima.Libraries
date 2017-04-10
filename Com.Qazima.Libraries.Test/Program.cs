using Com.Qazima.Libraries.Server.Http;
using Com.Qazima.Libraries.Server.Http.Actions;
using Com.Qazima.Libraries.Server.Http.Actions.Events;
using System;
using System.Collections.Generic;

namespace Com.Qazima.Libraries.Test
{
  class Program
  {
    static Tests Tests { get; set; }

    static string Url
    {
      get
      {
        return "127.0.0.1";
      }
    }

    static int Port
    {
      get
      {
        return 8070;
      }
    }

    static void Main(string[] args)
    {
      DirectoryAction da1 = new DirectoryAction(@"F:\AdminLTE-2.3.11");
      //DirectoryAction da2 = new DirectoryAction(@"F:\pictures");
      //ImageAction ia = new ImageAction(@"F:\pictures\pictures\BlueMoon.jpg");
      Tests = new Tests();
      Tests.Add(new Test() { Id = 1, Name = "Clément", Date = new DateTime(2016, 01, 01) });
      Tests.Add(new Test() { Id = 2, Name = "Clément", Date = new DateTime(2016, 01, 02) });
      Tests.Add(new Test() { Id = 3, Name = "éèàçù", Date = new DateTime(2016, 01, 03) });
      Tests.Add(new Test() { Id = 4, Name = "éèàçù", Date = new DateTime(2016, 01, 04) });
      JSonAction<Tests, Test> js = new JSonAction<Tests, Test>(Tests);

      Tests = new Tests();
      Tests.Add(new Test() { Id = 5, Name = "éèàçù", Date = new DateTime(2016, 01, 05) });
      Tests.Add(new Test() { Id = 6, Name = "éèàçù", Date = new DateTime(2016, 01, 06) });
      Tests.Add(new Test() { Id = 7, Name = "éèàçù", Date = new DateTime(2016, 01, 07) });
      Tests.Add(new Test() { Id = 8, Name = "éèàçù", Date = new DateTime(2016, 01, 08) });
      JSonActionReadOnly<Tests, Test> jsro = new JSonActionReadOnly<Tests, Test>(Tests);

      da1.MimeTypeMappings.Add(".htm", "text/html");
      da1.MimeTypeMappings.Add(".html", "text/html");
      HttpServer srv = new HttpServer(Url, Port);
      srv.AddAction("/", da1);
      //srv.AddAction("/pictures", da2);
      //srv.AddAction("/tests", ia);

      srv.AddAction("/data", js);
      srv.AddAction("/dataro", jsro);
      srv.Start();
      Console.WriteLine(string.Format("Listening on http://{0}:{1}", Url, Port));
      Console.ReadLine();
    }
  }

  public class Tests : List<Test>
  { }

  public class Test : IHasId
  {
    public int Id { get; set; }

    public string Name { get; set; }

    public DateTime Date { get; set; }
  }
}
