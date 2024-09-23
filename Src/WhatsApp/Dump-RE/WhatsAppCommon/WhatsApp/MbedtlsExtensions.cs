// Decompiled with JetBrains decompiler
// Type: WhatsApp.MbedtlsExtensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public static class MbedtlsExtensions
  {
    private static IMbedtls instance;

    public static IMbedtls Instance
    {
      get
      {
        return MbedtlsExtensions.instance ?? (MbedtlsExtensions.instance = (IMbedtls) NativeInterfaces.CreateInstance<Mbedtls>());
      }
    }

    public static byte[] AesGcmEncrypt(
      byte[] cipherKey,
      byte[] iv,
      byte[] add,
      byte[] plainText,
      int? offset = null,
      int? length = null)
    {
      IByteBuffer instance1 = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
      IByteBuffer instance2 = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
      IByteBuffer byteBuffer = (IByteBuffer) null;
      IByteBuffer instance3 = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
      instance1.Put(cipherKey);
      instance2.Put(iv);
      if (offset.HasValue && length.HasValue)
        instance3.Put(plainText, offset.Value, length.Value);
      else
        instance3.Put(plainText);
      if (add != null)
      {
        byteBuffer = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
        byteBuffer.Put(add);
      }
      return MbedtlsExtensions.Instance.AesGcmEncrypt(instance1, instance2, byteBuffer, instance3).Get();
    }

    public static byte[] AesGcmDecrypt(
      byte[] cipherKey,
      byte[] iv,
      byte[] add,
      byte[] cipherText,
      int? offset = null,
      int? length = null)
    {
      if (cipherText.Length < 16)
        throw new ArgumentOutOfRangeException();
      IByteBuffer instance1 = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
      IByteBuffer instance2 = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
      IByteBuffer byteBuffer = (IByteBuffer) null;
      IByteBuffer instance3 = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
      instance1.Put(cipherKey);
      instance2.Put(iv);
      if (offset.HasValue && length.HasValue)
        instance3.Put(cipherText, offset.Value, length.Value);
      else
        instance3.Put(cipherText);
      if (add != null)
      {
        byteBuffer = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
        byteBuffer.Put(add);
      }
      return MbedtlsExtensions.Instance.AesGcmDecrypt(instance1, instance2, byteBuffer, instance3).Get();
    }
  }
}
