// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChangeDecriptionPage
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
using WhatsApp.RegularExpressions;

#nullable disable
namespace WhatsApp
{
  public class ChangeDecriptionPage : PhoneApplicationPage
  {
    private Conversation convo_;
    private string currentDescription;
    private GlobalProgressIndicator progress;
    private List<IDisposable> disposables = new List<IDisposable>();
    private bool acted_;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal EmojiTextBox DescriptionTextBox;
    private bool _contentLoaded;

    public ChangeDecriptionPage()
    {
      this.InitializeComponent();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.progress = new GlobalProgressIndicator((DependencyObject) this);
      this.DescriptionTextBox.MaxLength = Settings.GroupDescriptionLength;
      this.DescriptionTextBox.SIPChanged += (EventHandler) ((sender, e) => this.RefreshPageMargin());
    }

    private void RefreshPageMargin()
    {
      this.LayoutRoot.Margin = !this.Orientation.IsLandscape() || !this.DescriptionTextBox.IsSIPUp ? new Thickness(0.0) : new Thickness(0.0, -97.0, 0.0, 0.0);
    }

    private void UpdateConvo(Conversation convo)
    {
      this.currentDescription = this.DescriptionTextBox.Text = Emoji.ConvertToTextOnly(Emoji.ConvertToUnicode(convo.GroupDescription), (byte[]) null);
      Log.d("ChangeDescription", "Changing description to {0}", (object) this.currentDescription);
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
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            this.convo_ = db.GetConversation(jid, CreateOptions.None);
            if (this.convo_ == null)
              return;
            this.UpdateConvo(this.convo_);
            this.disposables.Add(db.UpdatedConversationSubject.Select<ConvoAndMessage, Conversation>((Func<ConvoAndMessage, Conversation>) (convoAndMsg => convoAndMsg.Conversation)).Where<Conversation>((Func<Conversation, bool>) (c => c.Jid == this.convo_.Jid)).ObserveOnDispatcher<Conversation>().Subscribe<Conversation>(new Action<Conversation>(this.UpdateConvo)));
            abort = false;
          }));
      }
      base.OnNavigatedTo(e);
      if (!abort)
        return;
      this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack()));
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      this.DescriptionTextBox.CloseEmojiKeyboard();
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
      if (this.currentDescription == this.DescriptionTextBox.Text)
      {
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack()));
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
        string descriptionBody = Emoji.ConvertToRichText(this.DescriptionTextBox.Text);
        if (string.IsNullOrEmpty(descriptionBody) && this.convo_.IsGroup())
          descriptionBody = "";
        App.CurrentApp.Connection.SendSetGroupDescription(this.convo_.Jid, new GroupDescription(this.SanitizeDescription(descriptionBody))
        {
          PreviousId = this.convo_.GroupDescriptionId ?? "none"
        }, (Action) (() =>
        {
          release();
          this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack()));
        }), (Action<int>) (errCode =>
        {
          if (errCode != 406)
            ;
          release();
          this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack()));
        }));
      }
    }

    private string SanitizeDescription(string descriptionBody)
    {
      return new Regex("[\n|\r|\r\n]{3,}").Replace(descriptionBody, "\n\n");
    }

    private void Background_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.DescriptionTextBox.CloseEmojiKeyboard();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/ChangeDescriptionPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.DescriptionTextBox = (EmojiTextBox) this.FindName("DescriptionTextBox");
    }
  }
}
