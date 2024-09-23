// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactInfoPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WhatsApp.CommonOps;
using WhatsApp.ContactClasses;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class ContactInfoPage : PhoneApplicationPage
  {
    private const string LogHeader = "contact info";
    private static System.Windows.Media.ImageSource NextInstanceProfilePic;
    private static UserStatus NextInstanceUser;
    private static Contact NextInstanceContact;
    private static ContactInfoPage.Tab[] NextInstanceTabs;
    private ContactInfoPageViewModel viewModel;
    private ContactInfoPageData pageData;
    private bool pageRemoved;
    private PivotItem infoPivotItem;
    private ContactInfoTabView infoTabView;
    private PivotItem callsPivotItem;
    private ContactCallLogView callLogView;
    private ContactInfoPage.Tab currTab;
    private IDisposable emailSub;
    private GlobalProgressIndicator progressIndicator;
    private Microsoft.Phone.Shell.ApplicationBar infoAppBar;
    private Microsoft.Phone.Shell.ApplicationBar saveImageAppBar;
    private ApplicationBarIconButton saveImageButton;
    private ApplicationBarIconButton pinButton;
    private ApplicationBarMenuItem blockMenuItem;
    private ApplicationBarMenuItem unblockMenuItem;
    private ApplicationBarMenuItem reportSpamMenuItem;
    private Microsoft.Phone.Shell.ApplicationBar appbar = new Microsoft.Phone.Shell.ApplicationBar();
    internal Storyboard EnterProfileImage;
    internal DoubleAnimation EnterTranslateX;
    internal DoubleAnimation EnterTranslateY;
    internal DoubleAnimation EnterScaleX;
    internal DoubleAnimation EnterScaleY;
    internal Storyboard ExitProfileImage;
    internal DoubleAnimation ExitTranslateX;
    internal DoubleAnimation ExitTranslateY;
    internal DoubleAnimation ExitScaleX;
    internal DoubleAnimation ExitScaleY;
    internal Grid LayoutRoot;
    internal Grid ContentPanel;
    internal Image VerifiedBadge;
    internal Pivot Pivot;
    internal ImageViewControl LargeImage;
    internal ListPicker InvitationPicker;
    private bool _contentLoaded;

    public ContactInfoPage()
    {
      this.InitializeComponent();
      this.progressIndicator = new GlobalProgressIndicator((DependencyObject) this);
      ContactInfoPage.Tab[] nextInstanceTabs = ContactInfoPage.NextInstanceTabs;
      ContactInfoPage.NextInstanceTabs = (ContactInfoPage.Tab[]) null;
      this.InitPivot(nextInstanceTabs);
      if (ContactInfoPage.NextInstanceUser != null || ContactInfoPage.NextInstanceContact != null)
        this.Init(ContactInfoPage.NextInstanceUser, ContactInfoPage.NextInstanceContact, ContactInfoPage.NextInstanceProfilePic);
      ContactInfoPage.NextInstanceUser = (UserStatus) null;
      ContactInfoPage.NextInstanceContact = (Contact) null;
      ContactInfoPage.NextInstanceProfilePic = (System.Windows.Media.ImageSource) null;
      this.infoTabView.ProfileImage.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.ProfilePic_Tap);
    }

    public static void Start(Contact contact, System.Windows.Media.ImageSource profilePic = null, bool replacePage = false)
    {
      if (contact == null)
        return;
      ContactInfoPage.NextInstanceContact = contact;
      ContactInfoPage.NextInstanceUser = (UserStatus) null;
      ContactInfoPage.NextInstanceProfilePic = profilePic;
      ContactInfoPage.NextInstanceTabs = (ContactInfoPage.Tab[]) null;
      WaUriParams uriParams = new WaUriParams();
      uriParams.AddString("contactId", contact.GetIdentifier());
      if (replacePage)
        uriParams.AddBool("PageReplace", replacePage);
      NavUtils.NavigateToPage(nameof (ContactInfoPage), uriParams);
    }

    public static void Start(
      UserStatus user,
      System.Windows.Media.ImageSource profilePic = null,
      ContactInfoPage.Tab startingTab = ContactInfoPage.Tab.Info,
      bool replacePage = false)
    {
      if (user == null)
        return;
      ContactInfoPage.NextInstanceContact = (Contact) null;
      ContactInfoPage.NextInstanceUser = user;
      ContactInfoPage.NextInstanceProfilePic = profilePic;
      ContactInfoPage.Tab[] tabArray;
      if (startingTab != ContactInfoPage.Tab.Info && startingTab == ContactInfoPage.Tab.Calls)
        tabArray = new ContactInfoPage.Tab[2]
        {
          ContactInfoPage.Tab.Calls,
          ContactInfoPage.Tab.Info
        };
      else
        tabArray = new ContactInfoPage.Tab[2]
        {
          ContactInfoPage.Tab.Info,
          ContactInfoPage.Tab.Calls
        };
      ContactInfoPage.NextInstanceTabs = tabArray;
      WaUriParams uriParams = new WaUriParams();
      uriParams.AddString("jid", user.Jid);
      if (replacePage)
        uriParams.AddBool("PageReplace", replacePage);
      NavUtils.NavigateToPage(nameof (ContactInfoPage), uriParams);
    }

    private void InitPivot(ContactInfoPage.Tab[] tabs)
    {
      if (tabs == null || !((IEnumerable<ContactInfoPage.Tab>) tabs).Any<ContactInfoPage.Tab>())
        tabs = new ContactInfoPage.Tab[2]
        {
          ContactInfoPage.Tab.Info,
          ContactInfoPage.Tab.Calls
        };
      Dictionary<ContactInfoPage.Tab, PivotItem> tabsDict = new Dictionary<ContactInfoPage.Tab, PivotItem>();
      PivotHeaderConverter pivotHeaderConverter = new PivotHeaderConverter();
      Dictionary<ContactInfoPage.Tab, PivotItem> dictionary1 = tabsDict;
      PivotItem pivotItem1;
      if (!((IEnumerable<ContactInfoPage.Tab>) tabs).Contains<ContactInfoPage.Tab>(ContactInfoPage.Tab.Info))
      {
        pivotItem1 = (PivotItem) null;
      }
      else
      {
        pivotItem1 = new PivotItem();
        pivotItem1.Margin = new Thickness(0.0);
        pivotItem1.Header = (object) pivotHeaderConverter.Convert(AppResources.ProfileTitleUpper);
        ContactInfoTabView contactInfoTabView1 = new ContactInfoTabView();
        contactInfoTabView1.Margin = new Thickness(0.0, 6.0, 0.0, 0.0);
        ContactInfoTabView contactInfoTabView2 = contactInfoTabView1;
        this.infoTabView = contactInfoTabView1;
        pivotItem1.Content = (object) contactInfoTabView2;
      }
      PivotItem pivotItem2 = pivotItem1;
      this.infoPivotItem = pivotItem1;
      PivotItem pivotItem3 = pivotItem2;
      dictionary1[ContactInfoPage.Tab.Info] = pivotItem3;
      if (this.callsPivotItem == null && this.callLogView == null)
      {
        Dictionary<ContactInfoPage.Tab, PivotItem> dictionary2 = tabsDict;
        PivotItem pivotItem4;
        if (!((IEnumerable<ContactInfoPage.Tab>) tabs).Contains<ContactInfoPage.Tab>(ContactInfoPage.Tab.Calls))
        {
          pivotItem4 = (PivotItem) null;
        }
        else
        {
          pivotItem4 = new PivotItem();
          pivotItem4.Margin = new Thickness(0.0);
          pivotItem4.Header = (object) pivotHeaderConverter.Convert(AppResources.CallsHeader);
          ContactCallLogView contactCallLogView1 = new ContactCallLogView();
          contactCallLogView1.Margin = new Thickness(0.0, 6.0, 0.0, 0.0);
          ContactCallLogView contactCallLogView2 = contactCallLogView1;
          this.callLogView = contactCallLogView1;
          pivotItem4.Content = (object) contactCallLogView2;
        }
        PivotItem pivotItem5 = pivotItem4;
        this.callsPivotItem = pivotItem4;
        PivotItem pivotItem6 = pivotItem5;
        dictionary2[ContactInfoPage.Tab.Calls] = pivotItem6;
      }
      ((IEnumerable<ContactInfoPage.Tab>) tabs).Select<ContactInfoPage.Tab, PivotItem>((Func<ContactInfoPage.Tab, PivotItem>) (t =>
      {
        PivotItem pivotItem7 = (PivotItem) null;
        return !tabsDict.TryGetValue(t, out pivotItem7) ? (PivotItem) null : pivotItem7;
      })).Where<PivotItem>((Func<PivotItem, bool>) (pi => pi != null)).ToList<PivotItem>().ForEach((Action<PivotItem>) (pi => this.Pivot.Items.Add((object) pi)));
      TabHeaderControl tabHeaderControl = new TabHeaderControl(this.Pivot);
      tabHeaderControl.Margin = new Thickness(24.0, 0.0, 24.0, 0.0);
      tabHeaderControl.HorizontalAlignment = HorizontalAlignment.Stretch;
      TabHeaderControl element = tabHeaderControl;
      Grid.SetRow((FrameworkElement) element, 1);
      this.ContentPanel.Children.Add((UIElement) element);
    }

    private void Init(UserStatus user, Contact contact, System.Windows.Media.ImageSource startingPic)
    {
      this.pageData = new ContactInfoPageData();
      this.pageData.GetChatDataLoadedObservable().Take<Unit>(1).ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ => this.OnChatDataLoaded()));
      if (user != null)
        this.pageData.Load(user);
      else if (contact != null)
        this.pageData.Load(contact);
      this.DataContext = (object) (this.viewModel = new ContactInfoPageViewModel(this.pageData, this.Orientation));
      this.infoTabView.PresetPicture(startingPic, this.pageData);
      if (!VerifiedNameRules.IsApplicable(user))
        return;
      bool checkMark = false;
      VerifiedNameRules.GetFirstInfoName(user, out checkMark);
      this.VerifiedBadge.Source = (System.Windows.Media.ImageSource) AssetStore.InlineVerified;
      this.VerifiedBadge.Visibility = checkMark.ToVisibility();
    }

    private void CreateInfoTabAppBar(UserStatus[] knownWaAccounts)
    {
      if (((IEnumerable<UserStatus>) knownWaAccounts).Any<UserStatus>())
      {
        this.pinButton = new ApplicationBarIconButton()
        {
          IconUri = new Uri("/Images/pin-to-start.png", UriKind.Relative),
          Text = "CreateConversationTileShort"
        };
        this.pinButton.Click += new EventHandler(this.PinToStart_Click);
        this.appbar.Buttons.Add((object) this.pinButton);
        this.UpdatePinButton();
      }
      this.blockMenuItem = new ApplicationBarMenuItem()
      {
        Text = "Block"
      };
      this.blockMenuItem.Click += new EventHandler(this.Block_Click);
      this.unblockMenuItem = new ApplicationBarMenuItem()
      {
        Text = "UnblockContact"
      };
      this.unblockMenuItem.Click += new EventHandler(this.Block_Click);
      this.ResetAppBar();
      this.infoAppBar = this.appbar;
    }

    private void DisableAppBar()
    {
      if (this.pinButton != null)
        this.pinButton.IsEnabled = false;
      this.appbar.IsMenuEnabled = false;
    }

    private void ResetAppBar()
    {
      this.appbar.MenuItems.Clear();
      Contact contact = this.pageData?.Contact;
      bool flag = contact != null;
      if (!flag)
      {
        UserStatus targetWaAccount = this.pageData?.TargetWaAccount;
        if (targetWaAccount != null && !targetWaAccount.IsInDeviceContactList && targetWaAccount.IsVerified() && targetWaAccount.VerifiedLevel == VerifiedLevel.high)
          flag = true;
      }
      if (flag)
      {
        ApplicationBarMenuItem applicationBarMenuItem = new ApplicationBarMenuItem()
        {
          Text = "ShareContact"
        };
        applicationBarMenuItem.Click += new EventHandler(this.ShareContact_Click);
        this.appbar.MenuItems.Add((object) applicationBarMenuItem);
      }
      if (this.pageData != null && !this.pageData.HasWaAccount && contact != null && (contact.PhoneNumbers.Any<ContactPhoneNumber>() || contact.EmailAddresses.Any<ContactEmailAddress>()))
      {
        ApplicationBarMenuItem applicationBarMenuItem = new ApplicationBarMenuItem()
        {
          Text = "InviteToWAWithoutName"
        };
        applicationBarMenuItem.Click += new EventHandler(this.Invite_Click);
        this.appbar.MenuItems.Add((object) applicationBarMenuItem);
      }
      ApplicationBarMenuItem applicationBarMenuItem1 = new ApplicationBarMenuItem()
      {
        Text = "EmailChatHistory"
      };
      applicationBarMenuItem1.Click += new EventHandler(this.EmailChatHistory_Click);
      this.appbar.MenuItems.Add((object) applicationBarMenuItem1);
      ApplicationBarMenuItem applicationBarMenuItem2 = new ApplicationBarMenuItem()
      {
        Text = "ClearChatHistory"
      };
      applicationBarMenuItem2.Click += new EventHandler(this.ClearChatHistory_Click);
      this.appbar.MenuItems.Add((object) applicationBarMenuItem2);
      if (!ContactsContext.Instance<bool>((Func<ContactsContext, bool>) (cdb => this.pageData?.TargetWaAccount?.Jid != null && cdb.BlockListSet.ContainsKey(this.pageData?.TargetWaAccount?.Jid))))
      {
        this.appbar.MenuItems.Add((object) this.blockMenuItem);
        this.blockMenuItem.Text = "Block";
      }
      else
      {
        this.appbar.MenuItems.Add((object) this.unblockMenuItem);
        this.unblockMenuItem.Text = "UnblockContact";
      }
      ApplicationBarMenuItem applicationBarMenuItem3 = new ApplicationBarMenuItem()
      {
        Text = "ReportSpamButton"
      };
      applicationBarMenuItem3.Click += new EventHandler(this.ReportSpamButton_Click);
      this.appbar.MenuItems.Add((object) applicationBarMenuItem3);
      if (Settings.IsWaAdmin)
      {
        ApplicationBarMenuItem applicationBarMenuItem4 = new ApplicationBarMenuItem()
        {
          Text = "AddToGroup"
        };
        applicationBarMenuItem4.Click += new EventHandler(this.AddToGroup_Click);
        this.appbar.MenuItems.Add((object) applicationBarMenuItem4);
      }
      UserStatus user = this.pageData?.GetPrimaryWaAccount();
      UserStatus userStatus = user;
      if ((userStatus != null ? (!userStatus.IsInDeviceContactList ? 1 : 0) : 0) != 0 && VerifiedNameRules.IsApplicable(user))
      {
        ApplicationBarMenuItem createContact = new ApplicationBarMenuItem()
        {
          Text = "CreateNewContact"
        };
        ApplicationBarMenuItem addToExisting = new ApplicationBarMenuItem()
        {
          Text = "AddToExistingContact"
        };
        createContact.Click += (EventHandler) ((s, e) =>
        {
          AddContact.Launch(user.Jid, false);
          this.appbar.MenuItems.Remove((object) createContact);
          this.appbar.MenuItems.Remove((object) addToExisting);
        });
        addToExisting.Click += (EventHandler) ((sender, e) =>
        {
          AddContact.Launch(user.Jid, true);
          this.appbar.MenuItems.Remove((object) createContact);
          this.appbar.MenuItems.Remove((object) addToExisting);
        });
        this.appbar.MenuItems.Add((object) createContact);
        this.appbar.MenuItems.Add((object) addToExisting);
      }
      if (this.pinButton != null)
      {
        this.pinButton.Text = "CreateConversationTileShort";
        this.UpdatePinButton();
      }
      this.appbar.IsMenuEnabled = true;
      Localizable.LocalizeAppBar(this.appbar);
    }

    private void UpdatePinButton()
    {
      UserStatus primaryWaAccount = this.pageData?.GetPrimaryWaAccount();
      if (primaryWaAccount == null || this.pinButton == null)
        return;
      this.pinButton.IsEnabled = !TileHelper.ChatTileExists(primaryWaAccount.Jid);
    }

    private void SetEnteringAnimationTransforms()
    {
      System.Windows.Point point = this.infoTabView.ProfileImage.TransformToVisual((UIElement) this.LayoutRoot).Transform(new System.Windows.Point(0.0, 0.0));
      this.EnterTranslateY.From = new double?(point.Y);
      this.EnterTranslateY.To = new double?((this.LayoutRoot.ActualHeight - 480.0) / 2.0);
      this.EnterTranslateX.From = new double?(point.X);
      this.EnterTranslateX.To = new double?((this.LayoutRoot.ActualWidth - 480.0) / 2.0);
      double num = 1.0 / 3.0 / ResolutionHelper.ZoomFactor;
      this.EnterScaleX.From = new double?(num);
      this.EnterScaleY.From = new double?(num);
      this.ExitScaleX.To = new double?(num);
      this.ExitScaleY.To = new double?(num);
    }

    private void SetExitingAnimationTransforms()
    {
      System.Windows.Point point = this.infoTabView.ProfileImage.TransformToVisual((UIElement) this.LayoutRoot).Transform(new System.Windows.Point(0.0, 0.0));
      this.ExitTranslateY.From = new double?((this.LayoutRoot.ActualHeight - 480.0) / 2.0);
      this.ExitTranslateY.To = new double?(point.Y);
      this.ExitTranslateX.From = new double?((this.LayoutRoot.ActualWidth - 480.0) / 2.0);
      this.ExitTranslateX.To = new double?(point.X);
    }

    private void OnChatDataLoaded()
    {
      if (this.pageData == null || this.pageRemoved)
        return;
      UserStatus[] knownWaAccounts = this.pageData.GetKnownWaAccounts();
      this.CreateInfoTabAppBar(knownWaAccounts);
      if (this.currTab == ContactInfoPage.Tab.Info)
        this.ApplicationBar = (IApplicationBar) this.infoAppBar;
      if (((IEnumerable<UserStatus>) knownWaAccounts).Any<UserStatus>() || this.callsPivotItem == null)
        return;
      this.Pivot.Items.Remove((object) this.callsPivotItem);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      this.emailSub.SafeDispose();
      this.emailSub = (IDisposable) null;
      base.OnNavigatedFrom(e);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      bool flag = false;
      if (this.pageData == null && e.NavigationMode != NavigationMode.Reset)
      {
        string jid = (string) null;
        this.NavigationContext.QueryString.TryGetValue("jid", out jid);
        if (jid != null)
        {
          this.Init(UserCache.Get(jid, true), (Contact) null, (System.Windows.Media.ImageSource) null);
        }
        else
        {
          string id = (string) null;
          this.NavigationContext.QueryString.TryGetValue("contactId", out id);
          if (id != null)
            AddressBook.Instance.GetContactById(id).ObserveOnDispatcher<AddressBookSearchArgs>().Subscribe<AddressBookSearchArgs>((Action<AddressBookSearchArgs>) (args =>
            {
              Contact contact = args.Results.FirstOrDefault<Contact>();
              if (contact != null)
              {
                this.Init((UserStatus) null, contact, (System.Windows.Media.ImageSource) null);
              }
              else
              {
                if (this.pageRemoved)
                  return;
                NavUtils.GoBack(this.NavigationService);
              }
            }));
          else
            flag = true;
        }
      }
      base.OnNavigatedTo(e);
      if (flag)
      {
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
      }
      else
      {
        if (e.NavigationMode == NavigationMode.Reset)
          return;
        FieldStats.ReportUiUsage(wam_enum_ui_usage_type.PROFILE);
      }
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.pageRemoved = true;
      this.viewModel.SafeDispose();
      this.viewModel = (ContactInfoPageViewModel) null;
      this.infoTabView.SafeDispose();
      this.pageData.SafeDispose();
      this.pageData = (ContactInfoPageData) null;
      base.OnRemovedFromJournal(e);
    }

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      if (this.LargeImage.Visibility == Visibility.Visible)
      {
        this.EnterProfileImage.Stop();
        this.ApplicationBar = (IApplicationBar) this.infoAppBar;
        this.LargeImage.VerticalAlignment = VerticalAlignment.Top;
        this.LargeImage.HorizontalAlignment = HorizontalAlignment.Left;
        this.LargeImage.Height = 480.0;
        this.LargeImage.Width = 480.0;
        this.SetExitingAnimationTransforms();
        if (this.ExitProfileImage.GetCurrentState() == ClockState.Stopped)
          Storyboarder.Perform(this.ExitProfileImage, onComplete: (Action) (() =>
          {
            this.ContentPanel.Opacity = 1.0;
            this.ContentPanel.IsHitTestVisible = true;
            this.LargeImage.Visibility = Visibility.Collapsed;
          }));
        e.Cancel = true;
      }
      else
        base.OnBackKeyPress(e);
    }

    private void ProfilePic_Tap(object sender, EventArgs e)
    {
      if (this.pageData.LargeFormatPicSource == null)
        return;
      this.EnterProfileImage.Stop();
      this.LargeImage.ImageSource = (sender as Image).Source as BitmapSource;
      this.LargeImage.Visibility = Visibility.Visible;
      this.LargeImage.VerticalAlignment = VerticalAlignment.Top;
      this.LargeImage.HorizontalAlignment = HorizontalAlignment.Left;
      this.SetEnteringAnimationTransforms();
      if (this.saveImageAppBar == null)
      {
        Microsoft.Phone.Shell.ApplicationBar bar = new Microsoft.Phone.Shell.ApplicationBar();
        ApplicationBarIconButton applicationBarIconButton = new ApplicationBarIconButton()
        {
          IconUri = new Uri("/Images/icon-save.png", UriKind.Relative),
          Text = "Save"
        };
        applicationBarIconButton.Click += new EventHandler(this.SaveImage_Click);
        bar.Buttons.Add((object) applicationBarIconButton);
        bar.IsMenuEnabled = false;
        Localizable.LocalizeAppBar(bar);
        this.saveImageButton = applicationBarIconButton;
        this.saveImageAppBar = bar;
      }
      this.ApplicationBar = (IApplicationBar) this.saveImageAppBar;
      Storyboarder.Perform(this.EnterProfileImage, onComplete: (Action) (() =>
      {
        this.LargeImage.Width = double.NaN;
        this.LargeImage.Height = double.NaN;
        this.LargeImage.VerticalAlignment = VerticalAlignment.Stretch;
        this.LargeImage.HorizontalAlignment = HorizontalAlignment.Stretch;
        this.ContentPanel.Opacity = 0.0;
        this.ContentPanel.IsHitTestVisible = false;
      }));
    }

    private void InfoTab_ProfilePicOpened(object sender, EventArgs e)
    {
      if (this.saveImageAppBar == null)
      {
        Microsoft.Phone.Shell.ApplicationBar bar = new Microsoft.Phone.Shell.ApplicationBar();
        ApplicationBarIconButton applicationBarIconButton = new ApplicationBarIconButton()
        {
          IconUri = new Uri("/Images/icon-save.png", UriKind.Relative),
          Text = "Save"
        };
        applicationBarIconButton.Click += new EventHandler(this.SaveImage_Click);
        bar.Buttons.Add((object) applicationBarIconButton);
        bar.IsMenuEnabled = false;
        Localizable.LocalizeAppBar(bar);
        this.saveImageButton = applicationBarIconButton;
        this.saveImageAppBar = bar;
      }
      this.ApplicationBar = (IApplicationBar) this.saveImageAppBar;
    }

    private void PinToStart_Click(object sender, EventArgs e)
    {
      UserStatus primaryWaAccount = this.pageData.GetPrimaryWaAccount();
      if (primaryWaAccount == null || !PinToStart.Pin(primaryWaAccount))
        return;
      this.UpdatePinButton();
    }

    private void ShareContact_Click(object sender, EventArgs e)
    {
      if (this.pageData == null)
        return;
      IObservable<ContactVCard> source = (IObservable<ContactVCard>) null;
      UserStatus targetWaAccount = this.pageData.TargetWaAccount;
      if (targetWaAccount != null && !targetWaAccount.IsInDeviceContactList)
      {
        ContactVCard bizContactVcard = this.pageData.GetBizContactVCard();
        if (bizContactVcard != null)
          source = ShareContactPage.Start(bizContactVcard);
      }
      if (source == null && this.pageData.Contact != null)
        source = ShareContactPage.Start(this.pageData.Contact);
      if (source == null)
        return;
      source.Take<ContactVCard>(1).Where<ContactVCard>((Func<ContactVCard, bool>) (vcard => vcard != null)).ObserveOnDispatcher<ContactVCard>().Subscribe<ContactVCard>((Action<ContactVCard>) (vcard =>
      {
        Message msg = (Message) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => msg = vcard.ToMessage(db)));
        SendMessage.ChooseRecipientAndSendNew(new Message[1]
        {
          msg
        }, AppResources.ShareContactTitle);
      }));
    }

    private void Invite_Click(object sender, EventArgs e)
    {
      if (this.pageData == null)
        return;
      ContactInfoPageData.ContactInfoItem[] invitationItems = this.pageData.GetInvitationItems();
      int length = invitationItems.Length;
      if (length > 1)
      {
        this.InvitationPicker.FullModeHeader = (object) AppResources.InviteToWAWithoutName.ToUpper();
        this.InvitationPicker.SelectionChanged -= new SelectionChangedEventHandler(this.InvitiationPicker_SelectionChanged);
        List<ContactInfoPageData.ContactInfoItem> list = ((IEnumerable<ContactInfoPageData.ContactInfoItem>) invitationItems).ToList<ContactInfoPageData.ContactInfoItem>();
        ContactInfoPageData.ContactInfoItem contactInfoItem = new ContactInfoPageData.ContactInfoItem();
        list.Add(contactInfoItem);
        this.InvitationPicker.ItemsSource = (IEnumerable) list;
        this.InvitationPicker.SelectedItem = (object) contactInfoItem;
        this.InvitationPicker.SelectionChanged += new SelectionChangedEventHandler(this.InvitiationPicker_SelectionChanged);
        this.InvitationPicker.Open();
      }
      else
      {
        if (length != 1)
          return;
        this.Dispatcher.BeginInvoke((Action) (() => invitationItems[0].Act()));
      }
    }

    private void InvitiationPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (this.InvitationPicker.SelectedIndex == -1 || e.AddedItems.Count <= 0 || e.RemovedItems.Count <= 0)
        return;
      object addedItem = e.AddedItems[0];
      if (!(addedItem is ContactInfoPageData.ContactInfoItem))
        return;
      ((ContactInfoPageData.ContactInfoItem) addedItem).Act();
    }

    private void EmailChatHistory_Click(object sender, EventArgs e)
    {
      if (this.pageData.TargetWaAccount == null || this.emailSub != null)
        return;
      this.progressIndicator.Acquire();
      this.emailSub = EmailChatHistory.Create(this.pageData.TargetWaAccount.Jid).SubscribeOn<Unit>(WAThreadPool.Scheduler).ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ => { }), (Action) (() =>
      {
        this.progressIndicator.Release();
        this.emailSub.SafeDispose();
        this.emailSub = (IDisposable) null;
      }));
    }

    private void ClearChatHistory_Click(object sender, EventArgs e)
    {
      string[] array = ((IEnumerable<UserStatus>) this.pageData.GetKnownWaAccounts()).Select<UserStatus, string>((Func<UserStatus, string>) (u => u.Jid)).ToArray<string>();
      UserStatus targetWaAccount = this.pageData.TargetWaAccount;
      string jid = targetWaAccount == null ? (string) null : targetWaAccount.Jid;
      ClearChatPicker.Launch(array, jid).ObserveOnDispatcher<Unit>().Subscribe<Unit>();
    }

    private void Block_Click(object sender, EventArgs e) => this.ToggleBlock();

    private void ReportSpamButton_Click(object sender, EventArgs e)
    {
      Conversation convo = (Conversation) null;
      Message[] msgsToReport = (Message[]) null;
      UserStatus targetWaAccount = this.pageData.TargetWaAccount;
      string jidToBlock = targetWaAccount == null ? (string) null : targetWaAccount.Jid;
      if (jidToBlock == null)
      {
        Log.l("contact info", "ReportSpamButton_Click invoked with no jid - {0}", (object) (targetWaAccount == null));
      }
      else
      {
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          convo = db.GetConversation(jidToBlock, CreateOptions.None);
          if (convo == null)
            return;
          msgsToReport = db.GetLatestMessages(convo.Jid, convo.MessageLoadingStart(), new int?(5), new int?(0));
        }));
        this.ToggleBlock("account_info", msgsToReport);
      }
    }

    private void AddToGroup_Click(object sender, EventArgs e)
    {
      ((IEnumerable<UserStatus>) this.pageData.GetKnownWaAccounts()).Select<UserStatus, string>((Func<UserStatus, string>) (u => u.Jid)).ToArray<string>();
      UserStatus targetWaAccount = this.pageData.TargetWaAccount;
      string jid = targetWaAccount == null ? (string) null : targetWaAccount.Jid;
      if (jid == null)
        return;
      AddToGroupsPage.Start(jid);
    }

    private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (this.pageRemoved)
        return;
      if (this.infoPivotItem != null && e.AddedItems.Contains((object) this.infoPivotItem))
      {
        this.currTab = ContactInfoPage.Tab.Info;
        this.ApplicationBar = (IApplicationBar) this.infoAppBar;
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          if (this.pageRemoved)
            return;
          this.infoTabView.Show(this.pageData);
        }));
      }
      else
      {
        if (this.callsPivotItem == null || !e.AddedItems.Contains((object) this.callsPivotItem))
          return;
        this.currTab = ContactInfoPage.Tab.Calls;
        this.ApplicationBar = (IApplicationBar) null;
        if (this.pageData == null || this.pageData.TargetWaAccount == null || this.callLogView == null)
          return;
        this.Dispatcher.BeginInvoke((Action) (() => this.callLogView.Show(this.pageData.TargetWaAccount.Jid)));
      }
    }

    private void SaveImage_Click(object sender, EventArgs e)
    {
      if (!ChatPictureStore.SaveToPhone(this.pageData.PicJid))
      {
        int num = (int) MessageBox.Show(AppResources.SavePictureFailure);
      }
      this.saveImageButton.IsEnabled = false;
    }

    private void ToggleBlock(string reportSpamSource = "block_dialog", Message[] msgsToReport = null)
    {
      UserStatus targetWaAccount = this.pageData.TargetWaAccount;
      string jidToBlock = targetWaAccount?.Jid;
      if (jidToBlock == null)
      {
        Log.l("contact info", "ToggleBlock invoked with no jid - {0}", (object) (targetWaAccount == null));
      }
      else
      {
        this.progressIndicator.Acquire();
        this.DisableAppBar();
        bool exitedPage = false;
        BlockContact.ToggleBlock(jidToBlock, reportSpamSource).ObserveOnDispatcher<Pair<bool, bool>>().Subscribe<Pair<bool, bool>>((Action<Pair<bool, bool>>) (blockSelection => exitedPage = BlockContact.OnBlockDialogSelection(blockSelection, reportSpamSource, jidToBlock, msgsToReport)), (Action) (() =>
        {
          if (!exitedPage)
            this.progressIndicator.Release();
          this.ResetAppBar();
        }));
      }
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/ContactInfoPage.xaml", UriKind.Relative));
      this.EnterProfileImage = (Storyboard) this.FindName("EnterProfileImage");
      this.EnterTranslateX = (DoubleAnimation) this.FindName("EnterTranslateX");
      this.EnterTranslateY = (DoubleAnimation) this.FindName("EnterTranslateY");
      this.EnterScaleX = (DoubleAnimation) this.FindName("EnterScaleX");
      this.EnterScaleY = (DoubleAnimation) this.FindName("EnterScaleY");
      this.ExitProfileImage = (Storyboard) this.FindName("ExitProfileImage");
      this.ExitTranslateX = (DoubleAnimation) this.FindName("ExitTranslateX");
      this.ExitTranslateY = (DoubleAnimation) this.FindName("ExitTranslateY");
      this.ExitScaleX = (DoubleAnimation) this.FindName("ExitScaleX");
      this.ExitScaleY = (DoubleAnimation) this.FindName("ExitScaleY");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.ContentPanel = (Grid) this.FindName("ContentPanel");
      this.VerifiedBadge = (Image) this.FindName("VerifiedBadge");
      this.Pivot = (Pivot) this.FindName("Pivot");
      this.LargeImage = (ImageViewControl) this.FindName("LargeImage");
      this.InvitationPicker = (ListPicker) this.FindName("InvitationPicker");
    }

    public enum Tab
    {
      Info,
      Calls,
    }
  }
}
