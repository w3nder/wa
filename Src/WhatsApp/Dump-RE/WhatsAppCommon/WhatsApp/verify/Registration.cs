// Decompiled with JetBrains decompiler
// Type: WhatsApp.verify.Registration
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using WhatsAppNative;

#nullable disable
namespace WhatsApp.verify
{
  public static class Registration
  {
    private static Registration.RegResult ParseResultV2(
      Stream stream,
      string cc,
      string number,
      bool usePIN = false)
    {
      Registration.RegResult r = new DataContractJsonSerializer(typeof (Registration.RegResult)).ReadObject(stream) as Registration.RegResult;
      Settings.Delete(Settings.Key.CodeEntryWaitToRetryUtc);
      Log.l("reg", "status={0} reason={1}", (object) r.Status, (object) r.Reason);
      if (r.Status == "fail")
      {
        switch (r.Reason)
        {
          case "blocked":
            string newValue = AppState.FormatPhoneNumber(r.ChatID ?? cc + number);
            string str = string.Format(AppResources.BlockedRegistration, (object) newValue).Replace("<number>", newValue);
            r.ErrorString = str;
            r.ContactSupport = true;
            r.ActionTitle = AppResources.ContactSupportButton;
            Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.NewlyEntered;
            break;
          case "format_wrong":
          case "length_long":
          case "length_short":
            r.ErrorString = AppResources.InvalidPhoneNumberLengthOrFormat;
            break;
          case "guessed_too_fast":
          case "mismatch":
            object[] objArray1 = new object[1];
            int? waitSeconds1 = r.waitSeconds;
            objArray1[0] = (object) (waitSeconds1 ?? -1);
            Log.l("reg", "mismatch/guessed_too_fast | wait={0}s", objArray1);
            if (!string.IsNullOrEmpty(r.WaitString))
            {
              waitSeconds1 = r.WaitSeconds;
              int num = 5;
              if ((waitSeconds1.GetValueOrDefault() <= num ? (waitSeconds1.HasValue ? 1 : 0) : 0) == 0)
              {
                DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
                waitSeconds1 = r.WaitSeconds;
                TimeSpan timeSpan = TimeSpan.FromSeconds((double) waitSeconds1.Value);
                Settings.CodeEntryWaitToRetryUtc = currentServerTimeUtc + timeSpan;
                r.ActionTitle = AppResources.WrongCodeTitle;
                r.ErrorString = !usePIN ? string.Format(AppResources.ReenterCodeLaterWithTime, (object) r.WaitString) : string.Format(AppResources.ReenterPINLaterWithTime, (object) r.WaitString);
                goto label_18;
              }
            }
            Settings.CodeEntryWaitToRetryUtc = FunRunner.CurrentServerTimeUtc;
            r.ActionTitle = AppResources.WrongCodeTitle;
            r.ErrorString = !usePIN ? AppResources.WrongCodeMessage : AppResources.WrongPINMessage;
label_18:
            r.PromptForReenter = true;
            break;
          case "incorrect":
            if (r.WaitSeconds.HasValue)
            {
              object[] objArray2 = new object[1];
              int? waitSeconds2 = r.waitSeconds;
              objArray2[0] = (object) (waitSeconds2 ?? -1);
              Log.l("reg", "incorrect code | wait={0}s", objArray2);
              DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
              waitSeconds2 = r.WaitSeconds;
              TimeSpan timeSpan = TimeSpan.FromSeconds((double) waitSeconds2.Value);
              Settings.CodeEntryWaitToRetryUtc = currentServerTimeUtc + timeSpan;
              break;
            }
            break;
          case "invalid_skey":
            AppState.GetConnection().Encryption.Reset();
            break;
          case "missing":
          case "stale":
            Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.NewlyEntered;
            r.ErrorString = AppResources.ReenterPhoneNumber;
            break;
          case "no_routes":
            object[] objArray3 = new object[2];
            int? waitSeconds3 = r.waitSeconds;
            objArray3[0] = (object) (waitSeconds3 ?? -1);
            objArray3[1] = (object) Settings.PhoneNumberVerificationState;
            Log.l("reg", "no_routes | wait={0}s {1}", objArray3);
            r.ErrorString = AppResources.VerificationErrorNoRoutes;
            if (!string.IsNullOrEmpty(r.WaitString))
            {
              waitSeconds3 = r.WaitSeconds;
              int num = 5;
              if ((waitSeconds3.GetValueOrDefault() <= num ? (waitSeconds3.HasValue ? 1 : 0) : 0) == 0)
              {
                r.ErrorString += string.Format(" " + AppResources.CheckAndTryAgainWithTime, (object) r.WaitString);
                break;
              }
            }
            Registration.RegResult regResult1 = r;
            regResult1.ErrorString = regResult1.ErrorString + " " + AppResources.CheckAndTryAgainGeneric;
            break;
          case "old_version":
            r.ErrorString = AppResources.VerificationNeedsUpdate;
            break;
          case "provider_timeout":
          case "provider_unroutable":
            object[] objArray4 = new object[2];
            int? waitSeconds4 = r.waitSeconds;
            objArray4[0] = (object) (waitSeconds4 ?? -1);
            objArray4[1] = (object) Settings.PhoneNumberVerificationState;
            Log.l("reg", "provider error | wait={0}s {1}", objArray4);
            r.ErrorString = AppResources.RegistrationErrorProviderError;
            if (!string.IsNullOrEmpty(r.WaitString))
            {
              waitSeconds4 = r.WaitSeconds;
              int num = 5;
              if ((waitSeconds4.GetValueOrDefault() <= num ? (waitSeconds4.HasValue ? 1 : 0) : 0) == 0)
              {
                r.ErrorString += string.Format(" " + AppResources.CheckAndTryAgainWithTime, (object) r.WaitString);
                goto label_34;
              }
            }
            Registration.RegResult regResult2 = r;
            regResult2.ErrorString = regResult2.ErrorString + " " + AppResources.CheckAndTryAgainGeneric;
label_34:
            r.PromptForReenter = true;
            break;
          case "temporarily_unavailable":
            object[] objArray5 = new object[1];
            int? waitSeconds5 = r.waitSeconds;
            objArray5[0] = (object) (waitSeconds5 ?? -1);
            Log.l("reg", "temporarily_unavailable | wait={0}s", objArray5);
            r.ErrorString = AppResources.TemporarilyUnavailable;
            if (!string.IsNullOrEmpty(r.WaitString))
            {
              waitSeconds5 = r.WaitSeconds;
              int num = 5;
              if ((waitSeconds5.GetValueOrDefault() <= num ? (waitSeconds5.HasValue ? 1 : 0) : 0) == 0)
              {
                DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
                waitSeconds5 = r.WaitSeconds;
                TimeSpan timeSpan = TimeSpan.FromSeconds((double) waitSeconds5.Value);
                Settings.CodeEntryWaitToRetryUtc = currentServerTimeUtc + timeSpan;
                Registration.RegResult regResult3 = r;
                regResult3.ErrorString = regResult3.ErrorString + " " + AppResources.TryAgainGeneric;
                goto label_8;
              }
            }
            Settings.CodeEntryWaitToRetryUtc = FunRunner.CurrentServerTimeUtc;
            r.ErrorString += string.Format(" " + AppResources.TryAgainWithTime, (object) r.WaitString);
label_8:
            r.PromptForReenter = true;
            break;
          case "too_many":
          case "too_many_all_methods":
            Registration.ProcessTooMany(r, AppResources.TooMany, (IEnumerable<string>) new string[2]
            {
              AppResources.CheckSMSAndCalling,
              AppResources.CheckForCode
            });
            break;
          case "too_many_guesses":
            Registration.ProcessTooMany(r, AppResources.TooManyGuesses, (IEnumerable<string>) new string[2]
            {
              AppResources.CheckSMSAndCalling,
              AppResources.WaitForCode
            });
            break;
          case "too_recent":
            object[] objArray6 = new object[1];
            int? waitSeconds6 = r.waitSeconds;
            objArray6[0] = (object) (waitSeconds6 ?? -1);
            Log.l("reg", "too_recent | wait={0}s", objArray6);
            r.ErrorString = AppResources.TooRecent;
            if (!string.IsNullOrEmpty(r.WaitString))
            {
              waitSeconds6 = r.WaitSeconds;
              int num = 5;
              if ((waitSeconds6.GetValueOrDefault() <= num ? (waitSeconds6.HasValue ? 1 : 0) : 0) == 0)
              {
                DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
                waitSeconds6 = r.WaitSeconds;
                TimeSpan timeSpan = TimeSpan.FromSeconds((double) waitSeconds6.Value);
                Settings.CodeEntryWaitToRetryUtc = currentServerTimeUtc + timeSpan;
                r.ErrorString += string.Format(" " + AppResources.TryAgainWithTime, (object) r.WaitString);
                goto label_13;
              }
            }
            Settings.CodeEntryWaitToRetryUtc = FunRunner.CurrentServerTimeUtc;
            Registration.RegResult regResult4 = r;
            regResult4.ErrorString = regResult4.ErrorString + " " + AppResources.TryAgainGeneric;
label_13:
            r.PromptForReenter = true;
            break;
        }
      }
      else if (r.Status == "ok")
      {
        if (!string.IsNullOrEmpty(r.EdgeRoutingInfo))
        {
          try
          {
            Settings.EdgeRoutingInfo = Convert.FromBase64String(r.EdgeRoutingInfo);
          }
          catch
          {
            Settings.EdgeRoutingInfo = (byte[]) null;
          }
        }
        if (!string.IsNullOrEmpty(r.ChatDnsDomain))
          NonDbSettings.ChatDnsDomain = r.ChatDnsDomain;
        Settings.LastSignedPreKeySent = new DateTime?(DateTime.Now);
      }
      return r;
    }

    private static void ProcessTooMany(
      Registration.RegResult r,
      string baseString,
      IEnumerable<string> mitigations)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(baseString);
      if (r.WaitString != null)
      {
        stringBuilder.Append("\n\n");
        foreach (string mitigation in mitigations)
        {
          stringBuilder.Append("• ");
          stringBuilder.Append(mitigation);
          stringBuilder.Append("\n");
        }
        stringBuilder.Append("\n");
        stringBuilder.AppendFormat(AppResources.TryAgainWithTime, (object) r.WaitString);
        r.SupportUrl = WaWebUrls.FaqUrlVerification;
        r.ActionTitle = AppResources.LearnMoreButton;
      }
      else
      {
        stringBuilder.Append(" ");
        stringBuilder.Append(AppResources.ContactSupport);
        r.ContactSupport = true;
        r.ActionTitle = AppResources.ContactSupportButton;
      }
      r.ErrorString = stringBuilder.ToString();
    }

    public static IObservable<Registration.RegResult> Exists(string cc, string phone, byte[] udid)
    {
      Log.l("reg", "check exists | number={0} {1}", (object) cc, (object) phone);
      return Registration.WebRequest("https://v.whatsapp.net/v2/exist", string.Format("cc={0}&in={1}&id={2}", (object) Uri.EscapeDataString(cc), (object) Uri.EscapeDataString(phone), (object) Registration.UrlEncode(udid))).Select<Stream, Registration.RegResult>((Func<Stream, Registration.RegResult>) (stream =>
      {
        using (stream)
        {
          Registration.RegResult resultV2 = Registration.ParseResultV2(stream, cc, phone);
          if (resultV2.Status == "fail" && resultV2.ErrorString == null && resultV2.Reason != "incorrect")
          {
            resultV2.ErrorString = string.Format(AppResources.RegExistsFail, (object) resultV2.Reason);
            resultV2.ContactSupport = true;
            resultV2.ActionTitle = AppResources.ContactSupportButton;
          }
          return resultV2;
        }
      }));
    }

    private static IByteBuffer NewByteBuffer(byte[] b)
    {
      IByteBuffer bb = (IByteBuffer) null;
      if (b != null)
      {
        bb = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
        bb.Put(b);
      }
      return bb;
    }

    private static string PadMcc(uint digits) => digits.ToString().PadLeft(3, '0');

    public static IObservable<Registration.RegResult> RequestCode(
      string cc,
      string number,
      byte[] udid,
      string codeType)
    {
      Log.l("reg", "request code | number={0} {1} method={2}", (object) cc, (object) number, (object) codeType);
      string stringToEscape = cc + number;
      string lower = NativeInterfaces.Misc.GetToken(Registration.NewByteBuffer(Encoding.UTF8.GetBytes(Registration.GetBuildHash() + number))).Get().ToHexString().ToLower();
      uint digits1 = 0;
      uint digits2 = 0;
      try
      {
        CELL_INFO cellInfo = NativeInterfaces.Misc.GetCellInfo(CellInfoFlags.MccMnc);
        digits1 = cellInfo.Mcc;
        digits2 = cellInfo.Mnc;
      }
      catch (Exception ex)
      {
      }
      IObservable<Stream> reqObs = Registration.WebRequest("https://v.whatsapp.net/v2/code", string.Format("cc={0}&in={1}&to={2}&method={3}&mcc={4}&mnc={5}&token={6}&id={7}", (object) Uri.EscapeDataString(cc), (object) Uri.EscapeDataString(number), (object) Uri.EscapeDataString(stringToEscape), (object) Uri.EscapeDataString(codeType), (object) Registration.PadMcc(digits1), (object) Registration.PadMcc(digits2), (object) lower, (object) Registration.UrlEncode(udid)));
      return Observable.Timer(TimeSpan.FromSeconds(1.0), (IScheduler) Scheduler.Dispatcher).SelectMany<long, Stream, Stream>((Func<long, IObservable<Stream>>) (l => reqObs), (Func<long, Stream, Stream>) ((l, s) => s)).Select<Stream, Registration.RegResult>((Func<Stream, Registration.RegResult>) (s =>
      {
        using (s)
          return Registration.ParseResultV2(s, cc, number);
      })).Do<Registration.RegResult>((Action<Registration.RegResult>) (r =>
      {
        if (r.Status == "sent")
        {
          r.CodeSent = true;
          if (r.Length.HasValue)
            Settings.CodeLength = r.Length.Value;
          if (!r.WaitSeconds.HasValue)
            return;
          Settings.PhoneNumberVerificationRetryUtc = FunRunner.CurrentServerTimeUtc + TimeSpan.FromSeconds((double) r.WaitSeconds.Value);
        }
        else if (r.Status == "ok")
          r.RegistrationOk = true;
        else if (r.Status == "fail" && r.Reason == "next_method")
        {
          if (Settings.PhoneNumberVerificationState == PhoneNumberVerificationState.ServerSendSmsFailed)
            Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.ServerSendVoiceFailed;
          else
            Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.ServerSendSmsFailed;
        }
        else
        {
          if (!(r.Status == "fail") || r.ErrorString != null)
            return;
          r.ErrorString = string.Format(AppResources.ErrorSendingCode, (object) r.Reason);
        }
      }));
    }

    private static string GetBuildHash() => AppState.ClientInstance.GetBuildHash();

    public static IObservable<Registration.RegResult> Register(
      string cc,
      string number,
      byte[] udid,
      string code,
      Registration.CodeEntryMethod method)
    {
      Log.l("reg", "registering | number={0} {1}", (object) cc, (object) number);
      string str = (string) null;
      switch (method)
      {
        case Registration.CodeEntryMethod.Manual:
          str = "1";
          break;
        case Registration.CodeEntryMethod.AutoDetect:
          str = "2";
          break;
        case Registration.CodeEntryMethod.Link:
          str = "3";
          break;
        case Registration.CodeEntryMethod.Retry:
          str = "4";
          break;
      }
      return Registration.WebRequest("https://v.whatsapp.net/v2/register", string.Format("cc={0}&in={1}&id={2}&code={3}&entered={4}", (object) Uri.EscapeDataString(cc), (object) Uri.EscapeDataString(number), (object) Registration.UrlEncode(udid), (object) Uri.EscapeDataString(code), (object) str)).Select<Stream, Registration.RegResult>((Func<Stream, Registration.RegResult>) (respStream =>
      {
        using (respStream)
        {
          Registration.RegResult resultV2 = Registration.ParseResultV2(respStream, cc, number);
          if (resultV2.Status == "fail" && resultV2.ErrorString == null)
            resultV2.ErrorString = string.Format(AppResources.VerificationError, (object) resultV2.Reason);
          return resultV2;
        }
      }));
    }

    public static IObservable<Registration.RegResult> CheckSecurityCode(
      string cc,
      string number,
      byte[] udid,
      string code)
    {
      return Registration.WebRequest("https://v.whatsapp.net/v2/security", string.Format("cc={0}&in={1}&id={2}&code={3}", (object) Uri.EscapeDataString(cc), (object) Uri.EscapeDataString(number), (object) Registration.UrlEncode(udid), (object) Uri.EscapeDataString(code))).Select<Stream, Registration.RegResult>((Func<Stream, Registration.RegResult>) (respStream =>
      {
        using (respStream)
        {
          new StreamReader(respStream).ReadToEnd();
          Registration.RegResult resultV2 = Registration.ParseResultV2(respStream, cc, number, true);
          if (resultV2.Status == "fail" && resultV2.ErrorString == null)
            resultV2.ErrorString = string.Format(AppResources.VerificationError, (object) resultV2.Reason);
          return resultV2;
        }
      }));
    }

    public static IObservable<Registration.RegResult> UseEmailRecovery(
      string cc,
      string number,
      byte[] udid)
    {
      return Registration.WebRequest("https://v.whatsapp.net/v2/security", string.Format("cc={0}&in={1}&id={2}&reset=email", (object) Uri.EscapeDataString(cc), (object) Uri.EscapeDataString(number), (object) Registration.UrlEncode(udid))).Select<Stream, Registration.RegResult>((Func<Stream, Registration.RegResult>) (respStream =>
      {
        using (respStream)
        {
          new StreamReader(respStream).ReadToEnd();
          Registration.RegResult resultV2 = Registration.ParseResultV2(respStream, cc, number);
          if (resultV2.Status == "fail" && resultV2.ErrorString == null)
            resultV2.ErrorString = string.Format(AppResources.VerificationError, (object) resultV2.Reason);
          return resultV2;
        }
      }));
    }

    public static IObservable<Registration.RegResult> WipeAccount(
      string cc,
      string number,
      byte[] udid,
      string wipeToken)
    {
      return Registration.WebRequest("https://v.whatsapp.net/v2/security", string.Format("cc={0}&in={1}&id={2}&reset=wipe&wipe_token={3}", (object) Uri.EscapeDataString(cc), (object) Uri.EscapeDataString(number), (object) Registration.UrlEncode(udid), (object) Uri.EscapeDataString(wipeToken))).Select<Stream, Registration.RegResult>((Func<Stream, Registration.RegResult>) (respStream =>
      {
        using (respStream)
        {
          new StreamReader(respStream).ReadToEnd();
          Registration.RegResult resultV2 = Registration.ParseResultV2(respStream, cc, number);
          if (resultV2.Status == "fail" && resultV2.ErrorString == null)
            resultV2.ErrorString = string.Format(AppResources.VerificationError, (object) resultV2.Reason);
          return resultV2;
        }
      }));
    }

    private static string UrlEncode(byte[] arr)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string str = "0123456789abcdef";
      foreach (byte c in arr)
      {
        if (c < (byte) 128 && (char.IsLetterOrDigit((char) c) || "-_.~".Contains<char>((char) c)))
        {
          stringBuilder.Append((char) c);
        }
        else
        {
          stringBuilder.Append('%');
          stringBuilder.Append(str[(int) c >> 4]);
          stringBuilder.Append(str[(int) c & 15]);
        }
      }
      return stringBuilder.ToString();
    }

    private static IObservable<Stream> WebRequest(string url, string query)
    {
      query = Registration.AddCommonQueryParams(query);
      string userAgent = AppState.GetUserAgent();
      byte[] bytes = Encoding.UTF8.GetBytes(query);
      byte[] pubKey = NativeInterfaces.Misc.GetString(9).FromHexString();
      byte[] staticPublic;
      byte[] staticPrivate;
      WAProtocol.GenerateClientStaticKeyPair(out staticPublic, out staticPrivate);
      byte[] privKey = staticPrivate;
      byte[] sourceArray = MbedtlsExtensions.AesGcmEncrypt(Curve22519Extensions.Derive(pubKey, privKey), WAProtocol.LongToByteArray(0L, 12), (byte[]) null, bytes, new int?(0), new int?(bytes.Length));
      byte[] numArray = new byte[staticPublic.Length + sourceArray.Length];
      Array.Copy((Array) staticPublic, (Array) numArray, staticPublic.Length);
      Array.Copy((Array) sourceArray, 0, (Array) numArray, staticPublic.Length, sourceArray.Length);
      query = "ENC=" + Convert.ToBase64String(numArray, 0, numArray.Length).ToUrlSafeBase64String();
      return NativeWeb.SimpleGet(string.Format("{0}?{1}", (object) url, (object) query), userAgent, NativeWeb.Options.PinCertificate);
    }

    private static string AddCommonQueryParams(string query)
    {
      string lang;
      string locale;
      AppState.GetLangAndLocale(out lang, out locale);
      Axolotl encryption = AppState.GetConnection().Encryption;
      uint localRegistrationId = encryption.Store.LocalRegistrationId;
      byte[] identityKeyForSending = encryption.Store.IdentityKeyForSending;
      AxolotlPreKey latestSignedPreKey = encryption.Store.LatestSignedPreKey;
      if (query.Length != 0)
        query += "&";
      query += string.Format("lc={0}&lg={1}&authkey={2}&e_regid={3}&e_keytype={4}&e_ident={5}&e_skey_id={6}&e_skey_val={7}&e_skey_sig={8}&rc={9}", (object) Uri.EscapeDataString(locale), (object) Uri.EscapeDataString(lang), (object) Settings.ClientStaticPublicKey.ToUrlSafeBase64String(), (object) FunXMPP.Connection.UIntToBytes(localRegistrationId).ToUrlSafeBase64String(), (object) new byte[1]
      {
        (byte) 5
      }.ToUrlSafeBase64String(), (object) identityKeyForSending.ToUrlSafeBase64String(), (object) latestSignedPreKey.Id.ToUrlSafeBase64String(), (object) latestSignedPreKey.Data.ToUrlSafeBase64String(), (object) latestSignedPreKey.Signature.ToUrlSafeBase64String(), (object) (int) Constants.ReleaseChannel);
      return query;
    }

    public static PhoneNumberAccountCreationType GetAccountCreationType(Registration.RegResult resp)
    {
      if (string.Equals(resp.AccountCreationType, "new", StringComparison.InvariantCultureIgnoreCase))
        return PhoneNumberAccountCreationType.New;
      return string.Equals(resp.AccountCreationType, "existing", StringComparison.InvariantCultureIgnoreCase) ? PhoneNumberAccountCreationType.Existing : PhoneNumberAccountCreationType.Unknown;
    }

    [DataContract]
    public class SecurityResult
    {
    }

    [DataContract]
    public class RegResult
    {
      public int? waitSeconds;
      private string waitString;
      public string ErrorString;
      public bool CodeSent;
      public bool RegistrationOk;
      public bool PromptForReenter;
      public string ActionTitle;
      public string SupportUrl;
      public bool ContactSupport;
      public string Result;
      public string Login;

      [DataMember(Name = "status")]
      public string Status { get; set; }

      [DataMember(Name = "login")]
      public string ChatID { get; set; }

      [DataMember(Name = "type")]
      public string AccountCreationType { get; set; }

      [DataMember(Name = "expiration")]
      public double? ExpirationDouble { get; set; }

      [DataMember(Name = "kind")]
      public string AccountType { get; set; }

      [DataMember(Name = "price")]
      public string ServicePrice { get; set; }

      [DataMember(Name = "cost")]
      public string ServiceCost { get; set; }

      [DataMember(Name = "currency")]
      public string CostCurrency { get; set; }

      [DataMember(Name = "price_expiration")]
      public double? PriceExpiration { get; set; }

      [DataMember(Name = "reason")]
      public string Reason { get; set; }

      [DataMember(Name = "retry_after")]
      public int? WaitSeconds
      {
        get => this.waitSeconds;
        set
        {
          this.waitSeconds = value;
          this.waitString = (string) null;
        }
      }

      public static string WaitStringFromTime(int seconds)
      {
        TimeSpan timeSpan = TimeSpan.FromSeconds((double) seconds);
        Plurals instance = Plurals.Instance;
        string str1;
        if (timeSpan.Days > 0)
        {
          str1 = instance.GetString(AppResources.DaysPlural, timeSpan.Days);
        }
        else
        {
          string str2 = (string) null;
          string str3 = (string) null;
          if (timeSpan.Hours > 0)
            str2 = instance.GetString(AppResources.HoursPlural, timeSpan.Hours);
          if (timeSpan.Minutes > 0)
            str3 = instance.GetString(AppResources.MinutesPlural, timeSpan.Minutes);
          str1 = Utils.CommaSeparate((IEnumerable<string>) ((IEnumerable<string>) new string[2]
          {
            str2,
            str3
          }).Where<string>((Func<string, bool>) (s => s != null)).ToArray<string>());
        }
        if (string.IsNullOrEmpty(str1))
          str1 = instance.GetString(AppResources.SecondsPlural, timeSpan.Seconds);
        return str1;
      }

      public string WaitString
      {
        get
        {
          if (this.waitString == null && this.WaitSeconds.HasValue)
            this.waitString = Registration.RegResult.WaitStringFromTime(this.WaitSeconds.Value);
          return this.waitString;
        }
      }

      [DataMember(Name = "sms_length")]
      public int? SmsLength { get; set; }

      [DataMember(Name = "voice_length")]
      public int? VoiceLength { get; set; }

      [DataMember(Name = "method")]
      public string Method { get; set; }

      [DataMember(Name = "code")]
      public string Code { get; set; }

      [DataMember(Name = "length")]
      public int? Length { get; set; }

      [DataMember(Name = "guess_after")]
      public int? GuessWaitSeconds { get; set; }

      [DataMember(Name = "guesses_left")]
      public int? GuessesLeft { get; set; }

      [DataMember(Name = "wipe_type")]
      public string WipeType { get; set; }

      [DataMember(Name = "wipe_token")]
      public string WipeToken { get; set; }

      [DataMember(Name = "wipe_wait")]
      public int? WipeWait { get; set; }

      [DataMember(Name = "email_wait")]
      public int? EmailWait { get; set; }

      [DataMember(Name = "wipe_expiry_time")]
      public long? WipeExpiryTime { get; set; }

      [DataMember(Name = "server_time")]
      public long? ServerTime { get; set; }

      [DataMember(Name = "min_poll")]
      public int? MinPoll { get; set; }

      [DataMember(Name = "edge_routing_info")]
      public string EdgeRoutingInfo { get; set; }

      [DataMember(Name = "chat_dns_domain")]
      public string ChatDnsDomain { get; set; }

      public int? WaitMinutes
      {
        get => !this.waitSeconds.HasValue ? new int?() : new int?(this.waitSeconds.Value / 60);
      }
    }

    public enum CodeEntryMethod
    {
      Manual,
      AutoDetect,
      Link,
      Retry,
    }
  }
}
