// Decompiled with JetBrains decompiler
// Type: WhatsApp.SettingsStorage
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;


namespace WhatsApp
{
  public class SettingsStorage : ISettings
  {
    private static Settings.Key[] _nvKeys = new Settings.Key[7]
    {
      Settings.Key.ChatID,
      Settings.Key.EULAAcceptedUtc,
      Settings.Key.PhoneNumberVerificationState,
      Settings.Key.RecoveryTokenSet,
      Settings.Key.RegV2Password,
      Settings.Key.ClientStaticPrivateKey,
      Settings.Key.ClientStaticPublicKey
    };
    private static Dictionary<Settings.Key, bool> NonvolatileKeys = SettingsStorage.GenerateDict((IEnumerable<Settings.Key>) SettingsStorage._nvKeys);
    private static Settings.Key[] _backedUpKeys = new Settings.Key[2]
    {
      Settings.Key.LastSeenMissedCallTimeUtc,
      Settings.Key.BizChat2TierOneTimeSysMsgAdded
    };
    private static Dictionary<Settings.Key, bool> BackedUpKeys = SettingsStorage.GenerateDict((IEnumerable<Settings.Key>) SettingsStorage._backedUpKeys);
    private SettingsStorage.Storage Volatile;
    private SettingsStorage.Storage Nonvolatile;
    private SettingsStorage.Storage BackedUp;

    public SettingsStorage()
    {
      this.Volatile = new SettingsStorage.Storage()
      {
        Settings = (ISettings) new SqliteSettings("settingsv2\\volatile")
      };
      this.Nonvolatile = new SettingsStorage.Storage()
      {
        Settings = (ISettings) new SqliteSettings("settingsv2\\nv", new SqliteSynchronizeOptions?(SqliteSynchronizeOptions.Full))
      };
      this.BackedUp = new SettingsStorage.Storage()
      {
        Settings = (ISettings) new SqliteSettings((string) null)
      };
      if (!BinarySettings.Exists())
        return;
      BinarySettings.CreateInstance().Export((Action<Settings.Key, object>) ((key, value) => this.Set<object>(key, value, false)));
      this.Save();
      BinarySettings.Remove();
    }

    private ISettings StorageForKey(Settings.Key key, bool write = false)
    {
      SettingsStorage.Storage storage = !SettingsStorage.NonvolatileKeys.ContainsKey(key) ? (!SettingsStorage.BackedUpKeys.ContainsKey(key) ? this.Volatile : this.BackedUp) : this.Nonvolatile;
      if (write)
        storage.Dirty = true;
      return storage.Settings;
    }

    private void Foreach(Action<SettingsStorage.Storage> onStorage)
    {
      onStorage(this.Nonvolatile);
      onStorage(this.Volatile);
      onStorage(this.BackedUp);
    }

    public void Reset() => this.Foreach((Action<SettingsStorage.Storage>) (s => s.Reset()));

    public void Set<T>(Settings.Key Key, T obj, bool bypassCache = false)
    {
      this.StorageForKey(Key, true).Set<T>(Key, obj, bypassCache);
    }

    public void Delete(Settings.Key Key) => this.StorageForKey(Key, true).Delete(Key);

    public void InPlaceSet<T>(Settings.Key Key, T obj)
    {
      this.StorageForKey(Key).InPlaceSet<T>(Key, obj);
    }

    public void Save() => this.Foreach((Action<SettingsStorage.Storage>) (s => s.Save()));

    public bool TryGet<T>(Settings.Key k, out T v) => this.StorageForKey(k).TryGet<T>(k, out v);

    public T Get<T>(Settings.Key k, T defaultVal) => this.StorageForKey(k).Get<T>(k, defaultVal);

    public void Export(Action<Settings.Key, object> onItem)
    {
      this.Foreach((Action<SettingsStorage.Storage>) (s => s.Settings.Export(onItem)));
    }

    private static Dictionary<Settings.Key, bool> GenerateDict(IEnumerable<Settings.Key> keys)
    {
      Dictionary<Settings.Key, bool> dict = new Dictionary<Settings.Key, bool>();
      foreach (Settings.Key key in keys)
        dict[key] = true;
      return dict;
    }

    private class Storage
    {
      public ISettings Settings;
      public bool Dirty;

      public void Reset()
      {
        this.Settings.Reset();
        this.Dirty = true;
      }

      public void Save()
      {
        if (!this.Dirty)
          return;
        this.Settings.Save();
        this.Dirty = false;
      }
    }
  }
}
