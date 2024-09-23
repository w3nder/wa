// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.TfeCoverageMapState
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal sealed class TfeCoverageMapState
  {
    internal TfeCoverageMapState(TfeCoverageMapCallback callback, object userState)
    {
      this.Callback = callback;
      this.UserState = userState;
    }

    internal TfeCoverageMapCallback Callback { get; private set; }

    internal object UserState { get; private set; }
  }
}
