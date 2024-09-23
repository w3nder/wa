// Decompiled with JetBrains decompiler
// Type: WhatsApp.GroupCallInfoPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class GroupCallInfoPage : PhoneApplicationPage
  {
    private static CallRecord NextCallRecord;
    private ObservableCollection<GroupParticipantLogItemViewModel> connectedParticipants = new ObservableCollection<GroupParticipantLogItemViewModel>();
    private ObservableCollection<GroupParticipantLogItemViewModel> missedParticipants = new ObservableCollection<GroupParticipantLogItemViewModel>();
    private GroupCallInfoPageViewModel viewModel;
    internal Grid LayoutRoot;
    internal Grid TimeStampBlock;
    internal WhatsApp.CompatibilityShims.LongListSelector ConnectedParticipantList;
    internal Border Separator;
    internal WhatsApp.CompatibilityShims.LongListSelector MissedParticipantList;
    private bool _contentLoaded;

    public GroupCallInfoPage()
    {
      this.InitializeComponent();
      if (GroupCallInfoPage.NextCallRecord == null)
        return;
      this.Init(GroupCallInfoPage.NextCallRecord);
    }

    public static void Start(CallRecord callRecord)
    {
      GroupCallInfoPage.NextCallRecord = callRecord;
      NavUtils.NavigateToPage(nameof (GroupCallInfoPage));
    }

    private void Init(CallRecord callRecord)
    {
      this.DataContext = (object) (this.viewModel = new GroupCallInfoPageViewModel(callRecord, this.Orientation));
      ContactCallLogView element = new ContactCallLogView();
      this.TimeStampBlock.Children.Add((UIElement) element);
      Grid.SetRow((FrameworkElement) element, 0);
      this.ConnectedParticipantList.ItemsSource = (IList) this.connectedParticipants;
      this.MissedParticipantList.ItemsSource = (IList) this.missedParticipants;
      this.Separator.BorderBrush = UIUtils.SubtleBrush;
      foreach (CallRecord.CallLogEntryParticipant participant in callRecord.ParticipantEntriesSorted)
      {
        GroupParticipantLogItemViewModel logItemViewModel = new GroupParticipantLogItemViewModel(UserCache.Get(participant.jid, false), participant);
        if (participant.res == CallRecord.CallResult.Connected)
        {
          this.connectedParticipants.Add(logItemViewModel);
        }
        else
        {
          this.missedParticipants.Add(logItemViewModel);
          this.Separator.Visibility = Visibility.Visible;
        }
      }
      element.Show(callRecord.PeerJid, callRecord);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/GroupCallInfoPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TimeStampBlock = (Grid) this.FindName("TimeStampBlock");
      this.ConnectedParticipantList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("ConnectedParticipantList");
      this.Separator = (Border) this.FindName("Separator");
      this.MissedParticipantList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("MissedParticipantList");
    }
  }
}
