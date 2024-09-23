// Decompiled with JetBrains decompiler
// Type: System.Text.Latin1Encoding
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

#nullable disable
namespace System.Text
{
  internal class Latin1Encoding : Encoding
  {
    private const byte substitution = 63;

    public override int GetByteCount(char[] chars, int index, int count) => count;

    public override int GetBytes(
      char[] chars,
      int charIndex,
      int charCount,
      byte[] bytes,
      int byteIndex)
    {
      if (chars == null || bytes == null)
        throw new ArgumentNullException(chars == null ? nameof (chars) : nameof (bytes));
      if (charIndex < 0 || charCount < 0)
        throw new ArgumentOutOfRangeException(charIndex < 0 ? nameof (charIndex) : nameof (charCount));
      if (chars.Length - charIndex < charCount)
        throw new ArgumentOutOfRangeException(nameof (chars));
      if (byteIndex < 0 || byteIndex > bytes.Length)
        throw new ArgumentOutOfRangeException(nameof (byteIndex));
      if (bytes.Length - byteIndex < charCount)
        throw new ArgumentOutOfRangeException(nameof (charCount));
      for (int index = 0; index < charCount; ++index)
      {
        char ch = chars[charIndex + index];
        bytes[byteIndex + index] = ch > 'ÿ' ? (byte) 63 : (byte) ch;
      }
      return charCount;
    }

    public override int GetCharCount(byte[] bytes, int index, int count) => count;

    public override int GetChars(
      byte[] bytes,
      int byteIndex,
      int byteCount,
      char[] chars,
      int charIndex)
    {
      if (chars == null || bytes == null)
        throw new ArgumentNullException(chars == null ? nameof (chars) : nameof (bytes));
      if (byteIndex < 0 || byteCount < 0)
        throw new ArgumentOutOfRangeException(charIndex < 0 ? nameof (byteIndex) : nameof (byteCount));
      if (bytes.Length - byteIndex < byteCount)
        throw new ArgumentOutOfRangeException(nameof (bytes));
      if (charIndex < 0 || charIndex > chars.Length)
        throw new ArgumentOutOfRangeException(nameof (charIndex));
      if (chars.Length - charIndex < byteCount)
        throw new ArgumentOutOfRangeException(nameof (byteCount));
      for (int index = 0; index < byteCount; ++index)
        chars[charIndex + index] = (char) bytes[byteIndex + index];
      return byteCount;
    }

    public override int GetMaxByteCount(int charCount) => charCount;

    public override int GetMaxCharCount(int byteCount) => byteCount;

    public static Encoding GetEncoding()
    {
      try
      {
        return Encoding.GetEncoding("iso-8859-1");
      }
      catch (ArgumentException ex)
      {
        return (Encoding) new Latin1Encoding();
      }
    }
  }
}
