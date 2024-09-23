// Decompiled with JetBrains decompiler
// Type: System.Runtime.Serialization.Formatters.FormatterAssemblyStyle
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

#nullable disable
namespace System.Runtime.Serialization.Formatters
{
  /// <summary>
  /// Indicates the method that will be used during deserialization for locating and loading assemblies.
  /// </summary>
  public enum FormatterAssemblyStyle
  {
    /// <summary>
    /// In simple mode, the assembly used during deserialization need not match exactly the assembly used during serialization. Specifically, the version numbers need not match as the LoadWithPartialName method is used to load the assembly.
    /// </summary>
    Simple,
    /// <summary>
    /// In full mode, the assembly used during deserialization must match exactly the assembly used during serialization. The Load method of the Assembly class is used to load the assembly.
    /// </summary>
    Full,
  }
}
