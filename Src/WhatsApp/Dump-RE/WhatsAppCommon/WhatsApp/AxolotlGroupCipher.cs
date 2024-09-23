// Decompiled with JetBrains decompiler
// Type: WhatsApp.AxolotlGroupCipher
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Runtime.InteropServices;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class AxolotlGroupCipher : IAxolotlSessionCipherCallbacks, IDisposable
  {
    private IAxolotlGroupCipherNative native;
    private MessageDecryptionState DecryptionState = MessageDecryptionState.Success;
    private uint[] UnknownTags;
    private Axolotl Axolotl;
    private string GroupId;
    private string SenderKeyId;
    private FunXMPP.FMessage Message;

    public AxolotlGroupCipher(Axolotl axolotl, string groupId, string senderKeyId)
    {
      this.Axolotl = axolotl;
      this.GroupId = groupId;
      this.SenderKeyId = senderKeyId;
      this.native = (IAxolotlGroupCipherNative) NativeInterfaces.CreateInstance<AxolotlGroupCipherNative>();
      this.native.Initialize(groupId, senderKeyId, (IAxolotlSessionCipherCallbacks) this, axolotl.NativeInterface);
    }

    public void Dispose() => Marshal.ReleaseComObject((object) this.native);

    public void EncryptMessage(FunXMPP.FMessage message, out byte[] CipherText)
    {
      byte[] plainText = WhatsApp.ProtoBuf.Message.CreateFromFMessage(message, new CipherTextIncludes(true)).ToPlainText();
      IByteBuffer bb = (IByteBuffer) null;
      IByteBuffer instance = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
      instance.Put(plainText);
      try
      {
        bb = this.native.EncryptMessage(instance);
        CipherText = bb.Get();
      }
      finally
      {
        bb?.Reset();
      }
    }

    public void DecryptMessage(FunXMPP.FMessage message, byte[] CipherText)
    {
      this.Message = message;
      IByteBuffer instance = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
      instance.Put(CipherText);
      this.native.DecryptMessage(instance);
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
      try
      {
        this.Axolotl.ProcessPlainTextToProtocolBufferMessage(this.SenderKeyId, PlainTextBuffer.Get(), this.Message);
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
  }
}
