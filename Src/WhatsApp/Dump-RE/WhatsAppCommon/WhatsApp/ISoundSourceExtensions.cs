// Decompiled with JetBrains decompiler
// Type: WhatsApp.ISoundSourceExtensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public static class ISoundSourceExtensions
  {
    public static void Initialize(this ISoundPort soundPort, ISoundSource src)
    {
      soundPort.Initialize(src, (ISampleSink) null, src.GetMetadata());
    }
  }
}
