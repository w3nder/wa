// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IAxolotlStore
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Guid(129123290, 5979, 19059, 142, 9, 245, 197, 254, 146, 17, 101)]
  [Version(100794368)]
  public interface IAxolotlStore
  {
    IByteBuffer SessionLoadSession([In] string RecipientId, [In] int DeviceId);

    IByteBuffer SessionGetSubDeviceSessions([In] string RecipientId);

    void SessionStoreSession([In] string RecipientId, [In] int DeviceId, [In] IByteBuffer RecordBuffer);

    bool SessionContainsSession([In] string RecipientId, [In] int DeviceId);

    void SessionDeleteSession([In] string RecipientId, [In] int DeviceId);

    void SessionDeleteAllSessions([In] string RecipientId);

    void SessionDestroy();

    IByteBuffer PreKeyLoadPreKey([In] uint KeyId);

    void PreKeyStorePreKey([In] uint KeyId, [In] IByteBuffer RecordBuffer);

    bool PreKeyContainsPreKey([In] uint KeyId);

    void PreKeyRemovePreKey([In] uint KeyId);

    void PreKeyDestroy();

    IByteBuffer SignedPreKeyLoadSignedPreKey([In] uint KeyId);

    void SignedPreKeyStoreSignedPreKey([In] uint KeyId, [In] IByteBuffer RecordBuffer);

    bool SignedPreKeyContainsSignedPreKey([In] uint KeyId);

    void SignedPreKeyRemoveSignedPreKey([In] uint KeyId);

    void SignedPreKeyDestroy();

    void IdentityGetIdentityKeyPair(
      out IByteBuffer PublicKeyBuffer,
      out IByteBuffer PrivateKeyBuffer);

    uint IdentityGetLocalRegistrationId();

    void IdentitySaveIdentity([In] string RecipientId, [In] IByteBuffer KeyBuffer);

    bool IdentityIsTrustedIdentity([In] string RecipientId, [In] IByteBuffer PublicKeyBuffer);

    void IdentityDestroy();

    void IdentityGetPublicKey([In] string RecipientId, out IByteBuffer PublicKeyBuffer);

    void IdentityRegisterSelf(
      [In] int Registration,
      [In] IByteBuffer PublicKeyBuffer,
      [In] IByteBuffer PrivateKeyBuffer,
      [In] int NextPreKeyId,
      [In] int Timestamp);

    void UnsentPreKeysInDatabase(
      [In] int Limit,
      out int UnsentPreKeysCount,
      out IByteBuffer UnsentPreKeysBuffer);

    IByteBuffer LatestSignedPreKeyInDatabase();

    IByteBuffer SenderKeyLoadSenderKey([In] string GroupId, [In] string SenderKeyId);

    void SenderKeyStoreSenderKey([In] string GroupId, [In] string SenderKeyId, [In] IByteBuffer RecordBuffer);

    void SenderKeyDestroy();

    IByteBuffer FastRatchetSenderKeyLoadFastRatchetSenderKey([In] string GroupId, [In] string SenderKeyId);

    void FastRatchetSenderKeyStoreFastRatchetSenderKey(
      [In] string GroupId,
      [In] string SenderKeyId,
      [In] IByteBuffer RecordBuffer);

    void FastRatchetSenderKeyDestroy();
  }
}
