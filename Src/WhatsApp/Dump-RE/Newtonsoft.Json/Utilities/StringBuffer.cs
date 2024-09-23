// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.StringBuffer
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;

#nullable disable
namespace Newtonsoft.Json.Utilities
{
  /// <summary>
  /// Builds a string. Unlike StringBuilder this class lets you reuse it's internal buffer.
  /// </summary>
  internal class StringBuffer
  {
    private char[] _buffer;
    private int _position;
    private static readonly char[] EmptyBuffer = new char[0];

    public int Position
    {
      get => this._position;
      set => this._position = value;
    }

    public StringBuffer() => this._buffer = StringBuffer.EmptyBuffer;

    public StringBuffer(int initalSize) => this._buffer = new char[initalSize];

    public void Append(char value)
    {
      if (this._position == this._buffer.Length)
        this.EnsureSize(1);
      this._buffer[this._position++] = value;
    }

    public void Append(char[] buffer, int startIndex, int count)
    {
      if (this._position + count >= this._buffer.Length)
        this.EnsureSize(count);
      Array.Copy((Array) buffer, startIndex, (Array) this._buffer, this._position, count);
      this._position += count;
    }

    public void Clear()
    {
      this._buffer = StringBuffer.EmptyBuffer;
      this._position = 0;
    }

    private void EnsureSize(int appendLength)
    {
      char[] destinationArray = new char[(this._position + appendLength) * 2];
      Array.Copy((Array) this._buffer, (Array) destinationArray, this._position);
      this._buffer = destinationArray;
    }

    public override string ToString() => this.ToString(0, this._position);

    public string ToString(int start, int length) => new string(this._buffer, start, length);

    public char[] GetInternalBuffer() => this._buffer;
  }
}
