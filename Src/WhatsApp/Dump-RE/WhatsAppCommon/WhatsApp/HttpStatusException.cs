// Decompiled with JetBrains decompiler
// Type: WhatsApp.HttpStatusException
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Net;

#nullable disable
namespace WhatsApp
{
  public class HttpStatusException : Exception
  {
    public HttpStatusCode StatusCode;

    public HttpStatusException(HttpStatusCode code, string msg = null)
      : base(msg ?? string.Format("Unexpected status code {0}", (object) code))
    {
      this.StatusCode = code;
    }
  }
}
