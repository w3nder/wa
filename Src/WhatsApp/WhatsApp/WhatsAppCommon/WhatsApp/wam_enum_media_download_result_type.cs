// Decompiled with JetBrains decompiler
// Type: WhatsApp.wam_enum_media_download_result_type
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp
{
  public enum wam_enum_media_download_result_type
  {
    OK = 1,
    ERROR_UNKNOWN = 2,
    ERROR_TIMEOUT = 3,
    ERROR_DNS = 4,
    ERROR_INSUFFICIENT_SPACE = 5,
    ERROR_TOO_OLD = 6,
    ERROR_CANNOT_RESUME = 7,
    ERROR_HASH_MISMATCH = 8,
    ERROR_INVALID_URL = 9,
    ERROR_OUTPUT_STREAM = 10, // 0x0000000A
    ERROR_CANCEL = 11, // 0x0000000B
    DEDUPED = 12, // 0x0000000C
    ERROR_INVALID_MEDIA = 13, // 0x0000000D
    ERROR_ENC_HASH_MISMATCH = 14, // 0x0000000E
    PREFETCH_END = 15, // 0x0000000F
    ERROR_CANCEL_PROGRAMMATIC = 16, // 0x00000010
    ERROR_MEDIA_CONN = 17, // 0x00000011
  }
}
