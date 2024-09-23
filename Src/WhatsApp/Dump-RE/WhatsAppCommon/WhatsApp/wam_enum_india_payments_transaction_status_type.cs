// Decompiled with JetBrains decompiler
// Type: WhatsApp.wam_enum_india_payments_transaction_status_type
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp
{
  public enum wam_enum_india_payments_transaction_status_type
  {
    SUCCESS = 1,
    FAILED = 2,
    FAILED_DA = 3,
    FAILED_RISK = 4,
    PENDING_SETUP = 5,
    FAILED_DA_FINAL = 8,
    REFUNDED = 9,
    REFUND_FAILED_PROCESSING = 10, // 0x0000000A
    REFUND_FAILED = 11, // 0x0000000B
    REFUND_FAILED_DA = 12, // 0x0000000C
    FAILED_RECEIVER_PROCESSING = 13, // 0x0000000D
    FAILED_PROCESSING = 14, // 0x0000000E
    EXPIRED = 15, // 0x0000000F
  }
}
