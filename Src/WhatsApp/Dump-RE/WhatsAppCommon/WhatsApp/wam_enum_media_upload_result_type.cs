// Decompiled with JetBrains decompiler
// Type: WhatsApp.wam_enum_media_upload_result_type
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp
{
  public enum wam_enum_media_upload_result_type
  {
    OK = 1,
    ERROR_UNKNOWN = 2,
    DUPLICATE = 3,
    ERROR_REQUEST = 4,
    ERROR_UPLOAD = 5,
    ERROR_OOM = 6,
    ERROR_IO = 7,
    ERROR_NO_PERMISSIONS = 8,
    ERROR_BAD_MEDIA = 9,
    ERROR_INSUFFICIENT_SPACE = 10, // 0x0000000A
    ERROR_FNF = 11, // 0x0000000B
    ERROR_CANCEL = 12, // 0x0000000C
    ERROR_SERVER = 13, // 0x0000000D
    ERROR_REQUEST_TIMEOUT = 14, // 0x0000000E
    ERROR_NOT_FINALIZED = 15, // 0x0000000F
    ERROR_OPTIMISTIC_HASH = 16, // 0x00000010
    ERROR_MEDIA_CONN = 17, // 0x00000011
  }
}
