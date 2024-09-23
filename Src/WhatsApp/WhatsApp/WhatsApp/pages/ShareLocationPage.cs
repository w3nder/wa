// Decompiled with JetBrains decompiler
// Type: WhatsApp.ShareLocationPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Device.Location;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Windows.Devices.Geolocation;
using Windows.System;


namespace WhatsApp
{
  public class ShareLocationPage : PhoneApplicationPage
  {
    private const string LogHeader = "share loc";
    private ChatPage.InputMode inputMode_ = ChatPage.InputMode.None;
    private Storyboard fadeInSb_;
    private DoubleAnimation fadeInAnimation_;
    private IDisposable fadeInSbSub_;
    private TranslateTransform shiftedRootFrameTransformPortrait_ = new TranslateTransform();
    private IDisposable captionFocusChangedSub;
    private bool _pageLoaded;
    private MapPoint _myMapPoint;
    private MapPoint _myMapProfilePhoto;
    private bool _listCollapsed;
    private double _listCollapsedMapValue;
    private double _listCollapsedListValue;
    private double _listExpandedMapValue;
    private double _listExpandedListValue;
    private bool _LoadAdditionalPlacesPending;
    private MapPoint _mapSign;
    private MapPoint _mapSignNew;
    private bool _showingPlaceDetails;
    private List<MapPoint> _mapPlacesPins;
    private DispatcherTimer _timerDropDraggablePin;
    private object _lockDraggablePin = new object();
    private bool _droppedDraggablePin;
    private ObservableCollection<ShareLocationPage.HighlightedSearchResult> _searchResultsCollection = new ObservableCollection<ShareLocationPage.HighlightedSearchResult>();
    private Geolocator _geoLocator = new Geolocator();
    private IDisposable _locationSubscription;
    private GeoCoordinate _myLocation;
    private DispatcherTimer _locationTimeout = new DispatcherTimer();
    private IDisposable _placeSearchSubscription;
    private IDisposable _nameSearchSubscription;
    private IDisposable _mapZoomSubscription;
    private IObserver<Unit> _successfulShareObserver;
    private string _jid;
    private Message _quotedMsg;
    private string _quotedChat;
    private bool _c2cStarted;
    private bool _liveLocationOnly;
    private static ShareLocationPage.Parameters _params;
    private ShareLocationPage.Parameters currentParams;
    private bool ignoreEmojiKeyboardClosedOnce_;
    internal SolidColorBrush HandleRectangleBrush;
    internal Grid LayoutRoot;
    internal CompositeTransform MapXForm;
    internal MapControl MyMap;
    internal MapDraggablePin DraggablePin;
    internal CompositeTransform MapOverlayXForm;
    internal Grid CenterButton;
    internal Ellipse AccuracyEllipse;
    internal Ellipse Inner;
    internal Image CartographicModeButton;
    internal Grid ListContainer;
    internal CompositeTransform ListXForm;
    internal Grid Handle;
    internal Rectangle HandleRect1;
    internal Rectangle HandleRect2;
    internal StackPanel ShareLiveLocationPanel;
    internal RadioButton LiveLocationShare15RadioButton;
    internal RadioButton LiveLocationShare480RadioButton;
    internal Grid ShareLiveLocationCaption;
    internal Rectangle CaptionBackground;
    internal ScrollViewer CaptionPanel;
    internal RichTextBlock CaptionBlock;
    internal Button SubmitButton;
    internal Image SubmitButtonIcon;
    internal Grid HiddenRectangle;
    internal Rectangle ShareLiveLocationHiddenRectangle;
    internal Rectangle ShareLocationHiddenRectangle;
    internal WhatsApp.CompatibilityShims.LongListSelector PlacesList;
    internal Grid LiveLocationGrid;
    internal Image ShareLiveLocationButton;
    internal TextBlock ShareLiveLocationButtonText;
    internal TextBlock NearbyPlacesTitle;
    internal Grid LocationGrid;
    internal TextBlock ShareLocationAccuracyText;
    internal Image ShareLocationButton;
    internal AttributionControl Attribution;
    internal StackPanel StatusPanel;
    internal Grid SearchPanel;
    internal TextBox SearchBox;
    internal ListBox SearchResultsBox;
    internal EmojiTextBox CaptionBox;
    private bool _contentLoaded;

    private ChatPage.InputMode InputMode
    {
      get => this.inputMode_;
      set
      {
        if (this.inputMode_ == value && value != ChatPage.InputMode.None)
          return;
        this.OnInputModeChanged(this.inputMode_, this.inputMode_ = value);
      }
    }

    private Storyboard GetFadeInAnimation(double durationInMs = 250.0)
    {
      if (this.fadeInSb_ == null)
      {
        DoubleAnimation doubleAnimation = new DoubleAnimation();
        doubleAnimation.From = new double?(0.0);
        doubleAnimation.To = new double?(1.0);
        ExponentialEase exponentialEase = new ExponentialEase();
        exponentialEase.Exponent = 2.0;
        exponentialEase.EasingMode = EasingMode.EaseIn;
        doubleAnimation.EasingFunction = (IEasingFunction) exponentialEase;
        DoubleAnimation element = doubleAnimation;
        Storyboard.SetTargetProperty((Timeline) element, new PropertyPath("Opacity", new object[0]));
        this.fadeInSb_ = new Storyboard();
        this.fadeInSb_.Children.Add((Timeline) element);
        this.fadeInAnimation_ = element;
      }
      this.fadeInAnimation_.Duration = (Duration) TimeSpan.FromMilliseconds(durationInMs);
      return this.fadeInSb_;
    }

    private void UpdateRootFrameShifting(ChatPage.InputMode mode)
    {
      if (mode == ChatPage.InputMode.Emoji)
      {
        TranslateTransform transformPortrait = this.shiftedRootFrameTransformPortrait_;
        transformPortrait.Y = -UIUtils.SIPHeightPortrait;
        this.RenderTransform = (Transform) transformPortrait;
      }
      else
      {
        TranslateTransform transformPortrait = this.shiftedRootFrameTransformPortrait_;
        transformPortrait.Y = 0.0;
        this.RenderTransform = (Transform) transformPortrait;
      }
    }

    public ShareLocationPage()
    {
      this.InitializeComponent();
      this.Loaded += new RoutedEventHandler(this.ShareLocationPage_Loaded);
      this.currentParams = ShareLocationPage._params;
      ShareLocationPage._params = (ShareLocationPage.Parameters) null;
      this.PlacesList.OverlapScrollBar = true;
      this.PlacesList.ItemsSource = (IList) new ObservableCollection<PlaceSearchResult>();
      this.ShareLocationButton.Source = (System.Windows.Media.ImageSource) ImageStore.GetStockIcon("/Images/icon-currentlocation.png");
      if (Settings.LiveLocationEnabled)
      {
        this.HiddenRectangle.RowDefinitions.ElementAt<RowDefinition>(0).Height = new GridLength(1.0, GridUnitType.Star);
        this.ShareLiveLocationHiddenRectangle.Visibility = Visibility.Visible;
        this.ShareLiveLocationHiddenRectangle.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.ShareLiveLocation_Tap);
        this.LiveLocationGrid.Visibility = Visibility.Visible;
        this.ShareLiveLocationButton.Source = (System.Windows.Media.ImageSource) AssetStore.LiveLocationAccent;
        this.SubmitButtonIcon.Width = this.SubmitButtonIcon.Height = 36.0 * ResolutionHelper.ZoomMultiplier;
        this.SubmitButtonIcon.Source = (System.Windows.Media.ImageSource) AssetStore.InputBarSendIcon;
        this.SubmitButtonIcon.FlowDirection = App.CurrentApp.RootFrame.FlowDirection;
        this.CaptionBox.CounterForeground = (Brush) new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 153, (byte) 153, (byte) 153));
        this.CaptionBlock.Text = new RichTextBlock.TextSet()
        {
          Text = AppResources.AddACaption
        };
        this.CaptionBox.EmojiKeyboardOpening += new EventHandler(this.CaptionBox_EmojiKeyboardOpening);
        this.CaptionBox.EmojiKeyboardClosed += new EventHandler(this.CaptionBox_EmojiKeyboardClosed);
        this.captionFocusChangedSub = this.CaptionBox.TextBoxFocusChangedObservable().Subscribe<bool>(new Action<bool>(this.CaptionBox_FocusChanged));
        this.CaptionBox.KeyDown += (KeyEventHandler) ((sender, e) =>
        {
          if (e.Key != System.Windows.Input.Key.Enter)
            return;
          e.Handled = true;
          this.OnEnterKeyPressed();
        });
        this.Tap += (EventHandler<System.Windows.Input.GestureEventArgs>) ((sender, e) =>
        {
          if (this.InputMode != ChatPage.InputMode.Emoji)
            return;
          this.CaptionBox.CloseEmojiKeyboard();
        });
      }
      else
        this.ShareLiveLocationHiddenRectangle.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.ShareLocation_Tap);
      Microsoft.Phone.Shell.ApplicationBar resource = this.Resources[(object) "NonLocalizedAppBar"] as Microsoft.Phone.Shell.ApplicationBar;
      Localizable.LocalizeAppBar(resource);
      this.ApplicationBar = (IApplicationBar) resource;
    }

    private void ShareLocationPage_Loaded(object sender, RoutedEventArgs e)
    {
      this.Loaded -= new RoutedEventHandler(this.ShareLocationPage_Loaded);
      this._pageLoaded = true;
      GeoCoordinate geoCoordinate = new GeoCoordinate(Settings.LastKnownLatitude, Settings.LastKnownLongitude, 0.0, double.MaxValue, double.MaxValue, 0.0, 0.0);
      this.MyMap.Center = geoCoordinate;
      this._myLocation = geoCoordinate;
      this.UpdateMapPoints();
      this.MyMap.CartographicMode = Settings.MapCartographicModeRoad ? MapMode.Road : MapMode.Hybrid;
      this.MapXForm.TranslateY = -this.ListContainer.ActualHeight / 2.0;
      this.SearchResultsBox.ItemsSource = (IEnumerable) this._searchResultsCollection;
      this.HiddenRectangle.Height = ((FrameworkElement) this.PlacesList.ListHeader).ActualHeight + 12.0;
      ViewportControl logicalChildByType = TemplatedVisualTreeExtensions.GetFirstLogicalChildByType<ViewportControl>(this.PlacesList, false);
      if (logicalChildByType == null)
        return;
      logicalChildByType.ViewportChanged += new EventHandler<ViewportChangedEventArgs>(this.placesViewport_ViewportChanged);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (this.currentParams == null)
      {
        Log.l("share loc", "missing arguments to start sharelocationpage");
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
      }
      else
      {
        this._successfulShareObserver = this.currentParams.SuccessfulShareObserver;
        this._jid = this.currentParams.Jid;
        this._quotedMsg = this.currentParams.QuotedMsg;
        this._quotedChat = this.currentParams.QuotedChat;
        this._c2cStarted = this.currentParams.C2cStarted;
        this._liveLocationOnly = this.currentParams.LiveLocationOnly;
        this.BeginLocationSearch();
        if (this._liveLocationOnly)
          this.AnimateInShareLiveLocation();
        base.OnNavigatedTo(e);
      }
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
      if (this._locationSubscription != null)
        this._locationSubscription.SafeDispose();
      base.OnNavigatingFrom(e);
    }

    private void UpdateMapPoints()
    {
      if (this._myLocation == (GeoCoordinate) null)
        return;
      bool flag = this.ShareLiveLocationPanel.Visibility == Visibility.Visible;
      if (this._myMapPoint == null)
        this._myMapPoint = this.MyMap.AddPoint(MapPointStyle.MyLocation);
      this._myMapPoint.SetCoordinate(this._myLocation);
      this._myMapPoint.Element().Visibility = flag ? Visibility.Collapsed : Visibility.Visible;
      if (this._myMapProfilePhoto == null)
      {
        this._myMapProfilePhoto = this.MyMap.AddPoint(MapPointStyle.Live);
        this._myMapProfilePhoto.SetCoordinate(this._myLocation);
        LiveLocationPin liveLocationPin = this._myMapProfilePhoto.Element() as LiveLocationPin;
        System.Windows.Media.ImageSource cachedImgSrc = (System.Windows.Media.ImageSource) null;
        ChatPictureStore.GetCache(Settings.MyJid, out cachedImgSrc);
        liveLocationPin.PinIcons = new List<System.Windows.Media.ImageSource>()
        {
          cachedImgSrc ?? (System.Windows.Media.ImageSource) AssetStore.DefaultContactIconBlack
        };
      }
      else
        this._myMapProfilePhoto.SetCoordinate(this._myLocation);
      this._myMapProfilePhoto.Element().Visibility = flag ? Visibility.Visible : Visibility.Collapsed;
    }

    private void SetStatusPanel(DataTemplate content)
    {
      this.StatusPanel.Children.Clear();
      if (content == null)
        return;
      this.StatusPanel.Children.Add(content.LoadContent() as UIElement);
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
      this.PlacesList.IsHitTestVisible = false;
      this.ShareLocationHiddenRectangle.IsHitTestVisible = true;
      this.ShareLiveLocationHiddenRectangle.IsHitTestVisible = true;
      this.ShowPlacePins();
      if (this.PlacesList.Visibility == Visibility.Visible)
        this.PlacesList.ScrollTo(this.PlacesList.ListHeader);
      Storyboard resource = this.Resources[(object) "MoveList"] as Storyboard;
      DoubleAnimation child1 = resource.Children[0] as DoubleAnimation;
      DoubleAnimation child2 = resource.Children[1] as DoubleAnimation;
      double num1 = this.Handle.ActualHeight + ((FrameworkElement) this.PlacesList.ListHeader).ActualHeight + 12.0;
      double num2 = -num1 / 2.0;
      double num3 = this.ListContainer.ActualHeight - num1;
      child1.From = new double?(this.MapXForm.TranslateY);
      child1.To = new double?(num2);
      child2.From = new double?(this.ListXForm.TranslateY);
      child2.To = new double?(num3);
      this.MapOverlayXForm.TranslateY = -num1;
      Storyboarder.Perform(resource, false);
    }

    private void AnimateExpandList()
    {
      this.PlacesList.IsHitTestVisible = true;
      this.ShareLocationHiddenRectangle.IsHitTestVisible = false;
      this.ShareLiveLocationHiddenRectangle.IsHitTestVisible = false;
      this.HideDraggablePin();
      this.RemovePlacePins(true);
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
    }

    private void MyMap_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (!this.ListCollapsed && this.ShareLiveLocationPanel.Visibility == Visibility.Collapsed)
        this.ListCollapsed = true;
      else
        this.HidePlaceDetails();
    }

    private void ListHandle_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.ListCollapsed = !this.ListCollapsed;
    }

    private void ListContainer_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      double num = ((FrameworkElement) this.PlacesList.ListHeader).ActualHeight + this.Handle.ActualHeight + 12.0;
      this._listCollapsedMapValue = -num / 2.0;
      this._listCollapsedListValue = this.ListContainer.ActualHeight - num;
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
      {
        this.ListCollapsed = this.ListXForm.TranslateY - this._listExpandedListValue > (this._listCollapsedListValue - this._listExpandedListValue) * 2.0 / 3.0;
        if (!this.ListCollapsed)
          return;
        this.AnimateCollapseList();
      }
      else
      {
        this.ListCollapsed = this.ListXForm.TranslateY - this._listExpandedListValue > (this._listCollapsedListValue - this._listExpandedListValue) * 1.0 / 3.0;
        if (this.ListCollapsed)
          return;
        this.AnimateExpandList();
      }
    }

    private void CartographicMode_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.MyMap.CartographicMode == MapMode.Road)
      {
        this.MyMap.CartographicMode = MapMode.Hybrid;
        Settings.MapCartographicModeRoad = false;
      }
      else
      {
        this.MyMap.CartographicMode = MapMode.Road;
        Settings.MapCartographicModeRoad = true;
      }
    }

    private void CenterCanvas_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.ResetUI();
      this.MyMap.SetView(this._myLocation, 0.0, 0.0);
    }

    private void ClearPlaces()
    {
      this.Attribution.Clear();
      this.SetStatusPanel((DataTemplate) null);
      this.RemovePlacePins(false);
      this._mapPlacesPins = new List<MapPoint>();
      (this.PlacesList.ItemsSource as ObservableCollection<PlaceSearchResult>).Clear();
    }

    private void PopulatePlaces(
      GeoCoordinate center,
      double radius,
      string searchTerm,
      bool adjustView)
    {
      this.ClearPlaces();
      ObservableCollection<PlaceSearchResult> places = this.PlacesList.ItemsSource as ObservableCollection<PlaceSearchResult>;
      this.SetStatusPanel(this.PlacesList.Resources[(object) "PlaceSearchPanel"] as DataTemplate);
      this._placeSearchSubscription = WebServices.Instance.PlaceSearch(center.Latitude, center.Longitude, radius, searchTerm).ObserveOnDispatcher<PlaceSearchResult>().Do<PlaceSearchResult>((Action<PlaceSearchResult>) (place =>
      {
        place.Distance = this._myLocation.GetDistanceTo(new GeoCoordinate(place.Latitude, place.Longitude));
        places.Add(place);
      }), (Action<Exception>) (ex =>
      {
        Log.l(ex, "getting places");
        this.SetStatusPanel(this.PlacesList.Resources[(object) "NoPlacesFoundMessage"] as DataTemplate);
      })).Finally<PlaceSearchResult>((Action) (() =>
      {
        this.SetStatusPanel((DataTemplate) null);
        this.NearbyPlacesTitle.Text = !string.IsNullOrEmpty(searchTerm) ? string.Format(AppResources.PlaceSearchResultsTitle, (object) searchTerm) : AppResources.NearbyPlaces;
        if (places.Count > 0)
        {
          this.Attribution.Add(places[0].Attribution);
          if (!AppState.IsLowMemoryDevice)
          {
            PlaceSearchResult placeSearchResult = (PlaceSearchResult) null;
            foreach (PlaceSearchResult place in (IEnumerable<PlaceSearchResult>) places.OrderByDescending<PlaceSearchResult, double>((Func<PlaceSearchResult, double>) (place => place.Latitude)))
            {
              this.CreatePlacePin(place);
              if (placeSearchResult == null || place.Distance < placeSearchResult.Distance)
                placeSearchResult = place;
            }
            GeoCoordinate center1 = searchTerm != null ? this.MyMap.Center : this._myLocation;
            double distanceTo1 = center1.GetDistanceTo(new GeoCoordinate(center1.Latitude, placeSearchResult.Longitude));
            double distanceTo2 = center1.GetDistanceTo(new GeoCoordinate(placeSearchResult.Latitude, center1.Longitude));
            double num = this.MyMap.ShownAreaRadius();
            bool flag = this.ListCollapsed ? num > distanceTo1 * 0.8 && num > distanceTo2 * 0.8 : num > distanceTo1 * 0.8 && num > distanceTo2 * 0.4;
            if (adjustView && this.DraggablePin.Visibility != Visibility.Visible && !flag)
              this.MyMap.SetView(center1, Math.Abs(placeSearchResult.Longitude - center1.Longitude) * 2.0, Math.Abs(placeSearchResult.Latitude - center1.Latitude) * (this.ListCollapsed ? 2.0 : 5.0));
            if (this.ListCollapsed)
              this.Dispatcher.BeginInvoke((Action) (() => this.AnimateCollapseList()));
          }
        }
        else
          this.SetStatusPanel(this.PlacesList.Resources[(object) "NoPlacesFoundMessage"] as DataTemplate);
        this.RefreshButtonEnabled = true;
      })).Subscribe<PlaceSearchResult>();
    }

    private void LoadAdditionalPlaces()
    {
      if (this._placeSearchSubscription != null && this._LoadAdditionalPlacesPending)
        return;
      this._LoadAdditionalPlacesPending = true;
      ObservableCollection<PlaceSearchResult> places = this.PlacesList.ItemsSource as ObservableCollection<PlaceSearchResult>;
      this._placeSearchSubscription = WebServices.Instance.LoadAdditionalPlacesFromSearch().ObserveOnDispatcher<PlaceSearchResult>().Do<PlaceSearchResult>((Action<PlaceSearchResult>) (place =>
      {
        place.Distance = this._myLocation.GetDistanceTo(new GeoCoordinate(place.Latitude, place.Longitude));
        places.Add(place);
      }), (Action<Exception>) (ex => Log.l(ex, "getting more places"))).Finally<PlaceSearchResult>((Action) (() =>
      {
        if (places.Count > 0 && !AppState.IsLowMemoryDevice)
        {
          this.RemovePlacePins(false);
          foreach (PlaceSearchResult place in (IEnumerable<PlaceSearchResult>) places.OrderByDescending<PlaceSearchResult, double>((Func<PlaceSearchResult, double>) (place => place.Latitude)))
            this.CreatePlacePin(place);
        }
        this._LoadAdditionalPlacesPending = false;
      })).Subscribe<PlaceSearchResult>((Action<PlaceSearchResult>) (_ => { }), (Action<Exception>) (ex => { }));
    }

    private void MapPlacePin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      e.Handled = true;
      this.ListCollapsed = true;
      if (sender is MapPlacePin pin)
        this.ShowPlaceDetails(pin);
      this.HideDraggablePin();
    }

    private void HidePlaceDetails()
    {
      if (!this._showingPlaceDetails || this._mapSign == null)
        return;
      MapSign mapSign = this._mapSign.Element() as MapSign;
      mapSign.Hide((Action) null);
      ((MapPlacePin) mapSign.Tag).Expand();
      this._showingPlaceDetails = false;
    }

    private void ShowPlaceDetails(MapPlacePin pin)
    {
      this.HidePlaceDetails();
      PlaceSearchResult tag = pin.Tag as PlaceSearchResult;
      GeoCoordinate loc = new GeoCoordinate(tag.Latitude, tag.Longitude);
      if (this._mapSignNew == null)
      {
        this._mapSignNew = this.MyMap.AddPoint(MapPointStyle.Sign);
        this._mapSignNew.SetCoordinate(loc);
        this._mapSignNew.Element().Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.PlaceDetail_Tap);
      }
      this._mapSignNew.SetCoordinate(loc);
      MapSign mapSign1 = this._mapSignNew.Element() as MapSign;
      mapSign1.Tag = (object) pin;
      mapSign1.Title = tag.Name;
      mapSign1.SubTitle = tag.ShortText;
      mapSign1.Show();
      pin.Collapse();
      this.MyMap.Center = loc;
      MapPoint mapSign2 = this._mapSign;
      this._mapSign = this._mapSignNew;
      this._mapSignNew = mapSign2;
      this._showingPlaceDetails = true;
    }

    private void RefreshPlaces_Click(object sender, EventArgs e)
    {
      this.RefreshButtonEnabled = false;
      this.ResetUI();
      this.PopulatePlaces(this.MyMap.Center, this.MyMap.ShownAreaRadius(), (string) null, false);
    }

    private bool RefreshButtonEnabled
    {
      set
      {
        if (this.ApplicationBar == null)
          return;
        IList buttons = this.ApplicationBar.Buttons;
        for (int index = 0; index < buttons.Count; ++index)
        {
          if (buttons[index] is ApplicationBarIconButton applicationBarIconButton && applicationBarIconButton.Text == AppResources.RefreshPlaces)
          {
            applicationBarIconButton.IsEnabled = value;
            break;
          }
        }
      }
    }

    private void CreatePlacePin(PlaceSearchResult place)
    {
      MapPoint mapPoint = this.MyMap.AddPoint(MapPointStyle.Place);
      mapPoint.SetCoordinate(new GeoCoordinate()
      {
        Longitude = place.Longitude,
        Latitude = place.Latitude
      });
      MapPlacePin mapPlacePin = mapPoint.Element() as MapPlacePin;
      mapPlacePin.Graphic.Source = (System.Windows.Media.ImageSource) ImageStore.GetStockIcon(place.LocalIcon.Replace("Light", "Dark"));
      mapPlacePin.Tag = (object) place;
      if (mapPoint.Element() != null)
        mapPoint.Element().Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.MapPlacePin_Tap);
      this._mapPlacesPins.Add(mapPoint);
    }

    private void RemovePlacePins(bool hide)
    {
      if (this._mapPlacesPins != null)
      {
        foreach (MapPoint mapPlacesPin in this._mapPlacesPins)
        {
          if (mapPlacesPin.Element() is MapPlacePin mapPlacePin)
          {
            Action a = hide ? (Action) null : new Action(mapPlacesPin.Remove);
            mapPlacePin.Collapse(a);
          }
          else if (!hide)
            mapPlacesPin.Remove();
        }
        if (!hide)
          this._mapPlacesPins.Clear();
      }
      MapPoint[] mapPointArray = new MapPoint[2]
      {
        this._mapSign,
        this._mapSignNew
      };
      foreach (MapPoint mapPoint in mapPointArray)
      {
        if (mapPoint != null)
        {
          if (mapPoint.Element() is MapSign mapSign)
            mapSign.Hide(new Action(mapPoint.Remove));
          else
            mapPoint.Remove();
        }
      }
    }

    private void ShowPlacePins()
    {
      if (this._mapPlacesPins == null)
        return;
      foreach (MapPoint mapPlacesPin in this._mapPlacesPins)
      {
        if (mapPlacesPin.Element() is MapPlacePin mapPlacePin)
          mapPlacePin.Expand();
      }
    }

    private void ResetUI()
    {
      this.HideSearchPanel();
      this.HidePlaceDetails();
    }

    private void DragPinButton_Click(object sender, EventArgs e)
    {
      if (this.DraggablePin.Visibility == Visibility.Collapsed)
      {
        this.ListCollapsed = true;
        this.ResetUI();
        this.ShowDraggablePin();
      }
      else
        this.HideDraggablePin();
    }

    private void ShowDraggablePin()
    {
      if (this._timerDropDraggablePin == null)
      {
        this._timerDropDraggablePin = new DispatcherTimer();
        this._timerDropDraggablePin.Interval = TimeSpan.FromMilliseconds(400.0);
        this._timerDropDraggablePin.Tick += new EventHandler(this.AttachDraggablePin);
        this.MyMap.ViewChanged().Subscribe<EventArgs>((Action<EventArgs>) (eventArgs =>
        {
          lock (this._lockDraggablePin)
          {
            if (this._droppedDraggablePin)
            {
              this.DraggablePin.Tap -= new EventHandler<System.Windows.Input.GestureEventArgs>(this.DraggablePin_Tap);
              this.DraggablePin.DetachPin();
              this._timerDropDraggablePin.Start();
              this._droppedDraggablePin = false;
            }
            else
            {
              this._timerDropDraggablePin.Stop();
              this._timerDropDraggablePin.Start();
            }
          }
        }));
      }
      if (this.DraggablePin.Visibility == Visibility.Visible)
        return;
      this._timerDropDraggablePin.Start();
      this.DraggablePin.Visibility = Visibility.Visible;
      this.DraggablePin.Tap -= new EventHandler<System.Windows.Input.GestureEventArgs>(this.DraggablePin_Tap);
    }

    private void HideDraggablePin()
    {
      if (this.DraggablePin.Visibility == Visibility.Collapsed)
        return;
      this._timerDropDraggablePin.Stop();
      this.DraggablePin.Tap -= new EventHandler<System.Windows.Input.GestureEventArgs>(this.DraggablePin_Tap);
      this.DraggablePin.Visibility = Visibility.Collapsed;
      this.DraggablePin.Reset();
      this._droppedDraggablePin = false;
    }

    private void AttachDraggablePin(object sender, EventArgs e)
    {
      lock (this._lockDraggablePin)
      {
        this._timerDropDraggablePin.Stop();
        this.DraggablePin.AttachPin();
        this.DraggablePin.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.DraggablePin_Tap);
        this.DraggablePin.Subtitle.Text = AppResources.SearchingAddress;
        WebServices.Instance.ReverseGeocode(this.MyMap.Center.Latitude, this.MyMap.Center.Longitude).ObserveOnDispatcher<PlaceSearchResult>().Take<PlaceSearchResult>(1).Do<PlaceSearchResult>((Action<PlaceSearchResult>) (place => this.DraggablePin.Subtitle.Text = place.Address), (Action<Exception>) (ex =>
        {
          Log.l(ex, "reverse geocoding");
          this.DraggablePin.Subtitle.Text = "";
        }), (Action) (() =>
        {
          if (!(this.DraggablePin.Subtitle.Text == AppResources.SearchingAddress))
            return;
          this.DraggablePin.Subtitle.Text = "";
        })).Subscribe<PlaceSearchResult>();
        this._droppedDraggablePin = true;
      }
    }

    private void Search_Click(object sender, EventArgs e)
    {
      if (this.SearchPanel.Visibility == Visibility.Collapsed)
      {
        this.ResetUI();
        this.ListCollapsed = true;
        this.HideDraggablePin();
        this.SearchPanel.Visibility = Visibility.Visible;
        this.SearchBox.Focus();
        this.SearchBox.Select(this.SearchBox.Text.Length, 0);
        this.SearchBox.LostFocus += new RoutedEventHandler(this.SearchBox_LostFocus);
      }
      else
        this.HideSearchPanel();
    }

    private void SearchBox_LostFocus(object sender, RoutedEventArgs e) => this.HideSearchPanel();

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (this.SearchBox.Text.Length > 2)
      {
        if (this._nameSearchSubscription != null)
          this._nameSearchSubscription.Dispose();
        this._nameSearchSubscription = this.AutoCompleteSearch(this.MyMap.Center, Math.Max(this.MyMap.ShownAreaRadius(), 1500.0), this.SearchBox.Text).Subscribe<PlaceSearchResult>();
      }
      else if (this._searchResultsCollection.Count > 0)
        this._searchResultsCollection.Clear();
      this.SearchResultsBox.Visibility = Visibility.Collapsed;
    }

    private void SearchBox_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key != System.Windows.Input.Key.Enter)
        return;
      string searchTerm = this.SearchBox.Text.Trim();
      if (searchTerm.Length <= 2)
        return;
      this.HideSearchPanel();
      this.PopulatePlaces(this.MyMap.Center, Math.Max(this.MyMap.ShownAreaRadius(), 1500.0), searchTerm, true);
      this.ListCollapsed = false;
    }

    private void HideSearchPanel()
    {
      this.SearchPanel.Visibility = Visibility.Collapsed;
      this.SearchBox.Text = string.Empty;
      this.SearchBox.LostFocus -= new RoutedEventHandler(this.SearchBox_LostFocus);
    }

    private IObservable<PlaceSearchResult> AutoCompleteSearch(
      GeoCoordinate center,
      double accuracy,
      string name)
    {
      this._searchResultsCollection.Clear();
      return WebServices.Instance.AutoCompleteSearch(center.Latitude, center.Longitude, accuracy, name).ObserveOnDispatcher<PlaceSearchResult>().Do<PlaceSearchResult>((Action<PlaceSearchResult>) (place =>
      {
        if (!this._searchResultsCollection.Any<ShareLocationPage.HighlightedSearchResult>((Func<ShareLocationPage.HighlightedSearchResult, bool>) (p => place.Name.Trim().ToLower() == p.Place.Name.Trim().ToLower())))
          this._searchResultsCollection.Add(new ShareLocationPage.HighlightedSearchResult(place, name));
        int count = this._searchResultsCollection.Count;
      }), (Action<Exception>) (ex => Log.l(ex, "searching places"))).Finally<PlaceSearchResult>((Action) (() => this.SearchResultsBox.Visibility = this._searchResultsCollection.Count > 0 ? Visibility.Visible : Visibility.Collapsed));
    }

    private void SearchResultsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      this.ResetUI();
      if (e.AddedItems.Count <= 0)
        return;
      ShareLocationPage.HighlightedSearchResult addedItem = e.AddedItems[0] as ShareLocationPage.HighlightedSearchResult;
      this.PopulatePlaces(this.MyMap.Center, Math.Max(this.MyMap.ShownAreaRadius(), 1500.0), addedItem.Place.Name.Trim(), true);
      this.ListCollapsed = false;
    }

    private void PreBeginLocationSearch()
    {
      this.SetStatusPanel(this.PlacesList.Resources[(object) "LocatingPanel"] as DataTemplate);
      this._locationTimeout.Interval = TimeSpan.FromSeconds(10.0);
      this._locationTimeout.Tick += new EventHandler(this.LocationTimeout_Tick);
      this._locationTimeout.Start();
    }

    private async void BeginLocationSearch()
    {
      this.PreBeginLocationSearch();
      try
      {
        this.SetMyLocation(await LocationHelper.GetInaccurateGeoCoordinate());
        this._locationSubscription = LocationHelper.ObserveAccurateGeoCoordinate().ObserveOnDispatcher<GeoCoordinate>().Subscribe<GeoCoordinate>(new Action<GeoCoordinate>(this.OnAccurateGeoCoordinate), (Action<Exception>) (ex =>
        {
          bool locationServiceOff = ex.HResult == -2147467260 || ex is UnauthorizedAccessException;
          this.ShowError(locationServiceOff);
          if (!locationServiceOff)
            Log.l(ex, "LocationSharing, GetLocation");
          this._locationTimeout.Stop();
        }));
      }
      catch (UnauthorizedAccessException ex)
      {
        this.ShowError(true);
        this._locationTimeout.Stop();
      }
      catch (Exception ex)
      {
        bool locationServiceOff = ex.HResult == -2147467260;
        this.ShowError(locationServiceOff);
        if (!locationServiceOff)
          Log.l(ex, "LocationSharing, GetLocation");
        this._locationTimeout.Stop();
      }
    }

    private void LocationTimeout_Tick(object sender, EventArgs e)
    {
      this._locationTimeout.Stop();
      Log.p("share loc", "location look up timed out after 10 seconds");
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        if (this._locationSubscription != null)
          this._locationSubscription.SafeDispose();
        this.PopulatePlaces(this._myLocation, Math.Max(this._myLocation.HorizontalAccuracy, 100.0), (string) null, true);
      }));
    }

    private void OnAccurateGeoCoordinate(GeoCoordinate gcoord)
    {
      this.SetMyLocation(gcoord);
      if (gcoord.HorizontalAccuracy >= 200.0)
        return;
      this._locationTimeout.Stop();
      if (this._locationSubscription != null)
        this._locationSubscription.SafeDispose();
      this.PopulatePlaces(this._myLocation, Math.Max(gcoord.HorizontalAccuracy, 100.0), (string) null, true);
    }

    private void SetMyLocation(GeoCoordinate gpos)
    {
      if (!(this._myLocation == (GeoCoordinate) null) && this._myLocation.HorizontalAccuracy <= gpos.HorizontalAccuracy)
        return;
      this._myLocation = gpos;
      Settings.LastKnownLatitude = gpos.Latitude;
      Settings.LastKnownLongitude = gpos.Longitude;
      string str = new DistanceToStringConverter().Convert((object) gpos.HorizontalAccuracy, typeof (double), (object) null, (CultureInfo) null) as string;
      if (!string.IsNullOrEmpty(str))
      {
        this.ShareLocationAccuracyText.Visibility = Visibility.Visible;
        this.ShareLocationAccuracyText.Text = string.Format("{0} {1}", (object) AppResources.AccurateTo, (object) str);
      }
      if (this._myMapPoint == null)
        this._myMapPoint = this.MyMap.AddPoint(MapPointStyle.MyLocation);
      this._myMapPoint.SetCoordinate(this._myLocation);
      MapMyLocation me = this._myMapPoint.Element() as MapMyLocation;
      me.IsAccurate = true;
      double num = gpos.HorizontalAccuracy / (Math.Cos(gpos.Latitude * Math.PI / 180.0) * 2.0 * Math.PI * 6378137.0 / (256.0 * Math.Pow(2.0, this.MyMap.ZoomLevel)));
      me.AccuracyRadius = num;
      this._mapZoomSubscription = this.MyMap.ViewChanged().Subscribe<EventArgs>((Action<EventArgs>) (eventArgs =>
      {
        if (!(eventArgs is MapZoomLevelChangedEventArgs))
          return;
        me.AccuracyRadius = 0.0;
      }));
      if (this.DraggablePin.Visibility == Visibility.Visible)
        return;
      this.MyMap.Center = this._myLocation;
    }

    private void ShowError(bool locationServiceOff)
    {
      UIUtils.MessageBox(" ", locationServiceOff ? AppResources.LocationServicesOff : AppResources.LocationFailure, (IEnumerable<string>) new string[1]
      {
        AppResources.LocationServicesLink
      }, (Action<int>) (async idx =>
      {
        if (idx == 0)
        {
          int num = await Launcher.LaunchUriAsync(new Uri("ms-settings-location:")) ? 1 : 0;
        }
        else
          this.NavigationService.JumpBack();
      }));
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.MyMap.DisposeAll();
      this.DisposeAll();
      base.OnRemovedFromJournal(e);
    }

    private void DisposeAll()
    {
      if (this._placeSearchSubscription != null)
        this._placeSearchSubscription.Dispose();
      if (this._nameSearchSubscription != null)
        this._nameSearchSubscription.Dispose();
      if (this._mapZoomSubscription == null)
        return;
      this._mapZoomSubscription.Dispose();
    }

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      if (this._pageLoaded)
      {
        if (this.CaptionBox.IsEmojiKeyboardOpen)
          this.InputMode = ChatPage.InputMode.None;
        else if (this.ShareLiveLocationPanel.Visibility == Visibility.Visible)
        {
          if (!this._liveLocationOnly)
          {
            e.Cancel = true;
            this.AnimateOutShareLiveLocation();
          }
        }
        else if (!this.ListCollapsed)
        {
          e.Cancel = true;
          this.ListCollapsed = true;
        }
        else if (this._showingPlaceDetails)
        {
          e.Cancel = true;
          this.HidePlaceDetails();
        }
        else if (this.SearchPanel.Visibility == Visibility.Visible)
        {
          e.Cancel = true;
          this.HideSearchPanel();
        }
        else if (this.PlacesList.ItemsSource != null && this.PlacesList.ItemsSource.Count > 0 && this.NearbyPlacesTitle.Text != AppResources.NearbyPlaces)
        {
          e.Cancel = true;
          this.ClearPlaces();
          this.Dispatcher.BeginInvoke((Action) (() => this.ListCollapsed = true));
        }
      }
      base.OnBackKeyPress(e);
    }

    public static IObservable<Unit> StartForLiveLocationOnly(
      NavigationService navService,
      string jid)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        ShareLocationPage._params = new ShareLocationPage.Parameters()
        {
          Jid = jid,
          LiveLocationOnly = true,
          SuccessfulShareObserver = observer
        };
        navService.Navigate(UriUtils.CreatePageUri(nameof (ShareLocationPage)));
        return (Action) (() => { });
      }));
    }

    public static IObservable<Unit> Start(
      NavigationService navService,
      string jid,
      Message quotedMsg,
      string quotedChat,
      bool c2cStarted)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        ShareLocationPage._params = new ShareLocationPage.Parameters()
        {
          Jid = jid,
          QuotedMsg = quotedMsg,
          QuotedChat = quotedChat,
          C2cStarted = c2cStarted,
          LiveLocationOnly = false,
          SuccessfulShareObserver = observer
        };
        navService.Navigate(UriUtils.CreatePageUri(nameof (ShareLocationPage)));
        return (Action) (() => { });
      }));
    }

    private void ShareLocation_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      GeoCoordinate location = (GeoCoordinate) null;
      if (this._myMapPoint != null && ((MapMyLocation) this._myMapPoint.Element()).IsAccurate)
        location = this._myLocation;
      this.ShareLocation(location);
    }

    private void DraggablePin_Tap(object sender, EventArgs e)
    {
      this.ShareLocation(this.MyMap.Center);
    }

    private void PlacesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      Microsoft.Phone.Controls.LongListSelector longListSelector = sender as Microsoft.Phone.Controls.LongListSelector;
      if (longListSelector.SelectedItem == null)
        return;
      this.SharePlace(longListSelector.SelectedItem as PlaceSearchResult);
    }

    private void placesViewport_ViewportChanged(object sender, ViewportChangedEventArgs e)
    {
      ObservableCollection<PlaceSearchResult> itemsSource = this.PlacesList.ItemsSource as ObservableCollection<PlaceSearchResult>;
      ViewportControl viewportControl = sender as ViewportControl;
      if (itemsSource.Count <= 0)
        return;
      Rect rect = viewportControl.Viewport;
      double bottom = rect.Bottom;
      rect = viewportControl.Bounds;
      double num = rect.Bottom - 50.0;
      if (bottom <= num)
        return;
      this.LoadAdditionalPlaces();
    }

    private void PlaceDetail_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.SharePlace(((sender as MapSign).Tag as MapPlacePin).Tag as PlaceSearchResult);
    }

    private void ShareLocation(GeoCoordinate location)
    {
      if (location != (GeoCoordinate) null)
        this.ShareLocationData(new LocationData()
        {
          Latitude = new double?(location.Latitude),
          Longitude = new double?(location.Longitude)
        }, false);
      else
        Log.d("share loc", "trying to share an empty geocoordinate");
    }

    private void ShareLiveLocation(float durationInSeconds, string caption)
    {
      this.ShareLocationData(new LocationData()
      {
        Caption = caption,
        DurationInSeconds = new float?(durationInSeconds)
      }, true);
    }

    private void SharePlace(PlaceSearchResult res)
    {
      if (res != null)
      {
        string str = string.IsNullOrEmpty(res.Name) ? (string.IsNullOrEmpty(res.Address) ? (string) null : res.Address) : res.Name + (string.IsNullOrEmpty(res.Address) ? "" : "\n" + res.Address);
        this.ShareLocationData(new LocationData()
        {
          Latitude = new double?(res.Latitude),
          Longitude = new double?(res.Longitude),
          PlaceDetails = str,
          PlaceUrl = res.Url
        }, false);
      }
      else
        Log.d("share loc", "trying to share an empty place");
    }

    private void ShareLocationData(LocationData loc, bool isLive)
    {
      if (!this.IsEnabled)
        return;
      this.IsEnabled = false;
      Message msg = isLive ? this.CreateLiveLocationMessage(loc) : this.CreateLocationMessage(loc);
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        msg.SetQuote(this._quotedMsg, this._quotedChat);
        msg.SetC2cFlags(this._c2cStarted);
        db.InsertMessageOnSubmit(msg);
        db.SubmitChanges();
      }));
      if (this._successfulShareObserver != null)
        this._successfulShareObserver.OnNext(new Unit());
      this.NavigationService.JumpBackTo("ChatPage", fallbackToHome: true);
    }

    private Message CreateLiveLocationMessage(LocationData locData)
    {
      int num = (int) locData.DurationInSeconds.Value;
      Message locationMessage = this.CreateLocationMessage(locData);
      locationMessage.MediaWaType = FunXMPP.FMessage.Type.LiveLocation;
      locationMessage.MediaDurationSeconds = num;
      locationMessage.MediaSize = LiveLocationManager.Instance.getNextSequenceNumber();
      MessageProperties forMessage = MessageProperties.GetForMessage(locationMessage);
      MessageProperties.LiveLocationProperties locationProperties = forMessage.EnsureLiveLocationProperties;
      locationProperties.AccuracyInMeters = locData.AccuracyInMeters;
      locationProperties.Caption = locData.Caption;
      locationProperties.DegreesClockwiseFromMagneticNorth = locData.DegreesClockwiseFromMagneticNorth;
      locationProperties.SpeedInMps = locData.SpeedInMps;
      forMessage.Save();
      return locationMessage;
    }

    private Message CreateLocationMessage(LocationData locData)
    {
      Message locationMessage = new Message(true);
      locationMessage.KeyFromMe = true;
      locationMessage.KeyRemoteJid = this._jid;
      locationMessage.KeyId = FunXMPP.GenerateMessageId();
      locationMessage.Status = FunXMPP.FMessage.Status.Pending;
      locationMessage.MediaWaType = FunXMPP.FMessage.Type.Location;
      locationMessage.Latitude = locData.Latitude.HasValue ? locData.Latitude.Value : double.MinValue;
      double? longitude = locData.Longitude;
      double minValue;
      if (!longitude.HasValue)
      {
        minValue = double.MinValue;
      }
      else
      {
        longitude = locData.Longitude;
        minValue = longitude.Value;
      }
      locationMessage.Longitude = minValue;
      locationMessage.LocationDetails = locData.PlaceDetails;
      locationMessage.LocationUrl = locData.PlaceUrl;
      return locationMessage;
    }

    private void ShareLiveLocation_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (!Settings.LiveLocationEnabled)
        return;
      if (Settings.LiveLocationIsNewUser)
        this.ShowLiveLocationNewUserDialog();
      else
        this.AnimateInShareLiveLocation();
    }

    private void AnimateInShareLiveLocation()
    {
      this.ApplicationBar.IsVisible = false;
      this.Handle.Visibility = Visibility.Collapsed;
      this.PlacesList.Visibility = Visibility.Collapsed;
      this.ShareLiveLocationPanel.Visibility = Visibility.Visible;
      this.Handle.Tap -= new EventHandler<System.Windows.Input.GestureEventArgs>(this.ListHandle_Tap);
      this.ListContainer.ManipulationStarted -= new EventHandler<ManipulationStartedEventArgs>(this.ListContainer_ManipulationStarted);
      this.ListContainer.ManipulationDelta -= new EventHandler<ManipulationDeltaEventArgs>(this.ListContainer_ManipulationDelta);
      this.ListContainer.ManipulationCompleted -= new EventHandler<ManipulationCompletedEventArgs>(this.ListContainer_ManipulationCompleted);
      this.RemovePlacePins(true);
      this.UpdateMapPoints();
      this.ListContainer.SizeChanged += new SizeChangedEventHandler(this.ListContainer_SizeChanged);
    }

    private void ListContainer_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.AdjustListHeightForLiveLocation();
    }

    private void AdjustListHeightForLiveLocation()
    {
      Storyboard resource = this.Resources[(object) "MoveList"] as Storyboard;
      DoubleAnimation child1 = resource.Children[0] as DoubleAnimation;
      DoubleAnimation child2 = resource.Children[1] as DoubleAnimation;
      double actualHeight = this.ShareLiveLocationPanel.ActualHeight;
      double num1 = -actualHeight / 2.0;
      double num2 = this.ListContainer.ActualHeight - actualHeight;
      child1.From = new double?(this.MapXForm.TranslateY);
      child1.To = new double?(num1);
      child2.From = new double?(this.ListXForm.TranslateY);
      child2.To = new double?(num2);
      Storyboarder.Perform(resource, false);
    }

    private void AnimateOutShareLiveLocation()
    {
      this.ListContainer.SizeChanged -= new SizeChangedEventHandler(this.ListContainer_SizeChanged);
      this.ApplicationBar.IsVisible = true;
      this.Handle.Visibility = Visibility.Visible;
      this.PlacesList.Visibility = Visibility.Visible;
      this.ShareLiveLocationPanel.Visibility = Visibility.Collapsed;
      this.Handle.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.ListHandle_Tap);
      this.ListContainer.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.ListContainer_ManipulationStarted);
      this.ListContainer.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(this.ListContainer_ManipulationDelta);
      this.ListContainer.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(this.ListContainer_ManipulationCompleted);
      this.UpdateMapPoints();
      this.AnimateExpandList();
      FieldStats.ReportLiveLocationDurationPicker(0L);
    }

    private void Submit_Click(object sender, RoutedEventArgs e)
    {
      int durationInSeconds = 3600;
      bool? isChecked1 = this.LiveLocationShare15RadioButton.IsChecked;
      bool flag1 = true;
      if ((isChecked1.GetValueOrDefault() == flag1 ? (isChecked1.HasValue ? 1 : 0) : 0) != 0)
      {
        durationInSeconds = 900;
      }
      else
      {
        bool? isChecked2 = this.LiveLocationShare480RadioButton.IsChecked;
        bool flag2 = true;
        if ((isChecked2.GetValueOrDefault() == flag2 ? (isChecked2.HasValue ? 1 : 0) : 0) != 0)
          durationInSeconds = 28800;
      }
      string text = this.CaptionBox.Text;
      this.ShareLiveLocation((float) durationInSeconds, text);
      FieldStats.ReportLiveLocationDurationPicker((long) durationInSeconds);
    }

    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
      TimeSpentManager.GetInstance().UserAction();
    }

    private void CaptionPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.CaptionBox.Opacity = 0.0;
      this.CaptionBox.Visibility = Visibility.Visible;
      this.CaptionBox.MaxLength = 1024;
      this.fadeInSbSub_.SafeDispose();
      this.fadeInSbSub_ = Storyboarder.PerformWithDisposable(this.GetFadeInAnimation(350.0), (DependencyObject) this.CaptionBox, true, (Action) (() =>
      {
        if (this.InputMode == ChatPage.InputMode.None)
          return;
        this.CaptionBox.Opacity = 1.0;
        int length = this.CaptionBox.Text == null ? 0 : this.CaptionBox.Text.Length;
        this.CaptionBox.TextBox.SelectionLength = 0;
        this.CaptionBox.TextBox.SelectionStart = length;
      }), false, "pic preview: fade in caption box");
      this.CaptionBox.OpenTextKeyboard();
    }

    private void CaptionBox_EmojiKeyboardOpening(object sender, EventArgs e)
    {
      Log.p("share loc", "emoji keyboard opening");
      this.InputMode = ChatPage.InputMode.Emoji;
    }

    private void CaptionBox_EmojiKeyboardClosed(object sender, EventArgs e)
    {
      if (this.ignoreEmojiKeyboardClosedOnce_)
      {
        this.ignoreEmojiKeyboardClosedOnce_ = false;
      }
      else
      {
        Log.p("share loc", "emoji keyboard closed");
        if (this.InputMode != ChatPage.InputMode.Emoji)
          return;
        this.InputMode = ChatPage.InputMode.None;
      }
    }

    private void CaptionBox_FocusChanged(bool focused)
    {
      Log.p("share loc", "caption box focus changed: {0}", (object) focused);
      if (focused)
      {
        if (this.InputMode == ChatPage.InputMode.Emoji)
          this.ignoreEmojiKeyboardClosedOnce_ = true;
        this.InputMode = ChatPage.InputMode.Keyboard;
      }
      else
        this.InputMode = ChatPage.InputMode.None;
    }

    private void OnInputModeChanged(ChatPage.InputMode oldMode, ChatPage.InputMode newMode)
    {
      Log.p("share loc", "input mode {0} -> {1}", (object) oldMode, (object) newMode);
      if (newMode != ChatPage.InputMode.Keyboard)
        this.Focus();
      else if (newMode != ChatPage.InputMode.Emoji)
        this.CaptionBox.CloseEmojiKeyboard();
      this.UpdateRootFrameShifting(newMode);
      if (newMode == ChatPage.InputMode.None)
      {
        string str = this.CaptionBox.Text == null ? (string) null : Emoji.ConvertToRichText(this.CaptionBox.Text).Trim();
        if (string.IsNullOrEmpty(str))
        {
          this.CaptionBlock.Text = new RichTextBlock.TextSet()
          {
            Text = AppResources.AddACaption
          };
          this.CaptionBox.Text = "";
        }
        else
        {
          this.CaptionBox.Text = str;
          this.CaptionBlock.Text = new RichTextBlock.TextSet()
          {
            Text = str
          };
          this.CaptionBox.TextBox.SelectionLength = 0;
          this.CaptionBox.TextBox.SelectionStart = str.Length;
        }
        this.CaptionBox.Visibility = Visibility.Collapsed;
        this.ShareLiveLocationCaption.Opacity = 0.0;
        this.ShareLiveLocationCaption.Visibility = Visibility.Visible;
        this.fadeInSbSub_.SafeDispose();
        this.fadeInSbSub_ = Storyboarder.PerformWithDisposable(this.GetFadeInAnimation(), (DependencyObject) this.ShareLiveLocationCaption, true, (Action) (() =>
        {
          if (this.InputMode != ChatPage.InputMode.None)
            return;
          this.ShareLiveLocationCaption.Opacity = 1.0;
        }), false, "pic preview: fade in bottom panel");
      }
      else
      {
        this.ShareLiveLocationCaption.Visibility = Visibility.Collapsed;
        this.ShareLiveLocationCaption.Opacity = 0.0;
        this.CaptionBox.Visibility = Visibility.Visible;
      }
    }

    private void OnEnterKeyPressed()
    {
      Log.p("share loc", "enter key pressed");
      this.Focus();
    }

    private void ShowLiveLocationNewUserDialog()
    {
      Hyperlink hyperlink1 = new Hyperlink();
      hyperlink1.Foreground = (Brush) UIUtils.AccentBrush;
      hyperlink1.TextDecorations = (TextDecorationCollection) null;
      hyperlink1.Command = (ICommand) new ActionCommand((Action) (() => new WebBrowserTask()
      {
        Uri = new Uri(WaWebUrls.FaqlUrlLiveLocation)
      }.Show()));
      Hyperlink hyperlink2 = hyperlink1;
      hyperlink2.Inlines.Add(AppResources.LearnMoreText);
      Paragraph body = new Paragraph();
      body.Inlines.Add(AppResources.LiveLocationFirstUseDialogDescription + "\n");
      body.Inlines.Add((Inline) hyperlink2);
      MessageBoxControl.Show(ImageStore.GetStockIcon("/Images/nux_live_location.png"), (string) null, body, (IEnumerable<string>) new string[2]
      {
        AppResources.Cancel,
        AppResources.Continue
      }, (Action<int>) (selectedButtonIndex =>
      {
        if (selectedButtonIndex != 1)
          return;
        Settings.LiveLocationIsNewUser = false;
        this.AnimateInShareLiveLocation();
      }));
    }

    private void Handle_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      this.Handle.Background = this.Resources[(object) "PhoneAccentBrush"] as Brush;
      this.HandleRect1.Fill = (Brush) this.HandleRectangleBrush;
      this.HandleRect2.Fill = (Brush) this.HandleRectangleBrush;
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
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/ShareLocationPage.xaml", UriKind.Relative));
      this.HandleRectangleBrush = (SolidColorBrush) this.FindName("HandleRectangleBrush");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.MapXForm = (CompositeTransform) this.FindName("MapXForm");
      this.MyMap = (MapControl) this.FindName("MyMap");
      this.DraggablePin = (MapDraggablePin) this.FindName("DraggablePin");
      this.MapOverlayXForm = (CompositeTransform) this.FindName("MapOverlayXForm");
      this.CenterButton = (Grid) this.FindName("CenterButton");
      this.AccuracyEllipse = (Ellipse) this.FindName("AccuracyEllipse");
      this.Inner = (Ellipse) this.FindName("Inner");
      this.CartographicModeButton = (Image) this.FindName("CartographicModeButton");
      this.ListContainer = (Grid) this.FindName("ListContainer");
      this.ListXForm = (CompositeTransform) this.FindName("ListXForm");
      this.Handle = (Grid) this.FindName("Handle");
      this.HandleRect1 = (Rectangle) this.FindName("HandleRect1");
      this.HandleRect2 = (Rectangle) this.FindName("HandleRect2");
      this.ShareLiveLocationPanel = (StackPanel) this.FindName("ShareLiveLocationPanel");
      this.LiveLocationShare15RadioButton = (RadioButton) this.FindName("LiveLocationShare15RadioButton");
      this.LiveLocationShare480RadioButton = (RadioButton) this.FindName("LiveLocationShare480RadioButton");
      this.ShareLiveLocationCaption = (Grid) this.FindName("ShareLiveLocationCaption");
      this.CaptionBackground = (Rectangle) this.FindName("CaptionBackground");
      this.CaptionPanel = (ScrollViewer) this.FindName("CaptionPanel");
      this.CaptionBlock = (RichTextBlock) this.FindName("CaptionBlock");
      this.SubmitButton = (Button) this.FindName("SubmitButton");
      this.SubmitButtonIcon = (Image) this.FindName("SubmitButtonIcon");
      this.HiddenRectangle = (Grid) this.FindName("HiddenRectangle");
      this.ShareLiveLocationHiddenRectangle = (Rectangle) this.FindName("ShareLiveLocationHiddenRectangle");
      this.ShareLocationHiddenRectangle = (Rectangle) this.FindName("ShareLocationHiddenRectangle");
      this.PlacesList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("PlacesList");
      this.LiveLocationGrid = (Grid) this.FindName("LiveLocationGrid");
      this.ShareLiveLocationButton = (Image) this.FindName("ShareLiveLocationButton");
      this.ShareLiveLocationButtonText = (TextBlock) this.FindName("ShareLiveLocationButtonText");
      this.NearbyPlacesTitle = (TextBlock) this.FindName("NearbyPlacesTitle");
      this.LocationGrid = (Grid) this.FindName("LocationGrid");
      this.ShareLocationAccuracyText = (TextBlock) this.FindName("ShareLocationAccuracyText");
      this.ShareLocationButton = (Image) this.FindName("ShareLocationButton");
      this.Attribution = (AttributionControl) this.FindName("Attribution");
      this.StatusPanel = (StackPanel) this.FindName("StatusPanel");
      this.SearchPanel = (Grid) this.FindName("SearchPanel");
      this.SearchBox = (TextBox) this.FindName("SearchBox");
      this.SearchResultsBox = (ListBox) this.FindName("SearchResultsBox");
      this.CaptionBox = (EmojiTextBox) this.FindName("CaptionBox");
    }

    public class HighlightedSearchResult
    {
      public HighlightedSearchResult(PlaceSearchResult p, string toHighlight)
      {
        this.Place = p;
        string name = p.Name;
        int num = name.IndexOf(toHighlight, StringComparison.CurrentCultureIgnoreCase);
        if (num >= 0 && num < name.Length)
        {
          this.TextPart1 = name.Substring(0, num);
          this.TextHighlight = name.Substring(num, toHighlight.Length);
          this.TextPart2 = name.Substring(num + toHighlight.Length);
        }
        else
          this.TextPart1 = name;
      }

      public string TextPart1 { get; set; }

      public string TextHighlight { get; set; }

      public string TextPart2 { get; set; }

      public PlaceSearchResult Place { get; set; }
    }

    private class Parameters
    {
      public IObserver<Unit> SuccessfulShareObserver { get; set; }

      public string Jid { get; set; }

      public Message QuotedMsg { get; set; }

      public string QuotedChat { get; set; }

      public bool C2cStarted { get; set; }

      public bool LiveLocationOnly { get; set; }
    }
  }
}
