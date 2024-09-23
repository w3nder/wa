// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.Base64Encoder
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;
using System.IO;

#nullable disable
namespace Newtonsoft.Json.Utilities
{
  internal class Base64Encoder
  {
    private const int Base64LineSize = 76;
    private const int LineSizeInBytes = 57;
    private readonly char[] _charsLine = new char[76];
    private readonly TextWriter _writer;
    private byte[] _leftOverBytes;
    private int _leftOverBytesCount;

    public Base64Encoder(TextWriter writer)
    {
      ValidationUtils.ArgumentNotNull((object) writer, nameof (writer));
      this._writer = writer;
    }

    public void Encode(byte[] buffer, int index, int count)
    {
      if (buffer == null)
        throw new ArgumentNullException(nameof (buffer));
      if (index < 0)
        throw new ArgumentOutOfRangeException(nameof (index));
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof (count));
      if (count > buffer.Length - index)
        throw new ArgumentOutOfRangeException(nameof (count));
      if (this._leftOverBytesCount > 0)
      {
        int leftOverBytesCount;
        for (leftOverBytesCount = this._leftOverBytesCount; leftOverBytesCount < 3 && count > 0; --count)
          this._leftOverBytes[leftOverBytesCount++] = buffer[index++];
        if (count == 0 && leftOverBytesCount < 3)
        {
          this._leftOverBytesCount = leftOverBytesCount;
          return;
        }
        this.WriteChars(this._charsLine, 0, Convert.ToBase64CharArray(this._leftOverBytes, 0, 3, this._charsLine, 0));
      }
      this._leftOverBytesCount = count % 3;
      if (this._leftOverBytesCount > 0)
      {
        count -= this._leftOverBytesCount;
        if (this._leftOverBytes == null)
          this._leftOverBytes = new byte[3];
        for (int index1 = 0; index1 < this._leftOverBytesCount; ++index1)
          this._leftOverBytes[index1] = buffer[index + count + index1];
      }
      int num = index + count;
      int length = 57;
      for (; index < num; index += length)
      {
        if (index + length > num)
          length = num - index;
        this.WriteChars(this._charsLine, 0, Convert.ToBase64CharArray(buffer, index, length, this._charsLine, 0));
      }
    }

    public void Flush()
    {
      if (this._leftOverBytesCount <= 0)
        return;
      this.WriteChars(this._charsLine, 0, Convert.ToBase64CharArray(this._leftOverBytes, 0, this._leftOverBytesCount, this._charsLine, 0));
      this._leftOverBytesCount = 0;
    }

    private void WriteChars(char[] chars, int index, int count)
    {
      this._writer.Write(chars, index, count);
    }
  }
}
