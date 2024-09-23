// Decompiled with JetBrains decompiler
// Type: WhatsApp.PaymentsMethodSummary
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public class PaymentsMethodSummary
  {
    public string CredentialId { get; private set; }

    public string ReadableName { get; private set; }

    public PaymentsMethod.PaymentTypes PaymentType { get; private set; }

    public int PaymentSubType { get; private set; }

    private PaymentsMethodSummary()
    {
    }

    public PaymentsMethodSummary(PaymentsMethod cloneFrom)
    {
      this.CredentialId = cloneFrom?.CredentialId;
      this.ReadableName = cloneFrom?.ReadableName;
      this.PaymentType = cloneFrom == null ? PaymentsMethod.PaymentTypes.Unknown : cloneFrom.PaymentType;
      this.PaymentSubType = cloneFrom == null ? 0 : cloneFrom.PaymentSubType;
    }

    public static PaymentsMethodSummary[] Deserialize(byte[] binaryData)
    {
      BinaryData binaryData1 = new BinaryData(binaryData);
      try
      {
        int newOffset = 0;
        int length = !(binaryData1.ReadStrWithLengthPrefix(0, out newOffset) != "1") ? binaryData1.ReadInt32(newOffset) : throw new ArgumentException("PaymentsMethodSummary Binary data does not match expected format");
        newOffset += 4;
        PaymentsMethodSummary[] paymentsMethodSummaryArray = new PaymentsMethodSummary[length];
        for (int index = 0; index < length; ++index)
        {
          PaymentsMethodSummary paymentsMethodSummary = new PaymentsMethodSummary();
          paymentsMethodSummary.CredentialId = binaryData1.ReadStrWithLengthPrefix(newOffset, out newOffset);
          paymentsMethodSummary.ReadableName = binaryData1.ReadStrWithLengthPrefix(newOffset, out newOffset);
          paymentsMethodSummary.PaymentType = (PaymentsMethod.PaymentTypes) binaryData1.ReadInt32(newOffset);
          newOffset += 4;
          paymentsMethodSummary.PaymentSubType = binaryData1.ReadInt32(newOffset);
          newOffset += 4;
          paymentsMethodSummaryArray[index] = paymentsMethodSummary;
        }
        return paymentsMethodSummaryArray;
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception deserializing payments methods");
        return (PaymentsMethodSummary[]) null;
      }
    }

    public static byte[] Serialize(PaymentsMethodSummary method)
    {
      if (method == null)
        return (byte[]) null;
      return PaymentsMethodSummary.Serialize(new PaymentsMethodSummary[1]
      {
        method
      });
    }

    public static byte[] Serialize(PaymentsMethodSummary[] methods)
    {
      if (methods == null || methods.Length == 0)
        return (byte[]) null;
      BinaryData bd = new BinaryData();
      bd.AppendStrWithLengthPrefix("1");
      bd.AppendInt32(methods.Length);
      for (int index = 0; index < methods.Length; ++index)
        methods[index].Serialize(bd);
      return bd.Get();
    }

    private void Serialize(BinaryData bd)
    {
      bd.AppendStrWithLengthPrefix(this.CredentialId);
      bd.AppendStrWithLengthPrefix(this.ReadableName);
      bd.AppendInt32((int) this.PaymentType);
      bd.AppendInt32(this.PaymentSubType);
    }
  }
}
