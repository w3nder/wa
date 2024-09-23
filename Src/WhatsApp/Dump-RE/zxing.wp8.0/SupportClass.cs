// Decompiled with JetBrains decompiler
// Type: ZXing.SupportClass
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace ZXing
{
  /// <summary>
  /// Contains conversion support elements such as classes, interfaces and static methods.
  /// </summary>
  public static class SupportClass
  {
    /// <summary>
    /// Copies an array of chars obtained from a String into a specified array of chars
    /// </summary>
    /// <param name="sourceString">The String to get the chars from</param>
    /// <param name="sourceStart">Position of the String to start getting the chars</param>
    /// <param name="sourceEnd">Position of the String to end getting the chars</param>
    /// <param name="destinationArray">Array to return the chars</param>
    /// <param name="destinationStart">Position of the destination array of chars to start storing the chars</param>
    /// <returns>An array of chars</returns>
    public static void GetCharsFromString(
      string sourceString,
      int sourceStart,
      int sourceEnd,
      char[] destinationArray,
      int destinationStart)
    {
      int index1 = sourceStart;
      int index2 = destinationStart;
      while (index1 < sourceEnd)
      {
        destinationArray[index2] = sourceString[index1];
        ++index1;
        ++index2;
      }
    }

    /// <summary>Sets the capacity for the specified List</summary>
    /// <param name="vector">The List which capacity will be set</param>
    /// <param name="newCapacity">The new capacity value</param>
    public static void SetCapacity<T>(IList<T> vector, int newCapacity) where T : new()
    {
      while (newCapacity > vector.Count)
        vector.Add(new T());
      while (newCapacity < vector.Count)
        vector.RemoveAt(vector.Count - 1);
    }

    /// <summary>Converts a string-Collection to an array</summary>
    /// <param name="strings">The strings.</param>
    /// <returns></returns>
    public static string[] toStringArray(ICollection<string> strings)
    {
      string[] array = new string[strings.Count];
      strings.CopyTo(array, 0);
      return array;
    }

    /// <summary>Joins all elements to one string.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="separator">The separator.</param>
    /// <param name="values">The values.</param>
    /// <returns></returns>
    public static string Join<T>(string separator, IEnumerable<T> values)
    {
      StringBuilder stringBuilder = new StringBuilder();
      separator = separator ?? string.Empty;
      if (values != null)
      {
        foreach (T obj in values)
        {
          stringBuilder.Append((object) obj);
          stringBuilder.Append(separator);
        }
        if (stringBuilder.Length > 0)
          stringBuilder.Length -= separator.Length;
      }
      return stringBuilder.ToString();
    }

    /// <summary>
    /// Fills the specified array.
    /// (can't use extension method because of .Net 2.0 support)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array">The array.</param>
    /// <param name="value">The value.</param>
    public static void Fill<T>(T[] array, T value)
    {
      for (int index = 0; index < array.Length; ++index)
        array[index] = value;
    }

    /// <summary>
    /// Fills the specified array.
    /// (can't use extension method because of .Net 2.0 support)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array">The array.</param>
    /// <param name="startIndex">The start index.</param>
    /// <param name="endIndex">The end index.</param>
    /// <param name="value">The value.</param>
    public static void Fill<T>(T[] array, int startIndex, int endIndex, T value)
    {
      for (int index = startIndex; index < endIndex; ++index)
        array[index] = value;
    }

    public static string ToBinaryString(int x)
    {
      char[] chArray = new char[32];
      int length = 0;
      for (; x != 0; x >>= 1)
        chArray[length++] = (x & 1) == 1 ? '1' : '0';
      Array.Reverse((Array) chArray, 0, length);
      return new string(chArray);
    }

    public static int bitCount(int n)
    {
      int num = 0;
      while (n != 0)
      {
        n &= n - 1;
        ++num;
      }
      return num;
    }

    /// <summary>
    /// Savely gets the value of a decoding hint
    /// if hints is null the default is returned
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="hints">The hints.</param>
    /// <param name="hintType">Type of the hint.</param>
    /// <param name="default">The @default.</param>
    /// <returns></returns>
    public static T GetValue<T>(
      IDictionary<DecodeHintType, object> hints,
      DecodeHintType hintType,
      T @default)
    {
      return hints == null || !hints.ContainsKey(hintType) ? @default : (T) hints[hintType];
    }
  }
}
