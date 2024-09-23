// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Bson.BsonValue
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

#nullable disable
namespace Newtonsoft.Json.Bson
{
  internal class BsonValue : BsonToken
  {
    private readonly object _value;
    private readonly BsonType _type;

    public BsonValue(object value, BsonType type)
    {
      this._value = value;
      this._type = type;
    }

    public object Value => this._value;

    public override BsonType Type => this._type;
  }
}
