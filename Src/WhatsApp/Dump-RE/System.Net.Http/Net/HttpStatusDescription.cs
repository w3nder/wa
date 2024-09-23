// Decompiled with JetBrains decompiler
// Type: System.Net.HttpStatusDescription
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Runtime.CompilerServices;

#nullable disable
namespace System.Net
{
  internal static class HttpStatusDescription
  {
    private static readonly string[][] httpStatusDescriptions = new string[6][]
    {
      null,
      new string[3]
      {
        "Continue",
        "Switching Protocols",
        "Processing"
      },
      new string[8]
      {
        "OK",
        "Created",
        "Accepted",
        "Non-Authoritative Information",
        "No Content",
        "Reset Content",
        "Partial Content",
        "Multi-Status"
      },
      new string[8]
      {
        "Multiple Choices",
        "Moved Permanently",
        "Found",
        "See Other",
        "Not Modified",
        "Use Proxy",
        null,
        "Temporary Redirect"
      },
      new string[27]
      {
        "Bad Request",
        "Unauthorized",
        "Payment Required",
        "Forbidden",
        "Not Found",
        "Method Not Allowed",
        "Not Acceptable",
        "Proxy Authentication Required",
        "Request Timeout",
        "Conflict",
        "Gone",
        "Length Required",
        "Precondition Failed",
        "Request Entity Too Large",
        "Request-Uri Too Long",
        "Unsupported Media Type",
        "Requested Range Not Satisfiable",
        "Expectation Failed",
        null,
        null,
        null,
        null,
        "Unprocessable Entity",
        "Locked",
        "Failed Dependency",
        null,
        "Upgrade Required"
      },
      new string[8]
      {
        "Internal Server Error",
        "Not Implemented",
        "Bad Gateway",
        "Service Unavailable",
        "Gateway Timeout",
        "Http Version Not Supported",
        null,
        "Insufficient Storage"
      }
    };

    [FriendAccessAllowed]
    internal static string Get(HttpStatusCode code) => HttpStatusDescription.Get((int) code);

    internal static string Get(int code)
    {
      if (code >= 100 && code < 600)
      {
        int index1 = code / 100;
        int index2 = code % 100;
        if (index2 < HttpStatusDescription.httpStatusDescriptions[index1].Length)
          return HttpStatusDescription.httpStatusDescriptions[index1][index2];
      }
      return (string) null;
    }
  }
}
