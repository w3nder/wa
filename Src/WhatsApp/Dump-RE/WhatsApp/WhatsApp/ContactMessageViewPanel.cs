// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactMessageViewPanel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WhatsApp.CommonOps;

#nullable disable
namespace WhatsApp
{
  public class ContactMessageViewPanel : MessageViewPanel
  {
    private Grid topPanel;
    private Grid bottomPanel;
    private Grid sendMessageButton;
    private Image thumbnailImage;
    private TextBlock nameBlock;
    private IDisposable thumbSub;
    private List<string> addedJids = new List<string>();

    public override MessageViewPanel.ViewTypes ViewType => MessageViewPanel.ViewTypes.Contact;

    public ContactMessageViewPanel()
    {
      this.Width = MessageViewModel.DefaultContentWidth;
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto
      });
      Grid grid = new Grid();
      grid.Background = (Brush) new SolidColorBrush(Color.FromArgb((byte) 51, byte.MaxValue, byte.MaxValue, byte.MaxValue));
      this.topPanel = grid;
      Grid.SetRow((FrameworkElement) this.topPanel, 0);
      this.Children.Add((UIElement) this.topPanel);
      int contactThumbnailSize = ContactMessageViewModel.ContactThumbnailSize;
      this.topPanel.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength((double) contactThumbnailSize)
      });
      this.topPanel.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Star)
      });
      Image image = new Image();
      image.HorizontalAlignment = HorizontalAlignment.Center;
      image.VerticalAlignment = VerticalAlignment.Center;
      image.Width = (double) contactThumbnailSize;
      image.Height = (double) contactThumbnailSize;
      image.Stretch = Stretch.UniformToFill;
      this.thumbnailImage = image;
      Grid.SetColumn((FrameworkElement) this.thumbnailImage, 0);
      this.topPanel.Children.Add((UIElement) this.thumbnailImage);
      TextBlock textBlock = new TextBlock();
      textBlock.Foreground = (Brush) UIUtils.WhiteBrush;
      textBlock.TextWrapping = TextWrapping.NoWrap;
      textBlock.TextTrimming = TextTrimming.WordEllipsis;
      textBlock.HorizontalAlignment = HorizontalAlignment.Left;
      textBlock.VerticalAlignment = VerticalAlignment.Center;
      textBlock.Margin = new Thickness(24.0 * this.zoomMultiplier, 0.0, 24.0 * this.zoomMultiplier, 0.0);
      textBlock.FontSize = 28.0 * this.zoomMultiplier;
      this.nameBlock = textBlock;
      Grid.SetColumn((FrameworkElement) this.nameBlock, 1);
      this.topPanel.Children.Add((UIElement) this.nameBlock);
      this.topPanel.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.TopPanel_Tap);
    }

    public override void Render(MessageViewModel vm)
    {
      ContactMessageViewModel cmvm = vm as ContactMessageViewModel;
      if (cmvm == null)
        return;
      base.Render(vm);
      this.thumbSub.SafeDispose();
      this.thumbSub = vm.GetThumbnailObservable().SubscribeOn<MessageViewModel.ThumbnailState>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<MessageViewModel.ThumbnailState>().Subscribe<MessageViewModel.ThumbnailState>((Action<MessageViewModel.ThumbnailState>) (thumbState =>
      {
        if (thumbState.Image != null && thumbState.KeyId == this.viewModel.Message.KeyId)
          this.thumbnailImage.Source = thumbState.Image;
        else
          this.thumbnailImage.Source = (System.Windows.Media.ImageSource) AssetStore.GetMessageDefaultIcon(FunXMPP.FMessage.Type.Contact);
      }));
      this.nameBlock.Text = vm.Message.GetPreviewText(true, true);
      if (this.RowDefinitions.Count < 2)
        this.RowDefinitions.Add(new RowDefinition()
        {
          Height = GridLength.Auto
        });
      if (this.bottomPanel != null)
        this.Children.Remove((UIElement) this.bottomPanel);
      Grid grid1 = new Grid();
      grid1.Margin = new Thickness(0.0, 6.0 * this.zoomMultiplier, 0.0, 0.0);
      grid1.Height = 48.0 * this.zoomMultiplier;
      this.bottomPanel = grid1;
      Grid.SetRow((FrameworkElement) this.bottomPanel, 1);
      this.Children.Add((UIElement) this.bottomPanel);
      if (cmvm.IsForContactCardView)
      {
        this.bottomPanel.ColumnDefinitions.Add(new ColumnDefinition()
        {
          Width = new GridLength(0.5, GridUnitType.Star)
        });
        this.bottomPanel.ColumnDefinitions.Add(new ColumnDefinition()
        {
          Width = new GridLength(6.0 * this.zoomMultiplier)
        });
        this.bottomPanel.ColumnDefinitions.Add(new ColumnDefinition()
        {
          Width = new GridLength(0.5, GridUnitType.Star)
        });
        Grid grid2 = new Grid();
        grid2.Margin = new Thickness(0.0);
        grid2.Background = (Brush) new SolidColorBrush(Color.FromArgb((byte) 51, byte.MaxValue, byte.MaxValue, byte.MaxValue));
        Grid element1 = grid2;
        UIElementCollection children1 = element1.Children;
        TextBlock textBlock1 = new TextBlock();
        textBlock1.Text = AppResources.VCardSaveContactButton;
        textBlock1.Foreground = (Brush) UIUtils.WhiteBrush;
        textBlock1.VerticalAlignment = VerticalAlignment.Center;
        textBlock1.HorizontalAlignment = HorizontalAlignment.Center;
        textBlock1.Margin = new Thickness(0.0, 0.0, 0.0, 4.0 * this.zoomMultiplier);
        children1.Add((UIElement) textBlock1);
        Grid.SetColumn((FrameworkElement) element1, 0);
        element1.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.SaveContact_Tap);
        this.bottomPanel.Children.Add((UIElement) element1);
        Grid grid3 = new Grid();
        grid3.Margin = new Thickness(0.0);
        grid3.Background = (Brush) new SolidColorBrush(Color.FromArgb((byte) 51, byte.MaxValue, byte.MaxValue, byte.MaxValue));
        Grid element2 = grid3;
        UIElementCollection children2 = element2.Children;
        TextBlock textBlock2 = new TextBlock();
        textBlock2.Text = "view message";
        textBlock2.Foreground = (Brush) UIUtils.WhiteBrush;
        textBlock2.VerticalAlignment = VerticalAlignment.Center;
        textBlock2.HorizontalAlignment = HorizontalAlignment.Center;
        textBlock2.Margin = new Thickness(0.0, 0.0, 0.0, 4.0 * this.zoomMultiplier);
        children2.Add((UIElement) textBlock2);
        Grid.SetColumn((FrameworkElement) element2, 2);
        this.sendMessageButton = element2;
        element2.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.ViewContext_Tap);
        this.bottomPanel.Children.Add((UIElement) element2);
      }
      else if (cmvm.IsBulkContactCard)
      {
        this.bottomPanel.ColumnDefinitions.Add(new ColumnDefinition()
        {
          Width = new GridLength(1.0, GridUnitType.Star)
        });
        Grid grid4 = new Grid();
        grid4.Margin = new Thickness(0.0);
        grid4.Background = (Brush) new SolidColorBrush(Color.FromArgb((byte) 51, byte.MaxValue, byte.MaxValue, byte.MaxValue));
        Grid element = grid4;
        UIElementCollection children = element.Children;
        TextBlock textBlock = new TextBlock();
        textBlock.Text = AppResources.ViewBulkContactCard;
        textBlock.Foreground = (Brush) UIUtils.WhiteBrush;
        textBlock.VerticalAlignment = VerticalAlignment.Center;
        textBlock.HorizontalAlignment = HorizontalAlignment.Center;
        textBlock.Margin = new Thickness(0.0, 0.0, 0.0, 4.0 * this.zoomMultiplier);
        children.Add((UIElement) textBlock);
        Grid.SetColumn((FrameworkElement) element, 0);
        element.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.ViewBulkCards_Tap);
        this.bottomPanel.Children.Add((UIElement) element);
      }
      else if (cmvm.ShouldShowWhatsAppableContactActions)
      {
        bool inContacts = false;
        ContactsContext.Instance((Action<ContactsContext>) (cdb =>
        {
          ContactVCard contactVcard = ContactVCard.Create(cmvm.Message.Data);
          if (contactVcard == null)
            return;
          foreach (ContactVCard.PhoneNumber phoneNumber in contactVcard.PhoneNumbers)
          {
            if (phoneNumber.Jid != null)
            {
              if (this.addedJids.Contains(phoneNumber.Jid))
              {
                inContacts = true;
              }
              else
              {
                UserStatus userStatus = cdb.GetUserStatus(phoneNumber.Jid, false);
                if (userStatus != null)
                  inContacts = userStatus.IsInDeviceContactList;
              }
            }
            if (inContacts)
              break;
          }
        }));
        if (!inContacts)
        {
          this.bottomPanel.ColumnDefinitions.Add(new ColumnDefinition()
          {
            Width = new GridLength(0.5, GridUnitType.Star)
          });
          this.bottomPanel.ColumnDefinitions.Add(new ColumnDefinition()
          {
            Width = new GridLength(6.0 * this.zoomMultiplier)
          });
          this.bottomPanel.ColumnDefinitions.Add(new ColumnDefinition()
          {
            Width = new GridLength(0.5, GridUnitType.Star)
          });
          Grid grid5 = new Grid();
          grid5.Margin = new Thickness(0.0);
          grid5.Background = (Brush) new SolidColorBrush(Color.FromArgb((byte) 51, byte.MaxValue, byte.MaxValue, byte.MaxValue));
          Grid element3 = grid5;
          UIElementCollection children3 = element3.Children;
          TextBlock textBlock3 = new TextBlock();
          textBlock3.Text = AppResources.VCardSaveContactButton;
          textBlock3.Foreground = (Brush) UIUtils.WhiteBrush;
          textBlock3.VerticalAlignment = VerticalAlignment.Center;
          textBlock3.HorizontalAlignment = HorizontalAlignment.Center;
          textBlock3.Margin = new Thickness(0.0, 0.0, 0.0, 4.0 * this.zoomMultiplier);
          children3.Add((UIElement) textBlock3);
          Grid.SetColumn((FrameworkElement) element3, 0);
          element3.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.SaveContact_Tap);
          this.bottomPanel.Children.Add((UIElement) element3);
          Grid grid6 = new Grid();
          grid6.Margin = new Thickness(0.0);
          grid6.Background = (Brush) new SolidColorBrush(Color.FromArgb((byte) 51, byte.MaxValue, byte.MaxValue, byte.MaxValue));
          Grid element4 = grid6;
          UIElementCollection children4 = element4.Children;
          TextBlock textBlock4 = new TextBlock();
          textBlock4.Text = AppResources.VCardSendMessageButton;
          textBlock4.Foreground = (Brush) UIUtils.WhiteBrush;
          textBlock4.VerticalAlignment = VerticalAlignment.Center;
          textBlock4.HorizontalAlignment = HorizontalAlignment.Center;
          textBlock4.Margin = new Thickness(0.0, 0.0, 0.0, 4.0 * this.zoomMultiplier);
          children4.Add((UIElement) textBlock4);
          Grid.SetColumn((FrameworkElement) element4, 2);
          this.sendMessageButton = element4;
          element4.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.SendMessage_Tap);
          this.bottomPanel.Children.Add((UIElement) element4);
        }
        else
          this.AddMessageButtonOnly(this.bottomPanel);
      }
      else
      {
        this.bottomPanel.ColumnDefinitions.Add(new ColumnDefinition()
        {
          Width = new GridLength(1.0, GridUnitType.Star)
        });
        Grid grid7 = new Grid();
        grid7.Margin = new Thickness(0.0);
        grid7.Background = (Brush) new SolidColorBrush(Color.FromArgb((byte) 51, byte.MaxValue, byte.MaxValue, byte.MaxValue));
        Grid element = grid7;
        UIElementCollection children = element.Children;
        TextBlock textBlock = new TextBlock();
        textBlock.Text = AppResources.InviteToWAWithoutName;
        textBlock.Foreground = (Brush) UIUtils.WhiteBrush;
        textBlock.VerticalAlignment = VerticalAlignment.Center;
        textBlock.HorizontalAlignment = HorizontalAlignment.Center;
        textBlock.Margin = new Thickness(0.0, 0.0, 0.0, 4.0 * this.zoomMultiplier);
        children.Add((UIElement) textBlock);
        Grid.SetColumn((FrameworkElement) element, 0);
        element.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.Invite_Tap);
        this.bottomPanel.Children.Add((UIElement) element);
      }
      this.ShowElement((FrameworkElement) this.bottomPanel, true);
    }

    private void AddMessageButtonOnly(Grid bottomPanel)
    {
      bottomPanel.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Star)
      });
      Grid grid = new Grid();
      grid.Margin = new Thickness(0.0);
      grid.Background = (Brush) new SolidColorBrush(Color.FromArgb((byte) 51, byte.MaxValue, byte.MaxValue, byte.MaxValue));
      Grid element = grid;
      UIElementCollection children = element.Children;
      TextBlock textBlock = new TextBlock();
      textBlock.Text = AppResources.VCardSendMessageButton;
      textBlock.Foreground = (Brush) UIUtils.WhiteBrush;
      textBlock.VerticalAlignment = VerticalAlignment.Center;
      textBlock.HorizontalAlignment = HorizontalAlignment.Center;
      textBlock.Margin = new Thickness(0.0, 0.0, 0.0, 4.0 * this.zoomMultiplier);
      children.Add((UIElement) textBlock);
      Grid.SetColumn((FrameworkElement) element, 0);
      element.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.SendMessage_Tap);
      bottomPanel.Children.Add((UIElement) element);
    }

    public override void Cleanup()
    {
      base.Cleanup();
      this.thumbnailImage.Source = (System.Windows.Media.ImageSource) null;
    }

    protected override void DisposeSubscriptions()
    {
      base.DisposeSubscriptions();
      this.thumbSub.SafeDispose();
      this.thumbSub = (IDisposable) null;
    }

    private void TopPanel_Tap(object sender, EventArgs e)
    {
      ViewMessage.View(this.viewModel.Message);
    }

    private void SaveContact_Tap(object sender, EventArgs e)
    {
      IEnumerable<ContactVCard> contactCards = this.viewModel.Message.GetContactCards();
      ContactVCard thisContact = (ContactVCard) null;
      if (contactCards.Count<ContactVCard>() > 1)
      {
        foreach (ContactVCard contactVcard in contactCards)
        {
          foreach (ContactVCard.PhoneNumber phoneNumber in contactVcard.PhoneNumbers)
          {
            if (phoneNumber.Jid == this.viewModel.JidForContactCardView)
            {
              thisContact = contactVcard;
              break;
            }
          }
          if (thisContact != null)
            break;
        }
      }
      else
        thisContact = contactCards.FirstOrDefault<ContactVCard>();
      thisContact.ToSaveContactTask().GetShowTaskAsync<SaveContactResult>().Subscribe<IEvent<SaveContactResult>>((Action<IEvent<SaveContactResult>>) (result =>
      {
        if (result.EventArgs.TaskResult != TaskResult.OK)
          return;
        this.bottomPanel.ColumnDefinitions.Clear();
        this.bottomPanel.Children.Clear();
        this.AddMessageButtonOnly(this.bottomPanel);
        foreach (ContactVCard.PhoneNumber phoneNumber in thisContact.PhoneNumbers)
        {
          if (!string.IsNullOrEmpty(phoneNumber.Jid))
          {
            this.addedJids.Add(phoneNumber.Jid);
            break;
          }
        }
      }));
    }

    private void Invite_Tap(object sender, EventArgs e)
    {
      ContactVCard contactVcard = ContactVCard.Create(this.viewModel.Message.Data);
      string[] strArray = contactVcard == null ? new string[0] : ((IEnumerable<ContactVCard.PhoneNumber>) contactVcard.PhoneNumbers).Select<ContactVCard.PhoneNumber, string>((Func<ContactVCard.PhoneNumber, string>) (pn => pn.Number)).ToArray<string>();
      new ShareOptions.TaskContainer()
      {
        Task = ((object) new SmsComposeTask()
        {
          To = (strArray.Length != 0 ? strArray[0] : ""),
          Body = string.Format(AppResources.TellFriendBodyShort, (object) "https://whatsapp.com/dl/")
        }),
        Description = AppResources.SMS
      }.Show();
    }

    private void ViewContext_Tap(object sender, EventArgs e)
    {
      Message msg = this.viewModel.Message;
      ChatPage.NextInstanceInitState = new ChatPage.InitState()
      {
        MessageLoader = MessageLoader.Get(msg.KeyRemoteJid, new int?(), 0, targetInitialLandingMsgId: new int?(msg.MessageID))
      };
      this.Dispatcher.BeginInvoke((Action) (() => NavUtils.NavigateToChat(msg.KeyRemoteJid, false)));
    }

    private void ViewBulkCards_Tap(object sender, EventArgs e)
    {
      Message message = this.viewModel.Message;
      if (message.MediaWaType != FunXMPP.FMessage.Type.Contact)
        return;
      BulkContactCardViewPage.Start(message.MediaName, message.GetContactCards());
    }

    private void SendMessage_Tap(object sender, EventArgs e)
    {
      Message message = this.viewModel.Message;
      if (message.MediaWaType != FunXMPP.FMessage.Type.Contact)
        return;
      ContactVCard contactVcard = ContactVCard.Create(message.Data);
      string[] strArray = contactVcard == null ? new string[0] : ((IEnumerable<ContactVCard.PhoneNumber>) contactVcard.PhoneNumbers).Where<ContactVCard.PhoneNumber>((Func<ContactVCard.PhoneNumber, bool>) (pn => pn.Jid != null && JidHelper.IsUserJid(pn.Jid))).Select<ContactVCard.PhoneNumber, string>((Func<ContactVCard.PhoneNumber, string>) (pn => pn.Jid)).ToArray<string>();
      if (strArray.Length == 1)
      {
        NavUtils.NavigateToChat(strArray[0], false);
      }
      else
      {
        if (strArray.Length <= 1)
          return;
        ContextMenu contextMenu = new ContextMenu()
        {
          IsZoomEnabled = false
        };
        contextMenu.ItemsSource = (IEnumerable) ((IEnumerable<ContactVCard.PhoneNumber>) contactVcard.PhoneNumbers).Where<ContactVCard.PhoneNumber>((Func<ContactVCard.PhoneNumber, bool>) (pn => pn.Jid != null && JidHelper.IsUserJid(pn.Jid))).Select<ContactVCard.PhoneNumber, MenuItem>((Func<ContactVCard.PhoneNumber, MenuItem>) (pn =>
        {
          MenuItem menuItem = new MenuItem();
          menuItem.Header = (object) string.Format("{0} {1}", (object) pn.Kind.ToLocalizedString(), (object) JidHelper.GetPhoneNumber(pn.Jid, true));
          menuItem.Tag = (object) pn.Jid;
          menuItem.Click += new RoutedEventHandler(this.SendMessageMenuItem_Tap);
          return menuItem;
        }));
        Grid msgBtn = this.sendMessageButton;
        if (msgBtn != null)
        {
          ContextMenuService.SetContextMenu((DependencyObject) msgBtn, contextMenu);
          contextMenu.Closed += (RoutedEventHandler) ((_, __) => ContextMenuService.SetContextMenu((DependencyObject) msgBtn, (ContextMenu) null));
        }
        contextMenu.IsOpen = true;
      }
    }

    private void SendMessageMenuItem_Tap(object sender, EventArgs e)
    {
      if (!(sender is FrameworkElement frameworkElement) || !(frameworkElement.Tag is string tag))
        return;
      NavUtils.NavigateToChat(tag, false);
    }
  }
}
