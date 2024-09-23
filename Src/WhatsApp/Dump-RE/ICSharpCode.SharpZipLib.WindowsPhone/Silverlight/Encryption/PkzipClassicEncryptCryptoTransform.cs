// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Encryption.PkzipClassicEncryptCryptoTransform
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System;
using System.Security.Cryptography;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Encryption
{
  internal class PkzipClassicEncryptCryptoTransform : 
    PkzipClassicCryptoBase,
    ICryptoTransform,
    IDisposable
  {
    internal PkzipClassicEncryptCryptoTransform(byte[] keyBlock) => this.SetKeys(keyBlock);

    public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
    {
      byte[] outputBuffer = new byte[inputCount];
      this.TransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, 0);
      return outputBuffer;
    }

    public int TransformBlock(
      byte[] inputBuffer,
      int inputOffset,
      int inputCount,
      byte[] outputBuffer,
      int outputOffset)
    {
      for (int index = inputOffset; index < inputOffset + inputCount; ++index)
      {
        byte ch = inputBuffer[index];
        outputBuffer[outputOffset++] = (byte) ((uint) inputBuffer[index] ^ (uint) this.TransformByte());
        this.UpdateKeys(ch);
      }
      return inputCount;
    }

    public bool CanReuseTransform => true;

    public int InputBlockSize => 1;

    public int OutputBlockSize => 1;

    public bool CanTransformMultipleBlocks => true;

    public void Dispose() => this.Reset();
  }
}
