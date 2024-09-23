// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChangeSubjectPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;


namespace WhatsApp
{
  public class ChangeSubjectPage : PhoneApplicationPage
  {
    private Conversation convo_;
    private string currentSubject;
    private GlobalProgressIndicator progress;
    private List<IDisposable> disposables = new List<IDisposable>();
    private bool acted_;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal EmojiTextBox SubjectTextBox;
    private bool _contentLoaded;

    public ChangeSubjectPage()
    {
      this.InitializeComponent();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.progress = new GlobalProgressIndicator((DependencyObject) this);
      this.SubjectTextBox.MaxLength = Settings.MaxGroupSubject;
      this.SubjectTextBox.SIPChanged += (EventHandler) ((sender, e) => this.RefreshPageMargin());
    }

    private void RefreshPageMargin()
    {
      this.LayoutRoot.Margin = !this.Orientation.IsLandscape() || !this.SubjectTextBox.IsSIPUp ? new Thickness(0.0) : new Thickness(0.0, -97.0, 0.0, 0.0);
    }

    private void UpdateConvo(Conversation convo)
    {
      if (!(convo.GroupSubject != this.currentSubject))
        return;
      this.currentSubject = this.SubjectTextBox.Text = Emoji.ConvertToTextOnly(Emoji.ConvertToUnicode(convo.GroupSubject), (byte[]) null);
    }

    protected override void OnOrientationChanged(OrientationChangedEventArgs e)
    {
      base.OnOrientationChanged(e);
      this.RefreshPageMargin();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      bool abort = true;
      if (this.convo_ == null)
      {
        string jid = (string) null;
        if (this.NavigationContext.QueryString.TryGetValue("jid", out jid) && !string.IsNullOrEmpty(jid))
        {
          if (jid.IsBroadcastJid())
            this.TitlePanel.LargeTitle = AppResources.BroadcastListNameLower;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            this.convo_ = db.GetConversation(jid, CreateOptions.None);
            if (this.convo_ == null)
              return;
            this.UpdateConvo(this.convo_);
            this.disposables.Add(db.UpdatedConversationSubject.Select<ConvoAndMessage, Conversation>((Func<ConvoAndMessage, Conversation>) (convoAndMsg => convoAndMsg.Conversation)).Where<Conversation>((Func<Conversation, bool>) (c => c.Jid == this.convo_.Jid)).ObserveOnDispatcher<Conversation>().Subscribe<Conversation>((Action<Conversation>) (c => this.UpdateConvo(c))));
            abort = false;
          }));
        }
      }
      base.OnNavigatedTo(e);
      if (!abort)
        return;
      this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack()));
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      this.SubjectTextBox.CloseEmojiKeyboard();
      base.OnNavigatedFrom(e);
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.disposables.ForEach((Action<IDisposable>) (d => d.SafeDispose()));
      this.disposables.Clear();
      base.OnRemovedFromJournal(e);
    }

    private void Accept_Click(object sender, EventArgs e)
    {
      if (this.convo_ == null || this.acted_)
        return;
      if (this.currentSubject == this.SubjectTextBox.Text)
      {
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack()));
      }
      else
      {
        string newName = Emoji.ConvertToRichText(this.SubjectTextBox.Text.Length < 1 || this.SubjectTextBox.Text[this.SubjectTextBox.Text.Length - 1] != '\u200B' ? this.SubjectTextBox.Text.Trim() : this.SubjectTextBox.Text.TrimStart());
        if (string.IsNullOrEmpty(newName) && this.convo_.IsGroup())
        {
          int num = (int) MessageBox.Show(AppResources.NoSubject);
        }
        else
        {
          this.acted_ = true;
          this.IsEnabled = false;
          this.progress.Acquire();
          Action release = (Action) (() =>
          {
            release = (Action) (() => { });
            this.Dispatcher.BeginInvoke((Action) (() =>
            {
              this.IsEnabled = true;
              this.progress.Release();
            }));
          });
          if (this.convo_.IsGroup())
          {
            App.CurrentApp.Connection.SendSetGroupSubject(this.convo_.Jid, newName, (Action) (() =>
            {
              release();
              this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack()));
            }), (Action<int>) (errCode =>
            {
              release();
              this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack()));
            }));
          }
          else
          {
            if (!this.convo_.IsBroadcast() || !(newName != this.convo_.GroupSubject))
              return;
            AppState.Worker.Enqueue((Action) (() =>
            {
              MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
              {
                this.convo_.GroupSubject = string.IsNullOrEmpty(newName) ? (string) null : newName;
                db.SubmitChanges();
              }));
              ITile chatTile = TileHelper.GetChatTile(this.convo_.Jid);
              if (chatTile != null)
              {
                string name = this.convo_.GetName();
                chatTile.SetTitle(name);
                chatTile.SetTitle(name, true);
                chatTile.Update();
              }
              Settings.UpdateContactsChecksum();
              AppState.QrPersistentAction.NotifyContactChange(new FunXMPP.ContactResponse()
              {
                Jid = this.convo_.Jid,
                DisplayName = newName
              });
            }));
            NavUtils.GoBack();
          }
        }
      }
    }

    private void Background_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.SubjectTextBox.CloseEmojiKeyboard();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/ChangeSubjectPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.SubjectTextBox = (EmojiTextBox) this.FindName("SubjectTextBox");
    }
  }
}
