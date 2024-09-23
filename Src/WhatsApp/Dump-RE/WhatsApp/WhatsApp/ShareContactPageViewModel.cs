// Decompiled with JetBrains decompiler
// Type: WhatsApp.ShareContactPageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System.Collections.Generic;
using System.Linq;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class ShareContactPageViewModel : PageViewModelBase
  {
    private IEnumerable<ContactVCard> vCards;

    public ShareContactPageViewModel(
      IEnumerable<ContactVCard> contactVCards,
      PageOrientation initialOrientation)
      : base(initialOrientation)
    {
      this.vCards = contactVCards;
    }

    public override string PageTitle
    {
      get
      {
        if (this.vCards == null)
          return (string) null;
        return this.vCards.Count<ContactVCard>() <= 1 ? this.vCards.FirstOrDefault<ContactVCard>().GetDisplayName(true) : this.vCards.Count<ContactVCard>().ToString() + " contacts";
      }
    }

    public new string PageSubtitle => AppResources.ShareContactInformation;

    public bool? IsPictureSelected { get; set; }
  }
}
