// Decompiled with JetBrains decompiler
// Type: WhatsApp.ProtoBuf.MessageTypes
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.ProtoBuf
{
  public enum MessageTypes
  {
    none = 0,
    conversation = 1,
    sender_key_distribution_message = 2,
    image_message = 3,
    contact_message = 4,
    location_message = 5,
    extended_text_message = 6,
    document_message = 7,
    audio_message = 8,
    video_message = 9,
    call = 10, // 0x0000000A
    protocol_message = 12, // 0x0000000C
    contacts_array_message = 13, // 0x0000000D
    highly_structured_message = 14, // 0x0000000E
    send_payments_message = 16, // 0x00000010
    request_payment_message = 17, // 0x00000011
    live_location_message = 18, // 0x00000012
    sticker_message = 19, // 0x00000013
  }
}
