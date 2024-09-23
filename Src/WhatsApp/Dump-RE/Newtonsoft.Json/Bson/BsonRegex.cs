// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Bson.BsonRegex
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

#nullable disable
namespace Newtonsoft.Json.Bson
{
  internal class BsonRegex : BsonToken
  {
    public BsonString Pattern { get; set; }

    public BsonString Options { get; set; }

    public BsonRegex(string pattern, string options)
    {
      this.Pattern = new BsonString((object) pattern, false);
      this.Options = new BsonString((object) options, false);
    }

    public override BsonType Type => BsonType.Regex;
  }
}
