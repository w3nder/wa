// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IKbdWatcher
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Guid(576357798, 14807, 18794, 136, 149, 103, 58, 194, 239, 237, 249)]
  [Version(100794368)]
  public interface IKbdWatcher
  {
    uint GetKeyboardLCID();

    bool GetSuggestionBoxEnabled();

    uint GetSuggestionLines();

    bool WaitOne();

    void Dispose();

    bool GetIsCancelled();
  }
}
