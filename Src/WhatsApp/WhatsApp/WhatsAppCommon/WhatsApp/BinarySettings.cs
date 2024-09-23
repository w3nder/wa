// Decompiled with JetBrains decompiler
// Type: WhatsApp.BinarySettings
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;


namespace WhatsApp
{
  public class BinarySettings : ISettings
  {
    private static string[] LegacyKeys = new string[16]
    {
      "BBFR",
      "ChatID",
      "CountryCode",
      "EULAAccepted",
      "FirstRegistrationUIShown",
      "LastFullSync",
      "LastGroupsUpdate",
      "MediaAutoSaveEnabled",
      "MyJid",
      "NextChallenge",
      "PhoneNumber",
      "PhoneNumberVerificationRetry",
      "PhoneNumberVerificationState",
      "PhoneNumberVerificationTimeout",
      "PushEnabled",
      "PushName"
    };
    public static BinarySettings Instance = (BinarySettings) null;
    private Dictionary<Settings.Key, object> items;
    private Dictionary<Settings.Key, object> dirtyItems;
    private Dictionary<Settings.Key, BinarySettings.Extent> extents;
    private byte[] buffer;
    private int buffer_length;
    private const string settingsFile = "settings";
    private const string backupFile = "settings-backup";

    private static Settings.Key KeyFromLegacyString(string str)
    {
      return (Settings.Key) BinarySettings.LegacyKeys.BinarySearch<string>(str);
    }

    public static BinarySettings CreateInstance()
    {
      if (BinarySettings.Instance == null)
      {
        BinarySettings.Instance = new BinarySettings();
        BinarySettings.Instance.Load();
      }
      return BinarySettings.Instance;
    }

    public static void ResetInstance() => BinarySettings.Instance = (BinarySettings) null;

    public void Reset()
    {
      if (this.items != null)
        this.items.Clear();
      if (this.dirtyItems != null)
        this.dirtyItems.Clear();
      if (this.extents != null)
        this.extents.Clear();
      this.Set<bool>(Settings.Key.FirstRegistrationUIShown, false, false);
    }

    public bool Contains(Settings.Key Key)
    {
      if (this.dirtyItems != null && this.dirtyItems.ContainsKey(Key))
        return true;
      return this.items != null && this.items.ContainsKey(Key);
    }

    private bool ContainsKeyValue(Settings.Key Key, object value)
    {
      object obj = this.Get<object>(Key, (object) null);
      return obj != null && obj.Equals(value);
    }

    public void Set<T>(Settings.Key Key, T obj, bool bypassCache = false)
    {
      if (this.dirtyItems == null)
        this.dirtyItems = new Dictionary<Settings.Key, object>();
      if (!bypassCache && this.ContainsKeyValue(Key, (object) obj))
        return;
      this.dirtyItems[Key] = (object) obj;
    }

    public void Delete(Settings.Key Key)
    {
      if (this.dirtyItems != null)
        this.dirtyItems.Remove(Key);
      if (this.items == null)
        return;
      this.items.Remove(Key);
    }

    public void InPlaceSet<T>(Settings.Key Key, T obj)
    {
      byte[] numArray = (byte[]) null;
      BinarySettings.Extent extent = new BinarySettings.Extent();
      int num1 = 0;
      int num2 = 0;
      if (this.ContainsKeyValue(Key, (object) obj))
      {
        if (this.dirtyItems == null || !this.dirtyItems.ContainsKey(Key))
          return;
        this.Save();
      }
      else
      {
        if (this.extents != null && this.extents.TryGetValue(Key, out extent))
        {
          using (MemoryStream output = new MemoryStream())
          {
            using (BinaryWriter writer = new BinaryWriter((Stream) output, Encoding.BigEndianUnicode))
            {
              this.Serialize(writer, (object) Key);
              num1 = (int) output.Position;
              this.Serialize(writer, (object) obj);
              num2 = (int) output.Position;
              if (num2 == extent.len)
                numArray = output.GetBuffer();
            }
          }
        }
        if (numArray != null)
        {
          using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
          {
            using (IsolatedStorageFileStream storageFileStream = storeForApplication.OpenFile("settings", FileMode.Open, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete))
            {
              int num3 = extent.off + num1;
              int num4 = num2 - num1;
              Array.Copy((Array) numArray, num1, (Array) this.buffer, num3, num4);
              storageFileStream.Seek((long) num3, SeekOrigin.Begin);
              storageFileStream.Write(numArray, num1, num4);
            }
          }
          if (this.items == null)
            this.items = new Dictionary<Settings.Key, object>();
          this.items[Key] = (object) obj;
          if (this.dirtyItems == null)
            return;
          this.dirtyItems.Remove(Key);
        }
        else
        {
          this.Set<T>(Key, obj, false);
          this.Save();
        }
      }
    }

    public void Export(Action<Settings.Key, object> onItem)
    {
      Dictionary<Settings.Key, Settings.Key> deprecated = ((IEnumerable<Settings.Key>) Settings.DeprecatedKeys).ToDictionary<Settings.Key, Settings.Key>((Func<Settings.Key, Settings.Key>) (v => v));
      foreach (KeyValuePair<Settings.Key, object> keyValuePair in this.items.Where<KeyValuePair<Settings.Key, object>>((Func<KeyValuePair<Settings.Key, object>, bool>) (key => !deprecated.ContainsKey(key.Key))))
        onItem(keyValuePair.Key, keyValuePair.Value);
    }

    public static bool Exists()
    {
      using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
      {
        string[] strArray = new string[2]
        {
          "settings",
          "settings-backup"
        };
        foreach (string path in strArray)
        {
          if (storeForApplication.FileExists(path))
            return true;
        }
      }
      return false;
    }

    public static void Remove()
    {
      using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
      {
        string[] strArray = new string[2]
        {
          "settings",
          "settings-backup"
        };
        foreach (string file in strArray)
        {
          try
          {
            storeForApplication.DeleteFile(file);
          }
          catch (Exception ex)
          {
          }
        }
      }
    }

    public void Save()
    {
      string str = "settings-tmp";
      bool flag = false;
      if (this.dirtyItems == null || this.dirtyItems.Count == 0)
        return;
      using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
      {
        if (storeForApplication.FileExists(str))
          storeForApplication.DeleteFile(str);
        if (storeForApplication.FileExists("settings"))
        {
          storeForApplication.MoveFile("settings", "settings-backup");
          flag = true;
        }
        try
        {
          this.Save(storeForApplication, str);
          storeForApplication.MoveFile(str, "settings");
          if (!flag)
            return;
          storeForApplication.DeleteFile("settings-backup");
        }
        catch (Exception ex)
        {
          if (flag)
            storeForApplication.MoveFile("settings-backup", "settings");
          throw;
        }
      }
    }

    private void Load()
    {
      using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
      {
        if (storeForApplication.FileExists("settings"))
        {
          this.Load(storeForApplication, "settings");
          if (!storeForApplication.FileExists("settings-backup"))
            return;
          storeForApplication.DeleteFile("settings-backup");
        }
        else if (storeForApplication.FileExists("settings-backup"))
        {
          storeForApplication.MoveFile("settings-backup", "settings");
          this.Load(storeForApplication, "settings");
        }
        else
          this.MigrateFromLegacy(storeForApplication);
      }
    }

    private void MigrateFromLegacy(IsolatedStorageFile fs)
    {
      if (!fs.FileExists("__ApplicationSettings"))
        return;
      foreach (KeyValuePair<string, object> applicationSetting in (IEnumerable<KeyValuePair<string, object>>) IsolatedStorageSettings.ApplicationSettings)
        this.Set<object>(BinarySettings.KeyFromLegacyString(applicationSetting.Key), applicationSetting.Value, false);
      this.Save();
      fs.DeleteFile("__ApplicationSettings");
    }

    private static void Read(Stream stream, byte[] b, int off, int len)
    {
      int num;
      for (; len != 0; len -= num)
      {
        num = stream.Read(b, off, len);
        if (num <= 0)
          throw new EndOfStreamException("unexpected EOF reading settings");
        off += num;
      }
    }

    private void Load(IsolatedStorageFile fs, string filename)
    {
      using (IsolatedStorageFileStream storageFileStream = fs.OpenFile(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
      {
        int length = (int) storageFileStream.Length;
        this.buffer = new byte[length];
        this.buffer_length = length;
        BinarySettings.Read((Stream) storageFileStream, this.buffer, 0, length);
      }
      this.dirtyItems = (Dictionary<Settings.Key, object>) null;
      this.items = new Dictionary<Settings.Key, object>();
      this.extents = new Dictionary<Settings.Key, BinarySettings.Extent>();
      using (MemoryStream input = new MemoryStream(this.buffer, false))
      {
        using (BinaryReader reader = new BinaryReader((Stream) input, Encoding.BigEndianUnicode))
        {
          while (true)
          {
            try
            {
              BinarySettings.Extent extent;
              extent.off = (int) input.Position;
              object obj1 = this.Deserialize(reader);
              object obj2 = this.Deserialize(reader);
              extent.len = (int) input.Position - extent.off;
              Settings.Key key;
              if (obj1 is string)
              {
                key = BinarySettings.KeyFromLegacyString(obj1 as string);
                if (key != Settings.Key.Invalid)
                {
                  if (this.dirtyItems == null)
                    this.dirtyItems = new Dictionary<Settings.Key, object>();
                  this.dirtyItems.Add(key, obj2);
                }
                else
                  continue;
              }
              else
                key = (Settings.Key) (obj1 as int?).Value;
              this.items.Add(key, obj2);
              this.extents.Add(key, extent);
            }
            catch (EndOfStreamException ex)
            {
              break;
            }
          }
        }
      }
      if (this.dirtyItems == null || this.dirtyItems.Count == 0)
        return;
      this.Save();
    }

    public void Save(IsolatedStorageFile fs, string filename)
    {
      byte[] buffer = (byte[]) null;
      int count = 0;
      using (IsolatedStorageFileStream storageFileStream = fs.OpenFile(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete))
      {
        Dictionary<Settings.Key, BinarySettings.Extent> dictionary = new Dictionary<Settings.Key, BinarySettings.Extent>();
        if (this.items == null)
          this.items = new Dictionary<Settings.Key, object>();
        using (MemoryStream output = new MemoryStream())
        {
          using (BinaryWriter writer = new BinaryWriter((Stream) output, Encoding.BigEndianUnicode))
          {
            foreach (KeyValuePair<Settings.Key, object> dirtyItem in this.dirtyItems)
            {
              BinarySettings.Extent extent;
              extent.off = (int) output.Position;
              this.Serialize(writer, (object) dirtyItem.Key);
              this.Serialize(writer, dirtyItem.Value);
              extent.len = (int) output.Position - extent.off;
              dictionary.Add(dirtyItem.Key, extent);
            }
            if (this.items.Count != 0)
            {
              foreach (KeyValuePair<Settings.Key, object> keyValuePair in this.items)
              {
                if (!this.dirtyItems.ContainsKey(keyValuePair.Key))
                {
                  BinarySettings.Extent extent1 = this.extents[keyValuePair.Key];
                  BinarySettings.Extent extent2;
                  extent2.off = (int) output.Position;
                  extent2.len = extent1.len;
                  writer.Write(this.buffer, extent1.off, extent2.len);
                  dictionary.Add(keyValuePair.Key, extent2);
                }
              }
            }
            buffer = output.GetBuffer();
            count = (int) output.Position;
          }
        }
        storageFileStream.Write(buffer, 0, count);
        foreach (KeyValuePair<Settings.Key, object> dirtyItem in this.dirtyItems)
          this.items[dirtyItem.Key] = dirtyItem.Value;
        this.dirtyItems.Clear();
        this.extents = dictionary;
        this.buffer = buffer;
        this.buffer_length = count;
      }
    }

    private object Deserialize(BinaryReader reader)
    {
      int num = reader.ReadInt32();
      switch (num)
      {
        case 0:
          throw new EndOfStreamException();
        case 1:
          int count = (int) reader.ReadInt16();
          return (object) new string(reader.ReadChars(count));
        case 2:
          return (object) (reader.ReadByte() > (byte) 0);
        case 3:
          return (object) DateTime.FromFileTimeUtc(reader.ReadInt64());
        case 4:
          return (object) reader.ReadInt32();
        case 5:
          byte[] buffer = new byte[reader.ReadInt32()];
          reader.Read(buffer, 0, buffer.Length);
          return (object) buffer;
        case 6:
          return (object) reader.ReadDouble();
        default:
          throw new ArgumentException("unknown serialization type " + (object) num);
      }
    }

    private void Serialize(BinaryWriter writer, object o)
    {
      switch (o)
      {
        case Settings.Key? _:
          Settings.Key key = (o as Settings.Key?).Value;
          writer.Write(4);
          writer.Write((int) key);
          break;
        case string _:
          string str = o as string;
          writer.Write(1);
          writer.Write((short) str.Length);
          writer.Write(str.ToCharArray());
          break;
        case bool _:
          bool flag = (o as bool?).Value;
          writer.Write(2);
          writer.Write(flag ? (byte) 1 : (byte) 0);
          break;
        case DateTime? _:
          DateTime dateTime = (o as DateTime?).Value;
          writer.Write(3);
          writer.Write(dateTime.ToFileTimeUtc());
          break;
        case int? _:
          writer.Write(4);
          writer.Write((o as int?).Value);
          break;
        case PhoneNumberVerificationState? _:
          writer.Write(4);
          writer.Write((int) (o as PhoneNumberVerificationState?).Value);
          break;
        case byte[] _:
          byte[] buffer = (byte[]) o;
          writer.Write(5);
          writer.Write(buffer.Length);
          writer.Write(buffer, 0, buffer.Length);
          break;
        case double? nullable:
          writer.Write(6);
          writer.Write(nullable.Value);
          break;
        default:
          throw new ArgumentException("need to add new object type");
      }
    }

    public bool TryGet<T>(Settings.Key k, out T v)
    {
      object obj = (object) null;
      bool flag = false;
      if (this.dirtyItems != null && this.dirtyItems.TryGetValue(k, out obj))
      {
        v = (T) obj;
        flag = true;
      }
      else if (this.items != null && this.items.TryGetValue(k, out obj))
      {
        v = (T) obj;
        flag = true;
      }
      else
        v = default (T);
      return flag;
    }

    public T Get<T>(Settings.Key k, T defaultVal)
    {
      object obj = (object) null;
      if (this.dirtyItems != null && this.dirtyItems.TryGetValue(k, out obj))
        return (T) obj;
      return this.items != null && this.items.TryGetValue(k, out obj) ? (T) obj : defaultVal;
    }

    private struct Extent
    {
      public int off;
      public int len;
    }

    private enum Types
    {
      Eof,
      String,
      Bool,
      DateTime,
      Int32,
      ByteArray,
      Double,
    }
  }
}
