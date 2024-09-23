// Decompiled with JetBrains decompiler
// Type: ZXing.Aztec.AztecResultMetadata
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.Aztec
{
  /// <summary>Aztec result meta data.</summary>
  public sealed class AztecResultMetadata
  {
    /// <summary>
    /// Gets a value indicating whether this Aztec code is compact.
    /// </summary>
    /// <value>
    ///   <c>true</c> if compact; otherwise, <c>false</c>.
    /// </value>
    public bool Compact { get; private set; }

    /// <summary>Gets the nb datablocks.</summary>
    public int Datablocks { get; private set; }

    /// <summary>Gets the nb layers.</summary>
    public int Layers { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="compact"></param>
    /// <param name="datablocks"></param>
    /// <param name="layers"></param>
    public AztecResultMetadata(bool compact, int datablocks, int layers)
    {
      this.Compact = compact;
      this.Datablocks = datablocks;
      this.Layers = layers;
    }
  }
}
