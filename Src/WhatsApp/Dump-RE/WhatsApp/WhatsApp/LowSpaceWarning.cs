// Decompiled with JetBrains decompiler
// Type: WhatsApp.LowSpaceWarning
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace WhatsApp
{
  public class LowSpaceWarning : PhoneApplicationPage
  {
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal StackPanel ContentPanel;
    internal TextBlock Content;
    internal Button OkButton;
    private bool _contentLoaded;

    public LowSpaceWarning()
    {
      this.InitializeComponent();
      this.TitlePanel.LargeTitle = AppResources.LowStorageSpaceTitle;
      this.Content.Text = AppResources.LowStorageSpace;
      if (NativeInterfaces.Misc.GetDiskSpace(Constants.IsoStorePath).FreeBytes / 1024UL / 1024UL >= 15UL)
        return;
      this.TitlePanel.LargeTitle = AppResources.CriticalStorageSpaceTitle;
      this.Content.Text = AppResources.CriticalStorageSpace;
      this.OkButton.Visibility = Visibility.Collapsed;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      NavUtils.GoBack(this.NavigationService);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/LowSpaceWarning.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.ContentPanel = (StackPanel) this.FindName("ContentPanel");
      this.Content = (TextBlock) this.FindName("Content");
      this.OkButton = (Button) this.FindName("OkButton");
    }
  }
}
