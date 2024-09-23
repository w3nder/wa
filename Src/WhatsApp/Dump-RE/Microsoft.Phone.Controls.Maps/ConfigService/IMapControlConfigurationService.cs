// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.ConfigService.IMapControlConfigurationService
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.CodeDom.Compiler;
using System.ServiceModel;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.ConfigService
{
  [ServiceContract(Namespace = "http://dev.virtualearth.net/webservices/v1/mapcontrolconfiguration/contracts", ConfigurationName = "ConfigService.IMapControlConfigurationService")]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [GeneratedCode("System.ServiceModel", "4.0.0.0")]
  internal interface IMapControlConfigurationService
  {
    [OperationContract(AsyncPattern = true, Action = "http://dev.virtualearth.net/webservices/v1/mapcontrolconfiguration/contracts/IMapControlConfigurationService/GetConfiguration", ReplyAction = "http://dev.virtualearth.net/webservices/v1/mapcontrolconfiguration/contracts/IMapControlConfigurationService/GetConfigurationResponse")]
    [FaultContract(typeof (ResponseSummary), Action = "http://dev.virtualearth.net/webservices/v1/mapcontrolconfiguration/contracts/IMapControlConfigurationService/GetConfigurationResponseSummaryFault", Name = "ResponseSummary", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
    IAsyncResult BeginGetConfiguration(
      MapControlConfigurationRequest request,
      AsyncCallback callback,
      object asyncState);

    MapControlConfigurationResponse EndGetConfiguration(IAsyncResult result);
  }
}
