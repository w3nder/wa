// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.IWrappedDictionary
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System.Collections;

#nullable disable
namespace Newtonsoft.Json.Utilities
{
  internal interface IWrappedDictionary : IDictionary, ICollection, IEnumerable
  {
    object UnderlyingDictionary { get; }
  }
}
