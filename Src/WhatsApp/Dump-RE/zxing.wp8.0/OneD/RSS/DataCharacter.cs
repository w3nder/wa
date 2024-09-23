// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.DataCharacter
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.OneD.RSS
{
  /// <summary>
  /// 
  /// </summary>
  public class DataCharacter
  {
    /// <summary>Gets the value.</summary>
    public int Value { get; private set; }

    /// <summary>Gets the checksum portion.</summary>
    public int ChecksumPortion { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.OneD.RSS.DataCharacter" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="checksumPortion">The checksum portion.</param>
    public DataCharacter(int value, int checksumPortion)
    {
      this.Value = value;
      this.ChecksumPortion = checksumPortion;
    }

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return this.Value.ToString() + "(" + (object) this.ChecksumPortion + (object) ')';
    }

    /// <summary>
    /// Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
    /// </summary>
    /// <param name="o">The <see cref="T:System.Object" /> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object o)
    {
      if (!(o is DataCharacter))
        return false;
      DataCharacter dataCharacter = (DataCharacter) o;
      return this.Value == dataCharacter.Value && this.ChecksumPortion == dataCharacter.ChecksumPortion;
    }

    /// <summary>Returns a hash code for this instance.</summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override int GetHashCode() => this.Value ^ this.ChecksumPortion;
  }
}
