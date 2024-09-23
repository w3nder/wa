// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactSupportHelper
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Info;
using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using WhatsApp.WaCollections;
using WhatsAppNative;
using Windows.ApplicationModel.Email;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.Storage.Streams;

#nullable disable
namespace WhatsApp
{
  public class ContactSupportHelper
  {
    public static List<string> SupportEmailAddresses
    {
      get
      {
        List<string> supportEmailAddresses = new List<string>();
        supportEmailAddresses.Add("wp@support.whatsapp.com");
        if (Settings.IsWaAdmin)
          supportEmailAddresses.Add("wp-dev@whatsapp.com");
        return supportEmailAddresses;
      }
    }

    public static async void SendSupportEmail(
      string userFeedback,
      string logId,
      string context,
      int faqResultsReturned,
      int faqResultsRead,
      object attachments)
    {
      try
      {
        EmailMessage mail = new EmailMessage();
        List<string> supportEmailAddresses = ContactSupportHelper.SupportEmailAddresses;
        IList<EmailRecipient> to = mail.To;
        EmailRecipient emailRecipient1 = new EmailRecipient();
        emailRecipient1.put_Address(supportEmailAddresses[0]);
        to.Add(emailRecipient1);
        if (supportEmailAddresses.Count > 1)
        {
          IList<EmailRecipient> cc = mail.CC;
          EmailRecipient emailRecipient2 = new EmailRecipient();
          emailRecipient2.put_Address(supportEmailAddresses[1]);
          cc.Add(emailRecipient2);
        }
        string emailSubjectBeta = AppResources.SupportEmailSubjectBeta;
        if (Settings.IsWaAdmin)
          emailSubjectBeta += string.Format(" {0} ({1})", (object) AppState.GetAppVersion(), (object) AppState.OSVersion.ToString());
        mail.put_Subject(emailSubjectBeta);
        mail.put_Body(ContactSupportHelper.GenerateSupportEmailBody(userFeedback, logId, context, faqResultsReturned, faqResultsRead));
        string jsonDebugInfo = ContactSupportHelper.GenerateSupportJson(context, faqResultsReturned, faqResultsRead);
        Log.WriteLineDebug("JSON Debug Info:\n{0}", (object) jsonDebugInfo);
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        if (localFolder != null)
        {
          StorageFolder folderAsync = await localFolder.CreateFolderAsync("tmp", (CreationCollisionOption) 3);
          if (folderAsync != null)
          {
            StorageFile jsonFile = await folderAsync.CreateFileAsync("debuginfo.json", (CreationCollisionOption) 1);
            if (jsonFile != null)
            {
              using (Stream s = await ((IStorageFile) jsonFile).OpenStreamForWriteAsync())
              {
                using (StreamWriter writer = new StreamWriter(s))
                  await writer.WriteAsync(jsonDebugInfo);
              }
              mail.Attachments.Add(new EmailAttachment(jsonFile.Name, (IRandomAccessStreamReference) jsonFile));
            }
            jsonFile = (StorageFile) null;
          }
        }
        using (NativeMediaStorage fs = new NativeMediaStorage())
        {
          NativeStream tempFile = fs.GetTempFile();
          Log.AttachSupportLogs(logId, tempFile);
          mail.Attachments.Add(new EmailAttachment(logId + ".txt.gz", (IRandomAccessStreamReference) RandomAccessStreamReference.CreateFromStream(tempFile.AsWinRtStream())));
          if (attachments is IEnumerable<StorageFile> storageFiles)
          {
            foreach (StorageFile storageFile in storageFiles)
              mail.Attachments.Add(new EmailAttachment(storageFile.Name, (IRandomAccessStreamReference) storageFile));
          }
          await EmailManager.ShowComposeNewEmailAsync(mail);
        }
        mail = (EmailMessage) null;
        jsonDebugInfo = (string) null;
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "Sending Support Mail Failed", false);
      }
    }

    private static Pair<string, string> P(string k, string v)
    {
      return new Pair<string, string>()
      {
        First = k,
        Second = v
      };
    }

    private static Pair<string, string> LineBreak()
    {
      return new Pair<string, string>() { Second = "" };
    }

    public static LinkedList<Pair<string, string>> CollectDebugInfo(
      string context,
      int faqResultsReturned,
      int faqResultsRead)
    {
      LinkedList<Pair<string, string>> linkedList = new LinkedList<Pair<string, string>>();
      if (Settings.OldChatID == null)
      {
        if (string.IsNullOrEmpty(Settings.ChatID))
          linkedList.AddLast(ContactSupportHelper.P("Debug info", "unregistered"));
        else
          linkedList.AddLast(ContactSupportHelper.P("Debug info", string.Format("+{0}", (object) Settings.ChatID)));
      }
      else
        linkedList.AddLast(ContactSupportHelper.P("Debug info", string.Format("chnum unregistered (+{0} to +{1}{2})", (object) Settings.OldChatID, (object) Settings.CountryCode, (object) Settings.PhoneNumber)));
      string countryCode = Settings.CountryCode;
      if (!string.IsNullOrEmpty(countryCode))
        linkedList.AddLast(ContactSupportHelper.P("CCode", countryCode));
      string v1 = (string) null;
      try
      {
        MemoryStream destination = new MemoryStream();
        using (Stream stream = App.OpenFromXAP("git-info"))
          stream.CopyTo((Stream) destination);
        byte[] array = destination.ToArray();
        v1 = Encoding.UTF8.GetString(array, 0, array.Length).Trim();
      }
      catch (Exception ex)
      {
      }
      if (!string.IsNullOrEmpty(v1))
        linkedList.AddLast(ContactSupportHelper.P("Description", v1));
      linkedList.AddLast(ContactSupportHelper.P("Version", AppState.GetAppVersion()));
      string lang;
      string locale;
      AppState.GetLangAndLocale(out lang, out locale);
      linkedList.AddLast(ContactSupportHelper.P("LC", string.IsNullOrEmpty(locale) ? "zz" : locale));
      linkedList.AddLast(ContactSupportHelper.P("LG", string.IsNullOrEmpty(lang) ? "" : lang.ToUpper()));
      linkedList.AddLast(ContactSupportHelper.P("Context", context ?? ContactSupportHelper.AppendPhoneNumberIfNotLoggedIn("unk")));
      linkedList.AddLast(ContactSupportHelper.P("Carrier", DeviceNetworkInformation.CellularMobileOperator));
      linkedList.AddLast(ContactSupportHelper.P("Manufacturer", DeviceStatus.DeviceManufacturer));
      linkedList.AddLast(ContactSupportHelper.P("Model", DeviceStatus.DeviceName));
      linkedList.AddLast(ContactSupportHelper.P("OS", AppState.OSVersion.ToString()));
      TimeSpan timeSpan1 = TimeSpan.FromMilliseconds((double) NativeInterfaces.Misc.GetTickCount());
      linkedList.AddLast(ContactSupportHelper.P("Uptime", timeSpan1.ToString()));
      string v2 = (string) null;
      switch (FunRunner.SocketState)
      {
        case FunRunner.SocketStates.Disconnected:
          v2 = "DN";
          break;
        case FunRunner.SocketStates.Connecting:
          v2 = "SC";
          break;
        case FunRunner.SocketStates.LoggingIn:
          v2 = "XC";
          break;
        case FunRunner.SocketStates.Connected:
          v2 = "UP";
          break;
      }
      if (Settings.LoginFailed)
        v2 = "PW";
      DateTime? nullable1;
      DateTime? nullable2;
      if (v2 != "UP")
      {
        string str = App.LastLoginTime.HasValue ? "lost" : "none";
        v2 = string.Format("{0} {1}", (object) v2, (object) str);
        nullable1 = Settings.SuccessfulLoginUtc;
        DateTime? nullable3;
        if (!nullable1.HasValue)
        {
          nullable1 = new DateTime?();
          nullable3 = nullable1;
        }
        else
        {
          nullable1 = Settings.SuccessfulLoginUtc;
          nullable3 = new DateTime?(DateTimeUtils.FunTimeToPhoneTime(nullable1.Value));
        }
        nullable2 = nullable3;
      }
      else
        nullable2 = App.LastLoginTime;
      DateTime now = DateTime.Now;
      if (nullable2.HasValue)
      {
        nullable1 = nullable2;
        DateTime dateTime = now;
        if ((nullable1.HasValue ? (nullable1.GetValueOrDefault() < dateTime ? 1 : 0) : 0) != 0)
        {
          TimeSpan timeSpan2 = now - nullable2.Value;
          object[] objArray = new object[5]
          {
            (object) v2,
            (object) (int) timeSpan2.TotalHours,
            null,
            null,
            null
          };
          int num = timeSpan2.Minutes;
          objArray[2] = (object) num.ToString().PadLeft(2, '0');
          num = timeSpan2.Seconds;
          objArray[3] = (object) num.ToString().PadLeft(2, '0');
          num = timeSpan2.Milliseconds % 100;
          objArray[4] = (object) num.ToString().PadLeft(3, '0');
          v2 = string.Format("{0} {1}:{2}:{3}.{4}", objArray);
        }
      }
      linkedList.AddLast(ContactSupportHelper.P("Socket Conn", v2));
      string str1 = (string) null;
      bool flag1 = false;
      try
      {
        CELL_INFO cellInfo = NativeInterfaces.Misc.GetCellInfo();
        str1 = string.Format("{0}-{1}", (object) cellInfo.Mcc.ToString().PadLeft(3, '0'), (object) cellInfo.Mnc.ToString().PadLeft(3, '0'));
        flag1 = cellInfo.Roaming;
      }
      catch (Exception ex)
      {
      }
      linkedList.AddLast(ContactSupportHelper.P("Radio MCC-MNC", str1 ?? "n/a"));
      try
      {
        ConnectionProfile connectionProfile1 = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
        if (connectionProfile1 != null)
        {
          string str2 = string.Format("{0} - Wifi {1} Cellular {2}", (object) connectionProfile1.GetNetworkConnectivityLevel(), (object) connectionProfile1.IsWlanConnectionProfile, (object) connectionProfile1.IsWwanConnectionProfile);
          Log.l("Conn", "Internet connection profile {0}: {1}", (object) connectionProfile1.ProfileName, (object) str2);
        }
        else
          Log.l("Conn", "No internet profile found");
        int num1 = 0;
        int num2 = 0;
        try
        {
          foreach (ConnectionProfile connectionProfile2 in Windows.Networking.Connectivity.NetworkInformation.GetConnectionProfiles().Where<ConnectionProfile>((Func<ConnectionProfile, bool>) (i => i.GetNetworkConnectivityLevel() > 0)))
          {
            if (connectionProfile2.IsWlanConnectionProfile)
            {
              linkedList.AddLast(ContactSupportHelper.P("Wifi Conn", string.Format("Access {0}", (object) connectionProfile2.GetNetworkConnectivityLevel())));
              ++num1;
            }
            if (connectionProfile2.IsWwanConnectionProfile)
            {
              linkedList.AddLast(ContactSupportHelper.P("Cellular Conn", string.Format("Access {0}", (object) connectionProfile2.GetNetworkConnectivityLevel())));
              ++num2;
            }
            string str3 = string.Format("{0} - Wifi {1} Cellular {2}", (object) connectionProfile2.GetNetworkConnectivityLevel(), (object) connectionProfile2.IsWlanConnectionProfile, (object) connectionProfile2.IsWwanConnectionProfile);
            WwanConnectionProfileDetails connectionProfileDetails = connectionProfile1.WwanConnectionProfileDetails;
            if (connectionProfile2.IsWwanConnectionProfile && connectionProfileDetails != null)
            {
              WwanDataClass currentDataClass = connectionProfileDetails.GetCurrentDataClass();
              str3 = string.Format("{0}, {1}", (object) str3, (object) currentDataClass);
            }
            Log.l("Conn", "Connected profile {0}: {1}", (object) connectionProfile2.ProfileName, (object) str3);
          }
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "Exception checking connectivity using connection profile");
        }
        if (num2 == 0)
        {
          Log.l("Conn", "No connected cellular profiles found: {0}", (object) DeviceNetworkInformation.IsCellularDataEnabled);
          linkedList.AddLast(ContactSupportHelper.P("Cellular Conn", string.Format("None - {0}", DeviceNetworkInformation.IsCellularDataEnabled ? (object) "Enabled" : (object) "Not enabled")));
        }
        if (num1 == 0)
        {
          Log.l("Conn", "No connected Wifi profiles found {0}", (object) DeviceNetworkInformation.IsWiFiEnabled);
          linkedList.AddLast(ContactSupportHelper.P("Wifi Conn", string.Format("None - {0}", DeviceNetworkInformation.IsWiFiEnabled ? (object) "Enabled" : (object) "Not enabled")));
        }
        NetworkInterfaceList source = new NetworkInterfaceList();
        if (source.Count<NetworkInterfaceInfo>() > 0)
        {
          foreach (NetworkInterfaceInfo networkInterfaceInfo in source)
            Log.l("Conn", "Connected Interface {0}: {1}, {2}, {3}, {4}", (object) networkInterfaceInfo.InterfaceName, (object) networkInterfaceInfo.InterfaceState, (object) networkInterfaceInfo.InterfaceType, (object) networkInterfaceInfo.InterfaceSubtype, (object) networkInterfaceInfo.Characteristics);
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception processing connection profiles/interfaces");
      }
      ulong? nullable4 = new ulong?();
      try
      {
        nullable4 = new ulong?(NativeInterfaces.Misc.GetDiskSpace("C:").FreeBytes);
      }
      catch (Exception ex)
      {
      }
      if (nullable4.HasValue)
      {
        int num = (int) (nullable4.Value / 1048576UL);
        linkedList.AddLast(ContactSupportHelper.P("Free Space Built-In", nullable4.ToString() + " (" + num.ToString() + " MB)"));
      }
      ulong? nullable5 = new ulong?();
      try
      {
        nullable5 = new ulong?(NativeInterfaces.Misc.GetDiskSpace("D:").FreeBytes);
      }
      catch (Exception ex)
      {
      }
      if (nullable5.HasValue)
      {
        int num = (int) (nullable5.Value / 1048576UL);
        linkedList.AddLast(ContactSupportHelper.P("Free Space Removable", nullable5.ToString() + " (" + num.ToString() + " MB)"));
      }
      if (faqResultsReturned >= 0)
      {
        linkedList.AddLast(ContactSupportHelper.P("FAQ Results Returned", faqResultsReturned.ToString()));
        string v3 = faqResultsReturned > 0 ? faqResultsRead.ToString() : "n/a";
        linkedList.AddLast(ContactSupportHelper.P("FAQ Results Read", v3));
      }
      linkedList.AddLast(ContactSupportHelper.P("Cached Connection FAQ", ConnectionHelp.ConnectionFAQTimeRead > 0 ? "yes" : "no"));
      linkedList.AddLast(ContactSupportHelper.P("Cached Connection FAQ Time Read", ConnectionHelp.ConnectionFAQTimeRead > 0 ? ConnectionHelp.ConnectionFAQTimeRead.ToString() + " s" : "n/a"));
      ConnectionHelp.ConnectionFAQTimeRead = 0;
      linkedList.AddLast(ContactSupportHelper.LineBreak());
      linkedList.AddLast(ContactSupportHelper.P("Device Model Version", DeviceStatus.DeviceHardwareVersion));
      linkedList.AddLast(ContactSupportHelper.P("Device Firmware Version", DeviceStatus.DeviceFirmwareVersion));
      string v4 = PushSystem.Instance.PushState ?? "null";
      linkedList.AddLast(ContactSupportHelper.P("Push", v4));
      bool flag2 = NetworkStateMonitor.Is3GOrBetter();
      bool flag3 = NetworkStateMonitor.Is2GConnection();
      if (NetworkStateMonitor.IsWifiDataConnected())
        linkedList.AddLast(ContactSupportHelper.P("Connection", "wifi"));
      else if (flag2 | flag3)
        linkedList.AddLast(ContactSupportHelper.P("Connection", "mobile"));
      else
        linkedList.AddLast(ContactSupportHelper.P("Connection", "none"));
      if (flag2 | flag3)
        linkedList.AddLast(ContactSupportHelper.P("Network Type", (flag2 ? "3G+" : "Edge") + (flag1 ? " roaming" : "")));
      else
        linkedList.AddLast(ContactSupportHelper.P("Network Type", "none"));
      string lastDataCenterUsed = NonDbSettings.LastDataCenterUsed;
      linkedList.AddLast(ContactSupportHelper.P("Datacenter", lastDataCenterUsed ?? "Unknown"));
      linkedList.AddLast(ContactSupportHelper.P("Device ISO8601", DateTime.Now.ToString("yyyy-MM-dd HHmmss.fffzzz")));
      linkedList.AddLast(ContactSupportHelper.P("Battery Saver Mode", AppState.BatterySaverEnabled.ToString()));
      if (AppState.GetConnection().EventHandler.Qr.Session.Active)
        linkedList.AddLast(ContactSupportHelper.P("Web session", "active"));
      else if (AppState.GetConnection().EventHandler.Qr.Session.HasConnections)
        linkedList.AddLast(ContactSupportHelper.P("Web session", "token saved"));
      string contactException = Settings.LastContactException;
      if (contactException != null)
      {
        string[] strArray = contactException.Split(',');
        long result = 0;
        if (strArray.Length > 1 && long.TryParse(strArray[1], out result) && FunXMPP.UnixEpoch.AddSeconds((double) result) >= DateTime.Now.AddDays(-1.0))
          linkedList.AddLast(ContactSupportHelper.P("Contacts error", strArray[0]));
      }
      linkedList.AddLast(ContactSupportHelper.P("target", "beta"));
      linkedList.AddLast(ContactSupportHelper.P("Video calls", "enabled"));
      int certSmbUsers = 0;
      int certEntUsers = 0;
      int certTotalUsers = 0;
      ContactsContext.Instance((Action<ContactsContext>) (db =>
      {
        foreach (UserStatus statusesWithCertificate in db.UserStatusesWithCertificates())
        {
          ++certTotalUsers;
          if (statusesWithCertificate.IsSmb())
            ++certSmbUsers;
          else if (statusesWithCertificate.IsEnterprise())
            ++certEntUsers;
        }
      }));
      linkedList.AddLast(ContactSupportHelper.P("Smb count", certSmbUsers.ToString()));
      linkedList.AddLast(ContactSupportHelper.P("Ent count", certEntUsers.ToString()));
      if (certSmbUsers + certEntUsers != certTotalUsers)
        Log.l("biz", "Odd cert counts {0} != {1} + {2}", (object) certTotalUsers, (object) certSmbUsers, (object) certEntUsers);
      return linkedList;
    }

    private static string GenerateSupportEmailBody(
      string userFeedback,
      string uniqueId,
      string context,
      int faqResultsReturned,
      int faqResultsRead)
    {
      StringBuilder stringBuilder = new StringBuilder(userFeedback);
      stringBuilder.Append("\n\n--Support Info--\n");
      foreach (Pair<string, string> pair in ContactSupportHelper.CollectDebugInfo(context, faqResultsReturned, faqResultsRead))
      {
        if (string.IsNullOrEmpty(pair.First))
          stringBuilder.AppendFormat("{0}\n", (object) (pair.Second ?? ""));
        else
          stringBuilder.AppendFormat("{0}: {1}\n", (object) pair.First, (object) (pair.Second ?? ""));
      }
      return stringBuilder.ToString();
    }

    private static string GenerateSupportJson(
      string context,
      int faqResultsReturned,
      int faqResultsRead)
    {
      StringBuilder stringBuilder = new StringBuilder("{\n");
      foreach (Pair<string, string> pair in ContactSupportHelper.CollectDebugInfo(context, faqResultsReturned, faqResultsRead))
      {
        if (!string.IsNullOrEmpty(pair.First) && !string.IsNullOrEmpty(pair.Second))
          stringBuilder.AppendFormat("\"{0}\": \"{1}\",\n", (object) pair.First, (object) pair.Second);
      }
      stringBuilder.Remove(stringBuilder.Length - 2, 2);
      stringBuilder.Append("\n}");
      return stringBuilder.ToString();
    }

    public static string GenerateUniqueLogId()
    {
      string str1 = Settings.ChatID ?? Settings.PhoneNumber ?? ((IEnumerable<byte>) Settings.RecoveryToken).Take<byte>(8).ToArray<byte>().ToHexString();
      string str2 = DateTime.UtcNow.ToUnixTime().ToString();
      using (SHA1Managed shA1Managed = new SHA1Managed())
        return new string(shA1Managed.ComputeHash(Encoding.UTF8.GetBytes(str1 + str2)).ToHexString().Take<char>(16).ToArray<char>());
    }

    public static bool IsFeedbackAcceptable(string s)
    {
      int num = Settings.IsWaAdmin ? -1 : 10;
      return Encoding.UTF8.GetByteCount((s ?? "").Trim()) >= num;
    }

    public static string AppendPhoneNumberIfNotLoggedIn(string standardContext)
    {
      return string.IsNullOrEmpty(Settings.ChatID) && (!string.IsNullOrEmpty(Settings.CountryCode) || !string.IsNullOrEmpty(Settings.PhoneNumber)) ? string.Format("{0} +{1}{2}", (object) standardContext, (object) Settings.CountryCode, (object) Settings.PhoneNumber) : standardContext;
    }
  }
}
