// Decompiled with JetBrains decompiler
// Type: System.SR
// Assembly: Portable.ConcurrentDictionary, Version=1.0.2.0, Culture=neutral, PublicKeyToken=null
// MVID: DB56BACC-BDC4-4C60-BF1D-8E1E2F27714A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Portable.ConcurrentDictionary.dll

#nullable disable
namespace System
{
  internal static class SR
  {
    public static string ConcurrentCollection_SyncRoot_NotSupported = "The SyncRoot property may not be used for the synchronization of concurrent collections.";
    public static string ConcurrentDictionary_ArrayIncorrectType = "The array is multidimensional, or the type parameter for the set cannot be cast automatically to the type of the destination array.";
    public static string ConcurrentDictionary_SourceContainsDuplicateKeys = "The source argument contains duplicate keys.";
    public static string ConcurrentDictionary_ConcurrencyLevelMustBePositive = "The concurrencyLevel argument must be positive.";
    public static string ConcurrentDictionary_CapacityMustNotBeNegative = "The capacity argument must be greater than or equal to zero.";
    public static string ConcurrentDictionary_IndexIsNegative = "The index argument is less than zero.";
    public static string ConcurrentDictionary_ArrayNotLargeEnough = "The index is equal to or greater than the length of the array, or the number of elements in the dictionary is greater than the available space from index to the end of the destination array.";
    public static string ConcurrentDictionary_KeyAlreadyExisted = "The key already existed in the dictionary.";
    public static string ConcurrentDictionary_ItemKeyIsNull = "TKey is a reference type and item.Key is null.";
    public static string ConcurrentDictionary_TypeOfKeyIncorrect = "The key was of an incorrect type for this dictionary.";
    public static string ConcurrentDictionary_TypeOfValueIncorrect = "The value was of an incorrect type for this dictionary.";
  }
}
