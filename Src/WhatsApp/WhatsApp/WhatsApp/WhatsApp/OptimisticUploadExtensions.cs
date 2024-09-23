// Decompiled with JetBrains decompiler
// Type: WhatsApp.OptimisticUploadExtensions
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll


namespace WhatsApp
{
  public static class OptimisticUploadExtensions
  {
    public static string GetOptimisticUniqueId(this MediaSharingState.PicInfo picInfo)
    {
      string optimisticUpload = picInfo.GetPathForOptimisticUpload();
      if (string.IsNullOrEmpty(optimisticUpload))
        optimisticUpload = picInfo.GetHashCode().ToString();
      return optimisticUpload;
    }

    public static string GetPathForOptimisticUpload(this MediaSharingState.PicInfo picInfo)
    {
      string optimisticUpload = picInfo.PathForDb;
      if (string.IsNullOrEmpty(optimisticUpload) && picInfo is MediaPickerState.ChosenPicInfo chosenPicInfo)
        optimisticUpload = chosenPicInfo.PathForOpen;
      return optimisticUpload;
    }
  }
}
