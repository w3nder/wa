// Decompiled with JetBrains decompiler
// Type: WhatsApp.IWASocketExtensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using WhatsAppNative;


namespace WhatsApp
{
  public static class IWASocketExtensions
  {
    public static void Send(this IWASocket socket, byte[] bytes)
    {
      IByteBuffer instance = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
      instance.Put(bytes);
      socket.Send(instance);
      instance.Reset();
    }

    public static void Send(this IWASocket socket, byte[] bytes, int off, int len)
    {
      IByteBuffer instance = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
      instance.Put(bytes, off, len);
      socket.Send(instance);
      instance.Reset();
    }
  }
}
