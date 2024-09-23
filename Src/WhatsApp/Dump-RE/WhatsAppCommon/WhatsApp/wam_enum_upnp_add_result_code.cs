﻿// Decompiled with JetBrains decompiler
// Type: WhatsApp.wam_enum_upnp_add_result_code
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp
{
  public enum wam_enum_upnp_add_result_code
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
    TOO_MANY_PORTS = 101, // 0x00000065
    TIMEOUT = 102, // 0x00000066
    ERR_CODE_INVALID_ACTION = 401, // 0x00000191
    ERR_CODE_INVALID_ARGS = 402, // 0x00000192
    ERR_CODE_ACTION_FAILED = 501, // 0x000001F5
    ERR_CODE_ACTION_NOT_AUTHORIZED = 606, // 0x0000025E
    ERR_CODE_PINHOLE_SPACE_EXHAUSTED = 701, // 0x000002BD
    ERR_CODE_FIREWALL_DISABLED = 702, // 0x000002BE
    ERR_CODE_INBOUND_PINHOLE_NOT_ALLOWED = 703, // 0x000002BF
    ERR_CODE_NO_SUCH_ENTRY = 704, // 0x000002C0
    ERR_CODE_PROTOCOL_NOT_SUPPORTED = 705, // 0x000002C1
    ERR_CODE_INTERNAL_PORT_WILDCARD_NOT_ALLOWED = 706, // 0x000002C2
    ERR_CODE_PROTOCOL_WILDCARD_NOT_ALLOWED = 707, // 0x000002C3
    ERR_CODE_WILDCARD_NOT_PERMITTED_IN_SRC_IP = 708, // 0x000002C4
    ERR_CODE_NO_PACKET_SENT = 709, // 0x000002C5
    ERR_CODE_SPECIFIED_ARRAY_INDEX_INVALID = 713, // 0x000002C9
    ERR_CODE_NO_SUCH_ENTRY_IN_ARRAY = 714, // 0x000002CA
    ERR_CODE_WILD_CARD_NOT_PERMITTED_IN_SRC_IP = 715, // 0x000002CB
    ERR_CODE_WILD_CARD_NOT_PERMITTED_IN_EXT_PORT = 716, // 0x000002CC
    ERR_CODE_CONFLICT_IN_MAPPING_ENTRY = 718, // 0x000002CE
    ERR_CODE_SAME_PORT_VALUES_REQUIRED = 724, // 0x000002D4
    ERR_CODE_ONLY_PERMANENT_LEASES_SUPPORTED = 725, // 0x000002D5
    ERR_CODE_REMOTE_HOST_ONLY_SUPPORTS_WILDCARD = 726, // 0x000002D6
    ERR_CODE_EXTERNAL_PORT_ONLY_SUPORTS_WILDCARD = 727, // 0x000002D7
    ERR_CODE_NO_PORT_MAPS_AVAILABLE = 728, // 0x000002D8
    ERR_CODE_CONFLICT_WITH_OTHER_MECHANISMS = 729, // 0x000002D9
    ERR_CODE_WILD_CARD_NOT_PERMITTED_IN_INT_PORT = 732, // 0x000002DC
    DISABLED = 999, // 0x000003E7
  }
}
