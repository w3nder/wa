// Decompiled with JetBrains decompiler
// Type: WhatsApp.DateDividerViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;


namespace WhatsApp
{
  public class DateDividerViewModel : SystemMessageViewModel
  {
    private DateTime? displayTimestamp;
    public bool OverlayMode;

    public DateDividerViewModel(Message m, DateTime displayTimestamp)
      : base(m)
    {
      this.DisplayTimestamp = new DateTime?(displayTimestamp);
    }

    public override DateTime? DisplayTimestamp
    {
      get => this.displayTimestamp;
      set
      {
        DateTime? nullable = value;
        DateTime? displayTimestamp = this.displayTimestamp;
        if ((nullable.HasValue == displayTimestamp.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != displayTimestamp.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
          return;
        this.displayTimestamp = value;
        this.TextStrCache = (string) null;
        this.Notify("TextChanged");
      }
    }

    protected override string GetTextStr()
    {
      return DateTimeUtils.FormatFromDateSpan(this.DisplayTimestamp.Value);
    }

    public override bool ShowSystemMessageIcon => false;

    public override bool EnableContextMenu => false;

    public override int MessageID => -102;

    public override bool IsTapAllowed => false;
  }
}
