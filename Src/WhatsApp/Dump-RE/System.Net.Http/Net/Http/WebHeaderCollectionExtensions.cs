// Decompiled with JetBrains decompiler
// Type: System.Net.Http.WebHeaderCollectionExtensions
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

#nullable disable
namespace System.Net.Http
{
  internal static class WebHeaderCollectionExtensions
  {
    public static string[] GetValues(this WebHeaderCollection collection, int index)
    {
      return new string[1]
      {
        collection[collection.AllKeys[index]]
      };
    }

    public static string GetKey(this WebHeaderCollection collection, int index)
    {
      return collection.AllKeys[index];
    }

    public static void Add(this WebHeaderCollection collection, string key, string value)
    {
      if (string.IsNullOrEmpty(collection[key]))
      {
        collection[key] = value;
      }
      else
      {
        WebHeaderCollection headerCollection;
        string name;
        (headerCollection = collection)[name = key] = headerCollection[name] + "," + value;
      }
    }
  }
}
