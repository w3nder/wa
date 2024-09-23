// Decompiled with JetBrains decompiler
// Type: WhatsApp.StreamingUploadContextMms4
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;


namespace WhatsApp
{
  public class StreamingUploadContextMms4 : StreamingUploadContext
  {
    private MediaUploadMms4.Mms4UploadResult res;

    public new FunXMPP.Connection.UploadResult UploadResult
    {
      get
      {
        throw new NotImplementedException("should not get FunXMPP.Connection.UploadResult for Mms4 upload");
      }
      set
      {
        throw new NotImplementedException("should not set FunXMPP.Connection.UploadResult for Mms4 upload");
      }
    }

    public MediaUploadMms4.Mms4UploadResult Mms4UploadResult
    {
      get => this.res;
      set
      {
        lock (this.@lock)
        {
          this.res = value;
          this.CheckComplete();
        }
      }
    }

    public override void CheckComplete()
    {
      if (this.res == null || this.msg == null)
        return;
      MediaUploadMms4.ProcessUploadResponse(this.msg, this.MediaCipher, this.res, (WhatsApp.Events.MediaUpload) null);
      this.active = false;
    }
  }
}
