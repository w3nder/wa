// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IWASocket
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Guid(4254055909, 30299, 16519, 179, 57, 96, 132, 60, 208, 149, 175)]
  [Version(100794368)]
  public interface IWASocket
  {
    void SetHost([In] IHost Host);

    void SetPort([In] ushort Port);

    void SetHandler([In] IWASocketHandler Handler);

    void SetTimeoutMilliseconds([In] int Milliseconds, [In] bool Cumulative);

    void Connect([In] IWAScheduler sched);

    void Send([In] IByteBuffer Buffer);

    void Dispose();

    void Close();
  }
}
