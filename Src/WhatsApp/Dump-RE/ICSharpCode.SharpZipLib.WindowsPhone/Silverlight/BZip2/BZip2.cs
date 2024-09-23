// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.BZip2.BZip2
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System;
using System.IO;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.BZip2
{
  public static class BZip2
  {
    public static void Decompress(Stream inStream, Stream outStream)
    {
      if (inStream == null)
        throw new ArgumentNullException(nameof (inStream));
      if (outStream == null)
        throw new ArgumentNullException(nameof (outStream));
      using (outStream)
      {
        using (BZip2InputStream bzip2InputStream = new BZip2InputStream(inStream))
        {
          for (int index = bzip2InputStream.ReadByte(); index != -1; index = bzip2InputStream.ReadByte())
            outStream.WriteByte((byte) index);
        }
      }
    }

    public static void Compress(Stream inStream, Stream outStream, int blockSize)
    {
      if (inStream == null)
        throw new ArgumentNullException(nameof (inStream));
      if (outStream == null)
        throw new ArgumentNullException(nameof (outStream));
      using (inStream)
      {
        using (BZip2OutputStream bzip2OutputStream = new BZip2OutputStream(outStream, blockSize))
        {
          for (int index = inStream.ReadByte(); index != -1; index = inStream.ReadByte())
            bzip2OutputStream.WriteByte((byte) index);
        }
      }
    }
  }
}
