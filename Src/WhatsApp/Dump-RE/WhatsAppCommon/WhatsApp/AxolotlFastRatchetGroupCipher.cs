// Decompiled with JetBrains decompiler
// Type: WhatsApp.AxolotlFastRatchetGroupCipher
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Runtime.InteropServices;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class AxolotlFastRatchetGroupCipher : IAxolotlSessionCipherCallbacks, IDisposable
  {
    private IAxolotlGroupCipherNative native;
    private Axolotl Axolotl;
    private string GroupId;
    private string SenderKeyId;
    private AxolotlFastRatchetGroupCipher.PayloadDecryptedHandler handler;

    public AxolotlFastRatchetGroupCipher(Axolotl axolotl, string groupId, string senderKeyId)
    {
      this.Axolotl = axolotl;
      this.GroupId = groupId;
      this.SenderKeyId = senderKeyId;
      this.native = (IAxolotlGroupCipherNative) NativeInterfaces.CreateInstance<AxolotlFastRatchetGroupCipherNative>();
      this.native.Initialize(groupId, senderKeyId, (IAxolotlSessionCipherCallbacks) this, axolotl.NativeInterface);
    }

    public void Dispose() => Marshal.ReleaseComObject((object) this.native);

    public void EncryptPayload(byte[] PlainText, out byte[] CipherText)
    {
      IByteBuffer bb = (IByteBuffer) null;
      IByteBuffer instance = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
      instance.Put(PlainText);
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

    public void DecryptPayload(
      byte[] CipherText,
      AxolotlFastRatchetGroupCipher.PayloadDecryptedHandler handler)
    {
      try
      {
        this.handler = handler;
        IByteBuffer instance = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
        instance.Put(CipherText);
        this.native.DecryptMessage(instance);
      }
      catch (Exception ex)
      {
        switch ((Axolotl_Error) ex.GetHResult())
        {
          case Axolotl_Error.AX_ERR_DUPLICATE_MESSAGE:
            Log.l(nameof (DecryptPayload), "Duplicate Message");
            break;
          case Axolotl_Error.AX_ERR_INVALID_MESSAGE:
            Log.l(nameof (DecryptPayload), "Invalid Message");
            throw new Axolotl.DecryptRetryException()
            {
              RetryCount = 0
            };
          case Axolotl_Error.AX_ERR_LEGACY_MESSAGE:
            Log.l(nameof (DecryptPayload), "Legacy Message");
            break;
          case Axolotl_Error.AX_ERR_NO_SESSION:
            Log.l(nameof (DecryptPayload), "No session");
            throw new Axolotl.DecryptRetryException()
            {
              RetryCount = 0
            };
          default:
            Log.SendCrashLog(ex, "DecryptPayload: Unexpected error - HRESULT: " + ex.GetHResult().ToString("X8"));
            break;
        }
      }
    }

    public void DecryptCallback(IByteBuffer PlainTextBuffer)
    {
      byte[] plaintext = PlainTextBuffer.Get();
      if (this.handler == null)
        return;
      this.handler(plaintext);
    }

    public delegate void PayloadDecryptedHandler(byte[] plaintext);
  }
}
