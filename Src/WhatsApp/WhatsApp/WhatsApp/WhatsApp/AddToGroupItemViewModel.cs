// Decompiled with JetBrains decompiler
// Type: WhatsApp.AddToGroupItemViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class AddToGroupItemViewModel : ChatItemViewModel
  {
    public AddToGroupItemViewModel(Conversation convo)
      : base(convo)
    {
    }

    public override RichTextBlock.TextSet GetSubtitle()
    {
      RichTextBlock.TextSet subtitle = new RichTextBlock.TextSet();
      if (this.Conversation == null)
        subtitle.Text = "";
      else if (!this.Conversation.IsGroupParticipant())
        subtitle.Text = AppResources.NotAParticipant;
      else if (this.TargetIsParticipant)
        subtitle.Text = AppResources.AlreadyInGroup;
      else if (!this.SelfIsAdmin)
      {
        subtitle.Text = AppResources.NotAdmin;
      }
      else
      {
        subtitle.Text = this.Conversation.GetParticipantNames(12);
        if (string.IsNullOrEmpty(subtitle.Text))
          subtitle.Text = AppResources.You;
      }
      return subtitle;
    }

    public override bool ShowLabel => false;

    public override bool ShouldDisable()
    {
      return !this.SelfIsAdmin || this.TargetIsParticipant || this.Conversation.IsReadOnly();
    }

    public bool SelfIsAdmin { get; set; }

    public bool TargetIsParticipant { get; set; }
  }
}
