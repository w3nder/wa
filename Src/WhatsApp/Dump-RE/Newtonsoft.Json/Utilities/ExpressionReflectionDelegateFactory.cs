// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.ExpressionReflectionDelegateFactory
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace Newtonsoft.Json.Utilities
{
  internal class ExpressionReflectionDelegateFactory : ReflectionDelegateFactory
  {
    private static readonly ExpressionReflectionDelegateFactory _instance = new ExpressionReflectionDelegateFactory();

    internal static ReflectionDelegateFactory Instance
    {
      get => (ReflectionDelegateFactory) ExpressionReflectionDelegateFactory._instance;
    }

    public override ObjectConstructor<object> CreateParametrizedConstructor(MethodBase method)
    {
      ValidationUtils.ArgumentNotNull((object) method, nameof (method));
      Type type = typeof (object);
      ParameterExpression argsParameterExpression = Expression.Parameter(typeof (object[]), "args");
      return (ObjectConstructor<object>) Expression.Lambda(typeof (ObjectConstructor<object>), this.BuildMethodCall(method, type, (ParameterExpression) null, argsParameterExpression), argsParameterExpression).Compile();
    }

    public override MethodCall<T, object> CreateMethodCall<T>(MethodBase method)
    {
      ValidationUtils.ArgumentNotNull((object) method, nameof (method));
      Type type = typeof (object);
      ParameterExpression targetParameterExpression = Expression.Parameter(type, "target");
      ParameterExpression argsParameterExpression = Expression.Parameter(typeof (object[]), "args");
      return (MethodCall<T, object>) Expression.Lambda(typeof (MethodCall<T, object>), this.BuildMethodCall(method, type, targetParameterExpression, argsParameterExpression), targetParameterExpression, argsParameterExpression).Compile();
    }

    private Expression BuildMethodCall(
      MethodBase method,
      Type type,
      ParameterExpression targetParameterExpression,
      ParameterExpression argsParameterExpression)
    {
      ParameterInfo[] parameters = method.GetParameters();
      Expression[] expressionArray = new Expression[parameters.Length];
      IList<ExpressionReflectionDelegateFactory.ByRefParameter> byRefParameterList = (IList<ExpressionReflectionDelegateFactory.ByRefParameter>) new List<ExpressionReflectionDelegateFactory.ByRefParameter>();
      for (int index1 = 0; index1 < parameters.Length; ++index1)
      {
        ParameterInfo parameterInfo = parameters[index1];
        Type type1 = parameterInfo.ParameterType;
        bool flag = false;
        if (type1.IsByRef)
        {
          type1 = type1.GetElementType();
          flag = true;
        }
        Expression index2 = (Expression) Expression.Constant((object) index1);
        Expression expression1 = (Expression) Expression.ArrayIndex((Expression) argsParameterExpression, index2);
        Expression expression2 = !type1.IsValueType() ? this.EnsureCastExpression(expression1, type1) : this.EnsureCastExpression((Expression) Expression.Coalesce(expression1, (Expression) Expression.New(type1)), type1);
        if (flag)
        {
          ParameterExpression parameterExpression = Expression.Variable(type1);
          byRefParameterList.Add(new ExpressionReflectionDelegateFactory.ByRefParameter()
          {
            Value = expression2,
            Variable = parameterExpression,
            IsOut = parameterInfo.IsOut
          });
          expression2 = (Expression) parameterExpression;
        }
        expressionArray[index1] = expression2;
      }
      Expression expression3 = !method.IsConstructor ? (!method.IsStatic ? (Expression) Expression.Call(this.EnsureCastExpression((Expression) targetParameterExpression, method.DeclaringType), (MethodInfo) method, expressionArray) : (Expression) Expression.Call((MethodInfo) method, expressionArray)) : (Expression) Expression.New((ConstructorInfo) method, expressionArray);
      Expression expression4 = !(method is MethodInfo) ? this.EnsureCastExpression(expression3, type) : (((MethodInfo) method).ReturnType == typeof (void) ? (Expression) Expression.Block(expression3, (Expression) Expression.Constant((object) null)) : this.EnsureCastExpression(expression3, type));
      if (byRefParameterList.Count > 0)
      {
        IList<ParameterExpression> variables = (IList<ParameterExpression>) new List<ParameterExpression>();
        IList<Expression> expressionList = (IList<Expression>) new List<Expression>();
        foreach (ExpressionReflectionDelegateFactory.ByRefParameter byRefParameter in (IEnumerable<ExpressionReflectionDelegateFactory.ByRefParameter>) byRefParameterList)
        {
          if (!byRefParameter.IsOut)
            expressionList.Add((Expression) Expression.Assign((Expression) byRefParameter.Variable, byRefParameter.Value));
          variables.Add(byRefParameter.Variable);
        }
        expressionList.Add(expression4);
        expression4 = (Expression) Expression.Block((IEnumerable<ParameterExpression>) variables, (IEnumerable<Expression>) expressionList);
      }
      return expression4;
    }

    public override Func<T> CreateDefaultConstructor<T>(Type type)
    {
      ValidationUtils.ArgumentNotNull((object) type, nameof (type));
      if (type.IsAbstract())
        return (Func<T>) (() => (T) Activator.CreateInstance(type));
      try
      {
        Type targetType = typeof (T);
        return (Func<T>) Expression.Lambda(typeof (Func<T>), this.EnsureCastExpression((Expression) Expression.New(type), targetType)).Compile();
      }
      catch
      {
        return (Func<T>) (() => (T) Activator.CreateInstance(type));
      }
    }

    public override Func<T, object> CreateGet<T>(PropertyInfo propertyInfo)
    {
      ValidationUtils.ArgumentNotNull((object) propertyInfo, nameof (propertyInfo));
      Type type = typeof (T);
      Type targetType = typeof (object);
      ParameterExpression parameterExpression = Expression.Parameter(type, "instance");
      return (Func<T, object>) Expression.Lambda(typeof (Func<T, object>), this.EnsureCastExpression(!propertyInfo.GetGetMethod(true).IsStatic ? (Expression) Expression.MakeMemberAccess(this.EnsureCastExpression((Expression) parameterExpression, propertyInfo.DeclaringType), (MemberInfo) propertyInfo) : (Expression) Expression.MakeMemberAccess((Expression) null, (MemberInfo) propertyInfo), targetType), parameterExpression).Compile();
    }

    public override Func<T, object> CreateGet<T>(FieldInfo fieldInfo)
    {
      ValidationUtils.ArgumentNotNull((object) fieldInfo, nameof (fieldInfo));
      ParameterExpression parameterExpression;
      return Expression.Lambda<Func<T, object>>(this.EnsureCastExpression(!fieldInfo.IsStatic ? (Expression) Expression.Field(this.EnsureCastExpression((Expression) parameterExpression, fieldInfo.DeclaringType), fieldInfo) : (Expression) Expression.Field((Expression) null, fieldInfo), typeof (object)), parameterExpression).Compile();
    }

    public override Action<T, object> CreateSet<T>(FieldInfo fieldInfo)
    {
      ValidationUtils.ArgumentNotNull((object) fieldInfo, nameof (fieldInfo));
      if (fieldInfo.DeclaringType.IsValueType() || fieldInfo.IsInitOnly)
        return LateBoundReflectionDelegateFactory.Instance.CreateSet<T>(fieldInfo);
      ParameterExpression parameterExpression1 = Expression.Parameter(typeof (T), "source");
      ParameterExpression parameterExpression2 = Expression.Parameter(typeof (object), "value");
      Expression left = !fieldInfo.IsStatic ? (Expression) Expression.Field(this.EnsureCastExpression((Expression) parameterExpression1, fieldInfo.DeclaringType), fieldInfo) : (Expression) Expression.Field((Expression) null, fieldInfo);
      Expression right = this.EnsureCastExpression((Expression) parameterExpression2, left.Type);
      return (Action<T, object>) Expression.Lambda(typeof (Action<T, object>), (Expression) Expression.Assign(left, right), parameterExpression1, parameterExpression2).Compile();
    }

    public override Action<T, object> CreateSet<T>(PropertyInfo propertyInfo)
    {
      ValidationUtils.ArgumentNotNull((object) propertyInfo, nameof (propertyInfo));
      if (propertyInfo.DeclaringType.IsValueType())
        return LateBoundReflectionDelegateFactory.Instance.CreateSet<T>(propertyInfo);
      Type type1 = typeof (T);
      Type type2 = typeof (object);
      ParameterExpression parameterExpression1 = Expression.Parameter(type1, "instance");
      ParameterExpression parameterExpression2 = Expression.Parameter(type2, "value");
      Expression expression = this.EnsureCastExpression((Expression) parameterExpression2, propertyInfo.PropertyType);
      MethodInfo setMethod = propertyInfo.GetSetMethod(true);
      Expression body;
      if (setMethod.IsStatic)
        body = (Expression) Expression.Call(setMethod, expression);
      else
        body = (Expression) Expression.Call(this.EnsureCastExpression((Expression) parameterExpression1, propertyInfo.DeclaringType), setMethod, expression);
      return (Action<T, object>) Expression.Lambda(typeof (Action<T, object>), body, parameterExpression1, parameterExpression2).Compile();
    }

    private Expression EnsureCastExpression(Expression expression, Type targetType)
    {
      Type type = expression.Type;
      return type == targetType || !type.IsValueType() && targetType.IsAssignableFrom(type) ? expression : (Expression) Expression.Convert(expression, targetType);
    }

    private class ByRefParameter
    {
      public Expression Value;
      public ParameterExpression Variable;
      public bool IsOut;
    }
  }
}
