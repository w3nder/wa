// Decompiled with JetBrains decompiler
// Type: WhatsApp.MediaUploadException
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;

#nullable disable
namespace WhatsApp
{
  public class MediaUploadException : Exception
  {
    public int ResponseCode { get; private set; }

    public MediaUploadException(string message = null, Exception innerException = null, int statusCode = 0)
      : base(message, innerException)
    {
      this.ResponseCode = statusCode;
    }
  }
}
