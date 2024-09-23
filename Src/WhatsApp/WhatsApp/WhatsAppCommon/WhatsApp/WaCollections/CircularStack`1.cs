// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaCollections.CircularStack`1
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;


namespace WhatsApp.WaCollections
{
  public class CircularStack<T>
  {
    private T[] list;
    private int capacity;
    private int writePos;
    private int count;

    public int Count => this.count;

    public CircularStack(int capacity)
    {
      this.capacity = capacity > 0 ? capacity : throw new ArgumentException();
      this.list = new T[capacity];
    }

    public T PeekAtOrDefault(int i = 0)
    {
      return i >= 0 && i < this.count ? this.list[(this.writePos - 1 - i + this.capacity) % this.capacity] : default (T);
    }

    public T PopOrDefault()
    {
      if (this.count <= 0)
        return default (T);
      this.writePos = (this.writePos - 1 + this.capacity) % this.capacity;
      --this.count;
      return this.list[this.writePos];
    }

    public void Push(T t)
    {
      this.list[this.writePos++] = t;
      if (this.writePos >= this.capacity)
        this.writePos = 0;
      ++this.count;
      if (this.count <= this.capacity)
        return;
      this.count = this.capacity;
    }

    public void Clear() => this.count = this.writePos = 0;
  }
}
