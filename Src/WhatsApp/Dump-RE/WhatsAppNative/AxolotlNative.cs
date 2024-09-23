// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.AxolotlNative
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [MarshalingBehavior]
  [Version(100794368)]
  [Activatable(100794368)]
  public sealed class AxolotlNative : IAxolotlNative
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern AxolotlNative();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetManagedCallbacks([In] IAxolotlStore StoreCallbacks);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void CreateRegistrationData();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void GenerateSignedPreKey([In] int NextSignedPreKeyId);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void GeneratePreKeys([In] int NextPreKeyId, [In] int BatchSize);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SessionProcessPreKeyBundle(
      [In] string RecipientId,
      [In] int RegistrationId,
      [In] byte Type,
      [In] IByteBuffer Identity,
      [In] IByteBuffer PreKeyData,
      [In] int PreKeyId,
      [In] IByteBuffer SignedPreKeyData,
      [In] int SignedPreKeyId,
      [In] IByteBuffer SignedPreKeySignature);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void GetSessionInfo(
      [In] string RecipientId,
      out int RemoteRegistrationId,
      out IByteBuffer AliceBaseKey);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void GetIdentityKeyForSending(out IByteBuffer IdentityKey);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void GetUnsentPreKeys([In] int Limit, out IByteBuffer UnsentPreKeysBuffer);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void GetLatestSignedPreKey(out IByteBuffer SignedPreKeyBuffer);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IByteBuffer GetPreKeysDataFromRecord([In] IByteBuffer PreKeysBuffer, [In] int Count);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void GroupProcessSenderKeyBundle(
      [In] string GroupId,
      [In] string RecipientId,
      [In] bool FastRatchet,
      [In] IByteBuffer SenderKeyDistributionMessage);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IByteBuffer GroupCreateSenderKeyBundle(
      [In] string GroupId,
      [In] string RecipientId,
      [In] bool FastRatchet);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void IdentityGetFingerprint(
      [In] string MyStableId,
      [In] string RecipientId,
      [In] string RecipientStableId,
      out string Displayable,
      out IByteBuffer Scannable);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern int IdentityVerifyFingerprint([In] IByteBuffer Expected, [In] IByteBuffer Scanned);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern uint GetSignalContext();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern uint GetStoreContext();
  }
}
