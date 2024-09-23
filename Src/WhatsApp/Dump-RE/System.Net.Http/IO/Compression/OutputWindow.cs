// Decompiled with JetBrains decompiler
// Type: System.IO.Compression.OutputWindow
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

#nullable disable
namespace System.IO.Compression
{
  internal class OutputWindow
  {
    private const int WindowSize = 32768;
    private const int WindowMask = 32767;
    private byte[] window = new byte[32768];
    private int end;
    private int bytesUsed;

    public void Write(byte b)
    {
      this.window[this.end++] = b;
      this.end &= (int) short.MaxValue;
      ++this.bytesUsed;
    }

    public void WriteLengthDistance(int length, int distance)
    {
      this.bytesUsed += length;
      int sourceIndex = this.end - distance & (int) short.MaxValue;
      int num1 = 32768 - length;
      if (sourceIndex <= num1 && this.end < num1)
      {
        if (length <= distance)
        {
          Array.Copy((Array) this.window, sourceIndex, (Array) this.window, this.end, length);
          this.end += length;
        }
        else
        {
          while (length-- > 0)
            this.window[this.end++] = this.window[sourceIndex++];
        }
      }
      else
      {
        while (length-- > 0)
        {
          byte[] window1 = this.window;
          int index1 = this.end++;
          byte[] window2 = this.window;
          int index2 = sourceIndex;
          int num2 = index2 + 1;
          int num3 = (int) window2[index2];
          window1[index1] = (byte) num3;
          this.end &= (int) short.MaxValue;
          sourceIndex = num2 & (int) short.MaxValue;
        }
      }
    }

    public int CopyFrom(InputBuffer input, int length)
    {
      length = Math.Min(Math.Min(length, 32768 - this.bytesUsed), input.AvailableBytes);
      int length1 = 32768 - this.end;
      int num;
      if (length > length1)
      {
        num = input.CopyTo(this.window, this.end, length1);
        if (num == length1)
          num += input.CopyTo(this.window, 0, length - length1);
      }
      else
        num = input.CopyTo(this.window, this.end, length);
      this.end = this.end + num & (int) short.MaxValue;
      this.bytesUsed += num;
      return num;
    }

    public int FreeBytes => 32768 - this.bytesUsed;

    public int AvailableBytes => this.bytesUsed;

    public int CopyTo(byte[] output, int offset, int length)
    {
      int num1;
      if (length > this.bytesUsed)
      {
        num1 = this.end;
        length = this.bytesUsed;
      }
      else
        num1 = this.end - this.bytesUsed + length & (int) short.MaxValue;
      int num2 = length;
      int length1 = length - num1;
      if (length1 > 0)
      {
        Array.Copy((Array) this.window, 32768 - length1, (Array) output, offset, length1);
        offset += length1;
        length = num1;
      }
      Array.Copy((Array) this.window, num1 - length, (Array) output, offset, length);
      this.bytesUsed -= num2;
      return num2;
    }
  }
}
