// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.BZip2.BZip2Exception
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using ICSharpCode.SharpZipLib.Silverlight.Serialization;
using System;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.BZip2
{
  [Serializable]
  public class BZip2Exception : SharpZipBaseException
  {
    protected BZip2Exception(SerializationInfo info)
      : base(info)
    {
    }

    public BZip2Exception()
    {
    }

    public BZip2Exception(string message)
      : base(message)
    {
    }

    public BZip2Exception(string message, Exception exception)
      : base(message, exception)
    {
    }
  }
}
