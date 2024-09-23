// Decompiled with JetBrains decompiler
// Type: WhatsApp.GroupCreationPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using WhatsApp.CommonOps;
using WhatsApp.WaCollections;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class GroupCreationPage : PhoneApplicationPage
  {
    private static List<string> nextInstanceInitialParticipantJids;
    private List<string> initialParticipantJids;
    private bool isPageLoaded;
    private GroupCreationPageViewModel viewModel;
    private IDisposable groupCreationSub;
    internal Grid LayoutRoot;
    internal Image GroupImage;
    internal EmojiTextBox GroupNameBox;
    internal Grid GroupDescriptionBlock;
    internal EmojiTextBox GroupDescriptionBox;
    internal WhatsApp.CompatibilityShims.LongListSelector ParticipantsList;
    private bool _contentLoaded;

    private ObservableCollection<UserViewModel> participantVms { get; set; }

    private byte[] groupIconBytes { get; set; }

    private byte[] groupIconThumbBytes { get; set; }

    public GroupCreationPage()
    {
      this.InitializeComponent();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.initialParticipantJids = GroupCreationPage.nextInstanceInitialParticipantJids;
      GroupCreationPage.nextInstanceInitialParticipantJids = (List<string>) null;
      this.viewModel = new GroupCreationPageViewModel(new Conversation("tmp_group@g.us"), this.Orientation);
      this.DataContext = (object) this.viewModel;
      this.GroupNameBox.MaxLength = 25;
      this.GroupDescriptionBox.MaxLength = Settings.GroupDescriptionLength;
      if (Settings.GroupDescriptionLength > 0)
        this.GroupDescriptionBlock.Visibility = Visibility.Visible;
      this.GroupDescriptionBlock.Visibility = Visibility.Collapsed;
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
    }

    public static void Start(List<string> jids, bool replacePage)
    {
      GroupCreationPage.nextInstanceInitialParticipantJids = jids;
      WaUriParams uriParams = (WaUriParams) null;
      if (replacePage)
      {
        uriParams = new WaUriParams();
        uriParams.AddBool("PageReplace", true);
      }
      NavUtils.NavigateToPage(nameof (GroupCreationPage), uriParams);
    }

    private static IObservable<Pair<string, GroupDescription>> CreateGroupWithMembersAsync(
      string subject,
      GroupDescription description,
      IEnumerable<string> participantJids,
      GroupProperties props = null)
    {
      return Observable.Create<Pair<string, GroupDescription>>((Func<IObserver<Pair<string, GroupDescription>>, Action>) (observer =>
      {
        App.CurrentApp.Connection.SendCreateGroupChat(subject, participantJids, (Action<string, GroupDescription, List<string>, List<Pair<string, int>>>) ((groupJid, desc, addedParticipants, failedParticipants) =>
        {
          GroupCreationPage.OnAddParticipantsErrors(failedParticipants);
          observer.OnNext(new Pair<string, GroupDescription>(groupJid, desc));
          observer.OnCompleted();
          observer.OnCompleted();
        }), (Action<int>) (errorCode =>
        {
          Log.WriteLineDebug("group creation: failed | err={0}", (object) errorCode);
          if (errorCode == 406)
            AppState.ClientInstance.ShowMessageBox(string.Format(AppResources.SubjectChangedFailTooLong, (object) subject));
          else
            AppState.ClientInstance.ShowMessageBox(string.Format(AppResources.GroupCreateFail, (object) subject));
          observer.OnCompleted();
        }), description: description, properties: props);
        return (Action) (() => { });
      }));
    }

    public static IObservable<Dictionary<string, int>> AddMembersAsync(
      string groupJid,
      IEnumerable<string> participantJids)
    {
      return Observable.Create<Dictionary<string, int>>((Func<IObserver<Dictionary<string, int>>, Action>) (observer =>
      {
        App.CurrentApp.Connection.SendAddParticipants(groupJid, participantJids, (Action<List<Pair<string, int>>>) (failedParticipants =>
        {
          GroupCreationPage.OnAddParticipantsErrors(failedParticipants);
          Dictionary<string, int> dictionary = new Dictionary<string, int>();
          foreach (Pair<string, int> failedParticipant in failedParticipants)
            dictionary[failedParticipant.First] = failedParticipant.Second;
          observer.OnNext(dictionary);
          observer.OnCompleted();
        }), (Action<int>) (errCode =>
        {
          GroupCreationPage.OnAddParticipantsRequestError(errCode);
          observer.OnCompleted();
        }));
        return (Action) (() => { });
      }));
    }

    public static IObservable<Dictionary<string, int>> AddMemberAsync(
      string groupJid,
      string participantJid)
    {
      return GroupCreationPage.AddMembersAsync(groupJid, (IEnumerable<string>) new string[1]
      {
        participantJid
      });
    }

    private static void OnAddParticipantsRequestError(int errCode)
    {
      string str;
      switch (errCode)
      {
        case 401:
        case 403:
          str = AppResources.GroupAddParticipantsFailNotAdmin;
          break;
        case 404:
          str = AppResources.GroupAddParticipantsFailGroupEnded;
          break;
        default:
          str = string.Format("{0} {1}", (object) AppResources.GroupAddParticipantFailGeneric, (object) AppResources.TryAgainGeneric);
          break;
      }
      if (str == null)
        return;
      AppState.ClientInstance.ShowMessageBox(str);
    }

    private static void OnAddParticipantsErrors(List<Pair<string, int>> failedParticipants)
    {
      string errMsg = (string) null;
      int count = failedParticipants.Count;
      if (count == 1)
      {
        Pair<string, int> pair = failedParticipants.FirstOrDefault<Pair<string, int>>();
        if (pair != null)
          errMsg = GroupCreationPage.GetAddParticipantErrorMessage(pair.Second, pair.First);
      }
      else if (count > 1)
      {
        failedParticipants = failedParticipants.Where<Pair<string, int>>((Func<Pair<string, int>, bool>) (p => p.Second != 409)).ToList<Pair<string, int>>();
        if (failedParticipants.Any<Pair<string, int>>())
        {
          errMsg = string.Format(AppResources.GroupAddParticipantsFail, (object) string.Join(",", failedParticipants.Select<Pair<string, int>, string>((Func<Pair<string, int>, string>) (p => JidHelper.GetDisplayNameForContactJid(p.First)))));
          if (!failedParticipants.Any<Pair<string, int>>((Func<Pair<string, int>, bool>) (p => p.Second == 401 || p.Second == 500)))
            errMsg = string.Format("{0} {1}", (object) errMsg, (object) AppResources.TryAgainGeneric);
        }
      }
      if (errMsg == null)
        return;
      AppState.ClientInstance.ShowErrorMessage(errMsg, true);
    }

    private static string GetAddParticipantErrorMessage(int errCode, string name)
    {
      string participantErrorMessage = (string) null;
      switch (errCode)
      {
        case 401:
          participantErrorMessage = string.Format(AppResources.GroupAddParticipantFail, (object) JidHelper.GetDisplayNameForContactJid(name));
          goto case 409;
        case 406:
          participantErrorMessage = string.Format(AppResources.GroupAddParticipantFailBizUser, (object) JidHelper.GetDisplayNameForContactJid(name));
          goto case 409;
        case 408:
          participantErrorMessage = string.Format(AppResources.GroupAddParticipantFailLeftRecently, (object) JidHelper.GetDisplayNameForContactJid(name));
          goto case 409;
        case 409:
          return participantErrorMessage;
        case 500:
          participantErrorMessage = string.Format(AppResources.GroupAddParticipantFailGroupFull, (object) JidHelper.GetDisplayNameForContactJid(name));
          goto case 409;
        default:
          participantErrorMessage = string.Format("{0} {1}", (object) string.Format(AppResources.GroupAddParticipantFail, (object) JidHelper.GetDisplayNameForContactJid(name)), (object) AppResources.TryAgainGeneric);
          goto case 409;
      }
    }

    private static void OnMakeAdminRequestError(int errCode)
    {
      string str;
      switch (errCode)
      {
        case 401:
        case 403:
          str = AppResources.GroupPromoteParticipantFailNotAdmin;
          break;
        case 404:
          str = AppResources.GroupPromoteParticipantFailGroupEnded;
          break;
        default:
          str = string.Format("{0} {1}", (object) AppResources.GroupPromoteParticipantFailGeneric, (object) AppResources.TryAgainGeneric);
          break;
      }
      if (str == null)
        return;
      AppState.ClientInstance.ShowMessageBox(str);
    }

    private static void OnMakeAdminParticipantsErrors(List<Pair<string, int>> failedParticipants)
    {
      string str = (string) null;
      if (failedParticipants.Count == 1)
      {
        Pair<string, int> pair = failedParticipants.FirstOrDefault<Pair<string, int>>();
        if (pair != null)
          str = pair.Second != 404 ? string.Format("{0} {1}", (object) string.Format(AppResources.GroupRemoveParticipantFail, (object) JidHelper.GetDisplayNameForContactJid(pair.First)), (object) AppResources.TryAgainGeneric) : string.Format(AppResources.GroupPromoteParticipantFailNoLongerParticipant, (object) JidHelper.GetDisplayNameForContactJid(pair.First));
      }
      if (str == null)
        return;
      AppState.ClientInstance.ShowMessageBox(str);
    }

    private void CreateGroup()
    {
      if (this.groupCreationSub != null)
        return;
      string groupName = (this.GroupNameBox.Text ?? "").Trim();
      if (string.IsNullOrEmpty(groupName))
      {
        int num1 = (int) MessageBox.Show(AppResources.NoSubject);
      }
      else
      {
        string[] array = this.participantVms.Select<UserViewModel, string>((Func<UserViewModel, string>) (vm => vm.Jid)).ToArray<string>();
        if (!((IEnumerable<string>) array).Any<string>())
        {
          int num2 = (int) MessageBox.Show(AppResources.NoInvitees);
        }
        else
        {
          GroupDescription description = (GroupDescription) null;
          if (!string.IsNullOrEmpty(this.GroupDescriptionBox.Text))
            description = new GroupDescription(this.GroupDescriptionBox.Text);
          IObservable<Pair<string, GroupDescription>> withMembersAsync = GroupCreationPage.CreateGroupWithMembersAsync(groupName, description, (IEnumerable<string>) array);
          Action release = (Action) (() =>
          {
            release = (Action) (() => { });
            this.groupCreationSub.SafeDispose();
            this.groupCreationSub = (IDisposable) null;
          });
          this.groupCreationSub = withMembersAsync.ObserveOnDispatcher<Pair<string, GroupDescription>>().Do<Pair<string, GroupDescription>>((Action<Pair<string, GroupDescription>>) (result =>
          {
            release();
            App currentApp = App.CurrentApp;
            string groupJid = result.First;
            GroupDescription groupDescription = result.Second;
            if (this.groupIconThumbBytes != null && this.groupIconBytes != null)
              SetChatPhoto.Set(groupJid, this.groupIconThumbBytes, this.groupIconBytes, false);
            MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
            {
              Conversation conversation = db.GetConversation(groupJid, CreateOptions.CreateToDbIfNotFound);
              conversation.GroupSubject = groupName;
              conversation.GroupSubjectOwner = conversation.GroupOwner = Settings.MyJid;
              conversation.GroupCreationT = new DateTime?(DateTime.UtcNow);
              if (groupDescription != null && !groupDescription.Error.HasValue && !string.IsNullOrEmpty(groupDescription.Body))
              {
                conversation.GroupDescription = groupDescription.Body;
                conversation.GroupDescriptionId = groupDescription.Id;
                conversation.GroupDescriptionT = conversation.GroupCreationT;
                conversation.GroupDescriptionOwner = Settings.MyJid;
              }
              db.SubmitChanges();
            }));
            NavUtils.NavigateToChat(this.NavigationService, groupJid, false);
          }), (Action<Exception>) (ex => release()), (Action) (() => release())).Subscribe<Pair<string, GroupDescription>>();
        }
      }
    }

    private void OnLoaded(object sender, EventArgs e)
    {
      if (this.isPageLoaded)
        return;
      this.isPageLoaded = true;
      if (this.initialParticipantJids != null)
      {
        List<UserStatus> users = (List<UserStatus>) null;
        ContactsContext.Instance((Action<ContactsContext>) (db => users = db.GetUserStatuses((IEnumerable<string>) this.initialParticipantJids, true, false)));
        if (users != null)
          this.participantVms = new ObservableCollection<UserViewModel>(users.Select<UserStatus, UserViewModel>((Func<UserStatus, UserViewModel>) (u => new UserViewModel(u, false))));
      }
      this.ParticipantsList.ItemsSource = (IList) this.participantVms;
    }

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      e.Cancel = true;
      RecipientListPage.StartContactPicker(AppResources.GroupAddParticipants, (IEnumerable<string>) this.participantVms.Select<UserViewModel, string>((Func<UserViewModel, string>) (vm => vm.Jid)).ToArray<string>(), false, (Brush) UIUtils.SelectionBrush, keepPageOnSubmit: true).ObserveOnDispatcher<RecipientListPage.RecipientListResults>().Subscribe<RecipientListPage.RecipientListResults>((Action<RecipientListPage.RecipientListResults>) (recipientListResults =>
      {
        List<string> selectedJids = recipientListResults?.SelectedJids;
        if (selectedJids == null || !selectedJids.Any<string>())
          NavUtils.GoBack(this.NavigationService);
        else
          Utils.UpdateInPlace<UserViewModel, string>((IList<UserViewModel>) this.participantVms, (IList<string>) selectedJids, (Func<UserViewModel, string>) (vm => vm.Jid), (Func<string, string>) (jid => jid), (Func<string, UserViewModel>) (jid => new UserViewModel(UserCache.Get(jid, true))), (Action<UserViewModel>) null);
      }));
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
      if (e.NavigationMode == NavigationMode.Back)
        ProfilePictureChooserPage.ClearPopup();
      this.GroupNameBox.CloseEmojiKeyboard();
      this.GroupDescriptionBox.CloseEmojiKeyboard();
      base.OnNavigatingFrom(e);
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.viewModel.SafeDispose();
      base.OnRemovedFromJournal(e);
    }

    protected override void OnOrientationChanged(OrientationChangedEventArgs e)
    {
      this.viewModel.Orientation = e.Orientation;
      base.OnOrientationChanged(e);
    }

    private void EditGroupPic_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.GroupImage.Source != null)
      {
        System.Windows.Point point = this.GroupImage.TransformToVisual(Application.Current.RootVisual).Transform(new System.Windows.Point(0.0, 0.0));
        ProfilePictureChooserPage.PlayEntranceAnimation(this.GroupImage, point, point, this.Orientation, false);
      }
      IDisposable d = (IDisposable) null;
      d = ProfilePictureChooserPage.Start((string) null, this.GroupNameBox.Text, this.viewModel.PictureSource).Subscribe<ProfilePictureChooserPage.ProfilePictureChooserArgs>((Action<ProfilePictureChooserPage.ProfilePictureChooserArgs>) (args =>
      {
        Application current = Application.Current;
        this.groupIconBytes = args.GroupImage;
        this.groupIconThumbBytes = args.GroupThumb;
        this.viewModel.SetPictureSource(args.GroupThumbSource);
        d.SafeDispose();
      }));
    }

    private void SubmitButton_Click(object sender, EventArgs e) => this.CreateGroup();

    private void Background_Tap(object sender, EventArgs e)
    {
      this.GroupNameBox.CloseEmojiKeyboard();
      this.GroupDescriptionBox.CloseEmojiKeyboard();
    }

    private void NameBox_Tap(object sender, EventArgs e)
    {
      this.GroupDescriptionBox.CloseEmojiKeyboard();
    }

    private void DescriptionBox_Tap(object sender, EventArgs e)
    {
      this.GroupNameBox.CloseEmojiKeyboard();
    }

    private void RemoveParticipant_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (!((sender is FrameworkElement frameworkElement ? frameworkElement.DataContext : (object) null) is UserViewModel dataContext))
        return;
      this.participantVms.Remove(dataContext);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/GroupCreationPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.GroupImage = (Image) this.FindName("GroupImage");
      this.GroupNameBox = (EmojiTextBox) this.FindName("GroupNameBox");
      this.GroupDescriptionBlock = (Grid) this.FindName("GroupDescriptionBlock");
      this.GroupDescriptionBox = (EmojiTextBox) this.FindName("GroupDescriptionBox");
      this.ParticipantsList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("ParticipantsList");
    }
  }
}
