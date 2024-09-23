// Decompiled with JetBrains decompiler
// Type: WhatsApp.EULA
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;


namespace WhatsApp
{
  public class EULA : PhoneApplicationPage
  {
    internal Grid LayoutRoot;
    internal Grid SysTrayPlaceHolder;
    internal PageTitlePanel PageTitle;
    internal StackPanel ContentPanel;
    internal RichTextBlock TosText;
    internal RoundButton ButtonIcon;
    internal TextBlock ButtonText;
    private bool _contentLoaded;

    public EULA()
    {
      this.InitializeComponent();
      SysTrayHelper.SetForegroundColor((DependencyObject) this, Constants.SysTrayOffWhite);
      this.ButtonIcon.ButtonIcon = (BitmapSource) ImageStore.GetStockIcon("/Images/next.png");
      this.ButtonText.Text = AppResources.AcceptEula;
      this.PageTitle.Mode = PageTitlePanel.Modes.Zoomed;
      this.PageTitle.SmallTitle = AppResources.WelcomeTitle;
      this.SysTrayPlaceHolder.Height = UIUtils.SystemTraySizePortrait;
      string viewTerms2 = AppResources.ViewTerms2;
      WaRichText.Chunk[] htmlLinkChunks = WaRichText.GetHtmlLinkChunks(viewTerms2);
      if (htmlLinkChunks == null || htmlLinkChunks.Length == 0)
        this.TosText.Text = new RichTextBlock.TextSet()
        {
          Text = viewTerms2
        };
      else if (htmlLinkChunks.Length == 1)
      {
        WaRichText.Chunk chunk = ((IEnumerable<WaRichText.Chunk>) htmlLinkChunks).SingleOrDefault<WaRichText.Chunk>();
        string str = "https://www.whatsapp.com/legal/";
        chunk.AuxiliaryInfo = str;
        chunk.Format = WaRichText.Formats.Link;
        this.TosText.Text = new RichTextBlock.TextSet()
        {
          Text = viewTerms2,
          PartialFormattings = (IEnumerable<WaRichText.Chunk>) new WaRichText.Chunk[1]
          {
            chunk
          }
        };
      }
      else
      {
        if (htmlLinkChunks.Length != 2)
          return;
        string str1 = "https://www.whatsapp.com/legal/#privacy-policy";
        string str2 = "https://www.whatsapp.com/legal/#terms-of-service";
        WaRichText.Chunk[] array = ((IEnumerable<WaRichText.Chunk>) htmlLinkChunks).ToArray<WaRichText.Chunk>();
        array[0].AuxiliaryInfo = str1;
        array[0].Format = WaRichText.Formats.Link;
        array[1].AuxiliaryInfo = str2;
        array[1].Format = WaRichText.Formats.Link;
        this.TosText.Text = new RichTextBlock.TextSet()
        {
          Text = viewTerms2,
          PartialFormattings = (IEnumerable<WaRichText.Chunk>) array
        };
      }
    }

    private void TermsButton_Click(object sender, RoutedEventArgs e)
    {
      new WebBrowserTask()
      {
        Uri = new Uri("https://www.whatsapp.com/legal/")
      }.Show();
    }

    private void AcceptButton_Click(object sender, EventArgs e)
    {
      Settings.EULAAcceptedUtc = new DateTime?(DateTime.UtcNow);
      NavUtils.NavigateHome(this.NavigationService);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/EULA.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.SysTrayPlaceHolder = (Grid) this.FindName("SysTrayPlaceHolder");
      this.PageTitle = (PageTitlePanel) this.FindName("PageTitle");
      this.ContentPanel = (StackPanel) this.FindName("ContentPanel");
      this.TosText = (RichTextBlock) this.FindName("TosText");
      this.ButtonIcon = (RoundButton) this.FindName("ButtonIcon");
      this.ButtonText = (TextBlock) this.FindName("ButtonText");
    }
  }
}
