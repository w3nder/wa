// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Bson.BsonBinary
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

#nullable disable
namespace Newtonsoft.Json.Bson
{
  internal class BsonBinary : BsonValue
  {
    public BsonBinaryType BinaryType { get; set; }

    public BsonBinary(byte[] value, BsonBinaryType binaryType)
      : base((object) value, BsonType.Binary)
    {
      this.BinaryType = binaryType;
    }
  }
}
