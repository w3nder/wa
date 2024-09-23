// Decompiled with JetBrains decompiler
// Type: WhatsApp.FileRoot
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp
{
  public enum FileRoot
  {
    IsoStore = 0,
    PhoneStorage = 1,
    SdCard = 2,
    StorageMask = 15, // 0x0000000F
    WhatsAppMedia = 256, // 0x00000100
    PhoneStorageWhatsAppMedia = 257, // 0x00000101
    SdCardWhatsAppMedia = 258, // 0x00000102
    Backup = 512, // 0x00000200
    PhoneStorageBackup = 513, // 0x00000201
  }
}
