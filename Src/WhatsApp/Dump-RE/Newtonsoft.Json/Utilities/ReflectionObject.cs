// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.ReflectionObject
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

#nullable disable
namespace Newtonsoft.Json.Utilities
{
  internal class ReflectionObject
  {
    public ObjectConstructor<object> Creator { get; private set; }

    public IDictionary<string, ReflectionMember> Members { get; private set; }

    public ReflectionObject()
    {
      this.Members = (IDictionary<string, ReflectionMember>) new Dictionary<string, ReflectionMember>();
    }

    public object GetValue(object target, string member) => this.Members[member].Getter(target);

    public void SetValue(object target, string member, object value)
    {
      this.Members[member].Setter(target, value);
    }

    public Type GetType(string member) => this.Members[member].MemberType;

    public static ReflectionObject Create(Type t, params string[] memberNames)
    {
      return ReflectionObject.Create(t, (MethodBase) null, memberNames);
    }

    public static ReflectionObject Create(Type t, MethodBase creator, params string[] memberNames)
    {
      ReflectionObject reflectionObject = new ReflectionObject();
      ReflectionDelegateFactory reflectionDelegateFactory = JsonTypeReflector.ReflectionDelegateFactory;
      if (creator != null)
        reflectionObject.Creator = reflectionDelegateFactory.CreateParametrizedConstructor(creator);
      else if (ReflectionUtils.HasDefaultConstructor(t, false))
      {
        Func<object> ctor = reflectionDelegateFactory.CreateDefaultConstructor<object>(t);
        reflectionObject.Creator = (ObjectConstructor<object>) (args => ctor());
      }
      foreach (string memberName in memberNames)
      {
        MemberInfo[] member = t.GetMember(memberName, BindingFlags.Instance | BindingFlags.Public);
        MemberInfo memberInfo = member.Length == 1 ? ((IEnumerable<MemberInfo>) member).Single<MemberInfo>() : throw new ArgumentException("Expected a single member with the name '{0}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) memberName));
        ReflectionMember reflectionMember = new ReflectionMember();
        switch (memberInfo.MemberType())
        {
          case MemberTypes.Property:
          case MemberTypes.Field:
            if (ReflectionUtils.CanReadMemberValue(memberInfo, false))
              reflectionMember.Getter = reflectionDelegateFactory.CreateGet<object>(memberInfo);
            if (ReflectionUtils.CanSetMemberValue(memberInfo, false, false))
            {
              reflectionMember.Setter = reflectionDelegateFactory.CreateSet<object>(memberInfo);
              break;
            }
            break;
          case MemberTypes.Method:
            MethodInfo method = (MethodInfo) memberInfo;
            if (method.IsPublic)
            {
              ParameterInfo[] parameters = method.GetParameters();
              if (parameters.Length == 0 && method.ReturnType != typeof (void))
              {
                MethodCall<object, object> call = reflectionDelegateFactory.CreateMethodCall<object>((MethodBase) method);
                reflectionMember.Getter = (Func<object, object>) (target => call(target, new object[0]));
                break;
              }
              if (parameters.Length == 1 && method.ReturnType == typeof (void))
              {
                MethodCall<object, object> call = reflectionDelegateFactory.CreateMethodCall<object>((MethodBase) method);
                object obj;
                reflectionMember.Setter = (Action<object, object>) ((target, arg) => obj = call(target, new object[1]
                {
                  arg
                }));
                break;
              }
              break;
            }
            break;
          default:
            throw new ArgumentException("Unexpected member type '{0}' for member '{1}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) memberInfo.MemberType(), (object) memberInfo.Name));
        }
        if (ReflectionUtils.CanReadMemberValue(memberInfo, false))
          reflectionMember.Getter = reflectionDelegateFactory.CreateGet<object>(memberInfo);
        if (ReflectionUtils.CanSetMemberValue(memberInfo, false, false))
          reflectionMember.Setter = reflectionDelegateFactory.CreateSet<object>(memberInfo);
        reflectionMember.MemberType = ReflectionUtils.GetMemberUnderlyingType(memberInfo);
        reflectionObject.Members[memberName] = reflectionMember;
      }
      return reflectionObject;
    }
  }
}
