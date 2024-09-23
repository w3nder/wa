// Decompiled with JetBrains decompiler
// Type: WhatsApp.AxolotlSessionCipher
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Runtime.InteropServices;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class AxolotlSessionCipher : IAxolotlSessionCipherCallbacks, IDisposable
  {
    private IAxolotlSessionCipherNative native;
    private MessageDecryptionState DecryptionState = MessageDecryptionState.Success;
    private uint[] UnknownTags;
    private Axolotl Axolotl;
    private string RecipientId;
    private FunXMPP.FMessage Message;

    public event AxolotlSessionCipher.PayloadDecryptedHandler PayloadDecrypted;

    public AxolotlSessionCipher(Axolotl axolotl, string recipientId)
    {
      this.Axolotl = axolotl;
      this.RecipientId = recipientId;
      this.native = (IAxolotlSessionCipherNative) NativeInterfaces.CreateInstance<AxolotlSessionCipherNative>();
      this.native.Initialize(recipientId, (IAxolotlSessionCipherCallbacks) this, axolotl.NativeInterface);
    }

    public void Dispose() => Marshal.ReleaseComObject((object) this.native);

    public void Encrypt(
      byte[] PlainText,
      out AxolotlCiphertextType CipherTextType,
      out byte[] CipherText)
    {
      IByteBuffer CipherText1 = (IByteBuffer) null;
      IByteBuffer instance = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
      instance.Put(PlainText);
      try
      {
        int CipherTextType1;
        this.native.EncryptMessage(instance, out CipherTextType1, out CipherText1);
        CipherTextType = (AxolotlCiphertextType) CipherTextType1;
        CipherText = CipherText1.Get();
      }
      finally
      {
        CipherText1?.Reset();
      }
    }

    public void DecryptMessage(
      FunXMPP.FMessage message,
      byte[] CipherText,
      bool preKeyMessage,
      AxolotlSessionCipher.PayloadDecryptedHandler handler)
    {
      this.Message = message;
      this.Decrypt(CipherText, preKeyMessage, handler);
    }

    public void Decrypt(
      byte[] CipherText,
      bool preKeyMessage,
      AxolotlSessionCipher.PayloadDecryptedHandler handler)
    {
      this.PayloadDecrypted += handler;
      try
      {
        IByteBuffer instance = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
        instance.Put(CipherText);
        if (preKeyMessage)
          this.native.DecryptPreKeyMessage(instance);
        else
          this.native.DecryptMessage(instance);
      }
      finally
      {
        this.PayloadDecrypted -= handler;
      }
      switch (this.DecryptionState)
      {
        case MessageDecryptionState.MultipleUnknownTags:
          throw new Axolotl.DecryptUnknownTagsException()
          {
            UnknownTags = this.UnknownTags
          };
        case MessageDecryptionState.InvalidProtocolBuffer:
          throw new Axolotl.DecryptInvalidProtocolBufferException();
      }
    }

    public void DecryptCallback(IByteBuffer PlainTextBuffer)
    {
      AxolotlSessionCipher.PayloadDecryptedHandler payloadDecrypted = this.PayloadDecrypted;
      if (payloadDecrypted == null)
        return;
      try
      {
        byte[] plainText = PlainTextBuffer.Get();
        payloadDecrypted(this.RecipientId, plainText, this.Message);
      }
      catch (Axolotl.DecryptUnknownTagsException ex)
      {
        this.DecryptionState = MessageDecryptionState.MultipleUnknownTags;
        this.UnknownTags = ex.UnknownTags;
      }
      catch (Axolotl.DecryptInvalidProtocolBufferException ex)
      {
        this.DecryptionState = MessageDecryptionState.InvalidProtocolBuffer;
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Unexpected Exception from DecryptionHandler");
        throw;
      }
    }

    public delegate void PayloadDecryptedHandler(
      string recipientId,
      byte[] plainText,
      FunXMPP.FMessage message);
  }
}
