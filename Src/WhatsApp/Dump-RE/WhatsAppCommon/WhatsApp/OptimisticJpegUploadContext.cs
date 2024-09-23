// Decompiled with JetBrains decompiler
// Type: WhatsApp.OptimisticJpegUploadContext
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;

#nullable disable
namespace WhatsApp
{
  public class OptimisticJpegUploadContext : UploadContext, IDisposable
  {
    public static readonly FunXMPP.FMessage.FunMediaType JpegFunXMPPMediaType = FunXMPP.FMessage.FunMediaType.Image;
    public AxolotlMediaCipher MediaCipher;
    public string PersonalRef;
    public FunXMPP.Connection.UploadResult uploadResult;

    public string OuId { get; private set; }

    public bool UploadedFlag { get; set; }

    public Stream JpegStream { get; set; }

    public long Size { get; private set; }

    public string LocalFileUri { get; set; }

    public byte[] Hash { get; set; }

    public byte[] MediaCipherHash { get; set; }

    public string MediaUploadUrl { get; set; }

    public Message Message { get; set; }

    public FunXMPP.FMessage.Type MediaWaType { get; private set; } = FunXMPP.FMessage.Type.Image;

    public string Extension { get; private set; } = "jpg";

    public string MediaMimeType { get; private set; } = "image/jpeg";

    public virtual bool IsMms4Upload { get; }

    public OptimisticJpegUploadContext(string ouId, Stream jpegStream)
      : base(UploadContext.UploadContextType.OptimisticUpload)
    {
      this.OuId = ouId;
      this.JpegStream = jpegStream;
      this.Size = jpegStream.Length;
    }

    public void Dispose()
    {
      this.JpegStream.SafeDispose();
      this.JpegStream = (Stream) null;
    }
  }
}
