// Decompiled with JetBrains decompiler
// Type: WhatsApp.Pages.ChatCountsPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp.Pages
{
  public class ChatCountsPage : PhoneApplicationPage
  {
    protected static readonly FunXMPP.FMessage.Type[] MSG_TYPES_FOR_DISPLAY = new FunXMPP.FMessage.Type[12]
    {
      FunXMPP.FMessage.Type.Undefined,
      FunXMPP.FMessage.Type.Image,
      FunXMPP.FMessage.Type.Video,
      FunXMPP.FMessage.Type.Audio,
      FunXMPP.FMessage.Type.Gif,
      FunXMPP.FMessage.Type.Location,
      FunXMPP.FMessage.Type.LiveLocation,
      FunXMPP.FMessage.Type.Contact,
      FunXMPP.FMessage.Type.Document,
      FunXMPP.FMessage.Type.ExtendedText,
      FunXMPP.FMessage.Type.LiveLocation,
      FunXMPP.FMessage.Type.Sticker
    };
    protected static readonly FunXMPP.FMessage.Type[] MSG_TYPES_TO_DISCARD = new FunXMPP.FMessage.Type[7]
    {
      FunXMPP.FMessage.Type.CallOffer,
      FunXMPP.FMessage.Type.CipherText,
      FunXMPP.FMessage.Type.Divider,
      FunXMPP.FMessage.Type.HSM,
      FunXMPP.FMessage.Type.ProtocolBuffer,
      FunXMPP.FMessage.Type.System,
      FunXMPP.FMessage.Type.Unsupported
    };
    private static long totalMessages = -1;
    private static long totalSize = -1;
    private static Dictionary<string, ChatCountsPage.ConvoStats> chatCountConvoItems = (Dictionary<string, ChatCountsPage.ConvoStats>) null;
    private static long listCreatedTicks = -1;
    private PivotHeaderConverter pivotHeaderCoverter = new PivotHeaderConverter();
    private bool cancelled;
    internal ZoomBox LayoutRootZoomBox;
    internal Grid LayoutRoot;
    internal PageTitlePanel PageTitle;
    internal StackPanel LoadingPanel;
    internal ProgressBar LoadingProgressBar;
    internal TextBlock LoadingTextBlock;
    internal Pivot Pivot;
    internal PivotItem MessagesPivotItem;
    internal WhatsApp.CompatibilityShims.LongListSelector MessagesList;
    internal PivotItem SizePivotItem;
    internal WhatsApp.CompatibilityShims.LongListSelector SizeList;
    private bool _contentLoaded;

    private static string GetTypeTitleStr(FunXMPP.FMessage.Type type)
    {
      switch (type)
      {
        case FunXMPP.FMessage.Type.Undefined:
          return AppResources.StorageInfoText;
        case FunXMPP.FMessage.Type.Image:
          return AppResources.StorageInfoImages;
        case FunXMPP.FMessage.Type.Audio:
          return AppResources.StorageInfoVoice;
        case FunXMPP.FMessage.Type.Video:
          return AppResources.StorageInfoVideos;
        case FunXMPP.FMessage.Type.Contact:
          return AppResources.StorageInfoContacts;
        case FunXMPP.FMessage.Type.Location:
          return AppResources.StorageInfoLocations;
        case FunXMPP.FMessage.Type.Document:
          return AppResources.StorageInfoDocs;
        case FunXMPP.FMessage.Type.ExtendedText:
          return AppResources.StorageInfoText;
        case FunXMPP.FMessage.Type.Gif:
          return AppResources.StorageInfoGifs;
        case FunXMPP.FMessage.Type.LiveLocation:
          return AppResources.StorageInfoLocations;
        default:
          return "";
      }
    }

    public static string FormatMediaSize(long mediaSize)
    {
      if (mediaSize > 0L)
        return Utils.FileSizeFormatter.Format(mediaSize);
      return mediaSize != 0L ? "" : "-";
    }

    public bool GetMediaSizes(
      ref Dictionary<string, ChatCountsPage.ConvoStats> chatsStats,
      Action<int> progressUpdate,
      Func<bool> isCancelled)
    {
      long lastMessageId = -1;
      if (progressUpdate != null)
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => lastMessageId = db.GetLastMsgId()));
      Dictionary<string, Dictionary<long, long>> chatsSizesByType = new Dictionary<string, Dictionary<long, long>>();
      WorkQueue CheckFileQueue = new WorkQueue(flags: WorkQueue.StartFlags.WatchdogExcempt);
      long filesChecked = 0;
      long differenceMessageToActualFileSize = 0;
      Dictionary<string, long> urlsFound = new Dictionary<string, long>();
      Action<string, string, long, long> checkFile = (Action<string, string, long, long>) ((jid, inputFilename, msgFileSize, mediaType) =>
      {
        ++filesChecked;
        long val2 = -1;
        string absolutePath = MediaStorage.GetAbsolutePath(inputFilename);
        try
        {
          val2 = NativeInterfaces.Misc.GetFileSizeFast(absolutePath);
        }
        catch (Exception ex)
        {
          string context = "Exception checking file exists: " + absolutePath;
          Log.LogException(ex, context);
        }
        if (val2 > 0L)
        {
          Dictionary<long, long> dictionary = (Dictionary<long, long>) null;
          if (!chatsSizesByType.TryGetValue(jid, out dictionary))
          {
            dictionary = new Dictionary<long, long>();
            chatsSizesByType.Add(jid, dictionary);
          }
          long num1 = 0;
          if (!dictionary.TryGetValue(mediaType, out num1))
          {
            dictionary.Add(mediaType, val2);
          }
          else
          {
            long num2 = num1 + val2;
          }
        }
        else
          Log.d("chats stats", "Missing file {0}, {1}", (object) inputFilename, (object) absolutePath);
        differenceMessageToActualFileSize += Math.Max(0L, msgFileSize) - Math.Max(0L, val2);
        if (!Settings.IsWaAdmin)
          return;
        urlsFound[inputFilename] = val2;
      });
      Action<string, long, string, long, int> processMediaFileCallback = (Action<string, long, string, long, int>) ((jid, mediaType, filename, mediaSize, messageCount) => CheckFileQueue.Enqueue((Action) (() => checkFile(jid, filename, mediaSize, mediaType))));
      int limit = 50;
      long msgIdStart = 0;
      long ticks1 = DateTime.Now.Ticks;
      long num3 = 0;
      bool finishedFiles = lastMessageId < 0L;
      int num4 = 0;
      DateTime now;
      while (!finishedFiles)
      {
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => finishedFiles = db.GetMediaMessages(processMediaFileCallback, ref msgIdStart, ref limit)));
        if (isCancelled())
          finishedFiles = true;
        ManualResetEvent stallEvent = new ManualResetEvent(false);
        Action<ManualResetEvent> signal = (Action<ManualResetEvent>) (semaphore => semaphore.Set());
        CheckFileQueue.Enqueue((Action) (() => signal(stallEvent)));
        now = DateTime.Now;
        long ticks2 = now.Ticks;
        stallEvent.WaitOne();
        long num5 = num3;
        now = DateTime.Now;
        long num6 = now.Ticks - ticks2;
        num3 = num5 + num6;
        if (progressUpdate != null)
        {
          int num7 = (int) (msgIdStart * 100L / lastMessageId);
          if (num4 != num7)
          {
            num4 = num7;
            if (progressUpdate != null)
              progressUpdate(num7);
          }
        }
      }
      CheckFileQueue?.Stop();
      CheckFileQueue = (WorkQueue) null;
      object[] objArray = new object[4];
      now = DateTime.Now;
      objArray[0] = (object) (now.Ticks - ticks1);
      objArray[1] = (object) num3;
      objArray[2] = (object) filesChecked;
      objArray[3] = (object) differenceMessageToActualFileSize;
      Log.l("chats stats", "Finished Media, timing {0}, stalled {1}, files checked {2}, diff {3}", objArray);
      if (isCancelled())
        Log.l("chats stats", "cancelled file processing");
      else if (progressUpdate != null)
        progressUpdate(100);
      if (urlsFound.Count > 0 && Settings.IsWaAdmin && !isCancelled())
      {
        GlobalProgressIndicator progressIndicator = (GlobalProgressIndicator) null;
        this.Dispatcher.BeginInvoke((Action) (() => UIUtils.Decision(string.Format("Do you want to check local files?\n Found {0}\nThis can take some time", (object) urlsFound.Count), "Yes", "No").ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (confirmed =>
        {
          if (!confirmed)
            return;
          progressIndicator = new GlobalProgressIndicator((DependencyObject) this);
          progressIndicator.Acquire();
          int num14 = (int) MessageBox.Show("Process indicator indicates this is working.\nA message like this will be displayed when completed.\nLeaving this screen will cancel the processing.");
          AppState.Worker.Enqueue((Action) (() =>
          {
            int inLocalFileCount = 0;
            int onDisk = 0;
            int onDiskAndIn = 0;
            int onDiskAndNotIn = 0;
            foreach (string key in urlsFound.Keys)
            {
              if (!isCancelled())
              {
                bool fileFoundInLocalFileTable = false;
                MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
                {
                  LocalFile localFileByUri = db.GetLocalFileByUri(localUri);
                  int num11;
                  if (localFileByUri != null)
                  {
                    int? referenceCount = localFileByUri.ReferenceCount;
                    int num12 = 0;
                    num11 = referenceCount.GetValueOrDefault() > num12 ? (referenceCount.HasValue ? 1 : 0) : 0;
                  }
                  else
                    num11 = 0;
                  fileFoundInLocalFileTable = num11 != 0;
                }));
                bool flag = urlsFound[localUri] > 0L;
                Log.l("Local file", "{0} onDisk={1}, inLocalFIleTable={2}", (object) localUri, (object) flag, (object) fileFoundInLocalFileTable);
                inLocalFileCount += fileFoundInLocalFileTable ? 1 : 0;
                onDisk += flag ? 1 : 0;
                onDiskAndIn += flag & fileFoundInLocalFileTable ? 1 : 0;
                onDiskAndNotIn += !flag || fileFoundInLocalFileTable ? 0 : 1;
              }
              else
                break;
            }
            this.Dispatcher.BeginInvoke((Action) (() =>
            {
              progressIndicator.Release();
              string str = string.Format("Found in messages {0}, if these, number missing from local filetable {1}.", (object) inLocalFileCount, (object) (urlsFound.Count - inLocalFileCount)) + string.Format("\nFound actual file {0}, of these, number missing from local filetable {1}.", (object) onDisk, (object) onDiskAndNotIn) + string.Format("\nFiles both found and in local filetable {0}.", (object) onDiskAndIn);
              Log.l("Local file", str);
              if (isCancelled())
                return;
              int num13 = (int) MessageBox.Show(str);
            }));
          }));
        }))));
      }
      foreach (KeyValuePair<string, ChatCountsPage.ConvoStats> keyValuePair1 in chatsStats)
      {
        string key1 = keyValuePair1.Key;
        Dictionary<long, long> dictionary = (Dictionary<long, long>) null;
        if (chatsSizesByType.TryGetValue(key1, out dictionary))
        {
          chatsSizesByType.Remove(key1);
          foreach (FunXMPP.FMessage.Type key2 in ChatCountsPage.MSG_TYPES_FOR_DISPLAY)
          {
            long num15 = 0;
            if (dictionary.TryGetValue((long) key2, out num15))
            {
              dictionary.Remove((long) key2);
              keyValuePair1.Value.ChatSize += num15;
              FunXMPP.FMessage.Type type = key2 == FunXMPP.FMessage.Type.ExtendedText || key2 == FunXMPP.FMessage.Type.Undefined ? FunXMPP.FMessage.Type.Undefined : key2;
              ChatCountsPage.StorageInfo storageInfo1 = (ChatCountsPage.StorageInfo) null;
              foreach (ChatCountsPage.StorageInfo chatDetail in keyValuePair1.Value.ChatDetails)
              {
                if (chatDetail.MediaType == type)
                {
                  storageInfo1 = chatDetail;
                  storageInfo1.MediaSize += num15;
                  break;
                }
              }
              if (storageInfo1 == null)
              {
                Log.l("chats stats", "Unexpectedly adding item for {0}, size {1}", (object) type, (object) num15);
                ChatCountsPage.StorageInfo storageInfo2 = new ChatCountsPage.StorageInfo()
                {
                  MediaType = type,
                  Count = 1,
                  MediaSize = num15,
                  Title = ChatCountsPage.GetTypeTitleStr(type)
                };
                keyValuePair1.Value.ChatDetails.Add(storageInfo2);
              }
            }
          }
          if (dictionary.Count > 0)
          {
            foreach (FunXMPP.FMessage.Type key3 in ChatCountsPage.MSG_TYPES_TO_DISCARD)
              dictionary.Remove((long) key3);
          }
          foreach (KeyValuePair<long, long> keyValuePair2 in dictionary)
            Log.d("chats stats", "Not collecting details for type {0}, {1}", (object) keyValuePair2.Key, (object) keyValuePair2.Value);
        }
      }
      foreach (string key in chatsSizesByType.Keys)
        Log.l("chats stats", "unknown jid: {0}" + key);
      return true;
    }

    public ChatCountsPage()
    {
      this.InitializeComponent();
      this.LayoutRootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.PageTitle.SmallTitle = AppResources.StorageInfoTitle;
      this.PageTitle.Mode = PageTitlePanel.Modes.NotZoomed;
      this.LoadingTextBlock.Text = AppResources.Loading;
      this.DataContext = (object) this;
      this.SizePivotItem.Header = (object) this.pivotHeaderCoverter.Convert(AppResources.StorageInfoSizes);
      this.MessagesPivotItem.Header = (object) this.pivotHeaderCoverter.Convert(AppResources.MessagesHeader);
    }

    public string TotalMessages => ChatCountsPage.totalMessages.ToString();

    public string TotalSize => ChatCountsPage.FormatMediaSize(ChatCountsPage.totalSize);

    public List<ChatCountsPage.ChatCountItemViewModel> MessageItems
    {
      get
      {
        return ChatCountsPage.chatCountConvoItems.Values.OrderByDescending<ChatCountsPage.ConvoStats, long>((Func<ChatCountsPage.ConvoStats, long>) (ccci => ccci.ChatCount)).Select<ChatCountsPage.ConvoStats, ChatCountsPage.ChatCountItemViewModel>((Func<ChatCountsPage.ConvoStats, ChatCountsPage.ChatCountItemViewModel>) (ccci =>
        {
          return new ChatCountsPage.ChatCountItemViewModel(ccci.Convo, ref ccci, false)
          {
            EnableChatPreview = false,
            EnableContextMenu = false,
            EnableRecipientCheck = false
          };
        })).ToList<ChatCountsPage.ChatCountItemViewModel>();
      }
    }

    public List<ChatCountsPage.ChatCountItemViewModel> SizeItems
    {
      get
      {
        return ChatCountsPage.chatCountConvoItems.Values.OrderByDescending<ChatCountsPage.ConvoStats, long>((Func<ChatCountsPage.ConvoStats, long>) (ccci => ccci.ChatSize)).Select<ChatCountsPage.ConvoStats, ChatCountsPage.ChatCountItemViewModel>((Func<ChatCountsPage.ConvoStats, ChatCountsPage.ChatCountItemViewModel>) (ccci =>
        {
          return new ChatCountsPage.ChatCountItemViewModel(ccci.Convo, ref ccci, true)
          {
            EnableChatPreview = false,
            EnableContextMenu = false,
            EnableRecipientCheck = false
          };
        })).ToList<ChatCountsPage.ChatCountItemViewModel>();
      }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      this.LoadData();
    }

    private void LoadData()
    {
      if (this.LoadingProgressBar.Visibility == Visibility.Visible)
        return;
      this.LoadingProgressBar.Visibility = Visibility.Visible;
      this.LoadingTextBlock.Visibility = Visibility.Visible;
      this.Pivot.Visibility = Visibility.Collapsed;
      this.Pivot.Items.Remove((object) this.SizePivotItem);
      this.LoadingProgressBar.IsIndeterminate = false;
      this.LoadingProgressBar.Maximum = 100.0;
      this.LoadingProgressBar.Value = 0.0;
      AppState.Worker.Enqueue((Action) (() =>
      {
        if (DateTime.Now.Ticks - ChatCountsPage.listCreatedTicks > 3000000000L)
        {
          ChatCountsPage.totalSize = 0L;
          ChatCountsPage.totalMessages = 0L;
          long ticks1 = DateTime.Now.Ticks;
          List<Conversation> chats = (List<Conversation>) null;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => chats = db.GetConversations(new JidHelper.JidTypes[2]
          {
            JidHelper.JidTypes.User,
            JidHelper.JidTypes.Group
          }, true)));
          ChatCountsPage.chatCountConvoItems = new Dictionary<string, ChatCountsPage.ConvoStats>();
          int numberOfConversations = chats.Count;
          foreach (Conversation convo in chats)
          {
            ChatCountsPage.ConvoStats convoStats = new ChatCountsPage.ConvoStats(convo);
            convoStats.UpdateMessagesCounts();
            ChatCountsPage.chatCountConvoItems.Add(convo.Jid, convoStats);
            this.Dispatcher.BeginInvoke((Action) (() => this.LoadingProgressBar.Value += 50.0 / (double) numberOfConversations));
            ChatCountsPage.totalMessages += convoStats.ChatCount;
            if (this.IsCancelled())
            {
              Log.l("chats stats", "Cancelled");
              return;
            }
          }
          Log.l("chats stats", "Timing ticks {0} to get chat messages", (object) (DateTime.Now.Ticks - ticks1));
          chats.Clear();
          chats = (List<Conversation>) null;
          this.Dispatcher.BeginInvoke((Action) (() =>
          {
            this.Pivot.Visibility = Visibility.Visible;
            this.LoadingProgressBar.Value = 50.0;
          }));
          long ticks2 = DateTime.Now.Ticks;
          int currentFileCheckPercent = 0;
          Action<int> progressUpdate = (Action<int>) (newPercent =>
          {
            if (newPercent / 2 <= currentFileCheckPercent + 1)
              return;
            currentFileCheckPercent = newPercent / 2;
            this.Dispatcher.BeginInvoke((Action) (() =>
            {
              this.LoadingProgressBar.IsIndeterminate = false;
              this.LoadingProgressBar.Value = (double) (50 + newPercent / 2);
            }));
          });
          this.GetMediaSizes(ref ChatCountsPage.chatCountConvoItems, progressUpdate, new Func<bool>(this.IsCancelled));
          if (this.IsCancelled())
          {
            Log.l("chats stats", "Cancelled");
            return;
          }
          Log.l("chats stats", "Timing ticks {0} to get file sizes", (object) (DateTime.Now.Ticks - ticks2));
          ChatCountsPage.totalSize = 0L;
          foreach (ChatCountsPage.ConvoStats convoStats in ChatCountsPage.chatCountConvoItems.Values)
          {
            ChatCountsPage.totalSize += convoStats.ChatSize;
            foreach (ChatCountsPage.StorageInfo chatDetail in convoStats.ChatDetails)
            {
              if (chatDetail.MediaSize < 0L)
                chatDetail.MediaSize = 0L;
            }
          }
          Log.l("chats stats", "Messages {0}, Size {1}, Timing ticks {2}", (object) ChatCountsPage.totalMessages, (object) ChatCountsPage.totalSize, (object) (DateTime.Now.Ticks - ticks1));
          ChatCountsPage.listCreatedTicks = DateTime.Now.Ticks;
        }
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          this.LoadingProgressBar.Visibility = Visibility.Collapsed;
          this.LoadingTextBlock.Visibility = Visibility.Collapsed;
          this.Pivot.Items.Add((object) this.SizePivotItem);
          this.Pivot.Visibility = Visibility.Visible;
        }));
      }));
    }

    private void RefreshButton_Click(object sender, EventArgs e)
    {
      ChatCountsPage.listCreatedTicks = 0L;
      this.LoadData();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      Log.l("chats stats", "Exit");
      this.cancelled = true;
      base.OnNavigatedFrom(e);
    }

    private bool IsCancelled() => this.cancelled;

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/Settings/ChatCountsPage.xaml", UriKind.Relative));
      this.LayoutRootZoomBox = (ZoomBox) this.FindName("LayoutRootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.PageTitle = (PageTitlePanel) this.FindName("PageTitle");
      this.LoadingPanel = (StackPanel) this.FindName("LoadingPanel");
      this.LoadingProgressBar = (ProgressBar) this.FindName("LoadingProgressBar");
      this.LoadingTextBlock = (TextBlock) this.FindName("LoadingTextBlock");
      this.Pivot = (Pivot) this.FindName("Pivot");
      this.MessagesPivotItem = (PivotItem) this.FindName("MessagesPivotItem");
      this.MessagesList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("MessagesList");
      this.SizePivotItem = (PivotItem) this.FindName("SizePivotItem");
      this.SizeList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("SizeList");
    }

    public class ConvoStats
    {
      public Conversation Convo;
      public List<ChatCountsPage.StorageInfo> ChatDetails;
      public long ChatSize;
      public long ChatCount;

      public ConvoStats(Conversation convo)
      {
        this.Convo = convo;
        this.ChatSize = -1L;
        this.ChatCount = 0L;
        this.ChatDetails = new List<ChatCountsPage.StorageInfo>();
      }

      public void UpdateMessagesCounts()
      {
        if (this.Convo == null || this.Convo.Jid == null)
          return;
        int? nullable = this.Convo.MessageLoadingStart();
        int startMessageId = nullable.HasValue ? nullable.Value : 0;
        Dictionary<long, long> chatCounts = new Dictionary<long, long>();
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => db.GetMessageCounts(this.Convo.Jid, ref chatCounts, startMessageId)));
        foreach (FunXMPP.FMessage.Type key in ChatCountsPage.MSG_TYPES_FOR_DISPLAY)
        {
          long num = 0;
          if (chatCounts.TryGetValue((long) key, out num))
          {
            chatCounts.Remove((long) key);
            FunXMPP.FMessage.Type type = key == FunXMPP.FMessage.Type.ExtendedText || key == FunXMPP.FMessage.Type.Undefined ? FunXMPP.FMessage.Type.Undefined : key;
            ChatCountsPage.StorageInfo storageInfo = (ChatCountsPage.StorageInfo) null;
            foreach (ChatCountsPage.StorageInfo chatDetail in this.ChatDetails)
            {
              if (chatDetail.MediaType == type)
              {
                storageInfo = chatDetail;
                storageInfo.Count += num;
                break;
              }
            }
            if (storageInfo == null)
              this.ChatDetails.Add(new ChatCountsPage.StorageInfo()
              {
                MediaType = type,
                MediaSize = -1L,
                Count = num,
                Title = ChatCountsPage.GetTypeTitleStr(type)
              });
            this.ChatCount += num;
          }
        }
        foreach (FunXMPP.FMessage.Type key in ChatCountsPage.MSG_TYPES_TO_DISCARD)
          chatCounts.Remove((long) key);
        foreach (KeyValuePair<long, long> keyValuePair in chatCounts)
          Log.l("chats stats", "Unexpected messages type found {0} {1}", (object) keyValuePair.Key, (object) keyValuePair.Value);
      }
    }

    public class StorageInfo
    {
      public FunXMPP.FMessage.Type MediaType;

      public long MediaSize { get; set; } = -1;

      public string FormattedSize => ChatCountsPage.FormatMediaSize(this.MediaSize);

      public long Count { get; set; }

      public string Title { get; set; }
    }

    public class ChatCountItemViewModel : ChatItemViewModel
    {
      public bool IsExpanded;
      private bool IsSizeShown;
      public ChatCountsPage.ConvoStats ChatCounts;

      public ChatCountItemViewModel(
        Conversation convo,
        ref ChatCountsPage.ConvoStats chatCount,
        bool isSizeDisplay)
        : base(convo)
      {
        this.ChatCounts = chatCount;
        this.IsSizeShown = isSizeDisplay;
      }

      public override bool ShowTimestamp => true;

      public override string TimestampStr
      {
        get
        {
          return this.IsSizeShown ? ChatCountsPage.FormatMediaSize(this.ChatCounts.ChatSize) : this.ChatCounts.ChatCount.ToString();
        }
      }

      public override RichTextBlock.TextSet GetSubtitle() => (RichTextBlock.TextSet) null;

      public List<ChatCountsPage.StorageInfo> ChatDetails
      {
        get
        {
          return this.IsSizeShown ? this.ChatCounts.ChatDetails.OrderByDescending<ChatCountsPage.StorageInfo, long>((Func<ChatCountsPage.StorageInfo, long>) (si => si.MediaSize)).ToList<ChatCountsPage.StorageInfo>() : this.ChatCounts.ChatDetails.OrderByDescending<ChatCountsPage.StorageInfo, long>((Func<ChatCountsPage.StorageInfo, long>) (si => si.Count)).ToList<ChatCountsPage.StorageInfo>();
        }
      }
    }
  }
}
