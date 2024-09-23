// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IWAWebAccountProvider
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(3293459058, 24607, 16924, 160, 95, 36, 131, 6, 254, 189, 116)]
  public interface IWAWebAccountProvider
  {
    string Id { get; }

    string DisplayName { get; }

    string DisplayPurpose { get; }

    string Authority { get; }

    uint GetWrappedObject();
  }
}
