// Decompiled with JetBrains decompiler
// Type: SilentOrbit.ProtocolBuffers.ThreadSafeStack
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace SilentOrbit.ProtocolBuffers
{
  public class ThreadSafeStack : MemoryStreamStack, IDisposable
  {
    private Stack<MemoryStream> stack = new Stack<MemoryStream>();

    public MemoryStream Pop()
    {
      lock (this.stack)
        return this.stack.Count == 0 ? new MemoryStream() : this.stack.Pop();
    }

    public void Push(MemoryStream stream)
    {
      lock (this.stack)
        this.stack.Push(stream);
    }

    public void Dispose()
    {
      lock (this.stack)
        this.stack.Clear();
    }
  }
}
