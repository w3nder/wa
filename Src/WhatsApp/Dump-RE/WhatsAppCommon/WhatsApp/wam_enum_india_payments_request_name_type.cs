// Decompiled with JetBrains decompiler
// Type: WhatsApp.wam_enum_india_payments_request_name_type
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp
{
  public enum wam_enum_india_payments_request_name_type
  {
    LIST_KEYS = 1,
    GET_TOKEN = 2,
    UPI_BATCH = 3,
    GET_BANKS = 4,
    REGISTER = 5,
    SET_PIN = 6,
    CHANGE_PIN = 7,
    GET_VPA = 8,
    VPA_SYNC = 9,
    GET_ONE_TRANSACTION = 10, // 0x0000000A
    GET_TRANSACTIONS = 11, // 0x0000000B
    GET_METHODS = 12, // 0x0000000C
    REMOVE_ONE_ACCOUNT = 13, // 0x0000000D
    DEREGISTER = 14, // 0x0000000E
    CHANGE_PRIMARY = 15, // 0x0000000F
    GENERATE_OTP = 16, // 0x00000010
    SET_TOS = 17, // 0x00000011
  }
}
