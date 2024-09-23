// Decompiled with JetBrains decompiler
// Type: WhatsApp.UsyncQueryRequest
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using WhatsAppCommon;


namespace WhatsApp
{
  public sealed class UsyncQueryRequest
  {
    public static void SendForDetailsForJid(string jid)
    {
      UsyncQueryRequest.SendForDetailsForJid(jid, FunXMPP.Connection.SyncMode.Query, FunXMPP.Connection.SyncContext.Interactive);
    }

    public static void SendNotificationQueryForJid(string jid)
    {
      UsyncQueryRequest.SendForDetailsForJid(jid, FunXMPP.Connection.SyncMode.Query, FunXMPP.Connection.SyncContext.Notification);
    }

    private static void SendForDetailsForJid(
      string jid,
      FunXMPP.Connection.SyncMode mode,
      FunXMPP.Connection.SyncContext context)
    {
      try
      {
        if (AppState.GetConnection() == null)
          return;
        DateTime.UtcNow.ToFileTimeUtc().ToString();
        AppState.Client clientInstance = AppState.ClientInstance;
        FunXMPP.Connection conn = clientInstance?.GetConnection();
        if (clientInstance == null || conn == null)
          return;
        Action a = (Action) (() => UsyncQueryRequest.SendUsyncUserQuery(conn, jid, mode, context));
        conn.InvokeWhenConnected(a);
      }
      catch (Exception ex)
      {
        string context1 = "Exception getting detail information for jid: " + jid;
        Log.SendCrashLog(ex, context1, logOnlyForRelease: true);
      }
    }

    public static void SendGetBusinesses(
      IEnumerable<FunXMPP.Connection.BusinessRequest> jids,
      FunXMPP.Connection.SyncMode mode,
      FunXMPP.Connection.SyncContext context,
      Action onComplete = null,
      Action<string, int> onError = null)
    {
      UsyncQuery query = new UsyncQuery(mode, context);
      FunXMPP.Connection connection = AppState.GetConnection();
      connection.AddUsyncGetBusinesses(query, jids, (IEnumerable<FunXMPP.Connection.BusinessRequest>) new FunXMPP.Connection.BusinessRequest[0], onComplete, onError);
      UsyncQueryRequest.Send(query, connection);
    }

    public static void SendGetRemoteCapabilities(
      IEnumerable<string> jids,
      Action<RemoteClientCaps> onRecord = null,
      Action onComplete = null,
      Action<int> onError = null)
    {
      UsyncQuery query = new UsyncQuery(FunXMPP.Connection.SyncMode.Query, FunXMPP.Connection.SyncContext.Interactive);
      FunXMPP.Connection connection = AppState.GetConnection();
      connection.AddUsyncGetRemoteCapabilities(query, jids, onRecord, onComplete, onError);
      UsyncQueryRequest.Send(query, connection);
    }

    public static void SendGetPhotoIds(
      IEnumerable<string> jids,
      Action onComplete = null,
      Action<int> onError = null)
    {
      UsyncQueryRequest.SendUsyncGetPhotoIds(jids, onComplete, onError);
    }

    private static void SendUsyncGetPhotoIds(
      IEnumerable<string> jids,
      Action onComplete = null,
      Action<int> onError = null)
    {
      UsyncQuery query = new UsyncQuery(FunXMPP.Connection.SyncMode.Delta, FunXMPP.Connection.SyncContext.Notification);
      FunXMPP.Connection connection = AppState.GetConnection();
      UsyncQueryRequest.AddUsyncGetPhotoIds(connection, query, jids, (IEnumerable<string>) new string[0], onComplete, onError);
      UsyncQueryRequest.Send(query, connection);
    }

    public static void SendGetStatuses(
      IEnumerable<FunXMPP.Connection.StatusRequest> jids,
      FunXMPP.Connection.SyncMode mode,
      FunXMPP.Connection.SyncContext context,
      Action<string, DateTime?, string> onRecord = null,
      Action onComplete = null,
      Action<string, int> onError = null)
    {
      UsyncQuery query = new UsyncQuery(mode, context);
      FunXMPP.Connection connection = AppState.GetConnection();
      connection.AddUsyncGetStatuses(query, jids, (IEnumerable<FunXMPP.Connection.StatusRequest>) new FunXMPP.Connection.StatusRequest[0], onRecord, onComplete, onError);
      UsyncQueryRequest.Send(query, connection);
    }

    private static void SendUsyncUserQuery(
      FunXMPP.Connection conn,
      string jid,
      FunXMPP.Connection.SyncMode mode,
      FunXMPP.Connection.SyncContext context)
    {
      UserStatus userStatus = UserCache.Get(jid, false);
      if (userStatus == null)
        return;
      if (userStatus.IsInDeviceContactList || !userStatus.IsSidelistSynced)
        UsyncQueryRequest.SendUsyncListUserQuery(conn, jid, mode, context);
      else
        UsyncQueryRequest.SendUsyncSidelistUserQuery(jid, mode, context);
    }

    public static void SendUsyncListUserQuery(
      string jid,
      FunXMPP.Connection.SyncMode mode,
      FunXMPP.Connection.SyncContext context)
    {
      UsyncQueryRequest.SendUsyncListUserQuery(AppState.ClientInstance.GetConnection(), jid, mode, context);
    }

    public static void SendUsyncListUserQuery(
      FunXMPP.Connection conn,
      string jid,
      FunXMPP.Connection.SyncMode mode,
      FunXMPP.Connection.SyncContext context)
    {
      UsyncQuery query = new UsyncQuery(mode, context);
      UsyncQueryRequest.AddUsyncGetPhotoIds(conn, query, (IEnumerable<string>) new string[1]
      {
        jid
      }, (IEnumerable<string>) new string[0]);
      FunXMPP.Connection.StatusRequest statusRequest = new FunXMPP.Connection.StatusRequest()
      {
        Jid = jid
      };
      conn.AddUsyncGetStatuses(query, (IEnumerable<FunXMPP.Connection.StatusRequest>) new FunXMPP.Connection.StatusRequest[1]
      {
        statusRequest
      }, (IEnumerable<FunXMPP.Connection.StatusRequest>) new FunXMPP.Connection.StatusRequest[0]);
      FunXMPP.Connection.BusinessRequest businessRequest = new FunXMPP.Connection.BusinessRequest()
      {
        Jid = jid
      };
      conn.AddUsyncGetBusinesses(query, (IEnumerable<FunXMPP.Connection.BusinessRequest>) new FunXMPP.Connection.BusinessRequest[1]
      {
        businessRequest
      }, (IEnumerable<FunXMPP.Connection.BusinessRequest>) new FunXMPP.Connection.BusinessRequest[0]);
      UsyncQueryRequest.Send(query, conn);
    }

    public static void SendUsyncSidelistUserQuery(
      string jid,
      FunXMPP.Connection.SyncMode mode,
      FunXMPP.Connection.SyncContext context)
    {
      if (!Settings.UsyncSidelist)
        return;
      UsyncQuery query = new UsyncQuery(mode, context);
      FunXMPP.Connection connection = AppState.GetConnection();
      UsyncQueryRequest.AddUsyncGetPhotoIds(connection, query, (IEnumerable<string>) new string[0], (IEnumerable<string>) new string[1]
      {
        jid
      });
      FunXMPP.Connection.StatusRequest statusRequest = new FunXMPP.Connection.StatusRequest()
      {
        Jid = jid
      };
      connection.AddUsyncGetStatuses(query, (IEnumerable<FunXMPP.Connection.StatusRequest>) new FunXMPP.Connection.StatusRequest[0], (IEnumerable<FunXMPP.Connection.StatusRequest>) new FunXMPP.Connection.StatusRequest[1]
      {
        statusRequest
      });
      FunXMPP.Connection.BusinessRequest businessRequest = new FunXMPP.Connection.BusinessRequest()
      {
        Jid = jid
      };
      connection.AddUsyncGetBusinesses(query, (IEnumerable<FunXMPP.Connection.BusinessRequest>) new FunXMPP.Connection.BusinessRequest[0], (IEnumerable<FunXMPP.Connection.BusinessRequest>) new FunXMPP.Connection.BusinessRequest[1]
      {
        businessRequest
      });
      UsyncQueryRequest.Send(query, connection);
    }

    public static bool Send(
      UsyncQuery query,
      FunXMPP.Connection conn,
      Action onIQReceive = null,
      Action onComplete = null,
      Action<int> onError = null)
    {
      return conn.SendUsyncQuery(query, onIQReceive, onComplete, onError);
    }

    public static void AddUsyncGetPhotoIds(
      FunXMPP.Connection conn,
      UsyncQuery query,
      Action onComplete = null,
      Action<int> onError = null)
    {
      UsyncQueryRequest.AddUsyncGetPhotoIds(conn, query, (IEnumerable<string>) new string[0], (IEnumerable<string>) new string[0], onComplete, onError);
    }

    private static void AddUsyncGetPhotoIds(
      FunXMPP.Connection conn,
      UsyncQuery query,
      IEnumerable<string> jids,
      IEnumerable<string> sidelistJids,
      Action onComplete = null,
      Action<int> onError = null)
    {
      query.OnGetPhotoIds = (Action<IEnumerable<UsyncQuery.PhotoIdResult>, IEnumerable<UsyncQuery.PhotoIdResult>>) ((listPhotoIds, sidelistPhotoIds) =>
      {
        if (listPhotoIds != null)
        {
          foreach (UsyncQuery.PhotoIdResult listPhotoId in listPhotoIds)
          {
            if (listPhotoId.jid != null)
              conn.EventHandler.OnNewPhotoIdFetched(listPhotoId.jid, (string) null, listPhotoId.id, true, "get_usync_photo_id_");
          }
        }
        if (sidelistPhotoIds != null)
        {
          foreach (UsyncQuery.PhotoIdResult sidelistPhotoId in sidelistPhotoIds)
          {
            if (sidelistPhotoId.jid != null)
              conn.EventHandler.OnNewPhotoIdFetched(sidelistPhotoId.jid, (string) null, sidelistPhotoId.id, true, "get_usync_sidelist_photo_id_");
          }
        }
        Action action = onComplete;
        if (action == null)
          return;
        action();
      });
      query.OnGetPhotoIdsError = onError;
      query.RequestPictureIds(jids, sidelistJids);
    }
  }
}
