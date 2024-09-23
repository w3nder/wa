// Decompiled with JetBrains decompiler
// Type: WhatsApp.Pages.ChatCountItemControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp.Pages
{
  public class ChatCountItemControl : ChatItemControl
  {
    private LongListSelector chatInfoList;
    protected Grid chatInfoPanel;

    protected override void InitComponents()
    {
      base.InitComponents();
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = new GridLength(1.0, GridUnitType.Auto)
      });
      this.subtitleRow.Visibility = Visibility.Collapsed;
      this.titleRow.Margin = new Thickness(0.0, 16.0, 0.0, 0.0);
      this.chatInfoPanel = new Grid();
      this.Children.Add((UIElement) this.chatInfoPanel);
      Grid.SetColumn((FrameworkElement) this.chatInfoPanel, 1);
      Grid.SetRow((FrameworkElement) this.chatInfoPanel, 1);
      LongListSelector longListSelector = new LongListSelector();
      longListSelector.IsGroupingEnabled = false;
      longListSelector.Visibility = Visibility.Collapsed;
      this.chatInfoList = longListSelector;
      this.chatInfoList.ItemTemplate = XamlReader.Load("\r\n                <DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\r\n                                xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\r\n                                xmlns:local=\"clr-namespace:WhatsApp;assembly=WhatsApp\">\r\n                    <Grid>\r\n                        <Grid.ColumnDefinitions>\r\n                            <ColumnDefinition Width=\"2*\"/>\r\n                            <ColumnDefinition Width=\"1*\"/>\r\n                            <ColumnDefinition Width=\"2*\"/>\r\n                        </Grid.ColumnDefinitions>\r\n                        <TextBlock Text=\"{Binding Title}\" Grid.Column=\"0\" \r\n                                   FontSize=\"{StaticResource PhoneFontSizeSmall}\" Style=\"{StaticResource PhoneTextSubtleStyle}\"/>\r\n                        <TextBlock Text=\"{Binding Count}\" Grid.Column=\"1\" TextAlignment=\"Right\" \r\n                                   FontSize=\"{StaticResource PhoneFontSizeSmall}\" Style=\"{StaticResource PhoneTextSubtleStyle}\"/>\r\n                        <TextBlock Text=\"{Binding FormattedSize}\" Grid.Column=\"2\" TextAlignment=\"Right\"\r\n                                   FontSize=\"{StaticResource PhoneFontSizeSmall}\" Style=\"{StaticResource PhoneTextSubtleStyle}\"/>\r\n                    </Grid>\r\n                </DataTemplate>\r\n            ") as DataTemplate;
      this.chatInfoPanel.Children.Add((UIElement) this.chatInfoList);
      this.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.ChatCountItemControl_Tap);
    }

    protected override void UpdateComponents(JidItemViewModel vm)
    {
      base.UpdateComponents(vm);
      if (!(vm is ChatCountsPage.ChatCountItemViewModel))
        return;
      this.chatInfoList.ItemsSource = (IList) null;
      this.chatInfoList.Visibility = Visibility.Collapsed;
      this.timestampBlock.Visibility = Visibility.Visible;
    }

    private void ChatCountItemControl_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      ChatCountsPage.ChatCountItemViewModel viewModel = (sender as ChatCountItemControl).ViewModel as ChatCountsPage.ChatCountItemViewModel;
      viewModel.IsExpanded = !viewModel.IsExpanded;
      if (viewModel.IsExpanded)
      {
        this.chatInfoList.ItemsSource = (IList) viewModel.ChatDetails;
        this.chatInfoList.Visibility = Visibility.Visible;
        this.timestampBlock.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.chatInfoList.ItemsSource = (IList) null;
        this.chatInfoList.Visibility = Visibility.Collapsed;
        this.timestampBlock.Visibility = Visibility.Visible;
      }
    }
  }
}
