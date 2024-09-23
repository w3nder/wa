// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactSupportOutagePageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System.Collections.Generic;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class ContactSupportOutagePageViewModel : PageViewModelBase
  {
    private ServerStatus status_;
    private string outagesHeader_;
    private List<string> outagesList_;
    private string outagesFooter_;

    public ContactSupportOutagePageViewModel(ServerStatus status, PageOrientation orientation)
      : base(orientation)
    {
      this.status_ = status;
      this.SetOutageMessage();
    }

    private void SetOutageMessage()
    {
      if (this.status_ == null)
        return;
      string header;
      List<string> failureList;
      string footer;
      this.status_.GetStatusMessage(out header, out failureList, out footer);
      if (header != null)
      {
        this.outagesHeader_ = header;
        this.NotifyPropertyChanged("OutagesHeader");
      }
      if (failureList != null)
      {
        this.outagesList_ = failureList;
        this.NotifyPropertyChanged("OutagesList");
      }
      if (footer == null)
        return;
      this.outagesFooter_ = footer;
      this.NotifyPropertyChanged("OutagesFooter");
    }

    public string OutagesHeader => this.outagesHeader_ ?? "";

    public List<string> OutagesList => this.outagesList_ ?? new List<string>();

    public string OutagesFooter => this.outagesFooter_ ?? "";

    public string ButtonText
    {
      get
      {
        if (this.status_ == null)
          return "";
        return !this.status_.Email.Available ? AppResources.DismissButton : AppResources.Continue;
      }
    }

    public bool CanContinue => this.status_.CanContinue();
  }
}
