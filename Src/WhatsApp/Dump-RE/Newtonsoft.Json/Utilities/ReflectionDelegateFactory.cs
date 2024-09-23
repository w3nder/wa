// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.ReflectionDelegateFactory
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Serialization;
using System;
using System.Globalization;
using System.Reflection;

#nullable disable
namespace Newtonsoft.Json.Utilities
{
  internal abstract class ReflectionDelegateFactory
  {
    public Func<T, object> CreateGet<T>(MemberInfo memberInfo)
    {
      switch (memberInfo)
      {
        case PropertyInfo propertyInfo:
          return this.CreateGet<T>(propertyInfo);
        case FieldInfo fieldInfo:
          return this.CreateGet<T>(fieldInfo);
        default:
          throw new Exception("Could not create getter for {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) memberInfo));
      }
    }

    public Action<T, object> CreateSet<T>(MemberInfo memberInfo)
    {
      switch (memberInfo)
      {
        case PropertyInfo propertyInfo:
          return this.CreateSet<T>(propertyInfo);
        case FieldInfo fieldInfo:
          return this.CreateSet<T>(fieldInfo);
        default:
          throw new Exception("Could not create setter for {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) memberInfo));
      }
    }

    public abstract MethodCall<T, object> CreateMethodCall<T>(MethodBase method);

    public abstract ObjectConstructor<object> CreateParametrizedConstructor(MethodBase method);

    public abstract Func<T> CreateDefaultConstructor<T>(Type type);

    public abstract Func<T, object> CreateGet<T>(PropertyInfo propertyInfo);

    public abstract Func<T, object> CreateGet<T>(FieldInfo fieldInfo);

    public abstract Action<T, object> CreateSet<T>(FieldInfo fieldInfo);

    public abstract Action<T, object> CreateSet<T>(PropertyInfo propertyInfo);
  }
}
