﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.BadFetchState
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Device.Location;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal class BadFetchState
  {
    internal BadFetchState(DateTime againAt, Credentials credentials, GeoCoordinate location)
    {
      this.TryAgainAt = againAt;
      this.CredentialsLastUsed = credentials;
      this.Location = location;
    }

    internal DateTime TryAgainAt { get; private set; }

    internal Credentials CredentialsLastUsed { get; private set; }

    internal GeoCoordinate Location { get; private set; }
  }
}