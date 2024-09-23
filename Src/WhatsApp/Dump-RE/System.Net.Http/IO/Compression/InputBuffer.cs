// Decompiled with JetBrains decompiler
// Type: System.IO.Compression.InputBuffer
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

#nullable disable
namespace System.IO.Compression
{
  internal class InputBuffer
  {
    private byte[] buffer;
    private int start;
    private int end;
    private uint bitBuffer;
    private int bitsInBuffer;

    public int AvailableBits => this.bitsInBuffer;

    public int AvailableBytes => this.end - this.start + this.bitsInBuffer / 8;

    public bool EnsureBitsAvailable(int count)
    {
      if (this.bitsInBuffer < count)
      {
        if (this.NeedsInput())
          return false;
        this.bitBuffer |= (uint) this.buffer[this.start++] << this.bitsInBuffer;
        this.bitsInBuffer += 8;
        if (this.bitsInBuffer < count)
        {
          if (this.NeedsInput())
            return false;
          this.bitBuffer |= (uint) this.buffer[this.start++] << this.bitsInBuffer;
          this.bitsInBuffer += 8;
        }
      }
      return true;
    }

    public uint TryLoad16Bits()
    {
      if (this.bitsInBuffer < 8)
      {
        if (this.start < this.end)
        {
          this.bitBuffer |= (uint) this.buffer[this.start++] << this.bitsInBuffer;
          this.bitsInBuffer += 8;
        }
        if (this.start < this.end)
        {
          this.bitBuffer |= (uint) this.buffer[this.start++] << this.bitsInBuffer;
          this.bitsInBuffer += 8;
        }
      }
      else if (this.bitsInBuffer < 16 && this.start < this.end)
      {
        this.bitBuffer |= (uint) this.buffer[this.start++] << this.bitsInBuffer;
        this.bitsInBuffer += 8;
      }
      return this.bitBuffer;
    }

    private uint GetBitMask(int count) => (uint) ((1 << count) - 1);

    public int GetBits(int count)
    {
      if (!this.EnsureBitsAvailable(count))
        return -1;
      int bits = (int) this.bitBuffer & (int) this.GetBitMask(count);
      this.bitBuffer >>= count;
      this.bitsInBuffer -= count;
      return bits;
    }

    public int CopyTo(byte[] output, int offset, int length)
    {
      int num1 = 0;
      while (this.bitsInBuffer > 0 && length > 0)
      {
        output[offset++] = (byte) this.bitBuffer;
        this.bitBuffer >>= 8;
        this.bitsInBuffer -= 8;
        --length;
        ++num1;
      }
      if (length == 0)
        return num1;
      int num2 = this.end - this.start;
      if (length > num2)
        length = num2;
      Array.Copy((Array) this.buffer, this.start, (Array) output, offset, length);
      this.start += length;
      return num1 + length;
    }

    public bool NeedsInput() => this.start == this.end;

    public void SetInput(byte[] buffer, int offset, int length)
    {
      this.buffer = buffer;
      this.start = offset;
      this.end = offset + length;
    }

    public void SkipBits(int n)
    {
      this.bitBuffer >>= n;
      this.bitsInBuffer -= n;
    }

    public void SkipToByteBoundary()
    {
      this.bitBuffer >>= this.bitsInBuffer % 8;
      this.bitsInBuffer -= this.bitsInBuffer % 8;
    }
  }
}
