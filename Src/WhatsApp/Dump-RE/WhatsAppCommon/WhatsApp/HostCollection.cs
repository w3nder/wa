// Decompiled with JetBrains decompiler
// Type: WhatsApp.HostCollection
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable disable
namespace WhatsApp
{
  public class HostCollection
  {
    private Dictionary<string, HostCollection.Entry> hostsByName = new Dictionary<string, HostCollection.Entry>();
    private static HostCollection instance = (HostCollection) null;
    private static object @lock = new object();

    private bool TryParse<T>(string str, bool ignoreCase, out T @enum) where T : struct
    {
      return Enum.TryParse<T>(str, ignoreCase, out @enum);
    }

    private HostCollection(Stream str)
    {
      using (StreamReader streamReader = new StreamReader(str))
      {
        string str1;
        while ((str1 = streamReader.ReadLine()) != null)
        {
          string[] source = str1.Split(',');
          HostCollection.EntryType @enum;
          if (source.Length >= 2 && this.TryParse<HostCollection.EntryType>(source[0], true, out @enum))
          {
            HostCollection.Entry entry = new HostCollection.Entry()
            {
              Type = @enum,
              Name = source[1],
              Addresses = (IEnumerable<string>) ((IEnumerable<string>) source).Skip<string>(2).ToArray<string>()
            };
            this.hostsByName[entry.Name] = entry;
          }
        }
      }
    }

    public HostCollection.Entry GetHostByName(string name)
    {
      HostCollection.Entry hostByName = (HostCollection.Entry) null;
      this.hostsByName.TryGetValue(name, out hostByName);
      return hostByName;
    }

    public IEnumerable<HostCollection.Entry> GetHostsByType(HostCollection.EntryType type)
    {
      return this.hostsByName.Values.Where<HostCollection.Entry>((Func<HostCollection.Entry, bool>) (v => v.Type == type));
    }

    public static HostCollection Instance
    {
      get
      {
        return Utils.LazyInit<HostCollection>(ref HostCollection.instance, (Func<HostCollection>) (() =>
        {
          using (Stream str = AppState.OpenFromXAP("hosts"))
            return new HostCollection(str);
        }), HostCollection.@lock);
      }
    }

    public enum EntryType
    {
      Chat,
      Reg,
      FBChat,
    }

    public class Entry
    {
      public HostCollection.EntryType Type;
      public string Name;
      public IEnumerable<string> Addresses;
    }
  }
}
