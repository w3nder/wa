// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactSupportOutagePage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;


namespace WhatsApp
{
  public class ContactSupportOutagePage : PhoneApplicationPage
  {
    private static IObserver<bool> nextInstanceObserver_;
    private static ServerStatus nextInstanceServerStatus_;
    private IObserver<bool> observer_;
    private ContactSupportOutagePageViewModel viewModel_;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    private bool _contentLoaded;

    public ContactSupportOutagePage()
    {
      this.InitializeComponent();
      this.viewModel_ = new ContactSupportOutagePageViewModel(ContactSupportOutagePage.nextInstanceServerStatus_, this.Orientation);
      this.observer_ = ContactSupportOutagePage.nextInstanceObserver_;
      ContactSupportOutagePage.nextInstanceServerStatus_ = (ServerStatus) null;
      ContactSupportOutagePage.nextInstanceObserver_ = (IObserver<bool>) null;
      this.DataContext = (object) this.viewModel_;
    }

    public static IObservable<bool> Start(ServerStatus status)
    {
      return Observable.Create<bool>((Func<IObserver<bool>, Action>) (observer =>
      {
        ContactSupportOutagePage.nextInstanceServerStatus_ = status;
        ContactSupportOutagePage.nextInstanceObserver_ = observer;
        NavUtils.NavigateToPage(nameof (ContactSupportOutagePage));
        return (Action) (() => { });
      }));
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      int num = this.observer_ == null ? 1 : (this.viewModel_ == null ? 1 : 0);
      base.OnNavigatedTo(e);
      if (num == 0)
        return;
      this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
    }

    public void Button_OnTap(object sender, RoutedEventArgs e)
    {
      this.observer_.OnNext(this.viewModel_.CanContinue);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/ContactSupportOutagePage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
    }
  }
}
