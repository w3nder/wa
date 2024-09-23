// Decompiled with JetBrains decompiler
// Type: WhatsApp.BuildHash
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
  public class BuildHash
  {
    public static byte[] Create(Stream file)
    {
      BuildHash.RawSha1 rawSha1 = new BuildHash.RawSha1();
      byte[] numArray = new byte[4096];
      Dictionary<string, string> interestingSections = ((IEnumerable<string>) new string[1]
      {
        ".rsrc"
      }).ToDictionary<string, string>((Func<string, string>) (k => k));
      foreach (BuildHash.SectionHeader sectionHeader in BuildHash.ParsePeImage(file).Where<BuildHash.SectionHeader>((Func<BuildHash.SectionHeader, bool>) (s => interestingSections.ContainsKey(s.Name))))
      {
        file.Seek(sectionHeader.Start, SeekOrigin.Begin);
        int num;
        for (long length = sectionHeader.Length; length != 0L; length -= (long) num)
        {
          num = (int) Math.Min(length, (long) numArray.Length);
          file.Read(numArray, 0, num);
          rawSha1.AddBytes(numArray, 0, num);
        }
      }
      return rawSha1.GetHash();
    }

    public static IEnumerable<BuildHash.SectionHeader> ParsePeImage(
      Stream file,
      BuildHash.AdditionalDetails details = null)
    {
      byte[] numArray1 = new byte[4]
      {
        (byte) 77,
        (byte) 90,
        (byte) 144,
        (byte) 0
      };
      byte[] numArray2 = new byte[64];
      file.Read(numArray2, 0, numArray2.Length);
      uint offset = BuildHash.Read32(numArray2, 60);
      file.Seek((long) offset, SeekOrigin.Begin);
      for (int index = 0; index < numArray1.Length; ++index)
      {
        if ((int) numArray1[index] != (int) numArray2[index])
          throw new Exception("DOS header mismatch");
      }
      byte[] numArray3 = new byte[4]
      {
        (byte) 80,
        (byte) 69,
        (byte) 0,
        (byte) 0
      };
      byte[] numArray4 = new byte[24];
      file.Read(numArray4, 0, numArray4.Length);
      for (int index = 0; index < numArray3.Length; ++index)
      {
        if ((int) numArray3[index] != (int) numArray4[index])
          throw new Exception("PE header mismatch");
      }
      byte[] numArray5 = new byte[(int) BuildHash.Read16(numArray4, 20)];
      file.Read(numArray5, 0, numArray5.Length);
      if (details != null)
        details.Timestamp = BuildHash.Read32(numArray4, 8);
      ushort num = BuildHash.Read16(numArray4, 6);
      uint imageBase = BuildHash.Read32(numArray5, 28);
      List<BuildHash.SectionHeader> peImage = new List<BuildHash.SectionHeader>();
      for (int index = 0; index < (int) num; ++index)
        peImage.Add(BuildHash.ReadSectionHeader(file, imageBase));
      return (IEnumerable<BuildHash.SectionHeader>) peImage;
    }

    private static BuildHash.SectionHeader ReadSectionHeader(Stream file, uint imageBase)
    {
      BuildHash.SectionHeader sectionHeader = new BuildHash.SectionHeader();
      byte[] numArray = new byte[40];
      file.Read(numArray, 0, numArray.Length);
      StringBuilder stringBuilder = new StringBuilder(8);
      for (int index = 0; index < 8; ++index)
      {
        char ch = (char) numArray[index];
        if (ch != char.MinValue)
          stringBuilder.Append(ch);
        else
          break;
      }
      sectionHeader.Name = stringBuilder.ToString();
      sectionHeader.VirtualAddress = (long) (BuildHash.Read32(numArray, 12) + imageBase);
      sectionHeader.Length = (long) Math.Min(BuildHash.Read32(numArray, 16), BuildHash.Read32(numArray, 8));
      sectionHeader.Start = (long) BuildHash.Read32(numArray, 20);
      return sectionHeader;
    }

    private static ushort Read16(byte[] buf, int offset)
    {
      return (ushort) ((uint) buf[offset] | (uint) buf[offset + 1] << 8);
    }

    private static uint Read32(byte[] buf, int offset)
    {
      return (uint) ((int) buf[offset] | (int) buf[offset + 1] << 8 | (int) buf[offset + 2] << 16 | (int) buf[offset + 3] << 24);
    }

    public class SectionHeader
    {
      public string Name;
      public long Start;
      public long Length;
      public long VirtualAddress;
    }

    public class AdditionalDetails
    {
      public uint Timestamp;
    }

    public class RawSha1 : SHA1Managed
    {
      public void AddBytes(byte[] b, int offset, int length) => this.HashCore(b, offset, length);

      public byte[] GetHash() => this.HashFinal();
    }
  }
}
