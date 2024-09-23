// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.CoreConstants
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

#nullable disable
namespace Microsoft.Graph
{
  public static class CoreConstants
  {
    public const int PollingIntervalInMs = 5000;

    public static class Headers
    {
      public const string Bearer = "Bearer";
      public const string SdkVersionHeaderName = "SdkVersion";
      public const string SdkVersionHeaderValueFormatString = "{0}-dotnet-{1}.{2}.{3}";
      public const string FormUrlEncodedContentType = "application/x-www-form-urlencoded";
      public const string ThrowSiteHeaderName = "X-ThrowSite";
    }

    public static class Serialization
    {
      public const string ODataType = "@odata.type";
    }
  }
}
