// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Schema.JsonSchemaException
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;

#nullable disable
namespace Newtonsoft.Json.Schema
{
  /// <summary>
  /// <para>
  /// Returns detailed information about the schema exception.
  /// </para>
  /// <note type="caution">
  /// JSON Schema validation has been moved to its own package. See <see href="http://www.newtonsoft.com/jsonschema">http://www.newtonsoft.com/jsonschema</see> for more details.
  /// </note>
  /// </summary>
  [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
  public class JsonSchemaException : JsonException
  {
    /// <summary>
    /// Gets the line number indicating where the error occurred.
    /// </summary>
    /// <value>The line number indicating where the error occurred.</value>
    public int LineNumber { get; private set; }

    /// <summary>
    /// Gets the line position indicating where the error occurred.
    /// </summary>
    /// <value>The line position indicating where the error occurred.</value>
    public int LinePosition { get; private set; }

    /// <summary>Gets the path to the JSON where the error occurred.</summary>
    /// <value>The path to the JSON where the error occurred.</value>
    public string Path { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Schema.JsonSchemaException" /> class.
    /// </summary>
    public JsonSchemaException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Schema.JsonSchemaException" /> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public JsonSchemaException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Schema.JsonSchemaException" /> class
    /// with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
    public JsonSchemaException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    internal JsonSchemaException(
      string message,
      Exception innerException,
      string path,
      int lineNumber,
      int linePosition)
      : base(message, innerException)
    {
      this.Path = path;
      this.LineNumber = lineNumber;
      this.LinePosition = linePosition;
    }
  }
}
