// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.ErrorEventArgs
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;

#nullable disable
namespace Newtonsoft.Json.Serialization
{
  /// <summary>Provides data for the Error event.</summary>
  public class ErrorEventArgs : EventArgs
  {
    /// <summary>
    /// Gets the current object the error event is being raised against.
    /// </summary>
    /// <value>The current object the error event is being raised against.</value>
    public object CurrentObject { get; private set; }

    /// <summary>Gets the error context.</summary>
    /// <value>The error context.</value>
    public ErrorContext ErrorContext { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Serialization.ErrorEventArgs" /> class.
    /// </summary>
    /// <param name="currentObject">The current object.</param>
    /// <param name="errorContext">The error context.</param>
    public ErrorEventArgs(object currentObject, ErrorContext errorContext)
    {
      this.CurrentObject = currentObject;
      this.ErrorContext = errorContext;
    }
  }
}
