// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaCollections.CircularBuffer
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;


namespace WhatsApp.WaCollections
{
  public class CircularBuffer
  {
    private int capacity;
    private byte[] buf;
    private int nextWritePos;

    public CircularBuffer(int capacity)
    {
      this.buf = capacity > 0 ? new byte[this.capacity = capacity] : throw new ArgumentException("capacity has to be larger than 0");
    }

    public void Add(byte b)
    {
      this.buf[this.nextWritePos++] = b;
      if (this.nextWritePos < this.capacity)
        return;
      this.nextWritePos %= this.capacity;
    }

    public bool EndMatches(byte[] bytes)
    {
      int num = this.nextWritePos - 1;
      for (int index1 = 0; index1 < bytes.Length; ++index1)
      {
        int index2 = num - index1;
        if (index2 < 0)
          index2 += this.capacity;
        if ((int) this.buf[index2] != (int) bytes[bytes.Length - 1 - index1])
          return false;
      }
      return true;
    }
  }
}
