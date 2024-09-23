// Decompiled with JetBrains decompiler
// Type: WhatsApp.LocationView
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;

#nullable disable
namespace WhatsApp
{
  public class LocationView : PhoneApplicationPage
  {
    private static LocationView.InitState nextInstanceInitState;
    private GeoCoordinate targetCoordinate;
    private MapPoint targetPoint;
    private string targetName;
    private Message.PlaceDetails placeDetails;
    private GeoCoordinate ownCoordinate;
    private MapPoint ownPoint;
    private IDisposable ownLocationSub;
    private bool isInited;
    private Storyboard slideDownSb;
    internal Grid LayoutRoot;
    internal MapControl Map;
    internal Grid CenterButton;
    internal Ellipse AccuracyEllipse;
    internal Ellipse Inner;
    internal Image CartographicModeButton;
    internal StackPanel DetailsPanel;
    internal TextBlock PlaceName;
    internal TextBlock PlaceAddress;
    internal TextBlock HostName;
    private bool _contentLoaded;

    public LocationView()
    {
      this.InitializeComponent();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.Map.ApplyMemoryWorkaround((PhoneApplicationPage) this);
      this.Loaded += (RoutedEventHandler) ((sender, e) => this.InitOnLoaded());
      LocationView.InitState instanceInitState = LocationView.nextInstanceInitState;
      LocationView.nextInstanceInitState = (LocationView.InitState) null;
      if (instanceInitState == null)
        return;
      if (instanceInitState.TargetMessageId > 0)
      {
        this.InitWithMessageId(instanceInitState.TargetMessageId);
      }
      else
      {
        this.targetName = instanceInitState.TargetName;
        this.targetCoordinate = instanceInitState.TargetCoordinate;
        this.placeDetails = instanceInitState.TargetPlaceDetails;
      }
      if (!instanceInitState.DisableSelfLocating)
        return;
      this.CenterButton.Visibility = Visibility.Collapsed;
    }

    private static void StartImpl(
      int msgId,
      string name,
      GeoCoordinate coordinate,
      Message.PlaceDetails placeDetails,
      bool disableSelfLocating)
    {
      LocationView.nextInstanceInitState = new LocationView.InitState()
      {
        TargetMessageId = msgId,
        TargetName = name,
        TargetCoordinate = coordinate,
        TargetPlaceDetails = placeDetails,
        DisableSelfLocating = disableSelfLocating
      };
      NavUtils.NavigateToPage(nameof (LocationView));
    }

    public static void Start(int msgId)
    {
      LocationView.StartImpl(msgId, (string) null, (GeoCoordinate) null, (Message.PlaceDetails) null, false);
    }

    public static void Start(
      string name,
      GeoCoordinate coordinate,
      Message.PlaceDetails placeDetails,
      bool disableSelfLocating)
    {
      LocationView.StartImpl(-1, name, coordinate, placeDetails, disableSelfLocating);
    }

    private void InitWithMessageId(int msgId)
    {
      Message msg = (Message) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => msg = db.GetMessageById(msgId)));
      if (msg == null || msg.MediaWaType != FunXMPP.FMessage.Type.Location)
        return;
      this.targetName = !msg.KeyFromMe ? UserCache.Get(msg.GetSenderJid(), true)?.GetDisplayName() : Settings.PushName + " (" + AppResources.You + ")";
      this.targetCoordinate = new GeoCoordinate(msg.Latitude, msg.Longitude);
      this.placeDetails = msg.ParsePlaceDetails();
    }

    private void InitOnLoaded()
    {
      if (this.isInited)
        return;
      this.isInited = true;
      if (this.targetCoordinate == (GeoCoordinate) null)
      {
        this.Dispatcher.BeginInvoke((Action) (() => this.SlideDownAndBackOut()));
      }
      else
      {
        try
        {
          this.targetPoint = this.Map.AddPoint(MapPointStyle.Sign);
          this.targetPoint.SetCoordinate(this.targetCoordinate);
          if (this.targetPoint.Element() is MapSign mapSign)
          {
            mapSign.Title = this.targetName;
            mapSign.Show();
          }
          this.Map.Center = this.targetCoordinate;
          this.LocatingSelf();
        }
        catch (ArgumentOutOfRangeException ex)
        {
          string context = string.Format("drawing location | lat:{0} lon:{1}", (object) this.targetCoordinate.Latitude, (object) this.targetCoordinate.Longitude);
          Log.SendCrashLog((Exception) ex, context);
          int num = (int) MessageBox.Show(AppResources.LocationParseFail);
        }
        if (this.placeDetails == null)
          return;
        if (!string.IsNullOrEmpty(this.placeDetails.Name))
        {
          this.PlaceName.Text = this.placeDetails.Name;
          this.PlaceName.Visibility = Visibility.Visible;
        }
        if (!string.IsNullOrEmpty(this.placeDetails.Address))
        {
          this.PlaceAddress.Text = this.placeDetails.Address;
          this.PlaceAddress.Visibility = Visibility.Visible;
        }
        if (this.placeDetails.Url != null)
        {
          Uri uri = new Uri(this.placeDetails.Url);
          if (!((IEnumerable<string>) Constants.LocationUrlWhitelist).Contains<string>(uri.Host))
          {
            this.HostName.Visibility = Visibility.Visible;
            this.HostName.Text = uri.Host;
          }
        }
        this.DetailsPanel.Visibility = Visibility.Visible;
      }
    }

    private void SlideDownAndBackOut()
    {
      if (this.slideDownSb == null)
        this.slideDownSb = WaAnimations.PageTransition(PageTransitionAnimation.SlideDownFadeOut);
      Storyboarder.Perform(this.slideDownSb, (DependencyObject) this.LayoutRoot, false, (Action) (() => NavUtils.GoBack(this.NavigationService)));
    }

    private void LocatingSelf()
    {
      try
      {
        this.ownLocationSub = LocationHelper.ObserveAccurateGeoCoordinate().ObserveOnDispatcher<GeoCoordinate>().Subscribe<GeoCoordinate>(new Action<GeoCoordinate>(this.OnAccurateGeoCoordinate), (Action<Exception>) (ex => Log.l(ex, "location view | locating")));
      }
      catch (Exception ex)
      {
        Log.l(ex, "location view | locating");
      }
    }

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      e.Cancel = true;
      base.OnBackKeyPress(e);
      this.SlideDownAndBackOut();
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      base.OnRemovedFromJournal(e);
      this.Map.DisposeAll();
    }

    private void OnDirectionsClick(object sender, EventArgs e)
    {
      string label = string.IsNullOrWhiteSpace(this.placeDetails?.Name) ? string.Format("{0}, {1}", (object) this.targetCoordinate.Latitude, (object) this.targetCoordinate.Longitude) : this.placeDetails.Name;
      new MapsDirectionsTask()
      {
        End = new LabeledMapLocation(label, this.targetCoordinate)
      }.Show();
    }

    private void CenterCanvas_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (!(this.ownCoordinate != (GeoCoordinate) null))
        return;
      this.Map.SetView(this.ownCoordinate, 0.0, 0.0);
    }

    private void CartographicMode_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.Map.CartographicMode == MapMode.Road)
        this.Map.CartographicMode = MapMode.Hybrid;
      else
        this.Map.CartographicMode = MapMode.Road;
    }

    private void OnAccurateGeoCoordinate(GeoCoordinate coordinate)
    {
      int num = this.ownPoint == null ? 1 : 0;
      if (num != 0)
        this.ownPoint = this.Map.AddPoint(MapPointStyle.MyLocation);
      this.ownCoordinate = coordinate;
      this.ownPoint.SetCoordinate(this.ownCoordinate);
      if (num != 0)
      {
        MapMyLocation mapMyLocation = this.ownPoint.Element() as MapMyLocation;
        mapMyLocation.IsAccurate = true;
        mapMyLocation.AccuracyRadius = 0.0;
      }
      if (!(this.targetCoordinate != (GeoCoordinate) null))
        return;
      string str = new DistanceToStringConverter().Convert((object) this.targetCoordinate.GetDistanceTo(this.ownCoordinate), typeof (double), (object) null, (CultureInfo) null) as string;
      if (string.IsNullOrEmpty(str))
        return;
      (this.targetPoint.Element() as MapSign).SubTitle = string.Format(AppResources.DistanceAway, (object) str);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      this.ownLocationSub.SafeDispose();
      this.ownLocationSub = (IDisposable) null;
      this.Map.DisposeAll();
      base.OnNavigatedFrom(e);
    }

    private void DetailsPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (string.IsNullOrEmpty(this.placeDetails.Url))
        return;
      AppState.ClientInstance.ShowWebTask(new Uri(this.placeDetails.Url));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/LocationView.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.Map = (MapControl) this.FindName("Map");
      this.CenterButton = (Grid) this.FindName("CenterButton");
      this.AccuracyEllipse = (Ellipse) this.FindName("AccuracyEllipse");
      this.Inner = (Ellipse) this.FindName("Inner");
      this.CartographicModeButton = (Image) this.FindName("CartographicModeButton");
      this.DetailsPanel = (StackPanel) this.FindName("DetailsPanel");
      this.PlaceName = (TextBlock) this.FindName("PlaceName");
      this.PlaceAddress = (TextBlock) this.FindName("PlaceAddress");
      this.HostName = (TextBlock) this.FindName("HostName");
    }

    private class InitState
    {
      public int TargetMessageId { get; set; }

      public GeoCoordinate TargetCoordinate { get; set; }

      public string TargetName { get; set; }

      public Message.PlaceDetails TargetPlaceDetails { get; set; }

      public bool DisableSelfLocating { get; set; }
    }
  }
}
