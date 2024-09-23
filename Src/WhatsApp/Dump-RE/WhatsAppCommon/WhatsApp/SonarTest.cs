// Decompiled with JetBrains decompiler
// Type: WhatsApp.SonarTest
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public static class SonarTest
  {
    private static SonarTest.Sonar currentTest;

    public static void runTest(string urlToTest)
    {
      if (string.IsNullOrEmpty(urlToTest) || !urlToTest.StartsWith("https"))
        Log.l("sonar", "ignoring sonar request - invalid string: {0}", (object) urlToTest);
      else if (!new Uri(urlToTest).Host.EndsWith(".whatsapp.net"))
        Log.l("sonar", "ignoring sonar request - invalid url: {0}", (object) urlToTest);
      else if (SonarTest.currentTest != null)
      {
        Log.l("sonar", "ignoring sonar request - already running test");
      }
      else
      {
        SonarTest.currentTest = new SonarTest.Sonar(urlToTest);
        WAThreadPool.QueueUserWorkItem((Action) (() => SonarTest.currentTest.testUrl()));
      }
    }

    public static void completedTest() => SonarTest.currentTest = (SonarTest.Sonar) null;

    public class Sonar
    {
      private string url;

      public Sonar(string url) => this.url = url;

      public void testUrl()
      {
        Log.l("sonar", "starting test");
        NativeWeb.Callback callbackObject = new NativeWeb.Callback()
        {
          OnBeginResponse = (Action<int, string>) ((code, headerStrings) => Log.l("sonar", "response {0}", (object) code)),
          OnBytesIn = (Action<byte[]>) (buf => Log.l("sonar", "data read {0}", (object) buf.Length)),
          OnEndResponse = (Action) (() => Log.l("sonar", "end of response"))
        };
        NativeWeb.Create<Unit>((Action<IWebRequest, IObserver<Unit>>) ((req, observer) =>
        {
          try
          {
            req.Open(this.url, (IWebCallback) callbackObject);
            observer.OnNext(new Unit());
            observer.OnCompleted();
          }
          catch (Exception ex)
          {
            req.Cancel();
            observer.OnError(ex);
          }
          finally
          {
            Log.l("sonar", "completed test");
            SonarTest.completedTest();
          }
        })).Subscribe<Unit>((Action<Unit>) (u => Log.l("sonar", "OnNext")), (Action<Exception>) (ex => Log.LogException(ex, "OnError in sonar")), (Action) (() => Log.l("sonar", "OnCompleted")));
      }
    }
  }
}
