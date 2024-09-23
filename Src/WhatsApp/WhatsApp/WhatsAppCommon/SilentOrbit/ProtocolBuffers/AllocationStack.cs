// Decompiled with JetBrains decompiler
// Type: SilentOrbit.ProtocolBuffers.AllocationStack
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;


namespace SilentOrbit.ProtocolBuffers
{
  public class AllocationStack : MemoryStreamStack, IDisposable
  {
    public MemoryStream Pop() => new MemoryStream();

    public void Push(MemoryStream stream)
    {
    }

    public void Dispose()
    {
    }
  }
}
