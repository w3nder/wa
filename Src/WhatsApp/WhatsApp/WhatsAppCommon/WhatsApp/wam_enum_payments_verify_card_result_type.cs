// Decompiled with JetBrains decompiler
// Type: WhatsApp.wam_enum_payments_verify_card_result_type
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp
{
  public enum wam_enum_payments_verify_card_result_type
  {
    OK = 1,
    ERROR_GENERIC = 2,
    DEBIT_CARD_INVALID = 3,
    EXPIRATION_IN_PAST = 4,
    EXPIRATION_EMPTY = 5,
    EXPIRATION_INVALID = 6,
    MONTH_INVALID = 7,
    YEAR_INVALID = 8,
  }
}
