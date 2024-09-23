// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.ConfigService.DeviceType
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System.CodeDom.Compiler;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.ConfigService
{
  [DataContract(Name = "DeviceType", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  internal enum DeviceType
  {
    [EnumMember] Desktop,
    [EnumMember] Mobile,
  }
}
