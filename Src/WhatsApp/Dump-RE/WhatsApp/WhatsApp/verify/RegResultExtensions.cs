// Decompiled with JetBrains decompiler
// Type: WhatsApp.verify.RegResultExtensions
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Tasks;
using System;
using System.Windows;

#nullable disable
namespace WhatsApp.verify
{
  public static class RegResultExtensions
  {
    public static bool HasSupportAction(this Registration.RegResult res)
    {
      return res.ContactSupport || res.SupportUrl != null;
    }

    public static void PerformSupportAction(this Registration.RegResult res)
    {
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        if (res.ContactSupport)
        {
          string val = string.Format("{0} {1}", (object) res.Reason, res.Login != null ? (object) res.Login : (object) (Settings.CountryCode + Settings.PhoneNumber));
          WaUriParams uriParams = new WaUriParams();
          uriParams.AddString("context", val);
          uriParams.AddBool("ClearStack", true);
          NavUtils.NavigateToPage("ContactSupportPage", uriParams);
        }
        else
        {
          if (res.SupportUrl == null)
            return;
          new WebBrowserTask()
          {
            Uri = new Uri(res.SupportUrl)
          }.Show();
        }
      }));
    }
  }
}
