// Decompiled with JetBrains decompiler
// Type: WhatsApp.Curve22519Extensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public static class Curve22519Extensions
  {
    private static ICurve25519 instance;

    public static ICurve25519 Instance
    {
      get
      {
        return Curve22519Extensions.instance ?? (Curve22519Extensions.instance = (ICurve25519) NativeInterfaces.CreateInstance<Curve25519>());
      }
    }

    private static IByteBuffer WrapBuffer(byte[] b)
    {
      ByteBuffer instance = NativeInterfaces.CreateInstance<ByteBuffer>();
      instance.Put(b);
      return (IByteBuffer) instance;
    }

    private static void Release(params IByteBuffer[] args)
    {
      foreach (IByteBuffer byteBuffer in args)
        byteBuffer?.Reset();
    }

    public static void GenKeyPair(out byte[] pubKey, out byte[] privKey)
    {
      IByteBuffer PubKey = (IByteBuffer) null;
      IByteBuffer PrivKey = (IByteBuffer) null;
      try
      {
        Curve22519Extensions.Instance.GenKeyPair(out PubKey, out PrivKey);
        pubKey = PubKey.Get();
        privKey = PrivKey.Get();
      }
      finally
      {
        Curve22519Extensions.Release(PubKey, PrivKey);
      }
    }

    public static byte[] Derive(byte[] pubKey, byte[] privKey)
    {
      IByteBuffer PubKey = (IByteBuffer) null;
      IByteBuffer PrivKey = (IByteBuffer) null;
      IByteBuffer bb = (IByteBuffer) null;
      try
      {
        PubKey = Curve22519Extensions.WrapBuffer(pubKey);
        PrivKey = Curve22519Extensions.WrapBuffer(privKey);
        bb = Curve22519Extensions.Instance.Derive(PubKey, PrivKey);
        return bb.Get();
      }
      finally
      {
        Curve22519Extensions.Release(PubKey, PrivKey, bb);
      }
    }

    public static byte[] Sign(byte[] message, byte[] signKey)
    {
      IByteBuffer Message = (IByteBuffer) null;
      IByteBuffer SignKey = (IByteBuffer) null;
      IByteBuffer bb = (IByteBuffer) null;
      try
      {
        Message = Curve22519Extensions.WrapBuffer(message);
        SignKey = Curve22519Extensions.WrapBuffer(signKey);
        bb = Curve22519Extensions.Instance.Sign(Message, SignKey);
        return bb.Get();
      }
      finally
      {
        Curve22519Extensions.Release(Message, SignKey, bb);
      }
    }

    public static bool Verify(byte[] message, byte[] signature, byte[] signKey)
    {
      IByteBuffer Message = (IByteBuffer) null;
      IByteBuffer Signature = (IByteBuffer) null;
      IByteBuffer SignKey = (IByteBuffer) null;
      try
      {
        Message = Curve22519Extensions.WrapBuffer(message);
        Signature = Curve22519Extensions.WrapBuffer(signature);
        SignKey = Curve22519Extensions.WrapBuffer(signKey);
        return Curve22519Extensions.Instance.Verify(Message, Signature, SignKey);
      }
      finally
      {
        Curve22519Extensions.Release(Message, Signature, SignKey);
      }
    }
  }
}
