// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChangeNumberNotifyPage
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
using WhatsApp.CommonOps;


namespace WhatsApp
{
  public class ChangeNumberNotifyPage : PhoneApplicationPage
  {
    private static string NextInstanceOldChatId;
    private static string NextInstanceNewNum;
    private static string NextInstanceNewCountryCode;
    private static bool NextInstanceSwitchingCountry;
    private string oldChatId;
    private string newNum;
    private string newCountryCode;
    private bool switchingCountry;
    private List<string> contactsToNotify;
    private List<string> customList;
    private IDisposable notifyRecipientsSub;
    private bool isLoaded;
    private bool isRemoved;
    private bool ignorePickerSelectionChange;
    private bool notifyContactsEnabled;
    private int currentPickerSelection;
    private DateTime? lastSelChangedAt;
    internal ZoomBox LayoutRootZoomBox;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal TextBlock ConfirmationBlock;
    internal TextBlock SwitchingCountryWarningBlock;
    internal TextBlock NotifyTooltipBlock;
    internal Grid NotifyPanel;
    internal ToggleSwitch NotifyContactsToggle;
    internal ListPicker NotifyPicker;
    internal ProgressBar ProgressBar;
    internal Grid NotifySummaryPanel;
    internal RichTextBlock NotifySummaryBlock;
    private bool _contentLoaded;

    public ChangeNumberNotifyPage()
    {
      this.InitializeComponent();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.LayoutRootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.oldChatId = ChangeNumberNotifyPage.NextInstanceOldChatId;
      ChangeNumberNotifyPage.NextInstanceOldChatId = (string) null;
      this.newNum = ChangeNumberNotifyPage.NextInstanceNewNum;
      ChangeNumberNotifyPage.NextInstanceNewNum = (string) null;
      this.newCountryCode = ChangeNumberNotifyPage.NextInstanceNewCountryCode;
      ChangeNumberNotifyPage.NextInstanceNewCountryCode = (string) null;
      this.switchingCountry = ChangeNumberNotifyPage.NextInstanceSwitchingCountry;
      ChangeNumberNotifyPage.NextInstanceSwitchingCountry = false;
      this.NotifyContactsToggle.IsChecked = new bool?(true);
      this.ignorePickerSelectionChange = true;
      this.NotifyPicker.ItemsSource = (IEnumerable) new string[3]
      {
        AppResources.ChangeNumberNotifyAllContacts,
        AppResources.ChangeNumberNotifyChattedContacts,
        AppResources.ChangeNumberNotifyCustom
      };
      this.NotifyPicker.SelectedIndex = this.currentPickerSelection = 1;
      this.ignorePickerSelectionChange = false;
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
      this.UpdateNotifyRecipients();
      this.ConfirmationBlock.Text = string.Format(AppResources.ChangeNumberConfirmation, (object) PhoneNumberFormatter.FormatInternationalNumber(this.oldChatId), (object) PhoneNumberFormatter.FormatInternationalNumber(this.newCountryCode + this.newNum));
      this.NotifyTooltipBlock.Text = AppResources.ChangeNumberNotificationTooltip;
      this.SwitchingCountryWarningBlock.Visibility = this.switchingCountry.ToVisibility();
      this.NotifyPanel.Visibility = (this.notifyContactsEnabled = Settings.ChangeNumberNotifyEnabled).ToVisibility();
    }

    public static void Start(
      string oldChatId,
      string newNum,
      string newCountryCode,
      bool switchingCountry)
    {
      ChangeNumberNotifyPage.NextInstanceOldChatId = oldChatId;
      ChangeNumberNotifyPage.NextInstanceNewNum = newNum;
      ChangeNumberNotifyPage.NextInstanceNewCountryCode = newCountryCode;
      ChangeNumberNotifyPage.NextInstanceSwitchingCountry = switchingCountry;
      NavUtils.NavigateToPage(nameof (ChangeNumberNotifyPage));
    }

    private void UpdateNotifyRecipients()
    {
      this.contactsToNotify = (List<string>) null;
      this.notifyRecipientsSub.SafeDispose();
      this.notifyRecipientsSub = (IDisposable) null;
      this.NotifySummaryBlock.Text = (RichTextBlock.TextSet) null;
      if (((int) this.NotifyContactsToggle.IsChecked ?? 0) == 0)
      {
        this.ProgressBar.Visibility = Visibility.Collapsed;
        this.NotifyPicker.Visibility = Visibility.Collapsed;
        this.NotifySummaryBlock.Text = new RichTextBlock.TextSet()
        {
          Text = AppResources.ChangeNumberNoneContactsToNotify
        };
        this.NotifySummaryPanel.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
      }
      else
      {
        this.NotifySummaryPanel.Margin = new Thickness(0.0, 24.0, 0.0, 0.0);
        this.NotifyPicker.Visibility = Visibility.Visible;
        this.ProgressBar.Visibility = Visibility.Visible;
        this.notifyRecipientsSub = this.GetNotifyRecipientsObservable(this.NotifyPicker.SelectedIndex).SubscribeOn<List<string>>(WAThreadPool.Scheduler).ObserveOnDispatcher<List<string>>().Subscribe<List<string>>((Action<List<string>>) (jids =>
        {
          this.notifyRecipientsSub = (IDisposable) null;
          if (this.isRemoved)
            return;
          this.contactsToNotify = jids;
          this.ProgressBar.Visibility = Visibility.Collapsed;
          bool anyBlockedContacts = false;
          ContactsContext.Instance((Action<ContactsContext>) (db => anyBlockedContacts = db.BlockListSet.Any<KeyValuePair<string, bool>>()));
          string s = Plurals.Instance.GetString(AppResources.ChangeNumberNContactsToNotifyPlural, jids.Count);
          if (anyBlockedContacts)
            s = string.Format("{0} {1}", (object) s, (object) AppResources.ChangeNumberBlockedContactsTooltip);
          WaRichText.Chunk chunk = ((IEnumerable<WaRichText.Chunk>) WaRichText.GetHtmlLinkChunks(s)).SingleOrDefault<WaRichText.Chunk>();
          if (chunk == null)
          {
            this.NotifySummaryBlock.Text = new RichTextBlock.TextSet()
            {
              Text = s
            };
          }
          else
          {
            chunk.Format = WaRichText.Formats.Link | WaRichText.Formats.Bold;
            chunk.ClickAction = (Action) (() => { });
            this.NotifySummaryBlock.Text = new RichTextBlock.TextSet()
            {
              Text = s,
              PartialFormattings = (IEnumerable<WaRichText.Chunk>) new WaRichText.Chunk[1]
              {
                chunk
              }
            };
          }
        }));
      }
    }

    private IObservable<List<string>> GetNotifyRecipientsObservable(int selIndex)
    {
      if (selIndex == 2)
      {
        if (this.customList == null)
          this.customList = new List<string>();
        return Observable.Return<List<string>>(this.customList);
      }
      bool withChatsOnly = selIndex == 1;
      return Observable.Create<List<string>>((Func<IObserver<List<string>>, Action>) (observer =>
      {
        List<string> notifyRecipients = AccountManagement.GetChangeNumberNotifyRecipients(withChatsOnly);
        observer.OnNext(notifyRecipients);
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    private void OpenCustomPicker(int previousPickerSelection)
    {
      RecipientListPage.StartContactPicker(AppResources.ChangeNumberNotifyNewNumber, (IEnumerable<string>) this.contactsToNotify, false).ObserveOnDispatcher<RecipientListPage.RecipientListResults>().Subscribe<RecipientListPage.RecipientListResults>((Action<RecipientListPage.RecipientListResults>) (recipientListResults =>
      {
        List<string> selectedJids = recipientListResults?.SelectedJids;
        if (selectedJids == null)
        {
          this.ignorePickerSelectionChange = true;
          this.NotifyPicker.SelectedIndex = previousPickerSelection;
          this.ignorePickerSelectionChange = false;
        }
        else
        {
          bool flag = true;
          if (selectedJids.Count == this.contactsToNotify.Count)
          {
            HashSet<string> stringSet = new HashSet<string>();
            foreach (string str in selectedJids)
              stringSet.Add(str);
            flag = false;
            foreach (string str in this.contactsToNotify)
            {
              if (!stringSet.Contains(str))
              {
                flag = true;
                break;
              }
            }
          }
          if (!flag)
            return;
          this.customList = selectedJids;
          this.ignorePickerSelectionChange = true;
          this.NotifyPicker.SelectedIndex = 2;
          this.ignorePickerSelectionChange = false;
          this.UpdateNotifyRecipients();
        }
      }));
    }

    private void OnLoaded(object sender, EventArgs e)
    {
      if (this.isLoaded)
        return;
      this.NotifyContactsToggle.Checked += new EventHandler<RoutedEventArgs>(this.NotifyContactsToggle_Checked);
      this.NotifyContactsToggle.Unchecked += new EventHandler<RoutedEventArgs>(this.NotifyContactsToggle_Unchecked);
      this.NotifyPicker.SelectionChanged += new SelectionChangedEventHandler(this.NotifyPicker_SelectionChanged);
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.isRemoved = true;
      this.notifyRecipientsSub.SafeDispose();
      this.notifyRecipientsSub = (IDisposable) null;
      base.OnRemovedFromJournal(e);
    }

    private void NotifyContactsToggle_Checked(object sender, RoutedEventArgs e)
    {
      this.UpdateNotifyRecipients();
    }

    private void NotifyContactsToggle_Unchecked(object sender, RoutedEventArgs e)
    {
      this.UpdateNotifyRecipients();
    }

    private void NotifyPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      int currentPickerSelection = this.currentPickerSelection;
      int selectedIndex = this.NotifyPicker.SelectedIndex;
      DateTime now = DateTime.Now;
      bool flag = false;
      if (this.lastSelChangedAt.HasValue && (now - this.lastSelChangedAt.Value).TotalMilliseconds < 500.0)
        flag = true;
      this.lastSelChangedAt = new DateTime?(now);
      Log.d("change number", "selection changed:{0}->{1},dup:{2},ignore:{3}", (object) currentPickerSelection, (object) selectedIndex, (object) flag, (object) this.ignorePickerSelectionChange);
      if (flag)
        return;
      this.currentPickerSelection = selectedIndex;
      if (this.ignorePickerSelectionChange)
        return;
      if (this.currentPickerSelection == 2)
        this.OpenCustomPicker(currentPickerSelection);
      else
        this.UpdateNotifyRecipients();
    }

    private void Submit_Click(object sender, EventArgs e)
    {
      Settings.ChangeNumberNotifyJids = this.contactsToNotify;
      if (this.switchingCountry)
      {
        Settings.SyncBackoffUtc = new DateTime?();
        Settings.EverstoreBackoffUtc = new DateTime?();
        Settings.EverstoreBackoffAttempt = 0;
      }
      Settings.OldChatID = this.oldChatId;
      Settings.OldCountryCode = Settings.CountryCode;
      Settings.OldPhoneNumber = Settings.PhoneNumber;
      Settings.CountryCode = this.newCountryCode;
      Settings.PhoneNumber = this.newNum;
      Settings.ChatID = (string) null;
      Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.NewlyEntered;
      Settings.SuppressRestoreFromBackupAtReg = true;
      Settings.Delete(Settings.Key.CodeEntryWaitToRetryUtc);
      Log.l("change number", "updated settings");
      ConversionRecordHelper.ClearConversionRecords();
      Backup.ResetBackupKeys();
      App.CurrentApp.ConnectionResetSubject.OnNext(new Unit());
      Log.l("change number", "disconnected");
      FieldStats.ReportChangeNumber();
      NavUtils.NavigateHome();
    }

    private void NotifySummaryBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.OpenCustomPicker(this.currentPickerSelection);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/ChangeNumberNotifyPage.xaml", UriKind.Relative));
      this.LayoutRootZoomBox = (ZoomBox) this.FindName("LayoutRootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.ConfirmationBlock = (TextBlock) this.FindName("ConfirmationBlock");
      this.SwitchingCountryWarningBlock = (TextBlock) this.FindName("SwitchingCountryWarningBlock");
      this.NotifyTooltipBlock = (TextBlock) this.FindName("NotifyTooltipBlock");
      this.NotifyPanel = (Grid) this.FindName("NotifyPanel");
      this.NotifyContactsToggle = (ToggleSwitch) this.FindName("NotifyContactsToggle");
      this.NotifyPicker = (ListPicker) this.FindName("NotifyPicker");
      this.ProgressBar = (ProgressBar) this.FindName("ProgressBar");
      this.NotifySummaryPanel = (Grid) this.FindName("NotifySummaryPanel");
      this.NotifySummaryBlock = (RichTextBlock) this.FindName("NotifySummaryBlock");
    }
  }
}
