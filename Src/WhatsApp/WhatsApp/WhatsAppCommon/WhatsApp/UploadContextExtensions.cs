// Decompiled with JetBrains decompiler
// Type: WhatsApp.UploadContextExtensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp
{
  public static class UploadContextExtensions
  {
    public static bool isType(
      this UploadContext uploadContext,
      UploadContext.UploadContextType contextType)
    {
      return uploadContext != null && uploadContext.ContextType == contextType;
    }

    public static bool isActiveStreamingUpload(this UploadContext uploadContext)
    {
      return uploadContext.isType(UploadContext.UploadContextType.Streaming) && ((StreamingUploadContext) uploadContext).Active;
    }

    public static bool isStreamingUpload(this UploadContext uploadContext)
    {
      return uploadContext.isType(UploadContext.UploadContextType.Streaming);
    }

    public static bool isOptimisticUpload(this UploadContext uploadContext)
    {
      return uploadContext.isType(UploadContext.UploadContextType.OptimisticUpload) && !((OptimisticJpegUploadContext) uploadContext).UploadedFlag;
    }

    public static bool wasOptimisticallyUploaded(this UploadContext uploadContext)
    {
      return uploadContext.isType(UploadContext.UploadContextType.OptimisticUpload) && ((OptimisticJpegUploadContext) uploadContext).UploadedFlag;
    }
  }
}
