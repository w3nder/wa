// Decompiled with JetBrains decompiler
// Type: WhatsApp.GdprReport
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;

#nullable disable
namespace WhatsApp
{
  public static class GdprReport
  {
    public const string LogHeader = "gdpr";
    public const int DefaultWaitDays = 3;
    public const string IsoStoreDirPath = "gdpr_report";

    public static void SetStateInit()
    {
      Log.l("gdpr", "set state: init");
      Settings.GdprReportState = GdprReport.States.Init;
      Settings.GdprReportReadyTimeUtc = new DateTime?();
      Settings.GdprReportCreationTimeUtc = new DateTime?();
      Settings.GdprReportExpirationTimeUtc = new DateTime?();
      Settings.GdprReportInfo = (byte[]) null;
      string gdprReportFilepath = Settings.GdprReportFilepath;
      Settings.GdprReportFilepath = (string) null;
      GdprReport.DeleteLocalReportFile(gdprReportFilepath);
    }

    public static void SetStateRequesting()
    {
      Log.l("gdpr", "set state: requesting");
      Settings.GdprReportState = GdprReport.States.Requesting;
      Settings.GdprReportReadyTimeUtc = new DateTime?();
      Settings.GdprReportCreationTimeUtc = new DateTime?();
      Settings.GdprReportExpirationTimeUtc = new DateTime?();
      Settings.GdprReportInfo = (byte[]) null;
      string gdprReportFilepath = Settings.GdprReportFilepath;
      Settings.GdprReportFilepath = (string) null;
      GdprReport.DeleteLocalReportFile(gdprReportFilepath);
    }

    public static void SetStateRequestSent(DateTime readyTime)
    {
      Log.l("gdpr", "set state: request sent | ready on:{0}", (object) readyTime);
      Settings.GdprReportState = GdprReport.States.RequestSent;
      Settings.GdprReportReadyTimeUtc = new DateTime?(readyTime);
      Settings.GdprReportCreationTimeUtc = new DateTime?();
      Settings.GdprReportExpirationTimeUtc = new DateTime?();
      Settings.GdprReportInfo = (byte[]) null;
      string gdprReportFilepath = Settings.GdprReportFilepath;
      Settings.GdprReportFilepath = (string) null;
      GdprReport.DeleteLocalReportFile(gdprReportFilepath);
    }

    public static void SetStateReady(
      DateTime creationTime,
      DateTime expirationTime,
      byte[] reportInfo)
    {
      Log.l("gdpr", "set state: ready | created on:{0},expire on:{1}", (object) creationTime, (object) expirationTime);
      Settings.GdprReportState = GdprReport.States.Ready;
      Settings.GdprReportCreationTimeUtc = new DateTime?(creationTime);
      Settings.GdprReportExpirationTimeUtc = new DateTime?(expirationTime);
      Settings.GdprReportInfo = reportInfo;
      string gdprReportFilepath = Settings.GdprReportFilepath;
      Settings.GdprReportFilepath = (string) null;
      GdprReport.DeleteLocalReportFile(gdprReportFilepath);
    }

    public static void SetStateDownloading()
    {
      Log.l("gdpr", "set state: downloading");
      Settings.GdprReportState = GdprReport.States.Downloading;
    }

    public static void SetStateDownloaded(string filepath)
    {
      Log.l("gdpr", "set state: downloaded | filepath:{0}", (object) filepath);
      Settings.GdprReportState = GdprReport.States.Downloaded;
      Settings.GdprReportFilepath = filepath;
    }

    private static void DeleteLocalReportFile(string filepath)
    {
      try
      {
        using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
        {
          if (!string.IsNullOrEmpty(filepath) && storeForApplication.FileExists(filepath))
          {
            storeForApplication.DeleteFile(filepath);
            Log.l("gdpr", "gdpr report local file deleted | path:{0}", (object) filepath);
          }
          else
            Log.l("gdpr", "gdpr report local file not found | path:{0}", (object) (filepath ?? "n/a"));
          string path = "gdpr_report";
          if (!storeForApplication.DirectoryExists(path))
            return;
          string[] fileNames = storeForApplication.GetFileNames(string.Format("{0}\\*", (object) path));
          if (!((IEnumerable<string>) fileNames).Any<string>())
            return;
          Log.l("gdpr", "found {0} copies to delete", (object) ((IEnumerable<string>) fileNames).Count<string>());
          foreach (string str in fileNames)
          {
            string file = string.Format("{0}\\{1}", (object) path, (object) str);
            storeForApplication.DeleteFile(file);
            Log.l("gdpr", "gdpr report local file copy deleted | path:{0}", (object) file);
          }
        }
      }
      catch (Exception ex)
      {
        string context = string.Format("gdpr delete report local file | path:{0}", (object) filepath);
        Log.l(ex, context);
      }
    }

    public static void RevertStateDownloadingToReady()
    {
      if (Settings.GdprReportState != GdprReport.States.Downloading)
        return;
      Settings.GdprReportState = GdprReport.States.Ready;
      string gdprReportFilepath = Settings.GdprReportFilepath;
      Settings.GdprReportFilepath = (string) null;
      GdprReport.DeleteLocalReportFile(gdprReportFilepath);
    }

    public static Message CreateMessageFromReportInfo(byte[] reportInfo)
    {
      Message messageFromReportInfo = (Message) null;
      if (reportInfo != null)
      {
        try
        {
          FunXMPP.FMessage fMessage = new FunXMPP.FMessage(new FunXMPP.FMessage.Key("dummy-jid", false, "dummy-id"));
          WhatsApp.ProtoBuf.Message.CreateFromUnpaddedPlainText(reportInfo).PopulateFMessage(fMessage);
          messageFromReportInfo = new Message(fMessage);
        }
        catch (Exception ex)
        {
          Log.l(ex, "parse gdpr report protobuf");
        }
      }
      return messageFromReportInfo;
    }

    public static string GetOutputFilepath()
    {
      using (IsoStoreMediaStorage storeMediaStorage = new IsoStoreMediaStorage())
      {
        MediaDownload.CreateIsoStoreDirectory("gdpr_report");
        return storeMediaStorage.GetFullFsPath(string.Format("{0}/my-account-info.zip", (object) "gdpr_report"));
      }
    }

    public static void DeleteReport(bool localOnly, string context)
    {
      Log.l("gdpr", "delete report | local only:{0},context:{1}", (object) localOnly, (object) context);
      GdprReport.SetStateInit();
      if (localOnly)
        return;
      GdprReport.ScheduleDeleteGdprReport();
    }

    private static void ScheduleDeleteGdprReport()
    {
      AppState.SchedulePersistentAction(new PersistentAction()
      {
        ActionType = 47
      }, true);
    }

    public static IObservable<Unit> PerformDeleteGdprReport()
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        FunXMPP.Connection connection = AppState.GetConnection();
        if (connection == null)
          observer.OnCompleted();
        else
          connection.SendGdprDeleteReport((Action) (() =>
          {
            observer.OnNext(new Unit());
            observer.OnCompleted();
          }), (Action) (() => observer.OnCompleted()));
        return (Action) (() => { });
      }));
    }

    public enum States
    {
      Init = 0,
      RequestSent = 1,
      Ready = 2,
      Downloading = 3,
      Downloaded = 4,
      Requesting = 101, // 0x00000065
    }
  }
}
