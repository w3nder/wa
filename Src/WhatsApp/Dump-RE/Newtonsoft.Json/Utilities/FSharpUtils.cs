// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.FSharpUtils
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

#nullable disable
namespace Newtonsoft.Json.Utilities
{
  internal static class FSharpUtils
  {
    public const string FSharpSetTypeName = "FSharpSet`1";
    public const string FSharpListTypeName = "FSharpList`1";
    public const string FSharpMapTypeName = "FSharpMap`2";
    private static readonly object Lock = new object();
    private static bool _initialized;
    private static MethodInfo _ofSeq;
    private static Type _mapType;

    public static Assembly FSharpCoreAssembly { get; private set; }

    public static MethodCall<object, object> IsUnion { get; private set; }

    public static MethodCall<object, object> GetUnionCases { get; private set; }

    public static MethodCall<object, object> PreComputeUnionTagReader { get; private set; }

    public static MethodCall<object, object> PreComputeUnionReader { get; private set; }

    public static MethodCall<object, object> PreComputeUnionConstructor { get; private set; }

    public static Func<object, object> GetUnionCaseInfoDeclaringType { get; private set; }

    public static Func<object, object> GetUnionCaseInfoName { get; private set; }

    public static Func<object, object> GetUnionCaseInfoTag { get; private set; }

    public static MethodCall<object, object> GetUnionCaseInfoFields { get; private set; }

    public static void EnsureInitialized(Assembly fsharpCoreAssembly)
    {
      if (FSharpUtils._initialized)
        return;
      lock (FSharpUtils.Lock)
      {
        if (FSharpUtils._initialized)
          return;
        FSharpUtils.FSharpCoreAssembly = fsharpCoreAssembly;
        Type type1 = fsharpCoreAssembly.GetType("Microsoft.FSharp.Reflection.FSharpType");
        FSharpUtils.IsUnion = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>((MethodBase) type1.GetMethod("IsUnion", BindingFlags.Public | BindingFlags.Static));
        FSharpUtils.GetUnionCases = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>((MethodBase) type1.GetMethod("GetUnionCases", BindingFlags.Public | BindingFlags.Static));
        Type type2 = fsharpCoreAssembly.GetType("Microsoft.FSharp.Reflection.FSharpValue");
        FSharpUtils.PreComputeUnionTagReader = FSharpUtils.CreateFSharpFuncCall(type2, "PreComputeUnionTagReader");
        FSharpUtils.PreComputeUnionReader = FSharpUtils.CreateFSharpFuncCall(type2, "PreComputeUnionReader");
        FSharpUtils.PreComputeUnionConstructor = FSharpUtils.CreateFSharpFuncCall(type2, "PreComputeUnionConstructor");
        Type type3 = fsharpCoreAssembly.GetType("Microsoft.FSharp.Reflection.UnionCaseInfo");
        FSharpUtils.GetUnionCaseInfoName = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(type3.GetProperty("Name"));
        FSharpUtils.GetUnionCaseInfoTag = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(type3.GetProperty("Tag"));
        FSharpUtils.GetUnionCaseInfoDeclaringType = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(type3.GetProperty("DeclaringType"));
        FSharpUtils.GetUnionCaseInfoFields = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>((MethodBase) type3.GetMethod("GetFields"));
        FSharpUtils._ofSeq = fsharpCoreAssembly.GetType("Microsoft.FSharp.Collections.ListModule").GetMethod("OfSeq");
        FSharpUtils._mapType = fsharpCoreAssembly.GetType("Microsoft.FSharp.Collections.FSharpMap`2");
        Thread.MemoryBarrier();
        FSharpUtils._initialized = true;
      }
    }

    private static MethodCall<object, object> CreateFSharpFuncCall(Type type, string methodName)
    {
      MethodInfo method1 = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
      MethodInfo method2 = method1.ReturnType.GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public);
      MethodCall<object, object> call = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>((MethodBase) method1);
      MethodCall<object, object> invoke = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>((MethodBase) method2);
      return (MethodCall<object, object>) ((target, args) => (object) new FSharpFunction(call(target, args), invoke));
    }

    public static ObjectConstructor<object> CreateSeq(Type t)
    {
      return JsonTypeReflector.ReflectionDelegateFactory.CreateParametrizedConstructor((MethodBase) FSharpUtils._ofSeq.MakeGenericMethod(t));
    }

    public static ObjectConstructor<object> CreateMap(Type keyType, Type valueType)
    {
      return (ObjectConstructor<object>) typeof (FSharpUtils).GetMethod("BuildMapCreator").MakeGenericMethod(keyType, valueType).Invoke((object) null, (object[]) null);
    }

    public static ObjectConstructor<object> BuildMapCreator<TKey, TValue>()
    {
      ObjectConstructor<object> ctorDelegate = JsonTypeReflector.ReflectionDelegateFactory.CreateParametrizedConstructor((MethodBase) FSharpUtils._mapType.MakeGenericType(typeof (TKey), typeof (TValue)).GetConstructor(new Type[1]
      {
        typeof (IEnumerable<Tuple<TKey, TValue>>)
      }));
      return (ObjectConstructor<object>) (args =>
      {
        IEnumerable<Tuple<TKey, TValue>> tuples = ((IEnumerable<KeyValuePair<TKey, TValue>>) args[0]).Select<KeyValuePair<TKey, TValue>, Tuple<TKey, TValue>>((Func<KeyValuePair<TKey, TValue>, Tuple<TKey, TValue>>) (kv => new Tuple<TKey, TValue>(kv.Key, kv.Value)));
        return ctorDelegate(new object[1]{ (object) tuples });
      });
    }
  }
}
