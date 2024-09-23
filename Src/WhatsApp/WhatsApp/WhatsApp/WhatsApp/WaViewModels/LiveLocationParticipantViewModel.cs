// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaViewModels.LiveLocationParticipantViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;


namespace WhatsApp.WaViewModels
{
  public class LiveLocationParticipantViewModel : UserViewModel
  {
    private string subtitle;
    private string chatJid;
    private bool isSelfJid;

    public virtual bool ShowRightButton => this.isSelfJid;

    public virtual string RightButtonText
    {
      get => !this.isSelfJid ? "" : AppResources.LiveLocationStopSharingLabel;
    }

    public override bool ShowSubtleRightText
    {
      get => !this.isSelfJid && this.User != null && !this.User.IsInDeviceContactList;
    }

    public override string SubtleRightText
    {
      get
      {
        return !string.IsNullOrEmpty(this.User?.PushName) ? string.Format("~{0}", (object) this.User.PushName) : "";
      }
    }

    public override string SubtitleStr => this.subtitle;

    public override RichTextBlock.TextSet GetSubtitle()
    {
      return new RichTextBlock.TextSet()
      {
        Text = this.subtitle,
        PartialFormattings = (IEnumerable<WaRichText.Chunk>) null
      };
    }

    public override bool ShowSubtitle => this.subtitle != "";

    public override bool EnableContextMenu => false;

    public override string GetTitle() => !this.isSelfJid ? base.GetTitle() : AppResources.You;

    public string ChatJid => this.chatJid;

    public void updateViewModel(int? expiration, DateTime ts)
    {
      this.subtitle = this.isSelfJid ? DateTimeUtils.FormatLiveLocationTimeLeft((long) expiration.Value) : DateTimeUtils.FormatLiveLocationUpdatedTime(ts);
      this.Notify("Subtitle");
    }

    public LiveLocationParticipantViewModel(
      string chatJid,
      UserStatus user,
      int? expiration,
      DateTime ts)
      : base(user)
    {
      this.chatJid = chatJid;
      this.isSelfJid = JidHelper.IsSelfJid(user.Jid);
      this.updateViewModel(expiration, ts);
    }
  }
}
