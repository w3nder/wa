// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.JsonSerializationException
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// The exception thrown when an error occurs during JSON serialization or deserialization.
  /// </summary>
  public class JsonSerializationException : JsonException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonSerializationException" /> class.
    /// </summary>
    public JsonSerializationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonSerializationException" /> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public JsonSerializationException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonSerializationException" /> class
    /// with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
    public JsonSerializationException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    internal static JsonSerializationException Create(JsonReader reader, string message)
    {
      return JsonSerializationException.Create(reader, message, (Exception) null);
    }

    internal static JsonSerializationException Create(
      JsonReader reader,
      string message,
      Exception ex)
    {
      return JsonSerializationException.Create(reader as IJsonLineInfo, reader.Path, message, ex);
    }

    internal static JsonSerializationException Create(
      IJsonLineInfo lineInfo,
      string path,
      string message,
      Exception ex)
    {
      message = JsonPosition.FormatMessage(lineInfo, path, message);
      return new JsonSerializationException(message, ex);
    }
  }
}
