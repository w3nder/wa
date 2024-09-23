// Decompiled with JetBrains decompiler
// Type: WhatsApp.MessageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WhatsApp.WaCollections;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class MessageViewModel : WaViewModelBase
  {
    private static double? defaultBubbleWidth = new double?();
    private static double? defaultContentWidth = new double?();
    private static KeyValueCache<string, MessageViewModel.ThumbnailState> cachedThumbnails = (KeyValueCache<string, MessageViewModel.ThumbnailState>) null;
    private string logHeader;
    private IDisposable propertyChangedSub;
    private IDisposable thumbnailSub;
    private MessageContextInfoWrapper contextInfo;
    private MessageViewModel quotedMsgVm;
    private PaymentsCurrency paymentCurrency;
    private double paymentAmount;
    private static long forwardedMsgUiTs = -1;
    private bool? shouldShowForwardMessageRow;
    private Size? targetThumbSize;
    private string pushName;
    private string displayName;
    private string senderJid;
    private UserStatus sender;
    protected bool isGroupMsg;
    private MessageViewModel.GroupingPosition groupedPosition;
    private bool? isPsaChat;
    public static Size TailSize = new Size(15.0 * ResolutionHelper.ZoomMultiplier, 10.0 * ResolutionHelper.ZoomMultiplier);
    public static double TailShift = 2.0 * ResolutionHelper.ZoomMultiplier;
    private static SolidColorBrush darkerMsgBgBrush = (SolidColorBrush) null;
    private bool? textHasRichContent;
    private string textStrCache;
    private static double smallTextFontSize = 18.667 * ResolutionHelper.ZoomMultiplier;
    private bool? allowLinks;
    private bool checkedSuspicious;
    private MessageViewModel.StatusIconTypes statusIconSourceCacheType;
    private WeakReference<System.Windows.Media.ImageSource> statusIconSourceCache;
    private string timestampStr;
    private string starredMsgSenderStr;
    private static int? largeThumbPixelWidth = new int?();
    private bool isLazyInited;

    public static double DefaultBubbleWidth
    {
      get
      {
        if (!MessageViewModel.defaultBubbleWidth.HasValue)
          MessageViewModel.defaultBubbleWidth = new double?((480.0 - 48.0 * ResolutionHelper.ZoomMultiplier) * 0.75);
        return MessageViewModel.defaultBubbleWidth.Value;
      }
    }

    public static double DefaultContentWidth
    {
      get
      {
        if (!MessageViewModel.defaultContentWidth.HasValue)
          MessageViewModel.defaultContentWidth = new double?(MessageViewModel.DefaultBubbleWidth - 24.0 * ResolutionHelper.ZoomMultiplier);
        return MessageViewModel.defaultContentWidth.Value;
      }
    }

    private static KeyValueCache<string, MessageViewModel.ThumbnailState> CachedThumbnails
    {
      get
      {
        if (MessageViewModel.cachedThumbnails == null)
          MessageViewModel.cachedThumbnails = new KeyValueCache<string, MessageViewModel.ThumbnailState>(AppState.IsDecentMemoryDevice ? 20 : 10, true);
        return MessageViewModel.cachedThumbnails;
      }
    }

    protected string LogHeader
    {
      get
      {
        return this.logHeader ?? (this.logHeader = string.Format("mvm > hash:{0} mid:{1} key:{2}", (object) this.GetHashCode(), (object) this.Message.MessageID, (object) this.Message.KeyId));
      }
    }

    public Message Message { get; private set; }

    public Set<MessageMenu.MessageMenuItem> ExcludedMenuItems { get; set; }

    public MessageViewModel QuotedMessageViewModel
    {
      get
      {
        if (this.quotedMsgVm == null && this.contextInfo != null)
          this.quotedMsgVm = MessageViewModel.Create(this.contextInfo.QuotedMessage, new Action<MessageViewModel>(this.SetQuotedMediaUri));
        return this.quotedMsgVm;
      }
    }

    public bool ShouldShowQuoteViewPanel
    {
      get
      {
        if (this.Message.MediaWaType == FunXMPP.FMessage.Type.Revoked || this.QuotedMessage == null && (this.QuotedGroupJid == null || string.IsNullOrEmpty(this.QuoteSenderNameStr)) || this.IsForGalleryView)
          return false;
        return this.MergedPosition == MessageViewModel.GroupingPosition.None || this.MergedPosition == MessageViewModel.GroupingPosition.Top;
      }
    }

    public Message QuotedMessage => this.contextInfo?.QuotedMessage;

    public string QuotedGroupJid
    {
      get
      {
        string quoteRemoteJid = this.contextInfo?.QuoteRemoteJid;
        return !JidHelper.IsGroupJid(quoteRemoteJid) ? (string) null : quoteRemoteJid;
      }
      set
      {
        if (this.contextInfo == null)
          return;
        this.QuotedGroupJid = value;
      }
    }

    public string QuoteSenderNameStr
    {
      get
      {
        string r = this.QuotedMessageViewModel?.DisplayNameStr ?? "";
        Message quotedMessage = this.QuotedMessage;
        if ((quotedMessage != null ? (quotedMessage.IsStatus() ? 1 : 0) : 0) != 0)
          r = string.Format("{0} {1} {2}", (object) r, (object) '·', (object) AppResources.StatusV3Title);
        else if (this.QuotedGroupJid != null)
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => r = string.Format("{0} {1} {2}", (object) r, (object) '·', (object) (db.GetConversation(this.QuotedGroupJid, CreateOptions.None)?.GroupSubject ?? ""))));
        return r;
      }
    }

    public string QuotedMediaStr
    {
      get
      {
        string quotedMediaStr = (string) null;
        Message quotedMessage = this.contextInfo?.QuotedMessage;
        if (quotedMessage != null)
        {
          switch (quotedMessage.MediaWaType)
          {
            case FunXMPP.FMessage.Type.Image:
            case FunXMPP.FMessage.Type.Gif:
              quotedMediaStr = AppResources.MediaImage;
              break;
            case FunXMPP.FMessage.Type.Audio:
              quotedMediaStr = quotedMessage.IsPtt() ? DateTimeUtils.FormatDuration(quotedMessage.MediaDurationSeconds) : AppResources.MediaAudio;
              break;
            case FunXMPP.FMessage.Type.Video:
              quotedMediaStr = AppResources.MediaVideo;
              break;
            case FunXMPP.FMessage.Type.Contact:
              quotedMediaStr = quotedMessage.MediaName;
              break;
            case FunXMPP.FMessage.Type.Location:
            case FunXMPP.FMessage.Type.LiveLocation:
              quotedMediaStr = quotedMessage.GetPreviewText(true, false);
              break;
            case FunXMPP.FMessage.Type.Document:
              DocumentMessageWrapper documentMessageWrapper = new DocumentMessageWrapper(quotedMessage);
              string str = documentMessageWrapper.PageCount > 0 ? Plurals.Instance.GetString(AppResources.DocPagesPlural, documentMessageWrapper.PageCount) : (string) null;
              quotedMediaStr = string.Join(string.Format("{0}•{1}", (object) ' ', (object) ' '), ((IEnumerable<string>) new string[2]
              {
                documentMessageWrapper.Title,
                str
              }).Where<string>((Func<string, bool>) (s => !string.IsNullOrEmpty(s))));
              break;
          }
        }
        return quotedMediaStr;
      }
    }

    public Thickness QuotedMessageViewPanelMargin
    {
      get
      {
        double num = (this.Message.IsPtt() ? 6.0 : 12.0) * this.zoomMultiplier;
        return new Thickness(num, this.ShouldShowHeader ? 0.0 : num, num, this.ShouldFillContentBackground ? 0.0 : num);
      }
    }

    public System.Windows.Media.ImageSource QuoteMediaIconSource
    {
      get => this.GetMediaIconSource(this.contextInfo?.QuotedMessage, false);
    }

    public System.Windows.Media.ImageSource QuotePreviewMediaIconSource
    {
      get => this.GetMediaIconSource(this.Message, true);
    }

    public string QuotePreviewSenderNameStr
    {
      get
      {
        string r = this.DisplayNameStr ?? "";
        Message message = this.Message;
        if ((message != null ? (message.IsStatus() ? 1 : 0) : 0) != 0)
          r = string.Format("{0} {1} {2}", (object) r, (object) '·', (object) AppResources.StatusV3Title);
        else if (JidHelper.IsGroupJid(this.Message?.KeyRemoteJid))
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => r = string.Format("{0} {1} {2}", (object) r, (object) '·', (object) (db.GetConversation(this.Message?.KeyRemoteJid, CreateOptions.None)?.GroupSubject ?? ""))));
        return r;
      }
    }

    public bool ShouldShowPayment
    {
      get
      {
        return this.Message.MediaWaType != FunXMPP.FMessage.Type.Revoked && this.Message.HasPaymentInfo();
      }
    }

    public string PaymentCurrencyStr => this.paymentCurrency?.CurrencySymbol ?? "";

    public string PaymentAmountIntegerStr => ((int) this.paymentAmount).ToString();

    public string PaymentAmountDecimalStr
    {
      get => string.Format(".{0:D2}", (object) ((int) (this.paymentAmount * 100.0) % 100));
    }

    public RichTextBlock.TextSet PaymentInfoStr
    {
      get
      {
        RichTextBlock.TextSet paymentInfoStr = (RichTextBlock.TextSet) null;
        if (this.paymentCurrency != null && this.Message.InternalProperties.PaymentsPropertiesField.Receiver != null)
        {
          string str1 = this.paymentCurrency.FormatAmount(this.paymentAmount);
          string str2 = (string) null;
          UserStatus userStatus = UserCache.Get(this.Message.InternalProperties.PaymentsPropertiesField.Receiver, false);
          if (userStatus != null)
          {
            str2 = userStatus.GetDisplayName(true, false, false);
            if (string.IsNullOrEmpty(str2) && !string.IsNullOrEmpty(userStatus.PushName))
              str2 = string.Format("~{0}", (object) userStatus.PushName);
          }
          if (string.IsNullOrEmpty(str2))
            str2 = JidHelper.GetPhoneNumber(this.Message.InternalProperties.PaymentsPropertiesField.Receiver, true);
          string s = string.Format(AppResources.SentPaymentTo, (object) str1, (object) str2);
          WaRichText.Chunk chunk = ((IEnumerable<WaRichText.Chunk>) WaRichText.GetHtmlLinkChunks(s)).SingleOrDefault<WaRichText.Chunk>();
          if (chunk != null)
          {
            chunk.Format = WaRichText.Formats.Bold | WaRichText.Formats.Foreground;
            chunk.ClickAction = (Action) null;
            chunk.AuxiliaryInfo = UIUtils.WhiteBrush.Color.ToString();
          }
          RichTextBlock.TextSet textSet1 = new RichTextBlock.TextSet();
          textSet1.Text = s;
          RichTextBlock.TextSet textSet2 = textSet1;
          WaRichText.Chunk[] chunkArray;
          if (chunk != null)
            chunkArray = new WaRichText.Chunk[1]{ chunk };
          else
            chunkArray = (WaRichText.Chunk[]) null;
          textSet2.PartialFormattings = (IEnumerable<WaRichText.Chunk>) chunkArray;
          paymentInfoStr = textSet1;
        }
        return paymentInfoStr;
      }
    }

    public bool ShouldShowForwardedMessageRow
    {
      get
      {
        if (this.Message.MediaWaType == FunXMPP.FMessage.Type.Revoked || this.Message.MediaWaType == FunXMPP.FMessage.Type.Sticker)
          return false;
        if (this.shouldShowForwardMessageRow.HasValue)
          return this.shouldShowForwardMessageRow.Value;
        if (MessageViewModel.forwardedMsgUiTs < 0L)
          MessageViewModel.forwardedMsgUiTs = Settings.ForwardedMsgUiTs;
        this.shouldShowForwardMessageRow = MessageViewModel.forwardedMsgUiTs <= 0L || MessageViewModel.forwardedMsgUiTs >= this.Message.TimestampLong ? new bool?(false) : new bool?(((int) WhatsApp.ProtoBuf.Message.CreateFromPlainText(this.Message.ProtoBuf)?.GetContextInfo()?.IsForwarded ?? 0) != 0);
        return this.shouldShowForwardMessageRow.Value;
      }
    }

    public virtual Thickness ForwardedRowMargin
    {
      get => new Thickness(6.0, this.ShouldShowHeader ? -4.0 : 6.0, 6.0, -4.0);
    }

    private System.Windows.Media.ImageSource GetMediaIconSource(Message m, bool themeDependent)
    {
      if (m == null)
        return (System.Windows.Media.ImageSource) null;
      if (m.IsStatus())
        return !themeDependent ? (System.Windows.Media.ImageSource) AssetStore.InlineStatusWhite : (System.Windows.Media.ImageSource) AssetStore.InlineStatus;
      System.Windows.Media.ImageSource mediaIconSource = (System.Windows.Media.ImageSource) null;
      switch (m.MediaWaType)
      {
        case FunXMPP.FMessage.Type.Image:
        case FunXMPP.FMessage.Type.Gif:
          mediaIconSource = themeDependent ? (System.Windows.Media.ImageSource) AssetStore.InlinePicture : (System.Windows.Media.ImageSource) AssetStore.InlinePictureWhite;
          break;
        case FunXMPP.FMessage.Type.Audio:
          mediaIconSource = !m.IsPtt() ? (System.Windows.Media.ImageSource) AssetStore.InlineAudioSubtle : (m.IsPlayedByTarget() ? (System.Windows.Media.ImageSource) AssetStore.InlineMicBlue : (m.KeyFromMe ? (System.Windows.Media.ImageSource) AssetStore.InlineMicWhite : (System.Windows.Media.ImageSource) AssetStore.InlineMicGreen));
          break;
        case FunXMPP.FMessage.Type.Video:
          mediaIconSource = themeDependent ? (System.Windows.Media.ImageSource) AssetStore.InlineVideo : (System.Windows.Media.ImageSource) AssetStore.InlineVideoWhite;
          break;
        case FunXMPP.FMessage.Type.Contact:
          mediaIconSource = themeDependent ? (System.Windows.Media.ImageSource) AssetStore.InlineContact : (System.Windows.Media.ImageSource) AssetStore.InlineContactWhite;
          break;
        case FunXMPP.FMessage.Type.Location:
          mediaIconSource = themeDependent ? (System.Windows.Media.ImageSource) AssetStore.InlineLocation : (System.Windows.Media.ImageSource) AssetStore.InlineLocationWhite;
          break;
        case FunXMPP.FMessage.Type.Document:
          mediaIconSource = themeDependent ? (System.Windows.Media.ImageSource) AssetStore.InlineDoc : (System.Windows.Media.ImageSource) AssetStore.InlineDocWhite;
          break;
        case FunXMPP.FMessage.Type.LiveLocation:
          mediaIconSource = themeDependent ? (System.Windows.Media.ImageSource) AssetStore.InlineLiveLocation : (System.Windows.Media.ImageSource) AssetStore.InlineLiveLocationWhite;
          break;
      }
      return mediaIconSource;
    }

    public virtual string SenderJid
    {
      get => this.senderJid ?? (this.senderJid = this.Message.GetSenderJid());
      set => this.senderJid = value;
    }

    public UserStatus Sender => this.sender ?? (this.sender = UserCache.Get(this.SenderJid, true));

    public bool IsIncomingGroupMessage => this.isGroupMsg && !this.Message.KeyFromMe;

    protected MessageViewModel.GroupingPosition GroupedPosition
    {
      get => this.groupedPosition;
      set
      {
        if (this.groupedPosition == value)
          return;
        this.groupedPosition = value;
        this.Notify("GroupedPositionChanged");
      }
    }

    public MessageViewModel.ChopState TextChop { get; set; }

    public MessageViewModel.GroupingPosition MergedPosition { get; set; }

    public bool IsForStarredView { get; set; }

    public bool IsForGalleryView { get; set; }

    public bool IsForContactCardView => this.JidForContactCardView != null;

    public string JidForContactCardView { get; set; }

    private bool RenderInvertedly { get; set; }

    public bool ShouldShowOnOutgoingSide => this.Message.KeyFromMe && !this.IsForStarredView;

    public bool IsPsaChat
    {
      get
      {
        if (!this.isPsaChat.HasValue)
          this.isPsaChat = new bool?(JidHelper.IsPsaJid(this.Message.KeyRemoteJid));
        return this.isPsaChat.Value;
      }
    }

    public virtual bool EnableContextMenu => true;

    public double TransformScaleY => this.RenderInvertedly ? -1.0 : 1.0;

    public virtual double ViewPanelTopMargin
    {
      get
      {
        return !this.ShouldShowHeader || this.QuotedMessage != null ? 12.0 * this.zoomMultiplier : 0.0;
      }
    }

    public virtual Thickness ViewPanelMargin
    {
      get
      {
        double num = 12.0 * this.zoomMultiplier;
        return new Thickness(num, this.ViewPanelTopMargin, num, num);
      }
    }

    public double ThumbnailWidth => this.GetTargetThumbnailSize().Width;

    public double ThumbnailHeight => this.GetTargetThumbnailSize().Height;

    public virtual LinkDetector.Result[] TextPerformanceHint
    {
      get
      {
        LinkDetector.Result[] richTextFormattings = this.Message.GetRichTextFormattings();
        List<LinkDetector.Result> list = richTextFormattings != null ? ((IEnumerable<LinkDetector.Result>) richTextFormattings).ToList<LinkDetector.Result>() : (List<LinkDetector.Result>) null;
        if (list != null && this.ShouldAddFooterPlaceHolder)
        {
          string textStrCache = this.TextStrCache;
          string footerSpaceHolder = this.FooterSpaceHolder;
          LinkDetector.Result result = new LinkDetector.Result(textStrCache.Length, footerSpaceHolder.Length, 0, (Func<string>) (() => footerSpaceHolder));
          list.Add(result);
        }
        return list?.ToArray();
      }
    }

    public MessageSearchResult SearchResult { get; set; }

    public virtual WaRichText.Chunk[] InlineFormattings
    {
      get
      {
        if (this.SearchResult == null)
          return (WaRichText.Chunk[]) null;
        Pair<int, int>[] source = (Pair<int, int>[]) null;
        switch (this.Message.MediaWaType)
        {
          case FunXMPP.FMessage.Type.Image:
          case FunXMPP.FMessage.Type.Video:
          case FunXMPP.FMessage.Type.Gif:
          case FunXMPP.FMessage.Type.Sticker:
            source = this.SearchResult.MediaCaptionOffsets;
            break;
          case FunXMPP.FMessage.Type.Contact:
            source = this.SearchResult.MediaNameOffsets;
            break;
          case FunXMPP.FMessage.Type.Location:
          case FunXMPP.FMessage.Type.LiveLocation:
            source = this.SearchResult.LocationDetailsOffsets;
            break;
        }
        return source != null ? ((IEnumerable<Pair<int, int>>) source).Select<Pair<int, int>, WaRichText.Chunk>((Func<Pair<int, int>, WaRichText.Chunk>) (p => new WaRichText.Chunk(p.First, p.Second, WaRichText.Formats.Bold))).ToArray<WaRichText.Chunk>() : (WaRichText.Chunk[]) null;
      }
    }

    public virtual bool ShouldShowStarredViewHeader
    {
      get
      {
        return this.MergedPosition == MessageViewModel.GroupingPosition.None || this.MergedPosition == MessageViewModel.GroupingPosition.Top;
      }
    }

    public virtual bool ShouldShowFooter
    {
      get
      {
        return this.MergedPosition == MessageViewModel.GroupingPosition.None || this.MergedPosition == MessageViewModel.GroupingPosition.Bottom;
      }
    }

    public virtual Thickness FooterMargin
    {
      get => new Thickness(0.0, 0.0, 6.0 * this.zoomMultiplier, 6.0 * this.zoomMultiplier);
    }

    public virtual Thickness FooterPanelPadding => new Thickness(0.0);

    public virtual bool ShouldShowFooterInfo => false;

    public virtual string FooterInfoStr => (string) null;

    public string FooterSpaceHolder
    {
      get
      {
        return string.Format("\t{0}", (object) new string(' ', (int) ((double) this.TimestampStr.Length * 1.5 + (this.Message.IsStarred ? 12.0 : 8.0))));
      }
    }

    public virtual bool ShouldUseFooterProtection => false;

    public virtual bool ShouldShowTail
    {
      get
      {
        return !this.IsForGalleryView && (this.MergedPosition == MessageViewModel.GroupingPosition.None || (this.ShouldShowOnOutgoingSide ? (this.MergedPosition == MessageViewModel.GroupingPosition.Bottom ? 1 : 0) : (this.MergedPosition == MessageViewModel.GroupingPosition.Top ? 1 : 0)) != 0) && (this.GroupedPosition == MessageViewModel.GroupingPosition.None || (this.ShouldShowOnOutgoingSide ? (this.GroupedPosition == MessageViewModel.GroupingPosition.Bottom ? 1 : 0) : (this.GroupedPosition == MessageViewModel.GroupingPosition.Top ? 1 : 0)) != 0) && !this.IsPsaChat;
      }
    }

    public virtual double MaxBubbleWidth => MessageViewModel.DefaultBubbleWidth;

    public virtual Thickness BubbleMargin
    {
      get
      {
        double top = 0.0;
        double bottom;
        if (this.IsForStarredView)
        {
          top = 3.0 * this.zoomMultiplier;
          bottom = this.MergedPosition == MessageViewModel.GroupingPosition.Top || this.MergedPosition == MessageViewModel.GroupingPosition.Middle ? -6.0 * this.zoomMultiplier : MessageViewModel.TailSize.Height;
        }
        else if (this.IsForGalleryView)
          bottom = 24.0 * this.zoomMultiplier;
        else if (this.Message.KeyFromMe)
        {
          bottom = 0.0;
          top = this.MergedPosition == MessageViewModel.GroupingPosition.Bottom || this.MergedPosition == MessageViewModel.GroupingPosition.Middle ? -6.0 * this.zoomMultiplier : (this.GroupedPosition == MessageViewModel.GroupingPosition.Bottom || this.GroupedPosition == MessageViewModel.GroupingPosition.Middle ? 4.0 * this.zoomMultiplier : MessageViewModel.TailSize.Height);
        }
        else
        {
          top = 0.0;
          bottom = this.MergedPosition == MessageViewModel.GroupingPosition.Top || this.MergedPosition == MessageViewModel.GroupingPosition.Middle ? -6.0 * this.zoomMultiplier : (this.GroupedPosition == MessageViewModel.GroupingPosition.Top || this.GroupedPosition == MessageViewModel.GroupingPosition.Middle ? 4.0 * this.zoomMultiplier : MessageViewModel.TailSize.Height);
        }
        return new Thickness(0.0, top, 0.0, bottom);
      }
    }

    private static SolidColorBrush DarkerMsgBgBrush
    {
      get
      {
        if (MessageViewModel.darkerMsgBgBrush == null)
        {
          Color color = UIUtils.AccentBrush.Color;
          double num = 0.3;
          MessageViewModel.darkerMsgBgBrush = new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) ((double) color.R * num), (byte) ((double) color.G * num), (byte) ((double) color.B * num)));
        }
        return MessageViewModel.darkerMsgBgBrush;
      }
    }

    public virtual SolidColorBrush BackgroundBrush
    {
      get
      {
        if (this.SearchResult != null)
          return MessageViewModel.DarkerMsgBgBrush;
        return !this.Message.KeyFromMe ? UIUtils.AccentBrush : UIUtils.DarkAccentBrush;
      }
    }

    public SolidColorBrush MaskedBackgroundBrush
    {
      get
      {
        Color color1 = this.BackgroundBrush.Color;
        Color color2 = Color.FromArgb((byte) 51, byte.MaxValue, byte.MaxValue, byte.MaxValue);
        double num = (double) color2.A / (double) byte.MaxValue;
        return new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) ((double) color2.R * num + (double) color1.R * (1.0 - num)), (byte) ((double) color2.G * num + (double) color1.G * (1.0 - num)), (byte) ((double) color2.B * num + (double) color1.B * (1.0 - num))));
      }
    }

    public virtual Brush ForegroundBrush => (Brush) UIUtils.WhiteBrush;

    public virtual HorizontalAlignment HorizontalAlignment
    {
      get => !this.Message.KeyFromMe ? HorizontalAlignment.Left : HorizontalAlignment.Right;
    }

    public bool TextHasRichContent
    {
      get
      {
        if (!this.textHasRichContent.HasValue && this.Message.TextPerformanceHint != null)
          this.textHasRichContent = WaRichText.BufferContainsValidChunks(this.Message.TextPerformanceHint);
        return this.textHasRichContent ?? true;
      }
    }

    public virtual bool ShouldShowText => false;

    protected string TextStrCache
    {
      get
      {
        if (this.textStrCache == null)
        {
          string str = this.GetTextStr()?.Replace(char.MinValue, '\u200B') ?? "";
          this.textStrCache = !this.ShouldAddFooterPlaceHolder || string.IsNullOrEmpty(str) ? str : string.Format("{0}{1}", (object) str, (object) this.FooterSpaceHolder);
        }
        return this.textStrCache;
      }
      set => this.textStrCache = value;
    }

    protected virtual bool ShouldAddFooterPlaceHolder => false;

    public string TextStr => this.TextStrCache;

    public FlowDirection? TextFlowDirection
    {
      get => BidiLanguageExtensions.getFlowDirection(this.TextStr);
    }

    public double TextFontSize => Settings.SystemFontSize * ResolutionHelper.ZoomMultiplier;

    public static double SmallTextFontSizeStatic => MessageViewModel.smallTextFontSize;

    public double SmallTextFontSize => MessageViewModel.smallTextFontSize;

    public bool AllowLinks
    {
      get
      {
        if (this.allowLinks.HasValue)
          return this.allowLinks.Value;
        if (this.Message.MediaWaType == FunXMPP.FMessage.Type.Undefined || this.Message.MediaWaType == FunXMPP.FMessage.Type.ExtendedText || this.Message.MediaCaption != null)
        {
          if (!WaRichText.ContainsFormat((IEnumerable<LinkDetector.Result>) this.TextPerformanceHint, WaRichText.Formats.Link))
            this.allowLinks = new bool?(false);
          else if (!this.checkedSuspicious)
          {
            this.checkedSuspicious = true;
            AppState.Worker.Enqueue((Action) (() =>
            {
              MessagesContext.Run((MessagesContext.MessagesCallback) (db => this.allowLinks = new bool?(SuspiciousJid.ShouldAllowLinksForJid(db, this.Message.KeyRemoteJid))));
              MessageViewModel.InvokeAsync((Action) (() => this.Notify("AllowLinksChanged")));
            }));
          }
        }
        else
          this.allowLinks = new bool?(false);
        return this.allowLinks ?? false;
      }
      set
      {
        this.allowLinks = new bool?(value);
        this.Notify("AllowLinksChanged");
      }
    }

    public int HeaderRow => !this.Message.IsPtt() || this.QuotedMessage != null ? 1 : 4;

    public virtual bool ShouldShowHeader
    {
      get
      {
        if (!this.IsIncomingGroupMessage || this.IsForStarredView)
          return false;
        bool shouldShowHeader = true;
        if (this.Message.MediaWaType == FunXMPP.FMessage.Type.Undefined)
          shouldShowHeader = this.Message.HasPaymentInfo() || (this.GroupedPosition == MessageViewModel.GroupingPosition.None || this.GroupedPosition == MessageViewModel.GroupingPosition.Top) && (this.MergedPosition == MessageViewModel.GroupingPosition.None || this.MergedPosition == MessageViewModel.GroupingPosition.Top);
        return shouldShowHeader;
      }
    }

    public virtual bool ShouldUseSeparateHeaderLine => this.IsIncomingGroupMessage;

    public bool ShouldShowPushName
    {
      get
      {
        if (!this.isGroupMsg)
          return false;
        this.GetSenderNames();
        return !string.IsNullOrEmpty(this.pushName);
      }
    }

    public string PushNameStr
    {
      get
      {
        if (!this.isGroupMsg)
          return (string) null;
        this.GetSenderNames();
        return this.pushName;
      }
    }

    public string DisplayNameStr
    {
      get
      {
        this.GetSenderNames();
        return this.displayName;
      }
    }

    protected virtual bool ShouldShowFooterInAccentColor => false;

    private MessageViewModel.StatusIconTypes StatusIconSourceType
    {
      get
      {
        if (this.Message.KeyFromMe)
        {
          switch (this.Message.Status)
          {
            case FunXMPP.FMessage.Status.Uploading:
            case FunXMPP.FMessage.Status.Unsent:
            case FunXMPP.FMessage.Status.UploadingCustomHash:
            case FunXMPP.FMessage.Status.Pending:
              if (this.ShouldShowFooterInAccentColor)
                return MessageViewModel.StatusIconTypes.InlineAccentClock;
              return this.ShouldUseFooterProtection ? MessageViewModel.StatusIconTypes.InlineSolidWhiteClock : MessageViewModel.StatusIconTypes.InlineWhiteClock;
            case FunXMPP.FMessage.Status.ReceivedByServer:
              if (this.ShouldShowFooterInAccentColor)
                return MessageViewModel.StatusIconTypes.InlineAccentCheck;
              return this.ShouldUseFooterProtection ? MessageViewModel.StatusIconTypes.InlineSolidWhiteCheck : MessageViewModel.StatusIconTypes.InlineWhiteCheck;
            case FunXMPP.FMessage.Status.ReceivedByTarget:
              if (this.ShouldShowFooterInAccentColor)
                return MessageViewModel.StatusIconTypes.InlineAccentDoubleChecks;
              return this.ShouldUseFooterProtection ? MessageViewModel.StatusIconTypes.InlineSolidWhiteDoubleChecks : MessageViewModel.StatusIconTypes.InlineWhiteDoubleChecks;
            case FunXMPP.FMessage.Status.Error:
              if (this.ShouldShowFooterInAccentColor)
                return MessageViewModel.StatusIconTypes.InlineAccentError;
              return this.ShouldUseFooterProtection ? MessageViewModel.StatusIconTypes.InlineSolidWhiteError : MessageViewModel.StatusIconTypes.InlineWhiteError;
            case FunXMPP.FMessage.Status.PlayedByTarget:
            case FunXMPP.FMessage.Status.ObsoletePlayedByTargetAcked:
            case FunXMPP.FMessage.Status.ReadByTarget:
            case FunXMPP.FMessage.Status.ObsoleteReadByTargetAcked:
              return MessageViewModel.StatusIconTypes.InlineBlueChecks;
          }
        }
        return MessageViewModel.StatusIconTypes.None;
      }
    }

    public System.Windows.Media.ImageSource StatusIconSource
    {
      get
      {
        System.Windows.Media.ImageSource target = (System.Windows.Media.ImageSource) null;
        if (this.statusIconSourceCache != null)
          this.statusIconSourceCache.TryGetTarget(out target);
        MessageViewModel.StatusIconTypes statusIconSourceType = this.StatusIconSourceType;
        if (statusIconSourceType != this.statusIconSourceCacheType || statusIconSourceType != MessageViewModel.StatusIconTypes.None && target == null)
        {
          switch (statusIconSourceType)
          {
            case MessageViewModel.StatusIconTypes.None:
              target = (System.Windows.Media.ImageSource) null;
              break;
            case MessageViewModel.StatusIconTypes.InlineAccentClock:
              target = (System.Windows.Media.ImageSource) AssetStore.InlineAccentClock;
              break;
            case MessageViewModel.StatusIconTypes.InlineWhiteClock:
              target = (System.Windows.Media.ImageSource) AssetStore.InlineWhiteClock;
              break;
            case MessageViewModel.StatusIconTypes.InlineSolidWhiteClock:
              target = (System.Windows.Media.ImageSource) AssetStore.InlineSolidWhiteClock;
              break;
            case MessageViewModel.StatusIconTypes.InlineAccentCheck:
              target = (System.Windows.Media.ImageSource) AssetStore.InlineAccentCheck;
              break;
            case MessageViewModel.StatusIconTypes.InlineWhiteCheck:
              target = (System.Windows.Media.ImageSource) AssetStore.InlineWhiteCheck;
              break;
            case MessageViewModel.StatusIconTypes.InlineSolidWhiteCheck:
              target = (System.Windows.Media.ImageSource) AssetStore.InlineSolidWhiteCheck;
              break;
            case MessageViewModel.StatusIconTypes.InlineAccentDoubleChecks:
              target = (System.Windows.Media.ImageSource) AssetStore.InlineAccentDoubleChecks;
              break;
            case MessageViewModel.StatusIconTypes.InlineWhiteDoubleChecks:
              target = (System.Windows.Media.ImageSource) AssetStore.InlineWhiteDoubleChecks;
              break;
            case MessageViewModel.StatusIconTypes.InlineSolidWhiteDoubleChecks:
              target = (System.Windows.Media.ImageSource) AssetStore.InlineSolidWhiteDoubleChecks;
              break;
            case MessageViewModel.StatusIconTypes.InlineBlueChecks:
              target = (System.Windows.Media.ImageSource) AssetStore.InlineBlueChecks;
              break;
            case MessageViewModel.StatusIconTypes.InlineAccentError:
              target = (System.Windows.Media.ImageSource) AssetStore.InlineAccentError;
              break;
            case MessageViewModel.StatusIconTypes.InlineWhiteError:
              target = (System.Windows.Media.ImageSource) AssetStore.InlineWhiteError;
              break;
            case MessageViewModel.StatusIconTypes.InlineSolidWhiteError:
              target = (System.Windows.Media.ImageSource) AssetStore.InlineSolidWhiteError;
              break;
          }
          this.statusIconSourceCacheType = statusIconSourceType;
          this.statusIconSourceCache = new WeakReference<System.Windows.Media.ImageSource>(target);
        }
        return target;
      }
    }

    public System.Windows.Media.ImageSource BroadcastIconSource
    {
      get
      {
        return !this.ShouldShowFooterInAccentColor ? (System.Windows.Media.ImageSource) AssetStore.InlineWhiteBroadcast : (System.Windows.Media.ImageSource) AssetStore.InlineAccentBroadcast;
      }
    }

    public bool ShouldShowStatusIcon
    {
      get
      {
        if (!this.Message.KeyFromMe || this.IsForGalleryView)
          return false;
        return this.Message.Status == FunXMPP.FMessage.Status.Unsent || this.Message.MediaWaType != FunXMPP.FMessage.Type.Revoked;
      }
    }

    public bool ShouldShowBroadcastIcon
    {
      get
      {
        return this.Message.KeyFromMe && this.Message.IsBroadcasted() && !JidHelper.IsBroadcastJid(this.Message.KeyRemoteJid);
      }
    }

    public virtual Brush TimestampBrush
    {
      get
      {
        if (this.ShouldShowFooterInAccentColor)
          return (Brush) UIUtils.DarkAccentBrush;
        return this.ShouldUseFooterProtection ? (Brush) UIUtils.WhiteBrush : UIUtils.SubtleBrushWhite;
      }
    }

    public string TimestampStr
    {
      get
      {
        if (this.timestampStr == null)
        {
          DateTime? localTimestamp = this.Message.LocalTimestamp;
          this.timestampStr = !localTimestamp.HasValue ? "" : DateTimeUtils.FormatCompactTime(localTimestamp.Value);
        }
        return this.timestampStr;
      }
    }

    public DateTime? Timestamp => this.Message?.LocalTimestamp;

    public virtual DateTime? DisplayTimestamp
    {
      get => this.Timestamp;
      set
      {
      }
    }

    public virtual int MessageID => this.Message.MessageID;

    public string StarredMessageSenderStr
    {
      get
      {
        if (this.starredMsgSenderStr != null)
          return this.starredMsgSenderStr;
        string senderStr = this.Message.KeyFromMe ? AppResources.You : this.DisplayNameStr;
        if (this.isGroupMsg)
          AppState.Worker.Enqueue((Action) (() => MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            Conversation conversation = db.GetConversation(this.Message.KeyRemoteJid, CreateOptions.None);
            if (conversation == null)
              return;
            string r = string.Format("{0} {1} {2}", (object) senderStr, (object) '·', (object) conversation.GetName());
            Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
            {
              this.starredMsgSenderStr = r;
              this.Notify("StarredMessageSenderStrChanged");
            }));
          }))));
        else
          this.starredMsgSenderStr = senderStr;
        return senderStr;
      }
    }

    public string StarredMessageDateStr
    {
      get
      {
        DateTime? localTimestamp = this.Message.LocalTimestamp;
        return !localTimestamp.HasValue ? "" : DateTimeUtils.FormatCompact(localTimestamp.Value, DateTimeUtils.TimeDisplay.Never);
      }
    }

    public virtual bool IsSelectable => true;

    public static bool IsOverWallpaper
    {
      get => ChatPage.Current != null && ChatPage.Current.IsWallpaperSet;
    }

    public virtual bool ContainsInlineVideo => false;

    public virtual bool ShouldReplace => false;

    public MessageViewModel(Message msg)
    {
      this.Message = msg;
      this.isGroupMsg = JidHelper.IsGroupJid(msg.KeyRemoteJid);
      if (msg.ProtoBuf != null)
        this.contextInfo = new MessageContextInfoWrapper(msg);
      if (!msg.HasPaymentInfo())
        return;
      this.paymentCurrency = PaymentsCurrency.Create(msg.InternalProperties?.PaymentsPropertiesField.Currency);
      this.paymentAmount = double.Parse(msg.InternalProperties?.PaymentsPropertiesField.Amount, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static MessageViewModel Create(Message m, Action<MessageViewModel> postModifier = null)
    {
      if (m == null)
        return (MessageViewModel) null;
      MessageViewModel messageViewModel = (MessageViewModel) null;
      switch (MessageViewPanel.GetViewType(m))
      {
        case MessageViewPanel.ViewTypes.Text:
          messageViewModel = (MessageViewModel) new TextMessageViewModel(m);
          break;
        case MessageViewPanel.ViewTypes.ImageAndVideo:
          messageViewModel = m.MediaWaType == FunXMPP.FMessage.Type.Video ? (MessageViewModel) new VideoMessageViewModel(m) : (MessageViewModel) new ImageMessageViewModel(m);
          break;
        case MessageViewPanel.ViewTypes.Audio:
          messageViewModel = !m.IsPtt() ? (MessageViewModel) new AudioMessageViewModel(m) : (MessageViewModel) new PttMessageViewModel(m);
          break;
        case MessageViewPanel.ViewTypes.Location:
          messageViewModel = (MessageViewModel) new LocationMessageViewModel(m);
          break;
        case MessageViewPanel.ViewTypes.LiveLocation:
          messageViewModel = (MessageViewModel) new LiveLocationMessageViewModel(m);
          break;
        case MessageViewPanel.ViewTypes.Contact:
          messageViewModel = (MessageViewModel) new ContactMessageViewModel(m);
          break;
        case MessageViewPanel.ViewTypes.Document:
          messageViewModel = (MessageViewModel) new DocumentMessageViewModel(m);
          break;
        case MessageViewPanel.ViewTypes.Url:
          messageViewModel = (MessageViewModel) new UrlMessageViewModel(m);
          break;
        case MessageViewPanel.ViewTypes.LargeEmoji:
          messageViewModel = (MessageViewModel) new LargeEmojiMessageViewModel(m);
          break;
        case MessageViewPanel.ViewTypes.InlineVideo:
          messageViewModel = (MessageViewModel) new InlineVideoMessageViewModel(m);
          break;
        case MessageViewPanel.ViewTypes.System:
          messageViewModel = (MessageViewModel) new SystemMessageViewModel(m);
          break;
        case MessageViewPanel.ViewTypes.UnreadDivider:
          messageViewModel = (MessageViewModel) new UnreadDividerViewModel(m);
          break;
        case MessageViewPanel.ViewTypes.Unsupported:
          messageViewModel = (MessageViewModel) new UnsupportedMessageViewModel(m);
          break;
        case MessageViewPanel.ViewTypes.Revoked:
          messageViewModel = (MessageViewModel) new RevokedMessageViewModel(m);
          break;
        case MessageViewPanel.ViewTypes.Sticker:
          messageViewModel = (MessageViewModel) new StickerMessageViewModel(m);
          break;
      }
      if (messageViewModel == null)
        messageViewModel = (MessageViewModel) new TextMessageViewModel(m);
      if (postModifier != null)
        postModifier(messageViewModel);
      return messageViewModel;
    }

    public static IEnumerable<MessageViewModel> CreateForMessage(
      Message msg,
      bool renderInvertly,
      bool reverseOrderForSplitted,
      bool updateSplittingHint,
      MessageSearchResult searchRes = null,
      bool forStarredView = false,
      Action<MessageViewModel> postModifier = null)
    {
      if (msg == null)
        return (IEnumerable<MessageViewModel>) new MessageViewModel[0];
      IEnumerable<MessageViewModel> forMessage = (IEnumerable<MessageViewModel>) null;
      if ((msg.MediaWaType == FunXMPP.FMessage.Type.Undefined || msg.MediaWaType == FunXMPP.FMessage.Type.ExtendedText) && msg.Data != null)
      {
        MessageViewModel[] source = LongMessageSplitter.TrySplitMessage(msg, MessageViewModel.DefaultContentWidth, updateSplittingHint);
        if (postModifier != null)
        {
          foreach (MessageViewModel messageViewModel in source)
            postModifier(messageViewModel);
        }
        if (source != null)
        {
          if (forStarredView || searchRes != null)
          {
            foreach (MessageViewModel messageViewModel in source)
            {
              messageViewModel.SearchResult = searchRes;
              messageViewModel.IsForStarredView = forStarredView;
              messageViewModel.RenderInvertedly = renderInvertly;
            }
          }
          else
          {
            foreach (MessageViewModel messageViewModel in source)
              messageViewModel.RenderInvertedly = renderInvertly;
          }
          forMessage = source.Length > 1 & reverseOrderForSplitted ? ((IEnumerable<MessageViewModel>) source).Reverse<MessageViewModel>() : (IEnumerable<MessageViewModel>) source;
        }
      }
      if (forMessage == null)
      {
        MessageViewModel messageViewModel = MessageViewModel.Create(msg);
        if (postModifier != null)
          postModifier(messageViewModel);
        messageViewModel.RenderInvertedly = renderInvertly;
        if (forStarredView || searchRes != null)
        {
          messageViewModel.SearchResult = searchRes;
          messageViewModel.IsForStarredView = forStarredView;
        }
        forMessage = (IEnumerable<MessageViewModel>) new MessageViewModel[1]
        {
          messageViewModel
        };
      }
      return forMessage;
    }

    public static void ProcessMessagesGrouping(
      IList<MessageViewModel> vms,
      bool reversed,
      bool processReversed,
      MessageViewModel lastVm = null)
    {
      if (vms == null)
        return;
      MessageViewModel messageViewModel = vms.FirstOrDefault<MessageViewModel>();
      if (messageViewModel == null)
        return;
      MessageViewModel.InsertDateDividers(vms, reversed, processReversed, lastVm);
      if (JidHelper.IsGroupJid(messageViewModel.Message.KeyRemoteJid))
      {
        MessageViewModel.ProcessGroupChatMessagesGrouping(vms, reversed, lastVm);
      }
      else
      {
        if (!JidHelper.IsUserJid(messageViewModel.Message.KeyRemoteJid))
          return;
        MessageViewModel.ProcessIndividualChatMessagesGrouping(vms, reversed, lastVm);
      }
    }

    private static void ProcessIndividualChatMessagesGrouping(
      IList<MessageViewModel> vms,
      bool reversed,
      MessageViewModel lastVm = null)
    {
      bool? nullable1 = new bool?();
      bool? nullable2 = new bool?();
      if (lastVm != null)
        nullable1 = new bool?(lastVm.Message.KeyFromMe);
      int index = reversed ? vms.Count - 1 : 0;
      int num1 = reversed ? -1 : vms.Count;
      int num2 = reversed ? -1 : 1;
      for (; (reversed ? (index > num1 ? 1 : 0) : (index < num1 ? 1 : 0)) != 0; index += num2)
      {
        MessageViewModel vm = vms[index];
        bool? nullable3 = vm.Message.MediaWaType == FunXMPP.FMessage.Type.Divider || vm.Message.MediaWaType == FunXMPP.FMessage.Type.System ? new bool?() : new bool?(vm.Message.KeyFromMe);
        if (nullable3.HasValue)
        {
          bool? nullable4 = nullable3;
          bool? nullable5 = nullable1;
          if ((nullable4.GetValueOrDefault() == nullable5.GetValueOrDefault() ? (nullable4.HasValue == nullable5.HasValue ? 1 : 0) : 0) != 0)
          {
            if (lastVm.GroupedPosition == MessageViewModel.GroupingPosition.None)
              lastVm.GroupedPosition = MessageViewModel.GroupingPosition.Top;
            else if (lastVm.GroupedPosition == MessageViewModel.GroupingPosition.Bottom)
              lastVm.GroupedPosition = MessageViewModel.GroupingPosition.Middle;
            vm.GroupedPosition = MessageViewModel.GroupingPosition.Bottom;
          }
        }
        lastVm = vm;
        nullable1 = nullable3;
      }
    }

    private static void ProcessGroupChatMessagesGrouping(
      IList<MessageViewModel> vms,
      bool reversed,
      MessageViewModel lastVm = null)
    {
      string str = (string) null;
      if (lastVm != null)
        str = lastVm.SenderJid;
      int index = reversed ? vms.Count - 1 : 0;
      int num1 = reversed ? -1 : vms.Count;
      int num2 = reversed ? -1 : 1;
      for (; (reversed ? (index > num1 ? 1 : 0) : (index < num1 ? 1 : 0)) != 0; index += num2)
      {
        MessageViewModel vm = vms[index];
        string senderJid = vm.SenderJid;
        if (senderJid != null && senderJid == str && lastVm.ShouldProcessGroupChatMessageGrouping())
        {
          if (lastVm.GroupedPosition == MessageViewModel.GroupingPosition.None)
            lastVm.GroupedPosition = MessageViewModel.GroupingPosition.Top;
          else if (lastVm.GroupedPosition == MessageViewModel.GroupingPosition.Bottom)
            lastVm.GroupedPosition = MessageViewModel.GroupingPosition.Middle;
          vm.GroupedPosition = MessageViewModel.GroupingPosition.Bottom;
        }
        lastVm = vm;
        str = senderJid;
      }
    }

    public virtual bool ShouldProcessGroupChatMessageGrouping() => true;

    private static void InsertDateDividers(
      IList<MessageViewModel> vms,
      bool reversed,
      bool processReversed,
      MessageViewModel lastVm = null)
    {
      bool flag1 = reversed == processReversed;
      bool flag2 = processReversed;
      DateTime? nullable1 = new DateTime?();
      DateTime? nullable2;
      if (lastVm != null)
      {
        DateTime? nullable3;
        if (lastVm == null)
        {
          nullable3 = new DateTime?();
        }
        else
        {
          nullable2 = lastVm.Timestamp;
          ref DateTime? local = ref nullable2;
          nullable3 = local.HasValue ? new DateTime?(local.GetValueOrDefault().Date) : new DateTime?();
        }
        nullable1 = nullable3;
      }
      int index = flag1 ? 0 : vms.Count - 1;
      int num1 = flag1 ? vms.Count : -1;
      int num2 = flag1 ? 1 : -1;
      for (; (flag1 ? (index < num1 ? 1 : 0) : (index > num1 ? 1 : 0)) != 0; index += num2)
      {
        nullable2 = vms[index].Timestamp;
        ref DateTime? local = ref nullable2;
        DateTime? nullable4 = local.HasValue ? new DateTime?(local.GetValueOrDefault().Date) : new DateTime?();
        if (!nullable1.HasValue)
          nullable1 = nullable4;
        if (nullable4.HasValue)
        {
          nullable2 = nullable4;
          DateTime? nullable5 = nullable1;
          if ((nullable2.HasValue == nullable5.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() != nullable5.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
          {
            int num3 = flag2 ? 1 : 0;
            DateTime? nullable6 = flag2 ? nullable1 : nullable4;
            vms.Insert(index, (MessageViewModel) new DateDividerViewModel(new Message()
            {
              MediaWaType = FunXMPP.FMessage.Type.System,
              FunTimestamp = nullable4
            }, nullable6.Value));
            nullable1 = nullable4;
            if (flag1)
              num1 += num2;
          }
        }
      }
    }

    public virtual void RefreshOnChatBackgroundChanged()
    {
    }

    public virtual void RefreshOnContactChanged(string targetJid)
    {
      if (this.Message.KeyFromMe || !this.ShouldShowHeader || !(this.SenderJid == targetJid))
        return;
      this.pushName = this.displayName = (string) null;
      this.Notify("HeaderChanged");
    }

    public virtual void RefreshTextFontSize()
    {
    }

    public virtual void Cleanup() => this.DisposeLazyResources();

    protected override void DisposeManagedResources()
    {
      this.Cleanup();
      base.DisposeManagedResources();
    }

    protected static void InvokeAsync(Action onDispatcher)
    {
      Deployment.Current.Dispatcher.BeginInvoke(onDispatcher);
    }

    public virtual bool IsThumbnailAvailable() => this.Message.BinaryData != null;

    public Size GetTargetThumbnailSize()
    {
      return this.targetThumbSize.HasValue ? this.targetThumbSize.Value : (this.targetThumbSize = new Size?(this.GetTargetThumbnailSizeImpl())).Value;
    }

    protected virtual Size GetTargetThumbnailSizeImpl() => new Size();

    protected void CacheThumbnail(System.Windows.Media.ImageSource thumb, bool isLarge)
    {
      if (this.Message.KeyId == null)
        return;
      MessageViewModel.CachedThumbnails.Set(this.Message.KeyId, new MessageViewModel.ThumbnailState(thumb, this.Message.KeyId, isLarge));
    }

    protected void RemoveCachedThumbnail()
    {
      if (this.Message.KeyId == null)
        return;
      MessageViewModel.CachedThumbnails.Remove(this.Message.KeyId);
    }

    protected MessageViewModel.ThumbnailState GetCachedThumbnail(
      MessageViewModel.ThumbnailOptions thumbOptions)
    {
      MessageViewModel.ThumbnailState val = (MessageViewModel.ThumbnailState) null;
      if (this.Message.KeyId != null && MessageViewModel.CachedThumbnails.TryGet(this.Message.KeyId, out val))
      {
        int num = (thumbOptions & MessageViewModel.ThumbnailOptions.GetSmall) == (MessageViewModel.ThumbnailOptions) 0 ? 0 : ((thumbOptions & MessageViewModel.ThumbnailOptions.GetLarge) == (MessageViewModel.ThumbnailOptions) 0 ? 1 : 0);
        bool flag = (thumbOptions & MessageViewModel.ThumbnailOptions.GetSmall) == (MessageViewModel.ThumbnailOptions) 0 && (thumbOptions & MessageViewModel.ThumbnailOptions.GetLarge) != 0;
        if ((num != 0 && val.IsLargeFormat || flag && !val.IsLargeFormat ? 1 : (val.KeyId != this.Message.KeyId ? 1 : 0)) != 0)
        {
          this.RemoveCachedThumbnail();
          val = (MessageViewModel.ThumbnailState) null;
        }
      }
      else
        val = (MessageViewModel.ThumbnailState) null;
      return val;
    }

    public virtual MessageViewModel.ThumbnailState GetThumbnail(
      MessageViewModel.ThumbnailOptions thumbOptions = MessageViewModel.ThumbnailOptions.Standard)
    {
      MessageViewModel.ThumbnailState thumbnail = this.GetCachedThumbnail(thumbOptions);
      if (thumbnail == null)
      {
        thumbnail = new MessageViewModel.ThumbnailState((System.Windows.Media.ImageSource) null, this.Message.KeyId, false);
        if (this.thumbnailSub == null)
          this.thumbnailSub = this.GetThumbnailObservable(thumbOptions).SubscribeOn<MessageViewModel.ThumbnailState>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<MessageViewModel.ThumbnailState>().Subscribe<MessageViewModel.ThumbnailState>((Action<MessageViewModel.ThumbnailState>) (res =>
          {
            if (res.Image == null || !(res.KeyId == this.Message.KeyId))
              return;
            this.CacheThumbnail(res.Image, res.IsLargeFormat);
            this.NotifyThumbnailChanged("fetched");
          }), (Action<Exception>) (ex =>
          {
            Log.LogException(ex, "msg bubble thumb fetch");
            this.thumbnailSub.SafeDispose();
            this.thumbnailSub = (IDisposable) null;
          }), (Action) (() =>
          {
            this.thumbnailSub.SafeDispose();
            this.thumbnailSub = (IDisposable) null;
          }));
      }
      return thumbnail;
    }

    public IObservable<MessageViewModel.ThumbnailState> GetThumbnailObservable(
      MessageViewModel.ThumbnailOptions thumbOptions = MessageViewModel.ThumbnailOptions.Standard)
    {
      MessageViewModel.ThumbnailState cachedThumbnail = this.GetCachedThumbnail(thumbOptions);
      return cachedThumbnail != null ? Observable.Return<MessageViewModel.ThumbnailState>(cachedThumbnail) : this.GetThumbnailObservableImpl(thumbOptions);
    }

    protected virtual IObservable<MessageViewModel.ThumbnailState> GetThumbnailObservableImpl(
      MessageViewModel.ThumbnailOptions thumbOptions = MessageViewModel.ThumbnailOptions.Standard)
    {
      return Observable.Return<MessageViewModel.ThumbnailState>(new MessageViewModel.ThumbnailState((System.Windows.Media.ImageSource) null, this.Message.KeyId, false));
    }

    protected virtual void NotifyThumbnailChanged(string context)
    {
      Log.d(this.LogHeader, "notify thumb change | {0}", (object) context);
      this.Notify("ThumbnailChanged", (object) context);
    }

    public virtual void GenerateLargeThumbnailAsync()
    {
    }

    public static int LargeThumbPixelWidth
    {
      get
      {
        if (!MessageViewModel.largeThumbPixelWidth.HasValue)
        {
          double num = (double) ResolutionHelper.GetScaleFactor() / 100.0;
          MessageViewModel.largeThumbPixelWidth = new int?((int) (MessageViewModel.DefaultContentWidth * num));
          Log.l("msg vm", "large thumb width: {0}, scale: {1}", (object) MessageViewModel.largeThumbPixelWidth.Value, (object) num);
        }
        return MessageViewModel.largeThumbPixelWidth.Value;
      }
    }

    public virtual bool ShouldFillFullTitleBackground => true;

    public virtual bool ShouldFillContentBackground => true;

    public virtual Brush FooterBackgroundBrush => (Brush) null;

    public virtual bool ShouldShowFooterOnLeft => false;

    public virtual GridLength BubbleContentWidth => GridLength.Auto;

    protected BitmapSource CreateBlurryThumb(BitmapSource rawSmallThumb)
    {
      return rawSmallThumb == null ? (BitmapSource) null : (BitmapSource) ImageStore.CreateThumbnail(rawSmallThumb, MessageViewModel.LargeThumbPixelWidth).Blur();
    }

    private void GetSenderNames()
    {
      if (this.pushName != null || this.displayName != null)
        return;
      if (this.Message.KeyFromMe)
      {
        this.displayName = AppResources.You;
        this.pushName = "";
      }
      else
      {
        this.pushName = this.displayName = "";
        this.GetSenderNamesImpl();
      }
    }

    private void GetSenderNamesImpl()
    {
      IDisposable sub = (IDisposable) null;
      sub = Observable.Create<Pair<string, string>>((Func<IObserver<Pair<string, string>>, Action>) (observer =>
      {
        string str1 = (string) null;
        UserStatus sender = this.Sender;
        Message msg = this.Message;
        if (sender == null || string.IsNullOrEmpty(sender.ContactName))
        {
          if (sender == null || sender.PushName == null)
          {
            if (!string.IsNullOrEmpty(msg.PushName))
            {
              str1 = "~" + Emoji.ConvertToTextOnly(msg.PushName, (byte[]) null);
              if (sender != null)
                ContactsContext.Instance((Action<ContactsContext>) (cdb =>
                {
                  sender.PushName = msg.PushName;
                  cdb.SubmitChanges();
                }));
            }
          }
          else
            str1 = "~" + Emoji.ConvertToTextOnly(sender.PushName, (byte[]) null);
        }
        string str2 = sender?.GetDisplayName();
        if (string.IsNullOrEmpty(str2))
        {
          string jid = sender?.Jid ?? this.SenderJid;
          if (JidHelper.IsUserJid(jid))
            str2 = JidHelper.GetPhoneNumber(jid, true);
        }
        observer.OnNext(new Pair<string, string>(str1 ?? "", str2 ?? ""));
        observer.OnCompleted();
        return (Action) (() => { });
      })).Subscribe<Pair<string, string>>((Action<Pair<string, string>>) (p =>
      {
        this.pushName = p.First ?? "";
        this.displayName = p.Second ?? "";
        sub.SafeDispose();
        sub = (IDisposable) null;
      }));
    }

    private void SetQuotedMediaUri(MessageViewModel quotedVm)
    {
      if (!(quotedVm is StickerMessageViewModel))
        return;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Message message = db.GetMessage(quotedVm.Message.KeyRemoteJid, quotedVm.Message.KeyId, quotedVm.Message.KeyFromMe);
        if (this.Message.QuotedMediaFileUri == null)
        {
          if (message == null || message.LocalFileUri == null)
            return;
          this.Message.QuotedMediaFileUri = message.LocalFileUri;
          db.LocalFileAddRef(this.Message.QuotedMediaFileUri, LocalFileType.QuotedMedia);
          db.SubmitChanges();
        }
        else
        {
          if (message == null || message.LocalFileUri == null || !(message.LocalFileUri != this.Message.QuotedMediaFileUri))
            return;
          db.LocalFileRelease(this.Message.QuotedMediaFileUri, LocalFileType.QuotedMedia);
          this.Message.QuotedMediaFileUri = message.LocalFileUri;
          db.LocalFileAddRef(this.Message.QuotedMediaFileUri, LocalFileType.QuotedMedia);
          db.SubmitChanges();
        }
      }));
    }

    public virtual Set<string> GetTrackedProperties()
    {
      Set<string> trackedProperties = new Set<string>();
      if (this.Message.KeyFromMe && !this.Message.IsReadByTarget())
        trackedProperties.Add("Status");
      trackedProperties.Add("IsStarred");
      return trackedProperties;
    }

    public virtual void InitLazyResources()
    {
      if (this.isLazyInited || this.propertyChangedSub != null)
        return;
      Set<string> trackedProperties = this.GetTrackedProperties();
      if (!trackedProperties.Any<string>())
        return;
      this.propertyChangedSub = this.Message.GetPropertyChangedAsync().Where<PropertyChangedEventArgs>((Func<PropertyChangedEventArgs, bool>) (p => trackedProperties.Contains(p.PropertyName))).Subscribe<PropertyChangedEventArgs>((Action<PropertyChangedEventArgs>) (p => this.OnMessagePropertyChanged(p.PropertyName)));
    }

    public virtual void DisposeLazyResources()
    {
      this.propertyChangedSub.SafeDispose();
      this.propertyChangedSub = (IDisposable) null;
      this.thumbnailSub.SafeDispose();
      this.thumbnailSub = (IDisposable) null;
      this.RemoveCachedThumbnail();
      this.isLazyInited = false;
    }

    public virtual RichTextBlock.TextSet GetRichText() => (RichTextBlock.TextSet) null;

    protected virtual string GetTextStr() => this.Message.GetTextForDisplay();

    protected void SaveLargeThumbnail(WriteableBitmap bitmap)
    {
      if (bitmap == null)
        return;
      byte[] bytes = bitmap.ToJpegByteArray(48128);
      if (bytes == null)
      {
        Log.WriteLineDebug("msg vm: invalid jpeg byte array");
      }
      else
      {
        int thumbWidth = bitmap.PixelWidth;
        int thumbHeight = bitmap.PixelHeight;
        AppState.Worker.Enqueue((Action) (() => MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          Message message = db.GetMessage(this.Message.KeyRemoteJid, this.Message.KeyId, this.Message.KeyFromMe);
          if (message == null || !message.SaveBinaryDataFile(bytes))
            return;
          Log.l(this.LogHeader, "large thumb saved to file | type:{1},size:{2}x{3},fsize:{4}kb", (object) message.KeyId, (object) message.MediaWaType, (object) thumbWidth, (object) thumbHeight, (object) (bytes.Length / 1024));
          db.LocalFileAddRef(message.DataFileName, LocalFileType.Thumbnail);
          db.SubmitChanges();
        }))));
      }
    }

    protected void OnLargeThumbnailCreated(WriteableBitmap largeThumb)
    {
      if (largeThumb == null)
        return;
      this.CacheThumbnail((System.Windows.Media.ImageSource) largeThumb, true);
      this.NotifyThumbnailChanged("large thumb created");
      this.SaveLargeThumbnail(largeThumb);
    }

    protected virtual bool OnMessagePropertyChanged(string prop)
    {
      bool flag = false;
      switch (prop)
      {
        case "Status":
          this.OnMessageStatusChanged();
          flag = true;
          break;
        case "IsStarred":
          this.Notify("IsStarredChanged");
          flag = true;
          break;
      }
      return flag;
    }

    protected virtual void OnMessageStatusChanged() => this.Notify("StatusChanged");

    public enum GroupingPosition
    {
      None,
      Top,
      Middle,
      Bottom,
    }

    public enum ThumbnailOptions
    {
      GetSmall = 1,
      GetLarge = 16, // 0x00000010
      DecodeInBackground = 256, // 0x00000100
      Standard = 273, // 0x00000111
    }

    public class ThumbnailState
    {
      public System.Windows.Media.ImageSource Image { get; private set; }

      public string KeyId { get; private set; }

      public bool IsLargeFormat { get; private set; }

      public string Context { get; set; }

      public ThumbnailState(System.Windows.Media.ImageSource thumb, string keyId, bool isLarge)
      {
        this.Image = thumb;
        this.KeyId = keyId;
        this.IsLargeFormat = isLarge;
      }
    }

    public class ChopState
    {
      public int Offset;
      public int Length;
    }

    private enum StatusIconTypes
    {
      None,
      InlineAccentClock,
      InlineWhiteClock,
      InlineSolidWhiteClock,
      InlineAccentCheck,
      InlineWhiteCheck,
      InlineSolidWhiteCheck,
      InlineAccentDoubleChecks,
      InlineWhiteDoubleChecks,
      InlineSolidWhiteDoubleChecks,
      InlineBlueChecks,
      InlineAccentError,
      InlineWhiteError,
      InlineSolidWhiteError,
    }
  }
}
