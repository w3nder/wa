// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaViewModels.ProfilePageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using WhatsApp.CommonOps;

#nullable disable
namespace WhatsApp.WaViewModels
{
  public class ProfilePageViewModel : PageViewModelBase
  {
    private UserStatus self_;
    private IDisposable statusChangedSub_;
    private IDisposable statusPendingSub_;
    private bool? isStatusPending_;
    private bool isProfilePicPending_;
    private IDisposable selfImageFetchSub_;
    private System.Windows.Media.ImageSource profilePicSource_;
    private bool isSIPUp_;

    public string PageSmallTitle => AppResources.SettingsTitle;

    public string PageLargeTitle => AppResources.ProfileTitle;

    public string NameTooltip => AppResources.NameInstruction;

    public string StatusHeader => AppResources.RevivedStatusV2UPPER;

    public string PhoneNumberHeader => AppResources.PhoneNumberUPPER;

    public string PhoneNumberStr => JidHelper.GetPhoneNumber(Settings.MyJid, true);

    public override Thickness PageMargin
    {
      get
      {
        return !this.isSIPUp_ || !this.Orientation.IsLandscape() ? new Thickness(0.0) : new Thickness(0.0, -160.0, 0.0, 0.0);
      }
    }

    public Visibility StatusProgressBarVisibility
    {
      get
      {
        if (!this.isStatusPending_.HasValue)
          Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
          {
            string pendingStatus = (string) null;
            this.isStatusPending_ = new bool?(SetStatus.TryGetPendingStatus(out pendingStatus) && pendingStatus == this.CurrentStatus.Text);
            this.NotifyPropertyChanged(nameof (StatusProgressBarVisibility));
            if (!this.isStatusPending_.Value || this.statusPendingSub_ != null)
              return;
            this.statusPendingSub_ = Observable.Timer(TimeSpan.FromMilliseconds(1000.0)).Repeat<long>().ObserveOnDispatcher<long>().Subscribe<long>((Action<long>) (_ =>
            {
              this.isStatusPending_ = new bool?(SetStatus.TryGetPendingStatus(out pendingStatus) && pendingStatus == this.CurrentStatus.Text);
              this.NotifyPropertyChanged(nameof (StatusProgressBarVisibility));
              if (this.isStatusPending_.Value)
                return;
              this.statusPendingSub_.SafeDispose();
              this.statusPendingSub_ = (IDisposable) null;
            }));
          }));
        return (((int) this.isStatusPending_ ?? 0) != 0).ToVisibility();
      }
    }

    public RichTextBlock.TextSet CurrentStatus
    {
      get
      {
        if (this.self_ == null)
          ContactsContext.Instance((Action<ContactsContext>) (db =>
          {
            if (this.self_ != null)
              return;
            this.self_ = db.GetUserStatus(Settings.MyJid);
          }));
        if (this.statusChangedSub_ == null)
          this.statusChangedSub_ = this.self_.GetPropertyChangedAsync().Where<PropertyChangedEventArgs>((Func<PropertyChangedEventArgs, bool>) (a => a.PropertyName == "Status")).ObserveOnDispatcher<PropertyChangedEventArgs>().Subscribe<PropertyChangedEventArgs>((Action<PropertyChangedEventArgs>) (_ =>
          {
            this.isStatusPending_ = new bool?();
            this.NotifyPropertyChanged("StatusProgressBarVisibility");
            this.NotifyPropertyChanged(nameof (CurrentStatus));
            this.NotifyPropertyChanged("VerticalScrollBarVisibility");
          }));
        if (this.self_.Status == null)
          UsyncQueryRequest.SendGetStatuses((IEnumerable<FunXMPP.Connection.StatusRequest>) new FunXMPP.Connection.StatusRequest[1]
          {
            new FunXMPP.Connection.StatusRequest()
            {
              Jid = this.self_.Jid
            }
          }, FunXMPP.Connection.SyncMode.Query, FunXMPP.Connection.SyncContext.Interactive, onComplete: (Action) (() => this.NotifyPropertyChanged(nameof (CurrentStatus))));
        return new RichTextBlock.TextSet()
        {
          Text = this.self_.Status
        };
      }
    }

    public Visibility ProfilePicProgressBarVisibility => this.isProfilePicPending_.ToVisibility();

    public System.Windows.Media.ImageSource ProfilePicSource
    {
      get
      {
        if (this.profilePicSource_ == null && this.selfImageFetchSub_ == null)
          this.selfImageFetchSub_ = ChatPictureStore.GetState(Settings.MyJid, true, false, false).SubscribeOn<ChatPictureStore.PicState>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<ChatPictureStore.PicState>().Subscribe<ChatPictureStore.PicState>((Action<ChatPictureStore.PicState>) (state =>
          {
            this.isProfilePicPending_ = state.IsPending;
            this.profilePicSource_ = (System.Windows.Media.ImageSource) state.Image;
            this.NotifyPropertyChanged(nameof (ProfilePicSource));
            this.NotifyPropertyChanged("ProfilePicProgressBarVisibility");
          }));
        return this.profilePicSource_ ?? (System.Windows.Media.ImageSource) AssetStore.DefaultContactIcon;
      }
    }

    public bool IsSIPUp
    {
      set
      {
        this.isSIPUp_ = value;
        this.NotifyPropertyChanged("PageMargin");
      }
    }

    public ProfilePageViewModel(PageOrientation initialOrientation)
      : base(initialOrientation)
    {
    }

    protected override void DisposeManagedResources()
    {
      this.statusChangedSub_.SafeDispose();
      this.statusChangedSub_ = (IDisposable) null;
      this.statusPendingSub_.SafeDispose();
      this.statusPendingSub_ = (IDisposable) null;
      this.selfImageFetchSub_.SafeDispose();
      this.selfImageFetchSub_ = (IDisposable) null;
      base.DisposeManagedResources();
    }
  }
}
