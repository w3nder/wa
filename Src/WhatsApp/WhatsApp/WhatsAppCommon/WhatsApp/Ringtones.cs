// Decompiled with JetBrains decompiler
// Type: WhatsApp.Ringtones
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;


namespace WhatsApp
{
  public static class Ringtones
  {
    public const string DefaultRingtonePath = "Sounds\\Ring01.wma";

    public static Ringtones.Tone[] LoadRingtones()
    {
      int num1 = 0;
      Ringtones.Tone[] toneArray = new Ringtones.Tone[4];
      string ringtonePure = AppResources.RingtonePure;
      int sortIndex1 = num1;
      int num2 = sortIndex1 + 1;
      toneArray[0] = new Ringtones.Tone("Sounds\\Ring01.wma", ringtonePure, sortIndex1);
      string ringtoneSilk = AppResources.RingtoneSilk;
      int sortIndex2 = num2;
      int num3 = sortIndex2 + 1;
      toneArray[1] = new Ringtones.Tone("Sounds\\Ring02.wma", ringtoneSilk, sortIndex2);
      string ringtoneSymmetry = AppResources.RingtoneSymmetry;
      int sortIndex3 = num3;
      int num4 = sortIndex3 + 1;
      toneArray[2] = new Ringtones.Tone("Sounds\\Ring03.wma", ringtoneSymmetry, sortIndex3);
      string ringtoneLattice = AppResources.RingtoneLattice;
      int sortIndex4 = num4;
      int num5 = sortIndex4 + 1;
      toneArray[3] = new Ringtones.Tone("Sounds\\Ring05.wma", ringtoneLattice, sortIndex4);
      return toneArray;
    }

    public static Dictionary<string, Ringtones.Tone> LoadRingtonesDict()
    {
      return ((IEnumerable<Ringtones.Tone>) Ringtones.LoadRingtones()).ToDictionary<Ringtones.Tone, string>((Func<Ringtones.Tone, string>) (t => t.Filepath));
    }

    public static Ringtones.Tone GetGlobalRingtone()
    {
      return Ringtones.GetRingtoneForPath(Ringtones.GetGlobalRingtonePath());
    }

    public static string GetGlobalRingtonePath() => Settings.VoipRingtone ?? "Sounds\\Ring01.wma";

    public static string GetRingtonePath(string jid)
    {
      if (jid == null)
        return Ringtones.GetGlobalRingtonePath();
      if (!JidHelper.IsUserJid(jid))
        return (string) null;
      string filepath = (string) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        JidInfo jidInfo = db.GetJidInfo(jid, CreateOptions.None);
        if (jidInfo == null)
          return;
        filepath = jidInfo.RingTone;
      }));
      if (filepath == null)
        filepath = Settings.VoipRingtone;
      return filepath ?? "Sounds\\Ring01.wma";
    }

    public static string GetGlobalRingtoneName()
    {
      return Ringtones.GetRingtoneNameForPath(Ringtones.GetGlobalRingtonePath());
    }

    public static string GetRingtoneName(string jid = null)
    {
      if (jid == null)
        return Ringtones.GetGlobalRingtoneName();
      return !JidHelper.IsUserJid(jid) ? (string) null : Ringtones.GetRingtoneNameForPath(Ringtones.GetRingtonePath(jid));
    }

    public static string GetRingtoneNameForPath(string tonePath)
    {
      return Ringtones.GetRingtoneForPath(tonePath)?.Name;
    }

    public static Ringtones.Tone GetRingtoneForPath(string tonePath)
    {
      if (string.IsNullOrEmpty(tonePath))
        return (Ringtones.Tone) null;
      Dictionary<string, Ringtones.Tone> dictionary = Ringtones.LoadRingtonesDict();
      Ringtones.Tone tone = (Ringtones.Tone) null;
      string key = tonePath;
      ref Ringtones.Tone local = ref tone;
      return !dictionary.TryGetValue(key, out local) ? (Ringtones.Tone) null : tone;
    }

    public class Tone
    {
      public string Filepath { get; private set; }

      public string Name { get; private set; }

      public int SortIndex { get; private set; }

      public string Grouping { get; private set; }

      public Tone(string filepath, string name, int sortIndex, string grouping = null)
      {
        this.Filepath = filepath;
        this.Name = name;
        this.SortIndex = sortIndex;
        this.Grouping = grouping ?? "";
      }
    }
  }
}
