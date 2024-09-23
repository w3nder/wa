// Decompiled with JetBrains decompiler
// Type: WhatsApp.OptimisticJpegUploadContextMms4
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;


namespace WhatsApp
{
  public class OptimisticJpegUploadContextMms4 : OptimisticJpegUploadContext
  {
    public MediaUploadMms4.Mms4UploadResult uploadResultMms4;

    public FunXMPP.Connection.UploadResult uploadResult
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

    public override bool IsMms4Upload { get; } = true;

    public OptimisticJpegUploadContextMms4(string ouId, Stream jpegStream)
      : base(ouId, jpegStream)
    {
    }
  }
}
