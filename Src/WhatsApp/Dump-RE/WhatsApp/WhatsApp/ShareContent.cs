// Decompiled with JetBrains decompiler
// Type: WhatsApp.ShareContent
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WhatsAppCommon;

#nullable disable
namespace WhatsApp
{
  public class ShareContent : PhoneApplicationPage
  {
    private bool deeplinkErrorJustShown;
    private bool pageExited;
    private bool isDeepLink;
    private IExternalShare shareContent;
    private const int MIN_PHONENUMBER_LENGTH = 6;
    private const int MAX_PHONENUMBER_LENGTH = 17;
    internal ZoomBox LayoutRootZoomBox;
    internal Grid LayoutRoot;
    internal Grid SysTrayPlaceHolder;
    internal Image WaLogo;
    internal StackPanel LoadingPanel;
    internal ProgressBar LoadingProgressBar;
    internal TextBlock LoadingTextBlock;
    internal TextBlock errorTextBlock;
    internal Button ActionButton;
    internal RoundButton ButtonIcon;
    internal TextBlock ButtonText;
    private bool _contentLoaded;

    public ShareContent() => this.InitializeComponent();

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      Log.d(nameof (ShareContent), "OnNavigatedTo {0}", (object) e.NavigationMode);
      if (e.NavigationMode == NavigationMode.New)
      {
        this.shareContent = ExternalShare.GetSourceForUri(this.NavigationContext.QueryString);
        if (this.shareContent is UriShareContent shareContent && !string.IsNullOrEmpty(shareContent.PhoneNumberFromLink))
        {
          this.isDeepLink = true;
          this.HandleDeepLink(shareContent);
        }
        else
          this.MaybeLaunchRecipientPicker(this.shareContent);
      }
      else
      {
        if (!this.deeplinkErrorJustShown || e.NavigationMode != NavigationMode.Back)
          return;
        this.deeplinkErrorJustShown = false;
        this.isDeepLink = false;
        NavUtils.NavigateHome();
      }
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      Log.d(nameof (ShareContent), "OnNavigatedFrom {0}", (object) e.NavigationMode);
      this.pageExited = e.NavigationMode == NavigationMode.Back;
      base.OnNavigatedFrom(e);
    }

    private void MaybeLaunchRecipientPicker(
      IExternalShare shareContent,
      List<string> selectedJids = null,
      DeepLinkData deepLinkData = null)
    {
      this.WaLogo.Visibility = Visibility.Collapsed;
      this.LoadingPanel.VerticalAlignment = VerticalAlignment.Center;
      Action tryToReturnToInvoker = (Action) (() =>
      {
        if (shareContent is ExternalShare81 externalShare81_2)
        {
          NavUtils.GoBack(false);
          externalShare81_2.ShareOperationInstance.ReportCompleted();
        }
        else
          NavUtils.NavigateHome(this.NavigationService);
      });
      Action<RecipientListPage.RecipientListResults> onNext = (Action<RecipientListPage.RecipientListResults>) (recipientListResults =>
      {
        List<string> selJids = recipientListResults?.SelectedJids;
        Log.l(nameof (ShareContent), "Recipients selected to share: {0}", (object) (selJids == null ? -1 : selJids.Count));
        if (selJids == null || selJids.Count < 1)
        {
          tryToReturnToInvoker();
        }
        else
        {
          IObservable<ExternalShare.ExternalShareResult> obs = ShareContent.MaybeIncludeSizeCheck(shareContent.ShareContent(selJids), shareContent.GetTruncationCheck());
          Observable.Defer<ExternalShare.ExternalShareResult>((Func<IObservable<ExternalShare.ExternalShareResult>>) (() => obs)).ObserveOnDispatcher<ExternalShare.ExternalShareResult>().Subscribe<ExternalShare.ExternalShareResult>((Action<ExternalShare.ExternalShareResult>) (result =>
          {
            switch (result)
            {
              case ExternalShare.ExternalShareResult.Discarded:
                tryToReturnToInvoker();
                break;
              case ExternalShare.ExternalShareResult.Shared:
                if (selJids.Count == 1 && !JidHelper.IsStatusJid(selJids.First<string>()))
                {
                  string selJid = selJids.First<string>();
                  MessageLoader msgLoader = (MessageLoader) null;
                  MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
                  {
                    Conversation conversation = db.GetConversation(selJid, CreateOptions.CreateToDbIfNotFound);
                    if (conversation == null)
                      return;
                    msgLoader = MessageLoader.Get(selJid, conversation.FirstUnreadMessageID, conversation.GetUnreadMessagesCount(), false);
                  }));
                  System.Windows.Media.ImageSource cachedImgSrc = (System.Windows.Media.ImageSource) null;
                  ChatPictureStore.GetCache(selJid, out cachedImgSrc);
                  ChatPage.NextInstanceInitState = new ChatPage.InitState()
                  {
                    ForceInitialScrollToRecent = true,
                    MessageLoader = msgLoader,
                    Picture = cachedImgSrc,
                    SharedDeepLinkData = deepLinkData
                  };
                  NavUtils.NavigateToChat(selJid, false);
                  if (deepLinkData == null || deepLinkData.DeepLinkType != DeepLinkData.DeepLinkTypes.Conversion)
                    break;
                  ConversionRecordHelper.CreateConversionRecord(selJid, FunRunner.CurrentServerTimeUtc, deepLinkData.SharedPhoneNumber, deepLinkData.SharedSource, deepLinkData.SharedData);
                  break;
                }
                NavUtils.NavigateHome(this.NavigationService);
                break;
              default:
                string messageBoxText = shareContent.DescribeError(result);
                if (!string.IsNullOrEmpty(messageBoxText))
                {
                  int num = (int) MessageBox.Show(messageBoxText);
                  goto case ExternalShare.ExternalShareResult.Discarded;
                }
                else
                  goto case ExternalShare.ExternalShareResult.Discarded;
            }
          }), (Action<Exception>) (ex =>
          {
            Log.LogException(ex, "ShareContent exception");
            if (selJids != null && selJids.Count == 1)
              NavUtils.NavigateToChat(selJids.FirstOrDefault<string>(), false);
            else
              NavUtils.NavigateHome(this.NavigationService);
            ExternalShareException innerEx = ex.InnerException as ExternalShareException;
            this.Dispatcher.RunAfterDelay(TimeSpan.FromSeconds(1.0), (Action) (() =>
            {
              if (innerEx != null)
              {
                UIUtils.ShowMessageBox(AppResources.DocumentNotSentTitle, innerEx.Message).Subscribe<Unit>();
              }
              else
              {
                string messageBoxText = shareContent.DescribeError(ExternalShare.ExternalShareResult.Unknown);
                if (string.IsNullOrEmpty(messageBoxText))
                  return;
                int num = (int) MessageBox.Show(messageBoxText);
              }
            }));
          }));
        }
      });
      if (selectedJids == null || !selectedJids.Any<string>())
      {
        Action<Exception> onError = (Action<Exception>) (ex =>
        {
          Log.LogException(ex, "Exception Sharing content");
          tryToReturnToInvoker();
        });
        Action onCompleted = (Action) (() => Log.l(nameof (ShareContent), "RecipientPicker onCompleted"));
        RecipientListPage.StartRecipientPicker("WHATSAPP", new FunXMPP.FMessage.Type?(), clearStack: !ExternalShareUtils.IsPreviewEnabled(), statusVisibilityObservable: shareContent.ShouldEnableSharingToStatus()).ObserveOnDispatcher<RecipientListPage.RecipientListResults>().Subscribe<RecipientListPage.RecipientListResults>(onNext, onError, onCompleted);
      }
      else
        onNext(new RecipientListPage.RecipientListResults(selectedJids));
    }

    private static IObservable<ExternalShare.ExternalShareResult> MaybeIncludeSizeCheck(
      IObservable<ExternalShare.ExternalShareResult> source,
      IObservable<bool> checkTruncation)
    {
      if (checkTruncation == null)
        return source;
      string descisionText = Plurals.Instance.GetStringWithIndex(AppResources.ConfirmTextTruncationPlural, 1, (object) 65536.ToString("###,###", (IFormatProvider) CultureInfo.CurrentCulture), (object) 65536);
      return checkTruncation.SelectMany((Func<bool, IObservable<bool>>) (a => !a ? Observable.Return<bool>(true) : Observable.Return<bool>(true).Decision(descisionText)), (a, b) => new
      {
        a = a,
        b = b
      }).SelectMany(_param1 => !_param1.b ? Observable.Return<ExternalShare.ExternalShareResult>(ExternalShare.ExternalShareResult.Discarded) : source, (_param1, c) => c);
    }

    private void HandleDeepLink(UriShareContent uriShared)
    {
      DeepLinkData deepLinkData = DeepLinkData.CreateFrom(uriShared);
      Log.l(nameof (ShareContent), "Attempting deeplink {0} {1}", (object) deepLinkData.SharedPhoneNumber, (object) deepLinkData.SharedText);
      SysTrayHelper.SetForegroundColor((DependencyObject) this, Constants.SysTrayOffWhite);
      this.SysTrayPlaceHolder.Height = UIUtils.SystemTraySizePortrait;
      this.LoadingPanel.Visibility = Visibility.Visible;
      this.LoadingProgressBar.IsIndeterminate = true;
      this.LoadingProgressBar.Maximum = 100.0;
      this.LoadingProgressBar.Value = 0.0;
      this.LoadingTextBlock.Text = AppResources.Searching;
      this.ButtonText.Text = AppResources.Cancel;
      this.ButtonIcon.ButtonIcon = (BitmapSource) ImageStore.GetStockIcon("/Images/x-active.png");
      this.ActionButton.Visibility = Visibility.Visible;
      string errorString;
      string checkedPhoneNumber = this.ConvertToInternationalFormat(deepLinkData.SharedPhoneNumber, out errorString);
      if (errorString != null)
      {
        this.ShowErrorText(errorString);
      }
      else
      {
        string formattedNumber = deepLinkData.SharedPhoneNumber.Trim();
        try
        {
          formattedNumber = PhoneNumberFormatter.FormatInternationalNumber(formattedNumber);
        }
        catch (Exception ex)
        {
          string context = "ShareContent: Exception formatting supplied number " + deepLinkData.SharedPhoneNumber;
          Log.LogException(ex, context);
        }
        AppState.Worker.Enqueue((Action) (() => ShareContent.CheckAndNormalize(checkedPhoneNumber, (Action<FunXMPP.Connection.SyncResult>) (result =>
        {
          object[] objArray = new object[3]
          {
            (object) deepLinkData.SharedPhoneNumber,
            null,
            null
          };
          FunXMPP.Connection.SyncResult.User[] swellFolks = result.SwellFolks;
          objArray[1] = swellFolks != null ? (object) ((IEnumerable<FunXMPP.Connection.SyncResult.User>) swellFolks).Count<FunXMPP.Connection.SyncResult.User>() : (object) -1;
          FunXMPP.Connection.SyncResult.User[] holdouts = result.Holdouts;
          objArray[2] = holdouts != null ? (object) ((IEnumerable<FunXMPP.Connection.SyncResult.User>) holdouts).Count<FunXMPP.Connection.SyncResult.User>() : (object) -1;
          Log.d(nameof (ShareContent), "deep link user check got result {0} {1} {2}", objArray);
          if (this.pageExited)
            return;
          if (result.SwellFolks != null && ((IEnumerable<FunXMPP.Connection.SyncResult.User>) result.SwellFolks).Count<FunXMPP.Connection.SyncResult.User>() > 0)
          {
            List<string> selectedJidList = new List<string>();
            string jid = result.SwellFolks[0].Jid;
            selectedJidList.Add(jid);
            this.Dispatcher.BeginInvoke((Action) (() =>
            {
              this.HideProgressbar();
              NavUtils.ClearBackStack();
              this.MaybeLaunchRecipientPicker(this.shareContent, selectedJidList, deepLinkData);
            }));
            UsyncQueryRequest.SendForDetailsForJid(jid);
          }
          else
          {
            string errorText = result.Holdouts == null || ((IEnumerable<FunXMPP.Connection.SyncResult.User>) result.Holdouts).Count<FunXMPP.Connection.SyncResult.User>() <= 0 ? string.Format(AppResources.DirectPhoneNumberInvalid, (object) formattedNumber) : string.Format(AppResources.DirectPhoneNumberNotWA, (object) formattedNumber);
            this.Dispatcher.BeginInvoke((Action) (() => this.ShowErrorText(errorText)));
          }
        }), (Action<int>) (errorCode =>
        {
          Log.d(nameof (ShareContent), "deep link got error {0} {1}", (object) errorCode, (object) deepLinkData.SharedPhoneNumber);
          if (this.pageExited)
            return;
          this.Dispatcher.BeginInvoke((Action) (() => this.ShowErrorText(string.Format(AppResources.DirectPhoneNumberSyncFailed, (object) formattedNumber))));
        }))));
      }
    }

    private void HideProgressbar() => this.LoadingPanel.Visibility = Visibility.Collapsed;

    private void ShowErrorText(string errorString)
    {
      this.HideProgressbar();
      this.errorTextBlock.Text = errorString;
      this.errorTextBlock.Visibility = Visibility.Visible;
      this.ButtonText.Text = AppResources.OK;
      this.ButtonIcon.ButtonIcon = (BitmapSource) ImageStore.GetStockIcon("/Images/prev.png");
      this.deeplinkErrorJustShown = true;
      App.CurrentApp.RootFrame?.RemoveBackEntry();
    }

    private void X_Button_Click(object sender, EventArgs e) => NavUtils.NavigateHome();

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      e.Cancel = true;
      base.OnBackKeyPress(e);
      NavUtils.NavigateHome();
    }

    private static bool CheckAndNormalize(
      string phoneNumber,
      Action<FunXMPP.Connection.SyncResult> onReceivedOK,
      Action<int> onReceivedError)
    {
      try
      {
        if (AppState.GetConnection() != null)
        {
          string sid = DateTime.UtcNow.ToFileTimeUtc().ToString();
          AppState.Client clientInstance = AppState.ClientInstance;
          FunXMPP.Connection conn = clientInstance?.GetConnection();
          if (clientInstance != null)
          {
            if (conn != null)
            {
              Action a = (Action) (() =>
              {
                UsyncQuery query = new UsyncQuery(FunXMPP.Connection.SyncMode.Query, FunXMPP.Connection.SyncContext.Interactive);
                conn.AddUsyncGetBusinesses(query, (IEnumerable<FunXMPP.Connection.BusinessRequest>) new FunXMPP.Connection.BusinessRequest[0], (IEnumerable<FunXMPP.Connection.BusinessRequest>) new FunXMPP.Connection.BusinessRequest[0]);
                conn.AddUsyncGetContacts(query, sid, 0, true, (IEnumerable<string>) new string[1]
                {
                  phoneNumber
                }, (IEnumerable<string>) new string[0], onReceivedOK, onReceivedError);
                UsyncQueryRequest.Send(query, conn);
              });
              conn.InvokeWhenConnected(a);
              return true;
            }
          }
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception processing phone number");
      }
      return false;
    }

    private string ConvertToInternationalFormat(string enteredPhoneNumber, out string errorString)
    {
      string rawNumber = enteredPhoneNumber.Replace("\\D", "").Trim();
      if (rawNumber.Length >= 1 && rawNumber.StartsWith("+"))
        rawNumber = rawNumber.Substring(1);
      Log.d("DeepLink", "convertToInternationalFormat after-replace " + rawNumber);
      if (rawNumber.Length < 5)
      {
        Log.l("DeepLink", "entered phone number too short {0}", (object) enteredPhoneNumber);
        errorString = string.Format(AppResources.DirectPhoneNumberTooShort, (object) enteredPhoneNumber);
        return (string) null;
      }
      CountryInfoItem infoFromRawNumber = PhoneNumberFormatter.getCountryInfoFromRawNumber(rawNumber);
      if (infoFromRawNumber == null)
      {
        Log.l("DeepLink", "entered phone number no cc {0}", (object) enteredPhoneNumber);
        errorString = string.Format(AppResources.DirectPhoneNumberIsMissingCC, (object) enteredPhoneNumber);
        return (string) null;
      }
      string str1 = rawNumber.Substring(infoFromRawNumber.PhoneCountryCode.Length);
      string str2 = infoFromRawNumber.ApplyLeadingDigitsFilter(str1.ExtractDigits());
      string str3 = infoFromRawNumber.PhoneCountryCode + str2;
      if (str3.Length < 6 || str3.Length > 17)
      {
        Log.l("DeepLink", "entered phone number length invalid {0} {1} {2}", (object) enteredPhoneNumber, (object) infoFromRawNumber.PhoneCountryCode, (object) str3.Length);
        errorString = string.Format(AppResources.DirectPhoneNumberIncorrectLengthForCC, (object) enteredPhoneNumber, (object) infoFromRawNumber.PhoneCountryCode);
        return (string) null;
      }
      errorString = (string) null;
      return "+" + str3;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/ShareContent.xaml", UriKind.Relative));
      this.LayoutRootZoomBox = (ZoomBox) this.FindName("LayoutRootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.SysTrayPlaceHolder = (Grid) this.FindName("SysTrayPlaceHolder");
      this.WaLogo = (Image) this.FindName("WaLogo");
      this.LoadingPanel = (StackPanel) this.FindName("LoadingPanel");
      this.LoadingProgressBar = (ProgressBar) this.FindName("LoadingProgressBar");
      this.LoadingTextBlock = (TextBlock) this.FindName("LoadingTextBlock");
      this.errorTextBlock = (TextBlock) this.FindName("errorTextBlock");
      this.ActionButton = (Button) this.FindName("ActionButton");
      this.ButtonIcon = (RoundButton) this.FindName("ButtonIcon");
      this.ButtonText = (TextBlock) this.FindName("ButtonText");
    }
  }
}
