// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.Constants
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public static class Constants
  {
    public const int PollingIntervalInMs = 5000;

    public static class Headers
    {
      public const string SdkVersionHeaderPrefix = "onedrive";
    }

    public static class Url
    {
      public const string Drive = "drive";
      public const string Root = "root";
      public const string AppRoot = "approot";
      public const string Documents = "documents";
      public const string Photos = "photos";
      public const string CameraRoll = "cameraroll";
      public const string Music = "music";
    }
  }
}
