// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.Login
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class Login : WamEvent
  {
    public wam_enum_login_result_type? loginResult;
    public long? retryCount;
    public bool? longConnect;
    public long? loginT;
    public long? connectionT;
    public wam_enum_connection_origin_type? connectionOrigin;

    public void Reset()
    {
      this.loginResult = new wam_enum_login_result_type?();
      this.retryCount = new long?();
      this.longConnect = new bool?();
      this.loginT = new long?();
      this.connectionT = new long?();
      this.connectionOrigin = new wam_enum_connection_origin_type?();
    }

    public override uint GetCode() => 460;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_login_result_type>(this.loginResult));
      Wam.MaybeSerializeField(2, this.retryCount);
      Wam.MaybeSerializeField(4, this.longConnect);
      Wam.MaybeSerializeField(3, this.loginT);
      Wam.MaybeSerializeField(5, this.connectionT);
      Wam.MaybeSerializeField(6, Wam.EnumToLong<wam_enum_connection_origin_type>(this.connectionOrigin));
    }
  }
}
