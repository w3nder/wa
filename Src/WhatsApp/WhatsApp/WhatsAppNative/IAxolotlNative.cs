// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IAxolotlNative
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(994805063, 57178, 20382, 149, 164, 56, 166, 34, 48, 59, 243)]
  public interface IAxolotlNative
  {
    void SetManagedCallbacks([In] IAxolotlStore StoreCallbacks);

    void CreateRegistrationData();

    void GenerateSignedPreKey([In] int NextSignedPreKeyId);

    void GeneratePreKeys([In] int NextPreKeyId, [In] int BatchSize);

    void SessionProcessPreKeyBundle(
      [In] string RecipientId,
      [In] int RegistrationId,
      [In] byte Type,
      [In] IByteBuffer Identity,
      [In] IByteBuffer PreKeyData,
      [In] int PreKeyId,
      [In] IByteBuffer SignedPreKeyData,
      [In] int SignedPreKeyId,
      [In] IByteBuffer SignedPreKeySignature);

    void GetSessionInfo(
      [In] string RecipientId,
      out int RemoteRegistrationId,
      out IByteBuffer AliceBaseKey);

    void GetIdentityKeyForSending(out IByteBuffer IdentityKey);

    void GetUnsentPreKeys([In] int Limit, out IByteBuffer UnsentPreKeysBuffer);

    void GetLatestSignedPreKey(out IByteBuffer SignedPreKeyBuffer);

    IByteBuffer GetPreKeysDataFromRecord([In] IByteBuffer PreKeysBuffer, [In] int Count);

    void GroupProcessSenderKeyBundle(
      [In] string GroupId,
      [In] string RecipientId,
      [In] bool FastRatchet,
      [In] IByteBuffer SenderKeyDistributionMessage);

    IByteBuffer GroupCreateSenderKeyBundle([In] string GroupId, [In] string RecipientId, [In] bool FastRatchet);

    void IdentityGetFingerprint(
      [In] string MyStableId,
      [In] string RecipientId,
      [In] string RecipientStableId,
      out string Displayable,
      out IByteBuffer Scannable);

    int IdentityVerifyFingerprint([In] IByteBuffer Expected, [In] IByteBuffer Scanned);

    uint GetSignalContext();

    uint GetStoreContext();
  }
}
