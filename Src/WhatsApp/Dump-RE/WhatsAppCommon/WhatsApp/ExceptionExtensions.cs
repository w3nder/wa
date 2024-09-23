// Decompiled with JetBrains decompiler
// Type: WhatsApp.ExceptionExtensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;

#nullable disable
namespace WhatsApp
{
  public static class ExceptionExtensions
  {
    private const string HResultPrefix = "Exception from HRESULT: ";

    private static void GetInnerExceptions(List<Exception> list, Exception e)
    {
      for (; e != null; e = e.InnerException)
      {
        list.Add(e);
        if (e is AggregateException aggregateException)
        {
          foreach (Exception innerException in aggregateException.InnerExceptions)
            ExceptionExtensions.GetInnerExceptions(list, innerException);
        }
      }
    }

    public static IEnumerable<Exception> GetInnerExceptions(this Exception e)
    {
      List<Exception> list = new List<Exception>();
      ExceptionExtensions.GetInnerExceptions(list, e);
      return (IEnumerable<Exception>) list;
    }

    public static string GetFriendlyMessage(this Exception e)
    {
      string friendlyMessage = e.Message;
      if (friendlyMessage == null)
        return (string) null;
      if (friendlyMessage.StartsWith("Exception from HRESULT: ", StringComparison.Ordinal))
      {
        try
        {
          string winRtString = NativeInterfaces.Misc.ComErrorToWinRtString(e);
          if (!string.IsNullOrEmpty(winRtString))
            friendlyMessage = winRtString;
        }
        catch (Exception ex)
        {
        }
      }
      return friendlyMessage;
    }

    public static void GetSynopsis(this Exception e, StringBuilder str)
    {
      str.Append(e.GetType().Name);
      int? nullable = new int?();
      if (e is ExternalException externalException)
        nullable = new int?(externalException.ErrorCode);
      else if (e.Message != null && e.Message.StartsWith("Exception from HRESULT: ", StringComparison.Ordinal))
        nullable = new int?(e.HResult);
      if (nullable.HasValue)
      {
        str.Append(": [");
        str.Append(nullable.Value.ToString("x"));
        str.Append("]");
      }
      if (string.IsNullOrEmpty(e.Message))
        return;
      str.Append(": ");
      str.Append(e.GetFriendlyMessage());
    }

    public static string GetSynopsis(this Exception ex)
    {
      StringBuilder str = new StringBuilder();
      ex.GetSynopsis(str);
      return str.ToString();
    }

    public static uint GetHResult(this Exception ex) => (uint) ex.HResult;

    public static IEnumerable<uint> GetHResults(this Exception e)
    {
      return e.GetInnerExceptions().Select<Exception, uint>((Func<Exception, uint>) (ex => ex.GetHResult()));
    }

    public static Action GetRethrowAction(this Exception e)
    {
      return new Action(ExceptionDispatchInfo.Capture(e).Throw);
    }
  }
}
