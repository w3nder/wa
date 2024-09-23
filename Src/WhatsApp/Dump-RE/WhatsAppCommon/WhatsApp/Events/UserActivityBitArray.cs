// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.UserActivityBitArray
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class UserActivityBitArray : WamEvent
  {
    public long? userActivityBitmapLen;
    public long? userActivityStartTime;
    public long? userActivityBitmap0;
    public long? userActivityBitmap1;
    public long? userActivityBitmap2;
    public long? userActivityBitmap3;
    public long? userActivityBitmap4;
    public long? userActivityBitmap5;
    public long? userActivityBitmap6;
    public long? userActivityBitmap7;
    public long? userActivityBitmap8;
    public long? userActivityBitmap9;
    public long? userActivityBitmap10;
    public long? userActivityBitmap11;
    public long? userActivityBitmap12;
    public long? userActivityBitmap13;
    public long? userActivityBitmap14;
    public long? userActivityBitmap15;
    public long? userActivityBitmap16;
    public long? userActivityBitmap17;
    public long? userActivityBitmap18;
    public long? userActivityBitmap19;

    public void Reset()
    {
      this.userActivityBitmapLen = new long?();
      this.userActivityStartTime = new long?();
      this.userActivityBitmap0 = new long?();
      this.userActivityBitmap1 = new long?();
      this.userActivityBitmap2 = new long?();
      this.userActivityBitmap3 = new long?();
      this.userActivityBitmap4 = new long?();
      this.userActivityBitmap5 = new long?();
      this.userActivityBitmap6 = new long?();
      this.userActivityBitmap7 = new long?();
      this.userActivityBitmap8 = new long?();
      this.userActivityBitmap9 = new long?();
      this.userActivityBitmap10 = new long?();
      this.userActivityBitmap11 = new long?();
      this.userActivityBitmap12 = new long?();
      this.userActivityBitmap13 = new long?();
      this.userActivityBitmap14 = new long?();
      this.userActivityBitmap15 = new long?();
      this.userActivityBitmap16 = new long?();
      this.userActivityBitmap17 = new long?();
      this.userActivityBitmap18 = new long?();
      this.userActivityBitmap19 = new long?();
    }

    public override uint GetCode() => 1424;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.userActivityBitmapLen);
      Wam.MaybeSerializeField(2, this.userActivityStartTime);
      Wam.MaybeSerializeField(3, this.userActivityBitmap0);
      Wam.MaybeSerializeField(4, this.userActivityBitmap1);
      Wam.MaybeSerializeField(5, this.userActivityBitmap2);
      Wam.MaybeSerializeField(6, this.userActivityBitmap3);
      Wam.MaybeSerializeField(7, this.userActivityBitmap4);
      Wam.MaybeSerializeField(8, this.userActivityBitmap5);
      Wam.MaybeSerializeField(9, this.userActivityBitmap6);
      Wam.MaybeSerializeField(10, this.userActivityBitmap7);
      Wam.MaybeSerializeField(11, this.userActivityBitmap8);
      Wam.MaybeSerializeField(12, this.userActivityBitmap9);
      Wam.MaybeSerializeField(13, this.userActivityBitmap10);
      Wam.MaybeSerializeField(14, this.userActivityBitmap11);
      Wam.MaybeSerializeField(15, this.userActivityBitmap12);
      Wam.MaybeSerializeField(16, this.userActivityBitmap13);
      Wam.MaybeSerializeField(17, this.userActivityBitmap14);
      Wam.MaybeSerializeField(18, this.userActivityBitmap15);
      Wam.MaybeSerializeField(19, this.userActivityBitmap16);
      Wam.MaybeSerializeField(20, this.userActivityBitmap17);
      Wam.MaybeSerializeField(21, this.userActivityBitmap18);
      Wam.MaybeSerializeField(22, this.userActivityBitmap19);
    }
  }
}
