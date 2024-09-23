// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Bson.BsonBinaryType
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;

#nullable disable
namespace Newtonsoft.Json.Bson
{
  internal enum BsonBinaryType : byte
  {
    Binary = 0,
    Function = 1,
    [Obsolete("This type has been deprecated in the BSON specification. Use Binary instead.")] BinaryOld = 2,
    [Obsolete("This type has been deprecated in the BSON specification. Use Uuid instead.")] UuidOld = 3,
    Uuid = 4,
    Md5 = 5,
    UserDefined = 128, // 0x80
  }
}
