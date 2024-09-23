// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.TypeExtensions
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable disable
namespace Newtonsoft.Json.Utilities
{
  internal static class TypeExtensions
  {
    public static MethodInfo Method(this Delegate d) => d.Method;

    public static MemberTypes MemberType(this MemberInfo memberInfo)
    {
      switch (memberInfo)
      {
        case PropertyInfo _:
          return MemberTypes.Property;
        case FieldInfo _:
          return MemberTypes.Field;
        case EventInfo _:
          return MemberTypes.Event;
        case MethodInfo _:
          return MemberTypes.Method;
        default:
          return MemberTypes.Other;
      }
    }

    public static bool ContainsGenericParameters(this Type type) => type.ContainsGenericParameters;

    public static bool IsInterface(this Type type) => type.IsInterface;

    public static bool IsGenericType(this Type type) => type.IsGenericType;

    public static bool IsGenericTypeDefinition(this Type type) => type.IsGenericTypeDefinition;

    public static Type BaseType(this Type type) => type.BaseType;

    public static System.Reflection.Assembly Assembly(this Type type) => type.Assembly;

    public static bool IsEnum(this Type type) => type.IsEnum;

    public static bool IsClass(this Type type) => type.IsClass;

    public static bool IsSealed(this Type type) => type.IsSealed;

    public static PropertyInfo GetProperty(
      this Type type,
      string name,
      BindingFlags bindingFlags,
      object placeholder1,
      Type propertyType,
      IList<Type> indexParameters,
      object placeholder2)
    {
      return ((IEnumerable<PropertyInfo>) type.GetProperties(bindingFlags)).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => (name == null || !(name != p.Name)) && (propertyType == null || propertyType == p.PropertyType) && (indexParameters == null || ((IEnumerable<ParameterInfo>) p.GetIndexParameters()).Select<ParameterInfo, Type>((Func<ParameterInfo, Type>) (ip => ip.ParameterType)).SequenceEqual<Type>((IEnumerable<Type>) indexParameters)))).SingleOrDefault<PropertyInfo>();
    }

    public static IEnumerable<MemberInfo> GetMember(
      this Type type,
      string name,
      MemberTypes memberType,
      BindingFlags bindingFlags)
    {
      return ((IEnumerable<MemberInfo>) type.GetMembers(bindingFlags)).Where<MemberInfo>((Func<MemberInfo, bool>) (m => (name == null || !(name != m.Name)) && m.MemberType() == memberType));
    }

    public static bool IsAbstract(this Type type) => type.IsAbstract;

    public static bool IsVisible(this Type type) => type.IsVisible;

    public static bool IsValueType(this Type type) => type.IsValueType;

    public static bool AssignableToTypeName(this Type type, string fullTypeName, out Type match)
    {
      for (Type type1 = type; type1 != null; type1 = type1.BaseType())
      {
        if (string.Equals(type1.FullName, fullTypeName, StringComparison.Ordinal))
        {
          match = type1;
          return true;
        }
      }
      foreach (MemberInfo memberInfo in type.GetInterfaces())
      {
        if (string.Equals(memberInfo.Name, fullTypeName, StringComparison.Ordinal))
        {
          match = type;
          return true;
        }
      }
      match = (Type) null;
      return false;
    }

    public static bool AssignableToTypeName(this Type type, string fullTypeName)
    {
      return type.AssignableToTypeName(fullTypeName, out Type _);
    }
  }
}
