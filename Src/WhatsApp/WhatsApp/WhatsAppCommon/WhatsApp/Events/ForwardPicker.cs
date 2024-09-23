// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.ForwardPicker
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class ForwardPicker : WamEvent
  {
    public wam_enum_forward_picker_result_type? forwardPickerResult;
    public bool? forwardPickerMulticastEnabled;
    public long? forwardPickerContactsSelected;
    public long? forwardPickerFrequentsNumberOfDays;
    public long? forwardPickerFrequentsLimit;
    public long? forwardPickerFrequentsDisplayed;
    public long? forwardPickerFrequentsSelected;
    public bool? forwardPickerSearchUsed;
    public long? forwardPickerSearchResultsSelected;
    public long? forwardPickerRecentsSelected;
    public long? forwardPickerSpendT;

    public void Reset()
    {
      this.forwardPickerResult = new wam_enum_forward_picker_result_type?();
      this.forwardPickerMulticastEnabled = new bool?();
      this.forwardPickerContactsSelected = new long?();
      this.forwardPickerFrequentsNumberOfDays = new long?();
      this.forwardPickerFrequentsLimit = new long?();
      this.forwardPickerFrequentsDisplayed = new long?();
      this.forwardPickerFrequentsSelected = new long?();
      this.forwardPickerSearchUsed = new bool?();
      this.forwardPickerSearchResultsSelected = new long?();
      this.forwardPickerRecentsSelected = new long?();
      this.forwardPickerSpendT = new long?();
    }

    public override uint GetCode() => 1034;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_forward_picker_result_type>(this.forwardPickerResult));
      Wam.MaybeSerializeField(2, this.forwardPickerMulticastEnabled);
      Wam.MaybeSerializeField(3, this.forwardPickerContactsSelected);
      Wam.MaybeSerializeField(4, this.forwardPickerFrequentsNumberOfDays);
      Wam.MaybeSerializeField(5, this.forwardPickerFrequentsLimit);
      Wam.MaybeSerializeField(6, this.forwardPickerFrequentsDisplayed);
      Wam.MaybeSerializeField(7, this.forwardPickerFrequentsSelected);
      Wam.MaybeSerializeField(8, this.forwardPickerSearchUsed);
      Wam.MaybeSerializeField(9, this.forwardPickerSearchResultsSelected);
      Wam.MaybeSerializeField(10, this.forwardPickerRecentsSelected);
      Wam.MaybeSerializeField(11, this.forwardPickerSpendT);
    }
  }
}
