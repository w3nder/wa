// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.WebServicesUtility
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.PlatformServices;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal static class WebServicesUtility
  {
    internal static ImageryServiceClient CreateImageryServiceClient(string imageryServiceAddress)
    {
      imageryServiceAddress = imageryServiceAddress.Replace("{UriScheme}", "HTTP");
      return new ImageryServiceClient((Binding) new CustomBinding(new BindingElement[2]
      {
        (BindingElement) new BinaryMessageEncodingBindingElement(),
        (BindingElement) new HttpTransportBindingElement()
      }), new EndpointAddress(imageryServiceAddress));
    }
  }
}
