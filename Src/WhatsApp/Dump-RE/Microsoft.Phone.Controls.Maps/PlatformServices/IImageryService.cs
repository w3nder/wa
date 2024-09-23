// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.PlatformServices.IImageryService
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.CodeDom.Compiler;
using System.ServiceModel;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.PlatformServices
{
  [GeneratedCode("System.ServiceModel", "4.0.0.0")]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [ServiceContract(Namespace = "http://dev.virtualearth.net/webservices/v1/imagery/contracts", ConfigurationName = "PlatformServices.IImageryService")]
  internal interface IImageryService
  {
    [OperationContract(AsyncPattern = true, Action = "http://dev.virtualearth.net/webservices/v1/imagery/contracts/IImageryService/GetImageryMetadata", ReplyAction = "http://dev.virtualearth.net/webservices/v1/imagery/contracts/IImageryService/GetImageryMetadataResponse")]
    [FaultContract(typeof (ResponseSummary), Action = "http://dev.virtualearth.net/webservices/v1/imagery/contracts/IImageryService/GetImageryMetadataResponseSummaryFault", Name = "ResponseSummary", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
    IAsyncResult BeginGetImageryMetadata(
      ImageryMetadataRequest request,
      AsyncCallback callback,
      object asyncState);

    ImageryMetadataResponse EndGetImageryMetadata(IAsyncResult result);

    [FaultContract(typeof (ResponseSummary), Action = "http://dev.virtualearth.net/webservices/v1/imagery/contracts/IImageryService/GetMapUriResponseSummaryFault", Name = "ResponseSummary", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
    [OperationContract(AsyncPattern = true, Action = "http://dev.virtualearth.net/webservices/v1/imagery/contracts/IImageryService/GetMapUri", ReplyAction = "http://dev.virtualearth.net/webservices/v1/imagery/contracts/IImageryService/GetMapUriResponse")]
    IAsyncResult BeginGetMapUri(MapUriRequest request, AsyncCallback callback, object asyncState);

    MapUriResponse EndGetMapUri(IAsyncResult result);
  }
}
