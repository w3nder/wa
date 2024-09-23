// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.SerializationErrorCallback
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System.Runtime.Serialization;

#nullable disable
namespace Newtonsoft.Json.Serialization
{
  /// <summary>
  /// Handles <see cref="T:Newtonsoft.Json.JsonSerializer" /> serialization error callback events.
  /// </summary>
  /// <param name="o">The object that raised the callback event.</param>
  /// <param name="context">The streaming context.</param>
  /// <param name="errorContext">The error context.</param>
  public delegate void SerializationErrorCallback(
    object o,
    StreamingContext context,
    ErrorContext errorContext);
}
