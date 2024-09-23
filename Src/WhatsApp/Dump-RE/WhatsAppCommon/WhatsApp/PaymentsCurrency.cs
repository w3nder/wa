// Decompiled with JetBrains decompiler
// Type: WhatsApp.PaymentsCurrency
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp
{
  public class PaymentsCurrency
  {
    public string Iso4217CurrencyCode { get; set; }

    public string CurrencySymbol { get; set; }

    public string AltCurrencySymbol { get; set; }

    public string CurrencyFormat { get; set; }

    public int DecimalScale { get; set; }

    public static PaymentsCurrency Create(string currency)
    {
      PaymentsCurrency paymentsCurrency = (PaymentsCurrency) null;
      if (currency?.ToUpper() == "INR")
        paymentsCurrency = new PaymentsCurrency()
        {
          Iso4217CurrencyCode = "INR",
          CurrencySymbol = "₹",
          AltCurrencySymbol = "Rs.",
          CurrencyFormat = "#,##,##,##,###.00",
          DecimalScale = 2
        };
      return paymentsCurrency;
    }

    public string FormatAmount(double amount)
    {
      return string.Format("{0}{1}", (object) this.CurrencySymbol, (object) amount.ToString(this.CurrencyFormat));
    }
  }
}
