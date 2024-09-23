// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.SharpZipBaseException
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using ICSharpCode.SharpZipLib.Silverlight.Serialization;
using System;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight
{
  [Serializable]
  public class SharpZipBaseException : Exception
  {
    protected SharpZipBaseException(SerializationInfo info)
    {
    }

    public SharpZipBaseException()
    {
    }

    public SharpZipBaseException(string message)
      : base(message)
    {
    }

    public SharpZipBaseException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
