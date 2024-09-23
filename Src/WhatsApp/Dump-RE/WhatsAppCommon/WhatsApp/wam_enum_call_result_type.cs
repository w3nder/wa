// Decompiled with JetBrains decompiler
// Type: WhatsApp.wam_enum_call_result_type
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp
{
  public enum wam_enum_call_result_type
  {
    INVALID,
    CONNECTED,
    REJECTED_BY_USER,
    REJECTED_BY_SERVER,
    MISSED,
    BUSY,
    SETUP_ERROR,
    SERVER_NACK,
    CALL_OFFER_ACK_NOT_RECEIVED,
    MISSED_NO_RECEIPT,
    ACCEPTED_BUT_NOT_CONNECTED,
    CALL_CANCELED_CELLULAR_IN_PROGRESS,
    CALL_CANCELED_AIRPLANE_MODE_ON,
    CALL_CANCELED_NO_NETWORK,
    CALL_OFFER_ACK_CORRUPT,
    CALL_REJECTED_TOS,
    CALL_REJECTED_E2E,
    CALL_REJECTED_UNAVAILABLE,
    CALL_CANCELED_OFFER_NOT_SENT,
    PEER_SETUP_ERROR,
    ACTIVE_ELSEWHERE,
  }
}
