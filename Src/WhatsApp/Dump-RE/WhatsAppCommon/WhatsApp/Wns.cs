// Decompiled with JetBrains decompiler
// Type: WhatsApp.Wns
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WhatsApp.WaCollections;
using Windows.Data.Xml.Dom;
using Windows.Networking.PushNotifications;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

#nullable disable
namespace WhatsApp
{
  public class Wns : IPushSystem, IPushSystemForeground
  {
    private static WorkQueue worker;
    private object toastInitLock = new object();
    private ToastNotifier toastNotifier;
    public static readonly string AppId = string.Format("x{0}x", (object) new string(AppState.GetAppGuid().Replace('-', 'y').Where<char>((Func<char, bool>) (ch => !"{}".Contains<char>(ch))).ToArray<char>()));
    private ManualResetEvent secondaryTileLoadedEvent = new ManualResetEvent(false);
    private object secondaryTileLock = new object();
    private volatile bool scannedSecondaryTiles;
    private Dictionary<string, SecondaryTile> secondaryTiles = new Dictionary<string, SecondaryTile>();
    private Set<string> pendingSecondaryTiles = new Set<string>();
    private Dictionary<string, Action<SecondaryTile>> tileNotifyActions = new Dictionary<string, Action<SecondaryTile>>();

    public static WorkQueue Worker
    {
      get
      {
        return Utils.LazyInit<WorkQueue>(ref Wns.worker, (Func<WorkQueue>) (() => new WorkQueue(flags: WorkQueue.StartFlags.Unpausable | WorkQueue.StartFlags.WatchdogExcempt)));
      }
    }

    public IObservable<Uri> UriObservable
    {
      get
      {
        return Wns.GetChannel().Select<PushNotificationChannel, Uri>((Func<PushNotificationChannel, Uri>) (channel => new Uri(channel.Uri))).Do<Uri>((Action<Uri>) (_ => this.BindPush()));
      }
    }

    private static IObservable<PushNotificationChannel> GetChannel()
    {
      return Observable.Defer<PushNotificationChannel>((Func<IObservable<PushNotificationChannel>>) (() => Wns.GetChannelAsync().ToObservable<PushNotificationChannel>()));
    }

    private static async Task<PushNotificationChannel> GetChannelAsync()
    {
      return await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
    }

    public void BindPush() => PushSystem.PushBoundSubject.OnNext(new Unit());

    public void OnAppReset() => this.InvalidateTileCache();

    public void OnPushRegistered() => Settings.WnsRegistered = true;

    public void RequestNewUri()
    {
      IObservable<PushNotificationChannel> channel = Wns.GetChannel();
      channel.Do<PushNotificationChannel>((Action<PushNotificationChannel>) (_ => _.Close())).Concat<PushNotificationChannel>(channel).Skip<PushNotificationChannel>(1).Take<PushNotificationChannel>(1).Subscribe<PushNotificationChannel>();
    }

    public string PushState => "unknown";

    public bool IsHealthy => true;

    private static string RemoveEmbeddedNulls(string str)
    {
      if (str != null)
      {
        try
        {
          byte[] bytes = Encoding.UTF8.GetBytes(str);
          string str1 = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
          if (str1 != str)
            str = str1;
        }
        catch (Exception ex)
        {
        }
        char[] source = new char[5]
        {
          char.MinValue,
          '￼',
          '�',
          '\uFFFE',
          char.MaxValue
        };
        StringBuilder sb = (StringBuilder) null;
        int length = 0;
        int i = 0;
        Action action = (Action) (() =>
        {
          if (sb == null)
            sb = new StringBuilder();
          sb.Append(str, length, i - length);
          length = i + 1;
        });
        for (i = 0; i < str.Length; ++i)
        {
          char c = str[i];
          switch (c)
          {
            case '\t':
            case '\n':
            case '\v':
            case '\r':
            case ' ':
              action();
              if (sb.Length == 0 || sb[sb.Length - 1] != ' ')
              {
                sb.Append(' ');
                break;
              }
              break;
            default:
              if (((IEnumerable<char>) source).Contains<char>(c) || char.IsControl(c))
              {
                action();
                break;
              }
              break;
          }
        }
        if (sb != null && length < str.Length)
          sb.Append(str, length, str.Length - length);
        if (sb != null)
          str = sb.ToString();
      }
      return str;
    }

    private void ShellToastExImpl(
      string[] content,
      string group,
      string uri,
      bool muted,
      string tone,
      string tag = null)
    {
      XmlDocument templateContent = ToastNotificationManager.GetTemplateContent(content.Length == 1 ? (ToastTemplateType) 4 : (ToastTemplateType) 5);
      XmlElement xmlElement = (XmlElement) templateContent.SelectSingleNode("/toast");
      xmlElement.SetAttribute("launch", "#" + uri);
      if (tone != null)
      {
        XmlElement element = templateContent.CreateElement("audio");
        element.SetAttribute("src", Wns.ConvertUri(tone));
        xmlElement.AppendChild((IXmlNode) element);
      }
      XmlNodeList elementsByTagName = ((XmlElement) xmlElement.SelectSingleNode("visual/binding")).GetElementsByTagName("text");
      int index = 0;
      foreach (string str in content)
      {
        if (str.Length > 128)
          str = Utils.TruncateAtIndex(str, 128);
        try
        {
          ((IXmlNodeSerializer) ((IReadOnlyList<IXmlNode>) elementsByTagName)[index]).put_InnerText(Wns.RemoveEmbeddedNulls(str));
        }
        catch (Exception ex)
        {
          Log.d("WNSToast", Convert.ToBase64String(Encoding.UTF8.GetBytes(str)));
          throw;
        }
        ++index;
      }
      ToastNotification toastNotification = new ToastNotification(templateContent);
      if (group != null)
        toastNotification.put_Group(group);
      if (!string.IsNullOrEmpty(tag))
      {
        try
        {
          toastNotification.put_Tag(tag);
        }
        catch (Exception ex)
        {
          Log.l("WNSToast", "Exception setting tag of length: {0}", (object) tag.Length);
          Log.SendCrashLog(ex, "Unexpected exception setting tag", logOnlyForRelease: true);
        }
      }
      toastNotification.put_SuppressPopup(muted);
      Utils.LazyInit<ToastNotifier>(ref this.toastNotifier, (Func<ToastNotifier>) (() => ToastNotificationManager.CreateToastNotifier(Wns.AppId)), this.toastInitLock);
      this.toastNotifier.Show(toastNotification);
    }

    public void ShellToastEx(
      string[] content,
      string group,
      string uri,
      bool muted,
      string tone,
      string tag)
    {
      Wns.Worker.Enqueue((Action) (() => this.ShellToastExImpl(content, group, uri, muted, tone, tag)));
    }

    public void ClearToastHistoryGroup(string group)
    {
      Log.l("WNS", "Clear toast group | group={0}", (object) group);
      if (!AppState.IsBackgroundAgent)
      {
        Wns.Worker.Enqueue((Action) (() =>
        {
          if (string.IsNullOrEmpty(group))
            ToastNotificationManager.History.Clear();
          else
            ToastNotificationManager.History.RemoveGroup(group);
        }));
      }
      else
      {
        Log.l("WNS", "Sending ClearToast to server - ConversationID: {0}", (object) group);
        AppState.GetConnection()?.SendClearToast(group);
      }
    }

    public void ClearToastHistoryMessage(string tag, string group)
    {
      Wns.Worker.Enqueue((Action) (() =>
      {
        if (string.IsNullOrEmpty(tag) || string.IsNullOrEmpty(group))
          return;
        Log.l("WNS", "Clear toast | tag={0}, group={1}", (object) tag, (object) group);
        ToastNotificationManager.History.Remove(tag, group, Wns.AppId);
      }));
    }

    private static string TruncateLeadingSlashes(string str, int startIndex = 0)
    {
      int num = startIndex;
      while (num < str.Length && str[num] == '/')
        ++num;
      if (num != 0)
        str = str.Substring(num);
      return str;
    }

    public static string ConvertUri(string source)
    {
      if (source == null)
        return (string) null;
      string str = "isostore:";
      source = !source.StartsWith(str) ? "ms-appx:///" + Wns.TruncateLeadingSlashes(source) : "ms-appdata:///local/" + Wns.TruncateLeadingSlashes(source, str.Length);
      return source;
    }

    public static Uri ConvertUri(Uri source)
    {
      if (source == (Uri) null)
        return source;
      string originalString = source.OriginalString;
      string uriString = Wns.ConvertUri(originalString);
      if (uriString != originalString)
        source = new Uri(uriString);
      return source;
    }

    private string FilterTileId(string key)
    {
      using (SHA1Managed shA1Managed = new SHA1Managed())
      {
        byte[] bytes = Encoding.UTF8.GetBytes(key);
        return shA1Managed.ComputeHash(bytes, 0, bytes.Length).ToHexString();
      }
    }

    public void CreateTile(
      string key,
      string title,
      int initialCount,
      string initialContent,
      Uri uri,
      Uri backgroundImage,
      Uri smallbackgroundImage)
    {
      SecondaryTile tile = new SecondaryTile(this.FilterTileId(key), title, uri.OriginalString, Wns.ConvertUri(backgroundImage), (TileSize) 3);
      tile.VisualElements.put_ShowNameOnSquare150x150Logo(true);
      lock (this.secondaryTileLock)
        this.pendingSecondaryTiles.Add(key);
      this.CreateTileAsync(key, tile, (Func<Wns.SecondaryTileWrapper>) (() =>
      {
        Wns.SecondaryTileWrapper tile1 = new Wns.SecondaryTileWrapper(tile);
        tile1.SetTitle(title, false);
        tile1.SetCount(new int?(initialCount));
        tile1.SetWideContent(initialContent);
        tile1.SetBackgroundImage(backgroundImage);
        tile1.SetSmallBackgroundImage(smallbackgroundImage);
        return tile1;
      }));
    }

    private async void CreateTileAsync(
      string key,
      SecondaryTile tile,
      Func<Wns.SecondaryTileWrapper> wrapper)
    {
      bool async = await tile.RequestCreateAsync();
      if (async)
        wrapper().Update();
      lock (this.secondaryTileLock)
      {
        this.pendingSecondaryTiles.Remove(key);
        if (async)
          this.secondaryTiles[key] = tile;
        Action<SecondaryTile> action;
        if (!this.tileNotifyActions.TryGetValue(key, out action) || action == null)
          return;
        this.tileNotifyActions.Remove(key);
        action(async ? tile : (SecondaryTile) null);
      }
    }

    public ITile PrimaryTile => (ITile) new Wns.PrimaryTileWrapper();

    private void AddTileNotifyAction(string key, Action<SecondaryTile> a)
    {
      Action<SecondaryTile> oldAction = (Action<SecondaryTile>) null;
      this.tileNotifyActions.TryGetValue(key, out oldAction);
      this.tileNotifyActions[key] = (Action<SecondaryTile>) (tile =>
      {
        if (oldAction != null)
          oldAction(tile);
        a(tile);
      });
    }

    private void AddDeferredTile(string key, Wns.DeferredTile tile)
    {
      this.AddTileNotifyAction(key, (Action<SecondaryTile>) (realTile =>
      {
        if (realTile == null)
          tile.Cancel();
        else
          tile.AttachTile(this.WrapSecondaryTile(realTile));
      }));
    }

    private ITile WrapSecondaryTile(SecondaryTile tile)
    {
      return (ITile) new Wns.SecondaryTileWrapper(tile);
    }

    public ITile GetSecondaryTile(Func<Uri, bool> selector, string key)
    {
      SecondaryTile tile1 = (SecondaryTile) null;
      Wns.DeferredTile tile2 = (Wns.DeferredTile) null;
      if (!this.SecondaryTileExists(selector, key))
        return (ITile) null;
      lock (this.secondaryTileLock)
      {
        if (!this.scannedSecondaryTiles)
        {
          ThreadPool.QueueUserWorkItem((WaitCallback) (_ => this.ScanSecondaryTiles()));
          tile2 = new Wns.DeferredTile();
          this.AddDeferredTile(key, tile2);
        }
        else if (this.pendingSecondaryTiles.Contains(key))
        {
          tile2 = new Wns.DeferredTile();
          this.AddDeferredTile(key, tile2);
        }
        else
          this.secondaryTiles.TryGetValue(key, out tile1);
      }
      return tile1 == null ? (ITile) tile2 : this.WrapSecondaryTile(tile1);
    }

    public bool SecondaryTileExists() => false;

    public bool SecondaryTileExists(Func<Uri, bool> selector, string key)
    {
label_0:
      bool flag = false;
      bool r = false;
      lock (this.secondaryTileLock)
      {
        if (!this.scannedSecondaryTiles)
        {
          Action<SecondaryTile> a = (Action<SecondaryTile>) (tile =>
          {
            if (tile == null)
              return;
            r = true;
          });
          this.AddTileNotifyAction(key, a);
          flag = true;
          ThreadPool.QueueUserWorkItem((WaitCallback) (_ => this.ScanSecondaryTiles()));
        }
      }
      if (flag)
        this.secondaryTileLoadedEvent.WaitOne();
      if (r)
        return true;
      lock (this.secondaryTileLock)
      {
        if (this.scannedSecondaryTiles)
          return this.pendingSecondaryTiles.Contains(key) || this.secondaryTiles.ContainsKey(key);
        goto label_0;
      }
    }

    private async void ScanSecondaryTiles()
    {
      IReadOnlyList<SecondaryTile> allAsync = await SecondaryTile.FindAllAsync(Wns.AppId);
      lock (this.secondaryTileLock)
      {
        foreach (SecondaryTile secondaryTile in (IEnumerable<SecondaryTile>) allAsync)
        {
          string jid = TileHelper.UriToJid(new Uri(secondaryTile.Arguments, UriKind.Relative));
          if (jid != null)
          {
            this.secondaryTiles[jid] = secondaryTile;
            Action<SecondaryTile> action;
            if (this.tileNotifyActions.TryGetValue(jid, out action) && action != null)
            {
              this.tileNotifyActions.Remove(jid);
              action(secondaryTile);
            }
          }
        }
        List<Action<SecondaryTile>> list = this.tileNotifyActions.Values.ToList<Action<SecondaryTile>>();
        this.tileNotifyActions.Clear();
        list.ForEach((Action<Action<SecondaryTile>>) (callback => callback((SecondaryTile) null)));
        this.scannedSecondaryTiles = true;
        this.secondaryTileLoadedEvent.Set();
      }
    }

    private void InvalidateTileCache()
    {
      lock (this.secondaryTileLock)
      {
        this.scannedSecondaryTiles = false;
        this.secondaryTiles.Clear();
        this.secondaryTileLoadedEvent.Reset();
      }
    }

    public Dictionary<string, string> ClientConfig
    {
      get
      {
        return new Dictionary<string, string>()
        {
          ["platform"] = "wns",
          ["version"] = "b"
        };
      }
    }

    private class TileWrapper : ITile
    {
      private Func<BadgeUpdater> badgeUpdateFunc;
      private BadgeUpdater badgeUpdater;
      private Func<TileUpdater> tileUpdateFunc;
      private Action deleteFunc;
      private int? count;
      private bool countDirty;
      private bool dirty;
      protected string title;
      protected string backTitle;
      protected string[] content;
      protected Uri backImage;
      protected Uri smallBackImage;
      private List<Action<TileUpdater>> callbackQueue = new List<Action<TileUpdater>>();

      public TileWrapper(
        Func<TileUpdater> tileUpdateFunc,
        Func<BadgeUpdater> badgeUpdateFunc,
        Action delete)
      {
        this.tileUpdateFunc = tileUpdateFunc;
        this.badgeUpdateFunc = badgeUpdateFunc;
        this.deleteFunc = delete;
      }

      public virtual void Clear()
      {
        this.tileUpdateFunc().Clear();
        this.dirty = true;
        this.SetCount(new int?());
      }

      private string EnforceMaxLength(string s, int maxLen = 128)
      {
        if (s != null && s.Length > maxLen)
          s = Utils.TruncateAtIndex(s, maxLen);
        return s;
      }

      public void SetTitle(string title, bool back = false)
      {
        title = this.EnforceMaxLength(title);
        if (back)
          this.backTitle = title;
        else
          this.title = title;
        this.dirty = true;
      }

      public void SetWideContent(IEnumerable<string> strs)
      {
        if (strs != null)
          strs = strs.Select<string, string>((Func<string, string>) (s => this.EnforceMaxLength(s)));
        this.content = PushSystem.SanitizeWideContent(strs).ToArray<string>();
        this.dirty = true;
      }

      public void SetCount(int? count)
      {
        this.countDirty = true;
        this.count = count;
      }

      public void SetBackgroundImage(Uri uri)
      {
        this.backImage = Wns.ConvertUri(uri);
        this.dirty = true;
      }

      public void SetSmallBackgroundImage(Uri uri)
      {
        this.smallBackImage = Wns.ConvertUri(uri);
        this.dirty = true;
      }

      public void Update()
      {
        Action onComplete = (Action) null;
        if (this.countDirty)
        {
          this.countDirty = false;
          XmlDocument badgeTemplate = BadgeUpdateManager.GetTemplateContent((BadgeTemplateType) 1);
          (badgeTemplate.SelectSingleNode("/badge") as XmlElement).SetAttribute("value", (this.count ?? 0).ToString());
          onComplete = (Action) (() =>
          {
            if (this.badgeUpdater == null)
              this.badgeUpdater = this.badgeUpdateFunc();
            this.badgeUpdater.Update(new BadgeNotification(badgeTemplate));
          });
        }
        if (this.dirty)
        {
          this.dirty = false;
          this.UpdateTileContent(onComplete);
          onComplete = (Action) null;
          this.title = this.backTitle = (string) null;
          this.content = (string[]) null;
          this.backImage = (Uri) null;
          this.smallBackImage = (Uri) null;
        }
        if (onComplete == null)
          return;
        onComplete();
      }

      protected void UpdateTileContent(Action onComplete = null)
      {
        List<Action<TileUpdater>> callbacks = new List<Action<TileUpdater>>();
        this.UpdateTileContent((Action<Action<TileUpdater>>) (callback => callbacks.Add(callback)));
        if (onComplete != null)
          callbacks.Add((Action<TileUpdater>) (u => onComplete()));
        Wns.Worker.Enqueue((Action) (() => this.UpdateTileContent(callbacks, 0)));
      }

      private void UpdateTileContent(List<Action<TileUpdater>> callbacks, int attempt)
      {
        TileUpdater tileUpdater;
        try
        {
          tileUpdater = this.tileUpdateFunc();
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "exception getting tile updater");
          this.callbackQueue.AddRange((IEnumerable<Action<TileUpdater>>) callbacks);
          callbacks.Clear();
          Wns.Worker.RunAfterDelay(TimeSpan.FromMilliseconds(500.0), (Action) (() => this.UpdateTileContent(callbacks, attempt + 1)));
          return;
        }
        foreach (Action<TileUpdater> action in this.callbackQueue.Concat<Action<TileUpdater>>((IEnumerable<Action<TileUpdater>>) callbacks))
          action(tileUpdater);
        this.callbackQueue.Clear();
      }

      protected virtual void UpdateTileContent(Action<Action<TileUpdater>> updater)
      {
      }

      public void Delete() => this.deleteFunc();
    }

    private class PrimaryTileWrapper : Wns.TileWrapper
    {
      public PrimaryTileWrapper()
        : base((Func<TileUpdater>) (() => TileUpdateManager.CreateTileUpdaterForApplication(Wns.AppId)), (Func<BadgeUpdater>) (() => BadgeUpdateManager.CreateBadgeUpdaterForApplication(Wns.AppId)), (Action) (() =>
        {
          throw new NotImplementedException();
        }))
      {
      }

      protected override void UpdateTileContent(Action<Action<TileUpdater>> onUpdater)
      {
        XmlDocument template = TileUpdateManager.GetTemplateContent((TileTemplateType) 77);
        XmlElement xmlElement1 = template.SelectSingleNode("/tile/visual/binding") as XmlElement;
        foreach (XmlElement xmlElement2 in ((IEnumerable<IXmlNode>) xmlElement1.ChildNodes).Select<IXmlNode, XmlElement>((Func<IXmlNode, XmlElement>) (n => n as XmlElement)).Where<XmlElement>((Func<XmlElement, bool>) (n => n != null)))
        {
          if (xmlElement2.NodeName == "image")
            xmlElement2.SetAttribute("src", "Images/IconicTileMedium.png");
          else if (xmlElement2.NodeName == "text")
          {
            string s = (string) null;
            string str = "";
            try
            {
              s = xmlElement2.GetAttribute("id");
            }
            catch (Exception ex)
            {
            }
            int result;
            if (this.content != null && s != null && int.TryParse(s, out result))
            {
              --result;
              if (result >= 0 && result < this.content.Length)
                str = this.content[result];
            }
            xmlElement2.put_InnerText(Wns.RemoveEmbeddedNulls(str));
          }
        }
        TileTemplateType[] tileTemplateTypeArray = new TileTemplateType[2]
        {
          (TileTemplateType) 76,
          (TileTemplateType) 75
        };
        foreach (int num in tileTemplateTypeArray)
        {
          XmlElement xmlElement3 = TileUpdateManager.GetTemplateContent((TileTemplateType) num).SelectSingleNode("/tile/visual/binding/image") as XmlElement;
          xmlElement3.SetAttribute("src", "Images/IconicTileMedium.png");
          IXmlNode ixmlNode = xmlElement1.OwnerDocument.ImportNode(xmlElement3.ParentNode, true);
          xmlElement1.ParentNode.AppendChild(ixmlNode);
        }
        onUpdater((Action<TileUpdater>) (updater => updater.Update(new TileNotification(template))));
      }
    }

    private class SecondaryTileWrapper : Wns.TileWrapper
    {
      private SecondaryTile tile;

      public SecondaryTileWrapper(SecondaryTile tile)
        : base((Func<TileUpdater>) (() => TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId)), (Func<BadgeUpdater>) (() => BadgeUpdateManager.CreateBadgeUpdaterForSecondaryTile(tile.TileId)), (Action) (() => tile.RequestDeleteAsync()))
      {
        this.tile = tile;
      }

      public override void Clear()
      {
        this.SetCount(new int?());
        this.title = this.backTitle = (string) null;
        this.content = (string[]) null;
        this.backImage = (Uri) null;
        this.smallBackImage = (Uri) null;
      }

      private TileNotification UpdateFront()
      {
        if (!(this.backImage != (Uri) null))
          return (TileNotification) null;
        string originalString1 = this.backImage.OriginalString;
        XmlDocument templateContent = TileUpdateManager.GetTemplateContent((TileTemplateType) 0);
        XmlElement xmlElement1 = templateContent.SelectSingleNode("/tile/visual/binding/image") as XmlElement;
        xmlElement1.SetAttribute("src", originalString1);
        XmlElement xmlElement2 = TileUpdateManager.GetTemplateContent((TileTemplateType) 78).SelectSingleNode("/tile/visual/binding/image") as XmlElement;
        XmlElement xmlElement3 = xmlElement2;
        Uri uri = this.smallBackImage;
        if ((object) uri == null)
          uri = this.backImage;
        string originalString2 = uri.OriginalString;
        xmlElement3.SetAttribute("src", originalString2);
        XmlElement parentNode = xmlElement1.ParentNode as XmlElement;
        parentNode.ParentNode.AppendChild(parentNode.OwnerDocument.ImportNode(xmlElement2.ParentNode, true));
        TileNotification tileNotification = new TileNotification(templateContent);
        tileNotification.put_Tag("0");
        return tileNotification;
      }

      private TileNotification UpdateBack()
      {
        XmlDocument templateContent = TileUpdateManager.GetTemplateContent((TileTemplateType) 5);
        XmlElement xmlElement = templateContent.SelectSingleNode("/tile/visual/binding/text") as XmlElement;
        string str1 = "";
        if (this.content != null && this.content.Length != 0)
          str1 = string.Join("\n", this.content);
        if (!string.IsNullOrEmpty(this.backTitle))
          str1 = str1.Length != 0 ? string.Format("{0}: {1}", (object) this.backTitle, (object) str1) : this.backTitle;
        string str2 = Wns.RemoveEmbeddedNulls(str1);
        xmlElement.put_InnerText(str2);
        TileNotification tileNotification = new TileNotification(templateContent);
        tileNotification.put_Tag("1");
        return tileNotification;
      }

      protected override void UpdateTileContent(Action<Action<TileUpdater>> onUpdater)
      {
        onUpdater((Action<TileUpdater>) (updater => updater.EnableNotificationQueue(true)));
        if (this.title != null)
        {
          string dpyName = this.title;
          onUpdater((Action<TileUpdater>) (u => this.tile.put_DisplayName(dpyName)));
        }
        Action<TileNotification> action = (Action<TileNotification>) (n =>
        {
          if (n == null)
            return;
          onUpdater((Action<TileUpdater>) (updater => updater.Update(n)));
        });
        action(this.UpdateFront());
        action(this.UpdateBack());
      }
    }

    private class DeferredTile : ITile
    {
      private List<Action<ITile>> pendingActions = new List<Action<ITile>>();
      private object pendingActionLock = new object();
      private ITile realTile;

      public void AttachTile(ITile tile)
      {
        List<Action<ITile>> actionList = (List<Action<ITile>>) null;
        lock (this.pendingActionLock)
        {
          this.realTile = tile;
          actionList = this.pendingActions;
          this.pendingActions = (List<Action<ITile>>) null;
        }
        actionList?.ForEach((Action<Action<ITile>>) (a => a(tile)));
      }

      public void Cancel()
      {
        lock (this.pendingActionLock)
          this.pendingActions = (List<Action<ITile>>) null;
      }

      private void PerformAction(Action<ITile> op)
      {
        if (this.realTile != null)
        {
          op(this.realTile);
        }
        else
        {
          if (this.pendingActions == null)
            return;
          lock (this.pendingActionLock)
          {
            if (this.realTile != null)
            {
              op(this.realTile);
            }
            else
            {
              if (this.pendingActions == null)
                return;
              this.pendingActions.Add(op);
            }
          }
        }
      }

      public void Clear() => this.PerformAction((Action<ITile>) (t => t.Clear()));

      public void SetTitle(string title, bool back = false)
      {
        this.PerformAction((Action<ITile>) (t => t.SetTitle(title, back)));
      }

      public void SetWideContent(IEnumerable<string> strs)
      {
        this.PerformAction((Action<ITile>) (t => t.SetWideContent(strs)));
      }

      public void SetCount(int? count)
      {
        this.PerformAction((Action<ITile>) (t => t.SetCount(count)));
      }

      public void SetBackgroundImage(Uri uri)
      {
        this.PerformAction((Action<ITile>) (t => t.SetBackgroundImage(uri)));
      }

      public void SetSmallBackgroundImage(Uri uri)
      {
        this.PerformAction((Action<ITile>) (t => t.SetSmallBackgroundImage(uri)));
      }

      public void Update() => this.PerformAction((Action<ITile>) (t => t.Update()));

      public void Delete() => this.PerformAction((Action<ITile>) (t => t.Delete()));
    }
  }
}
