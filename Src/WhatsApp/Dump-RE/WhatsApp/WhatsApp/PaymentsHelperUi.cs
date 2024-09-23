// Decompiled with JetBrains decompiler
// Type: WhatsApp.PaymentsHelperUi
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace WhatsApp
{
  internal class PaymentsHelperUi
  {
    public static bool sendCashInRequest(
      PaymentsMethod fromMethod,
      PaymentsMethod toMethod,
      long amountx1000,
      string currency,
      Func<bool> isCancelled,
      Action<string, string> onReceivedOk,
      Action<int> onReceivedError)
    {
      if (!PaymentsSettings.IsOkToSendCashIn())
        return false;
      string contextUuid = Guid.NewGuid().ToString();
      Action<string, long, string, string> onReceivedOK = (Action<string, long, string, string>) ((tranId, ts, status, verifyUrl) =>
      {
        if (isCancelled() || !PaymentsSettings.PersistCashIn(PaymentTransactionInfo.CreateCashInTransaction(contextUuid, tranId, amountx1000, currency, (byte[]) null, PaymentTransactionInfo.GetStatusAsEnum(status), ts)))
          return;
        onReceivedOk(contextUuid, verifyUrl);
      });
      App.CurrentApp.Connection.InvokeWhenConnected((Action) (() =>
      {
        try
        {
          App.CurrentApp.Connection.SendPaymentCashInRequest(contextUuid, fromMethod.CredentialId, toMethod.CredentialId, PaymentsHelper.FormatAmountForFB(amountx1000), currency, onReceivedOK, onReceivedError);
        }
        catch (Exception ex)
        {
          string context = string.Format("Sending SendCashInRequest");
          Log.LogException(ex, context);
        }
      }));
      return true;
    }

    public static bool sendCashOutRequest(
      PaymentsMethod fromMethod,
      PaymentsMethod toMethod,
      long amountx1000,
      string currency,
      Func<bool> isCancelled,
      Action<string> onReceivedOk,
      Action<int> onReceivedError)
    {
      if (!PaymentsSettings.IsOkToSendCashOut())
        return false;
      string contextUuid = Guid.NewGuid().ToString();
      Action<string, long, string> onReceivedOK = (Action<string, long, string>) ((tranId, ts, status) =>
      {
        if (!PaymentsSettings.PersistCashOut(PaymentTransactionInfo.CreateCashOutTransaction(contextUuid, tranId, amountx1000, currency, (byte[]) null, PaymentTransactionInfo.GetStatusAsEnum(status), ts)))
          return;
        onReceivedOk(contextUuid);
      });
      App.CurrentApp.Connection.InvokeWhenConnected((Action) (() =>
      {
        try
        {
          App.CurrentApp.Connection.SendPaymentCashOutRequest(contextUuid, fromMethod.CredentialId, toMethod.CredentialId, PaymentsHelper.FormatAmountForFB(amountx1000), currency, onReceivedOK, onReceivedError);
        }
        catch (Exception ex)
        {
          string context = string.Format("Sending sendCashOutRequest");
          Log.LogException(ex, context);
        }
      }));
      return true;
    }

    public static bool sendGetTranRequest(
      string tranid,
      Action<FunXMPP.Connection.PaymentsTransactionResponse> onReceivedOK,
      Action<int> onReceivedError)
    {
      App.CurrentApp.Connection.InvokeWhenConnected((Action) (() =>
      {
        try
        {
          App.CurrentApp.Connection.SendPaymentGetTransactionRequest(tranid, onReceivedOK, onReceivedError);
        }
        catch (Exception ex)
        {
          string context = string.Format("Sending sendGetTranRequest");
          Log.LogException(ex, context);
        }
      }));
      return true;
    }

    public static bool sendGetTransRequest(
      DateTime endDateTime,
      Action<List<FunXMPP.Connection.PaymentsTransactionResponse>> onReceivedOK,
      Action<int> onReceivedError)
    {
      App.CurrentApp.Connection.InvokeWhenConnected((Action) (() =>
      {
        try
        {
          App.CurrentApp.Connection.SendPaymentGetTransactionsRequest(endDateTime, onReceivedOK, onReceivedError);
        }
        catch (Exception ex)
        {
          string context = string.Format("Sending sendGetTranRequest");
          Log.LogException(ex, context);
        }
      }));
      return true;
    }
  }
}
