// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Core.PathFilter
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System.IO;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Core
{
  public class PathFilter : IScanFilter
  {
    private readonly NameFilter nameFilter_;

    public PathFilter(string filter) => this.nameFilter_ = new NameFilter(filter);

    public virtual bool IsMatch(string name)
    {
      bool flag = false;
      if (name != null)
        flag = this.nameFilter_.IsMatch(name.Length > 0 ? Path.GetFullPath(name) : "");
      return flag;
    }
  }
}
