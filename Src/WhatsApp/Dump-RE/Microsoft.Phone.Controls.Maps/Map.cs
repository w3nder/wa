// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Map
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.AutomationPeers;
using Microsoft.Phone.Controls.Maps.Core;
using Microsoft.Phone.Controls.Maps.Design;
using Microsoft.Phone.Controls.Maps.Overlays;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Threading;

#nullable disable
namespace Microsoft.Phone.Controls.Maps
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public sealed class Map : MapCore
  {
    private static readonly string version = Map.GetVersion();
    private WeakEventListener<Map, object, PropertyChangedEventArgs> _weakMapCredentials;
    private LoadingErrorMessage loadingErrorMessage;
    private string logServiceUriFormat;
    private Guid sessionId = Guid.Empty;
    private DispatcherTimer timer;
    private DispatcherTimer networkChangePollTimer;

    public Map()
    {
      this.LoadingError += new EventHandler<LoadingErrorEventArgs>(this.Map_LoadingError);
      base.CredentialsProvider = (CredentialsProvider) new ApplicationIdCredentialsProvider();
      this.Mode = (MapMode) new RoadMode();
      this.ScaleVisibility = Visibility.Collapsed;
      this.ZoomBarVisibility = Visibility.Collapsed;
      this.MapForeground = new MapForeground((MapBase) this);
      this.RootLayer.Children.Add((UIElement) this.MapForeground);
      this.Unloaded += (RoutedEventHandler) ((sender, e) =>
      {
        if (this.timer != null)
          this.timer.Tick -= new EventHandler(this.LogStartSession);
        if (this.networkChangePollTimer == null)
          return;
        this.networkChangePollTimer.Tick -= new EventHandler(this.NetworkChangePollTimer_Tick);
      });
    }

    ~Map()
    {
      if (this._weakMapCredentials == null)
        return;
      this._weakMapCredentials.Detach();
      this._weakMapCredentials = (WeakEventListener<Map, object, PropertyChangedEventArgs>) null;
    }

    public MapForeground MapForeground { get; private set; }

    [TypeConverter(typeof (MapModeConverter))]
    public override MapMode Mode
    {
      get => base.Mode;
      set => base.Mode = value;
    }

    [TypeConverter(typeof (ApplicationIdCredentialsProviderConverter))]
    public new CredentialsProvider CredentialsProvider
    {
      get => base.CredentialsProvider;
      set => base.CredentialsProvider = value;
    }

    protected override void OnCultureChanged(DependencyPropertyChangedEventArgs eventArgs)
    {
      base.OnCultureChanged(eventArgs);
      if (this.MapForeground.Culture != null && !(eventArgs.OldValue as string == this.MapForeground.Culture))
        return;
      this.MapForeground.Culture = eventArgs.NewValue as string;
    }

    protected override void OnCredentialsProviderChanged(
      DependencyPropertyChangedEventArgs eventArgs)
    {
      base.OnCredentialsProviderChanged(eventArgs);
      if (eventArgs.OldValue is INotifyPropertyChanged && this._weakMapCredentials != null)
      {
        this._weakMapCredentials.Detach();
        this._weakMapCredentials = (WeakEventListener<Map, object, PropertyChangedEventArgs>) null;
      }
      INotifyPropertyChanged newCredentials = eventArgs.NewValue as INotifyPropertyChanged;
      if (newCredentials != null)
      {
        this._weakMapCredentials = new WeakEventListener<Map, object, PropertyChangedEventArgs>(this);
        this._weakMapCredentials.OnEventAction = (Action<Map, object, PropertyChangedEventArgs>) ((instance, source, leventArgs) => instance.Credentials_PropertyChanged(source, leventArgs));
        this._weakMapCredentials.OnDetachAction = (Action<WeakEventListener<Map, object, PropertyChangedEventArgs>>) (weakEventListener => newCredentials.PropertyChanged -= new PropertyChangedEventHandler(weakEventListener.OnEvent));
        newCredentials.PropertyChanged += new PropertyChangedEventHandler(this._weakMapCredentials.OnEvent);
      }
      this.Log("2");
    }

    protected override void OnFirstFrame()
    {
      base.OnFirstFrame();
      MapConfiguration.GetSection("v1", "WP7SLMapControl", (string) null, new MapConfigurationCallback(this.ConfigurationLoadedAfterFirstFrame));
      if (DesignerProperties.IsInDesignTool || NetworkInterface.GetIsNetworkAvailable())
        return;
      this.ThrowLoadingException((Exception) new ConfigurationNotLoadedException());
      this.networkChangePollTimer = new DispatcherTimer()
      {
        Interval = TimeSpan.FromSeconds(5.0)
      };
      this.networkChangePollTimer.Tick += new EventHandler(this.NetworkChangePollTimer_Tick);
      this.networkChangePollTimer.Start();
    }

    private static string GetVersion()
    {
      string version = string.Empty;
      string[] strArray = Assembly.GetExecutingAssembly().FullName.Split(',');
      if (strArray.Length > 1)
        version = strArray[1].Replace("Version=", string.Empty).Trim();
      return version;
    }

    private void ConfigurationLoadedAfterFirstFrame(
      MapConfigurationSection config,
      object userState)
    {
      if (config == null)
        return;
      this.logServiceUriFormat = config["LogServiceUriFormat"];
      if (string.IsNullOrEmpty(this.logServiceUriFormat))
        return;
      this.logServiceUriFormat = this.logServiceUriFormat.Replace("{UriScheme}", "HTTP");
      this.timer = new DispatcherTimer();
      this.timer.Interval = new TimeSpan(0, 0, 0, 5);
      this.timer.Tick += new EventHandler(this.LogStartSession);
      this.timer.Start();
    }

    private void LogStartSession(object sender, EventArgs e)
    {
      this.timer.Stop();
      this.timer.Tick -= new EventHandler(this.LogStartSession);
      this.timer = (DispatcherTimer) null;
      this.sessionId = Guid.NewGuid();
      this.Log("0");
    }

    private void Log(string entry)
    {
      if (this.CredentialsProvider == null)
        return;
      this.CredentialsProvider.GetCredentials((Action<Credentials>) (credentials => this.Log(entry, credentials)));
    }

    private void Log(string entry, Credentials credentials)
    {
      try
      {
        if (string.IsNullOrEmpty(this.logServiceUriFormat) || DesignerProperties.GetIsInDesignMode((DependencyObject) this) || !(this.sessionId != Guid.Empty) || !NetworkInterface.GetIsNetworkAvailable())
          return;
        HttpWebRequest state = (HttpWebRequest) WebRequest.Create(string.Format((IFormatProvider) CultureInfo.InvariantCulture, this.logServiceUriFormat, (object) entry, (object) credentials, (object) Map.version, (object) this.sessionId, (object) this.Culture));
        state.BeginGetResponse(new AsyncCallback(this.LogResponse), (object) state);
      }
      catch (WebException ex)
      {
        this.OnCredentialsError();
      }
      catch (NotSupportedException ex)
      {
      }
    }

    private void LogResponse(IAsyncResult result)
    {
      bool flag;
      try
      {
        flag = ((HttpWebResponse) ((WebRequest) result.AsyncState).EndGetResponse(result)).StatusCode != HttpStatusCode.Unauthorized;
      }
      catch (WebException ex)
      {
        flag = !(ex.Response is HttpWebResponse response) || response.StatusCode != HttpStatusCode.Unauthorized;
      }
      if (flag)
        this.Dispatcher.BeginInvoke((Action) (() => this.OnCredentialsValid()));
      else
        this.Dispatcher.BeginInvoke((Action) (() => this.OnCredentialsError()));
    }

    private void NetworkChangePollTimer_Tick(object sender, EventArgs e)
    {
      if (!NetworkInterface.GetIsNetworkAvailable())
        return;
      this.networkChangePollTimer.Stop();
      this.networkChangePollTimer.Tick -= new EventHandler(this.NetworkChangePollTimer_Tick);
      this.networkChangePollTimer = (DispatcherTimer) null;
      if (!(this.LoadingException is ConfigurationNotLoadedException))
        return;
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        this.LoadingException = (Exception) null;
        if (this.loadingErrorMessage == null)
          return;
        this.RootLayer.Children.Remove((UIElement) this.loadingErrorMessage);
        this.loadingErrorMessage = (LoadingErrorMessage) null;
      }));
    }

    private void OnCredentialsError()
    {
      this.ThrowLoadingException((Exception) new CredentialsInvalidException());
    }

    private void OnCredentialsValid()
    {
      if (!(this.LoadingException is CredentialsInvalidException))
        return;
      this.LoadingException = (Exception) null;
      if (this.loadingErrorMessage == null)
        return;
      this.RootLayer.Children.Remove((UIElement) this.loadingErrorMessage);
      this.loadingErrorMessage = (LoadingErrorMessage) null;
    }

    private void Credentials_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      this.Log("2");
    }

    private void Map_LoadingError(object sender, LoadingErrorEventArgs e)
    {
      if (this.loadingErrorMessage == null)
      {
        this.loadingErrorMessage = new LoadingErrorMessage();
        this.RootLayer.Children.Add((UIElement) this.loadingErrorMessage);
      }
      if (e.LoadingException is UriSchemeNotSupportedException)
        this.loadingErrorMessage.SetUriSchemeError(this.Culture);
      else if (e.LoadingException is ConfigurationNotLoadedException)
      {
        this.loadingErrorMessage.SetConfigurationError(this.Culture);
      }
      else
      {
        if (!(e.LoadingException is CredentialsInvalidException))
          return;
        this.loadingErrorMessage.SetCredentialsError(this.Culture);
      }
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return (AutomationPeer) new MapAutomationPeer(this);
    }

    private static class LogEntry
    {
      public const string StartSession = "0";
      public const string ChangeCredentials = "2";
    }
  }
}
