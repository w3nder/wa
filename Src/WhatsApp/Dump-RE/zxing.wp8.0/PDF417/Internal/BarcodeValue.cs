// Decompiled with JetBrains decompiler
// Type: ZXing.PDF417.Internal.BarcodeValue
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;

#nullable disable
namespace ZXing.PDF417.Internal
{
  /// <summary>
  /// A Barcode Value for the PDF417 barcode.
  /// The scanner will iterate through the bitmatrix,
  /// and given the different methods or iterations
  /// will increment a given barcode value's confidence.
  /// 
  /// When done, this will return the values of highest confidence.
  /// </summary>
  /// <author>Guenther Grau</author>
  public sealed class BarcodeValue
  {
    private readonly IDictionary<int, int> values = (IDictionary<int, int>) new Dictionary<int, int>();

    /// <summary>
    /// Incremenets the Confidence for a given value. (Adds an occurance of a value)
    /// 
    /// </summary>
    /// <param name="value">Value.</param>
    public void setValue(int value)
    {
      int num1;
      this.values.TryGetValue(value, out num1);
      int num2 = num1 + 1;
      this.values[value] = num2;
    }

    /// <summary>
    /// Determines the maximum occurrence of a set value and returns all values which were set with this occurrence.
    /// </summary>
    /// <returns>an array of int, containing the values with the highest occurrence, or null, if no value was set.</returns>
    public int[] getValue()
    {
      int num = -1;
      List<int> intList = new List<int>();
      foreach (KeyValuePair<int, int> keyValuePair in (IEnumerable<KeyValuePair<int, int>>) this.values)
      {
        if (keyValuePair.Value > num)
        {
          num = keyValuePair.Value;
          intList.Clear();
          intList.Add(keyValuePair.Key);
        }
        else if (keyValuePair.Value == num)
          intList.Add(keyValuePair.Key);
      }
      return intList.ToArray();
    }

    /// <summary>Returns the confience value for a given barcode value</summary>
    /// <param name="barcodeValue">Barcode value.</param>
    public int getConfidence(int barcodeValue)
    {
      return !this.values.ContainsKey(barcodeValue) ? 0 : this.values[barcodeValue];
    }
  }
}
