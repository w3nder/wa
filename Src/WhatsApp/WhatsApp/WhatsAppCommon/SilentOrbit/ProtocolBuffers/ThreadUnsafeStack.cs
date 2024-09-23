﻿// Decompiled with JetBrains decompiler
// Type: SilentOrbit.ProtocolBuffers.ThreadUnsafeStack
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.IO;


namespace SilentOrbit.ProtocolBuffers
{
  public class ThreadUnsafeStack : MemoryStreamStack, IDisposable
  {
    private Stack<MemoryStream> stack = new Stack<MemoryStream>();

    public MemoryStream Pop() => this.stack.Count == 0 ? new MemoryStream() : this.stack.Pop();

    public void Push(MemoryStream stream) => this.stack.Push(stream);

    public void Dispose() => this.stack.Clear();
  }
}
