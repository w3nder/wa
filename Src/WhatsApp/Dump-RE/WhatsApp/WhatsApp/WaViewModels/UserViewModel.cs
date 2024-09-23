// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaViewModels.UserViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp.WaViewModels
{
  public class UserViewModel : JidItemViewModel
  {
    private IEnumerable<WaRichText.Chunk> subtitleStrHighlights;
    protected IEnumerable<WaRichText.Chunk> titleStrHighlights;
    private System.Windows.Media.ImageSource defaultIcon;

    public UserStatus User { get; private set; }

    public override object Model => (object) this.User;

    public override string Key => this.Jid;

    public override string Jid => this.User.Jid;

    public override RichTextBlock.TextSet GetRichTitle()
    {
      string title = this.GetTitle();
      return new RichTextBlock.TextSet()
      {
        Text = title,
        SerializedFormatting = LinkDetector.GetMatches(Emoji.ConvertToUnicode(title)),
        PartialFormattings = this.titleStrHighlights
      };
    }

    public override string GetTitle() => this.User?.GetDisplayName() ?? "";

    public override RichTextBlock.TextSet GetSubtitle()
    {
      string status = this.User?.Status;
      if (status == null)
        return (RichTextBlock.TextSet) null;
      return new RichTextBlock.TextSet()
      {
        Text = Emoji.ConvertToTextOnly(status, (byte[]) null),
        PartialFormattings = this.subtitleStrHighlights
      };
    }

    public virtual bool ShowSubtleRightText => false;

    public virtual string SubtleRightText => "";

    public virtual bool ShowAccentRightText => false;

    public virtual string AccentRightText => "";

    public System.Windows.Media.ImageSource DefaultIcon
    {
      set => this.defaultIcon = value;
    }

    public UserViewModel(UserStatus user, bool genContextMenu = true)
    {
      this.User = user;
      this.GenerateContextMenu = genContextMenu;
    }

    public void CopySearchItemsFrom(UserViewModel other)
    {
      this.titleStrHighlights = other.titleStrHighlights;
      this.Notify("Title");
    }

    public void SetSearchResult(UserStatusSearchResult searchResult)
    {
      if (!searchResult.UserStatus.Jid.Equals(this.User.Jid))
        throw new ArgumentException("Result must match model item");
      if (this.titleStrHighlights != null || searchResult.ContactNameOffsets != null)
      {
        this.titleStrHighlights = (IEnumerable<WaRichText.Chunk>) ((IEnumerable<Pair<int, int>>) searchResult.ContactNameOffsets).Select<Pair<int, int>, WaRichText.Chunk>((Func<Pair<int, int>, WaRichText.Chunk>) (p => new WaRichText.Chunk(p.First, p.Second, WaRichText.Formats.Foreground, UIUtils.AccentColorCode))).ToArray<WaRichText.Chunk>();
        this.Notify("Title");
      }
      if (this.subtitleStrHighlights == null && searchResult.StatusOffsets == null)
        return;
      this.subtitleStrHighlights = (IEnumerable<WaRichText.Chunk>) ((IEnumerable<Pair<int, int>>) searchResult.StatusOffsets).Select<Pair<int, int>, WaRichText.Chunk>((Func<Pair<int, int>, WaRichText.Chunk>) (p => new WaRichText.Chunk(p.First, p.Second, WaRichText.Formats.Foreground, UIUtils.AccentColorCode))).ToArray<WaRichText.Chunk>();
      this.Notify("Subtitle");
    }

    public void ClearSearchResult()
    {
      if (this.titleStrHighlights != null)
      {
        this.titleStrHighlights = (IEnumerable<WaRichText.Chunk>) null;
        this.Notify("Title");
      }
      if (this.subtitleStrHighlights == null)
        return;
      this.subtitleStrHighlights = (IEnumerable<WaRichText.Chunk>) null;
      this.Notify("Subtitle");
    }

    public override bool GetCachedPicSource(out System.Windows.Media.ImageSource cached)
    {
      return this.User != null ? this.GetCachedPicSource(this.User.Jid, out cached) : base.GetCachedPicSource(out cached);
    }

    public override bool GetCachedPicSource(string jid, out System.Windows.Media.ImageSource cached)
    {
      return ChatPictureStore.GetCache(jid, out cached);
    }

    public override System.Windows.Media.ImageSource GetDefaultPicture()
    {
      return this.defaultIcon ?? (System.Windows.Media.ImageSource) AssetStore.DefaultContactIcon;
    }
  }
}
