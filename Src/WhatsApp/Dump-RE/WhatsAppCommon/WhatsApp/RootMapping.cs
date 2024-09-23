// Decompiled with JetBrains decompiler
// Type: WhatsApp.RootMapping
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp
{
  public struct RootMapping
  {
    public FileRoot Base;
    public string Subdir;
    public FileRoot Value;
    public static readonly RootMapping[] Mappings = new RootMapping[3]
    {
      new RootMapping()
      {
        Base = FileRoot.PhoneStorage,
        Subdir = "Data\\Users\\Public\\Pictures\\WhatsApp",
        Value = FileRoot.WhatsAppMedia
      },
      new RootMapping()
      {
        Base = FileRoot.SdCard,
        Subdir = "Pictures\\WhatsApp",
        Value = FileRoot.WhatsAppMedia
      },
      new RootMapping()
      {
        Base = FileRoot.PhoneStorage,
        Subdir = "Data\\SharedData\\OEM\\Public\\WhatsApp\\Backup",
        Value = FileRoot.Backup
      }
    };
  }
}
