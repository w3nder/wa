// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Zip.Compression.Streams.DeflaterOutputStream
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using ICSharpCode.SharpZipLib.Silverlight.Encryption;
using System;
using System.IO;
using System.Security.Cryptography;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Zip.Compression.Streams
{
  public class DeflaterOutputStream : Stream
  {
    private ICryptoTransform cryptoTransform_;
    private string _password;
    private readonly byte[] buffer_;
    protected Stream baseOutputStream_;
    protected Deflater deflater_;
    private bool isClosed_;
    private bool isStreamOwner_ = true;

    public DeflaterOutputStream(Stream baseOutputStream)
      : this(baseOutputStream, new Deflater(), 512)
    {
    }

    public DeflaterOutputStream(Stream baseOutputStream, Deflater deflater)
      : this(baseOutputStream, deflater, 512)
    {
    }

    public DeflaterOutputStream(Stream baseOutputStream, Deflater deflater, int bufferSize)
    {
      if (baseOutputStream == null)
        throw new ArgumentNullException(nameof (baseOutputStream));
      if (!baseOutputStream.CanWrite)
        throw new ArgumentException("Must support writing", nameof (baseOutputStream));
      if (deflater == null)
        throw new ArgumentNullException(nameof (deflater));
      if (bufferSize <= 0)
        throw new ArgumentOutOfRangeException(nameof (bufferSize));
      this.baseOutputStream_ = baseOutputStream;
      this.buffer_ = new byte[bufferSize];
      this.deflater_ = deflater;
    }

    public bool IsStreamOwner
    {
      get => this.isStreamOwner_;
      set => this.isStreamOwner_ = value;
    }

    public bool CanPatchEntries => this.baseOutputStream_.CanSeek;

    public virtual void Finish()
    {
      this.deflater_.Finish();
      while (!this.deflater_.IsFinished)
      {
        int num = this.deflater_.Deflate(this.buffer_, 0, this.buffer_.Length);
        if (num > 0)
        {
          if (this.cryptoTransform_ != null)
            this.EncryptBlock(this.buffer_, 0, num);
          this.baseOutputStream_.Write(this.buffer_, 0, num);
        }
        else
          break;
      }
      if (!this.deflater_.IsFinished)
        throw new SharpZipBaseException("Can't deflate all input?");
      this.baseOutputStream_.Flush();
      if (this.cryptoTransform_ == null)
        return;
      this.cryptoTransform_.Dispose();
      this.cryptoTransform_ = (ICryptoTransform) null;
    }

    public string Password
    {
      get => this._password;
      set
      {
        if (value != null && value.Length == 0)
          this._password = (string) null;
        else
          this._password = value;
      }
    }

    protected void EncryptBlock(byte[] buffer, int offset, int length)
    {
      this.cryptoTransform_.TransformBlock(buffer, 0, length, buffer, 0);
    }

    protected void InitializePassword(string password)
    {
      this.cryptoTransform_ = new PkzipClassicManaged().CreateEncryptor(PkzipClassic.GenerateKeys(ZipConstants.ConvertToArray(password)), (byte[]) null);
    }

    protected void Deflate()
    {
      while (!this.deflater_.IsNeedingInput)
      {
        int num = this.deflater_.Deflate(this.buffer_, 0, this.buffer_.Length);
        if (num > 0)
        {
          if (this.cryptoTransform_ != null)
            this.EncryptBlock(this.buffer_, 0, num);
          this.baseOutputStream_.Write(this.buffer_, 0, num);
        }
        else
          break;
      }
      if (!this.deflater_.IsNeedingInput)
        throw new SharpZipBaseException("DeflaterOutputStream can't deflate all input?");
    }

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => this.baseOutputStream_.CanWrite;

    public override long Length => this.baseOutputStream_.Length;

    public override long Position
    {
      get => this.baseOutputStream_.Position;
      set => throw new NotSupportedException("Position property not supported");
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotSupportedException("DeflaterOutputStream Seek not supported");
    }

    public override void SetLength(long value)
    {
      throw new NotSupportedException("DeflaterOutputStream SetLength not supported");
    }

    public override int ReadByte()
    {
      throw new NotSupportedException("DeflaterOutputStream ReadByte not supported");
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException("DeflaterOutputStream Read not supported");
    }

    public override IAsyncResult BeginRead(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      throw new NotSupportedException("DeflaterOutputStream BeginRead not currently supported");
    }

    public override IAsyncResult BeginWrite(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      throw new NotSupportedException("BeginWrite is not supported");
    }

    public override void Flush()
    {
      this.deflater_.Flush();
      this.Deflate();
      this.baseOutputStream_.Flush();
    }

    public override void Close()
    {
      if (this.isClosed_)
        return;
      this.isClosed_ = true;
      try
      {
        this.Finish();
        if (this.cryptoTransform_ == null)
          return;
        this.cryptoTransform_.Dispose();
        this.cryptoTransform_ = (ICryptoTransform) null;
      }
      finally
      {
        if (this.isStreamOwner_)
          this.baseOutputStream_.Close();
      }
    }

    public override void WriteByte(byte value)
    {
      this.Write(new byte[1]{ value }, 0, 1);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      this.deflater_.SetInput(buffer, offset, count);
      this.Deflate();
    }
  }
}
