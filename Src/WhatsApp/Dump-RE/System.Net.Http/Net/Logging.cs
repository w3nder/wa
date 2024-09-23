// Decompiled with JetBrains decompiler
// Type: System.Net.Logging
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

#nullable disable
namespace System.Net
{
  internal static class Logging
  {
    public static object Http => (object) null;

    public static bool On => false;

    public static object Web => (object) null;

    public static void Associate(object traceSource, object objA, object objB)
    {
    }

    public static void Enter(object traceSource, object obj, string method, object retObject)
    {
    }

    public static void Exception(object traceSource, object obj, string method, System.Exception e)
    {
    }

    public static void Exit(object traceSource, object obj, string method, object retObject)
    {
    }

    internal static object[] GetObjectLogHash(object obj) => (object[]) null;

    internal static void PrintError(object traceSource, string errorMessage)
    {
    }

    internal static void PrintError(
      object traceSource,
      object obj,
      string method,
      string errorMessage)
    {
    }

    internal static void PrintInfo(object traceSource, object obj, string message)
    {
    }

    internal static void PrintWarning(object traceSource, string warning)
    {
    }
  }
}
