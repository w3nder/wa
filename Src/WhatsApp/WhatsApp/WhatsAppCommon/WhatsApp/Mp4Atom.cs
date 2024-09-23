// Decompiled with JetBrains decompiler
// Type: WhatsApp.Mp4Atom
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;


namespace WhatsApp
{
  public static class Mp4Atom
  {
    private static uint ReadInt32(byte[] bytes, int offset)
    {
      return (uint) ((int) bytes[offset + 3] | (int) bytes[offset + 2] << 8 | (int) bytes[offset + 1] << 16 | (int) bytes[offset + 0] << 24);
    }

    private static void WriteInt32(byte[] bytes, int offset, uint value)
    {
      bytes[offset] = (byte) (value >> 24);
      bytes[offset + 1] = (byte) (value >> 16);
      bytes[offset + 2] = (byte) (value >> 8);
      bytes[offset + 3] = (byte) value;
    }

    public static void Parse(Stream str, long len, Action<string, long, Action> onAtom)
    {
      long position = str.Position;
      byte[] numArray = new byte[8];
      long num1;
      for (bool cancelled = false; !cancelled && str.Position - position < len; str.Position = num1)
      {
        if (str.Read(numArray, 0, numArray.Length) != numArray.Length)
          throw new Exception("Unexpected EOF");
        uint num2 = Mp4Atom.ReadInt32(numArray, 0);
        num1 = str.Position + (long) num2 - 8L;
        if (str.Position - position > len || num1 - position > len)
          throw new Exception("exceeded bounds");
        string str1 = Encoding.UTF8.GetString(numArray, 4, 4);
        onAtom(str1, (long) num2, (Action) (() => cancelled = true));
      }
    }

    public static IObservable<double> GetDurationObservable(string filepath)
    {
      return filepath == null ? Observable.Empty<double>() : Observable.Create<double>((Func<IObserver<double>, Action>) (observer =>
      {
        try
        {
          Mp4Atom.GetDuration(filepath, (Mp4Atom.DurationParserCallback) (duration =>
          {
            observer.OnNext(duration);
            observer.OnCompleted();
          }));
        }
        catch (Exception ex)
        {
          observer.OnCompleted();
        }
        return (Action) (() => { });
      }));
    }

    public static void GetDuration(string filepath, Mp4Atom.DurationParserCallback onDuraton)
    {
      using (IMediaStorage mediaStorage = MediaStorage.Create(filepath))
      {
        try
        {
          using (Stream str = mediaStorage.OpenFile(filepath))
            Mp4Atom.GetDuration(str, str.Length, onDuraton);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public static void GetDuration(Stream str, long len, Mp4Atom.DurationParserCallback onDuraton)
    {
      Mp4Atom.Parse(str, len, (Action<string, long, Action>) ((atomName, innerLen, cancel) =>
      {
        if (innerLen == 1L)
        {
          cancel();
        }
        else
        {
          if (!(atomName == "moov"))
            return;
          Mp4Atom.Parse(str, innerLen - 8L, (Action<string, long, Action>) ((innerAtom, innerInnerLen, innerCancel) =>
          {
            if (!(innerAtom == "mvhd"))
              return;
            byte[] numArray = new byte[8];
            str.Position += 12L;
            if (str.Read(numArray, 0, numArray.Length) != numArray.Length)
              throw new IOException("unexpected eof");
            uint num3 = Mp4Atom.ReadInt32(numArray, 0);
            uint num4 = Mp4Atom.ReadInt32(numArray, 4);
            if (num3 != 0U)
              onDuraton(((double) num4 + 0.0) / (double) num3);
            innerCancel();
          }));
          cancel();
        }
      }));
    }

    public static void GetOrientationMatrices(
      Stream str,
      long len,
      Mp4Atom.MatrixParserCallback onMatrix)
    {
      int count = 2;
      Mp4Atom.MatrixParserCallback innerCallback = onMatrix;
      onMatrix = (Mp4Atom.MatrixParserCallback) ((name, offset, matrix, cancel) =>
      {
        cancel = Utils.IgnoreMultipleInvokes(cancel);
        innerCallback(name, offset, matrix, cancel);
        if (--count != 0)
          return;
        cancel();
      });
      Mp4Atom.ParseOrientationMatrix(str, len, onMatrix, (Action) null, new Mp4Atom.MatrixParser("moov", new Action<Stream, long, Mp4Atom.MatrixParserCallback, Action>(Mp4Atom.GetOrientationMatricesFromMoov)));
    }

    private static void GetOrientationMatricesFromMoov(
      Stream str,
      long len,
      Mp4Atom.MatrixParserCallback onMatrix,
      Action cancel)
    {
      Mp4Atom.ParseOrientationMatrix(str, len, onMatrix, cancel, new Mp4Atom.MatrixParser("mvhd", new Action<Stream, long, Mp4Atom.MatrixParserCallback, Action>(Mp4Atom.GetOrientationMatrixFromMvHd)), new Mp4Atom.MatrixParser("trak", new Action<Stream, long, Mp4Atom.MatrixParserCallback, Action>(Mp4Atom.GetOrientationMatrixFromTrak)));
    }

    private static void GetOrientationMatrixFromMvHd(
      Stream str,
      long len,
      Mp4Atom.MatrixParserCallback onMatrix,
      Action cancel)
    {
      if (len < 72L)
        return;
      str.Position += 36L;
      long position = str.Position;
      onMatrix("mvhd", position, new Mp4Atom.OrientationMatrix(str), cancel);
    }

    private static void GetOrientationMatrixFromTrak(
      Stream str,
      long len,
      Mp4Atom.MatrixParserCallback onMatrix,
      Action cancel)
    {
      List<Action> actions = new List<Action>();
      bool isVideo = false;
      Mp4Atom.ParseOrientationMatrix(str, len, onMatrix, cancel, new Mp4Atom.MatrixParser("tkhd", (Action<Stream, long, Mp4Atom.MatrixParserCallback, Action>) ((str2, len2, onMatrix2, cancel2) =>
      {
        long pos = str2.Position;
        actions.Add((Action) (() =>
        {
          str2.Position = pos;
          Mp4Atom.GetOrientationMatrixFromTkHd(str2, len2, onMatrix2, cancel2);
        }));
      })), new Mp4Atom.MatrixParser("mdia", (Action<Stream, long, Mp4Atom.MatrixParserCallback, Action>) ((str2, len2, onMatrix2, cancel2) => isVideo = Mp4Atom.MdiaIsVideo(str2, len2, cancel2))));
      if (!isVideo)
        return;
      actions.ForEach((Action<Action>) (a => a()));
    }

    private static bool MdiaIsVideo(Stream str, long len, Action callerCancel)
    {
      bool r = false;
      Mp4Atom.Parse(str, len, (Action<string, long, Action>) ((name, length, cancel) =>
      {
        if (!(name == "hdlr"))
          return;
        length -= 8L;
        if (length >= 12L)
        {
          byte[] numArray = new byte[4];
          str.Position += 8L;
          if (str.Read(numArray, 0, numArray.Length) < numArray.Length)
            throw new Exception("unexpected EOF");
          r = Encoding.UTF8.GetString(numArray, 0, numArray.Length) == "vide";
        }
        cancel();
      }));
      return r;
    }

    private static void GetOrientationMatrixFromTkHd(
      Stream str,
      long len,
      Mp4Atom.MatrixParserCallback onMatrix,
      Action cancel)
    {
      if (len < 76L)
        return;
      str.Position += 40L;
      long position = str.Position;
      onMatrix("tkhd", position, new Mp4Atom.OrientationMatrix(str), cancel);
    }

    private static void ParseOrientationMatrix(
      Stream str,
      long len,
      Mp4Atom.MatrixParserCallback onMatrix,
      Action callerCancel,
      params Mp4Atom.MatrixParser[] parsers)
    {
      Dictionary<string, Mp4Atom.MatrixParser> dict = ((IEnumerable<Mp4Atom.MatrixParser>) parsers).ToDictionary<Mp4Atom.MatrixParser, string>((Func<Mp4Atom.MatrixParser, string>) (kv => kv.Key));
      Mp4Atom.Parse(str, len, (Action<string, long, Action>) ((name, length, cancel) =>
      {
        if (callerCancel != null)
        {
          Action old = cancel;
          cancel = (Action) (() =>
          {
            old();
            callerCancel();
          });
        }
        if (length == 1L)
        {
          cancel();
        }
        else
        {
          Mp4Atom.MatrixParser matrixParser;
          if (!dict.TryGetValue(name, out matrixParser))
            return;
          matrixParser.Value(str, length - 8L, Mp4Atom.CombineCancel(onMatrix, cancel), cancel);
        }
      }));
    }

    private static Mp4Atom.MatrixParserCallback CombineCancel(
      Mp4Atom.MatrixParserCallback callback,
      Action cancel)
    {
      return (Mp4Atom.MatrixParserCallback) ((name, off, length, innerCancel) => callback(name, off, length, (Action) (() =>
      {
        innerCancel();
        cancel();
      })));
    }

    public delegate void DurationParserCallback(double seconds);

    public delegate void MatrixParserCallback(
      string name,
      long offsetToStructure,
      Mp4Atom.OrientationMatrix matrix,
      Action cancel);

    private class MatrixParser
    {
      public string Key;
      public Action<Stream, long, Mp4Atom.MatrixParserCallback, Action> Value;

      public MatrixParser(
        string key,
        Action<Stream, long, Mp4Atom.MatrixParserCallback, Action> value)
      {
        this.Key = key;
        this.Value = value;
      }
    }

    public class OrientationMatrix
    {
      private uint[] Values;
      public const int ByteLength = 36;

      private double Convert1616(uint value) => (double) (int) value / 65536.0;

      private uint ConvertTo1616(double value) => (uint) (int) (value * 65536.0);

      public Matrix Matrix
      {
        get
        {
          return new Matrix()
          {
            M11 = this.Convert1616(this.Values[0]),
            M12 = this.Convert1616(this.Values[1]),
            M21 = this.Convert1616(this.Values[3]),
            M22 = this.Convert1616(this.Values[4]),
            OffsetX = this.Convert1616(this.Values[6]),
            OffsetY = this.Convert1616(this.Values[7])
          };
        }
      }

      public void SetMatrix(Matrix m)
      {
        this.Values[0] = this.ConvertTo1616(m.M11);
        this.Values[1] = this.ConvertTo1616(m.M12);
        this.Values[2] = 0U;
        this.Values[3] = this.ConvertTo1616(m.M21);
        this.Values[4] = this.ConvertTo1616(m.M22);
        this.Values[5] = 0U;
        this.Values[6] = this.ConvertTo1616(m.OffsetX);
        this.Values[7] = this.ConvertTo1616(m.OffsetY);
        this.Values[8] = 1073741824U;
      }

      public void WriteMatrixToStream(Stream file, long offset)
      {
        byte[] numArray = new byte[36];
        int offset1 = 0;
        foreach (uint num in this.Values)
        {
          Mp4Atom.WriteInt32(numArray, offset1, num);
          offset1 += 4;
        }
        long position = file.Position;
        file.Position = offset;
        file.Write(numArray, 0, numArray.Length);
        file.Position = position;
      }

      public bool IsIdentity
      {
        get
        {
          return this.Values[0] == 65536U && this.Values[1] == 0U && this.Values[3] == 0U && this.Values[4] == 65536U;
        }
      }

      public OrientationMatrix(byte[] bytes, int offset = 0, int length = 36)
      {
        if (length < 36)
          throw new Exception("unexpected matrix size");
        if (offset < 0 || offset > bytes.Length || (long) offset + (long) length > (long) bytes.Length)
          throw new Exception("invalid offset/length");
        this.Values = new uint[9];
        for (int index = 0; index < this.Values.Length; ++index)
          this.Values[index] = Mp4Atom.ReadInt32(bytes, offset + index * 4);
      }

      private static byte[] ReadInitial(Stream str, int length)
      {
        byte[] buffer = new byte[length];
        if (str.Read(buffer, 0, buffer.Length) < buffer.Length)
          throw new Exception("unexpected EOF");
        return buffer;
      }

      public OrientationMatrix(Stream str, int length = 36)
        : this(Mp4Atom.OrientationMatrix.ReadInitial(str, length))
      {
      }

      public OrientationMatrix()
        : this(new byte[36])
      {
      }
    }
  }
}
