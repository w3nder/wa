// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.JsonWriterException
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// The exception thrown when an error occurs while reading JSON text.
  /// </summary>
  public class JsonWriterException : JsonException
  {
    /// <summary>Gets the path to the JSON where the error occurred.</summary>
    /// <value>The path to the JSON where the error occurred.</value>
    public string Path { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonWriterException" /> class.
    /// </summary>
    public JsonWriterException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonWriterException" /> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public JsonWriterException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonWriterException" /> class
    /// with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
    public JsonWriterException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    internal JsonWriterException(string message, Exception innerException, string path)
      : base(message, innerException)
    {
      this.Path = path;
    }

    internal static JsonWriterException Create(JsonWriter writer, string message, Exception ex)
    {
      return JsonWriterException.Create(writer.ContainerPath, message, ex);
    }

    internal static JsonWriterException Create(string path, string message, Exception ex)
    {
      message = JsonPosition.FormatMessage((IJsonLineInfo) null, path, message);
      return new JsonWriterException(message, ex, path);
    }
  }
}
