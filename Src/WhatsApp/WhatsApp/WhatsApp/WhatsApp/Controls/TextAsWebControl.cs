// Decompiled with JetBrains decompiler
// Type: WhatsApp.Controls.TextAsWebControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace WhatsApp.Controls
{
  public class TextAsWebControl : UserControl
  {
    public static readonly DependencyProperty TextFileProperty = DependencyProperty.Register(nameof (TextFile), typeof (string), typeof (TextAsWebControl), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty HtmlFontSizeProperty = DependencyProperty.Register(nameof (HtmlFontSize), typeof (int), typeof (TextAsWebControl), new PropertyMetadata((object) 5));
    internal Grid LayoutRoot;
    internal WebBrowser Content;
    private bool _contentLoaded;

    public string TextFile
    {
      get => this.GetValue(TextAsWebControl.TextFileProperty) as string;
      set => this.SetValue(TextAsWebControl.TextFileProperty, (object) value);
    }

    private void OnTextFileChanged(string filename) => this.TextFile = filename;

    public int HtmlFontSize
    {
      get => (int) this.GetValue(TextAsWebControl.HtmlFontSizeProperty);
      set => this.SetValue(TextAsWebControl.HtmlFontSizeProperty, (object) value);
    }

    public TextAsWebControl()
    {
      this.InitializeComponent();
      this.Loaded += new RoutedEventHandler(this.TextRenderToWebControl_Loaded);
    }

    private void TextRenderToWebControl_Loaded(object sender, RoutedEventArgs e)
    {
      if (this.TextFile == null)
        return;
      try
      {
        string end = new StreamReader(this.TextFile).ReadToEnd();
        CultureInfo cult = new CultureInfo(AppResources.CultureString);
        string str1;
        if (!cult.IsRightToLeft())
          str1 = "<body><font size='" + (object) this.HtmlFontSize + "'>" + end;
        else
          str1 = "<body dir='rtl'><font size='" + (object) this.HtmlFontSize + "'>" + end;
        string str2 = str1;
        double actualWidth = Application.Current.Host.Content.ActualWidth;
        this.Content.NavigateToString("<html><head><meta name='viewport' content='width=" + (object) actualWidth + ", user-scalable=yes' /></head>" + str2 + "</font></body></html>");
        if (!cult.IsRightToLeft())
          return;
        this.Content.RenderTransform = (Transform) new ScaleTransform()
        {
          CenterX = (actualWidth / 2.0),
          ScaleX = -1.0
        };
      }
      catch (FileNotFoundException ex)
      {
        Log.l("TextRenderToWeb", "File not found - {0}", (object) ex.FileName);
      }
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/TextAsWebControl.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.Content = (WebBrowser) this.FindName("Content");
    }
  }
}
