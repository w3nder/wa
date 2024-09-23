// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.ExpressionValueProvider
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;
using System.Reflection;

#nullable disable
namespace Newtonsoft.Json.Serialization
{
  /// <summary>
  /// Get and set values for a <see cref="T:System.Reflection.MemberInfo" /> using dynamic methods.
  /// </summary>
  public class ExpressionValueProvider : IValueProvider
  {
    private readonly MemberInfo _memberInfo;
    private Func<object, object> _getter;
    private Action<object, object> _setter;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Serialization.ExpressionValueProvider" /> class.
    /// </summary>
    /// <param name="memberInfo">The member info.</param>
    public ExpressionValueProvider(MemberInfo memberInfo)
    {
      ValidationUtils.ArgumentNotNull((object) memberInfo, nameof (memberInfo));
      this._memberInfo = memberInfo;
    }

    /// <summary>Sets the value.</summary>
    /// <param name="target">The target to set the value on.</param>
    /// <param name="value">The value to set on the target.</param>
    public void SetValue(object target, object value)
    {
      try
      {
        if (this._setter == null)
          this._setter = ExpressionReflectionDelegateFactory.Instance.CreateSet<object>(this._memberInfo);
        this._setter(target, value);
      }
      catch (Exception ex)
      {
        throw new JsonSerializationException("Error setting value to '{0}' on '{1}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) this._memberInfo.Name, (object) target.GetType()), ex);
      }
    }

    /// <summary>Gets the value.</summary>
    /// <param name="target">The target to get the value from.</param>
    /// <returns>The value.</returns>
    public object GetValue(object target)
    {
      try
      {
        if (this._getter == null)
          this._getter = ExpressionReflectionDelegateFactory.Instance.CreateGet<object>(this._memberInfo);
        return this._getter(target);
      }
      catch (Exception ex)
      {
        throw new JsonSerializationException("Error getting value from '{0}' on '{1}'.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) this._memberInfo.Name, (object) target.GetType()), ex);
      }
    }
  }
}
