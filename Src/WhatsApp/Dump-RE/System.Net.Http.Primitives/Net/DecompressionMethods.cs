// Decompiled with JetBrains decompiler
// Type: System.Net.DecompressionMethods
// Assembly: System.Net.Http.Primitives, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 254C62AD-DCB5-4A11-92C4-B88227BACC42
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.Primitives.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.Primitives.xml

#nullable disable
namespace System.Net
{
  /// <summary>
  /// Represents the file compression and decompression encoding format to be used to compress the data received in response to an <see cref="T:System.Net.HttpWebRequest" />.
  /// </summary>
  [Flags]
  public enum DecompressionMethods
  {
    /// <summary>Do not use compression.</summary>
    None = 0,
    /// <summary>Use the gZip compression-decompression algorithm.</summary>
    GZip = 1,
    /// <summary>Use the deflate compression-decompression algorithm.</summary>
    Deflate = 2,
  }
}
