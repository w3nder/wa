// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Compat.Extensions
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System;
using System.Globalization;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Compat
{
  public static class Extensions
  {
    public static int Compare(this string left, string right, bool ignoreCase, CultureInfo info)
    {
      if (info == null)
        throw new ArgumentNullException("CultureInfo cannot be null!");
      return !ignoreCase ? info.CompareInfo.Compare(left, right, CompareOptions.None) : info.CompareInfo.Compare(left, right, CompareOptions.IgnoreCase);
    }
  }
}
