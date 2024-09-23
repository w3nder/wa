// Decompiled with JetBrains decompiler
// Type: WhatsApp.LiveLocationView
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class LiveLocationView : PhoneApplicationPage
  {
    private DispatcherTimer UIUpdateTimer;
    private const string LogHdr = "llview";
    private const int PARTICIPANT_LIST_HEIGHT = 80;
    private DispatcherTimer SubscribeUpdateTimer;
    private static Message NextInstanceMessage;
    private string focusedJid;
    private bool autoFocus = true;
    private Message locationMessage;
    private string chatJid;
    private string senderJid;
    private ObservableCollection<LiveLocationParticipantViewModel> participantViewModels;
    private Dictionary<string, MapPoint> pins;
    private List<LiveLocationView.MergedPin> mergedPins = new List<LiveLocationView.MergedPin>();
    private HashSet<string> viewModelJids = new HashSet<string>();
    private bool isGroup;
    private IDisposable groupMembershipChangeSub;
    private HashSet<string> particpantsInGroup = new HashSet<string>();
    private bool userStoppedSharingTooltipActive;
    private int OFFSET = 268435456;
    private double RADIUS = 85445659.4471;
    private const double TARGET_SIZE = 1000.0;
    private const double METERS_IN_DEGREE_LATITUDE = 110574.0;
    private const double METERS_IN_DEGREE_LONG_AT_EQUATOR = 111320.0;
    private MapPoint _myMapPoint;
    private IDisposable _locationSubscription;
    private GeoCoordinate _myLocation;
    private bool _listCollapsed;
    private double _listCollapsedMapValue;
    private double _listCollapsedListValue;
    private double _listExpandedMapValue;
    private double _listExpandedListValue;
    internal SolidColorBrush HandleRectangleBrush;
    internal Grid LayoutRoot;
    internal CompositeTransform MapXForm;
    internal MapControl Map;
    internal Grid MapOverlay;
    internal CompositeTransform MapOverlayXForm;
    internal Grid CenterButton;
    internal Ellipse AccuracyEllipse;
    internal Ellipse Inner;
    internal Image CartographicModeButton;
    internal Grid ListContainer;
    internal CompositeTransform ListXForm;
    internal Grid ToolTip;
    internal TextBlock ToolTipText;
    internal Grid Handle;
    internal Rectangle HandleRect1;
    internal Rectangle HandleRect2;
    internal TextBlock Acc;
    internal WhatsApp.CompatibilityShims.LongListSelector SharingList;
    internal StackPanel ListHeader;
    internal Image ShareYourLiveLocationIcon;
    internal TextBlock SharingYourLiveLocationText;
    internal Rectangle SpacerForScrollingWhenListHeightIsSmall;
    internal StackPanel ErrorPanel;
    internal TextBlock ErrorTextBlock;
    private bool _contentLoaded;

    private void StartUIUpdateTimer()
    {
      if (this.UIUpdateTimer == null)
      {
        this.UIUpdateTimer = new DispatcherTimer();
        this.UIUpdateTimer.Tick += new EventHandler(this.UpdateTimer_Tick);
        this.UIUpdateTimer.Interval = TimeSpan.FromSeconds(3.0);
      }
      this.UIUpdateTimer.Start();
    }

    private void StopUIUpdateTimer()
    {
      if (this.UIUpdateTimer == null)
        return;
      this.UIUpdateTimer.Stop();
      this.UIUpdateTimer = (DispatcherTimer) null;
    }

    private void StartSubscribeUpdateTimer()
    {
      if (this.SubscribeUpdateTimer == null)
      {
        this.SubscribeUpdateTimer = new DispatcherTimer();
        this.SubscribeUpdateTimer.Tick += new EventHandler(this.SubscribeUpdate_Tick);
      }
      this.SubscribeUpdateTimer.Interval = TimeSpan.FromSeconds((double) Settings.LiveLocationSubscriptionDuration);
      this.SubscribeUpdateTimer.Start();
    }

    private void StopSubscribeUpdateTimer(bool dispose)
    {
      if (this.SubscribeUpdateTimer == null)
        return;
      this.SubscribeUpdateTimer.Stop();
      if (!dispose)
        return;
      this.SubscribeUpdateTimer = (DispatcherTimer) null;
    }

    public LiveLocationView()
    {
      this.InitializeComponent();
      this.locationMessage = LiveLocationView.NextInstanceMessage;
      LiveLocationView.NextInstanceMessage = (Message) null;
      this.Map.Loaded += new RoutedEventHandler(this.Map_Loaded);
      this.participantViewModels = new ObservableCollection<LiveLocationParticipantViewModel>();
      this.pins = new Dictionary<string, MapPoint>();
      this.ShareYourLiveLocationIcon.Source = (System.Windows.Media.ImageSource) AssetStore.InlineLiveLocationAccent;
      this.SharingYourLiveLocationText.Text = AppResources.LiveLocationTapToShare;
      this.MapOverlay.Margin = new Thickness(0.0, 0.0, 0.0, 144.0);
      this.SharingList.ItemsSource = (IList) this.participantViewModels;
      this.SharingList.MinHeight = 240.0;
      this.SharingList.SizeChanged += new SizeChangedEventHandler(this.SharingList_SizeChanged);
    }

    private void SharingList_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (!this.ListCollapsed)
        return;
      this.AnimateCollapseList();
    }

    private void Touch_FrameReported(object sender, TouchFrameEventArgs e)
    {
      if (e.GetPrimaryTouchPoint((UIElement) this.Map) == null)
        return;
      System.Windows.Point position = e.GetPrimaryTouchPoint((UIElement) this.Map).Position;
      double height = ResolutionHelper.GetRenderSize().Height;
      if (this._listCollapsed)
      {
        if (position.Y >= this.Map.ActualHeight || position.X >= this.Map.ActualWidth)
          return;
        this.focusedJid = (string) null;
        this.autoFocus = false;
      }
      else
      {
        if (position.Y >= height / 2.0 || position.X >= this.Map.ActualWidth)
          return;
        this.focusedJid = (string) null;
        this.autoFocus = false;
      }
    }

    private void SubscribeUpdate_Tick(object sender, EventArgs e)
    {
      this.StopSubscribeUpdateTimer(false);
      this.SubscribeToLocationUpdates(false);
      this.StartSubscribeUpdateTimer();
    }

    private void SubscribeToLocationUpdates(bool hasParticipants)
    {
      LiveLocationManager.Instance.SubscribeToLocationUpdates(this.chatJid, hasParticipants);
    }

    private void UpdateTimer_Tick(object sender, EventArgs e)
    {
      this.AddPins();
      if (!this.autoFocus)
        return;
      string key = (string) null;
      if (this.focusedJid != null)
      {
        key = this.focusedJid;
        if (this.pins.ContainsKey(key))
          this.Map.SetView(this.pins[key].Position(), -1.0, -1.0);
        else
          key = (string) null;
      }
      if (key != null)
        return;
      this.focusedJid = (string) null;
      this.autoFocus = false;
      this.CenterOnAll();
    }

    public void AddPins()
    {
      Dictionary<string, int?> jidsSharingForGroup = LiveLocationManager.Instance.GetJidsSharingForGroup(this.chatJid);
      Dictionary<string, int?> dictionary = jidsSharingForGroup;
      if (this.isGroup && this.particpantsInGroup.Count<string>() > 0)
      {
        dictionary = new Dictionary<string, int?>();
        foreach (string key in jidsSharingForGroup.Keys)
        {
          if (this.particpantsInGroup.Contains(key))
            dictionary.Add(key, jidsSharingForGroup[key]);
        }
      }
      Dictionary<string, int?>.KeyCollection keys = dictionary.Keys;
      this.ToolTip.Visibility = this.userStoppedSharingTooltipActive || keys.Count<string>() == 0 ? Visibility.Visible : Visibility.Collapsed;
      foreach (string str in keys)
      {
        if (!this.viewModelJids.Contains(str))
          this.AddJid(str, jidsSharingForGroup[str]);
      }
      List<LiveLocationParticipantViewModel> source1 = new List<LiveLocationParticipantViewModel>();
      foreach (LiveLocationParticipantViewModel participantViewModel in (Collection<LiveLocationParticipantViewModel>) this.participantViewModels)
      {
        if (keys.Contains<string>(participantViewModel.Jid))
        {
          if (!this.UpdateJid(participantViewModel, dictionary[participantViewModel.Jid]))
            source1.Add(participantViewModel);
        }
        else
          source1.Add(participantViewModel);
      }
      for (int index = 0; index < source1.Count<LiveLocationParticipantViewModel>(); ++index)
        this.RemoveJid(source1[index].Jid, source1[index]);
      if (jidsSharingForGroup.Count != this.pins.Count<KeyValuePair<string, MapPoint>>())
        Log.d("llview", "Group count: {0}, in group {1}, showing: {2}", (object) jidsSharingForGroup.Count, (object) keys.Count, (object) this.pins.Count<KeyValuePair<string, MapPoint>>());
      this.ListHeader.Visibility = !Settings.LiveLocationEnabled || Settings.LiveLocationIsNewUser || keys.Contains<string>(Settings.MyJid) ? Visibility.Collapsed : Visibility.Visible;
      int num = 64;
      List<LiveLocationView.MergedPin> source2 = new List<LiveLocationView.MergedPin>();
      HashSet<string> stringSet = new HashSet<string>();
      foreach (string key in this.pins.Keys)
      {
        if (!(key == this.focusedJid) && !stringSet.Contains(key))
        {
          bool flag = false;
          for (int index = 0; index < source2.Count<LiveLocationView.MergedPin>(); ++index)
          {
            GeoCoordinate geoCoordinate = this.pins[key].Position();
            GeoCoordinate center = source2[index].center;
            if (this.pixelDistance(geoCoordinate, center, (int) this.Map.ZoomLevel) < num)
            {
              source2[index].center = this.MidPoint(geoCoordinate, center);
              source2[index].pins.Add(key);
              stringSet.Add(key);
              flag = true;
              break;
            }
          }
          if (!flag)
          {
            for (int index = 0; index < this.pins.Count<KeyValuePair<string, MapPoint>>(); ++index)
            {
              string str = this.pins.Keys.ElementAt<string>(index);
              if (key != str && !stringSet.Contains(str))
              {
                GeoCoordinate geoCoordinate1 = this.pins[key].Position();
                GeoCoordinate geoCoordinate2 = this.pins[str].Position();
                if (this.pixelDistance(geoCoordinate1, geoCoordinate2, (int) this.Map.ZoomLevel) < num)
                {
                  source2.Add(new LiveLocationView.MergedPin(key, str, this.MidPoint(geoCoordinate1, geoCoordinate2)));
                  stringSet.Add(key);
                  stringSet.Add(str);
                  break;
                }
              }
            }
          }
        }
      }
      foreach (string key in this.pins.Keys)
        this.pins[key].Element().Visibility = stringSet.Contains(key) ? Visibility.Collapsed : Visibility.Visible;
      foreach (LiveLocationView.MergedPin mergedPin in this.mergedPins)
      {
        MapPoint point = mergedPin.point;
        point.Element().Tap -= new EventHandler<System.Windows.Input.GestureEventArgs>(this.Pin_Tap);
        point.Remove();
      }
      this.mergedPins = source2;
      this.addMergedPins();
    }

    private static double DegreesToRadians(double angle) => Math.PI * angle / 180.0;

    private double RadiansToDegrees(double radians) => 180.0 * radians / Math.PI;

    private GeoCoordinate MidPoint(GeoCoordinate posA, GeoCoordinate posB)
    {
      GeoCoordinate geoCoordinate = new GeoCoordinate();
      double radians = LiveLocationView.DegreesToRadians(posB.Longitude - posA.Longitude);
      double num = Math.Cos(LiveLocationView.DegreesToRadians(posB.Latitude)) * Math.Cos(radians);
      double y = Math.Cos(LiveLocationView.DegreesToRadians(posB.Latitude)) * Math.Sin(radians);
      geoCoordinate.Latitude = this.RadiansToDegrees(Math.Atan2(Math.Sin(LiveLocationView.DegreesToRadians(posA.Latitude)) + Math.Sin(LiveLocationView.DegreesToRadians(posB.Latitude)), Math.Sqrt((Math.Cos(LiveLocationView.DegreesToRadians(posA.Latitude)) + num) * (Math.Cos(LiveLocationView.DegreesToRadians(posA.Latitude)) + num) + y * y)));
      geoCoordinate.Longitude = posA.Longitude + this.RadiansToDegrees(Math.Atan2(y, Math.Cos(LiveLocationView.DegreesToRadians(posA.Latitude)) + num));
      return geoCoordinate;
    }

    public int pixelDistance(GeoCoordinate coord1, GeoCoordinate coord2, int zoom)
    {
      double x1 = this.longitudeToX(coord1.Longitude);
      double y1 = this.latitudeToY(coord1.Latitude);
      double x2 = this.longitudeToX(coord2.Longitude);
      double y2 = this.latitudeToY(coord2.Latitude);
      double num = x2;
      return (int) Math.Sqrt(Math.Pow(x1 - num, 2.0) + Math.Pow(y1 - y2, 2.0)) >> 21 - zoom;
    }

    public double longitudeToX(double lon)
    {
      return Math.Round((double) this.OFFSET + this.RADIUS * LiveLocationView.DegreesToRadians(lon));
    }

    public double latitudeToY(double lat)
    {
      return Math.Round((double) this.OFFSET - this.RADIUS * Math.Log((1.0 + Math.Sin(LiveLocationView.DegreesToRadians(lat))) / (1.0 - Math.Sin(LiveLocationView.DegreesToRadians(lat)))) / 2.0);
    }

    private void RemoveJid(string jid, LiveLocationParticipantViewModel model)
    {
      Log.l("llview", "Removing {0} from the map", (object) jid);
      this.viewModelJids.Remove(jid);
      this.participantViewModels.Remove(model);
      MapPoint pin = this.pins[jid];
      if (pin != null)
      {
        pin.Element().Tap -= new EventHandler<System.Windows.Input.GestureEventArgs>(this.Pin_Tap);
        pin.Remove();
      }
      this.pins.Remove(jid);
      this.userStoppedSharingTooltipActive = true;
      this.ToolTip.Visibility = Visibility.Visible;
      this.ToolTipText.Text = !(jid == Settings.MyJid) ? string.Format(AppResources.LiveLocationStoppedSharing, (object) model.GetTitle()) : AppResources.LiveLocationYouStoppedSharing;
      this.Dispatcher.RunAfterDelay(TimeSpan.FromMilliseconds(10000.0), (Action) (() =>
      {
        this.userStoppedSharingTooltipActive = false;
        this.ToolTip.Visibility = this.viewModelJids.Count<string>() == 0 ? Visibility.Visible : Visibility.Collapsed;
        this.ToolTipText.Text = AppResources.LiveLocationNoOneSharing;
      }));
    }

    private void addMergedPins()
    {
      foreach (LiveLocationView.MergedPin mergedPin in this.mergedPins)
      {
        MapPoint mapPoint = this.Map.AddPoint(MapPointStyle.Live);
        mapPoint.SetCoordinate(mergedPin.center);
        LiveLocationPin liveLocationPin1 = mapPoint.Element() as LiveLocationPin;
        List<System.Windows.Media.ImageSource> imageSourceList = new List<System.Windows.Media.ImageSource>();
        foreach (string pin in mergedPin.pins)
        {
          LiveLocationPin liveLocationPin2 = this.pins[pin].Element() as LiveLocationPin;
          imageSourceList.Add(liveLocationPin2.PinIcons.FirstOrDefault<System.Windows.Media.ImageSource>() ?? (System.Windows.Media.ImageSource) AssetStore.DefaultContactIconBlack);
        }
        liveLocationPin1.PinIcons = imageSourceList;
        liveLocationPin1.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.Pin_Tap);
        mergedPin.point = mapPoint;
      }
    }

    private void AddJid(string jid, int? expiration)
    {
      LocationData locationDataForJid = LiveLocationManager.Instance.GetLocationDataForJid(jid);
      if (locationDataForJid == null)
      {
        Log.l("llview", "No location data found for added jid {0}", (object) jid);
      }
      else
      {
        Log.l("llview", "Adding {0} to map", (object) jid);
        this.viewModelJids.Add(jid);
        LiveLocationParticipantViewModel participantViewModel = new LiveLocationParticipantViewModel(this.chatJid, UserCache.Get(jid, true), expiration, locationDataForJid.Timestamp);
        this.participantViewModels.Add(participantViewModel);
        MapPoint mapPoint = this.Map.AddPoint(MapPointStyle.Live);
        GeoCoordinate loc = new GeoCoordinate(locationDataForJid.Latitude.Value, locationDataForJid.Longitude.Value);
        mapPoint.SetCoordinate(loc);
        LiveLocationPin liveLocationPin = mapPoint.Element() as LiveLocationPin;
        List<System.Windows.Media.ImageSource> PinIcons = new List<System.Windows.Media.ImageSource>();
        System.Windows.Media.ImageSource cached = (System.Windows.Media.ImageSource) null;
        if (!participantViewModel.GetCachedPicSource(out cached))
          participantViewModel.GetPictureSourceObservable(true, true).SubscribeOn<System.Windows.Media.ImageSource>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<System.Windows.Media.ImageSource>().Subscribe<System.Windows.Media.ImageSource>((Action<System.Windows.Media.ImageSource>) (imgSrc => PinIcons.Add(imgSrc ?? (System.Windows.Media.ImageSource) AssetStore.DefaultContactIconBlack)));
        else
          PinIcons.Add(cached ?? (System.Windows.Media.ImageSource) AssetStore.DefaultContactIconBlack);
        List<System.Windows.Media.ImageSource> imageSourceList = PinIcons;
        liveLocationPin.PinIcons = imageSourceList;
        mapPoint.Element().Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.Pin_Tap);
        this.pins.Add(jid, mapPoint);
      }
    }

    private void CenterOnAll(int delay = 0)
    {
      this.CenterOnGroup(new List<string>((IEnumerable<string>) LiveLocationManager.Instance.GetJidsSharingForGroup(this.chatJid).Keys));
    }

    private void CenterOnGroup(List<string> userList)
    {
      List<GeoCoordinate> geoCoordinateList = new List<GeoCoordinate>();
      double num1 = -1.0;
      double num2 = -1.0;
      GeoCoordinate geoCoordinate1 = (GeoCoordinate) null;
      foreach (string user in userList)
      {
        LocationData locationDataForJid = LiveLocationManager.Instance.GetLocationDataForJid(user);
        if (locationDataForJid == null)
        {
          Log.d("llview", "Location not avalable for {0}, ignoring for center", (object) user);
        }
        else
        {
          GeoCoordinate geoCoordinate2 = new GeoCoordinate();
          double? nullable = locationDataForJid.Latitude;
          geoCoordinate2.Latitude = nullable.Value;
          nullable = locationDataForJid.Longitude;
          geoCoordinate2.Longitude = nullable.Value;
          GeoCoordinate geoCoordinate3 = geoCoordinate2;
          geoCoordinateList.Add(geoCoordinate3);
          if (user == this.senderJid)
            geoCoordinate1 = geoCoordinate3;
        }
      }
      GeoCoordinate geoCoordinate4 = geoCoordinate1;
      if ((object) geoCoordinate4 == null)
        geoCoordinate4 = geoCoordinateList.FirstOrDefault<GeoCoordinate>();
      GeoCoordinate geoCoordinate5 = geoCoordinate4;
      if (geoCoordinateList.Count > 1)
      {
        LocationRect locationRect = LocationRect.CreateLocationRect((IEnumerable<GeoCoordinate>) geoCoordinateList);
        num1 = locationRect.Height + locationRect.Height / 5.0;
        num2 = locationRect.Width + locationRect.Width / 5.0;
        geoCoordinate5 = locationRect.Center;
      }
      MapControl map = this.Map;
      GeoCoordinate center = geoCoordinate5;
      if ((object) center == null)
        center = this._myLocation;
      double widthInDegrees = num2;
      double heightInDegrees = num1;
      map.SetView(center, widthInDegrees, heightInDegrees);
    }

    private object GetPictureSourceObservable(bool v1, bool v2)
    {
      throw new NotImplementedException();
    }

    private bool UpdateJid(LiveLocationParticipantViewModel model, int? expiration)
    {
      string jid = model.Jid;
      UserCache.Get(jid, false);
      LocationData locationDataForJid = LiveLocationManager.Instance.GetLocationDataForJid(jid);
      if (locationDataForJid != null)
      {
        long unixTime = locationDataForJid.Timestamp.ToUnixTime();
        int? nullable1 = expiration;
        long? nullable2 = nullable1.HasValue ? new long?((long) nullable1.GetValueOrDefault()) : new long?();
        long valueOrDefault = nullable2.GetValueOrDefault();
        if ((unixTime > valueOrDefault ? (nullable2.HasValue ? 1 : 0) : 0) == 0)
        {
          model.updateViewModel(expiration, locationDataForJid.Timestamp);
          MapPoint pin = this.pins[jid];
          double? nullable3 = locationDataForJid.Latitude;
          double latitude = nullable3.Value;
          nullable3 = locationDataForJid.Longitude;
          double longitude = nullable3.Value;
          GeoCoordinate loc = new GeoCoordinate(latitude, longitude);
          pin.SetCoordinate(loc);
          this.pins[jid] = pin;
          return true;
        }
      }
      Log.l("llview", "Can't find location for {0} or we have expired", (object) jid);
      LiveLocationManager.Instance.ClearExpiredLocationsAndSave();
      return false;
    }

    private void Map_Loaded(object sender, RoutedEventArgs e)
    {
      this.ListCollapsed = true;
      this.CenterOnAll(500);
    }

    public static GeoCoordinate GetCentralGeoCoordinate(List<GeoCoordinate> locations)
    {
      if (locations.Count == 1)
        return locations.FirstOrDefault<GeoCoordinate>();
      double num1 = 0.0;
      double num2 = 0.0;
      double num3 = 0.0;
      foreach (GeoCoordinate location in locations)
      {
        double num4 = location.Latitude * Math.PI / 180.0;
        double num5 = location.Longitude * Math.PI / 180.0;
        num1 += Math.Cos(num4) * Math.Cos(num5);
        num2 += Math.Cos(num4) * Math.Sin(num5);
        num3 += Math.Sin(num4);
      }
      int count = locations.Count;
      double x1 = num1 / (double) count;
      double y1 = num2 / (double) count;
      double y2 = num3 / (double) count;
      double num6 = Math.Atan2(y1, x1);
      double x2 = Math.Sqrt(x1 * x1 + y1 * y1);
      return new GeoCoordinate(Math.Atan2(y2, x2) * 180.0 / Math.PI, num6 * 180.0 / Math.PI);
    }

    private static void CalculateMapSize(
      GeoCoordinate center,
      out double widthInDegrees,
      out double heightInDegrees)
    {
      widthInDegrees = 0.0090437173295711461;
      heightInDegrees = 1000.0 / (110574.0 * Math.Cos(LiveLocationView.DegreesToRadians(center.Latitude)));
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.groupMembershipChangeSub.SafeDispose();
      this.groupMembershipChangeSub = (IDisposable) null;
      base.OnRemovedFromJournal(e);
      this.Map.DisposeAll();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (this.locationMessage == null)
      {
        Log.l("llview", "missing arguments to start livelocationview");
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
      }
      else
      {
        this.chatJid = this.locationMessage.KeyRemoteJid;
        this.senderJid = this.locationMessage.GetSenderJid();
        this.isGroup = JidHelper.IsGroupJid(this.chatJid);
        this.BeginLocationSearch();
        this.SubscribeToLocationUpdates(true);
        this.StartUIUpdateTimer();
        this.StartSubscribeUpdateTimer();
        Touch.FrameReported += new TouchFrameEventHandler(this.Touch_FrameReported);
        bool leaveImmediately = false;
        if (this.isGroup)
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            Conversation conversation = db.GetConversation(this.chatJid, CreateOptions.None);
            if (conversation == null)
            {
              Log.l("llview", "unexpectedly null group conversation: {0}", (object) this.chatJid);
            }
            else
            {
              this.SaveParticipants(conversation);
              leaveImmediately = !conversation.IsGroupParticipant();
              if (leaveImmediately)
                return;
              this.groupMembershipChangeSub = FunEventHandler.Events.GroupMembershipUpdatedSubject.Select<FunEventHandler.Events.ConversationWithFlags, Conversation>((Func<FunEventHandler.Events.ConversationWithFlags, Conversation>) (i => i.Conversation)).Where<Conversation>((Func<Conversation, bool>) (c => c.Jid == this.chatJid)).Subscribe<Conversation>((Action<Conversation>) (c =>
              {
                if (!c.IsGroupParticipant())
                  this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
                else
                  this.SaveParticipants(c);
              }));
            }
          }));
        else if (JidHelper.IsUserJid(this.chatJid))
        {
          bool isBlockedContact = false;
          ContactsContext.Instance((Action<ContactsContext>) (cdb => isBlockedContact = cdb.BlockListSet.ContainsKey(this.chatJid)));
          leaveImmediately = isBlockedContact;
        }
        if (!leaveImmediately)
          return;
        Log.l("llview", "Exit immediately requested for {0}", (object) this.chatJid);
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
      }
    }

    private void SaveParticipants(Conversation convo)
    {
      HashSet<string> stringSet = new HashSet<string>();
      foreach (string participantJid in convo.GetParticipantJids())
        stringSet.Add(participantJid);
      this.particpantsInGroup = stringSet;
    }

    public static void Start(Message msg)
    {
      LiveLocationView.NextInstanceMessage = msg;
      NavUtils.NavigateToPage(nameof (LiveLocationView));
    }

    private void BeginLocationSearch()
    {
      try
      {
        this._locationSubscription = LocationHelper.ObserveAccurateGeoCoordinate().ObserveOnDispatcher<GeoCoordinate>().Subscribe<GeoCoordinate>(new Action<GeoCoordinate>(this.OnAccurateGeoCoordinate), (Action<Exception>) (e => { }));
      }
      catch (Exception ex)
      {
      }
    }

    private void CenterCanvas_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (!(this._myLocation != (GeoCoordinate) null))
        return;
      this.Map.SetView(this._myLocation, 0.0, 0.0);
    }

    private void CartographicMode_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.Map.CartographicMode == MapMode.Road)
        this.Map.CartographicMode = MapMode.Hybrid;
      else
        this.Map.CartographicMode = MapMode.Road;
    }

    private void OnAccurateGeoCoordinate(GeoCoordinate gcoord)
    {
      this._myLocation = gcoord;
      if (this.viewModelJids.Contains(Settings.MyJid))
      {
        LiveLocationManager.Instance.NewIncomingLocation(Settings.MyJid, gcoord);
      }
      else
      {
        int num = this._myMapPoint == null ? 1 : 0;
        if (num != 0)
          this._myMapPoint = this.Map.AddPoint(MapPointStyle.MyLocation);
        this._myMapPoint.SetCoordinate(this._myLocation);
        if (num == 0)
          return;
        MapMyLocation mapMyLocation = this._myMapPoint.Element() as MapMyLocation;
        mapMyLocation.IsAccurate = true;
        mapMyLocation.AccuracyRadius = 0.0;
      }
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      if (this._locationSubscription != null)
        this._locationSubscription.SafeDispose();
      this.Map.DisposeAll();
      this.StopUIUpdateTimer();
      this.StopSubscribeUpdateTimer(true);
      LiveLocationManager.Instance.UnsubscribeToLocationUpdates(this.chatJid, (string) null);
      LiveLocationManager.Instance.ClearExpiredLocationsAndSave();
      foreach (IDisposable participantViewModel in (Collection<LiveLocationParticipantViewModel>) this.participantViewModels)
        participantViewModel.SafeDispose();
      Touch.FrameReported -= new TouchFrameEventHandler(this.Touch_FrameReported);
      base.OnNavigatedFrom(e);
    }

    private bool ListCollapsed
    {
      get => this._listCollapsed;
      set
      {
        if (this._listCollapsed == value)
          return;
        this._listCollapsed = value;
        if (this._listCollapsed)
          this.AnimateCollapseList();
        else
          this.AnimateExpandList();
      }
    }

    private void AnimateCollapseList()
    {
      if (this.participantViewModels.Count > 0)
        this.SharingList.ScrollTo(this.SharingList.ListHeader);
      Storyboard resource = this.Resources[(object) "MoveList"] as Storyboard;
      DoubleAnimation child1 = resource.Children[0] as DoubleAnimation;
      DoubleAnimation child2 = resource.Children[1] as DoubleAnimation;
      double num1 = this.SharingList.ActualHeight - this.SharingList.MinHeight;
      double num2 = -num1 / 2.0;
      double num3 = num1;
      child1.From = new double?(this.MapXForm.TranslateY);
      child1.To = new double?(num2);
      child2.From = new double?(this.ListXForm.TranslateY);
      child2.To = new double?(num3);
      this.MapOverlayXForm.TranslateY = -num1;
      Storyboarder.Perform(resource, false);
      this.SpacerForScrollingWhenListHeightIsSmall.Height = num1;
    }

    private void AnimateExpandList()
    {
      Storyboard resource = this.Resources[(object) "MoveList"] as Storyboard;
      DoubleAnimation child1 = resource.Children[0] as DoubleAnimation;
      DoubleAnimation child2 = resource.Children[1] as DoubleAnimation;
      double num1 = -this.ListContainer.ActualHeight / 2.0;
      double num2 = 0.0;
      child1.From = new double?(this.MapXForm.TranslateY);
      child1.To = new double?(num1);
      child2.From = new double?(this.ListXForm.TranslateY);
      child2.To = new double?(num2);
      Storyboarder.Perform(resource, false);
      this.SpacerForScrollingWhenListHeightIsSmall.Height = 0.0;
    }

    private void MyMap_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.ListCollapsed)
        return;
      this.ListCollapsed = true;
    }

    private void ListHandle_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.ListCollapsed = !this.ListCollapsed;
    }

    private void ListContainer_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      double num = this.SharingList.ActualHeight - this.SharingList.MinHeight;
      this._listCollapsedMapValue = -num / 2.0;
      this._listCollapsedListValue = num;
      this._listExpandedMapValue = -this.ListContainer.ActualHeight / 2.0;
      this._listExpandedListValue = 0.0;
    }

    private void ListContainer_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      double num1 = this.ListCollapsed ? this._listCollapsedMapValue : this._listExpandedMapValue;
      double num2 = this.ListCollapsed ? this._listCollapsedListValue : this._listExpandedListValue;
      CompositeTransform mapXform = this.MapXForm;
      double num3 = num1;
      System.Windows.Point translation = e.CumulativeManipulation.Translation;
      double num4 = translation.Y / 2.0;
      double num5 = Math.Max(Math.Min(num3 + num4, this._listCollapsedMapValue), this._listExpandedMapValue);
      mapXform.TranslateY = num5;
      CompositeTransform listXform = this.ListXForm;
      double num6 = num2;
      translation = e.CumulativeManipulation.Translation;
      double y = translation.Y;
      double num7 = Math.Max(Math.Min(num6 + y, this._listCollapsedListValue), this._listExpandedListValue);
      listXform.TranslateY = num7;
    }

    private void ListContainer_ManipulationCompleted(
      object sender,
      ManipulationCompletedEventArgs e)
    {
      if (Math.Abs(e.TotalManipulation.Translation.Y) <= 0.0)
        return;
      if (this.ListCollapsed)
        this.ListCollapsed = this.ListXForm.TranslateY - this._listExpandedListValue > (this._listCollapsedListValue - this._listExpandedListValue) * 2.0 / 3.0;
      else
        this.ListCollapsed = this.ListXForm.TranslateY - this._listExpandedListValue > (this._listCollapsedListValue - this._listExpandedListValue) * 1.0 / 3.0;
    }

    private void SharingList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      LiveLocationParticipantViewModel item = (sender as UserItemControl).ViewModel as LiveLocationParticipantViewModel;
      GeoCoordinate point = this.pins[item.Jid].Position();
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        this.focusedJid = item.Jid;
        this.autoFocus = true;
        this.Map.SetView(point, 0.0, 0.0);
      }));
    }

    private void Pin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      MapPoint mapPoint = (MapPoint) null;
      foreach (string key in this.pins.Keys)
      {
        MapPoint pin = this.pins[key];
        if (pin.Element() == sender)
        {
          this.focusedJid = key;
          mapPoint = pin;
          break;
        }
      }
      if (mapPoint != null)
      {
        GeoCoordinate center = mapPoint.Position();
        this.autoFocus = true;
        this.Map.SetView(center, 0.0, 0.0);
      }
      else
      {
        LiveLocationView.MergedPin mergedPin1 = (LiveLocationView.MergedPin) null;
        foreach (LiveLocationView.MergedPin mergedPin2 in this.mergedPins)
        {
          if (mergedPin2.point.Element() == sender)
          {
            mergedPin1 = mergedPin2;
            break;
          }
        }
        if (mergedPin1 != null)
          this.CenterOnGroup(mergedPin1.pins);
        else
          Log.l("llview", "Pin_Tap did not find associated pin");
      }
    }

    private void ListHeader_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      IDisposable successfulShareSubscription = (IDisposable) null;
      successfulShareSubscription = ShareLocationPage.StartForLiveLocationOnly(this.NavigationService, this.chatJid).Subscribe<Unit>((Action<Unit>) (_ =>
      {
        successfulShareSubscription.SafeDispose();
        successfulShareSubscription = (IDisposable) null;
      }), (Action) (() =>
      {
        successfulShareSubscription.SafeDispose();
        successfulShareSubscription = (IDisposable) null;
      }));
    }

    private void Handle_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      this.Handle.Background = this.Resources[(object) "PhoneAccentBrush"] as Brush;
      this.HandleRect1.Fill = (Brush) UIUtils.WhiteBrush;
      this.HandleRect2.Fill = (Brush) UIUtils.WhiteBrush;
    }

    private void Handle_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      this.Handle.Background = this.Resources[(object) "PhoneChromeBrush"] as Brush;
      this.HandleRect1.Fill = this.Resources[(object) "PhoneForegroundBrush"] as Brush;
      this.HandleRect2.Fill = this.Resources[(object) "PhoneForegroundBrush"] as Brush;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/LiveLocationView.xaml", UriKind.Relative));
      this.HandleRectangleBrush = (SolidColorBrush) this.FindName("HandleRectangleBrush");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.MapXForm = (CompositeTransform) this.FindName("MapXForm");
      this.Map = (MapControl) this.FindName("Map");
      this.MapOverlay = (Grid) this.FindName("MapOverlay");
      this.MapOverlayXForm = (CompositeTransform) this.FindName("MapOverlayXForm");
      this.CenterButton = (Grid) this.FindName("CenterButton");
      this.AccuracyEllipse = (Ellipse) this.FindName("AccuracyEllipse");
      this.Inner = (Ellipse) this.FindName("Inner");
      this.CartographicModeButton = (Image) this.FindName("CartographicModeButton");
      this.ListContainer = (Grid) this.FindName("ListContainer");
      this.ListXForm = (CompositeTransform) this.FindName("ListXForm");
      this.ToolTip = (Grid) this.FindName("ToolTip");
      this.ToolTipText = (TextBlock) this.FindName("ToolTipText");
      this.Handle = (Grid) this.FindName("Handle");
      this.HandleRect1 = (Rectangle) this.FindName("HandleRect1");
      this.HandleRect2 = (Rectangle) this.FindName("HandleRect2");
      this.Acc = (TextBlock) this.FindName("Acc");
      this.SharingList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("SharingList");
      this.ListHeader = (StackPanel) this.FindName("ListHeader");
      this.ShareYourLiveLocationIcon = (Image) this.FindName("ShareYourLiveLocationIcon");
      this.SharingYourLiveLocationText = (TextBlock) this.FindName("SharingYourLiveLocationText");
      this.SpacerForScrollingWhenListHeightIsSmall = (Rectangle) this.FindName("SpacerForScrollingWhenListHeightIsSmall");
      this.ErrorPanel = (StackPanel) this.FindName("ErrorPanel");
      this.ErrorTextBlock = (TextBlock) this.FindName("ErrorTextBlock");
    }

    public class MergedPin
    {
      public List<string> pins = new List<string>();
      public GeoCoordinate center;
      public MapPoint point;

      public MergedPin(string jid1, string jid2, GeoCoordinate center)
      {
        this.pins.Add(jid1);
        this.pins.Add(jid2);
        this.center = center;
      }
    }
  }
}
