// Decompiled with JetBrains decompiler
// Type: WhatsApp.PhoneNumberVerificationState
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp
{
  public enum PhoneNumberVerificationState
  {
    NewlyEntered = 0,
    SameDeviceFailed = 1,
    ServerSentSms = 2,
    ServerSendSmsFailed = 3,
    ServerSentVoice = 4,
    ServerSendVoiceFailed = 5,
    VerifiedPendingBackupCheck = 6,
    VerifiedPendingHistoryRestore = 7,
    VerifiedPendingSecurityCode = 8,
    Verified = 100, // 0x00000064
  }
}
