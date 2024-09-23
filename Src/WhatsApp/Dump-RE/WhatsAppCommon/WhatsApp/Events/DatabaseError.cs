// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.DatabaseError
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class DatabaseError : WamEvent
  {
    public wam_enum_database_type? databaseType;
    public string databaseMethod;
    public wam_enum_database_error_code? databaseErrorCode;

    public void Reset()
    {
      this.databaseType = new wam_enum_database_type?();
      this.databaseMethod = (string) null;
      this.databaseErrorCode = new wam_enum_database_error_code?();
    }

    public override uint GetCode() => 498;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_database_type>(this.databaseType));
      Wam.MaybeSerializeField(2, this.databaseMethod);
      Wam.MaybeSerializeField(3, Wam.EnumToLong<wam_enum_database_error_code>(this.databaseErrorCode));
    }
  }
}
