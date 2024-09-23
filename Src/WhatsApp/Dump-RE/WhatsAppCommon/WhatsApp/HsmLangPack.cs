// Decompiled with JetBrains decompiler
// Type: WhatsApp.HsmLangPack
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsApp.ProtoBuf;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class HsmLangPack
  {
    private static readonly string HSM_MSG_PACK_DIR = "messagepacks";
    private static readonly string HSM_MSG_PACK_PATH = Constants.IsoStorePath + "\\" + HsmLangPack.HSM_MSG_PACK_DIR;
    private const int DAYS_BEFORE_DOWNLOAD_RETRY = 31;
    private const int DAYS_BEFORE_REFRESH = 7;
    private const int maxInCache = 2;
    private static List<HsmLangPack> cachedLangPacks = new List<HsmLangPack>();
    private static object cacheLock = new object();
    public string LpNamespace;
    public string LpLg;
    public string LpLc;
    public DateTime RequestedTimestamp;
    public HsmLangPackTypeCode LpType;
    public DateTime LastUsedTimestamp;
    public string LpHash;
    public HighlyStructuredMessagePack MessagePack;
    private static readonly string REASON_LOCALE = "locale";
    private static readonly string REASON_NS = "ns";
    private static readonly string REASON_REFRESH = "refresh";
    private static Dictionary<string, long> backoffRequest = new Dictionary<string, long>();
    private static int HSM_BACKOFF_MINS_SHORT = 5;
    private static int HSM_BACKOFF_MINS_LONG = 60;

    private static HsmLangPack GetCachedLangPack(string reqNamespace, string reqLg, string reqLc)
    {
      lock (HsmLangPack.cacheLock)
      {
        foreach (HsmLangPack cachedLangPack in HsmLangPack.cachedLangPacks)
        {
          if (cachedLangPack.Matches(reqNamespace, reqLg, reqLc))
            return cachedLangPack;
        }
      }
      return (HsmLangPack) null;
    }

    private static HsmLangPack RemoveCachedLangPack(
      string reqNamespace,
      string reqLg,
      string reqLc)
    {
      lock (HsmLangPack.cacheLock)
      {
        HsmLangPack hsmLangPack = (HsmLangPack) null;
        foreach (HsmLangPack cachedLangPack in HsmLangPack.cachedLangPacks)
        {
          if (cachedLangPack.Matches(reqNamespace, reqLg, reqLc))
          {
            hsmLangPack = cachedLangPack;
            break;
          }
        }
        if (hsmLangPack != null)
          HsmLangPack.cachedLangPacks.Remove(hsmLangPack);
      }
      return (HsmLangPack) null;
    }

    private static void CacheLangPack(HsmLangPack langPack)
    {
      if (langPack == null)
        return;
      lock (HsmLangPack.cacheLock)
      {
        for (int index = 0; index < HsmLangPack.cachedLangPacks.Count; ++index)
        {
          if (HsmLangPack.cachedLangPacks[index].Matches(langPack.LpNamespace, langPack.LpLg, langPack.LpLc))
          {
            HsmLangPack.cachedLangPacks[index] = langPack;
            return;
          }
        }
        while (HsmLangPack.cachedLangPacks.Count >= 2)
          HsmLangPack.cachedLangPacks.RemoveAt(0);
        HsmLangPack.cachedLangPacks.Add(langPack);
      }
    }

    public HsmLangPack(string lpNamespace, string lpLg, string lpLc)
    {
      this.LpNamespace = lpNamespace;
      this.LpLg = lpLg;
      this.LpLc = lpLc;
      this.RequestedTimestamp = FunXMPP.UnixEpoch;
      this.LpType = HsmLangPackTypeCode.Unknown;
      this.LastUsedTimestamp = FunXMPP.UnixEpoch;
    }

    private bool Matches(string lpNamespace, string lpLg, string lpLc)
    {
      return lpNamespace == this.LpNamespace && lpLg == this.LpLg && (lpLc ?? "") == (this.LpLc ?? "");
    }

    private bool PersistLanguagePack(byte[] blob, string lpHash)
    {
      this.LpType = HsmLangPackTypeCode.Pack;
      this.MessagePack = HsmLangPack.extractMessagePack(blob);
      this.RequestedTimestamp = DateTime.UtcNow;
      string filePath = HsmLangPack.ConvertLpIdToFilePath(this.LpNamespace, this.LpLg, this.LpLc);
      try
      {
        using (IsoStoreMediaStorage storeMediaStorage = new IsoStoreMediaStorage())
        {
          using (MemoryStream input = new MemoryStream(blob))
          {
            storeMediaStorage.CreateDirectory(HsmLangPack.HSM_MSG_PACK_DIR);
            using (IMediaStorage mediaStorage = MediaStorage.Create(filePath))
            {
              using (Stream destination = mediaStorage.OpenFile(filePath, FileMode.Create, FileAccess.Write))
                input.Gzip().CopyTo(destination);
            }
            Log.l("hsm", "saved lp {0}", (object) filePath);
          }
        }
        SqliteHsm.AddLanguagePack(this.LpNamespace, this.LpLg, this.LpLc, this.RequestedTimestamp, lpHash);
        return true;
      }
      catch (Exception ex)
      {
        string context = "hsm exception saving language pack " + filePath + " size " + (object) blob.Length;
        Log.LogException(ex, context);
        return false;
      }
    }

    private string Translate(
      WhatsApp.ProtoBuf.Message.HighlyStructuredMessage hsmMsg,
      out HsmLangPack.TranslateFailureReason failedReason)
    {
      string str1 = (string) null;
      bool flag1 = false;
      try
      {
        string str2 = hsmMsg.Namespace;
        string elementName = hsmMsg.ElementName;
        foreach (HighlyStructuredMessagePack.HighlyStructuredMessageTranslation translation in this.MessagePack.Translations)
        {
          if (translation.Element.ElementName == elementName)
          {
            flag1 = true;
            List<string> stringList = hsmMsg.Params;
            int num1 = -1;
            string str3 = (string) null;
            uint? pluralParamNo = translation.PluralParamNo;
            if (pluralParamNo.HasValue)
            {
              pluralParamNo = translation.PluralParamNo;
              if (pluralParamNo.Value >= 1U)
              {
                int number = -1;
                try
                {
                  pluralParamNo = translation.PluralParamNo;
                  num1 = (int) pluralParamNo.Value;
                  number = Convert.ToInt32(stringList[num1 - 1]);
                }
                catch (Exception ex)
                {
                  Log.LogException(ex, "hsm exception converting plural value to number");
                  failedReason = HsmLangPack.TranslateFailureReason.PluralDoesnotConvert;
                  return (string) null;
                }
                int index1 = translation.PluralExceptions.Count<HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException>() - 1;
                HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException[] array = translation.PluralExceptions.OrderBy<HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException, int>((Func<HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException, int>) (item => (int) item.Qty.Value)).ToArray<HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException>();
                if (number == -1)
                {
                  str1 = array[index1].TranslatedText;
                }
                else
                {
                  int index2 = new Plurals(this.LpLg).GetPluralCategory(number);
                  if (index2 >= translation.PluralExceptions.Count<HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException>())
                    index2 = translation.PluralExceptions.Count<HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException>() - 1;
                  str1 = array[index2].TranslatedText;
                }
                try
                {
                  str3 = NativeInterfaces.Misc.FormatLongIcu(HsmLangPack.CreateLocaleStringForIcu(this.LpLg, this.LpLc), (long) number);
                  goto label_18;
                }
                catch (Exception ex)
                {
                  Log.LogException(ex, "Exception converting plural number to local format");
                  str3 = number.ToString();
                  goto label_18;
                }
              }
            }
            str1 = translation.TranslatedText;
label_18:
            List<WhatsApp.ProtoBuf.Message.HighlyStructuredMessage.HSMLocalizableParameter> localizableParams = hsmMsg.LocalizableParams;
            uint? numParams = translation.Element.NumParams;
            if (str1 != null && numParams.HasValue && numParams.Value > 0U)
            {
              int[] numArray = new int[(int) numParams.Value];
              int startIndex = 0;
              int num2;
              while (startIndex < str1.Length && (num2 = str1.IndexOf("{{", startIndex)) >= startIndex)
              {
                string str4 = str1.Substring(num2);
                bool flag2 = false;
                for (int index = 1; (long) index <= (long) numParams.Value && !flag2; ++index)
                {
                  string defaultStringIfAllElseFails = stringList[index - 1];
                  if (index == num1)
                    defaultStringIfAllElseFails = str3;
                  else if (localizableParams != null && localizableParams.Count >= index)
                  {
                    if (localizableParams[index - 1] != null)
                    {
                      try
                      {
                        defaultStringIfAllElseFails = this.FormatLocalizableParam(defaultStringIfAllElseFails, localizableParams[index - 1], this.LpLg, this.LpLc);
                      }
                      catch (Exception ex)
                      {
                        Log.LogException(ex, "Exception converting localizableParameters");
                        defaultStringIfAllElseFails = stringList[index - 1];
                      }
                    }
                  }
                  string str5 = "{{" + (object) index + "}}";
                  if (str4.StartsWith(str5))
                  {
                    flag2 = true;
                    ++numArray[index - 1];
                    string str6 = "";
                    if (num2 > 0)
                      str6 = str1.Substring(0, num2);
                    string str7 = str6 + defaultStringIfAllElseFails;
                    if (num2 + str5.Length < str1.Length)
                      str7 += str1.Substring(num2 + str5.Length);
                    str1 = str7;
                    startIndex = num2 + defaultStringIfAllElseFails.Length;
                  }
                }
                if (!flag2)
                  startIndex = num2 + "{{".Length;
              }
              for (int index = 0; index < numArray.Length; ++index)
              {
                if (numArray[index] < 1)
                {
                  Log.l("hsm", "element {0} translation parameter {1} used an unexpected number of times {2}", (object) elementName, (object) (index + 1), (object) numArray[index]);
                  failedReason = HsmLangPack.TranslateFailureReason.IncorrectNumberOfParameters;
                  return (string) null;
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "hsm message could not be translated");
        str1 = (string) null;
      }
      if (str1 != null)
      {
        DateTime utcNow = DateTime.UtcNow;
        if (utcNow - this.LastUsedTimestamp > TimeSpan.FromHours(6.0))
        {
          try
          {
            this.LastUsedTimestamp = utcNow;
            SqliteHsm.SetLanguagePackUsed(this.LpNamespace, this.LpLg, this.LpLc, utcNow);
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "hsm exception updating language pack usage");
          }
        }
        failedReason = HsmLangPack.TranslateFailureReason.NoError;
      }
      else
        failedReason = flag1 ? HsmLangPack.TranslateFailureReason.Other : HsmLangPack.TranslateFailureReason.ElementNameIsMissing;
      return str1;
    }

    private string FormatLocalizableParam(
      string defaultStringIfAllElseFails,
      WhatsApp.ProtoBuf.Message.HighlyStructuredMessage.HSMLocalizableParameter param,
      string lpLanguage,
      string lpLocale)
    {
      string str = (string) null;
      if (param.Currency != null)
        str = this.FormatCurrencyParam(param.Currency, lpLanguage, lpLocale);
      else if (param.DateTime != null)
      {
        if (param.DateTime.UnixEpoch != null)
          str = this.FormatUnixEpochDateTimeParam(param.DateTime.UnixEpoch, lpLanguage, lpLocale);
        else if (param.DateTime.Component != null)
          str = this.FormatComponentDateTimeParam(param.DateTime.Component, lpLanguage, lpLocale);
      }
      if (string.IsNullOrEmpty(str))
        str = param.Default;
      if (string.IsNullOrEmpty(str))
        str = defaultStringIfAllElseFails;
      return str;
    }

    public static string CreateLocaleStringForIcu(string lpLanguage, string lpLocale)
    {
      return string.IsNullOrEmpty(lpLocale) ? lpLanguage.ToLowerInvariant() : lpLanguage + "_" + lpLocale.ToUpperInvariant();
    }

    private string FormatCurrencyParam(
      WhatsApp.ProtoBuf.Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency currencyparam,
      string lpLanguage,
      string lpLocale)
    {
      long? amount1000;
      if (!string.IsNullOrEmpty(currencyparam.CurrencyCode))
      {
        amount1000 = currencyparam.Amount1000;
        if (amount1000.HasValue)
        {
          try
          {
            IMisc misc = NativeInterfaces.Misc;
            string localeStringForIcu = HsmLangPack.CreateLocaleStringForIcu(lpLanguage, lpLocale);
            string currencyCode = currencyparam.CurrencyCode;
            amount1000 = currencyparam.Amount1000;
            long amount_x_1000 = amount1000.Value;
            return misc.FormatCurrencyIcu(localeStringForIcu, currencyCode, amount_x_1000).Replace(" ", " ");
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "hsm exception formatting localized currency");
            return (string) null;
          }
        }
      }
      object[] objArray = new object[2]
      {
        (object) currencyparam.CurrencyCode,
        null
      };
      amount1000 = currencyparam.Amount1000;
      objArray[1] = (object) amount1000.HasValue;
      Log.l("Hsm", "localized currency param missing fields {0} {1}", objArray);
      return (string) null;
    }

    private string FormatUnixEpochDateTimeParam(
      WhatsApp.ProtoBuf.Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch epochParam,
      string lpLanguage,
      string lpLocale)
    {
      if (epochParam.Timestamp.HasValue)
        return NativeInterfaces.Misc.FormatDateTimeIcu(HsmLangPack.CreateLocaleStringForIcu(lpLanguage, lpLocale), "yyyyMMMMdEEEEjjmmz", epochParam.Timestamp.Value * 1000L);
      Log.l("Hsm", "localized unix epoch time param missing fields");
      return (string) null;
    }

    private string FormatComponentDateTimeParam(
      WhatsApp.ProtoBuf.Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent componentParam,
      string lpLanguage,
      string lpLocale)
    {
      if (!componentParam.Calendar.HasValue)
      {
        Log.l("Hsm", "localized component calendar param missing fields");
        return (string) null;
      }
      if (componentParam.Calendar.Value == WhatsApp.ProtoBuf.Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.CalendarType.GREGORIAN)
      {
        Calendar calendar1 = (Calendar) new GregorianCalendar();
        StringBuilder stringBuilder = new StringBuilder();
        int num = componentParam.Month.HasValue ? (int) (componentParam.Month.Value % 12U) : 1;
        DateTime dt;
        ref DateTime local = ref dt;
        uint? nullable = componentParam.Year;
        int year;
        if (!nullable.HasValue)
        {
          year = 1970;
        }
        else
        {
          nullable = componentParam.Year;
          year = (int) nullable.Value;
        }
        int month = num;
        nullable = componentParam.DayOfMonth;
        int day;
        if (!nullable.HasValue)
        {
          day = 1;
        }
        else
        {
          nullable = componentParam.DayOfMonth;
          day = (int) nullable.Value;
        }
        nullable = componentParam.Hour;
        int hour;
        if (!nullable.HasValue)
        {
          hour = 0;
        }
        else
        {
          nullable = componentParam.Hour;
          hour = (int) nullable.Value;
        }
        nullable = componentParam.Minute;
        int minute;
        if (!nullable.HasValue)
        {
          minute = 0;
        }
        else
        {
          nullable = componentParam.Minute;
          minute = (int) nullable.Value;
        }
        Calendar calendar2 = calendar1;
        local = new DateTime(year, month, day, hour, minute, 0, 0, calendar2, DateTimeKind.Local);
        nullable = componentParam.Year;
        if (nullable.HasValue)
          stringBuilder.Append("yyyy");
        nullable = componentParam.Month;
        if (nullable.HasValue)
          stringBuilder.Append("MMMM");
        nullable = componentParam.DayOfMonth;
        if (nullable.HasValue)
          stringBuilder.Append("d");
        if (componentParam.DayOfWeek.HasValue)
          stringBuilder.Append("EEEE");
        nullable = componentParam.Hour;
        if (nullable.HasValue)
          stringBuilder.Append("jjmm");
        string skeleton = stringBuilder.ToString();
        return NativeInterfaces.Misc.FormatDateTimeIcu(HsmLangPack.CreateLocaleStringForIcu(lpLanguage, lpLocale), skeleton, dt.ToUnixTime() * 1000L);
      }
      if (componentParam.Calendar.Value == WhatsApp.ProtoBuf.Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.CalendarType.SOLAR_HIJRI)
      {
        Log.l("Hsm", "Solar Hijri calendar not supported");
        return (string) null;
      }
      Log.l("Hsm", "localized component time could not be constructed");
      return (string) null;
    }

    public static string GetDownloadedLanguagePackList()
    {
      string languagePackList = "None.";
      List<HsmLangPack> langPacks = SqliteHsm.GetLangPacks();
      if (langPacks.Count<HsmLangPack>() > 0)
      {
        languagePackList = "";
        foreach (HsmLangPack hsmLangPack in langPacks)
        {
          if (languagePackList.Length > 0)
            languagePackList += "\n";
          languagePackList = languagePackList + hsmLangPack.LpNamespace + "/" + hsmLangPack.LpLg + "/" + (hsmLangPack.LpLc != null ? (object) hsmLangPack.LpLc : (object) "-") + "/" + hsmLangPack.LpHash + " " + (object) hsmLangPack.LpType;
        }
      }
      return languagePackList;
    }

    public static async Task<HsmTranslateResult> TranslateAsync(
      WhatsApp.ProtoBuf.Message.HighlyStructuredMessage hsmMsg,
      string langOverride = null,
      string localeOverride = null)
    {
      TaskCompletionSource<HsmTranslateResult> completionSource = new TaskCompletionSource<HsmTranslateResult>();
      string requestedNs = hsmMsg.Namespace;
      string lang;
      string locale;
      if (string.IsNullOrEmpty(langOverride))
      {
        CultureInfo.CurrentUICulture.GetLangAndLocale(out lang, out locale);
      }
      else
      {
        lang = langOverride;
        locale = localeOverride;
      }
      string lg = (string) null;
      string lc = (string) null;
      string str = (string) null;
      if (!string.IsNullOrEmpty(hsmMsg.DeterministicLg))
      {
        str = hsmMsg.DeterministicLg.ToLower();
        string upper = hsmMsg.DeterministicLc?.ToUpper();
        Log.l("hsm", "overriding requested language locale with deterministic");
        lang = str;
        locale = upper;
      }
      else
      {
        lg = string.IsNullOrEmpty(hsmMsg.FallbackLg) ? "en" : hsmMsg.FallbackLg?.ToLower();
        lc = hsmMsg.FallbackLc?.ToUpper();
      }
      string lower = lang?.ToLower();
      locale = locale?.ToUpper();
      List<HsmLocalePackInfo> localesToLookFor = new List<HsmLocalePackInfo>();
      localesToLookFor.Add(new HsmLocalePackInfo(lower, locale));
      if (string.IsNullOrEmpty(str))
      {
        bool flag = false;
        if (!string.IsNullOrEmpty(locale))
        {
          if (lg != lower)
            localesToLookFor.Add(new HsmLocalePackInfo(lower, (string) null));
          else
            flag = true;
        }
        if (lg != lower || !string.IsNullOrEmpty(lc) && lc != locale)
          localesToLookFor.Add(new HsmLocalePackInfo(lg, lc));
        if (flag)
          localesToLookFor.Add(new HsmLocalePackInfo(lower, (string) null));
      }
      Log.l("hsm", "looking for {0}, {1} in {2} packs", (object) hsmMsg.ElementName, (object) requestedNs, (object) localesToLookFor.Count<HsmLocalePackInfo>());
      HsmTranslateResult result = (HsmTranslateResult) null;
      bool flag1 = false;
      string requestReason = (string) null;
      for (int index1 = 0; index1 < localesToLookFor.Count<HsmLocalePackInfo>(); ++index1)
      {
        HsmLocalePackInfo suggestedLocaleInfo = localesToLookFor[index1];
        bool downloadNeeded;
        string downloadReason;
        result = HsmLangPack.TryTranslateUsingExistingPacks(hsmMsg, requestedNs, ref suggestedLocaleInfo, out downloadNeeded, out downloadReason);
        if (result != null)
        {
          Log.l("hsm", "Result {0} for pack {1}", (object) result.ResultCode, (object) suggestedLocaleInfo.ToString());
          break;
        }
        if (downloadNeeded)
        {
          Log.l("hsm", "Update required for pack {0}", (object) suggestedLocaleInfo.ToString());
          flag1 = true;
          requestReason = downloadReason;
          for (int index2 = index1 + 1; index2 < localesToLookFor.Count<HsmLocalePackInfo>(); ++index2)
          {
            suggestedLocaleInfo = localesToLookFor[index2];
            HsmLangPack localLanguagePack = HsmLangPack.GetLocalLanguagePack(requestedNs, suggestedLocaleInfo.Lg, suggestedLocaleInfo.Lc);
            if (localLanguagePack != null)
              suggestedLocaleInfo.Hash = localLanguagePack.LpHash;
          }
          break;
        }
      }
      if (result == null && !flag1 && HsmLangPack.ShouldBackoff(requestedNs, lower, locale))
        result = new HsmTranslateResult()
        {
          ResultCode = HsmTranslateResultCode.TryAgainLater,
          ResultString = (string) null
        };
      if (result == null)
        return await HsmLangPack.TranslateAfterRequestPackAsync(hsmMsg, requestedNs, localesToLookFor, requestReason);
      completionSource.SetResult(result);
      return await completionSource.Task;
    }

    private static async Task<HsmTranslateResult> TranslateAfterRequestPackAsync(
      WhatsApp.ProtoBuf.Message.HighlyStructuredMessage hsmMsg,
      string requestedNs,
      List<HsmLocalePackInfo> localesBeingRequested,
      string requestReason)
    {
      string requestedLg = localesBeingRequested != null && localesBeingRequested.Count<HsmLocalePackInfo>() >= 1 && localesBeingRequested[0].Lg != null ? localesBeingRequested[0].Lg : throw new ArgumentOutOfRangeException("TranslateAfterRequestPackAsync: Invalid locales for request for language packs");
      string requestedLc = localesBeingRequested[0].Lc;
      TaskCompletionSource<HsmTranslateResult> tcs = new TaskCompletionSource<HsmTranslateResult>();
      Action<string, string, string, string, string, byte[]> onReceivedOK = (Action<string, string, string, string, string, byte[]>) ((downloadReason, suppliedNs, suppliedLg, suppliedLc, lpHash, blob) =>
      {
        try
        {
          Log.l("hsm", "onDownloadComplete for {0}: {1} {2} {3} {4} {5}", (object) (downloadReason ?? "?"), (object) (suppliedNs ?? "-"), (object) (suppliedLg ?? "-"), (object) (suppliedLc ?? "-"), (object) (lpHash ?? "-"), (object) (blob != null ? blob.Length : -1));
          suppliedLg = suppliedLg?.ToLower();
          suppliedLc = suppliedLc?.ToUpper();
          if (!string.IsNullOrEmpty(suppliedNs) && suppliedNs != requestedNs)
            throw new ArgumentOutOfRangeException("Received unexpected namespace - expected: {0}, supplied {1}", (object) requestedNs, suppliedNs);
          HsmLocalePackInfo hsmLocalePackInfo1 = (HsmLocalePackInfo) null;
          foreach (HsmLocalePackInfo hsmLocalePackInfo2 in localesBeingRequested)
          {
            if (suppliedLg == hsmLocalePackInfo2.Lg && (suppliedLc ?? "") == (hsmLocalePackInfo2.Lc ?? ""))
            {
              hsmLocalePackInfo1 = hsmLocalePackInfo2;
              break;
            }
            HsmLangPack.SetLanguagePackMissing(requestedNs, hsmLocalePackInfo2.Lg, hsmLocalePackInfo2.Lc);
          }
          if (blob == null && (hsmLocalePackInfo1 == null || string.IsNullOrEmpty(hsmLocalePackInfo1.Hash)))
          {
            Log.l("hsm", "no update for request {0} {1} {2} {3} {4} {5}", (object) (requestedNs ?? "-"), (object) (requestedLg ?? "-"), (object) (requestedLc ?? "-"), (object) (suppliedLg ?? "-"), (object) (suppliedLc ?? "-"), (object) lpHash);
            HsmLangPack.SetBackoff(requestedNs, requestedLg, requestedLc, HsmLangPack.HSM_BACKOFF_MINS_SHORT);
            tcs.SetResult(new HsmTranslateResult()
            {
              ResultCode = HsmTranslateResultCode.Failed,
              ResultString = (string) null
            });
          }
          else
          {
            HsmLangPack langPack;
            if (blob == null)
            {
              langPack = HsmLangPack.GetLocalLanguagePack(requestedNs, hsmLocalePackInfo1.Lg, hsmLocalePackInfo1.Lc);
              if (langPack == null)
              {
                Log.l("hsm", "language pack unexpectedly deleted {0} {1} {2}", (object) (requestedNs ?? "-"), (object) (hsmLocalePackInfo1.Lg ?? "-"), (object) (hsmLocalePackInfo1.Lc ?? "-"));
                tcs.SetResult(new HsmTranslateResult()
                {
                  ResultCode = HsmTranslateResultCode.Failed,
                  ResultString = (string) null
                });
                return;
              }
              if (downloadReason == HsmLangPack.REASON_REFRESH)
              {
                langPack.RequestedTimestamp = DateTime.UtcNow;
                SqliteHsm.AddLanguagePack(langPack.LpNamespace, langPack.LpLg, langPack.LpLc, langPack.RequestedTimestamp, langPack.LpHash);
              }
            }
            else
            {
              langPack = new HsmLangPack(suppliedNs, suppliedLg, suppliedLc);
              langPack.LpHash = lpHash;
              langPack.LpType = HsmLangPackTypeCode.Pack;
              langPack.PersistLanguagePack(blob, lpHash);
            }
            HsmLangPack.CacheLangPack(langPack);
            HsmLangPack.TranslateFailureReason failedReason;
            string str = langPack.Translate(hsmMsg, out failedReason);
            if (str != null)
            {
              tcs.SetResult(new HsmTranslateResult()
              {
                ResultCode = HsmTranslateResultCode.Succeeded,
                ResultString = str
              });
            }
            else
            {
              HsmLangPack.SetBackoff(requestedNs, requestedLg, requestedLc, HsmLangPack.HSM_BACKOFF_MINS_SHORT);
              Log.l("hsm", "Translation failed {0}, {1}", (object) hsmMsg.ElementName, (object) failedReason);
              tcs.SetResult(new HsmTranslateResult()
              {
                ResultCode = HsmTranslateResultCode.Failed,
                ResultString = (string) null
              });
            }
          }
        }
        catch (Exception ex1)
        {
          Log.LogException(ex1, "hsm Exception processing in onDownloadComplete");
          try
          {
            tcs.SetResult(new HsmTranslateResult()
            {
              ResultCode = HsmTranslateResultCode.Failed,
              ResultString = (string) null
            });
          }
          catch (Exception ex2)
          {
            Log.LogException(ex2, "Exception completing task");
          }
        }
      });
      Action<int> onReceivedError = (Action<int>) (error =>
      {
        try
        {
          Log.l("hsm", "onDownloadError {0}", (object) error);
          HsmLangPack.SetBackoff(requestedNs, requestedLg, requestedLc, error == 501 ? HsmLangPack.HSM_BACKOFF_MINS_LONG : HsmLangPack.HSM_BACKOFF_MINS_SHORT);
          switch (error)
          {
            case 400:
              tcs.SetResult(new HsmTranslateResult()
              {
                ResultCode = HsmTranslateResultCode.Failed,
                ResultString = (string) null
              });
              break;
            case 404:
              foreach (HsmLocalePackInfo hsmLocalePackInfo in localesBeingRequested)
                HsmLangPack.SetLanguagePackMissing(requestedNs, hsmLocalePackInfo.Lg, hsmLocalePackInfo.Lc);
              tcs.SetResult(new HsmTranslateResult()
              {
                ResultCode = HsmTranslateResultCode.Failed,
                ResultString = (string) null
              });
              break;
            default:
              tcs.SetResult(new HsmTranslateResult()
              {
                ResultCode = HsmTranslateResultCode.TryAgainLater,
                ResultString = (string) null
              });
              break;
          }
        }
        catch (Exception ex3)
        {
          Log.LogException(ex3, "hsm exception processing in onDownloadError");
          try
          {
            tcs.SetResult(new HsmTranslateResult()
            {
              ResultCode = HsmTranslateResultCode.Failed,
              ResultString = (string) null
            });
          }
          catch (Exception ex4)
          {
            Log.LogException(ex4, "Exception completing task");
          }
        }
      });
      try
      {
        Log.l("hsm", "Requesting language packs for {0}", (object) requestedNs);
        if (!HsmLangPack.DownloadMessagePack(requestedNs, localesBeingRequested, requestReason, onReceivedOK, onReceivedError))
          tcs.SetResult(new HsmTranslateResult()
          {
            ResultCode = HsmTranslateResultCode.TryAgainLater,
            ResultString = (string) null
          });
      }
      catch (Exception ex)
      {
        string context = string.Format("hsm exception sending language request {0}", (object) requestedNs);
        Log.LogException(ex, context);
        tcs.SetResult(new HsmTranslateResult()
        {
          ResultCode = HsmTranslateResultCode.TryAgainLater,
          ResultString = (string) null
        });
      }
      return await tcs.Task;
    }

    private static HsmTranslateResult TryTranslateUsingExistingPacks(
      WhatsApp.ProtoBuf.Message.HighlyStructuredMessage hsmMsg,
      string suggestedNs,
      ref HsmLocalePackInfo suggestedLocaleInfo,
      out bool downloadNeeded,
      out string downloadReason)
    {
      HsmTranslateResult hsmTranslateResult = (HsmTranslateResult) null;
      downloadNeeded = false;
      downloadReason = (string) null;
      string str1 = suggestedNs;
      string lg = suggestedLocaleInfo.Lg;
      string lc = suggestedLocaleInfo.Lc;
      HsmLangPack localLanguagePack = HsmLangPack.GetLocalLanguagePack(str1, lg, lc, true);
      if (localLanguagePack == null)
      {
        Log.d("hsm", "Language pack not found");
        downloadNeeded = true;
        List<HsmLangPack> langPacks = SqliteHsm.GetLangPacks(str1);
        downloadReason = langPacks == null || langPacks.Count == 0 ? HsmLangPack.REASON_NS : HsmLangPack.REASON_LOCALE;
      }
      else if (localLanguagePack.LpType == HsmLangPackTypeCode.Pack)
      {
        suggestedLocaleInfo.Hash = localLanguagePack.LpHash;
        if (localLanguagePack.MessagePack == null)
        {
          Log.l("hsm", "Language pack lost from device");
          downloadNeeded = true;
          downloadReason = HsmLangPack.REASON_NS;
        }
        else if (localLanguagePack.RequestedTimestamp < DateTime.UtcNow.AddDays(-7.0))
        {
          Log.l("hsm", "Language pack too old");
          downloadNeeded = true;
          downloadReason = HsmLangPack.REASON_REFRESH;
        }
        else
        {
          HsmLangPack.TranslateFailureReason failedReason;
          string str2 = localLanguagePack.Translate(hsmMsg, out failedReason);
          if (str2 != null)
          {
            hsmTranslateResult = new HsmTranslateResult()
            {
              ResultCode = HsmTranslateResultCode.Succeeded,
              ResultString = str2
            };
          }
          else
          {
            Log.l("hsm", "Translate failed: {0}, trying download", (object) failedReason);
            downloadNeeded = true;
            downloadReason = hsmMsg.ElementName;
          }
        }
      }
      else
      {
        downloadNeeded = localLanguagePack.RequestedTimestamp < DateTime.UtcNow.AddDays(-7.0);
        if (downloadNeeded)
          downloadReason = HsmLangPack.REASON_REFRESH;
      }
      return hsmTranslateResult;
    }

    private static HsmLangPack GetLocalLanguagePack(
      string ns,
      string lg,
      string lc,
      bool cachePack = false)
    {
      HsmLangPack langPack = HsmLangPack.GetCachedLangPack(ns, lg, lc);
      if (langPack == null)
      {
        langPack = HsmLangPack.LoadLanguagePack(ns, lg, lc);
        if (cachePack)
          HsmLangPack.CacheLangPack(langPack);
      }
      return langPack;
    }

    private static HsmLangPack LoadLanguagePack(string lpNamespace, string lpLg, string lpLc)
    {
      HsmLangPack langPack = SqliteHsm.GetLangPack(lpNamespace, lpLg, lpLc);
      if (langPack != null && langPack.LpType == HsmLangPackTypeCode.Pack)
      {
        langPack.MessagePack = (HighlyStructuredMessagePack) null;
        string filePath = HsmLangPack.ConvertLpIdToFilePath(lpNamespace, lpLg, lpLc);
        try
        {
          using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
          {
            Stream stream;
            using (Stream input = nativeMediaStorage.OpenFile(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
              stream = (Stream) input.Gunzip();
            using (stream)
              langPack.MessagePack = HighlyStructuredMessagePack.Deserialize(stream);
          }
        }
        catch (Exception ex)
        {
          string context = "hsm exception reading language pack " + filePath;
          Log.LogException(ex, context);
          langPack.MessagePack = (HighlyStructuredMessagePack) null;
        }
      }
      return langPack;
    }

    private static bool DownloadMessagePack(
      string reqNamespace,
      List<HsmLocalePackInfo> localesToLookFor,
      string requestReason,
      Action<string, string, string, string, string, byte[]> onReceivedOK,
      Action<int> onReceivedError)
    {
      FunXMPP.Connection connection = AppState.GetConnection();
      if (connection == null)
        return false;
      connection.SendGetHSMLanguagePack(reqNamespace, localesToLookFor, requestReason, onReceivedOK, onReceivedError);
      return true;
    }

    private static HighlyStructuredMessagePack extractMessagePack(byte[] lpBlob)
    {
      using (MemoryStream memoryStream = new MemoryStream(lpBlob))
        return HighlyStructuredMessagePack.Deserialize((Stream) memoryStream);
    }

    private static string ConvertLpIdToFilePath(string lpNamespace, string lpLg, string lpLc)
    {
      return HsmLangPack.HSM_MSG_PACK_PATH + "\\" + lpNamespace.Replace(":", "_").Replace("|", "_") + "_" + lpLg + (string.IsNullOrEmpty(lpLc) ? "__" : "_" + lpLc) + ".lpz";
    }

    public static bool SetLanguagePackMissing(string lpNamespace, string lpLg, string lpLc)
    {
      HsmLangPack localLanguagePack = HsmLangPack.GetLocalLanguagePack(lpNamespace, lpLg, lpLc);
      DateTime utcNow = DateTime.UtcNow;
      SqliteHsm.AddLanguagePackNotOnServer(lpNamespace, lpLg, lpLc, utcNow);
      bool flag;
      if (localLanguagePack != null && localLanguagePack.LpType == HsmLangPackTypeCode.NotPresent)
      {
        flag = true;
        localLanguagePack.RequestedTimestamp = utcNow;
      }
      else
      {
        string filePath = HsmLangPack.ConvertLpIdToFilePath(lpNamespace, lpLg, lpLc);
        try
        {
          using (new IsoStoreMediaStorage())
          {
            try
            {
              using (IMediaStorage mediaStorage = MediaStorage.Create(filePath))
              {
                mediaStorage.DeleteFile(filePath);
                Log.l("hsm", "removed file associated with lp {0}", (object) filePath);
              }
            }
            catch (Exception ex)
            {
            }
          }
          HsmLangPack.RemoveCachedLangPack(lpNamespace, lpLg, lpLc);
          flag = true;
        }
        catch (Exception ex)
        {
          string context = "hsm exception indicating language pack missing " + filePath;
          Log.LogException(ex, context);
          flag = false;
        }
      }
      return flag;
    }

    private static bool ShouldBackoff(string ns, string lg, string lc)
    {
      string filePath = HsmLangPack.ConvertLpIdToFilePath(ns, lg, lc);
      long ticks = -1;
      HsmLangPack.backoffRequest.TryGetValue(filePath, out ticks);
      if (ticks >= 0L)
      {
        if (DateTime.Now.Ticks < ticks)
        {
          Log.l("hsm", "backoff for {0} {1} {2}, until {3}", (object) ns, (object) lg, (object) lc, (object) new DateTime(ticks));
          return true;
        }
        HsmLangPack.backoffRequest.Remove(filePath);
      }
      return false;
    }

    private static void SetBackoff(string ns, string lg, string lc, int minutesBackoff)
    {
      Log.l("hsm", "setting backoff for {0} {1} {2} {3}", (object) ns, (object) lg, (object) lc, (object) minutesBackoff);
      string filePath = HsmLangPack.ConvertLpIdToFilePath(ns, lg, lc);
      Dictionary<string, long> backoffRequest = HsmLangPack.backoffRequest;
      string key = filePath;
      DateTime dateTime = DateTime.Now;
      dateTime = dateTime.AddMinutes((double) minutesBackoff);
      long ticks = dateTime.Ticks;
      backoffRequest[key] = ticks;
    }

    public enum TranslateFailureReason
    {
      NoError,
      NoLanguagePack,
      ElementNameIsMissing,
      PluralDoesnotConvert,
      IncorrectNumberOfParameters,
      Other,
    }
  }
}
