// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaCollections.BinaryData
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable disable
namespace WhatsApp.WaCollections
{
  public class BinaryData
  {
    private List<byte> bytes_ = new List<byte>();

    public BinaryData()
    {
    }

    public BinaryData(byte[] bytes) => this.bytes_ = ((IEnumerable<byte>) bytes).ToList<byte>();

    public byte[] Get() => this.bytes_.ToArray();

    public int Length() => this.bytes_ != null ? this.bytes_.Count : -1;

    public void AppendByte(byte b) => this.bytes_.Add(b);

    public void AppendBytes(IEnumerable<byte> b) => this.bytes_.AddRange(b);

    public void AppendBytes(byte[] b, int offset, int length)
    {
      this.AppendBytes(((IEnumerable<byte>) b).Skip<byte>(offset).Take<byte>(length));
    }

    public byte ReadByte(int offset)
    {
      return ((IEnumerable<byte>) this.ReadBytes(offset, 1)).First<byte>();
    }

    public byte[] ReadBytes(int offset, int length = -1)
    {
      int num = this.bytes_.Count - offset;
      return length < 0 || length > num ? this.bytes_.Skip<byte>(offset).ToArray<byte>() : this.bytes_.Skip<byte>(offset).Take<byte>(length).ToArray<byte>();
    }

    public void AppendInt32(int val)
    {
      uint num = (uint) val;
      this.AppendByte((byte) (num & (uint) byte.MaxValue));
      this.AppendByte((byte) (num >> 8 & (uint) byte.MaxValue));
      this.AppendByte((byte) (num >> 16 & (uint) byte.MaxValue));
      this.AppendByte((byte) (num >> 24 & (uint) byte.MaxValue));
    }

    public int ReadInt32(int offset)
    {
      return (int) this.bytes_[offset + 0] | (int) this.bytes_[offset + 1] << 8 | (int) this.bytes_[offset + 2] << 16 | (int) this.bytes_[offset + 3] << 24;
    }

    public static int ReadInt32(byte[] bytes, int offset)
    {
      return (int) bytes[offset + 0] | (int) bytes[offset + 1] << 8 | (int) bytes[offset + 2] << 16 | (int) bytes[offset + 3] << 24;
    }

    public void AppendLong64(long val) => this.AppendULong64((ulong) val);

    public void AppendULong64(ulong val)
    {
      this.AppendByte((byte) (val & (ulong) byte.MaxValue));
      this.AppendByte((byte) (val >> 8 & (ulong) byte.MaxValue));
      this.AppendByte((byte) (val >> 16 & (ulong) byte.MaxValue));
      this.AppendByte((byte) (val >> 24 & (ulong) byte.MaxValue));
      this.AppendByte((byte) (val >> 32 & (ulong) byte.MaxValue));
      this.AppendByte((byte) (val >> 40 & (ulong) byte.MaxValue));
      this.AppendByte((byte) (val >> 48 & (ulong) byte.MaxValue));
      this.AppendByte((byte) (val >> 56 & (ulong) byte.MaxValue));
    }

    public long ReadLong64(int offset) => (long) this.ReadULong64(offset);

    public ulong ReadULong64(int offset)
    {
      return (ulong) ((long) this.bytes_[offset + 0] | (long) this.bytes_[offset + 1] << 8 | (long) this.bytes_[offset + 2] << 16 | (long) this.bytes_[offset + 3] << 24 | (long) this.bytes_[offset + 4] << 32 | (long) this.bytes_[offset + 5] << 40 | (long) this.bytes_[offset + 6] << 48 | (long) this.bytes_[offset + 7] << 56);
    }

    public void AppendBytesWithLengthPrefix(byte[] bytes)
    {
      int val = bytes == null ? -1 : bytes.Length;
      this.AppendInt32(val);
      if (val <= 0)
        return;
      this.AppendBytes((IEnumerable<byte>) bytes);
    }

    public byte[] ReadBytesWithLengthPrefix(int offset)
    {
      int newOffset = 0;
      return this.ReadBytesWithLengthPrefix(offset, out newOffset);
    }

    public byte[] ReadBytesWithLengthPrefix(int offset, out int newOffset)
    {
      int count = this.ReadInt32(offset);
      newOffset = offset + 4;
      if (count > this.bytes_.Count - newOffset)
        count = this.bytes_.Count - newOffset;
      byte[] numArray;
      if (count < 0)
        numArray = (byte[]) null;
      else if (count == 0)
      {
        numArray = new byte[0];
      }
      else
      {
        numArray = this.bytes_.Skip<byte>(newOffset).Take<byte>(count).ToArray<byte>();
        newOffset += count;
      }
      return numArray;
    }

    public void AppendStrWithLengthPrefix(string s)
    {
      byte[] bytes = (byte[]) null;
      if (s != null)
        bytes = Encoding.UTF8.GetBytes(s);
      this.AppendBytesWithLengthPrefix(bytes);
    }

    public string ReadStrWithLengthPrefix(int offset)
    {
      int newOffset = 0;
      return this.ReadStrWithLengthPrefix(offset, out newOffset);
    }

    public string ReadStrWithLengthPrefix(int offset, out int newOffset)
    {
      byte[] numArray = this.ReadBytesWithLengthPrefix(offset, out newOffset);
      return numArray != null ? (!((IEnumerable<byte>) numArray).Any<byte>() ? "" : Encoding.UTF8.GetString(numArray, 0, numArray.Length)) : (string) null;
    }
  }
}
