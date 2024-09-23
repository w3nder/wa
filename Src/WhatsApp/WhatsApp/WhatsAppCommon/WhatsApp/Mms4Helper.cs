// Decompiled with JetBrains decompiler
// Type: WhatsApp.Mms4Helper
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public static class Mms4Helper
  {
    public static string MMS4_DIRECT_IP = "direct_ip";

    public static bool IsMms4UploadMessage(Message msg)
    {
      return msg != null && msg.KeyFromMe && Mms4ServerPropHelper.IsMms4EnabledForType(msg.GetFunMediaType(), true);
    }

    public static bool IsMms4DownloadMessage(Message msg)
    {
      return msg != null && !msg.KeyFromMe && Mms4ServerPropHelper.IsMms4EnabledForType(msg.GetFunMediaType(), false) && msg.GetCipherMediaHash() != null;
    }

    public static string GenerateMediaDownloadUrl(
      Message m,
      out bool isMms4,
      out string mms4IpAddress)
    {
      isMms4 = false;
      mms4IpAddress = (string) null;
      if (m != null && !m.KeyFromMe && Mms4Helper.IsMms4DownloadMessage(m))
      {
        FunXMPP.FMessage.FunMediaType funMediaType = m.GetFunMediaType();
        byte[] cipherMediaHash = m.GetCipherMediaHash();
        isMms4 = true;
        Mms4RouteSelector.SelectedRoute currentSelectedRoute = Mms4RouteSelector.GetInstance().CurrentSelectedRoute;
        if (currentSelectedRoute == null)
          return (string) null;
        mms4IpAddress = currentSelectedRoute.RouteIpAddress;
        return Mms4Helper.CreateUrlStringForDownload(currentSelectedRoute.RouteHostName, funMediaType, cipherMediaHash, !string.IsNullOrEmpty(mms4IpAddress), m.GetDirectPath());
      }
      return m?.MediaUrl;
    }

    public static string CreateUrlStringForDownload(
      string hostName,
      FunXMPP.FMessage.FunMediaType funType,
      byte[] cipherHash,
      bool ipHintAvailable,
      string directPath)
    {
      if (string.IsNullOrEmpty(directPath))
        return "https://" + hostName + "/mms/" + FunXMPP.FMessage.GetFunMediaTypeStr(funType) + "/" + Mms4Helper.ConvertBytesToUrlParm(cipherHash) + "?direct_ip=" + (ipHintAvailable ? "1" : "0");
      Log.d("mms4h", "using direct path");
      return "https://" + hostName + directPath;
    }

    public static string CreateUrlStringForUpload(
      string hostName,
      FunXMPP.FMessage.FunMediaType funType,
      string authToken,
      byte[] cipherHash,
      bool streaming,
      bool optimisticUpload,
      bool ipHintAvailable)
    {
      return "https://" + hostName + (optimisticUpload ? "/optimistic" : "/mms") + "/" + FunXMPP.FMessage.GetFunMediaTypeStr(funType) + "/" + Mms4Helper.ConvertBytesToUrlParm(cipherHash) + "?auth=" + authToken.ToUrlSafeBase64String() + "&token=" + Mms4Helper.ConvertCipherHashAndBytesToUrlParm(cipherHash, Settings.UploadUniqueBytes) + (streaming ? "&stream=1" : "") + "&direct_ip=" + (ipHintAvailable ? "1" : "0");
    }

    public static string ConvertBytesToUrlParm(byte[] bytes)
    {
      return Convert.ToBase64String(bytes, 0, bytes.Length).ToUrlSafeBase64String();
    }

    private static string ConvertCipherHashAndBytesToUrlParm(byte[] cipherHash, byte[] randomBytes)
    {
      if (randomBytes == null || randomBytes.Length < 1 || cipherHash == null || cipherHash.Length < 1)
      {
        Log.l("MmsHelper", "Invalid data for Url gen {0} {1}", (object) (cipherHash == null ? -1 : cipherHash.Length), (object) (randomBytes == null ? -1 : randomBytes.Length));
        throw new InvalidOperationException("Can't generate Upload Url");
      }
      byte[] numArray = new byte[cipherHash.Length + randomBytes.Length];
      Array.Copy((Array) cipherHash, (Array) numArray, cipherHash.Length);
      Array.Copy((Array) randomBytes, 0, (Array) numArray, cipherHash.Length, randomBytes.Length);
      byte[] inArray = (byte[]) null;
      try
      {
        using (SHA256Managed shA256Managed = new SHA256Managed())
          inArray = shA256Managed.ComputeHash(numArray);
        return Convert.ToBase64String(inArray, 0, inArray.Length).ToUrlSafeBase64String();
      }
      catch (Exception ex)
      {
        Log.l("mms", "Exception creating media upload url " + ex.ToString());
      }
      return (string) null;
    }

    public static IObservable<Pair<string, string>> GetMms4DownloadUrlObservable(
      FunXMPP.FMessage.FunMediaType funType,
      byte[] cipherHash)
    {
      return Observable.Create<Pair<string, string>>((Func<IObserver<Pair<string, string>>, Action>) (observer =>
      {
        IDisposable disp = Mms4HostSelector.GetInstance().GetSelectedHostObservable(true, funType, true).Subscribe<Mms4HostSelector.Mms4HostSelection>((Action<Mms4HostSelector.Mms4HostSelection>) (hostSelection =>
        {
          string stringForDownload = Mms4Helper.CreateUrlStringForDownload(hostSelection.HostName, funType, cipherHash, false, (string) null);
          observer.OnNext(new Pair<string, string>()
          {
            First = stringForDownload,
            Second = (string) null
          });
        }));
        return (Action) (() => disp.SafeDispose());
      }));
    }

    public static void MaybeWarmupMms4Host(Message msg)
    {
      if (msg == null)
        return;
      Mms4Helper.MaybeWarmupMms4Host(msg.KeyFromMe, msg.GetFunMediaType());
    }

    public static void MaybeWarmupMms4Route(FunXMPP.FMessage.FunMediaType mediaType, bool upload)
    {
      if (!Mms4ServerPropHelper.IsMms4EnabledForType(mediaType, upload))
        return;
      Mms4RouteSelector.GetInstance().OnMediaUseImminent();
    }

    public static void MaybeWarmupMms4Host(bool upload, FunXMPP.FMessage.FunMediaType mediaType)
    {
      if (!Mms4ServerPropHelper.IsMms4EnabledForType(mediaType, upload))
        return;
      Mms4HostSelector.GetInstance().OnMediaUseImminent(!upload, mediaType);
    }

    public static void MaybeScheduleMms4RouteSelection()
    {
      PersistentAction createdPa = (PersistentAction) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        PersistentAction[] persistentActions = db.GetPersistentActions(PersistentAction.Types.Mms4RouteSelection);
        if (persistentActions == null || persistentActions.Length == 0)
        {
          createdPa = Mms4Helper.CreateMms4RouteSelectionPa();
          db.StorePersistentAction(createdPa);
        }
        else
          Log.l("mms", "Not adding route selection - already added");
      }));
      if (createdPa == null)
        return;
      AppState.Worker.Enqueue((Action) (() => AppState.AttemptPersistentAction(createdPa)));
    }

    private static PersistentAction CreateMms4RouteSelectionPa()
    {
      byte[] source = new byte[1]{ (byte) 49 };
      return new PersistentAction()
      {
        ActionType = 35,
        ActionData = ((IEnumerable<byte>) source).ToArray<byte>(),
        ExpirationTime = new DateTime?(FunRunner.CurrentServerTimeUtc + TimeSpan.FromHours(12.0))
      };
    }

    public static IObservable<Unit> PerformMms4RouteSelection(FunXMPP.Connection conn)
    {
      Mms4RouteSelector selector = Mms4RouteSelector.GetInstance();
      return conn == null || selector.BackOffUntilTimeUtcTicks > DateTime.UtcNow.Ticks ? Observable.Empty<Unit>() : Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        if (selector.RequestUpdatedRoutingInfo(true))
          observer.OnNext(new Unit());
        return (Action) (() => { });
      })).SubscribeOn<Unit>(WAThreadPool.Scheduler);
    }

    public static void MaybeScheduleMms4HostSelection()
    {
      PersistentAction createdPa = (PersistentAction) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        PersistentAction[] persistentActions = db.GetPersistentActions(PersistentAction.Types.Mms4HostSelection);
        if (persistentActions == null || persistentActions.Length == 0)
        {
          createdPa = Mms4Helper.CreateMms4HostSelectionPa();
          db.StorePersistentAction(createdPa);
        }
        else
          Log.l("mmshs", "Not adding host selection - already added");
      }));
      if (createdPa == null)
        return;
      AppState.Worker.Enqueue((Action) (() => AppState.AttemptPersistentAction(createdPa)));
    }

    private static PersistentAction CreateMms4HostSelectionPa()
    {
      byte[] source = new byte[1]{ (byte) 49 };
      return new PersistentAction()
      {
        ActionType = 49,
        ActionData = ((IEnumerable<byte>) source).ToArray<byte>(),
        ExpirationTime = new DateTime?(FunRunner.CurrentServerTimeUtc + TimeSpan.FromHours(12.0))
      };
    }

    public static IObservable<Unit> PerformMms4HostSelection(FunXMPP.Connection conn)
    {
      Mms4HostSelector selector = Mms4HostSelector.GetInstance();
      return conn == null || selector.BackOffUntilTimeUtcTicks > DateTime.UtcNow.Ticks ? Observable.Empty<Unit>() : Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        if (selector.RequestUpdatedRoutingInfo(true))
          observer.OnNext(new Unit());
        return (Action) (() => { });
      })).SubscribeOn<Unit>(WAThreadPool.Scheduler);
    }
  }
}
