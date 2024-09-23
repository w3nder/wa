﻿// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Core.StreamUtils
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System;
using System.IO;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Core
{
  public static class StreamUtils
  {
    public static void ReadFully(Stream stream, byte[] buffer)
    {
      StreamUtils.ReadFully(stream, buffer, 0, buffer.Length);
    }

    public static void ReadFully(Stream stream, byte[] buffer, int offset, int count)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      if (buffer == null)
        throw new ArgumentNullException(nameof (buffer));
      if (offset < 0 || offset > buffer.Length)
        throw new ArgumentOutOfRangeException(nameof (offset));
      if (count < 0 || offset + count > buffer.Length)
        throw new ArgumentOutOfRangeException(nameof (count));
      int num;
      for (; count > 0; count -= num)
      {
        num = stream.Read(buffer, offset, count);
        if (num <= 0)
          throw new EndOfStreamException();
        offset += num;
      }
    }

    public static void Copy(
      Stream source,
      Stream destination,
      byte[] buffer,
      ProgressHandler progressHandler,
      TimeSpan updateInterval,
      object sender,
      string name)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (destination == null)
        throw new ArgumentNullException(nameof (destination));
      if (buffer == null)
        throw new ArgumentNullException(nameof (buffer));
      if (buffer.Length < 128)
        throw new ArgumentException("Buffer is too small", nameof (buffer));
      if (progressHandler == null)
        throw new ArgumentNullException(nameof (progressHandler));
      bool flag1 = true;
      DateTime now = DateTime.Now;
      long processed = 0;
      long target = 0;
      if (source.CanSeek)
        target = source.Length - source.Position;
      ProgressEventArgs e1 = new ProgressEventArgs(name, processed, target);
      progressHandler(sender, e1);
      bool flag2 = false;
      while (flag1)
      {
        int count = source.Read(buffer, 0, buffer.Length);
        if (count > 0)
        {
          processed += (long) count;
          destination.Write(buffer, 0, count);
        }
        else
        {
          destination.Flush();
          flag1 = false;
        }
        if (DateTime.Now - now > updateInterval)
        {
          flag2 = processed == target;
          now = DateTime.Now;
          ProgressEventArgs e2 = new ProgressEventArgs(name, processed, target);
          progressHandler(sender, e2);
          flag1 = e2.ContinueRunning;
        }
      }
      if (flag2)
        return;
      ProgressEventArgs e3 = new ProgressEventArgs(name, processed, target);
      progressHandler(sender, e3);
    }

    public static void Copy(Stream source, Stream destination, byte[] buffer)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (destination == null)
        throw new ArgumentNullException(nameof (destination));
      if (buffer == null)
        throw new ArgumentNullException(nameof (buffer));
      if (buffer.Length < 128)
        throw new ArgumentException("Buffer is too small", nameof (buffer));
      bool flag = true;
      while (flag)
      {
        int count = source.Read(buffer, 0, buffer.Length);
        if (count > 0)
        {
          destination.Write(buffer, 0, count);
        }
        else
        {
          destination.Flush();
          flag = false;
        }
      }
    }
  }
}
