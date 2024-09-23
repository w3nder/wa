// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.TraceLevel
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// Specifies what messages to output for the <see cref="T:Newtonsoft.Json.Serialization.ITraceWriter" /> class.
  /// </summary>
  public enum TraceLevel
  {
    /// <summary>Output no tracing and debugging messages.</summary>
    Off,
    /// <summary>Output error-handling messages.</summary>
    Error,
    /// <summary>Output warnings and error-handling messages.</summary>
    Warning,
    /// <summary>
    /// Output informational messages, warnings, and error-handling messages.
    /// </summary>
    Info,
    /// <summary>Output all debugging and tracing messages.</summary>
    Verbose,
  }
}
