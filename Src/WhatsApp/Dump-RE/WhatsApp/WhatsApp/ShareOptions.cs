// Decompiled with JetBrains decompiler
// Type: WhatsApp.ShareOptions
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

#nullable disable
namespace WhatsApp
{
  public class ShareOptions : PhoneApplicationPage
  {
    private List<ShareOptions.TaskContainer> tasks;
    internal Grid LayoutRoot;
    internal ListBox OptionsList;
    private bool _contentLoaded;

    public List<ShareOptions.TaskContainer> Tasks
    {
      get
      {
        if (this.tasks == null)
          this.tasks = new List<ShareOptions.TaskContainer>()
          {
            new ShareOptions.TaskContainer()
            {
              Task = (object) new ShareLinkTask()
              {
                LinkUri = new Uri("https://whatsapp.com/dl/"),
                Message = AppResources.TellFriendBodyShort,
                Title = ""
              },
              Description = AppResources.SocialNetworks
            },
            new ShareOptions.TaskContainer()
            {
              Task = (object) new EmailComposeTask()
              {
                Subject = AppResources.TellFriendSubject,
                Body = string.Format(AppResources.TellFriendBodyLong, (object) "https://www.whatsapp.com/download/")
              },
              Description = AppResources.Email
            },
            new ShareOptions.TaskContainer()
            {
              Task = (object) new SmsComposeTask()
              {
                Body = string.Format(AppResources.TellFriendBodyShort, (object) "https://whatsapp.com/dl/")
              },
              Description = AppResources.SMS
            }
          };
        return this.tasks;
      }
      set => this.tasks = value;
    }

    public ShareOptions()
    {
      this.InitializeComponent();
      this.OptionsList.DataContext = (object) this.Tasks;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      FieldStats.ReportUiUsage(wam_enum_ui_usage_type.TELL_A_FRIEND);
    }

    private void OptionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (!(sender is ListBox listBox))
        return;
      if (listBox.SelectedItem is ShareOptions.TaskContainer selectedItem)
        selectedItem.Show();
      listBox.SelectedItem = (object) null;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/ShareOptions.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.OptionsList = (ListBox) this.FindName("OptionsList");
    }

    public class TaskContainer
    {
      public object Task { get; set; }

      public string Description { get; set; }

      public override string ToString() => this.Description;

      public void Show()
      {
        FieldStats.ReportTellAFriend();
        if (this.Task is ShareLinkTask)
          (this.Task as ShareLinkTask).Show();
        else if (this.Task is EmailComposeTask)
        {
          (this.Task as EmailComposeTask).Show();
        }
        else
        {
          if (!(this.Task is SmsComposeTask))
            return;
          (this.Task as SmsComposeTask).Show();
        }
      }
    }
  }
}
