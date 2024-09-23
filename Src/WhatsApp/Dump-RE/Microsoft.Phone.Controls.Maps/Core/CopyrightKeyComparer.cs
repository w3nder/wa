// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.CopyrightKeyComparer
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal class CopyrightKeyComparer : IEqualityComparer<CopyrightKey>
  {
    public bool Equals(CopyrightKey first, CopyrightKey second)
    {
      return first.Culture == second.Culture && first.Style == second.Style;
    }

    public int GetHashCode(CopyrightKey key) => (int) (key.Culture.GetHashCode() + key.Style);
  }
}
