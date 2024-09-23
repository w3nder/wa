// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactCallLogItemViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Text;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class ContactCallLogItemViewModel : WaViewModelBase
  {
    public string TimestampStr
    {
      get
      {
        return this.CallRecord != null ? DateTimeUtils.FormatCompact(this.CallRecord.StartTime, DateTimeUtils.TimeDisplay.Always) : (string) null;
      }
    }

    public string CallInfoStr
    {
      get
      {
        if (this.CallRecord == null)
          return (string) null;
        StringBuilder stringBuilder = new StringBuilder();
        string str = !this.CallRecord.FromMe ? (this.CallRecord.Result == CallRecord.CallResult.Missed ? AppResources.CallLogItemMissedCall : AppResources.CallLogItemIncoming) : (this.CallRecord.Result == CallRecord.CallResult.Canceled ? AppResources.CallLogItemCanceled : AppResources.CallLogItemOutgoing);
        stringBuilder.AppendFormat("{0}   ", (object) str);
        TimeSpan duration = this.CallRecord.Duration;
        if (duration > TimeSpan.FromSeconds(0.0))
          stringBuilder.AppendFormat("{0} ", (object) DateTimeUtils.FormatDurationDescriptive((int) duration.TotalSeconds));
        long dataUsage = this.CallRecord.DataUsage;
        if (dataUsage > 0L)
          stringBuilder.AppendFormat("({0})", (object) Utils.FileSizeFormatter.Format(dataUsage));
        return stringBuilder.ToString();
      }
    }

    public CallRecord CallRecord { get; private set; }

    public ContactCallLogItemViewModel(CallRecord call) => this.CallRecord = call;
  }
}
