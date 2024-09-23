﻿// Decompiled with JetBrains decompiler
// Type: System.LightupServices
// Assembly: System.Net.Http.Extensions, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4532C01-CAE1-4FEB-922A-3FFFB1361F31
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.Extensions.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.Extensions.xml

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable disable
namespace System
{
  internal static class LightupServices
  {
    public static Delegate NotFound = (Delegate) (() => { });
    public static Func<object[], object> ConstructorNotFound = (Func<object[], object>) (_ => new object());

    public static Delegate ReplaceWith(Delegate d, Type delegateType)
    {
      return Delegate.CreateDelegate(delegateType, d.Target, d.Method);
    }

    public static Type[] GetMethodArgumentTypes(Type actionOrFuncType, bool bindInstance = true)
    {
      Type[] source = actionOrFuncType.GetGenericArguments();
      if (!bindInstance)
        source = ((IEnumerable<Type>) source).Skip<Type>(1).ToArray<Type>();
      return LightupServices.IsActionType(actionOrFuncType) ? source : ((IEnumerable<Type>) source).Take<Type>(source.Length - 1).ToArray<Type>();
    }

    public static bool IsActionType(Type type)
    {
      if (type.IsGenericType)
        type = type.GetGenericTypeDefinition();
      return type == typeof (Action) || type == typeof (Action<>) || type == typeof (Action<,>) || type == typeof (Action<,,>) || type == typeof (Action<,,,>);
    }

    public static Delegate CreateDelegate(Type type, object instance, MethodInfo method)
    {
      if (method.IsStatic)
        instance = (object) null;
      try
      {
        return Delegate.CreateDelegate(type, instance, method);
      }
      catch (InvalidOperationException ex)
      {
      }
      catch (MemberAccessException ex)
      {
      }
      return LightupServices.NotFound;
    }
  }
}
