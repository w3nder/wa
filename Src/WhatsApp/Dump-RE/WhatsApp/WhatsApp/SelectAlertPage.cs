// Decompiled with JetBrains decompiler
// Type: WhatsApp.SelectAlertPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

#nullable disable
namespace WhatsApp
{
  public class SelectAlertPage : PhoneApplicationPage
  {
    private const string TargetGroup = "group";
    private const string TargetIndividual = "individual";
    private static string[] NextInstanceTargets;
    private string[] targets;
    private CustomTones.Tone selection;
    private bool initialized;
    private bool pageRemoved;
    private CustomTones.Tone[] loadedTones;
    internal Grid LayoutRoot;
    internal Grid ContentPanel;
    internal MediaElement MediaElement;
    internal ScrollViewer ScrollViewer;
    internal ItemsControl TonesItemsControl;
    private bool _contentLoaded;

    public SelectAlertPage()
    {
      this.InitializeComponent();
      this.targets = SelectAlertPage.NextInstanceTargets;
      SelectAlertPage.NextInstanceTargets = (string[]) null;
      (this.Resources[(object) "SelectedItemToAccentBrushConverter"] as SelectedItemToAccentBrushConverter).IsSelected = (Func<object, bool>) (o => o == this.selection);
      this.TonesItemsControl.ItemsSource = (IEnumerable) (this.loadedTones = this.ProcessAlerts((IEnumerable<CustomTones.Tone>) CustomTones.ListAlerts()).ToArray<CustomTones.Tone>());
    }

    public static void StartForJids(string[] jids)
    {
      SelectAlertPage.NextInstanceTargets = jids;
      NavUtils.NavigateToPage(nameof (SelectAlertPage), folderName: "Pages/Settings");
    }

    public static void StartForGroup()
    {
      SelectAlertPage.NextInstanceTargets = new string[1]
      {
        "group"
      };
      NavUtils.NavigateToPage(nameof (SelectAlertPage), folderName: "Pages/Settings");
    }

    public static void StartForIndividual()
    {
      SelectAlertPage.NextInstanceTargets = new string[1]
      {
        "individual"
      };
      NavUtils.NavigateToPage(nameof (SelectAlertPage), folderName: "Pages/Settings");
    }

    private IObservable<CustomTones.Tone> GetCurrentSelection()
    {
      return this.targets == null || !((IEnumerable<string>) this.targets).Any<string>() ? Observable.Empty<CustomTones.Tone>() : Observable.Create<CustomTones.Tone>((Func<IObserver<CustomTones.Tone>, Action>) (observer =>
      {
        string tonePath = (string) null;
        switch (((IEnumerable<string>) this.targets).FirstOrDefault<string>())
        {
          case "group":
            tonePath = Settings.GroupTone ?? CustomTones.DefaultAlertPath;
            break;
          case "individual":
            tonePath = Settings.IndividualTone ?? CustomTones.DefaultAlertPath;
            break;
          default:
            string[] jids = this.targets;
            List<string> tones = new List<string>();
            MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
            {
              foreach (string jid in jids)
              {
                JidInfo jidInfo = db.GetJidInfo(jid, CreateOptions.None);
                tones.Add(jidInfo == null ? (string) null : jidInfo.NotificationSound);
              }
            }));
            string[] array = tones.MakeUnique<string, string>((Func<string, string>) (path => path ?? "")).ToArray<string>();
            tonePath = ((IEnumerable<string>) array).Count<string>() > 1 ? (string) null : ((IEnumerable<string>) array).FirstOrDefault<string>() ?? CustomTones.DefaultAlertPath;
            break;
        }
        CustomTones.Tone tone = tonePath == null ? (CustomTones.Tone) null : ((IEnumerable<CustomTones.Tone>) this.loadedTones).FirstOrDefault<CustomTones.Tone>((Func<CustomTones.Tone, bool>) (t => t.Path == tonePath));
        observer.OnNext(tone);
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    private void SaveTone(CustomTones.Tone tone)
    {
      if (this.targets == null || !((IEnumerable<string>) this.targets).Any<string>())
        return;
      switch (((IEnumerable<string>) this.targets).FirstOrDefault<string>())
      {
        case "group":
          Settings.GroupTone = tone.Path;
          break;
        case "individual":
          Settings.IndividualTone = tone.Path;
          break;
        default:
          string[] jids = this.targets;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            foreach (string jid in jids)
              db.GetJidInfo(jid, CreateOptions.CreateToDbIfNotFound).NotificationSound = tone.Path;
            db.SubmitChanges();
          }));
          break;
      }
    }

    private IEnumerable<CustomTones.Tone> ProcessAlerts(IEnumerable<CustomTones.Tone> source)
    {
      DataTemplate headerTemplate = (DataTemplate) this.Resources[(object) "HeaderTemplate"];
      DataTemplate normalTemplate = (DataTemplate) this.Resources[(object) "ToneTemplate"];
      bool haveCategories = source.Select<CustomTones.Tone, string>((Func<CustomTones.Tone, string>) (t => t.Label)).MakeUnique<string>().Count<string>() > 1;
      string lastCategory = (string) null;
      foreach (CustomTones.Tone tone in source)
      {
        if (haveCategories && tone.Label != lastCategory)
        {
          lastCategory = tone.Label;
          yield return new CustomTones.Tone()
          {
            Title = tone.Label,
            Template = headerTemplate
          };
        }
        tone.Template = normalTemplate;
        yield return tone;
      }
    }

    private void Play(CustomTones.Tone tone)
    {
      if (this.MediaElement.Source != (Uri) null)
      {
        try
        {
          this.MediaElement.Stop();
        }
        catch (Exception ex)
        {
        }
      }
      this.MediaElement.Source = new Uri(tone.Path.Replace('\\', '/'), UriKind.Relative);
      this.MediaElement.Play();
    }

    public void Select(CustomTones.Tone tone)
    {
      CustomTones.Tone selection = this.selection;
      this.selection = tone;
      selection?.UpdateBinding();
      tone.UpdateBinding();
      NavUtils.GoBack();
      this.SaveTone(this.selection);
    }

    private void InitOnLoaded()
    {
      if (this.initialized || this.pageRemoved)
        return;
      this.initialized = true;
      this.GetCurrentSelection().SubscribeOn<CustomTones.Tone>((IScheduler) AppState.Worker).ObserveOnDispatcher<CustomTones.Tone>().Subscribe<CustomTones.Tone>((Action<CustomTones.Tone>) (tone =>
      {
        this.selection = tone;
        this.selection.UpdateBinding();
        if (!(this.TonesItemsControl.ItemContainerGenerator.ContainerFromItem((object) tone) is UIElement uiElement2))
          return;
        UIElement rootVisual = Application.Current.RootVisual;
        this.ScrollViewer.ScrollToVerticalOffset(uiElement2.TransformToVisual(rootVisual).Transform(new System.Windows.Point(0.0, 0.0)).Y - this.TonesItemsControl.TransformToVisual(rootVisual).Transform(new System.Windows.Point(0.0, 0.0)).Y);
      }));
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.pageRemoved = true;
      base.OnRemovedFromJournal(e);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (e.NavigationMode == NavigationMode.Reset)
        return;
      if (this.targets == null || !((IEnumerable<string>) this.targets).Any<string>())
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack()));
      else
        this.TonesItemsControl.GetLoadedAsync().Take<Unit>(1).Subscribe<Unit>((Action<Unit>) (_ => this.InitOnLoaded()));
    }

    private void RoundButton_Loaded(object sender, RoutedEventArgs e)
    {
      if (!(sender is Image image))
        return;
      CustomTones.Tone tone = image.Tag as CustomTones.Tone;
      if (tone == null)
        return;
      if (tone.Path == "Sounds\\Silent")
      {
        image.Source = (System.Windows.Media.ImageSource) AssetStore.DismissIconWhite;
      }
      else
      {
        image.Source = (System.Windows.Media.ImageSource) ImageStore.PlayButton;
        image.Tap += (EventHandler<System.Windows.Input.GestureEventArgs>) ((_, __) => this.Play(tone));
      }
    }

    private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (!(sender is FrameworkElement frameworkElement) || !(frameworkElement.Tag is CustomTones.Tone tag))
        return;
      this.Select(tag);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/Settings/SelectAlertPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.ContentPanel = (Grid) this.FindName("ContentPanel");
      this.MediaElement = (MediaElement) this.FindName("MediaElement");
      this.ScrollViewer = (ScrollViewer) this.FindName("ScrollViewer");
      this.TonesItemsControl = (ItemsControl) this.FindName("TonesItemsControl");
    }
  }
}
