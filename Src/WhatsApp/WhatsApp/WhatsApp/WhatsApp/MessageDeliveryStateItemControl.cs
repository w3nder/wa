// Decompiled with JetBrains decompiler
// Type: WhatsApp.MessageDeliveryStateItemControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace WhatsApp
{
  public class MessageDeliveryStateItemControl : UserControl
  {
    private IDisposable profilePicSub;
    private IDisposable selectionSub;
    private IDisposable pendingSbDispose;
    public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof (Item), typeof (MessageDeliveryState.RecipientItem), typeof (MessageDeliveryStateItemControl), new PropertyMetadata(new PropertyChangedCallback(MessageDeliveryStateItemControl.OnItemChanged)));
    internal Grid LayoutRoot;
    internal Grid RecipientInfoRow;
    internal Image ProfilePicImage;
    internal TextBlock NameBlock;
    internal TextBlock DateBlock;
    internal TextBlock TimeBlock;
    internal TextBlock DotsBlock;
    internal Grid ReceiptsRow;
    internal ItemsControl ReceiptsList;
    internal TranslateTransform ReceiptsListTransform;
    private bool _contentLoaded;

    public MessageDeliveryState.RecipientItem Item
    {
      get
      {
        return this.GetValue(MessageDeliveryStateItemControl.ItemProperty) as MessageDeliveryState.RecipientItem;
      }
      set => this.SetValue(MessageDeliveryStateItemControl.ItemProperty, (object) value);
    }

    public event EventHandler ReceiptsListOpened;

    protected void NotifyReceiptsListOpened()
    {
      if (this.ReceiptsListOpened == null)
        return;
      this.ReceiptsListOpened((object) this, new EventArgs());
    }

    public MessageDeliveryStateItemControl() => this.InitializeComponent();

    ~MessageDeliveryStateItemControl()
    {
      this.profilePicSub.SafeDispose();
      this.profilePicSub = (IDisposable) null;
      this.selectionSub.SafeDispose();
      this.selectionSub = (IDisposable) null;
    }

    public static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (!(d is MessageDeliveryStateItemControl stateItemControl))
        return;
      stateItemControl.InitContent();
    }

    private void Cleanup()
    {
      this.ProfilePicImage.Source = (System.Windows.Media.ImageSource) null;
      this.profilePicSub.SafeDispose();
      this.profilePicSub = (IDisposable) null;
      this.selectionSub.SafeDispose();
      this.selectionSub = (IDisposable) null;
    }

    private void InitContent()
    {
      this.Cleanup();
      MessageDeliveryState.RecipientItem recipient = this.Item;
      if (recipient == null)
        return;
      this.ReceiptsRow.Visibility = Visibility.Collapsed;
      UserStatus user = recipient.User;
      if (user == null)
      {
        this.DotsBlock.Visibility = Visibility.Visible;
        this.RecipientInfoRow.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.DotsBlock.Visibility = Visibility.Collapsed;
        this.RecipientInfoRow.Visibility = Visibility.Visible;
        this.profilePicSub = ChatPictureStore.Get(user.Jid, false, false, true, ChatPictureStore.SubMode.GetCurrent).SubscribeOn<ChatPictureStore.PicState>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<ChatPictureStore.PicState>().Subscribe<ChatPictureStore.PicState>((Action<ChatPictureStore.PicState>) (picState =>
        {
          this.ProfilePicImage.Source = (System.Windows.Media.ImageSource) (picState.Image ?? AssetStore.DefaultContactIcon);
          this.profilePicSub.SafeDispose();
          this.profilePicSub = (IDisposable) null;
        }));
        this.NameBlock.Text = user.GetDisplayName();
        ReceiptState mostRecentReceipt = recipient.MostRecentReceipt;
        DateTime now = DateTime.Now;
        DateTime localTimestamp = mostRecentReceipt.LocalTimestamp;
        this.DateBlock.Text = now.DayOfYear != localTimestamp.DayOfYear || (now - localTimestamp).TotalHours >= 24.0 ? DateTimeUtils.FormatCompactDate(localTimestamp) : "";
        this.TimeBlock.Text = DateTimeUtils.FormatCompactTime(localTimestamp);
        if (recipient.IsSelected)
          this.ShowReceiptsList(true, false);
        this.selectionSub.SafeDispose();
        this.selectionSub = recipient.GetPropertyChangedAsync().Where<PropertyChangedEventArgs>((Func<PropertyChangedEventArgs, bool>) (p => p.PropertyName == "IsSelected")).Subscribe<PropertyChangedEventArgs>((Action<PropertyChangedEventArgs>) (p => this.ShowReceiptsList(recipient.IsSelected)));
      }
    }

    private Storyboard GetReceiptsListOpenAnaimation()
    {
      DoubleAnimation doubleAnimation1 = new DoubleAnimation();
      doubleAnimation1.Duration = (Duration) TimeSpan.FromMilliseconds(200.0);
      doubleAnimation1.From = new double?(0.65);
      doubleAnimation1.To = new double?(0.0);
      DoubleAnimation element1 = doubleAnimation1;
      Storyboard.SetTargetProperty((Timeline) element1, new PropertyPath("Opacity", new object[0]));
      Storyboard.SetTarget((Timeline) element1, (DependencyObject) this.DateBlock);
      DoubleAnimation doubleAnimation2 = new DoubleAnimation();
      doubleAnimation2.Duration = (Duration) TimeSpan.FromMilliseconds(200.0);
      doubleAnimation2.From = new double?(1.0);
      doubleAnimation2.To = new double?(0.0);
      DoubleAnimation element2 = doubleAnimation2;
      Storyboard.SetTargetProperty((Timeline) element2, new PropertyPath("Opacity", new object[0]));
      Storyboard.SetTarget((Timeline) element2, (DependencyObject) this.TimeBlock);
      DoubleAnimation doubleAnimation3 = new DoubleAnimation();
      doubleAnimation3.Duration = (Duration) TimeSpan.FromMilliseconds(400.0);
      doubleAnimation3.From = new double?(0.0);
      doubleAnimation3.To = new double?(1.0);
      PowerEase powerEase1 = new PowerEase();
      powerEase1.Power = 12.0;
      powerEase1.EasingMode = EasingMode.EaseOut;
      doubleAnimation3.EasingFunction = (IEasingFunction) powerEase1;
      DoubleAnimation element3 = doubleAnimation3;
      Storyboard.SetTargetProperty((Timeline) element3, new PropertyPath("Opacity", new object[0]));
      Storyboard.SetTarget((Timeline) element3, (DependencyObject) this.ReceiptsRow);
      DoubleAnimation doubleAnimation4 = new DoubleAnimation();
      doubleAnimation4.Duration = (Duration) TimeSpan.FromMilliseconds(500.0);
      doubleAnimation4.From = new double?(-48.0);
      doubleAnimation4.To = new double?(0.0);
      PowerEase powerEase2 = new PowerEase();
      powerEase2.Power = 6.0;
      powerEase2.EasingMode = EasingMode.EaseOut;
      doubleAnimation4.EasingFunction = (IEasingFunction) powerEase2;
      DoubleAnimation element4 = doubleAnimation4;
      Storyboard.SetTargetProperty((Timeline) element4, new PropertyPath("Y", new object[0]));
      Storyboard.SetTarget((Timeline) element4, (DependencyObject) this.ReceiptsListTransform);
      Storyboard listOpenAnaimation = new Storyboard();
      listOpenAnaimation.Children.Add((Timeline) element1);
      listOpenAnaimation.Children.Add((Timeline) element2);
      listOpenAnaimation.Children.Add((Timeline) element3);
      listOpenAnaimation.Children.Add((Timeline) element4);
      return listOpenAnaimation;
    }

    private Storyboard GetReceiptsListCloseAnaimation()
    {
      DoubleAnimation doubleAnimation1 = new DoubleAnimation();
      doubleAnimation1.Duration = (Duration) TimeSpan.FromMilliseconds(350.0);
      doubleAnimation1.From = new double?(0.0);
      doubleAnimation1.To = new double?(0.65);
      DoubleAnimation element1 = doubleAnimation1;
      Storyboard.SetTargetProperty((Timeline) element1, new PropertyPath("Opacity", new object[0]));
      Storyboard.SetTarget((Timeline) element1, (DependencyObject) this.DateBlock);
      DoubleAnimation doubleAnimation2 = new DoubleAnimation();
      doubleAnimation2.Duration = (Duration) TimeSpan.FromMilliseconds(350.0);
      doubleAnimation2.From = new double?(0.0);
      doubleAnimation2.To = new double?(1.0);
      DoubleAnimation element2 = doubleAnimation2;
      Storyboard.SetTargetProperty((Timeline) element2, new PropertyPath("Opacity", new object[0]));
      Storyboard.SetTarget((Timeline) element2, (DependencyObject) this.TimeBlock);
      Storyboard listCloseAnaimation = new Storyboard();
      listCloseAnaimation.Children.Add((Timeline) element1);
      listCloseAnaimation.Children.Add((Timeline) element2);
      return listCloseAnaimation;
    }

    public void ShowReceiptsList(bool show, bool animate = true)
    {
      this.pendingSbDispose.SafeDispose();
      this.pendingSbDispose = (IDisposable) null;
      MessageDeliveryState.RecipientItem recipientItem = this.Item;
      if (show && recipientItem != null)
      {
        this.ReceiptsRow.Opacity = 0.0;
        this.ReceiptsList.ItemsSource = (IEnumerable) recipientItem.GetReceiptViewModels(false);
        this.ReceiptsRow.Visibility = Visibility.Visible;
        this.DateBlock.Visibility = this.TimeBlock.Visibility = Visibility.Collapsed;
        this.pendingSbDispose = Storyboarder.PerformWithDisposable(animate ? this.GetReceiptsListOpenAnaimation() : (Storyboard) null, (DependencyObject) null, true, (Action) (() =>
        {
          this.ReceiptsRow.Opacity = 1.0;
          this.pendingSbDispose = (IDisposable) null;
        }), false, "msg delivery state control: open receipts list");
        this.NotifyReceiptsListOpened();
      }
      else
      {
        if (this.ReceiptsRow.Visibility != Visibility.Visible)
          return;
        this.ReceiptsRow.Opacity = 0.0;
        this.ReceiptsList.ItemsSource = (IEnumerable) null;
        this.ReceiptsRow.Visibility = Visibility.Collapsed;
        this.DateBlock.Opacity = 0.65;
        this.TimeBlock.Opacity = 1.0;
        this.DateBlock.Visibility = this.TimeBlock.Visibility = Visibility.Visible;
        this.pendingSbDispose = Storyboarder.PerformWithDisposable(animate ? this.GetReceiptsListCloseAnaimation() : (Storyboard) null, (DependencyObject) null, true, (Action) (() => this.pendingSbDispose = (IDisposable) null), false, "msg delivery state control: close receipts list");
      }
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/MessageDeliveryStateItemControl.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.RecipientInfoRow = (Grid) this.FindName("RecipientInfoRow");
      this.ProfilePicImage = (Image) this.FindName("ProfilePicImage");
      this.NameBlock = (TextBlock) this.FindName("NameBlock");
      this.DateBlock = (TextBlock) this.FindName("DateBlock");
      this.TimeBlock = (TextBlock) this.FindName("TimeBlock");
      this.DotsBlock = (TextBlock) this.FindName("DotsBlock");
      this.ReceiptsRow = (Grid) this.FindName("ReceiptsRow");
      this.ReceiptsList = (ItemsControl) this.FindName("ReceiptsList");
      this.ReceiptsListTransform = (TranslateTransform) this.FindName("ReceiptsListTransform");
    }
  }
}
