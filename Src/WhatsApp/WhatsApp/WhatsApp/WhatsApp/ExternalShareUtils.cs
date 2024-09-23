// Decompiled with JetBrains decompiler
// Type: WhatsApp.ExternalShareUtils
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.IO;
using System.Runtime.InteropServices;


namespace WhatsApp
{
  public class ExternalShareUtils
  {
    public static bool IsAnimatedGif(Stream input)
    {
      byte[] buffer = new byte[3];
      input.Position = 0L;
      input.Read(buffer, 0, buffer.Length);
      input.Position = 0L;
      if (buffer[0] == (byte) 71 && buffer[1] == (byte) 73)
      {
        if (buffer[2] == (byte) 70)
        {
          try
          {
            input.Position = 0L;
            Marshal.ReleaseComObject((object) NativeInterfaces.MediaMisc.OpenVideo(input.ToWaStream(), true));
            return true;
          }
          catch (Exception ex)
          {
          }
        }
      }
      return false;
    }

    public static bool IsPreviewEnabled() => true;
  }
}
