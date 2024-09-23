// Decompiled with JetBrains decompiler
// Type: ZXing.QrCode.Internal.BitMatrixParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using ZXing.Common;

#nullable disable
namespace ZXing.QrCode.Internal
{
  /// <author>Sean Owen</author>
  internal sealed class BitMatrixParser
  {
    private readonly BitMatrix bitMatrix;
    private Version parsedVersion;
    private FormatInformation parsedFormatInfo;
    private bool mirrored;

    /// <param name="bitMatrix">{@link BitMatrix} to parse</param>
    /// <throws>ReaderException if dimension is not &gt;= 21 and 1 mod 4</throws>
    internal static BitMatrixParser createBitMatrixParser(BitMatrix bitMatrix)
    {
      int height = bitMatrix.Height;
      return height < 21 || (height & 3) != 1 ? (BitMatrixParser) null : new BitMatrixParser(bitMatrix);
    }

    private BitMatrixParser(BitMatrix bitMatrix) => this.bitMatrix = bitMatrix;

    /// <summary> <p>Reads format information from one of its two locations within the QR Code.</p>
    /// 
    /// </summary>
    /// <returns>{@link FormatInformation} encapsulating the QR Code's format info</returns>
    /// <throws>  ReaderException if both format information locations cannot be parsed as </throws>
    /// <summary>the valid encoding of format information</summary>
    internal FormatInformation readFormatInformation()
    {
      if (this.parsedFormatInfo != null)
        return this.parsedFormatInfo;
      int versionBits = 0;
      for (int i = 0; i < 6; ++i)
        versionBits = this.copyBit(i, 8, versionBits);
      int num1 = this.copyBit(8, 7, this.copyBit(8, 8, this.copyBit(7, 8, versionBits)));
      for (int j = 5; j >= 0; --j)
        num1 = this.copyBit(8, j, num1);
      int height = this.bitMatrix.Height;
      int num2 = 0;
      int num3 = height - 7;
      for (int j = height - 1; j >= num3; --j)
        num2 = this.copyBit(8, j, num2);
      for (int i = height - 8; i < height; ++i)
        num2 = this.copyBit(i, 8, num2);
      this.parsedFormatInfo = FormatInformation.decodeFormatInformation(num1, num2);
      return this.parsedFormatInfo != null ? this.parsedFormatInfo : (FormatInformation) null;
    }

    /// <summary> <p>Reads version information from one of its two locations within the QR Code.</p>
    /// 
    /// </summary>
    /// <returns>{@link Version} encapsulating the QR Code's version</returns>
    /// <throws>  ReaderException if both version information locations cannot be parsed as </throws>
    /// <summary>the valid encoding of version information</summary>
    internal Version readVersion()
    {
      if (this.parsedVersion != null)
        return this.parsedVersion;
      int height = this.bitMatrix.Height;
      int versionNumber = height - 17 >> 2;
      if (versionNumber <= 6)
        return Version.getVersionForNumber(versionNumber);
      int versionBits1 = 0;
      int num = height - 11;
      for (int j = 5; j >= 0; --j)
      {
        for (int i = height - 9; i >= num; --i)
          versionBits1 = this.copyBit(i, j, versionBits1);
      }
      this.parsedVersion = Version.decodeVersionInformation(versionBits1);
      if (this.parsedVersion != null && this.parsedVersion.DimensionForVersion == height)
        return this.parsedVersion;
      int versionBits2 = 0;
      for (int i = 5; i >= 0; --i)
      {
        for (int j = height - 9; j >= num; --j)
          versionBits2 = this.copyBit(i, j, versionBits2);
      }
      this.parsedVersion = Version.decodeVersionInformation(versionBits2);
      return this.parsedVersion != null && this.parsedVersion.DimensionForVersion == height ? this.parsedVersion : (Version) null;
    }

    private int copyBit(int i, int j, int versionBits)
    {
      return !(this.mirrored ? this.bitMatrix[j, i] : this.bitMatrix[i, j]) ? versionBits << 1 : versionBits << 1 | 1;
    }

    /// <summary> <p>Reads the bits in the {@link BitMatrix} representing the finder pattern in the
    /// correct order in order to reconstruct the codewords bytes contained within the
    /// QR Code.</p>
    /// 
    /// </summary>
    /// <returns>bytes encoded within the QR Code</returns>
    /// <throws>  ReaderException if the exact number of bytes expected is not read </throws>
    internal byte[] readCodewords()
    {
      FormatInformation formatInformation = this.readFormatInformation();
      if (formatInformation == null)
        return (byte[]) null;
      Version version = this.readVersion();
      if (version == null)
        return (byte[]) null;
      DataMask dataMask = DataMask.forReference((int) formatInformation.DataMask);
      int height = this.bitMatrix.Height;
      dataMask.unmaskBitMatrix(this.bitMatrix, height);
      BitMatrix bitMatrix = version.buildFunctionPattern();
      bool flag = true;
      byte[] numArray = new byte[version.TotalCodewords];
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      for (int index1 = height - 1; index1 > 0; index1 -= 2)
      {
        if (index1 == 6)
          --index1;
        for (int index2 = 0; index2 < height; ++index2)
        {
          int y = flag ? height - 1 - index2 : index2;
          for (int index3 = 0; index3 < 2; ++index3)
          {
            if (!bitMatrix[index1 - index3, y])
            {
              ++num3;
              num2 <<= 1;
              if (this.bitMatrix[index1 - index3, y])
                num2 |= 1;
              if (num3 == 8)
              {
                numArray[num1++] = (byte) num2;
                num3 = 0;
                num2 = 0;
              }
            }
          }
        }
        flag = !flag;
      }
      return num1 != version.TotalCodewords ? (byte[]) null : numArray;
    }

    /// Revert the mask removal done while reading the code words. The bit matrix should revert to its original state.
    internal void remask()
    {
      if (this.parsedFormatInfo == null)
        return;
      DataMask.forReference((int) this.parsedFormatInfo.DataMask).unmaskBitMatrix(this.bitMatrix, this.bitMatrix.Height);
    }

    /// Prepare the parser for a mirrored operation.
    ///             This flag has effect only on the {@link #readFormatInformation()} and the
    ///             {@link #readVersion()}. Before proceeding with {@link #readCodewords()} the
    ///             {@link #mirror()} method should be called.
    ///             
    ///             @param mirror Whether to read version and format information mirrored.
    internal void setMirror(bool mirror)
    {
      this.parsedVersion = (Version) null;
      this.parsedFormatInfo = (FormatInformation) null;
      this.mirrored = mirror;
    }

    /// Mirror the bit matrix in order to attempt a second reading.
    internal void mirror()
    {
      for (int index1 = 0; index1 < this.bitMatrix.Width; ++index1)
      {
        for (int index2 = index1 + 1; index2 < this.bitMatrix.Height; ++index2)
        {
          if (this.bitMatrix[index1, index2] != this.bitMatrix[index2, index1])
          {
            this.bitMatrix.flip(index2, index1);
            this.bitMatrix.flip(index1, index2);
          }
        }
      }
    }
  }
}
