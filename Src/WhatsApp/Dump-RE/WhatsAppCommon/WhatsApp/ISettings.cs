// Decompiled with JetBrains decompiler
// Type: WhatsApp.ISettings
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;

#nullable disable
namespace WhatsApp
{
  public interface ISettings
  {
    void Reset();

    void Set<T>(Settings.Key Key, T obj, bool bypassCache = false);

    void Delete(Settings.Key Key);

    void InPlaceSet<T>(Settings.Key Key, T obj);

    void Save();

    T Get<T>(Settings.Key k, T defaultVal);

    bool TryGet<T>(Settings.Key k, out T v);

    void Export(Action<Settings.Key, object> onItem);
  }
}
