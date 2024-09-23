// Decompiled with JetBrains decompiler
// Type: WhatsApp.MyStatusPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using WhatsApp.CommonOps;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class MyStatusPage : PhoneApplicationPage
  {
    private ApplicationBarIconButton acceptButton_;
    private bool pageRemoved_;
    private bool actionTaken_;
    private int statusBoxHeight_;
    private MyStatusPage.StatusCache SavedStatuses;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal WhatsApp.CompatibilityShims.LongListSelector Listbox;
    internal EmojiTextBox StatusBox;
    internal ProgressBar ProgressBar;
    internal TextBlock ListSelectorTitleBlock;
    private bool _contentLoaded;

    private ApplicationBarIconButton AcceptButton
    {
      get
      {
        if (this.acceptButton_ == null)
          this.acceptButton_ = this.ApplicationBar.Buttons[0] as ApplicationBarIconButton;
        return this.acceptButton_;
      }
    }

    public MyStatusPage()
    {
      this.InitializeComponent();
      this.LoadStatusCache();
      this.ListSelectorTitleBlock.Text = AppResources.SelectNewRevivedStatusV2Title;
      this.Listbox.ItemsSource = (IList) this.SavedStatuses.ConvertToItemList();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.Init();
    }

    private void LoadStatusCache()
    {
      byte[] statusesBlob = Settings.StatusesBlob;
      if (statusesBlob == null)
      {
        this.SavedStatuses = new MyStatusPage.StatusCache();
      }
      else
      {
        using (MemoryStream memoryStream = new MemoryStream(statusesBlob, 0, statusesBlob.Length, false))
          this.SavedStatuses = new DataContractJsonSerializer(typeof (MyStatusPage.StatusCache)).ReadObject((Stream) memoryStream) as MyStatusPage.StatusCache;
      }
    }

    private void SaveStatusCache()
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        new DataContractJsonSerializer(typeof (MyStatusPage.StatusCache)).WriteObject((Stream) memoryStream, (object) this.SavedStatuses);
        Settings.StatusesBlob = memoryStream.ToArray();
      }
    }

    private void Init()
    {
      AppState.Worker.Enqueue((Action) (() =>
      {
        string currStatus = ContactsContext.Instance<string>((Func<ContactsContext, string>) (db => db.GetUserStatus(Settings.MyJid).Status ?? ""));
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          if (this.pageRemoved_)
            return;
          this.StatusBox.Text = Emoji.ConvertToTextOnly(currStatus, (byte[]) null);
          this.StatusBox.IsReadOnly = false;
          this.StatusBox.TextBox.SelectionLength = 0;
          this.StatusBox.TextBox.SelectionStart = currStatus.Length;
          this.ProgressBar.Visibility = Visibility.Collapsed;
        }));
      }));
      Observable.Return<Unit>(new Unit()).Concat<Unit>(this.StatusBox.GetTextChangedAsync().Select<TextChangedEventArgs, Unit>((Func<TextChangedEventArgs, Unit>) (_ => new Unit()))).Subscribe<Unit>((Action<Unit>) (_ =>
      {
        if (this.AcceptButton == null)
          return;
        this.AcceptButton.IsEnabled = (this.StatusBox.Text ?? "").Trim().Length > 0;
      }));
      this.StatusBox.LayoutUpdated += new EventHandler(this.StatusBox_LayoutUpdated);
      this.StatusBox.MaxLength = 139;
      this.StatusBox.SIPChanged += (EventHandler) ((sender, e) => this.RefreshPageMargin());
      this.TitlePanel.SmallTitle = AppResources.ProfileTitle;
      this.TitlePanel.LargeTitle = AppResources.RevivedStatusV2Lower;
    }

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      MyStatusPage.Item selectedItem = (sender as Microsoft.Phone.Controls.LongListSelector).SelectedItem as MyStatusPage.Item;
      this.StatusBox.Text = selectedItem.Text.Text;
      foreach (MyStatusPage.Item obj in (IEnumerable) this.Listbox.ItemsSource)
        obj.IsSelected = false;
      selectedItem.IsSelected = true;
    }

    private void RefreshPageMargin()
    {
      if (this.StatusBox.IsSIPUp)
      {
        int num = this.Orientation.IsLandscape() ? 1 : 0;
        int top = 0;
        if (num != 0)
          top = -30 - this.statusBoxHeight_;
        else if (this.StatusBox.IsEmojiKeyboardOpen)
          top = Math.Min(0, 117 - this.statusBoxHeight_);
        this.LayoutRoot.Margin = new Thickness(0.0, (double) top, 0.0, 0.0);
      }
      else
        this.LayoutRoot.Margin = new Thickness(0.0);
    }

    private void OnAcceptClick(object sender, EventArgs e)
    {
      if (this.actionTaken_)
        return;
      this.actionTaken_ = true;
      string newStatus = Emoji.ConvertToRichText(Emoji.ConvertToUnicode(this.StatusBox.Text)).Trim();
      if (!string.IsNullOrEmpty(newStatus))
      {
        SetStatus.Set(newStatus);
        if (this.SavedStatuses.Statuses.Contains(newStatus))
          this.SavedStatuses.Statuses.Remove(newStatus);
        this.SavedStatuses.Statuses.Insert(0, newStatus);
        this.SaveStatusCache();
      }
      FieldStats.ReportStatusUpdate();
      this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
    }

    protected override void OnOrientationChanged(OrientationChangedEventArgs e)
    {
      base.OnOrientationChanged(e);
      this.RefreshPageMargin();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      FieldStats.ReportUiUsage(wam_enum_ui_usage_type.STATUS_VIEWS);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      this.StatusBox.CloseEmojiKeyboard();
      base.OnNavigatedFrom(e);
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.pageRemoved_ = true;
      base.OnRemovedFromJournal(e);
    }

    private void Background_Tap(object sender, EventArgs e) => this.StatusBox.CloseEmojiKeyboard();

    private void StatusBox_LayoutUpdated(object sender, EventArgs e)
    {
      int actualHeight = (int) this.StatusBox.ActualHeight;
      if (this.statusBoxHeight_ == actualHeight)
        return;
      this.statusBoxHeight_ = actualHeight;
      this.RefreshPageMargin();
    }

    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
      if (!((sender as MenuItem).DataContext is MyStatusPage.Item dataContext))
        return;
      ((Collection<MyStatusPage.Item>) this.Listbox.ItemsSource).Remove(dataContext);
      this.SavedStatuses.Statuses.Remove(dataContext.Text.Text);
      this.SaveStatusCache();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/MyStatusPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.Listbox = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("Listbox");
      this.StatusBox = (EmojiTextBox) this.FindName("StatusBox");
      this.ProgressBar = (ProgressBar) this.FindName("ProgressBar");
      this.ListSelectorTitleBlock = (TextBlock) this.FindName("ListSelectorTitleBlock");
    }

    public class Item : WaViewModelBase
    {
      private bool isSelected;

      public RichTextBlock.TextSet Text { get; private set; }

      public bool IsSelected
      {
        get => this.isSelected;
        set
        {
          if (this.isSelected == value)
            return;
          this.isSelected = value;
          this.NotifyPropertyChanged("ForegroundBrush");
        }
      }

      public Brush ForegroundBrush
      {
        get => !this.IsSelected ? (Brush) UIUtils.ForegroundBrush : (Brush) UIUtils.AccentBrush;
      }

      public Item(string text)
      {
        this.Text = new RichTextBlock.TextSet()
        {
          Text = text
        };
      }
    }

    [DataContract]
    public class StatusCache
    {
      [DataMember(Name = "Status")]
      public ObservableCollection<string> Statuses { get; set; }

      public StatusCache()
      {
        this.Statuses = new ObservableCollection<string>((IEnumerable<string>) new string[9]
        {
          AppResources.StatusAvailable,
          AppResources.StatusBusy,
          AppResources.StatusWork,
          AppResources.StatusSchool,
          AppResources.StatusMeeting,
          AppResources.StatusUrgentOnly,
          AppResources.StatusBattery,
          AppResources.StatusCantTalk,
          AppResources.StatusSleeping
        });
      }

      public ObservableCollection<MyStatusPage.Item> ConvertToItemList()
      {
        ObservableCollection<MyStatusPage.Item> source = new ObservableCollection<MyStatusPage.Item>();
        foreach (string text in (IEnumerable<string>) ((object) this.Statuses ?? (object) new string[0]))
          source.Add(new MyStatusPage.Item(Emoji.ConvertToUnicode(text)));
        string text1 = ContactsContext.Instance<string>((Func<ContactsContext, string>) (db => db.GetUserStatus(Settings.MyJid).Status ?? ""));
        if (source.Count<MyStatusPage.Item>() == 0)
          source.Add(new MyStatusPage.Item(Emoji.ConvertToUnicode(text1)));
        if (text1 == source.ElementAt<MyStatusPage.Item>(0).Text.Text)
          source.ElementAt<MyStatusPage.Item>(0).IsSelected = true;
        return source;
      }
    }
  }
}
