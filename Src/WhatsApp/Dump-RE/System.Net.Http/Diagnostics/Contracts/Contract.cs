// Decompiled with JetBrains decompiler
// Type: System.Diagnostics.Contracts.Contract
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Collections.Generic;

#nullable disable
namespace System.Diagnostics.Contracts
{
  internal static class Contract
  {
    public static void Assert(bool condition)
    {
    }

    public static void Assert(bool condition, string message)
    {
    }

    internal static void EndContractBlock()
    {
    }

    public static void Ensures(bool p)
    {
    }

    public static void Ensures(bool p, string message)
    {
    }

    public static bool ForAll<T>(IEnumerable<T> collection, Predicate<T> predicate)
    {
      if (collection == null)
        throw new ArgumentNullException(nameof (collection));
      if (predicate == null)
        throw new ArgumentNullException(nameof (predicate));
      foreach (T obj in collection)
      {
        if (!predicate(obj))
          return false;
      }
      return true;
    }

    public static void Requires(bool condition)
    {
    }

    internal static void Requires(bool condition, string message)
    {
    }

    public static T Result<T>() => default (T);

    internal static T ValueAtReturn<T>(out T value)
    {
      value = default (T);
      return value;
    }
  }
}
