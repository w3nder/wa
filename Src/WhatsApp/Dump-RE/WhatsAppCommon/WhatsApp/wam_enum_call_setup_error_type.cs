﻿// Decompiled with JetBrains decompiler
// Type: WhatsApp.wam_enum_call_setup_error_type
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp
{
  public enum wam_enum_call_setup_error_type
  {
    UNKNOWN = 1,
    CALL_ACCEPT_FAILED = 2,
    INIT_MEDIA_STREAM_FAILED = 3,
    START_MEDIA_STREAM_FAILED = 4,
    AUDIO_INIT_ERROR = 5,
    HANDLE_OFFER_FAILED = 6,
    HANDLE_ACCEPT_FAILED = 7,
    SOUND_PORT_CREATE_FAILED = 8,
    P2P_TRANSPORT_CREATE_FAILED = 9,
    P2P_TRANSPORT_MEDIA_CREATE_FAILED = 10, // 0x0000000A
    INCOMPATIBLE_SRTP_KEY_EXCHANGE = 11, // 0x0000000B
    SRTP_KEY_GENERATION_ERROR = 12, // 0x0000000C
    UNSUPPORTED_AUDIO_CAPS = 13, // 0x0000000D
    P2P_TRANSPORT_START_FAILED = 14, // 0x0000000E
    RELAY_BIND_FAILED = 15, // 0x0000000F
    CANNOT_INITIALIZE_AUDIO_RECORD_OBJECT = 16, // 0x00000010
    PEER_RELAY_BIND_FAILED = 17, // 0x00000011
    VIDEO_CAPTURE_INIT_FAILED = 18, // 0x00000012
    VIDEO_CAPTURE_START_FAILED = 19, // 0x00000013
    VIDEO_RENDER_INIT_FAILED = 20, // 0x00000014
    VIDEO_RENDER_START_FAILED = 21, // 0x00000015
    VIDEO_ENCODER_OPEN_FAILED = 22, // 0x00000016
    VIDEO_DECODER_OPEN_FAILED = 23, // 0x00000017
    VIDEO_STREAM_CREATE_FAILED = 24, // 0x00000018
    VIDEO_STREAM_SETUP_FAILED = 25, // 0x00000019
    PEER_SETUP_FAILED = 26, // 0x0000001A
    HANDLE_PREACCEPT_FAILED = 27, // 0x0000001B
  }
}