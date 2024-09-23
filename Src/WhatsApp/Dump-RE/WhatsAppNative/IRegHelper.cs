// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IRegHelper
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(170111887, 174, 17730, 129, 83, 164, 229, 223, 93, 140, 100)]
  public interface IRegHelper
  {
    string ReadString([In] uint Key, [In] string Subkey, [In] string Value);

    uint ReadDWord([In] uint Key, [In] string Subkey, [In] string Value);

    IAction NotifyChangedImpl([In] uint Key, [In] string Subkey, [In] IAction Callback);

    void WriteDWord([In] uint Key, [In] string Subkey, [In] string Value, [In] uint Word);
  }
}
