// Decompiled with JetBrains decompiler
// Type: WhatsApp.PdfUtils
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using ICSharpCode.SharpZipLib.Silverlight.Zip.Compression.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public static class PdfUtils
  {
    private const string NameType = "/Type";
    private const string NamePages = "/Pages";
    private const string NamePage = "/Page";
    private const string NameCount = "/Count";
    private const string NameParent = "/Parent";
    private const string NameFilter = "/Filter";
    private const string NameObjStm = "/ObjStm";
    private const string NameLength = "/Length";
    private const string NameFlateDecode = "/FlateDecode";
    private const string NameAction = "/Action";
    private const char DictStart = '<';
    private const char DictEnd = '>';
    private const char ArrayStart = '[';
    private const char ArrayEnd = ']';
    private const char StringStart = '(';
    private const char StringEnd = ')';
    private const char NameStart = '/';
    private static string[] SuspicousNames = new string[6]
    {
      "/RichMedia",
      "/JS",
      "/JavaScript",
      "/AA",
      "/Launch",
      "/RichMediaInstance"
    };

    private static byte[] PdfHeader
    {
      get => "%PDF-".Select<char, byte>((Func<char, byte>) (c => (byte) c)).ToArray<byte>();
    }

    private static byte[] FdfHeader
    {
      get => "%PDF-".Select<char, byte>((Func<char, byte>) (c => (byte) c)).ToArray<byte>();
    }

    private static byte[] ObjSig
    {
      get => " obj".Select<char, byte>((Func<char, byte>) (c => (byte) c)).ToArray<byte>();
    }

    private static byte[] EndObjSig
    {
      get => "endobj".Select<char, byte>((Func<char, byte>) (c => (byte) c)).ToArray<byte>();
    }

    private static byte[] StreamSig
    {
      get => "stream".Select<char, byte>((Func<char, byte>) (c => (byte) c)).ToArray<byte>();
    }

    private static byte[] EndStreamSig
    {
      get => "endstream".Select<char, byte>((Func<char, byte>) (c => (byte) c)).ToArray<byte>();
    }

    public static PdfUtils.PdfInfo Parse(Stream pdfStream)
    {
      byte[] numArray = new byte[5];
      if (pdfStream.Read(numArray, 0, 5) > 0 && !PdfUtils.BytesEqual(numArray, PdfUtils.PdfHeader))
      {
        pdfStream.SafeDispose();
        pdfStream = (Stream) null;
        throw new ArgumentException("malformed pdf");
      }
      PdfUtils.PdfInfo info = new PdfUtils.PdfInfo();
      PdfUtils.ParsePdfStream(pdfStream, true, ref info);
      return info;
    }

    public static void ParsePdfStream(Stream pdfStream, bool topLevel, ref PdfUtils.PdfInfo info)
    {
      while (true)
      {
        do
        {
          if (topLevel)
          {
            if (!PdfUtils.SkipToSignature(pdfStream, PdfUtils.ObjSig))
              return;
          }
          else if (pdfStream.ReadByte() == -1)
            goto label_3;
          int num = PdfUtils.SkipWhiteSpace(pdfStream);
          if (num != -1)
          {
            Dictionary<string, object> dict = (Dictionary<string, object>) null;
            while (true)
            {
              switch (num)
              {
                case -1:
                  goto label_11;
                case 60:
                  if (pdfStream.ReadByte() != 60)
                    break;
                  goto label_8;
              }
              num = pdfStream.ReadByte();
            }
label_8:
            dict = PdfUtils.ReadDict(pdfStream);
label_11:
            if (num != -1 && dict != null)
            {
              if (!info.IsSuspicious)
                info.IsSuspicious = PdfUtils.HasSuspiciousContent(dict);
              object obj1 = (object) null;
              string str1 = (string) null;
              if (dict.TryGetValue("/Type", out obj1))
                str1 = obj1 as string;
              if (info.PageCount <= 0 && str1 == "/Pages" && !dict.ContainsKey("/Parent"))
              {
                object s = (object) null;
                int result = 0;
                if (dict.TryGetValue("/Count", out s) && s != null && int.TryParse(s as string, out result) && result > 0)
                  info.PageCount = result;
              }
              if (num != -1)
              {
                object obj2 = (object) null;
                int result = 0;
                if (dict.TryGetValue("/Length", out obj2) && obj2 is string s && !string.IsNullOrEmpty(s) && s[s.Length - 1] != 'R')
                  int.TryParse(s, out result);
                if ("/ObjStm" == str1)
                {
                  PdfUtils.SkipToSignature(pdfStream, PdfUtils.StreamSig);
                  PdfUtils.SkipNewLine(pdfStream);
                  object obj3 = (object) null;
                  if (dict.TryGetValue("/Filter", out obj3) && obj3 is string str2 && str2 == "/FlateDecode")
                    PdfUtils.ParsePdfStream((Stream) new InflaterInputStream((Stream) new PdfUtils.StreamWithReadLimit(pdfStream, result)), false, ref info);
                  else
                    PdfUtils.SkipBytes(pdfStream, result);
                }
                else
                  PdfUtils.SkipBytes(pdfStream, result);
              }
              else
                goto label_32;
            }
            else
              goto label_31;
          }
          else
            goto label_30;
        }
        while (!topLevel);
        PdfUtils.SkipToSignature(pdfStream, PdfUtils.EndObjSig);
      }
label_3:
      return;
label_30:
      return;
label_31:
      return;
label_32:;
    }

    private static bool BytesEqual(byte[] bytes0, byte[] bytes1)
    {
      if (bytes0 == null || bytes1 == null)
        return bytes0 == null && bytes1 == null;
      if (bytes0.Length != bytes1.Length)
        return false;
      int length = bytes0.Length;
      for (int index = 0; index < length; ++index)
      {
        if ((int) bytes0[index] != (int) bytes1[index])
          return false;
      }
      return true;
    }

    private static Dictionary<string, object> ReadDict(Stream pdfStream)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      int num = PdfUtils.SkipWhiteSpace(pdfStream);
      while (true)
      {
        if (PdfUtils.IsWhiteSpace(num))
          num = PdfUtils.SkipWhiteSpace(pdfStream);
        switch (num)
        {
          case -1:
            goto label_23;
          case 62:
            if (pdfStream.ReadByte() == 62)
              goto label_23;
            else
              break;
        }
        StringBuilder stringBuilder1 = new StringBuilder();
        do
        {
          stringBuilder1.Append((char) num);
          num = pdfStream.ReadByte();
        }
        while (!PdfUtils.IsDelimiter(num) && !PdfUtils.IsWhiteSpace(num));
        if (PdfUtils.IsWhiteSpace(num))
          num = PdfUtils.SkipWhiteSpace(pdfStream);
        if (num != -1)
        {
          object obj = (object) null;
          if (num != 40)
          {
            if (num != 60)
            {
              if (num == 91)
              {
                PdfUtils.SkipArray(pdfStream);
                num = pdfStream.ReadByte();
              }
              else
              {
                StringBuilder stringBuilder2 = new StringBuilder();
                if (num == 47)
                {
                  stringBuilder2.Append((char) num);
                  num = pdfStream.ReadByte();
                }
                for (; !PdfUtils.IsDelimiter(num) || PdfUtils.IsWhiteSpace(num); num = pdfStream.ReadByte())
                  stringBuilder2.Append((char) num);
                obj = (object) stringBuilder2.ToString().Trim();
              }
            }
            else
            {
              num = pdfStream.ReadByte();
              if (num == 60)
              {
                obj = (object) PdfUtils.ReadDict(pdfStream);
                num = pdfStream.ReadByte();
              }
            }
          }
          else
          {
            PdfUtils.SkipString(pdfStream);
            num = pdfStream.ReadByte();
          }
          dictionary[stringBuilder1.ToString().Trim()] = obj;
        }
        else
          break;
      }
label_23:
      return dictionary;
    }

    private static bool IsNameSuspicious(string name)
    {
      name = PdfUtils.NormalizeName(name);
      return ((IEnumerable<string>) PdfUtils.SuspicousNames).Contains<string>(name);
    }

    private static string NormalizeName(string name)
    {
      if (string.IsNullOrEmpty(name) || name[0] != '/' || name.IndexOf('#') < 0)
        return name;
      StringBuilder stringBuilder = new StringBuilder();
      int length = name.Length;
      for (int index = 0; index < length; ++index)
      {
        char ch = name[index];
        if (ch == '#' && index <= length - 3)
        {
          char int32 = (char) Convert.ToInt32(name.Substring(index + 1, 2), 16);
          stringBuilder.Append(int32);
          index += 2;
        }
        else
          stringBuilder.Append(ch);
      }
      return stringBuilder.ToString();
    }

    private static bool HasSuspiciousContent(Dictionary<string, object> dict)
    {
      bool flag = false;
      if (dict == null)
        return flag;
      foreach (KeyValuePair<string, object> keyValuePair in dict)
      {
        flag = PdfUtils.IsNameSuspicious(keyValuePair.Key);
        if (!flag)
        {
          object obj = keyValuePair.Value;
          if (obj is string)
            flag = PdfUtils.IsNameSuspicious(obj as string);
          else if (obj is Dictionary<string, object>)
            flag = PdfUtils.HasSuspiciousContent(obj as Dictionary<string, object>);
          if (flag)
            break;
        }
        else
          break;
      }
      return flag;
    }

    private static string ReadString(Stream pdfStream)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append('(');
      int num1;
      do
      {
        num1 = pdfStream.ReadByte();
        stringBuilder.Append((char) num1);
        if (num1 == 92)
        {
          int num2 = pdfStream.ReadByte();
          stringBuilder.Append((char) num2);
        }
      }
      while (num1 != 41 && num1 != -1);
      return stringBuilder.ToString();
    }

    private static void SkipBytes(Stream pdfStream, int n)
    {
      for (; n > 0; --n)
        pdfStream.ReadByte();
    }

    private static void SkipString(Stream pdfStream)
    {
      int num1 = -1;
      int num2;
      do
      {
        num2 = pdfStream.ReadByte();
        if (num2 == 92)
          num1 = pdfStream.ReadByte();
      }
      while (num2 != 41 && num2 != -1);
    }

    private static void SkipHexString(Stream pdfStream)
    {
      do
        ;
      while (pdfStream.ReadByte() != 62);
    }

    private static void SkipArray(Stream pdfStream)
    {
      int num = PdfUtils.SkipWhiteSpace(pdfStream);
      while (true)
      {
        switch (num)
        {
          case -1:
            goto label_7;
          case 40:
            PdfUtils.SkipString(pdfStream);
            break;
          case 60:
            PdfUtils.SkipHexString(pdfStream);
            break;
          case 91:
            PdfUtils.SkipArray(pdfStream);
            break;
          case 93:
            goto label_6;
        }
        num = PdfUtils.SkipWhiteSpace(pdfStream);
      }
label_6:
      return;
label_7:;
    }

    private static bool SkipToSignature(Stream pdfStream, byte[] sig)
    {
      CircularBuffer circularBuffer = new CircularBuffer(sig.Length * 2);
      for (int b = pdfStream.ReadByte(); b >= 0; b = pdfStream.ReadByte())
      {
        circularBuffer.Add((byte) b);
        if (circularBuffer.EndMatches(sig))
          return true;
      }
      return false;
    }

    private static void SkipNewLine(Stream pdfStream)
    {
      if (pdfStream.ReadByte() != 13)
        return;
      pdfStream.ReadByte();
    }

    private static int SkipWhiteSpace(Stream pdfStream)
    {
      int c = pdfStream.ReadByte();
      while (PdfUtils.IsWhiteSpace(c))
        c = pdfStream.ReadByte();
      return c;
    }

    private static bool IsWhiteSpace(int c)
    {
      return c == 0 || c == 9 || c == 10 || c == 12 || c == 13 || c == 32;
    }

    private static bool IsDelimiter(int b)
    {
      return b == 47 || b == 60 || b == 62 || b == 91 || b == 93 || b == 40 || b == 41 || b == -1;
    }

    public class PdfInfo
    {
      public int PageCount { get; set; }

      public bool IsSuspicious { get; set; }
    }

    private class StreamWithReadLimit : Stream
    {
      private Stream stream;
      private int limit;

      public StreamWithReadLimit(Stream stream, int limit)
      {
        this.stream = stream;
        this.limit = limit;
      }

      public override int ReadByte()
      {
        if (this.limit <= 0)
          return -1;
        --this.limit;
        return this.stream.ReadByte();
      }

      public override bool CanRead => this.stream.CanRead;

      public override bool CanSeek => this.stream.CanSeek;

      public override bool CanWrite => this.stream.CanWrite;

      public override long Length => this.stream.Length;

      public override long Position
      {
        get => this.stream.Position;
        set => this.stream.Position = value;
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        return this.stream.Read(buffer, offset, count);
      }

      public override void Flush() => this.stream.Flush();

      public override long Seek(long offset, SeekOrigin origin) => this.stream.Seek(offset, origin);

      public override void SetLength(long value) => this.stream.SetLength(value);

      public override void Write(byte[] buffer, int offset, int count)
      {
        this.stream.Write(buffer, offset, count);
      }
    }
  }
}
