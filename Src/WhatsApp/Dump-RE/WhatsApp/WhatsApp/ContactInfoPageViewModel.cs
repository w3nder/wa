// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactInfoPageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class ContactInfoPageViewModel : PageViewModelBase
  {
    private ContactInfoPageData data;

    public override string PageTitle
    {
      get
      {
        UserStatus user = this.data?.TargetWaAccount;
        bool checkMark = false;
        string r = user == null || !VerifiedNameRules.IsApplicable(user) ? this.data?.GetDisplayName() ?? "" : VerifiedNameRules.GetFirstInfoName(user, out checkMark);
        if (user != null && !checkMark && Settings.IsWaAdmin)
          ContactsContext.Instance((Action<ContactsContext>) (db =>
          {
            UserStatus userStatus = db.GetUserStatus(user.Jid, false);
            if (userStatus == null)
              return;
            r = string.Format("{0} [ph:{1}, wa:{2}]", (object) r, (object) userStatus.IsInDeviceContactList, (object) userStatus.IsWaUser);
          }));
        return r;
      }
    }

    public ContactInfoPageViewModel(ContactInfoPageData pageData, PageOrientation orientation)
      : base(orientation)
    {
      this.data = pageData;
    }
  }
}
