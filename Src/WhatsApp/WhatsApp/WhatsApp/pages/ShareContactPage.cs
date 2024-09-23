// Decompiled with JetBrains decompiler
// Type: WhatsApp.ShareContactPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public class ShareContactPage : PhoneApplicationPage
  {
    private static IEnumerable<ContactVCard> NextInstanceVCards;
    private static IObserver<ContactVCard> NextInstanceObserver;
    private static bool NextInstanceReadOnly;
    private IEnumerable<ContactVCard> vCards;
    private IObserver<ContactVCard> pageObserver;
    private bool isReadOnly;
    private ApplicationBarIconButton sendButton;
    private ShareContactPageViewModel viewModel;
    private List<ContactDataViewModel> dataItems;
    internal Grid LayoutRoot;
    internal WhatsApp.CompatibilityShims.LongListSelector ContactItemsList;
    internal Grid PicturePanel;
    internal Image ContactPicture;
    internal Polygon CheckmarkBackground;
    internal Image Checkmark;
    private bool _contentLoaded;

    public ShareContactPage()
    {
      this.InitializeComponent();
      this.vCards = ShareContactPage.NextInstanceVCards;
      this.pageObserver = ShareContactPage.NextInstanceObserver;
      this.isReadOnly = ShareContactPage.NextInstanceReadOnly;
      ShareContactPage.NextInstanceVCards = (IEnumerable<ContactVCard>) null;
      ShareContactPage.NextInstanceObserver = (IObserver<ContactVCard>) null;
      ShareContactPage.NextInstanceReadOnly = false;
      this.DataContext = (object) (this.viewModel = new ShareContactPageViewModel(this.vCards, this.Orientation));
      this.Init();
    }

    public static IObservable<ContactVCard> Start(Contact contact, bool readOnly = false, bool replacePage = false)
    {
      if (contact == null)
        return Observable.Empty<ContactVCard>();
      return ShareContactPage.Start((IEnumerable<ContactVCard>) new ContactVCard[1]
      {
        ContactVCard.Create(contact)
      }, readOnly, replacePage);
    }

    public static IObservable<ContactVCard> Start(
      ContactVCard vCard,
      bool readOnly = false,
      bool replacePage = false)
    {
      return ShareContactPage.Start((IEnumerable<ContactVCard>) new ContactVCard[1]
      {
        vCard
      }, readOnly, replacePage);
    }

    public static IObservable<ContactVCard> Start(
      IEnumerable<ContactVCard> vCards,
      bool readOnly = false,
      bool replacePage = false)
    {
      return vCards == null || !vCards.Any<ContactVCard>() ? Observable.Empty<ContactVCard>() : Observable.Create<ContactVCard>((Func<IObserver<ContactVCard>, Action>) (observer =>
      {
        ShareContactPage.NextInstanceVCards = vCards;
        ShareContactPage.NextInstanceObserver = observer;
        ShareContactPage.NextInstanceReadOnly = readOnly;
        WaUriParams uriParams = new WaUriParams();
        if (replacePage)
          uriParams.AddBool("PageReplace", replacePage);
        NavUtils.NavigateToPage(nameof (ShareContactPage), uriParams);
        return (Action) (() => { });
      }));
    }

    private void InitAppBar()
    {
      this.sendButton = new ApplicationBarIconButton()
      {
        IconUri = new Uri("/Images/sendmsg.png", UriKind.Relative),
        Text = "Send"
      };
      this.sendButton.Click += new EventHandler(this.SubmitButton_Tap);
      Microsoft.Phone.Shell.ApplicationBar bar = new Microsoft.Phone.Shell.ApplicationBar();
      bar.Buttons.Add((object) this.sendButton);
      Localizable.LocalizeAppBar(bar);
      this.ApplicationBar = (IApplicationBar) bar;
    }

    private List<ContactDataViewModel> GetBulkContactDataItems()
    {
      LinkedList<ContactDataViewModel> linkedList = new LinkedList<ContactDataViewModel>();
      this.AggregateVCardField(this.vCards, linkedList, (Func<ContactVCard, bool>) (c => !string.IsNullOrEmpty(c.Nickname)), (Func<ContactVCard, string>) (c => c.Nickname), "NICKNAME", AppResources.Nickname);
      this.AggregateVCardField(this.vCards, linkedList, (Func<ContactVCard, bool>) (c => !string.IsNullOrEmpty(c.CompanyName)), (Func<ContactVCard, string>) (c => c.CompanyName), "ORG", AppResources.Company);
      this.AggregateVCardField(this.vCards, linkedList, (Func<ContactVCard, bool>) (c => !string.IsNullOrEmpty(c.JobTitle)), (Func<ContactVCard, string>) (c => c.JobTitle), "TITLE", AppResources.JobTitle);
      Set<PhoneNumberKind> set1 = new Set<PhoneNumberKind>();
      Set<AddressKind> set2 = new Set<AddressKind>();
      foreach (ContactVCard vCard in this.vCards)
      {
        foreach (ContactVCard.PhoneNumber phoneNumber in vCard.PhoneNumbers)
        {
          if (!set1.Contains(phoneNumber.Kind))
            set1.Add(phoneNumber.Kind);
        }
      }
      foreach (PhoneNumberKind phoneNumberKind in set1)
      {
        PhoneNumberKind k = phoneNumberKind;
        List<string> strs = new List<string>();
        foreach (ContactVCard vCard in this.vCards)
        {
          foreach (ContactVCard.PhoneNumber phoneNumber in vCard.PhoneNumbers)
          {
            if (phoneNumber.Kind == k)
              strs.Add(phoneNumber.Number);
          }
        }
        linkedList.AddLast(new ContactDataViewModel("TEL", k.ToLocalizedString(), Utils.CommaSeparate((IEnumerable<string>) strs), !this.isReadOnly, (Func<object, bool>) (phoneNumber => phoneNumber != null && phoneNumber is ContactVCard.PhoneNumber phoneNumber1 && phoneNumber1.Kind == k)));
      }
      this.AggregateVCardField(this.vCards, linkedList, (Func<ContactVCard, bool>) (c => ((IEnumerable<ContactVCard.EmailAddress>) c.EmailAddresses).Any<ContactVCard.EmailAddress>()), (Func<ContactVCard, string>) (c => ((IEnumerable<ContactVCard.EmailAddress>) c.EmailAddresses).First<ContactVCard.EmailAddress>().Address), "EMAIL", AppResources.EmailHeader);
      foreach (ContactVCard vCard in this.vCards)
      {
        foreach (ContactVCard.Address address in vCard.Addresses)
        {
          if (!set2.Contains(address.Kind))
            set2.Add(address.Kind);
        }
      }
      foreach (AddressKind addressKind in set2)
      {
        AddressKind k = addressKind;
        List<string> strs = new List<string>();
        foreach (ContactVCard vCard in this.vCards)
        {
          foreach (ContactVCard.Address address in vCard.Addresses)
          {
            if (address.Kind == k)
              strs.Add(this.AddressToString(address));
          }
        }
        linkedList.AddLast(new ContactDataViewModel("ADDR", k.ToString().ToLower(), Utils.CommaSeparate((IEnumerable<string>) strs), !this.isReadOnly, (Func<object, bool>) (address => address != null && address is ContactVCard.Address address1 && address1.Kind == k)));
      }
      this.AggregateVCardField(this.vCards, linkedList, (Func<ContactVCard, bool>) (c => c.Birthday != new DateTime()), (Func<ContactVCard, string>) (c => c.Birthday.ToShortDateString()), "BDAY", AppResources.Birthday);
      this.AggregateVCardField(this.vCards, linkedList, (Func<ContactVCard, bool>) (c => ((IEnumerable<string>) c.Websites).Any<string>()), (Func<ContactVCard, string>) (c => ((IEnumerable<string>) c.Websites).First<string>()), "URL", AppResources.Website);
      if (!this.isReadOnly)
      {
        foreach (ContactDataViewModel contactDataViewModel in linkedList)
          contactDataViewModel.IsChecked = true;
      }
      this.AggregateVCardField(this.vCards, linkedList, (Func<ContactVCard, bool>) (c => !string.IsNullOrEmpty(c.JobTitle)), (Func<ContactVCard, string>) (c => c.JobTitle), "TITLE", AppResources.JobTitle);
      foreach (ContactVCard vCard in this.vCards)
      {
        if (!string.IsNullOrEmpty(vCard.Note))
        {
          linkedList.AddLast(new ContactDataViewModel("NOTE", AppResources.Note, vCard.Note)
          {
            IsChecked = false
          });
          break;
        }
      }
      return linkedList.ToList<ContactDataViewModel>();
    }

    private void AggregateVCardField(
      IEnumerable<ContactVCard> vcards,
      LinkedList<ContactDataViewModel> items,
      Func<ContactVCard, bool> isPopulated,
      Func<ContactVCard, string> getDisplayStr,
      string field,
      string title)
    {
      List<string> stringList = new List<string>();
      foreach (ContactVCard vcard in vcards)
      {
        if (isPopulated(vcard))
          stringList.Add(getDisplayStr(vcard));
      }
      if (!stringList.Any<string>())
        return;
      items.AddLast(new ContactDataViewModel(field, title, Utils.CommaSeparate((IEnumerable<string>) stringList), !this.isReadOnly));
    }

    private List<ContactDataViewModel> GetContactDataItems(ContactVCard vCard)
    {
      if (vCard == null)
        return new List<ContactDataViewModel>();
      LinkedList<ContactDataViewModel> source = new LinkedList<ContactDataViewModel>();
      string displayName = vCard.GetDisplayName(false);
      if (!string.IsNullOrEmpty(displayName))
        source.AddLast(new ContactDataViewModel("N", AppResources.Name, displayName, false));
      if (!string.IsNullOrEmpty(vCard.Nickname))
        source.AddLast(new ContactDataViewModel("NICKNAME", AppResources.Nickname, vCard.Nickname, !this.isReadOnly));
      if (!string.IsNullOrEmpty(vCard.CompanyName))
        source.AddLast(new ContactDataViewModel("ORG", AppResources.Company, vCard.CompanyName, !this.isReadOnly));
      if (!string.IsNullOrEmpty(vCard.JobTitle))
        source.AddLast(new ContactDataViewModel("TITLE", AppResources.JobTitle, vCard.JobTitle, !this.isReadOnly));
      if (vCard.PhoneNumbers != null)
      {
        foreach (ContactVCard.PhoneNumber phoneNumber in vCard.PhoneNumbers)
          source.AddLast(new ContactDataViewModel("TEL", phoneNumber.Kind.ToLocalizedString(), phoneNumber.Number, !this.isReadOnly));
      }
      if (vCard.EmailAddresses != null)
      {
        foreach (ContactVCard.EmailAddress emailAddress in vCard.EmailAddresses)
          source.AddLast(new ContactDataViewModel("EMAIL", AppResources.EmailHeader, emailAddress.Address, !this.isReadOnly));
      }
      if (vCard.Addresses != null)
      {
        foreach (ContactVCard.Address address in vCard.Addresses)
          source.AddLast(new ContactDataViewModel("ADDR", address.Kind.ToString().ToLower(), this.AddressToString(address), !this.isReadOnly));
      }
      DateTime birthday = vCard.Birthday;
      if (birthday != new DateTime())
        source.AddLast(new ContactDataViewModel("BDAY", AppResources.Birthday, birthday.ToShortDateString(), !this.isReadOnly));
      if (vCard.Websites != null)
      {
        foreach (string website in vCard.Websites)
          source.AddLast(new ContactDataViewModel("URL", AppResources.Website, website, !this.isReadOnly));
      }
      if (!this.isReadOnly)
      {
        foreach (ContactDataViewModel contactDataViewModel in source)
          contactDataViewModel.IsChecked = true;
      }
      string note = vCard.Note;
      if (!string.IsNullOrEmpty(note))
        source.AddLast(new ContactDataViewModel("NOTE", AppResources.Note, note)
        {
          IsChecked = false
        });
      return source.ToList<ContactDataViewModel>();
    }

    private string AddressToString(ContactVCard.Address addr)
    {
      return string.Format("{0}\n{1} {2} {3}\n{4}", (object) addr.Street.Trim(), (object) addr.City, (object) addr.State, (object) addr.PostalCode, (object) addr.Country);
    }

    private void UpdateSendButton()
    {
      this.Dispatcher.BeginInvoke((Action) (() => this.sendButton.IsEnabled = this.viewModel.IsPictureSelected.HasValue && this.viewModel.IsPictureSelected.Value || this.dataItems.Any<ContactDataViewModel>((Func<ContactDataViewModel, bool>) (item => item.IsChecked))));
    }

    private void Init()
    {
      if (this.pageObserver == null || this.vCards == null || this.viewModel == null)
        return;
      this.ContactItemsList.ItemsSource = (IList) (this.dataItems = this.vCards.Count<ContactVCard>() > 1 ? this.GetBulkContactDataItems() : this.GetContactDataItems(this.vCards.FirstOrDefault<ContactVCard>()));
      if (!this.isReadOnly)
        this.InitAppBar();
      WriteableBitmap photoBitmap = this.vCards.FirstOrDefault<ContactVCard>().GetPhotoBitmap();
      if (photoBitmap == null)
      {
        this.PicturePanel.Visibility = Visibility.Collapsed;
        this.viewModel.IsPictureSelected = new bool?();
      }
      else
      {
        this.ContactPicture.Source = (System.Windows.Media.ImageSource) photoBitmap;
        if (this.isReadOnly)
        {
          this.CheckmarkBackground.Visibility = this.Checkmark.Visibility = Visibility.Collapsed;
        }
        else
        {
          this.viewModel.IsPictureSelected = new bool?(true);
          this.Checkmark.Source = (System.Windows.Media.ImageSource) ImageStore.SelectedCheckMark;
          this.CheckmarkBackground.Fill = (Brush) UIUtils.AccentBrush;
        }
        this.PicturePanel.Visibility = Visibility.Visible;
      }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (this.pageObserver != null && this.vCards != null && this.viewModel != null)
        return;
      this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      if (this.pageObserver != null)
        this.pageObserver.OnCompleted();
      base.OnRemovedFromJournal(e);
    }

    public void SubmitButton_Tap(object sender, EventArgs e)
    {
      if (this.pageObserver == null || this.viewModel == null || this.vCards == null)
      {
        NavUtils.GoBack();
      }
      else
      {
        if (this.viewModel.IsPictureSelected.HasValue && !this.viewModel.IsPictureSelected.Value)
        {
          foreach (ContactVCard vCard in this.vCards)
            vCard.Photo = (string) null;
        }
        foreach (ContactVCard vCard in this.vCards)
          this.RemoveUnselectedInfo(vCard);
        foreach (ContactVCard vCard in this.vCards)
          this.pageObserver.OnNext(vCard);
        this.pageObserver.OnCompleted();
      }
    }

    private void RemoveUnselectedInfo(ContactVCard vcard)
    {
      if (vcard == null)
        return;
      foreach (ContactDataViewModel contactDataViewModel in this.dataItems.Where<ContactDataViewModel>((Func<ContactDataViewModel, bool>) (item => !item.IsChecked)))
      {
        ContactDataViewModel item = contactDataViewModel;
        switch (item.VCardType)
        {
          case "ADDR":
            if (item.ShouldKeepValue != null)
            {
              vcard.Addresses = ((IEnumerable<ContactVCard.Address>) vcard.Addresses).Where<ContactVCard.Address>((Func<ContactVCard.Address, bool>) (p => item.ShouldKeepValue((object) p))).ToArray<ContactVCard.Address>();
              continue;
            }
            List<ContactVCard.Address> list1 = ((IEnumerable<ContactVCard.Address>) vcard.Addresses).ToList<ContactVCard.Address>();
            for (int index = 0; index < list1.Count; ++index)
            {
              if (this.AddressToString(list1[index]) == item.Value)
              {
                list1.RemoveAt(index);
                break;
              }
            }
            vcard.Addresses = list1.ToArray();
            continue;
          case "BDAY":
            vcard.Birthday = new DateTime();
            continue;
          case "EMAIL":
            if (this.vCards.Count<ContactVCard>() > 1)
            {
              vcard.EmailAddresses = new ContactVCard.EmailAddress[0];
              continue;
            }
            List<ContactVCard.EmailAddress> list2 = ((IEnumerable<ContactVCard.EmailAddress>) vcard.EmailAddresses).ToList<ContactVCard.EmailAddress>();
            for (int index = 0; index < list2.Count; ++index)
            {
              if (list2[index].Address == item.Value)
              {
                list2.RemoveAt(index);
                break;
              }
            }
            vcard.EmailAddresses = list2.ToArray();
            continue;
          case "NICKNAME":
            vcard.Nickname = (string) null;
            continue;
          case "NOTE":
            vcard.Note = (string) null;
            continue;
          case "ORG":
            vcard.CompanyName = (string) null;
            continue;
          case "TEL":
            if (item.ShouldKeepValue != null)
            {
              vcard.PhoneNumbers = ((IEnumerable<ContactVCard.PhoneNumber>) vcard.PhoneNumbers).Where<ContactVCard.PhoneNumber>((Func<ContactVCard.PhoneNumber, bool>) (p => item.ShouldKeepValue((object) p))).ToArray<ContactVCard.PhoneNumber>();
              continue;
            }
            List<ContactVCard.PhoneNumber> list3 = ((IEnumerable<ContactVCard.PhoneNumber>) vcard.PhoneNumbers).ToList<ContactVCard.PhoneNumber>();
            for (int index = 0; index < list3.Count; ++index)
            {
              if (list3[index].Number == item.Value)
              {
                list3.RemoveAt(index);
                break;
              }
            }
            vcard.PhoneNumbers = list3.ToArray();
            continue;
          case "TITLE":
            vcard.JobTitle = (string) null;
            continue;
          case "URL":
            if (this.vCards.Count<ContactVCard>() > 1)
            {
              vcard.Websites = new string[0];
              continue;
            }
            List<string> list4 = ((IEnumerable<string>) vcard.Websites).ToList<string>();
            for (int index = 0; index < list4.Count; ++index)
            {
              if (list4[index] == item.Value)
              {
                list4.RemoveAt(index);
                break;
              }
            }
            vcard.Websites = list4.ToArray();
            continue;
          default:
            continue;
        }
      }
    }

    private void ContactPicture_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.viewModel == null || this.isReadOnly)
        return;
      ShareContactPageViewModel viewModel = this.viewModel;
      bool? isPictureSelected = this.viewModel.IsPictureSelected;
      bool? nullable = new bool?(((int) isPictureSelected ?? 0) == 0);
      viewModel.IsPictureSelected = nullable;
      Polygon checkmarkBackground = this.CheckmarkBackground;
      isPictureSelected = this.viewModel.IsPictureSelected;
      SolidColorBrush solidColorBrush;
      if (isPictureSelected.HasValue)
      {
        isPictureSelected = this.viewModel.IsPictureSelected;
        if (isPictureSelected.Value)
        {
          solidColorBrush = UIUtils.AccentBrush;
          goto label_5;
        }
      }
      solidColorBrush = new SolidColorBrush(Colors.Gray);
label_5:
      checkmarkBackground.Fill = (Brush) solidColorBrush;
      this.UpdateSendButton();
    }

    private void ContactDataItem_SelectionChanged(object sender, RoutedEventArgs e)
    {
      this.UpdateSendButton();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/ShareContactPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.ContactItemsList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("ContactItemsList");
      this.PicturePanel = (Grid) this.FindName("PicturePanel");
      this.ContactPicture = (Image) this.FindName("ContactPicture");
      this.CheckmarkBackground = (Polygon) this.FindName("CheckmarkBackground");
      this.Checkmark = (Image) this.FindName("Checkmark");
    }
  }
}
