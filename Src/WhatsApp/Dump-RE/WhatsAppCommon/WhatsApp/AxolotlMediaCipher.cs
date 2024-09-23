// Decompiled with JetBrains decompiler
// Type: WhatsApp.AxolotlMediaCipher
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
  public class AxolotlMediaCipher : IDisposable
  {
    public static byte[] ImageHKDFInfo = Encoding.UTF8.GetBytes("WhatsApp Image Keys");
    public static byte[] AudioHKDFInfo = Encoding.UTF8.GetBytes("WhatsApp Audio Keys");
    public static byte[] VideoHKDFInfo = Encoding.UTF8.GetBytes("WhatsApp Video Keys");
    public static byte[] DocumentHKDFInfo = Encoding.UTF8.GetBytes("WhatsApp Document Keys");
    public static byte[] CallHKDFInfo = Encoding.UTF8.GetBytes("WhatsApp VoIP Keys");
    public static byte[] StickerHKDFInfo = Encoding.UTF8.GetBytes("WhatsApp Image Keys");
    private const string LogHeader = "E2EMedia";
    public const int MediaCipherBlockSize = 16;
    public const int MediaCipherMacEmbedSize = 10;
    public const int MediaKeyLength = 32;
    private IEnumerable<string> participantJids;
    private Message message;
    private byte[] hkdf;
    private bool sideCarWriterRequired;
    private byte[] InitializationVector = new byte[16];
    private byte[] CipherKey = new byte[32];
    private byte[] MacKey = new byte[32];
    public byte[] RefKey = new byte[32];
    private SHA256Managed SHA256Hash;
    private HMACSHA256 HmacVerifier;
    private AesManaged AesManaged;
    private ICryptoTransform CryptoTransform;
    private AxolotlMediaCipher.AxolotlMediaRefGenerator RefGenerator;
    private SHA256Managed PlaintextHashGenerator;
    private SidecarWriter SidecarWriter;
    private byte[] downloadSidecar;
    private bool HashGenerated;

    public int InputBlockSize => this.CryptoTransform.InputBlockSize;

    public static void ClearAxolotlMedia(MessagesContext db, Message message)
    {
      message.MediaKey = (byte[]) null;
      message.MediaUploadUrl = (string) null;
      message.ClearCipherMediaHash(db);
    }

    public byte[] UploadHash => StreamingUploadContext.GenerateCustomHash();

    public byte[] PlaintextHash
    {
      get => this.PlaintextHashGenerator == null ? (byte[]) null : this.PlaintextHashGenerator.Hash;
    }

    public byte[] Sidecar => this.downloadSidecar ?? this.SidecarWriter?.Value ?? (byte[]) null;

    public SidecarVerifier SidecarVerifier { get; private set; }

    public string MediaResumeUrl
    {
      get => this.message?.MediaUploadUrl;
      set
      {
        if (this.message == null)
          throw new InvalidOperationException("Can't set resume url for non message media");
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          this.message.MediaUploadUrl = value;
          db.SubmitChanges();
        }));
      }
    }

    public IEnumerable<string> Participants => this.participantJids;

    public static AxolotlMediaCipher CreateUploadCipher(
      Message message,
      Axolotl encryption,
      bool retryFromWeb)
    {
      AxolotlMediaCipher uploadCipher = (AxolotlMediaCipher) null;
      Axolotl.MessageParticipants messageParticipants;
      if (!retryFromWeb)
        messageParticipants = Axolotl.GetMessageParticipants(message);
      else
        messageParticipants = new Axolotl.MessageParticipants()
        {
          IsHashCurrent = true,
          Participants = new string[1]{ Settings.MyJid },
          ParticipantsNeedingWelcome = new GroupParticipantState[0]
        };
      if (encryption.ShouldEncryptMessage(message) == ShouldEncryptResult.Encrypted)
      {
        if (message.MediaKey == null)
        {
          Log.l("E2EMedia", "Creating MediaKey");
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            message.MediaKey = Axolotl.GenerateRandomBytes(32);
            message.MediaUploadUrl = (string) null;
            db.SubmitChanges();
          }));
        }
        uploadCipher = new AxolotlMediaCipher(message, (IEnumerable<string>) messageParticipants.Participants, message.MediaWaType, message.MediaKey, message.KeyFromMe);
      }
      else if (message.MediaKey != null)
      {
        Log.l("E2EMedia", "Clearing MediaKey");
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          AxolotlMediaCipher.ClearAxolotlMedia(db, message);
          db.SubmitChanges();
        }));
      }
      return uploadCipher;
    }

    public static AxolotlMediaCipher CreateDownloadCipher(Message message)
    {
      return new AxolotlMediaCipher(message, (IEnumerable<string>) null, message.MediaWaType, message.MediaKey, message.KeyFromMe, message.InternalProperties?.MediaPropertiesField?.Sidecar);
    }

    public static AxolotlMediaCipher CreateDownloadCipher(
      FunXMPP.FMessage.Type mediaType,
      byte[] mediaKey,
      byte[] downloadSidecar = null)
    {
      return new AxolotlMediaCipher((Message) null, (IEnumerable<string>) null, mediaType, mediaKey, false, downloadSidecar);
    }

    private AxolotlMediaCipher(
      Message message,
      IEnumerable<string> participantJids,
      FunXMPP.FMessage.Type mediaType,
      byte[] mediaKey,
      bool fromMe,
      byte[] downloadSidecar = null)
    {
      this.message = message;
      this.participantJids = participantJids;
      bool flag = false;
      byte[] info;
      switch (mediaType)
      {
        case FunXMPP.FMessage.Type.Image:
          info = AxolotlMediaCipher.ImageHKDFInfo;
          break;
        case FunXMPP.FMessage.Type.Audio:
          info = AxolotlMediaCipher.AudioHKDFInfo;
          flag = true;
          break;
        case FunXMPP.FMessage.Type.Video:
        case FunXMPP.FMessage.Type.Gif:
          info = AxolotlMediaCipher.VideoHKDFInfo;
          flag = true;
          break;
        case FunXMPP.FMessage.Type.Document:
          info = AxolotlMediaCipher.DocumentHKDFInfo;
          break;
        case FunXMPP.FMessage.Type.Sticker:
          info = AxolotlMediaCipher.StickerHKDFInfo;
          break;
        default:
          throw new NotSupportedException("unsupported media type " + message.MediaWaType.ToString());
      }
      this.hkdf = HkdfSha256.Perform(112, mediaKey, info: info);
      if (downloadSidecar != null && downloadSidecar.Length != 0)
        this.downloadSidecar = downloadSidecar;
      this.sideCarWriterRequired = flag & fromMe;
      this.ResetToInitialState();
    }

    public void ResetToInitialState()
    {
      this.AesManaged.SafeDispose();
      this.HmacVerifier.SafeDispose();
      this.SHA256Hash.SafeDispose();
      this.HashGenerated = false;
      Array.Copy((Array) this.hkdf, 0, (Array) this.InitializationVector, 0, this.InitializationVector.Length);
      Array.Copy((Array) this.hkdf, this.InitializationVector.Length, (Array) this.CipherKey, 0, this.CipherKey.Length);
      Array.Copy((Array) this.hkdf, this.InitializationVector.Length + this.CipherKey.Length, (Array) this.MacKey, 0, this.MacKey.Length);
      Array.Copy((Array) this.hkdf, this.InitializationVector.Length + this.CipherKey.Length + this.MacKey.Length, (Array) this.RefKey, 0, this.RefKey.Length);
      this.AesManaged = new AesManaged();
      this.HmacVerifier = new HMACSHA256(this.MacKey);
      this.HmacVerifier.TransformBlock(this.InitializationVector, 0, this.InitializationVector.Length, this.InitializationVector, 0);
      this.SHA256Hash = new SHA256Managed();
      if (this.sideCarWriterRequired)
        this.SidecarWriter = new SidecarWriter(this.MacKey, this.InitializationVector);
      byte[] downloadSidecar = this.downloadSidecar;
      if ((downloadSidecar != null ? downloadSidecar.Length : 0) != 0)
        this.SidecarVerifier = new SidecarVerifier(this.MacKey, this.InitializationVector, this.downloadSidecar);
      if (this.PlaintextHashGenerator == null)
        return;
      this.PlaintextHashGenerator.SafeDispose();
      this.PlaintextHashGenerator = new SHA256Managed();
    }

    public void Dispose()
    {
      this.SHA256Hash.SafeDispose();
      this.HmacVerifier.SafeDispose();
      this.AesManaged.SafeDispose();
      this.CryptoTransform.SafeDispose();
      this.RefGenerator.SafeDispose();
      this.PlaintextHashGenerator.SafeDispose();
    }

    public void EnsureCrypto()
    {
      if (this.CryptoTransform == null)
        this.CryptoTransform = this.AesManaged.CreateEncryptor(this.CipherKey, this.InitializationVector);
      if (this.PlaintextHashGenerator != null)
        return;
      this.PlaintextHashGenerator = new SHA256Managed();
    }

    public int EncryptMedia(byte[] plaintextBlock, byte[] cipherBlock)
    {
      this.EnsureCrypto();
      int length = plaintextBlock.Length;
      this.PlaintextHashGenerator.TransformBlock(plaintextBlock, 0, length, plaintextBlock, 0);
      int num = this.CryptoTransform.TransformBlock(plaintextBlock, 0, length, cipherBlock, 0);
      this.HmacVerifier.TransformBlock(cipherBlock, 0, num, cipherBlock, 0);
      this.SHA256Hash.TransformBlock(cipherBlock, 0, num, cipherBlock, 0);
      this.SidecarWriter?.OnBytesIn(cipherBlock, 0, num);
      return num;
    }

    public byte[] EncryptMediaFinal(byte[] plaintextBlock, int length)
    {
      this.EnsureCrypto();
      this.PlaintextHashGenerator.TransformFinalBlock(plaintextBlock, 0, length);
      byte[] numArray1 = this.CryptoTransform.TransformFinalBlock(plaintextBlock, 0, length);
      this.HmacVerifier.TransformFinalBlock(numArray1, 0, numArray1.Length);
      this.SHA256Hash.TransformBlock(numArray1, 0, numArray1.Length, numArray1, 0);
      byte[] hash = this.HmacVerifier.Hash;
      this.SHA256Hash.TransformFinalBlock(hash, 0, 10);
      byte[] numArray2 = new byte[numArray1.Length + 10];
      Array.Copy((Array) numArray1, (Array) numArray2, numArray1.Length);
      Array.Copy((Array) hash, 0, (Array) numArray2, numArray1.Length, 10);
      this.HashGenerated = true;
      this.SidecarWriter?.OnBytesIn(numArray2, 0, numArray2.Length);
      return numArray2;
    }

    public byte[] CipherMediaHash => this.HashGenerated ? this.SHA256Hash.Hash : (byte[]) null;

    public string GetFileExtensionForMessage(Message message)
    {
      return (message.MediaWaType != FunXMPP.FMessage.Type.Document ? Utils.InferFileExtensionFromMimeType(message.MediaMimeType) : new DocumentMessageWrapper(message).GetFileExtension()) ?? "dat";
    }

    public void DecryptMedia(Message message, Stream src, Stream dest, WhatsApp.Events.MediaDownload fsEvent)
    {
      this.DecryptMedia(message, new Func<byte[], int, int, int>(src.Read), (Func<long>) (() => src.Position), src.Length, new Action<byte[], int, int>(dest.Write), fsEvent);
    }

    public void DecryptMedia(
      Stream src,
      Stream dest,
      byte[] mediaPlainTextHash,
      WhatsApp.Events.MediaDownload fsEvent)
    {
      this.DecryptMedia(new Func<byte[], int, int, int>(src.Read), (Func<long>) (() => src.Position), src.Length, new Action<byte[], int, int>(dest.Write), mediaPlainTextHash, fsEvent);
    }

    public void DecryptMedia(
      Message message,
      Func<byte[], int, int, int> srcRead,
      Func<long> srcGetPosition,
      long srcLength,
      Action<byte[], int, int> destWrite,
      WhatsApp.Events.MediaDownload fsEvent)
    {
      this.DecryptMedia(srcRead, srcGetPosition, srcLength, destWrite, message.MediaHash, fsEvent);
    }

    private void DecryptMedia(
      Func<byte[], int, int, int> srcRead,
      Func<long> srcGetPosition,
      long srcLength,
      Action<byte[], int, int> destWrite,
      byte[] mediaPlainTextHash,
      WhatsApp.Events.MediaDownload fsEvent)
    {
      using (ICryptoTransform decryptor = this.AesManaged.CreateDecryptor(this.CipherKey, this.InitializationVector))
      {
        using (SHA256Managed shA256Managed = new SHA256Managed())
        {
          byte[] numArray1 = new byte[16];
          byte[] numArray2 = new byte[16];
          while (srcGetPosition() < srcLength - 10L)
          {
            if (srcRead(numArray1, 0, 16) != 16)
              throw new IOException("Short read - expected entire block");
            if (srcGetPosition() == srcLength - 10L)
            {
              this.SHA256Hash.TransformBlock(numArray1, 0, 16, numArray1, 0);
              this.HmacVerifier.TransformFinalBlock(numArray1, 0, 16);
              byte[] inputBuffer = decryptor.TransformFinalBlock(numArray1, 0, 16);
              destWrite(inputBuffer, 0, inputBuffer.Length);
              shA256Managed.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
            }
            else
            {
              this.SHA256Hash.TransformBlock(numArray1, 0, 16, numArray1, 0);
              this.HmacVerifier.TransformBlock(numArray1, 0, 16, numArray1, 0);
              int inputCount = decryptor.TransformBlock(numArray1, 0, 16, numArray2, 0);
              destWrite(numArray2, 0, inputCount);
              shA256Managed.TransformBlock(numArray2, 0, inputCount, numArray2, 0);
            }
          }
          byte[] inputBuffer1 = new byte[10];
          if (srcRead(inputBuffer1, 0, 10) != 10)
            throw new IOException("Short read - expected hmac");
          this.SHA256Hash.TransformFinalBlock(inputBuffer1, 0, 10);
          this.HashGenerated = true;
          byte[] hash = this.HmacVerifier.Hash;
          for (int index = 0; index < 10; ++index)
          {
            if ((int) hash[index] != (int) inputBuffer1[index])
              throw new Axolotl.AxolotlMediaDecryptException();
          }
          if (mediaPlainTextHash == null || shA256Managed.Hash.IsEqualBytes(mediaPlainTextHash))
            return;
          fsEvent.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.ERROR_HASH_MISMATCH);
        }
      }
    }

    public string GenerateMediaRef(string jid)
    {
      if (this.RefGenerator == null)
        this.RefGenerator = new AxolotlMediaCipher.AxolotlMediaRefGenerator(this.RefKey);
      return this.RefGenerator.GenerateRef(jid);
    }

    public string GenerateUploadRefs(bool webRetry = false)
    {
      if (this.message != null && this.message.IsStatus())
        return "statusRef";
      using (AxolotlMediaCipher.AxolotlMediaRefGenerator mediaRefGenerator = new AxolotlMediaCipher.AxolotlMediaRefGenerator(this.RefKey))
      {
        StringBuilder stringBuilder = new StringBuilder();
        if (!webRetry && this.Participants != null && this.Participants.Any<string>())
        {
          foreach (string participant in this.Participants)
          {
            stringBuilder.Append(mediaRefGenerator.GenerateRef(participant));
            stringBuilder.Append("\r\n");
          }
        }
        else
        {
          stringBuilder.Append(mediaRefGenerator.GenerateRef(Settings.MyJid));
          stringBuilder.Append("\r\n");
        }
        return stringBuilder.ToString();
      }
    }

    private class AxolotlMediaRefGenerator : IDisposable
    {
      private HMACSHA256 HmacGenerator;

      public AxolotlMediaRefGenerator(byte[] refKey) => this.HmacGenerator = new HMACSHA256(refKey);

      public string GenerateRef(string jid)
      {
        return Convert.ToBase64String(this.HmacGenerator.ComputeHash(Encoding.UTF8.GetBytes(jid)), 0, 20).ToUrlSafeBase64String();
      }

      public void Dispose() => this.HmacGenerator.SafeDispose();
    }
  }
}
