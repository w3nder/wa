// Decompiled with JetBrains decompiler
// Type: WhatsApp.MentionsListControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WhatsApp.CompatibilityShims;
using WhatsApp.WaCollections;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class MentionsListControl : UserControl
  {
    private string chatJid;
    private string currPrefix;
    private bool recentMentionsChanged;
    private string[] recentMentions;
    private List<MentionsListControl.MentionItemViewModel> allVms;
    private Dictionary<string, MentionsListControl.MentionItemViewModel> allVmsDict;
    private ObservableCollection<MentionsListControl.MentionItemViewModel> listSrc;
    private Subject<string> mentionedJidSubject = new Subject<string>();
    internal ZoomBox RootZoomBox;
    internal Grid LayoutRoot;
    internal LongListSelector MentionsList;
    internal TextBlock MentionsListTooltip;
    private bool _contentLoaded;

    public string[] RecentMentions
    {
      private get => this.recentMentions;
      set
      {
        this.recentMentions = value;
        this.recentMentionsChanged = true;
      }
    }

    public IList<MentionsListControl.MentionItemViewModel> CurrentList
    {
      get
      {
        return (IList<MentionsListControl.MentionItemViewModel>) this.listSrc ?? (IList<MentionsListControl.MentionItemViewModel>) new ObservableCollection<MentionsListControl.MentionItemViewModel>();
      }
    }

    public MentionsListControl() => this.InitializeComponent();

    public IObservable<string> GetMentionedJidObservable()
    {
      return (IObservable<string>) this.mentionedJidSubject;
    }

    public void SetChatJid(string jid)
    {
      if (!JidHelper.IsGroupJid(jid))
      {
        this.chatJid = (string) null;
      }
      else
      {
        this.chatJid = jid;
        AppState.Worker.Enqueue((Action) (() =>
        {
          UserStatus[] participants = ConversationHelper.GetParticipants(jid, true, false);
          List<MentionsListControl.MentionItemViewModel> vms = new List<MentionsListControl.MentionItemViewModel>(participants.Length);
          Dictionary<string, MentionsListControl.MentionItemViewModel> vmsDict = new Dictionary<string, MentionsListControl.MentionItemViewModel>();
          foreach (UserStatus user in participants)
          {
            MentionsListControl.MentionItemViewModel mentionItemViewModel = new MentionsListControl.MentionItemViewModel(user);
            vms.Add(mentionItemViewModel);
            vmsDict[user.Jid] = mentionItemViewModel;
          }
          this.Dispatcher.BeginInvoke((Action) (() =>
          {
            this.allVms = vms;
            this.allVmsDict = vmsDict;
            this.UpdateList(this.currPrefix, true);
          }));
        }));
      }
    }

    public void UpdateList(string prefix, bool force = false)
    {
      if (this.currPrefix == prefix && !this.recentMentionsChanged && !force || this.allVms == null)
        return;
      this.currPrefix = prefix;
      bool isPrefixNullOrEmpty = string.IsNullOrEmpty(prefix);
      List<MentionsListControl.MentionItemViewModel> mentionItemViewModelList;
      if (this.RecentMentions != null && ((IEnumerable<string>) this.RecentMentions).Any<string>())
      {
        MentionsListControl.MentionItemViewModel[] array = ((IEnumerable<string>) this.RecentMentions).Select<string, MentionsListControl.MentionItemViewModel>((Func<string, MentionsListControl.MentionItemViewModel>) (jid => this.allVmsDict.GetValueOrDefault<string, MentionsListControl.MentionItemViewModel>(jid))).Where<MentionsListControl.MentionItemViewModel>((Func<MentionsListControl.MentionItemViewModel, bool>) (vm =>
        {
          if (vm == null)
            return false;
          if (isPrefixNullOrEmpty)
            return true;
          return vm.User.ContactName != null && vm.User.ContactName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
        })).ToArray<MentionsListControl.MentionItemViewModel>();
        Set<MentionsListControl.MentionItemViewModel> vmsToRemove = new Set<MentionsListControl.MentionItemViewModel>((IEnumerable<MentionsListControl.MentionItemViewModel>) array);
        mentionItemViewModelList = this.allVms.Where<MentionsListControl.MentionItemViewModel>((Func<MentionsListControl.MentionItemViewModel, bool>) (vm => !vmsToRemove.Contains(vm) && (isPrefixNullOrEmpty || vm.User.ContactName != null && vm.User.ContactName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))).ToList<MentionsListControl.MentionItemViewModel>();
        mentionItemViewModelList.InsertRange(0, (IEnumerable<MentionsListControl.MentionItemViewModel>) array);
      }
      else
        mentionItemViewModelList = string.IsNullOrEmpty(prefix) ? this.allVms : this.allVms.Where<MentionsListControl.MentionItemViewModel>((Func<MentionsListControl.MentionItemViewModel, bool>) (vm => vm.User.ContactName != null && vm.User.ContactName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))).ToList<MentionsListControl.MentionItemViewModel>();
      if (this.listSrc == null || this.recentMentionsChanged)
      {
        this.MentionsList.ItemsSource = (IList) (this.listSrc = new ObservableCollection<MentionsListControl.MentionItemViewModel>(mentionItemViewModelList));
        this.MentionsListTooltip.Text = AppResources.MentionsListTooltip;
      }
      else
      {
        Utils.UpdateInPlace<MentionsListControl.MentionItemViewModel>((IList<MentionsListControl.MentionItemViewModel>) this.listSrc, (IList<MentionsListControl.MentionItemViewModel>) mentionItemViewModelList, (Func<MentionsListControl.MentionItemViewModel, string>) (vm => vm.Jid), (Action<MentionsListControl.MentionItemViewModel>) null);
        if (isPrefixNullOrEmpty && this.listSrc.Any<MentionsListControl.MentionItemViewModel>())
          this.MentionsList.ScrollTo((object) this.listSrc.First<MentionsListControl.MentionItemViewModel>());
      }
      this.recentMentionsChanged = false;
    }

    private void MentionItem_Tap(object sender, GestureEventArgs e)
    {
      string jid = (sender as FrameworkElement).DataContext is MentionsListControl.MentionItemViewModel dataContext ? dataContext.User?.Jid : (string) null;
      if (!JidHelper.IsUserJid(jid))
        return;
      this.mentionedJidSubject.OnNext(jid);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/MentionsListControl.xaml", UriKind.Relative));
      this.RootZoomBox = (ZoomBox) this.FindName("RootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.MentionsList = (LongListSelector) this.FindName("MentionsList");
      this.MentionsListTooltip = (TextBlock) this.FindName("MentionsListTooltip");
    }

    public class MentionItemViewModel : UserViewModel
    {
      public MentionItemViewModel(UserStatus user)
        : base(user)
      {
      }

      public override string GetTitle()
      {
        string contactName = this.User?.ContactName;
        return !string.IsNullOrEmpty(contactName) ? contactName : (!string.IsNullOrEmpty(this.User?.PushName) ? string.Format("{0} ~{1}", (object) this.User?.GetDisplayName(), (object) this.User.PushName) : this.User?.GetDisplayName());
      }
    }
  }
}
