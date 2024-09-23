// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactInfoTabViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WhatsApp.WaCollections;
using WhatsApp.WaViewModels;
using WhatsAppNative;


namespace WhatsApp
{
  public class ContactInfoTabViewModel : WaViewModelBase
  {
    private System.Windows.Media.ImageSource picSrc;
    private string picPhotoId;
    private System.Windows.Media.ImageSource initialPicSrc;
    private IDisposable picSub;
    private ContactInfoTabViewModel.WaNumberViewModel[] waNumberVms;
    private ContactInfoPageData data;
    private IDisposable bizDataLoadedSub;
    private IDisposable chatDataLoadedSub;
    private IDisposable jidInfoUpdatedSub;

    public static bool IsVoipEnabled { set; get; }

    public System.Windows.Media.ImageSource ProfilePicSource
    {
      get
      {
        if (this.picSrc == null && this.picSub == null)
          this.InitContactPictureSubscription(this.data);
        return this.picSrc ?? this.initialPicSrc ?? (System.Windows.Media.ImageSource) AssetStore.DefaultContactIcon;
      }
    }

    public Visibility WarningVisibility => this.data.IsAllDataFromFacebook.ToVisibility();

    public string WarningStr
    {
      get => !this.data.IsAllDataFromFacebook ? (string) null : AppResources.NoDetailsFromFacebook;
    }

    public double ZoomFactor => ResolutionHelper.ZoomFactor;

    public List<ChatItemViewModel> CommonGroupsSource => new List<ChatItemViewModel>();

    public IEnumerable<ContactInfoTabViewModel.WaNumberViewModel> WaNumberListSource
    {
      get
      {
        UserStatus[] knownWaAccounts = this.data.GetKnownWaAccounts();
        bool flag = this.waNumberVms == null || this.waNumberVms.Length != knownWaAccounts.Length;
        if (!flag)
        {
          Set<string> jids = new Set<string>(((IEnumerable<UserStatus>) knownWaAccounts).Select<UserStatus, string>((Func<UserStatus, string>) (u => u.Jid)));
          flag = ((IEnumerable<ContactInfoTabViewModel.WaNumberViewModel>) this.waNumberVms).Any<ContactInfoTabViewModel.WaNumberViewModel>((Func<ContactInfoTabViewModel.WaNumberViewModel, bool>) (vm => !jids.Contains(vm.Jid)));
        }
        if (flag)
          this.waNumberVms = ((IEnumerable<UserStatus>) knownWaAccounts).Where<UserStatus>((Func<UserStatus, bool>) (u => u != null)).Select<UserStatus, ContactInfoTabViewModel.WaNumberViewModel>((Func<UserStatus, ContactInfoTabViewModel.WaNumberViewModel>) (u => new ContactInfoTabViewModel.WaNumberViewModel(u, this.data.TargetWaAccount != null && this.data.TargetWaAccount.Jid == u.Jid))).ToArray<ContactInfoTabViewModel.WaNumberViewModel>();
        return (IEnumerable<ContactInfoTabViewModel.WaNumberViewModel>) this.waNumberVms ?? (IEnumerable<ContactInfoTabViewModel.WaNumberViewModel>) new ContactInfoTabViewModel.WaNumberViewModel[0];
      }
    }

    public IEnumerable<ContactInfoPageData.ContactInfoItem> ActionListSource
    {
      get => this.data.GetActionableItems();
    }

    public IEnumerable<ContactInfoPageData.ContactInfoItem> InfoListSource
    {
      get => this.data.GetInfoItems();
    }

    public ContactInfoPageData Data => this.data;

    public ContactInfoTabViewModel(ContactInfoPageData pageData, System.Windows.Media.ImageSource initialPic = null)
    {
      this.data = pageData;
      this.initialPicSrc = initialPic;
      this.jidInfoUpdatedSub = this.data.GetJidInfosUpdatedObservable().Subscribe<Unit>((Action<Unit>) (_ => this.Notify("JidInfoUpdated")));
      this.bizDataLoadedSub = this.data.GetBizDataUpdatedObservable().Take<Unit>(1).ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ =>
      {
        if (this.data.TargetWaAccount == null)
          return;
        this.Notify("BizDataUpdated");
      }));
      this.chatDataLoadedSub = this.data.GetChatDataLoadedObservable().Take<Unit>(1).ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ =>
      {
        if (this.data.TargetWaAccount == null)
        {
          if (this.picSrc == null)
            this.NotifyPropertyChanged(nameof (ProfilePicSource));
          this.NotifyPropertyChanged(nameof (WaNumberListSource));
          this.NotifyPropertyChanged(nameof (WarningStr));
          this.NotifyPropertyChanged(nameof (WarningVisibility));
          this.NotifyPropertyChanged(nameof (ActionListSource));
        }
        else
        {
          if (((IEnumerable<UserStatus>) this.data.GetKnownWaAccounts()).Any<UserStatus>((Func<UserStatus, bool>) (waAcct => waAcct.Jid != this.data.TargetWaAccount.Jid)))
            this.NotifyPropertyChanged(nameof (WaNumberListSource));
          if (this.data.Contact != null)
          {
            this.NotifyPropertyChanged(nameof (ActionListSource));
            this.NotifyPropertyChanged(nameof (InfoListSource));
          }
        }
        this.Notify("ChatDataLoaded");
      }));
    }

    public void InitContactPictureSubscription(ContactInfoPageData data)
    {
      if (this.picSub != null)
        return;
      Contact contact = data.Contact;
      UserStatus primaryWaAccount = data.GetPrimaryWaAccount();
      string jid = primaryWaAccount == null ? (string) null : primaryWaAccount.Jid;
      if (jid == null)
        this.picSub = Observable.Create<System.Windows.Media.ImageSource>((Func<IObserver<System.Windows.Media.ImageSource>, Action>) (observer =>
        {
          Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
          {
            Stream picture = contact == null ? (Stream) null : contact.GetPicture();
            if (picture == null)
              return;
            using (picture)
            {
              BitmapImage bitmapImage = new BitmapImage()
              {
                CreateOptions = BitmapCreateOptions.BackgroundCreation
              };
              bitmapImage.SetSource(picture);
              observer.OnNext((System.Windows.Media.ImageSource) bitmapImage);
              observer.OnCompleted();
            }
          }));
          return (Action) (() => { });
        })).ObserveOnDispatcher<System.Windows.Media.ImageSource>().Subscribe<System.Windows.Media.ImageSource>((Action<System.Windows.Media.ImageSource>) (imgSrc =>
        {
          this.picSrc = imgSrc;
          this.initialPicSrc = (System.Windows.Media.ImageSource) null;
          this.picPhotoId = (string) null;
          this.data.LargeFormatPicSource = (System.Windows.Media.ImageSource) null;
          this.NotifyPropertyChanged("ProfilePicSource");
        }));
      else
        this.picSub = ChatPictureStore.Get(jid, true, true, true).SubscribeOn<ChatPictureStore.PicState>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<ChatPictureStore.PicState>().Subscribe<ChatPictureStore.PicState>((Action<ChatPictureStore.PicState>) (picState =>
        {
          if ((this.picPhotoId != picState.PhotoId || this.picSrc == null != (picState.Image == null) ? 1 : (this.data.LargeFormatPicSource == null == (picState.Image == null) ? 0 : (picState.IsLargeFormat ? 1 : 0))) == 0)
          {
            Log.d("Contact tab", "Ignoring photo update as no change was detected {0}", (object) (picState.PhotoId ?? "null"));
          }
          else
          {
            this.initialPicSrc = (System.Windows.Media.ImageSource) null;
            if (this.picPhotoId != picState.PhotoId)
            {
              this.picSrc = (System.Windows.Media.ImageSource) null;
              this.data.LargeFormatPicSource = (System.Windows.Media.ImageSource) null;
            }
            this.picSrc = (System.Windows.Media.ImageSource) picState.Image;
            this.picPhotoId = picState.PhotoId;
            if (picState.IsLargeFormat)
            {
              this.data.PicJid = jid;
              this.data.LargeFormatPicSource = this.picSrc;
            }
            this.NotifyPropertyChanged("ProfilePicSource");
          }
        }));
    }

    protected override void DisposeManagedResources()
    {
      this.chatDataLoadedSub.SafeDispose();
      this.chatDataLoadedSub = (IDisposable) null;
      this.jidInfoUpdatedSub.SafeDispose();
      this.jidInfoUpdatedSub = (IDisposable) null;
      this.picSub.SafeDispose();
      this.picSub = (IDisposable) null;
      base.DisposeManagedResources();
    }

    public string[] GetJids()
    {
      return ((IEnumerable<UserStatus>) this.data.GetKnownWaAccounts()).Select<UserStatus, string>((Func<UserStatus, string>) (u => u.Jid)).ToArray<string>();
    }

    public class WaNumberViewModel : WaViewModelBase
    {
      private UserStatus user;
      private bool shouldHighlight;
      private bool? callButtonEnabled;
      private IDisposable voipCallSub;

      public UserStatus WaAccount => this.user;

      public string Jid => this.user != null ? this.user.Jid : (string) null;

      public string NumberStr
      {
        get => this.user != null ? JidHelper.GetPhoneNumber(this.user.Jid, true) : (string) null;
      }

      public string NumberKindStr
      {
        get => this.user != null ? this.user.PhoneNumberKind.ToLocalizedString() : (string) null;
      }

      public Brush NumberBrush
      {
        get
        {
          return !this.shouldHighlight ? (Brush) UIUtils.ForegroundBrush : (Brush) UIUtils.AccentBrush;
        }
      }

      public bool CallButtonEnabled
      {
        get
        {
          if (!this.callButtonEnabled.HasValue)
          {
            this.voipCallSub = VoipHandler.CallStateChangedSubject.ObserveOnDispatcher<WaCallStateChangedArgs>().Subscribe<WaCallStateChangedArgs>((Action<WaCallStateChangedArgs>) (state => this.CallButtonEnabled = state.CurrState == UiCallState.None));
            this.callButtonEnabled = new bool?(!Voip.IsInCall);
          }
          return this.callButtonEnabled.Value;
        }
        private set
        {
          bool? callButtonEnabled = this.callButtonEnabled;
          bool flag = value;
          if ((callButtonEnabled.GetValueOrDefault() == flag ? (!callButtonEnabled.HasValue ? 1 : 0) : 1) == 0)
            return;
          this.callButtonEnabled = new bool?(value);
          this.NotifyPropertyChanged(nameof (CallButtonEnabled));
        }
      }

      public string StatusTitleStr => AppResources.RevivedStatusV2Lower;

      public RichTextBlock.TextSet StatusStr
      {
        get
        {
          if (this.user == null)
            return (RichTextBlock.TextSet) null;
          return new RichTextBlock.TextSet()
          {
            Text = this.user.Status
          };
        }
      }

      public string StatusDateStr
      {
        get
        {
          return this.user == null || !this.user.DateTimeSet.HasValue || this.user.DateTimeSet.Value > FunRunner.CurrentServerTimeUtc ? (string) null : DateTimeUtils.FormatTimeSince(this.user.DateTimeSet.Value);
        }
      }

      public Visibility StatusVisibility
      {
        get => (!string.IsNullOrEmpty(this.StatusDateStr)).ToVisibility();
      }

      public WaNumberViewModel(UserStatus user, bool highlight)
      {
        this.user = user;
        this.shouldHighlight = highlight;
      }

      protected override void DisposeManagedResources()
      {
        this.voipCallSub.SafeDispose();
        this.voipCallSub = (IDisposable) null;
        base.DisposeManagedResources();
      }
    }
  }
}
