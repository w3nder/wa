// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Zip.ZipExtraData
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System;
using System.IO;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Zip
{
  public sealed class ZipExtraData : IDisposable
  {
    private int index_;
    private int readValueStart_;
    private int readValueLength_;
    private MemoryStream newEntry_;
    private byte[] data_;

    public ZipExtraData() => this.Clear();

    public ZipExtraData(byte[] data)
    {
      if (data == null)
        this.data_ = new byte[0];
      else
        this.data_ = data;
    }

    public byte[] GetEntryData()
    {
      if (this.Length > (int) ushort.MaxValue)
        throw new ZipException("Data exceeds maximum length");
      return (byte[]) this.data_.Clone();
    }

    public void Clear()
    {
      if (this.data_ != null && this.data_.Length == 0)
        return;
      this.data_ = new byte[0];
    }

    public int Length => this.data_.Length;

    public Stream GetStreamForTag(int tag)
    {
      Stream streamForTag = (Stream) null;
      if (this.Find(tag))
        streamForTag = (Stream) new MemoryStream(this.data_, this.index_, this.readValueLength_, false);
      return streamForTag;
    }

    private ITaggedData GetData(short tag)
    {
      ITaggedData data = (ITaggedData) null;
      if (this.Find((int) tag))
        data = this.Create(tag, this.data_, this.readValueStart_, this.readValueLength_);
      return data;
    }

    private ITaggedData Create(short tag, byte[] data, int offset, int count)
    {
      ITaggedData taggedData;
      switch (tag)
      {
        case 10:
          taggedData = (ITaggedData) new NTTaggedData();
          break;
        case 21589:
          taggedData = (ITaggedData) new ExtendedUnixData();
          break;
        default:
          taggedData = (ITaggedData) new RawTaggedData(tag);
          break;
      }
      taggedData.SetData(this.data_, this.readValueStart_, this.readValueLength_);
      return taggedData;
    }

    public int ValueLength => this.readValueLength_;

    public int CurrentReadIndex => this.index_;

    public int UnreadCount
    {
      get
      {
        if (this.readValueStart_ > this.data_.Length || this.readValueStart_ < 4)
          throw new ZipException("Find must be called before calling a Read method");
        return this.readValueStart_ + this.readValueLength_ - this.index_;
      }
    }

    public bool Find(int headerID)
    {
      this.readValueStart_ = this.data_.Length;
      this.readValueLength_ = 0;
      this.index_ = 0;
      int num1 = this.readValueStart_;
      int num2 = headerID - 1;
      while (num2 != headerID && this.index_ < this.data_.Length - 3)
      {
        num2 = this.ReadShortInternal();
        num1 = this.ReadShortInternal();
        if (num2 != headerID)
          this.index_ += num1;
      }
      bool flag = num2 == headerID && this.index_ + num1 <= this.data_.Length;
      if (flag)
      {
        this.readValueStart_ = this.index_;
        this.readValueLength_ = num1;
      }
      return flag;
    }

    public void AddEntry(ITaggedData taggedData)
    {
      if (taggedData == null)
        throw new ArgumentNullException(nameof (taggedData));
      this.AddEntry((int) taggedData.TagID, taggedData.GetData());
    }

    public void AddEntry(int headerID, byte[] fieldData)
    {
      if (headerID > (int) ushort.MaxValue || headerID < 0)
        throw new ArgumentOutOfRangeException(nameof (headerID));
      int length1 = fieldData == null ? 0 : fieldData.Length;
      if (length1 > (int) ushort.MaxValue)
        throw new ArgumentOutOfRangeException(nameof (fieldData), "exceeds maximum length");
      int length2 = this.data_.Length + length1 + 4;
      if (this.Find(headerID))
        length2 -= this.ValueLength + 4;
      if (length2 > (int) ushort.MaxValue)
        throw new ZipException("Data exceeds maximum length");
      this.Delete(headerID);
      byte[] numArray = new byte[length2];
      this.data_.CopyTo((Array) numArray, 0);
      int length3 = this.data_.Length;
      this.data_ = numArray;
      this.SetShort(ref length3, headerID);
      this.SetShort(ref length3, length1);
      fieldData?.CopyTo((Array) numArray, length3);
    }

    public void StartNewEntry() => this.newEntry_ = new MemoryStream();

    public void AddNewEntry(int headerID)
    {
      byte[] array = this.newEntry_.ToArray();
      this.newEntry_ = (MemoryStream) null;
      this.AddEntry(headerID, array);
    }

    public void AddData(byte data) => this.newEntry_.WriteByte(data);

    public void AddData(byte[] data)
    {
      if (data == null)
        throw new ArgumentNullException(nameof (data));
      this.newEntry_.Write(data, 0, data.Length);
    }

    public void AddLeShort(int toAdd)
    {
      this.newEntry_.WriteByte((byte) toAdd);
      this.newEntry_.WriteByte((byte) (toAdd >> 8));
    }

    public void AddLeInt(int toAdd)
    {
      this.AddLeShort((int) (short) toAdd);
      this.AddLeShort((int) (short) (toAdd >> 16));
    }

    public void AddLeLong(long toAdd)
    {
      this.AddLeInt((int) (toAdd & (long) uint.MaxValue));
      this.AddLeInt((int) (toAdd >> 32));
    }

    public bool Delete(int headerID)
    {
      bool flag = false;
      if (this.Find(headerID))
      {
        flag = true;
        int num = this.readValueStart_ - 4;
        byte[] destinationArray = new byte[this.data_.Length - (this.ValueLength + 4)];
        Array.Copy((Array) this.data_, 0, (Array) destinationArray, 0, num);
        int sourceIndex = num + this.ValueLength + 4;
        Array.Copy((Array) this.data_, sourceIndex, (Array) destinationArray, num, this.data_.Length - sourceIndex);
        this.data_ = destinationArray;
      }
      return flag;
    }

    public long ReadLong()
    {
      this.ReadCheck(8);
      return (long) this.ReadInt() & (long) uint.MaxValue | (long) this.ReadInt() << 32;
    }

    public int ReadInt()
    {
      this.ReadCheck(4);
      int num = (int) this.data_[this.index_] + ((int) this.data_[this.index_ + 1] << 8) + ((int) this.data_[this.index_ + 2] << 16) + ((int) this.data_[this.index_ + 3] << 24);
      this.index_ += 4;
      return num;
    }

    public int ReadShort()
    {
      this.ReadCheck(2);
      int num = (int) this.data_[this.index_] + ((int) this.data_[this.index_ + 1] << 8);
      this.index_ += 2;
      return num;
    }

    public int ReadByte()
    {
      int num = -1;
      if (this.index_ < this.data_.Length && this.readValueStart_ + this.readValueLength_ > this.index_)
      {
        num = (int) this.data_[this.index_];
        ++this.index_;
      }
      return num;
    }

    public void Skip(int amount)
    {
      this.ReadCheck(amount);
      this.index_ += amount;
    }

    private void ReadCheck(int length)
    {
      if (this.readValueStart_ > this.data_.Length || this.readValueStart_ < 4)
        throw new ZipException("Find must be called before calling a Read method");
      if (this.index_ > this.readValueStart_ + this.readValueLength_ - length)
        throw new ZipException("End of extra data");
    }

    private int ReadShortInternal()
    {
      if (this.index_ > this.data_.Length - 2)
        throw new ZipException("End of extra data");
      int num = (int) this.data_[this.index_] + ((int) this.data_[this.index_ + 1] << 8);
      this.index_ += 2;
      return num;
    }

    private void SetShort(ref int index, int source)
    {
      this.data_[index] = (byte) source;
      this.data_[index + 1] = (byte) (source >> 8);
      index += 2;
    }

    public void Dispose()
    {
      if (this.newEntry_ == null)
        return;
      this.newEntry_.Close();
    }
  }
}
