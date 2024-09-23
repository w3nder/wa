// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.PlatformServices.GetMapUriCompletedEventArgs
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.PlatformServices
{
  [GeneratedCode("System.ServiceModel", "4.0.0.0")]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [DebuggerStepThrough]
  internal class GetMapUriCompletedEventArgs : AsyncCompletedEventArgs
  {
    private object[] results;

    public GetMapUriCompletedEventArgs(
      object[] results,
      Exception exception,
      bool cancelled,
      object userState)
      : base(exception, cancelled, userState)
    {
      this.results = results;
    }

    public MapUriResponse Result
    {
      get
      {
        this.RaiseExceptionIfNecessary();
        return (MapUriResponse) this.results[0];
      }
    }
  }
}
