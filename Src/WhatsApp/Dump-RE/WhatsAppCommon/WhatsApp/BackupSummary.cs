// Decompiled with JetBrains decompiler
// Type: WhatsApp.BackupSummary
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

#nullable disable
namespace WhatsApp
{
  public class BackupSummary
  {
    public DateTime? Timestamp;
    public List<BackupMediaFile> Databases = new List<BackupMediaFile>();
    public List<BackupMediaFile> MediaFiles = new List<BackupMediaFile>();
    private List<Tuple<string, byte[], long>> dbHashes = new List<Tuple<string, byte[], long>>();
    private const byte Version = 1;

    internal void OnDatabaseHashKnown(string filename, byte[] hash, long size)
    {
      this.dbHashes.Add(Tuple.Create<string, byte[], long>(filename, hash, size));
    }

    internal void OnFinalDirectoryKnown(string dir)
    {
      this.Databases.AddRange(this.dbHashes.Select<Tuple<string, byte[], long>, BackupMediaFile>((Func<Tuple<string, byte[], long>, BackupMediaFile>) (file => new BackupMediaFile()
      {
        FileRef = MediaStorage.AnalyzePath(dir + "\\" + file.Item1),
        Sha1Hash = file.Item2,
        Size = new long?(file.Item3)
      })));
      this.dbHashes.Clear();
    }

    public void Serialize(Action<byte[], int, int> output)
    {
      BackupSummary.Serializer serializer = new BackupSummary.Serializer(output);
      serializer.WriteHeader(this.dbHashes.Count + this.Databases.Count);
      foreach (Tuple<string, byte[], long> dbHash in this.dbHashes)
        serializer.Serialize(Utils.BaseName(dbHash.Item1), dbHash.Item2, dbHash.Item3);
      foreach (BackupMediaFile database in this.Databases)
        serializer.Serialize(database.FileRef.FilePart, database.Sha1Hash, database.Size ?? -1L);
      foreach (BackupMediaFile mediaFile in this.MediaFiles)
        serializer.Serialize(mediaFile.FileRef.ToAbsolutePath(), mediaFile.Sha1Hash, mediaFile.Size ?? -1L);
    }

    public static BackupSummary Deserialize(Stream input)
    {
      BackupSummary backupSummary = new BackupSummary();
      BackupSummary.Deserializer deserializer = new BackupSummary.Deserializer(input);
      bool isDatabase;
      string filename;
      byte[] hash;
      long sz;
      while (deserializer.TryDeserialize(out filename, out hash, out sz, out isDatabase))
      {
        if (isDatabase)
          backupSummary.OnDatabaseHashKnown(filename, hash, sz);
        else
          backupSummary.MediaFiles.Add(new BackupMediaFile()
          {
            FileRef = MediaStorage.AnalyzePath(filename),
            Sha1Hash = hash,
            Size = sz < 0L ? new long?() : new long?(sz)
          });
      }
      return backupSummary;
    }

    public bool ValidateHashes()
    {
      bool flag = true;
      foreach (BackupMediaFile database in this.Databases)
      {
        long? size;
        if (database.Sha1Hash == null)
        {
          size = database.Size;
          long num = 0;
          if ((size.GetValueOrDefault() >= num ? (size.HasValue ? 1 : 0) : 0) == 0)
            continue;
        }
        string str = NativeMediaStorage.MakeUri(database.FileRef.ToAbsolutePath());
        using (IMediaStorage mediaStorage = MediaStorage.Create(str))
        {
          using (Stream inputStream = mediaStorage.OpenFile(str))
          {
            if (database.Sha1Hash != null && !new SHA1Managed().ComputeHash(inputStream).IsEqualBytes(database.Sha1Hash))
              flag = false;
            size = database.Size;
            long num = 0;
            if ((size.GetValueOrDefault() >= num ? (size.HasValue ? 1 : 0) : 0) != 0)
            {
              size = database.Size;
              long length = inputStream.Length;
              if ((size.GetValueOrDefault() == length ? (size.HasValue ? 1 : 0) : 0) == 0)
                flag = false;
            }
          }
        }
      }
      return flag;
    }

    private class Serializer
    {
      private Action<byte[], int, int> write;

      public Serializer(Action<byte[], int, int> write) => this.write = write;

      public void WriteHeader(int dbCount)
      {
        this.Write((byte) 1);
        this.Write((byte) dbCount);
      }

      public void Serialize(string path, byte[] hash, long sz)
      {
        this.Serialize(path);
        string str = "";
        if (hash != null)
          str = Convert.ToBase64String(hash);
        this.Serialize(str);
        this.Serialize(sz.ToString());
      }

      private void Serialize(string str)
      {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        this.Write(bytes, 0, bytes.Length);
        this.Write((byte) 0);
      }

      private void Write(byte[] buffer, int offset, int len) => this.write(buffer, offset, len);

      private void Write(byte b)
      {
        this.Write(new byte[1]{ b }, 0, 1);
      }
    }

    private class Deserializer
    {
      private byte[] buffer = new byte[65536];
      private int currentLen;
      private int currentOff;
      private Stream source;
      private int dbCount;
      private long idx;

      public Deserializer(Stream source)
      {
        this.source = source;
        this.dbCount = source.ReadByte() == 1 ? source.ReadByte() : throw new IOException("Invalid version or unexpected EOF");
        if (this.dbCount < 0)
          throw new IOException("Unexpected EOF");
      }

      public bool TryDeserialize(
        out string filename,
        out byte[] hash,
        out long sz,
        out bool isDatabase)
      {
        filename = (string) null;
        hash = (byte[]) null;
        sz = -1L;
        isDatabase = this.idx++ < (long) this.dbCount;
        string str = this.ReadString();
        if (str == null)
          return false;
        filename = str;
        string s = this.ReadString();
        switch (s)
        {
          case null:
            throw new IOException("Unexpected EOF");
          case "":
            long.TryParse(this.ReadString() ?? throw new IOException("Unexpected EOF"), out sz);
            return true;
          default:
            hash = Convert.FromBase64String(s);
            goto case "";
        }
      }

      private bool TryReadByte(out byte b)
      {
        if (this.currentOff == this.currentLen)
        {
          this.currentOff = 0;
          this.currentLen = this.source.Read(this.buffer, 0, this.buffer.Length);
          if (this.currentLen == 0)
          {
            b = (byte) 0;
            return false;
          }
        }
        b = this.buffer[this.currentOff++];
        return true;
      }

      private string ReadString()
      {
        List<byte> byteList = new List<byte>();
        byte b = 0;
        bool flag = true;
        while (this.TryReadByte(out b))
        {
          if (b == (byte) 0)
          {
            flag = false;
            break;
          }
          byteList.Add(b);
        }
        if (!flag)
          return Encoding.UTF8.GetString(byteList.ToArray(), 0, byteList.Count);
        if (byteList.Count != 0)
          throw new IOException("Unexpected EOF");
        return (string) null;
      }
    }
  }
}
