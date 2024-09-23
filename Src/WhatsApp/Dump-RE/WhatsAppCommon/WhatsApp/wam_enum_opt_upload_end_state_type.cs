// Decompiled with JetBrains decompiler
// Type: WhatsApp.wam_enum_opt_upload_end_state_type
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp
{
  public enum wam_enum_opt_upload_end_state_type
  {
    SUCCEEDED_USED = 1,
    SUCCEEDED_NOT_USED = 2,
    FAILED_UPLOAD = 3,
    FAILED_EXPORT = 4,
    REMOVED = 5,
    STOPPED_ALL = 6,
    SENT_NOT_COMPLETED = 7,
    NOT_SET = 8,
    STOPPED_BEFORE_STARTED = 9,
    TAKEOVER = 10, // 0x0000000A
    DISCARDED = 11, // 0x0000000B
    DISABLED = 12, // 0x0000000C
  }
}
