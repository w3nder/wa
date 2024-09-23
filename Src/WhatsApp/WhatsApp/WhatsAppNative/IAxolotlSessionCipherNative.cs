﻿// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IAxolotlSessionCipherNative
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(2150201713, 58780, 17703, 178, 131, 183, 153, 214, 77, 34, 133)]
  public interface IAxolotlSessionCipherNative
  {
    void Initialize(
      [In] string RecipientId,
      [In] IAxolotlSessionCipherCallbacks ManagedCallbacks,
      [In] IAxolotlNative AxolotlNative);

    void EncryptMessage([In] IByteBuffer PlainText, out int CipherTextType, out IByteBuffer CipherText);

    void DecryptPreKeyMessage([In] IByteBuffer CipherText);

    void DecryptMessage([In] IByteBuffer CipherText);
  }
}