// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.ReflectionUtils
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Text;

#nullable disable
namespace Newtonsoft.Json.Utilities
{
  internal static class ReflectionUtils
  {
    public static readonly Type[] EmptyTypes = new Type[0];

    public static bool IsVirtual(this PropertyInfo propertyInfo)
    {
      ValidationUtils.ArgumentNotNull((object) propertyInfo, nameof (propertyInfo));
      MethodInfo getMethod = propertyInfo.GetGetMethod();
      if (getMethod != null && getMethod.IsVirtual)
        return true;
      MethodInfo setMethod = propertyInfo.GetSetMethod();
      return setMethod != null && setMethod.IsVirtual;
    }

    public static MethodInfo GetBaseDefinition(this PropertyInfo propertyInfo)
    {
      ValidationUtils.ArgumentNotNull((object) propertyInfo, nameof (propertyInfo));
      MethodInfo getMethod = propertyInfo.GetGetMethod();
      if (getMethod != null)
        return getMethod.GetBaseDefinition();
      return propertyInfo.GetSetMethod()?.GetBaseDefinition();
    }

    public static bool IsPublic(PropertyInfo property)
    {
      return property.GetGetMethod() != null && property.GetGetMethod().IsPublic || property.GetSetMethod() != null && property.GetSetMethod().IsPublic;
    }

    public static Type GetObjectType(object v) => v?.GetType();

    public static string GetTypeName(
      Type t,
      FormatterAssemblyStyle assemblyFormat,
      SerializationBinder binder)
    {
      string fullyQualifiedTypeName;
      if (binder != null)
      {
        string assemblyName;
        string typeName;
        binder.BindToName(t, out assemblyName, out typeName);
        fullyQualifiedTypeName = typeName + (assemblyName == null ? "" : ", " + assemblyName);
      }
      else
        fullyQualifiedTypeName = t.AssemblyQualifiedName;
      switch (assemblyFormat)
      {
        case FormatterAssemblyStyle.Simple:
          return ReflectionUtils.RemoveAssemblyDetails(fullyQualifiedTypeName);
        case FormatterAssemblyStyle.Full:
          return fullyQualifiedTypeName;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private static string RemoveAssemblyDetails(string fullyQualifiedTypeName)
    {
      StringBuilder stringBuilder = new StringBuilder();
      bool flag1 = false;
      bool flag2 = false;
      for (int index = 0; index < fullyQualifiedTypeName.Length; ++index)
      {
        char ch = fullyQualifiedTypeName[index];
        switch (ch)
        {
          case ',':
            if (!flag1)
            {
              flag1 = true;
              stringBuilder.Append(ch);
              break;
            }
            flag2 = true;
            break;
          case '[':
            flag1 = false;
            flag2 = false;
            stringBuilder.Append(ch);
            break;
          case ']':
            flag1 = false;
            flag2 = false;
            stringBuilder.Append(ch);
            break;
          default:
            if (!flag2)
            {
              stringBuilder.Append(ch);
              break;
            }
            break;
        }
      }
      return stringBuilder.ToString();
    }

    public static bool HasDefaultConstructor(Type t, bool nonPublic)
    {
      ValidationUtils.ArgumentNotNull((object) t, nameof (t));
      return t.IsValueType() || ReflectionUtils.GetDefaultConstructor(t, nonPublic) != null;
    }

    public static ConstructorInfo GetDefaultConstructor(Type t)
    {
      return ReflectionUtils.GetDefaultConstructor(t, false);
    }

    public static ConstructorInfo GetDefaultConstructor(Type t, bool nonPublic)
    {
      BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public;
      if (nonPublic)
        bindingAttr |= BindingFlags.NonPublic;
      return ((IEnumerable<ConstructorInfo>) t.GetConstructors(bindingAttr)).SingleOrDefault<ConstructorInfo>((Func<ConstructorInfo, bool>) (c => !((IEnumerable<ParameterInfo>) c.GetParameters()).Any<ParameterInfo>()));
    }

    public static bool IsNullable(Type t)
    {
      ValidationUtils.ArgumentNotNull((object) t, nameof (t));
      return !t.IsValueType() || ReflectionUtils.IsNullableType(t);
    }

    public static bool IsNullableType(Type t)
    {
      ValidationUtils.ArgumentNotNull((object) t, nameof (t));
      return t.IsGenericType() && t.GetGenericTypeDefinition() == typeof (Nullable<>);
    }

    public static Type EnsureNotNullableType(Type t)
    {
      return !ReflectionUtils.IsNullableType(t) ? t : Nullable.GetUnderlyingType(t);
    }

    public static bool IsGenericDefinition(Type type, Type genericInterfaceDefinition)
    {
      return type.IsGenericType() && type.GetGenericTypeDefinition() == genericInterfaceDefinition;
    }

    public static bool ImplementsGenericDefinition(Type type, Type genericInterfaceDefinition)
    {
      return ReflectionUtils.ImplementsGenericDefinition(type, genericInterfaceDefinition, out Type _);
    }

    public static bool ImplementsGenericDefinition(
      Type type,
      Type genericInterfaceDefinition,
      out Type implementingType)
    {
      ValidationUtils.ArgumentNotNull((object) type, nameof (type));
      ValidationUtils.ArgumentNotNull((object) genericInterfaceDefinition, nameof (genericInterfaceDefinition));
      if (!genericInterfaceDefinition.IsInterface() || !genericInterfaceDefinition.IsGenericTypeDefinition())
        throw new ArgumentNullException("'{0}' is not a generic interface definition.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) genericInterfaceDefinition));
      if (type.IsInterface() && type.IsGenericType())
      {
        Type genericTypeDefinition = type.GetGenericTypeDefinition();
        if (genericInterfaceDefinition == genericTypeDefinition)
        {
          implementingType = type;
          return true;
        }
      }
      foreach (Type type1 in type.GetInterfaces())
      {
        if (type1.IsGenericType())
        {
          Type genericTypeDefinition = type1.GetGenericTypeDefinition();
          if (genericInterfaceDefinition == genericTypeDefinition)
          {
            implementingType = type1;
            return true;
          }
        }
      }
      implementingType = (Type) null;
      return false;
    }

    public static bool InheritsGenericDefinition(Type type, Type genericClassDefinition)
    {
      return ReflectionUtils.InheritsGenericDefinition(type, genericClassDefinition, out Type _);
    }

    public static bool InheritsGenericDefinition(
      Type type,
      Type genericClassDefinition,
      out Type implementingType)
    {
      ValidationUtils.ArgumentNotNull((object) type, nameof (type));
      ValidationUtils.ArgumentNotNull((object) genericClassDefinition, nameof (genericClassDefinition));
      return genericClassDefinition.IsClass() && genericClassDefinition.IsGenericTypeDefinition() ? ReflectionUtils.InheritsGenericDefinitionInternal(type, genericClassDefinition, out implementingType) : throw new ArgumentNullException("'{0}' is not a generic class definition.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) genericClassDefinition));
    }

    private static bool InheritsGenericDefinitionInternal(
      Type currentType,
      Type genericClassDefinition,
      out Type implementingType)
    {
      if (currentType.IsGenericType())
      {
        Type genericTypeDefinition = currentType.GetGenericTypeDefinition();
        if (genericClassDefinition == genericTypeDefinition)
        {
          implementingType = currentType;
          return true;
        }
      }
      if (currentType.BaseType() != null)
        return ReflectionUtils.InheritsGenericDefinitionInternal(currentType.BaseType(), genericClassDefinition, out implementingType);
      implementingType = (Type) null;
      return false;
    }

    /// <summary>Gets the type of the typed collection's items.</summary>
    /// <param name="type">The type.</param>
    /// <returns>The type of the typed collection's items.</returns>
    public static Type GetCollectionItemType(Type type)
    {
      ValidationUtils.ArgumentNotNull((object) type, nameof (type));
      if (type.IsArray)
        return type.GetElementType();
      Type implementingType;
      if (ReflectionUtils.ImplementsGenericDefinition(type, typeof (IEnumerable<>), out implementingType))
        return !implementingType.IsGenericTypeDefinition() ? implementingType.GetGenericArguments()[0] : throw new Exception("Type {0} is not a collection.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) type));
      if (typeof (IEnumerable).IsAssignableFrom(type))
        return (Type) null;
      throw new Exception("Type {0} is not a collection.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) type));
    }

    public static void GetDictionaryKeyValueTypes(
      Type dictionaryType,
      out Type keyType,
      out Type valueType)
    {
      ValidationUtils.ArgumentNotNull((object) dictionaryType, "type");
      Type implementingType;
      if (ReflectionUtils.ImplementsGenericDefinition(dictionaryType, typeof (IDictionary<,>), out implementingType))
      {
        Type[] typeArray = !implementingType.IsGenericTypeDefinition() ? implementingType.GetGenericArguments() : throw new Exception("Type {0} is not a dictionary.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) dictionaryType));
        keyType = typeArray[0];
        valueType = typeArray[1];
      }
      else
      {
        if (!typeof (IDictionary).IsAssignableFrom(dictionaryType))
          throw new Exception("Type {0} is not a dictionary.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) dictionaryType));
        keyType = (Type) null;
        valueType = (Type) null;
      }
    }

    /// <summary>Gets the member's underlying type.</summary>
    /// <param name="member">The member.</param>
    /// <returns>The underlying type of the member.</returns>
    public static Type GetMemberUnderlyingType(MemberInfo member)
    {
      ValidationUtils.ArgumentNotNull((object) member, nameof (member));
      switch (member.MemberType())
      {
        case MemberTypes.Property:
          return ((PropertyInfo) member).PropertyType;
        case MemberTypes.Field:
          return ((FieldInfo) member).FieldType;
        case MemberTypes.Event:
          return ((EventInfo) member).EventHandlerType;
        case MemberTypes.Method:
          return ((MethodInfo) member).ReturnType;
        default:
          throw new ArgumentException("MemberInfo must be of type FieldInfo, PropertyInfo, EventInfo or MethodInfo", nameof (member));
      }
    }

    /// <summary>Determines whether the member is an indexed property.</summary>
    /// <param name="member">The member.</param>
    /// <returns>
    /// 	<c>true</c> if the member is an indexed property; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsIndexedProperty(MemberInfo member)
    {
      ValidationUtils.ArgumentNotNull((object) member, nameof (member));
      return member is PropertyInfo property && ReflectionUtils.IsIndexedProperty(property);
    }

    /// <summary>
    /// Determines whether the property is an indexed property.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns>
    /// 	<c>true</c> if the property is an indexed property; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsIndexedProperty(PropertyInfo property)
    {
      ValidationUtils.ArgumentNotNull((object) property, nameof (property));
      return property.GetIndexParameters().Length > 0;
    }

    /// <summary>Gets the member's value on the object.</summary>
    /// <param name="member">The member.</param>
    /// <param name="target">The target object.</param>
    /// <returns>The member's value on the object.</returns>
    public static object GetMemberValue(MemberInfo member, object target)
    {
      ValidationUtils.ArgumentNotNull((object) member, nameof (member));
      ValidationUtils.ArgumentNotNull(target, nameof (target));
      switch (member.MemberType())
      {
        case MemberTypes.Property:
          try
          {
            return ((PropertyInfo) member).GetValue(target, (object[]) null);
          }
          catch (TargetParameterCountException ex)
          {
            throw new ArgumentException("MemberInfo '{0}' has index parameters".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) member.Name), (Exception) ex);
          }
        case MemberTypes.Field:
          return ((FieldInfo) member).GetValue(target);
        default:
          throw new ArgumentException("MemberInfo '{0}' is not of type FieldInfo or PropertyInfo".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) CultureInfo.InvariantCulture, (object) member.Name), nameof (member));
      }
    }

    /// <summary>Sets the member's value on the target object.</summary>
    /// <param name="member">The member.</param>
    /// <param name="target">The target.</param>
    /// <param name="value">The value.</param>
    public static void SetMemberValue(MemberInfo member, object target, object value)
    {
      ValidationUtils.ArgumentNotNull((object) member, nameof (member));
      ValidationUtils.ArgumentNotNull(target, nameof (target));
      switch (member.MemberType())
      {
        case MemberTypes.Property:
          ((PropertyInfo) member).SetValue(target, value, (object[]) null);
          break;
        case MemberTypes.Field:
          ((FieldInfo) member).SetValue(target, value);
          break;
        default:
          throw new ArgumentException("MemberInfo '{0}' must be of type FieldInfo or PropertyInfo".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) member.Name), nameof (member));
      }
    }

    /// <summary>
    /// Determines whether the specified MemberInfo can be read.
    /// </summary>
    /// <param name="member">The MemberInfo to determine whether can be read.</param>
    /// 
    ///             /// <param name="nonPublic">if set to <c>true</c> then allow the member to be gotten non-publicly.</param>
    /// <returns>
    /// 	<c>true</c> if the specified MemberInfo can be read; otherwise, <c>false</c>.
    /// </returns>
    public static bool CanReadMemberValue(MemberInfo member, bool nonPublic)
    {
      switch (member.MemberType())
      {
        case MemberTypes.Property:
          PropertyInfo propertyInfo = (PropertyInfo) member;
          if (!propertyInfo.CanRead)
            return false;
          return nonPublic || propertyInfo.GetGetMethod(nonPublic) != null;
        case MemberTypes.Field:
          FieldInfo fieldInfo = (FieldInfo) member;
          return nonPublic || fieldInfo.IsPublic;
        default:
          return false;
      }
    }

    /// <summary>
    /// Determines whether the specified MemberInfo can be set.
    /// </summary>
    /// <param name="member">The MemberInfo to determine whether can be set.</param>
    /// <param name="nonPublic">if set to <c>true</c> then allow the member to be set non-publicly.</param>
    /// <param name="canSetReadOnly">if set to <c>true</c> then allow the member to be set if read-only.</param>
    /// <returns>
    /// 	<c>true</c> if the specified MemberInfo can be set; otherwise, <c>false</c>.
    /// </returns>
    public static bool CanSetMemberValue(MemberInfo member, bool nonPublic, bool canSetReadOnly)
    {
      switch (member.MemberType())
      {
        case MemberTypes.Property:
          PropertyInfo propertyInfo = (PropertyInfo) member;
          if (!propertyInfo.CanWrite)
            return false;
          return nonPublic || propertyInfo.GetSetMethod(nonPublic) != null;
        case MemberTypes.Field:
          FieldInfo fieldInfo = (FieldInfo) member;
          return !fieldInfo.IsLiteral && (!fieldInfo.IsInitOnly || canSetReadOnly) && (nonPublic || fieldInfo.IsPublic);
        default:
          return false;
      }
    }

    public static List<MemberInfo> GetFieldsAndProperties(Type type, BindingFlags bindingAttr)
    {
      List<MemberInfo> source1 = new List<MemberInfo>();
      source1.AddRange((IEnumerable<MemberInfo>) ReflectionUtils.GetFields(type, bindingAttr));
      source1.AddRange((IEnumerable<MemberInfo>) ReflectionUtils.GetProperties(type, bindingAttr));
      List<MemberInfo> fieldsAndProperties = new List<MemberInfo>(source1.Count);
      foreach (IGrouping<string, MemberInfo> source2 in source1.GroupBy<MemberInfo, string>((Func<MemberInfo, string>) (m => m.Name)))
      {
        int num = source2.Count<MemberInfo>();
        IList<MemberInfo> list = (IList<MemberInfo>) source2.ToList<MemberInfo>();
        if (num == 1)
        {
          fieldsAndProperties.Add(list.First<MemberInfo>());
        }
        else
        {
          IList<MemberInfo> collection = (IList<MemberInfo>) new List<MemberInfo>();
          foreach (MemberInfo memberInfo in (IEnumerable<MemberInfo>) list)
          {
            if (collection.Count == 0)
              collection.Add(memberInfo);
            else if (!ReflectionUtils.IsOverridenGenericMember(memberInfo, bindingAttr) || memberInfo.Name == "Item")
              collection.Add(memberInfo);
          }
          fieldsAndProperties.AddRange((IEnumerable<MemberInfo>) collection);
        }
      }
      return fieldsAndProperties;
    }

    private static bool IsOverridenGenericMember(MemberInfo memberInfo, BindingFlags bindingAttr)
    {
      if (memberInfo.MemberType() != MemberTypes.Property)
        return false;
      PropertyInfo propertyInfo = (PropertyInfo) memberInfo;
      if (!propertyInfo.IsVirtual())
        return false;
      Type declaringType = propertyInfo.DeclaringType;
      if (!declaringType.IsGenericType())
        return false;
      Type genericTypeDefinition = declaringType.GetGenericTypeDefinition();
      if (genericTypeDefinition == null)
        return false;
      MemberInfo[] member = genericTypeDefinition.GetMember(propertyInfo.Name, bindingAttr);
      return member.Length != 0 && ReflectionUtils.GetMemberUnderlyingType(member[0]).IsGenericParameter;
    }

    public static T GetAttribute<T>(object attributeProvider) where T : Attribute
    {
      return ReflectionUtils.GetAttribute<T>(attributeProvider, true);
    }

    public static T GetAttribute<T>(object attributeProvider, bool inherit) where T : Attribute
    {
      T[] attributes = ReflectionUtils.GetAttributes<T>(attributeProvider, inherit);
      return attributes == null ? default (T) : ((IEnumerable<T>) attributes).FirstOrDefault<T>();
    }

    public static T[] GetAttributes<T>(object attributeProvider, bool inherit) where T : Attribute
    {
      Attribute[] attributes = ReflectionUtils.GetAttributes(attributeProvider, typeof (T), inherit);
      return attributes is T[] objArray ? objArray : attributes.Cast<T>().ToArray<T>();
    }

    public static Attribute[] GetAttributes(
      object attributeProvider,
      Type attributeType,
      bool inherit)
    {
      ValidationUtils.ArgumentNotNull(attributeProvider, nameof (attributeProvider));
      object obj = attributeProvider;
      switch (obj)
      {
        case Type _:
          Type type = (Type) obj;
          return (attributeType != null ? (IEnumerable) type.GetCustomAttributes(attributeType, inherit) : (IEnumerable) type.GetCustomAttributes(inherit)).Cast<Attribute>().ToArray<Attribute>();
        case Assembly _:
          Assembly element1 = (Assembly) obj;
          return attributeType == null ? Attribute.GetCustomAttributes(element1) : Attribute.GetCustomAttributes(element1, attributeType);
        case MemberInfo _:
          MemberInfo element2 = (MemberInfo) obj;
          return attributeType == null ? Attribute.GetCustomAttributes(element2, inherit) : Attribute.GetCustomAttributes(element2, attributeType, inherit);
        case ParameterInfo _:
          ParameterInfo element3 = (ParameterInfo) obj;
          return attributeType == null ? Attribute.GetCustomAttributes(element3, inherit) : Attribute.GetCustomAttributes(element3, attributeType, inherit);
        default:
          throw new Exception("Cannot get attributes from '{0}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, obj));
      }
    }

    public static void SplitFullyQualifiedTypeName(
      string fullyQualifiedTypeName,
      out string typeName,
      out string assemblyName)
    {
      int? assemblyDelimiterIndex = ReflectionUtils.GetAssemblyDelimiterIndex(fullyQualifiedTypeName);
      if (assemblyDelimiterIndex.HasValue)
      {
        typeName = fullyQualifiedTypeName.Substring(0, assemblyDelimiterIndex.Value).Trim();
        assemblyName = fullyQualifiedTypeName.Substring(assemblyDelimiterIndex.Value + 1, fullyQualifiedTypeName.Length - assemblyDelimiterIndex.Value - 1).Trim();
      }
      else
      {
        typeName = fullyQualifiedTypeName;
        assemblyName = (string) null;
      }
    }

    private static int? GetAssemblyDelimiterIndex(string fullyQualifiedTypeName)
    {
      int num = 0;
      for (int index = 0; index < fullyQualifiedTypeName.Length; ++index)
      {
        switch (fullyQualifiedTypeName[index])
        {
          case ',':
            if (num == 0)
              return new int?(index);
            break;
          case '[':
            ++num;
            break;
          case ']':
            --num;
            break;
        }
      }
      return new int?();
    }

    public static MemberInfo GetMemberInfoFromType(Type targetType, MemberInfo memberInfo)
    {
      if (memberInfo.MemberType() != MemberTypes.Property)
        return targetType.GetMember(memberInfo.Name, memberInfo.MemberType(), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).SingleOrDefault<MemberInfo>();
      PropertyInfo propertyInfo = (PropertyInfo) memberInfo;
      Type[] array = ((IEnumerable<ParameterInfo>) propertyInfo.GetIndexParameters()).Select<ParameterInfo, Type>((Func<ParameterInfo, Type>) (p => p.ParameterType)).ToArray<Type>();
      return (MemberInfo) targetType.GetProperty(propertyInfo.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, (object) null, propertyInfo.PropertyType, (IList<Type>) array, (object) null);
    }

    public static IEnumerable<FieldInfo> GetFields(Type targetType, BindingFlags bindingAttr)
    {
      ValidationUtils.ArgumentNotNull((object) targetType, nameof (targetType));
      List<MemberInfo> memberInfoList = new List<MemberInfo>((IEnumerable<MemberInfo>) targetType.GetFields(bindingAttr));
      ReflectionUtils.GetChildPrivateFields((IList<MemberInfo>) memberInfoList, targetType, bindingAttr);
      return memberInfoList.Cast<FieldInfo>();
    }

    private static void GetChildPrivateFields(
      IList<MemberInfo> initialFields,
      Type targetType,
      BindingFlags bindingAttr)
    {
      if ((bindingAttr & BindingFlags.NonPublic) == (BindingFlags) 0)
        return;
      BindingFlags bindingAttr1 = bindingAttr.RemoveFlag(BindingFlags.Public);
      while ((targetType = targetType.BaseType()) != null)
      {
        IEnumerable<MemberInfo> collection = ((IEnumerable<FieldInfo>) targetType.GetFields(bindingAttr1)).Where<FieldInfo>((Func<FieldInfo, bool>) (f => f.IsPrivate)).Cast<MemberInfo>();
        initialFields.AddRange<MemberInfo>(collection);
      }
    }

    public static IEnumerable<PropertyInfo> GetProperties(Type targetType, BindingFlags bindingAttr)
    {
      ValidationUtils.ArgumentNotNull((object) targetType, nameof (targetType));
      List<PropertyInfo> initialProperties = new List<PropertyInfo>((IEnumerable<PropertyInfo>) targetType.GetProperties(bindingAttr));
      if (targetType.IsInterface())
      {
        foreach (Type type in targetType.GetInterfaces())
          initialProperties.AddRange((IEnumerable<PropertyInfo>) type.GetProperties(bindingAttr));
      }
      ReflectionUtils.GetChildPrivateProperties((IList<PropertyInfo>) initialProperties, targetType, bindingAttr);
      for (int index = 0; index < initialProperties.Count; ++index)
      {
        PropertyInfo propertyInfo = initialProperties[index];
        if (propertyInfo.DeclaringType != targetType)
        {
          PropertyInfo memberInfoFromType = (PropertyInfo) ReflectionUtils.GetMemberInfoFromType(propertyInfo.DeclaringType, (MemberInfo) propertyInfo);
          initialProperties[index] = memberInfoFromType;
        }
      }
      return (IEnumerable<PropertyInfo>) initialProperties;
    }

    public static BindingFlags RemoveFlag(this BindingFlags bindingAttr, BindingFlags flag)
    {
      return (bindingAttr & flag) != flag ? bindingAttr : bindingAttr ^ flag;
    }

    private static void GetChildPrivateProperties(
      IList<PropertyInfo> initialProperties,
      Type targetType,
      BindingFlags bindingAttr)
    {
      while ((targetType = targetType.BaseType()) != null)
      {
        foreach (PropertyInfo property in targetType.GetProperties(bindingAttr))
        {
          PropertyInfo subTypeProperty = property;
          if (!ReflectionUtils.IsPublic(subTypeProperty))
          {
            int index = initialProperties.IndexOf<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.Name == subTypeProperty.Name));
            if (index == -1)
              initialProperties.Add(subTypeProperty);
            else if (!ReflectionUtils.IsPublic(initialProperties[index]))
              initialProperties[index] = subTypeProperty;
          }
          else if (!subTypeProperty.IsVirtual())
          {
            if (initialProperties.IndexOf<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.Name == subTypeProperty.Name && p.DeclaringType == subTypeProperty.DeclaringType)) == -1)
              initialProperties.Add(subTypeProperty);
          }
          else if (initialProperties.IndexOf<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.Name == subTypeProperty.Name && p.IsVirtual() && p.GetBaseDefinition() != null && p.GetBaseDefinition().DeclaringType.IsAssignableFrom(subTypeProperty.DeclaringType))) == -1)
            initialProperties.Add(subTypeProperty);
        }
      }
    }

    public static bool IsMethodOverridden(
      Type currentType,
      Type methodDeclaringType,
      string method)
    {
      return ((IEnumerable<MethodInfo>) currentType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)).Any<MethodInfo>((Func<MethodInfo, bool>) (info => info.Name == method && info.DeclaringType != methodDeclaringType && info.GetBaseDefinition().DeclaringType == methodDeclaringType));
    }

    public static object GetDefaultValue(Type type)
    {
      if (!type.IsValueType())
        return (object) null;
      switch (ConvertUtils.GetTypeCode(type))
      {
        case PrimitiveTypeCode.Char:
        case PrimitiveTypeCode.SByte:
        case PrimitiveTypeCode.Int16:
        case PrimitiveTypeCode.UInt16:
        case PrimitiveTypeCode.Int32:
        case PrimitiveTypeCode.Byte:
        case PrimitiveTypeCode.UInt32:
          return (object) 0;
        case PrimitiveTypeCode.Boolean:
          return (object) false;
        case PrimitiveTypeCode.Int64:
        case PrimitiveTypeCode.UInt64:
          return (object) 0L;
        case PrimitiveTypeCode.Single:
          return (object) 0.0f;
        case PrimitiveTypeCode.Double:
          return (object) 0.0;
        case PrimitiveTypeCode.DateTime:
          return (object) new DateTime();
        case PrimitiveTypeCode.DateTimeOffset:
          return (object) new DateTimeOffset();
        case PrimitiveTypeCode.Decimal:
          return (object) 0M;
        case PrimitiveTypeCode.Guid:
          return (object) new Guid();
        default:
          return ReflectionUtils.IsNullable(type) ? (object) null : Activator.CreateInstance(type);
      }
    }
  }
}
