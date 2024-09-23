// Decompiled with JetBrains decompiler
// Type: System.ArrayHelper
// Assembly: Portable.ConcurrentDictionary, Version=1.0.2.0, Culture=neutral, PublicKeyToken=null
// MVID: DB56BACC-BDC4-4C60-BF1D-8E1E2F27714A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Portable.ConcurrentDictionary.dll

using System.Collections.Generic;

#nullable disable
namespace System
{
  internal static class ArrayHelper
  {
    public static KeyValuePair<TKey, TValue>[] Empty<TKey, TValue>()
    {
      return new KeyValuePair<TKey, TValue>[0];
    }
  }
}
