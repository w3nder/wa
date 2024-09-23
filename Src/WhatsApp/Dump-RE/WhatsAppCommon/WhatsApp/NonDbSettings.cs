// Decompiled with JetBrains decompiler
// Type: WhatsApp.NonDbSettings
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#nullable disable
namespace WhatsApp
{
  public class NonDbSettings
  {
    private static readonly string NON_DB_LOG_HEADER = "nonDbSet";
    private const long max_summary_count_value = 10000;
    private static Dictionary<NonDbSettings.FileId, byte[]> nonDbCache = new Dictionary<NonDbSettings.FileId, byte[]>();
    private static object lockNonDbSettings = new object();
    private static byte[] defaultBytesForGet = new byte[0];
    private static readonly string NON_DB_FILE_PATH = Constants.IsoStorePath + "\\nondb_settings";
    private static readonly string NON_DB_FILE_EXT = ".bin";
    private const int MIN_LENGTH = 12;

    public static string ChatDnsDomain
    {
      get => NonDbSettings.GetString(NonDbSettings.FileId.ChatDnsDomain, (string) null);
      set => NonDbSettings.SetString(NonDbSettings.FileId.ChatDnsDomain, value);
    }

    public static int TimeSpentRecordOption
    {
      get
      {
        long? nullable = NonDbSettings.GetLong(NonDbSettings.FileId.TimeSpentRecordOption, 0L);
        return nullable.HasValue ? (int) nullable.Value : 0;
      }
      set
      {
        NonDbSettings.SetLong(NonDbSettings.FileId.TimeSpentRecordOption, new long?((long) value));
      }
    }

    public static long TimeSpentCheckTime
    {
      get
      {
        long? nullable = NonDbSettings.GetLong(NonDbSettings.FileId.TimeSpentCheckTime, 0L);
        return nullable.HasValue ? nullable.Value : 0L;
      }
      set => NonDbSettings.SetLong(NonDbSettings.FileId.TimeSpentCheckTime, new long?(value));
    }

    public static int FieldStatsSendIntervalSecs
    {
      get
      {
        long? nullable = NonDbSettings.GetLong(NonDbSettings.FileId.FieldStatsSendIntervalSecs, 300L, true);
        return nullable.HasValue ? (int) nullable.Value : 0;
      }
      set
      {
        NonDbSettings.SetLong(NonDbSettings.FileId.FieldStatsSendIntervalSecs, new long?((long) value));
      }
    }

    public static byte[] TimeSpentSummaryData
    {
      get => NonDbSettings.GetBytes(NonDbSettings.FileId.TimeSpentSummaryData, (byte[]) null);
      set => NonDbSettings.SetBytes(NonDbSettings.FileId.TimeSpentSummaryData, value, false);
    }

    public static long IncrementSessionSummaryCount
    {
      get
      {
        long? nullable = NonDbSettings.GetLong(NonDbSettings.FileId.TimeSpentSummaryCounter, 0L);
        long sessionSummaryCount;
        if (!nullable.HasValue || nullable.Value < 0L)
        {
          sessionSummaryCount = 0L;
        }
        else
        {
          sessionSummaryCount = (nullable.Value + 1L) % 10000L;
          if (sessionSummaryCount == 0L)
            sessionSummaryCount = 1L;
        }
        NonDbSettings.SetLong(NonDbSettings.FileId.TimeSpentSummaryCounter, new long?(sessionSummaryCount));
        return sessionSummaryCount;
      }
      private set
      {
      }
    }

    public static string HostOverride
    {
      get => NonDbSettings.GetString(NonDbSettings.FileId.HostOverride, (string) null);
      set => NonDbSettings.SetString(NonDbSettings.FileId.HostOverride, value);
    }

    public static int LocalTileCount
    {
      get
      {
        long? nullable = NonDbSettings.GetLong(NonDbSettings.FileId.LocalTileCount, -1L, true);
        return nullable.HasValue ? (int) nullable.Value : -1;
      }
      set
      {
        if (value >= 0)
          NonDbSettings.SetLong(NonDbSettings.FileId.LocalTileCount, new long?((long) value));
        else
          Log.l(nameof (NonDbSettings), "Ignoring attempt to set tile count < 0: {0}", (object) value);
      }
    }

    public static string LastDataCenterUsed
    {
      get => NonDbSettings.GetString(NonDbSettings.FileId.LastDataCenterUsed, (string) null);
      set => NonDbSettings.SetString(NonDbSettings.FileId.LastDataCenterUsed, value);
    }

    public static string FallbackIp
    {
      get => NonDbSettings.GetString(NonDbSettings.FileId.FallbackIp, (string) null);
      set => NonDbSettings.SetString(NonDbSettings.FileId.FallbackIp, value);
    }

    public static long? NextSupportLogUnixTime
    {
      get => NonDbSettings.GetLong(NonDbSettings.FileId.NextSupportLogUnixTime, -1L);
      set => NonDbSettings.SetLong(NonDbSettings.FileId.NextSupportLogUnixTime, value);
    }

    public static int CreateSupportLogIntervalHours
    {
      get
      {
        long? nullable = NonDbSettings.GetLong(NonDbSettings.FileId.SendSupportLogIntervalHours, 0L);
        return nullable.HasValue ? (int) nullable.Value : 0;
      }
      set
      {
        NonDbSettings.SetLong(NonDbSettings.FileId.SendSupportLogIntervalHours, new long?((long) value));
      }
    }

    public static bool SendCallStateForVideoEnabled
    {
      get
      {
        bool? nullable = NonDbSettings.GetBool(NonDbSettings.FileId.SendCallStateForVideoEnabled, false, true);
        return nullable.HasValue && nullable.Value;
      }
      set
      {
        NonDbSettings.SetBool(NonDbSettings.FileId.SendCallStateForVideoEnabled, new bool?(value));
      }
    }

    public static int AddParticipantToCallPromptCount
    {
      get
      {
        long? nullable = NonDbSettings.GetLong(NonDbSettings.FileId.AddParticipantToCallPromptCount, 0L);
        return nullable.HasValue ? (int) nullable.Value : 0;
      }
      set
      {
        NonDbSettings.SetLong(NonDbSettings.FileId.AddParticipantToCallPromptCount, new long?((long) value));
      }
    }

    public static int KeepStackTrace
    {
      get
      {
        long? nullable = NonDbSettings.GetLong(NonDbSettings.FileId.KeepStackTrace, 0L);
        return nullable.HasValue ? (int) nullable.Value : 1;
      }
      set => NonDbSettings.SetLong(NonDbSettings.FileId.KeepStackTrace, new long?((long) value));
    }

    private static byte[] GetBytes(NonDbSettings.FileId id, byte[] defaultBytes, bool ignoreCache = false)
    {
      lock (NonDbSettings.lockNonDbSettings)
      {
        byte[] returnBytes = (byte[]) null;
        if (!ignoreCache)
        {
          if (NonDbSettings.nonDbCache.TryGetValue(id, out returnBytes))
            return returnBytes;
        }
        else
          NonDbSettings.nonDbCache.Remove(id);
        if (!NonDbSettings.GetBytes(id, out returnBytes))
          return defaultBytes;
        if (!ignoreCache)
          NonDbSettings.nonDbCache[id] = returnBytes;
        return returnBytes;
      }
    }

    private static bool SetBytes(NonDbSettings.FileId id, byte[] valueBytes, bool ignoreCache = false)
    {
      lock (NonDbSettings.lockNonDbSettings)
      {
        if (!ignoreCache)
        {
          byte[] first = (byte[]) null;
          if (NonDbSettings.nonDbCache.TryGetValue(id, out first))
          {
            bool flag = first == null && valueBytes == null;
            if (!flag && first != null && valueBytes != null)
              flag = ((IEnumerable<byte>) first).SequenceEqual<byte>((IEnumerable<byte>) valueBytes);
            if (flag)
              return true;
          }
          NonDbSettings.nonDbCache[id] = valueBytes;
        }
        else
          NonDbSettings.nonDbCache.Remove(id);
        return NonDbSettings.SetBytes(id, valueBytes);
      }
    }

    private static string GetString(
      NonDbSettings.FileId id,
      string defaultString,
      bool ignoreCache = false)
    {
      byte[] bytes = NonDbSettings.GetBytes(id, NonDbSettings.defaultBytesForGet, ignoreCache);
      if (bytes == NonDbSettings.defaultBytesForGet)
        return defaultString;
      if (bytes == null)
        return (string) null;
      return bytes.Length == 0 ? "" : Encoding.UTF8.GetString(bytes, 0, bytes.Length);
    }

    private static bool SetString(NonDbSettings.FileId id, string valueString, bool ignoreCache = false)
    {
      byte[] bytes = valueString == null ? (byte[]) null : Encoding.UTF8.GetBytes(valueString);
      return NonDbSettings.SetBytes(id, bytes, ignoreCache);
    }

    private static long? GetLong(NonDbSettings.FileId id, long defaultLong, bool ignoreCache = false)
    {
      byte[] sourceArray = NonDbSettings.GetBytes(id, NonDbSettings.defaultBytesForGet, ignoreCache);
      if (sourceArray == NonDbSettings.defaultBytesForGet)
        return new long?(defaultLong);
      if (sourceArray == null)
        return new long?();
      if (sourceArray.Length == 0)
        return new long?();
      if (!BitConverter.IsLittleEndian)
      {
        byte[] destinationArray = new byte[sourceArray.Length];
        Array.Copy((Array) sourceArray, (Array) destinationArray, sourceArray.Length);
        Array.Reverse((Array) destinationArray);
        sourceArray = destinationArray;
      }
      return new long?(BitConverter.ToInt64(sourceArray, 0));
    }

    private static bool SetLong(NonDbSettings.FileId id, long? valueLong, bool ignoreCache = false)
    {
      byte[] valueBytes = (byte[]) null;
      if (valueLong.HasValue)
      {
        valueBytes = BitConverter.GetBytes(valueLong.Value);
        if (!BitConverter.IsLittleEndian)
          Array.Reverse((Array) valueBytes);
      }
      return NonDbSettings.SetBytes(id, valueBytes, ignoreCache);
    }

    private static bool? GetBool(NonDbSettings.FileId id, bool defaultBool, bool ignoreCache = false)
    {
      byte[] sourceArray = NonDbSettings.GetBytes(id, NonDbSettings.defaultBytesForGet, ignoreCache);
      if (sourceArray == NonDbSettings.defaultBytesForGet)
        return new bool?(defaultBool);
      if (sourceArray == null)
        return new bool?();
      if (sourceArray.Length == 0)
        return new bool?();
      if (!BitConverter.IsLittleEndian)
      {
        byte[] destinationArray = new byte[sourceArray.Length];
        Array.Copy((Array) sourceArray, (Array) destinationArray, sourceArray.Length);
        Array.Reverse((Array) destinationArray);
        sourceArray = destinationArray;
      }
      return new bool?(BitConverter.ToBoolean(sourceArray, 0));
    }

    private static bool SetBool(NonDbSettings.FileId id, bool? valueBool, bool ignoreCache = false)
    {
      byte[] valueBytes = (byte[]) null;
      if (valueBool.HasValue)
      {
        valueBytes = BitConverter.GetBytes(valueBool.Value);
        if (!BitConverter.IsLittleEndian)
          Array.Reverse((Array) valueBytes);
      }
      return NonDbSettings.SetBytes(id, valueBytes, ignoreCache);
    }

    private static bool GetBytes(NonDbSettings.FileId id, out byte[] returnBytes)
    {
      Log.d(NonDbSettings.NON_DB_LOG_HEADER, "Get {0}", (object) id);
      bool bytes = false;
      string filePath = NonDbSettings.CreateFilePath(id);
      returnBytes = (byte[]) null;
      try
      {
        using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
        {
          if (nativeMediaStorage.FileExists(filePath))
          {
            using (Stream stream = nativeMediaStorage.OpenFile(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
              int length = (int) stream.Length;
              byte[] numArray = new byte[length];
              int num1 = stream.Read(numArray, 0, length);
              if (num1 != length || num1 < 12)
              {
                Log.l(NonDbSettings.NON_DB_LOG_HEADER, "Unexpected length reading settings file {0} {1}", (object) num1, (object) length);
                Log.d(NonDbSettings.NON_DB_LOG_HEADER, "Unexpected length", numArray);
                Log.SendCrashLog(new Exception("Unexpected length"), "NonDbSettings file", false, logOnlyForRelease: true);
              }
              else
              {
                int num2 = NonDbSettings.IntFromByteArray(numArray, 0);
                int val1 = NonDbSettings.IntFromByteArray(numArray, 4);
                int num3 = NonDbSettings.IntFromByteArray(numArray, numArray.Length - 4);
                if (num2 != num3 || Math.Max(val1, 0) + 12 != num1 || val1 < -1)
                {
                  Log.l(NonDbSettings.NON_DB_LOG_HEADER, "Unexpected data reading settings file {0} {1} {2} {3}", (object) num2, (object) val1, (object) num3, (object) numArray.Length);
                  Log.d(NonDbSettings.NON_DB_LOG_HEADER, "Unexpected data", numArray);
                  Log.SendCrashLog(new Exception("Unexpected data"), "NonDbSettings file", false, logOnlyForRelease: true);
                }
                else
                {
                  if (val1 == -1)
                  {
                    returnBytes = (byte[]) null;
                  }
                  else
                  {
                    returnBytes = new byte[val1];
                    if (val1 > 0)
                      Array.Copy((Array) numArray, 8, (Array) returnBytes, 0, returnBytes.Length);
                  }
                  bytes = true;
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Log.l(NonDbSettings.NON_DB_LOG_HEADER, "Exception getting data {0} {1}", (object) id, (object) filePath);
        Log.LogException(ex, NonDbSettings.NON_DB_LOG_HEADER + " - Exception get settings");
      }
      return bytes;
    }

    private static bool SetBytes(NonDbSettings.FileId id, byte[] valueBytes)
    {
      Log.d(NonDbSettings.NON_DB_LOG_HEADER, "Set {0} {1}", (object) id, valueBytes != null ? (object) valueBytes.Length.ToString() : (object) "null");
      bool flag = false;
      string filePath = NonDbSettings.CreateFilePath(id);
      try
      {
        int length;
        byte[] numArray;
        if (valueBytes == null)
        {
          length = -1;
          numArray = new byte[12];
        }
        else
        {
          length = valueBytes.Length;
          numArray = new byte[12 + length];
          Array.Copy((Array) valueBytes, 0, (Array) numArray, 8, length);
        }
        int ticks = (int) DateTime.Now.Ticks;
        NonDbSettings.IntIntoByteArray(ticks, numArray, 0);
        NonDbSettings.IntIntoByteArray(length, numArray, 4);
        NonDbSettings.IntIntoByteArray(ticks, numArray, numArray.Length - 4);
        using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
        {
          using (Stream stream = nativeMediaStorage.OpenFile(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
          {
            stream.Write(numArray, 0, numArray.Length);
            stream.SetLength((long) numArray.Length);
            stream.Flush();
          }
          flag = true;
        }
      }
      catch (Exception ex)
      {
        Log.l(NonDbSettings.NON_DB_LOG_HEADER, "Exception setting data {0} {1}", (object) id, (object) filePath);
        Log.LogException(ex, NonDbSettings.NON_DB_LOG_HEADER + " - Exception set settings");
      }
      return flag;
    }

    private static string CreateFilePath(NonDbSettings.FileId id)
    {
      return NonDbSettings.NON_DB_FILE_PATH + ((int) id).ToString() + NonDbSettings.NON_DB_FILE_EXT;
    }

    private static void IntIntoByteArray(int value, byte[] dest, int start)
    {
      dest[start + 3] = (byte) value;
      dest[start + 2] = (byte) (value >> 8);
      dest[start + 1] = (byte) (value >> 16);
      dest[start + 0] = (byte) (value >> 24);
    }

    private static int IntFromByteArray(byte[] src, int start)
    {
      return 0 | ((int) src[start + 0] & (int) byte.MaxValue) << 24 | ((int) src[start + 1] & (int) byte.MaxValue) << 16 | ((int) src[start + 2] & (int) byte.MaxValue) << 8 | (int) src[start + 3] & (int) byte.MaxValue;
    }

    private enum FileId
    {
      ChatDnsDomain,
      TimeSpentRecordOption,
      FieldStatsSendIntervalSecs,
      FieldStatsSendBufferCount,
      FieldStatsSendBufferCountBG,
      TimeSpentCheckTime,
      TimeSpentSummaryData,
      TimeSpentSummaryCounter,
      HostOverride,
      LocalTileCount,
      LastDataCenterUsed,
      FallbackIp,
      NextSupportLogUnixTime,
      SendSupportLogIntervalHours,
      SendCallStateForVideoEnabled,
      AddParticipantToCallPromptCount,
      FieldStatsBeaconChance,
      KeepStackTrace,
    }
  }
}
