// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaUris
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;


namespace WhatsApp
{
  public class WaUris
  {
    public static string HomePageStr(bool clearStack = true)
    {
      return !clearStack ? "/PageSelect" : "/PageSelect?ClearStack=true";
    }

    public static Uri HomePage(bool clearStack = true)
    {
      return new Uri(WaUris.HomePageStr(clearStack), UriKind.Relative);
    }

    public static string ChatPageStr(WaUriParams uriParams = null)
    {
      return UriUtils.CreatePageUriStr("ChatPage", uriParams);
    }

    public static Uri ChatPage(WaUriParams uriParams = null)
    {
      return UriUtils.CreatePageUri(nameof (ChatPage), uriParams);
    }
  }
}
