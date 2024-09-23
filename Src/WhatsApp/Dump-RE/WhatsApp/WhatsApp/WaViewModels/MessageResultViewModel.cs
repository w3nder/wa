// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaViewModels.MessageResultViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;

#nullable disable
namespace WhatsApp.WaViewModels
{
  public class MessageResultViewModel : ChatItemViewModel
  {
    private MessageSearchResult searchResult;

    public MessageSearchResult SearchResult
    {
      get => this.searchResult;
      set
      {
        this.searchResult = value;
        this.Notify("Refresh");
      }
    }

    public override string Key => this.searchResult.Message.MessageID.ToString();

    public MessageResultViewModel(Conversation convo, MessageSearchResult res)
      : base(convo, res.Message)
    {
      this.searchResult = res;
      this.EnableChatPreview = true;
    }

    public override bool ShouldHighlight => false;

    public override string TimestampStr
    {
      get
      {
        DateTime? localTimestamp = this.searchResult.Message.LocalTimestamp;
        return !localTimestamp.HasValue ? (string) null : DateTimeUtils.FormatCompact(localTimestamp.Value, DateTimeUtils.TimeDisplay.SameDayOnly);
      }
    }
  }
}
