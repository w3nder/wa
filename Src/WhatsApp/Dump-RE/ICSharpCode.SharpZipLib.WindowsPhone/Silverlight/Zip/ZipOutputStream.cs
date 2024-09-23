﻿// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Zip.ZipOutputStream
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using ICSharpCode.SharpZipLib.Silverlight.Checksums;
using ICSharpCode.SharpZipLib.Silverlight.Zip.Compression;
using ICSharpCode.SharpZipLib.Silverlight.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Zip
{
  public class ZipOutputStream : DeflaterOutputStream
  {
    private readonly Crc32 crc = new Crc32();
    private long crcPatchPos = -1;
    private ZipEntry curEntry;
    private CompressionMethod curMethod = CompressionMethod.Deflated;
    private int defaultCompressionLevel = -1;
    private List<ZipEntry> entries = new List<ZipEntry>();
    private long _offset;
    private bool patchEntryHeader;
    private long size;
    private long sizePatchPos = -1;
    private UseZip64 useZip64_ = UseZip64.Dynamic;
    private byte[] zipComment = new byte[0];

    public ZipOutputStream(Stream baseOutputStream)
      : base(baseOutputStream, new Deflater(-1, true))
    {
    }

    public bool IsFinished => this.entries == null;

    public UseZip64 UseZip64
    {
      get => this.useZip64_;
      set => this.useZip64_ = value;
    }

    public void SetComment(string comment)
    {
      byte[] array = ZipConstants.ConvertToArray(comment);
      this.zipComment = array.Length <= (int) ushort.MaxValue ? array : throw new ArgumentOutOfRangeException(nameof (comment));
    }

    public void SetLevel(int level)
    {
      this.deflater_.SetLevel(level);
      this.defaultCompressionLevel = level;
    }

    public int GetLevel() => this.deflater_.GetLevel();

    private void WriteLeShort(int value)
    {
      this.baseOutputStream_.WriteByte((byte) (value & (int) byte.MaxValue));
      this.baseOutputStream_.WriteByte((byte) (value >> 8 & (int) byte.MaxValue));
    }

    private void WriteLeInt(int value)
    {
      this.WriteLeShort(value);
      this.WriteLeShort(value >> 16);
    }

    private void WriteLeLong(long value)
    {
      this.WriteLeInt((int) value);
      this.WriteLeInt((int) (value >> 32));
    }

    public void PutNextEntry(ZipEntry entry)
    {
      if (entry == null)
        throw new ArgumentNullException(nameof (entry));
      if (this.entries == null)
        throw new InvalidOperationException("ZipOutputStream was finished");
      if (this.curEntry != null)
        this.CloseEntry();
      if (this.entries.Count == int.MaxValue)
        throw new ZipException("Too many entries for Zip file");
      CompressionMethod compressionMethod = entry.CompressionMethod;
      int level = this.defaultCompressionLevel;
      entry.Flags &= 2048;
      this.patchEntryHeader = false;
      bool flag = true;
      if (compressionMethod == CompressionMethod.Stored)
      {
        entry.Flags &= -9;
        if (entry.CompressedSize >= 0L)
        {
          if (entry.Size < 0L)
            entry.Size = entry.CompressedSize;
          else if (entry.Size != entry.CompressedSize)
            throw new ZipException("Method STORED, but compressed size != size");
        }
        else if (entry.Size >= 0L)
          entry.CompressedSize = entry.Size;
        if (entry.Size < 0L || entry.Crc < 0L)
        {
          if (this.CanPatchEntries)
          {
            flag = false;
          }
          else
          {
            compressionMethod = CompressionMethod.Deflated;
            level = 0;
          }
        }
      }
      if (compressionMethod == CompressionMethod.Deflated)
      {
        if (entry.Size == 0L)
        {
          entry.CompressedSize = entry.Size;
          entry.Crc = 0L;
          compressionMethod = CompressionMethod.Stored;
        }
        else if (entry.CompressedSize < 0L || entry.Size < 0L || entry.Crc < 0L)
          flag = false;
      }
      if (!flag)
      {
        if (!this.CanPatchEntries)
          entry.Flags |= 8;
        else
          this.patchEntryHeader = true;
      }
      if (this.Password != null)
      {
        entry.IsCrypted = true;
        if (entry.Crc < 0L)
          entry.Flags |= 8;
      }
      entry.Offset = this._offset;
      entry.CompressionMethod = compressionMethod;
      this.curMethod = compressionMethod;
      this.sizePatchPos = -1L;
      if (this.useZip64_ == UseZip64.On || entry.Size < 0L && this.useZip64_ == UseZip64.Dynamic)
        entry.ForceZip64();
      this.WriteLeInt(67324752);
      this.WriteLeShort(entry.Version);
      this.WriteLeShort(entry.Flags);
      this.WriteLeShort((int) (byte) compressionMethod);
      this.WriteLeInt((int) entry.DosTime);
      if (flag)
      {
        this.WriteLeInt((int) entry.Crc);
        if (entry.LocalHeaderRequiresZip64)
        {
          this.WriteLeInt(-1);
          this.WriteLeInt(-1);
        }
        else
        {
          this.WriteLeInt(entry.IsCrypted ? (int) entry.CompressedSize + 12 : (int) entry.CompressedSize);
          this.WriteLeInt((int) entry.Size);
        }
      }
      else
      {
        if (this.patchEntryHeader)
          this.crcPatchPos = this.baseOutputStream_.Position;
        this.WriteLeInt(0);
        if (this.patchEntryHeader)
          this.sizePatchPos = this.baseOutputStream_.Position;
        if (entry.LocalHeaderRequiresZip64 && this.patchEntryHeader)
        {
          this.WriteLeInt(-1);
          this.WriteLeInt(-1);
        }
        else
        {
          this.WriteLeInt(0);
          this.WriteLeInt(0);
        }
      }
      byte[] array = ZipConstants.ConvertToArray(entry.Flags, entry.Name);
      if (array.Length > (int) ushort.MaxValue)
        throw new ZipException("Entry name too long.");
      ZipExtraData zipExtraData = new ZipExtraData(entry.ExtraData);
      if (entry.LocalHeaderRequiresZip64 && (flag || this.patchEntryHeader))
      {
        zipExtraData.StartNewEntry();
        if (flag)
        {
          zipExtraData.AddLeLong(entry.Size);
          zipExtraData.AddLeLong(entry.CompressedSize);
        }
        else
        {
          zipExtraData.AddLeLong(-1L);
          zipExtraData.AddLeLong(-1L);
        }
        zipExtraData.AddNewEntry(1);
        if (!zipExtraData.Find(1))
          throw new ZipException("Internal error cant find extra data");
        if (this.patchEntryHeader)
          this.sizePatchPos = (long) zipExtraData.CurrentReadIndex;
      }
      else
        zipExtraData.Delete(1);
      byte[] entryData = zipExtraData.GetEntryData();
      this.WriteLeShort(array.Length);
      this.WriteLeShort(entryData.Length);
      if (array.Length > 0)
        this.baseOutputStream_.Write(array, 0, array.Length);
      if (entry.LocalHeaderRequiresZip64 && this.patchEntryHeader)
        this.sizePatchPos += this.baseOutputStream_.Position;
      if (entryData.Length > 0)
        this.baseOutputStream_.Write(entryData, 0, entryData.Length);
      this._offset += (long) (30 + array.Length + entryData.Length);
      this.curEntry = entry;
      this.crc.Reset();
      if (compressionMethod == CompressionMethod.Deflated)
      {
        this.deflater_.Reset();
        this.deflater_.SetLevel(level);
      }
      this.size = 0L;
      if (!entry.IsCrypted)
        return;
      if (entry.Crc < 0L)
        this.WriteEncryptionHeader(entry.DosTime << 16);
      else
        this.WriteEncryptionHeader(entry.Crc);
    }

    public void CloseEntry()
    {
      if (this.curEntry == null)
        throw new InvalidOperationException("No open entry");
      if (this.curMethod == CompressionMethod.Deflated)
        base.Finish();
      long num = this.curMethod == CompressionMethod.Deflated ? this.deflater_.TotalOut : this.size;
      if (this.curEntry.Size < 0L)
        this.curEntry.Size = this.size;
      else if (this.curEntry.Size != this.size)
        throw new ZipException("size was " + (object) this.size + ", but I expected " + (object) this.curEntry.Size);
      if (this.curEntry.CompressedSize < 0L)
        this.curEntry.CompressedSize = num;
      else if (this.curEntry.CompressedSize != num)
        throw new ZipException("compressed size was " + (object) num + ", but I expected " + (object) this.curEntry.CompressedSize);
      if (this.curEntry.Crc < 0L)
        this.curEntry.Crc = this.crc.Value;
      else if (this.curEntry.Crc != this.crc.Value)
        throw new ZipException("crc was " + (object) this.crc.Value + ", but I expected " + (object) this.curEntry.Crc);
      this._offset += num;
      if (this.curEntry.IsCrypted)
        this.curEntry.CompressedSize += 12L;
      if (this.patchEntryHeader)
      {
        this.patchEntryHeader = false;
        long position = this.baseOutputStream_.Position;
        this.baseOutputStream_.Seek(this.crcPatchPos, SeekOrigin.Begin);
        this.WriteLeInt((int) this.curEntry.Crc);
        if (this.curEntry.LocalHeaderRequiresZip64)
        {
          if (this.sizePatchPos == -1L)
            throw new ZipException("Entry requires zip64 but this has been turned off");
          this.baseOutputStream_.Seek(this.sizePatchPos, SeekOrigin.Begin);
          this.WriteLeLong(this.curEntry.Size);
          this.WriteLeLong(this.curEntry.CompressedSize);
        }
        else
        {
          this.WriteLeInt((int) this.curEntry.CompressedSize);
          this.WriteLeInt((int) this.curEntry.Size);
        }
        this.baseOutputStream_.Seek(position, SeekOrigin.Begin);
      }
      if ((this.curEntry.Flags & 8) != 0)
      {
        this.WriteLeInt(134695760);
        this.WriteLeInt((int) this.curEntry.Crc);
        if (this.curEntry.LocalHeaderRequiresZip64)
        {
          this.WriteLeLong(this.curEntry.CompressedSize);
          this.WriteLeLong(this.curEntry.Size);
          this._offset += 24L;
        }
        else
        {
          this.WriteLeInt((int) this.curEntry.CompressedSize);
          this.WriteLeInt((int) this.curEntry.Size);
          this._offset += 16L;
        }
      }
      this.entries.Add(this.curEntry);
      this.curEntry = (ZipEntry) null;
    }

    private void WriteEncryptionHeader(long crcValue)
    {
      this._offset += 12L;
      this.InitializePassword(this.Password);
      byte[] buffer = new byte[12];
      new Random().NextBytes(buffer);
      buffer[11] = (byte) (crcValue >> 24);
      this.EncryptBlock(buffer, 0, buffer.Length);
      this.baseOutputStream_.Write(buffer, 0, buffer.Length);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (this.curEntry == null)
        throw new InvalidOperationException("No open entry.");
      if (buffer == null)
        throw new ArgumentNullException(nameof (buffer));
      if (offset < 0)
        throw new ArgumentOutOfRangeException(nameof (offset), "Cannot be negative");
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof (count), "Cannot be negative");
      if (buffer.Length - offset < count)
        throw new ArgumentException("Invalid offset/count combination");
      this.crc.Update(buffer, offset, count);
      this.size += (long) count;
      switch (this.curMethod)
      {
        case CompressionMethod.Stored:
          if (this.Password != null)
          {
            this.CopyAndEncrypt(buffer, offset, count);
            break;
          }
          this.baseOutputStream_.Write(buffer, offset, count);
          break;
        case CompressionMethod.Deflated:
          base.Write(buffer, offset, count);
          break;
      }
    }

    private void CopyAndEncrypt(byte[] buffer, int offset, int count)
    {
      byte[] numArray = new byte[4096];
      while (count > 0)
      {
        int num = count < 4096 ? count : 4096;
        Array.Copy((Array) buffer, offset, (Array) numArray, 0, num);
        this.EncryptBlock(numArray, 0, num);
        this.baseOutputStream_.Write(numArray, 0, num);
        count -= num;
        offset += num;
      }
    }

    public override void Finish()
    {
      if (this.entries == null)
        return;
      if (this.curEntry != null)
        this.CloseEntry();
      long count = (long) this.entries.Count;
      long sizeEntries = 0;
      foreach (ZipEntry entry in this.entries)
      {
        this.WriteLeInt(33639248);
        this.WriteLeShort(45);
        this.WriteLeShort(entry.Version);
        this.WriteLeShort(entry.Flags);
        this.WriteLeShort((int) (short) entry.CompressionMethod);
        this.WriteLeInt((int) entry.DosTime);
        this.WriteLeInt((int) entry.Crc);
        if (entry.IsZip64Forced() || entry.CompressedSize >= (long) uint.MaxValue)
          this.WriteLeInt(-1);
        else
          this.WriteLeInt((int) entry.CompressedSize);
        if (entry.IsZip64Forced() || entry.Size >= (long) uint.MaxValue)
          this.WriteLeInt(-1);
        else
          this.WriteLeInt((int) entry.Size);
        byte[] array = ZipConstants.ConvertToArray(entry.Flags, entry.Name);
        if (array.Length > (int) ushort.MaxValue)
          throw new ZipException("Name too long.");
        ZipExtraData zipExtraData = new ZipExtraData(entry.ExtraData);
        if (entry.CentralHeaderRequiresZip64)
        {
          zipExtraData.StartNewEntry();
          if (entry.IsZip64Forced() || entry.Size >= (long) uint.MaxValue)
            zipExtraData.AddLeLong(entry.Size);
          if (entry.IsZip64Forced() || entry.CompressedSize >= (long) uint.MaxValue)
            zipExtraData.AddLeLong(entry.CompressedSize);
          if (entry.Offset >= (long) uint.MaxValue)
            zipExtraData.AddLeLong(entry.Offset);
          zipExtraData.AddNewEntry(1);
        }
        else
          zipExtraData.Delete(1);
        byte[] entryData = zipExtraData.GetEntryData();
        byte[] buffer = entry.Comment != null ? ZipConstants.ConvertToArray(entry.Flags, entry.Comment) : new byte[0];
        if (buffer.Length > (int) ushort.MaxValue)
          throw new ZipException("Comment too long.");
        this.WriteLeShort(array.Length);
        this.WriteLeShort(entryData.Length);
        this.WriteLeShort(buffer.Length);
        this.WriteLeShort(0);
        this.WriteLeShort(0);
        if (entry.ExternalFileAttributes != -1)
          this.WriteLeInt(entry.ExternalFileAttributes);
        else if (entry.IsDirectory)
          this.WriteLeInt(16);
        else
          this.WriteLeInt(0);
        if (entry.Offset >= (long) uint.MaxValue)
          this.WriteLeInt(-1);
        else
          this.WriteLeInt((int) entry.Offset);
        if (array.Length > 0)
          this.baseOutputStream_.Write(array, 0, array.Length);
        if (entryData.Length > 0)
          this.baseOutputStream_.Write(entryData, 0, entryData.Length);
        if (buffer.Length > 0)
          this.baseOutputStream_.Write(buffer, 0, buffer.Length);
        sizeEntries += (long) (46 + array.Length + entryData.Length + buffer.Length);
      }
      using (ZipHelperStream zipHelperStream = new ZipHelperStream(this.baseOutputStream_))
        zipHelperStream.WriteEndOfCentralDirectory(count, sizeEntries, this._offset, this.zipComment);
      this.entries = (List<ZipEntry>) null;
    }
  }
}
