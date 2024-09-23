// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaViewModels.MultiParticipantsChatViewModelBase
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WhatsApp.CommonOps;


namespace WhatsApp.WaViewModels
{
  public abstract class MultiParticipantsChatViewModelBase : PageViewModelBase
  {
    private Conversation convo_;
    private ObservableCollection<UserViewModel> participantViewModels_;
    private MultiParticipantsChatViewModelBase.ChatParticipantPicker participantPicker_;
    private ICommand launchPickerCommand_;
    private ICommand removeParticipantCommand_;
    private IDisposable propertyChangedSub_;

    public event EventHandler<MultiParticipantsChatViewModelBase.ParticipantAddedEventArgs> ParticipantAdded;

    protected void NotifyParticipantAdded(UserStatus user, UserViewModel userVM)
    {
      if (this.ParticipantAdded == null)
        return;
      this.ParticipantAdded((object) this, new MultiParticipantsChatViewModelBase.ParticipantAddedEventArgs(user, userVM));
    }

    public Conversation Conversation => this.convo_;

    public string ChatName
    {
      get
      {
        if (this.Conversation == null)
          return (string) null;
        string name = this.Conversation.GetName(true);
        if (this.Conversation.IsGroup() && (name == null || name == AppResources.GroupChatSubject))
          App.CurrentApp.Connection.SendGetGroupInfo(this.Conversation.Jid);
        return Emoji.ConvertToTextOnly(name, (byte[]) null);
      }
    }

    public int MaxNameLength => Settings.MaxGroupSubject;

    public ObservableCollection<UserViewModel> ParticipantViewModels
    {
      get
      {
        if (this.participantViewModels_ == null)
          this.participantViewModels_ = new ObservableCollection<UserViewModel>(((IEnumerable<UserStatus>) this.convo_.GetParticipants(false, false)).Select<UserStatus, UserViewModel>((Func<UserStatus, UserViewModel>) (u => new UserViewModel(u, false))));
        return this.participantViewModels_;
      }
    }

    public System.Windows.Media.ImageSource PictureSource => this.GetPictureSource();

    protected MultiParticipantsChatViewModelBase.ChatParticipantPicker ParticipantPicker
    {
      get
      {
        if (this.participantPicker_ == null)
          this.participantPicker_ = new MultiParticipantsChatViewModelBase.ChatParticipantPicker()
          {
            Conversation = this.convo_,
            OnParticipantPicked = (Action<UserStatus>) (user => this.AddParticipant(user, true))
          };
        return this.participantPicker_;
      }
    }

    public ICommand LaunchPickerCommand
    {
      get
      {
        return this.launchPickerCommand_ ?? (this.launchPickerCommand_ = (ICommand) new UIUtils.DelegateCommand(new Action<object>(this.LaunchParticipantPicker)));
      }
    }

    public ICommand RemoveParticipantCommand
    {
      get
      {
        return this.removeParticipantCommand_ ?? (this.removeParticipantCommand_ = (ICommand) new UIUtils.DelegateCommand(new Action<object>(this.RemoveParticipantCommandImpl)));
      }
    }

    public int MaxParticipants
    {
      get
      {
        return !this.convo_.IsGroup() ? Settings.MaxListRecipients : Settings.MaxGroupParticipants - 1;
      }
    }

    public Visibility AddParticipantButtonVisibility
    {
      get => (this.convo_.GetParticipantCount() < this.MaxParticipants).ToVisibility();
    }

    public virtual string AddParticipantButtonStr => AppResources.GroupAddParticipants;

    public virtual string ParticipantsStr => AppResources.GroupInfoParticipants;

    public string ParticipantsCountStr
    {
      get
      {
        return string.Format(AppResources.NthOutOfTotal, (object) this.convo_.GetParticipantCount(), (object) this.MaxParticipants);
      }
    }

    public string EditChatNameStr => AppResources.Edit.ToLangFriendlyLower();

    public MultiParticipantsChatViewModelBase(
      Conversation convo,
      PageOrientation initialOrientation)
      : base(initialOrientation)
    {
      this.convo_ = convo != null ? convo : throw new ArgumentNullException("multi parti chat view model ctor");
      if (this.convo_ == null)
        return;
      this.propertyChangedSub_ = convo.GetPropertyChangedAsync().Where<PropertyChangedEventArgs>((Func<PropertyChangedEventArgs, bool>) (a => a.PropertyName == "GroupSubject")).ObserveOnDispatcher<PropertyChangedEventArgs>().Subscribe<PropertyChangedEventArgs>((Action<PropertyChangedEventArgs>) (_ => this.NotifyPropertyChanged(nameof (ChatName))));
    }

    protected override void DisposeManagedResources()
    {
      this.propertyChangedSub_.SafeDispose();
      this.propertyChangedSub_ = (IDisposable) null;
      base.DisposeManagedResources();
    }

    private void RemoveParticipantCommandImpl(object obj)
    {
      if (!(obj is UserViewModel userViewModel) || userViewModel.User == null)
        return;
      this.RemoveParticipant(userViewModel.User);
    }

    public void LaunchParticipantPicker(object ignored = null) => this.ParticipantPicker.Launch();

    public void AddParticipant(UserStatus user, bool appendToEnd = false)
    {
      bool added = false;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        if (this.convo_.GetParticipantCount() >= this.MaxParticipants || !this.convo_.AddParticipant(db, user.Jid))
          return;
        added = true;
        bool isConvoInDb = db.GetConversation(this.convo_.Jid, CreateOptions.None) != null;
        this.OnParticipantAddSubmitting(db, user, isConvoInDb);
        db.AddParticipantsHistory(this.convo_, this.convo_.Jid, ParticipantsHashHistory.ParticipantActions.Added);
        if (!isConvoInDb)
          return;
        db.SubmitChanges();
      }));
      if (!added)
        return;
      this.OnParticipantAdded(user, appendToEnd);
    }

    public void RemoveParticipant(UserStatus user)
    {
      bool removed = false;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        if (!this.convo_.RemoveParticipant(db, user.Jid))
          return;
        removed = true;
        bool isConvoInDb = db.GetConversation(this.convo_.Jid, CreateOptions.None) != null;
        this.OnParticipantRemoveSubmitting(db, user, isConvoInDb);
        db.AddParticipantsHistory(this.convo_, this.convo_.Jid, ParticipantsHashHistory.ParticipantActions.Removed);
        if (!isConvoInDb)
          return;
        db.SubmitChanges();
      }));
      if (!removed)
        return;
      this.OnParticipantRemoved(user);
      AppState.GetConnection().Encryption.OnParticipantRemovedFromConversation(this.convo_.Jid);
    }

    protected virtual System.Windows.Media.ImageSource GetPictureSource() => (System.Windows.Media.ImageSource) null;

    protected virtual void OnParticipantAddSubmitting(
      MessagesContext db,
      UserStatus user,
      bool isConvoInDb)
    {
    }

    protected virtual void OnParticipantRemoveSubmitting(
      MessagesContext db,
      UserStatus user,
      bool isConvoInDb)
    {
    }

    protected void OnParticipantAdded(UserStatus user, bool appendToEnd = false)
    {
      UserViewModel userVM = new UserViewModel(user, false);
      if (appendToEnd)
        this.ParticipantViewModels.Add(userVM);
      else
        this.ParticipantViewModels.Insert(0, userVM);
      this.NotifyPropertyChanged("ParticipantsCountStr");
      this.NotifyPropertyChanged("AddParticipantButtonVisibility");
      this.NotifyParticipantAdded(user, userVM);
    }

    protected virtual void OnParticipantRemoved(UserStatus user)
    {
      int index = 0;
      foreach (UserViewModel participantViewModel in (Collection<UserViewModel>) this.ParticipantViewModels)
      {
        if (participantViewModel.User.Jid == user.Jid)
        {
          this.ParticipantViewModels.RemoveAt(index);
          this.NotifyPropertyChanged("ParticipantsCountStr");
          this.NotifyPropertyChanged("AddParticipantButtonVisibility");
          this.NotifyPropertyChanged("EncryptionStateStr");
          this.NotifyPropertyChanged("EncryptionStateIcon");
          break;
        }
        ++index;
      }
    }

    protected void InsertParticipantChangedMessage(
      MessagesContext db,
      string participantJid,
      SystemMessageUtils.ParticipantChange changeType)
    {
      Message participantChanged = SystemMessageUtils.CreateParticipantChanged(db, changeType, this.convo_.Jid, participantJid, Settings.MyJid, new DateTime?(FunRunner.CurrentServerTimeUtc));
      db.InsertMessageOnSubmit(participantChanged);
    }

    protected class ChatParticipantPicker
    {
      private WaContactsListTabData tabData;

      public Conversation Conversation { get; set; }

      public Action<UserStatus> OnParticipantPicked { get; set; }

      public void Launch()
      {
        string selfJid = Settings.MyJid;
        Func<string, IObservable<bool>> confirmSelectionFunc = (Func<string, IObservable<bool>>) (selJid =>
        {
          UserStatus userStatus = UserCache.Get(selJid, false);
          if (userStatus == null)
            return Observable.Return<bool>(false);
          userStatus.GetDisplayName();
          string promptBody = (string) null;
          if (this.Conversation.IsGroup())
            promptBody = AppResources.ConfirmUnblockAndAddToGroup;
          else if (this.Conversation.IsBroadcast())
            promptBody = AppResources.ConfirmUnblockAndAddToBroadcastList;
          return BlockContact.PromptUnblockIfBlocked(userStatus.Jid, promptBody);
        });
        ListTabData[] tabs = new ListTabData[1];
        WaContactsListTabData contactsListTabData1 = this.tabData;
        if (contactsListTabData1 == null)
        {
          WaContactsListTabData contactsListTabData2 = new WaContactsListTabData();
          contactsListTabData2.EnableCache = true;
          contactsListTabData2.ItemVisibleFilter = (Func<JidItemViewModel, bool>) (item => !this.Conversation.ContainsParticipant(item.Jid));
          contactsListTabData2.Header = (string) null;
          WaContactsListTabData contactsListTabData3 = contactsListTabData2;
          this.tabData = contactsListTabData2;
          contactsListTabData1 = contactsListTabData3;
        }
        tabs[0] = (ListTabData) contactsListTabData1;
        JidItemPickerPage.Start(tabs, AppResources.GroupAddParticipant, confirmSelectionFunc, true).ObserveOnDispatcher<List<string>>().Select<List<string>, List<UserStatus>>((Func<List<string>, List<UserStatus>>) (selJids =>
        {
          List<UserStatus> userStatusList = new List<UserStatus>();
          foreach (string selJid in selJids)
            userStatusList.Add(UserCache.Get(selJid, false));
          return userStatusList;
        })).Subscribe<List<UserStatus>>((Action<List<UserStatus>>) (users =>
        {
          foreach (UserStatus user in users)
          {
            if (user != null && !StringComparer.Ordinal.Equals(user.Jid, selfJid) && this.OnParticipantPicked != null)
              this.OnParticipantPicked(user);
          }
        }));
      }
    }

    public class ParticipantAddedEventArgs : EventArgs
    {
      public UserStatus AddedParticipant { get; private set; }

      public UserViewModel AddedParticipantViewModel { get; private set; }

      public ParticipantAddedEventArgs(
        UserStatus addedParticipant,
        UserViewModel addedParticipantViewModel)
      {
        this.AddedParticipant = addedParticipant;
        this.AddedParticipantViewModel = addedParticipantViewModel;
      }
    }
  }
}
