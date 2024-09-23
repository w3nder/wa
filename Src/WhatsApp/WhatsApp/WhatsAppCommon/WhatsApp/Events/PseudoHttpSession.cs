// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.PseudoHttpSession
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class PseudoHttpSession : WamEvent
  {
    public double? pseudoHttpTotalBytesSent;
    public double? pseudoHttpTotalBytesReceived;
    public double? pseudoHttpHeaderBytesSent;
    public double? pseudoHttpHeaderBytesReceived;
    public long? pseudoHttpSendOverheadT;
    public long? pseudoHttpReceiveOverheadT;

    public void Reset()
    {
      this.pseudoHttpTotalBytesSent = new double?();
      this.pseudoHttpTotalBytesReceived = new double?();
      this.pseudoHttpHeaderBytesSent = new double?();
      this.pseudoHttpHeaderBytesReceived = new double?();
      this.pseudoHttpSendOverheadT = new long?();
      this.pseudoHttpReceiveOverheadT = new long?();
    }

    public override uint GetCode() => 722;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.pseudoHttpTotalBytesSent);
      Wam.MaybeSerializeField(2, this.pseudoHttpTotalBytesReceived);
      Wam.MaybeSerializeField(3, this.pseudoHttpHeaderBytesSent);
      Wam.MaybeSerializeField(4, this.pseudoHttpHeaderBytesReceived);
      Wam.MaybeSerializeField(5, this.pseudoHttpSendOverheadT);
      Wam.MaybeSerializeField(6, this.pseudoHttpReceiveOverheadT);
    }
  }
}
