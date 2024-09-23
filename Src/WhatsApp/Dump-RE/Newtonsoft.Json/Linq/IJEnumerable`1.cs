// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.IJEnumerable`1
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Newtonsoft.Json.Linq
{
  /// <summary>
  /// Represents a collection of <see cref="T:Newtonsoft.Json.Linq.JToken" /> objects.
  /// </summary>
  /// <typeparam name="T">The type of token</typeparam>
  public interface IJEnumerable<out T> : IEnumerable<T>, IEnumerable where T : JToken
  {
    /// <summary>
    /// Gets the <see cref="T:Newtonsoft.Json.Linq.IJEnumerable`1" /> with the specified key.
    /// </summary>
    /// <value></value>
    IJEnumerable<JToken> this[object key] { get; }
  }
}
