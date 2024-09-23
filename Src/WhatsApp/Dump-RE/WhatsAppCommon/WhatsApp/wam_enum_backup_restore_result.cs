// Decompiled with JetBrains decompiler
// Type: WhatsApp.wam_enum_backup_restore_result
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp
{
  public enum wam_enum_backup_restore_result
  {
    OK = 1,
    UNKNOWN_ERROR = 2,
    REMOTE_STORAGE_IS_FULL = 3,
    WIFI_REQUIRED_BUT_MISSING = 4,
    BACKUP_SERVER_UNREACHABLE = 5,
    AUTH_FAILED = 6,
    DATA_CONNECTION_REQUIRED_BUT_MISSING = 7,
    LOCAL_STORAGE_IS_FULL = 8,
    MISSING_CHAT_STORE = 9,
    FILE_NOT_FOUND = 10, // 0x0000000A
    BASE_FOLDER_DOES_NOT_EXIST = 11, // 0x0000000B
    BACKUP_SERVER_NOT_WORKING = 12, // 0x0000000C
    WHATSAPP_SERVER_UNREACHABLE = 13, // 0x0000000D
    SUPPORT_SERVICE_UNAVAILABLE_ON_DEVICE = 14, // 0x0000000E
    ACCOUNT_MISSING_FROM_DEVICE = 15, // 0x0000000F
    READ_STORAGE_PERMISSION_DENIED = 16, // 0x00000010
  }
}
