// Decompiled with JetBrains decompiler
// Type: WhatsApp.wam_enum_upnp_remove_result_code
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp
{
  public enum wam_enum_upnp_remove_result_code
  {
    SUCCESS = 0,
    UNKNOWN_ERROR = 1,
    INVALID_ARGS = 2,
    HTTP_ERROR = 3,
    INVALID_RESPONSE = 4,
    MEM_ALLOC_ERROR = 5,
    NO_DEVICE_FOUND = 6,
    NO_IGD_FOUND = 7,
    NOT_ON_WIFI = 100, // 0x00000064
    TIMEOUT = 102, // 0x00000066
    ERR_CODE_INVALID_ACTION = 401, // 0x00000191
    ERR_CODE_INVALID_ARGS = 402, // 0x00000192
    ERR_CODE_ACTION_FAILED = 501, // 0x000001F5
    ERR_CODE_ACTION_NOT_AUTHORIZED = 606, // 0x0000025E
    ERR_CODE_WILD_CARD_NOT_PERMITTED_IN_SRC_IP = 715, // 0x000002CB
    ERR_CODE_WILD_CARD_NOT_PERMITTED_IN_EXT_PORT = 716, // 0x000002CC
    ERR_CODE_NO_PORT_MAPS_AVAILABLE = 728, // 0x000002D8
    ERR_CODE_CONFLICT_WITH_OTHER_MECHANISMS = 729, // 0x000002D9
    ERR_CODE_WILD_CARD_NOT_PERMITTED_IN_INT_PORT = 732, // 0x000002DC
    DISABLED = 999, // 0x000003E7
  }
}
