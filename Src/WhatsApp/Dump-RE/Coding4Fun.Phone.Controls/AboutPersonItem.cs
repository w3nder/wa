// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.AboutPersonItem
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using Coding4Fun.Phone.Controls.Data;
using Microsoft.Phone.Tasks;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  public class AboutPersonItem : Control
  {
    private const string EmailAddressName = "emailAddress";
    private const string WebsiteName = "website";
    private const string AuthorTxtBlockName = "author";
    private const string Http = "http://";
    private const string Https = "https://";
    private const string Twitter = "www.twitter.com";
    private TextBlock _emailAddress;
    private TextBlock _website;
    private TextBlock _author;
    private string _webSiteUrl;
    public static readonly DependencyProperty WebSiteDisplayProperty = DependencyProperty.Register(nameof (WebSiteDisplay), typeof (string), typeof (AboutPersonItem), new PropertyMetadata((object) ""));
    public static readonly DependencyProperty RoleProperty = DependencyProperty.Register(nameof (Role), typeof (string), typeof (AboutPersonItem), new PropertyMetadata((object) ""));
    public static readonly DependencyProperty EmailAddressProperty = DependencyProperty.Register(nameof (EmailAddress), typeof (string), typeof (AboutPersonItem), new PropertyMetadata((object) ""));
    public static readonly DependencyProperty AuthorNameProperty = DependencyProperty.Register(nameof (AuthorName), typeof (string), typeof (AboutPersonItem), new PropertyMetadata((object) ""));

    public AboutPersonItem() => this.DefaultStyleKey = (object) typeof (AboutPersonItem);

    public virtual void OnApplyTemplate()
    {
      ((FrameworkElement) this).OnApplyTemplate();
      if (this._website != null)
        ((UIElement) this._website).ManipulationCompleted -= new EventHandler<ManipulationCompletedEventArgs>(this.websiteClick_ManipulationCompleted);
      if (this._emailAddress != null)
        ((UIElement) this._emailAddress).ManipulationCompleted -= new EventHandler<ManipulationCompletedEventArgs>(this.email_ManipulationCompleted);
      this._emailAddress = this.GetTemplateChild("emailAddress") as TextBlock;
      this._website = this.GetTemplateChild("website") as TextBlock;
      this._author = this.GetTemplateChild("author") as TextBlock;
      AboutPersonItem.SetVisibility(this._emailAddress);
      AboutPersonItem.SetVisibility(this._website);
      AboutPersonItem.SetVisibility(this._author);
      if (this._emailAddress != null)
        ((UIElement) this._emailAddress).ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(this.email_ManipulationCompleted);
      if (this._website == null)
        return;
      ((UIElement) this._website).ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(this.websiteClick_ManipulationCompleted);
    }

    private static void SetVisibility(TextBlock control)
    {
      if (control == null)
        return;
      ((UIElement) control).Visibility = string.IsNullOrEmpty(control.Text) ? (Visibility) 1 : (Visibility) 0;
    }

    public string WebSiteUrl
    {
      get => this._webSiteUrl;
      set
      {
        this._webSiteUrl = value;
        this.WebSiteDisplay = value;
        AboutPersonItem.SetVisibility(this._website);
      }
    }

    protected internal string WebSiteDisplay
    {
      get => (string) ((DependencyObject) this).GetValue(AboutPersonItem.WebSiteDisplayProperty);
      set
      {
        if (value != null)
        {
          value = value.ToLowerInvariant().TrimEnd('/');
          if (value.StartsWith("https://"))
            value = value.Remove(0, "https://".Length);
          if (value.StartsWith("http://"))
            value = value.Remove(0, "http://".Length);
          if (!string.IsNullOrEmpty(value) && value.StartsWith("www.twitter.com"))
            value = "@" + value.Remove(0, "www.twitter.com".Length).TrimStart('/');
        }
        ((DependencyObject) this).SetValue(AboutPersonItem.WebSiteDisplayProperty, (object) value);
      }
    }

    public string Role
    {
      get => (string) ((DependencyObject) this).GetValue(AboutPersonItem.RoleProperty);
      set
      {
        if (value != null)
          value = value.ToLowerInvariant();
        ((DependencyObject) this).SetValue(AboutPersonItem.RoleProperty, (object) value);
      }
    }

    public string EmailAddress
    {
      get => (string) ((DependencyObject) this).GetValue(AboutPersonItem.EmailAddressProperty);
      set
      {
        if (value != null)
          value = value.ToLowerInvariant();
        ((DependencyObject) this).SetValue(AboutPersonItem.EmailAddressProperty, (object) value);
        AboutPersonItem.SetVisibility(this._emailAddress);
      }
    }

    protected internal void email_ManipulationCompleted(
      object sender,
      ManipulationCompletedEventArgs e)
    {
      new EmailComposeTask()
      {
        To = this.EmailAddress,
        Subject = (PhoneHelper.GetAppAttribute("Title") + " Feedback")
      }.Show();
    }

    public string AuthorName
    {
      get => (string) ((DependencyObject) this).GetValue(AboutPersonItem.AuthorNameProperty);
      set
      {
        if (value != null)
          value = value.ToLowerInvariant();
        ((DependencyObject) this).SetValue(AboutPersonItem.AuthorNameProperty, (object) value);
        AboutPersonItem.SetVisibility(this._author);
      }
    }

    protected internal void websiteClick_ManipulationCompleted(
      object sender,
      ManipulationCompletedEventArgs e)
    {
      AboutPersonItem.NavigateTo(this.WebSiteUrl);
    }

    private static void NavigateTo(string uri)
    {
      new WebBrowserTask() { Uri = new Uri(uri) }.Show();
    }
  }
}
