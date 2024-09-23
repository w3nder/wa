// Decompiled with JetBrains decompiler
// Type: WhatsApp.ConversionRecordHelper
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;


namespace WhatsApp
{
  public static class ConversionRecordHelper
  {
    private static List<string> jidsWithConversionRecords = new List<string>();
    private static bool jidsWithConversionRecordsInitialized = false;

    private static bool HasConversionRecord(string jid)
    {
      if (!ConversionRecordHelper.jidsWithConversionRecordsInitialized)
      {
        try
        {
          ContactsContext.Instance((Action<ContactsContext>) (cdb =>
          {
            if (ConversionRecordHelper.jidsWithConversionRecordsInitialized)
              return;
            ConversionRecordHelper.jidsWithConversionRecordsInitialized = true;
            ConversionRecordHelper.jidsWithConversionRecords = cdb.GetConversionRecordJids();
            Log.d("crec", "loaded optimization table: {0}", (object) ConversionRecordHelper.jidsWithConversionRecords.Count.ToString());
          }));
        }
        catch (Exception ex)
        {
          Log.SendCrashLog(ex, "Crec optimized cache list initialization");
          ConversionRecordHelper.jidsWithConversionRecordsInitialized = true;
        }
      }
      return ConversionRecordHelper.jidsWithConversionRecords.Contains(jid);
    }

    public static void MaybeUpdateConversionRecord(string jid, DateTime? updateTimeStamp)
    {
      if (!string.IsNullOrEmpty(jid) || !updateTimeStamp.HasValue || !JidHelper.IsUserJid(jid))
        return;
      ConversionRecordHelper.MaybeUpdateConversionRecord(jid, updateTimeStamp.Value);
    }

    public static ConversionRecord MaybeUpdateConversionRecord(string jid, DateTime lastActivity)
    {
      if (string.IsNullOrEmpty(jid) || !JidHelper.IsUserJid(jid))
        return (ConversionRecord) null;
      if (!ConversionRecordHelper.HasConversionRecord(jid))
        return (ConversionRecord) null;
      ConversionRecord r = (ConversionRecord) null;
      try
      {
        ContactsContext.Instance((Action<ContactsContext>) (cdb =>
        {
          r = cdb.HasUpdatedConversionRecord(jid, lastActivity);
          if (r != null)
            return;
          ConversionRecordHelper.jidsWithConversionRecords.Remove(jid);
        }));
      }
      catch (Exception ex)
      {
        r = (ConversionRecord) null;
        Log.l(ex, "Exception updating crec timestamp");
      }
      if (r == null)
      {
        Log.l("crec", "Unexpectedly null conversion record for {0}", (object) jid);
        try
        {
          ContactsContext.Instance((Action<ContactsContext>) (cdb =>
          {
            cdb.DeleteConversionRecordForJid(jid);
            ConversionRecordHelper.jidsWithConversionRecords.Remove(jid);
          }));
        }
        catch (Exception ex)
        {
          Log.SendCrashLog(ex, "Conversion Record update failure");
        }
      }
      return r;
    }

    public static void CreateConversionRecord(
      string jid,
      DateTime createTime,
      string phoneNumber,
      string source,
      byte[] data)
    {
      if (string.IsNullOrEmpty(jid))
        return;
      if (!JidHelper.IsUserJid(jid))
        return;
      try
      {
        ContactsContext.Instance((Action<ContactsContext>) (cdb =>
        {
          if (!string.IsNullOrEmpty(source) || data != null && data.Length != 0)
          {
            cdb.AddOrReplaceConversionRecord(jid, createTime, createTime, phoneNumber, source, data);
            ConversionRecordHelper.jidsWithConversionRecords.Add(jid);
          }
          else
          {
            cdb.DeleteConversionRecordForJid(jid);
            ConversionRecordHelper.jidsWithConversionRecords.Remove(jid);
          }
        }));
      }
      catch (Exception ex)
      {
        Log.l(ex, "Exception updating crec");
      }
    }

    public static void ClearConversionRecords()
    {
      Log.l("crec", "Clear crecs");
      try
      {
        ContactsContext.Instance((Action<ContactsContext>) (cdb =>
        {
          cdb.ClearConversionRecordTables();
          ConversionRecordHelper.jidsWithConversionRecords.Clear();
        }));
      }
      catch (Exception ex)
      {
        Log.l(ex, "Exception clearing crecs");
      }
    }
  }
}
