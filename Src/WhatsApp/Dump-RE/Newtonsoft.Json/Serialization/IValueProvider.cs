// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.IValueProvider
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

#nullable disable
namespace Newtonsoft.Json.Serialization
{
  /// <summary>Provides methods to get and set values.</summary>
  public interface IValueProvider
  {
    /// <summary>Sets the value.</summary>
    /// <param name="target">The target to set the value on.</param>
    /// <param name="value">The value to set on the target.</param>
    void SetValue(object target, object value);

    /// <summary>Gets the value.</summary>
    /// <param name="target">The target to get the value from.</param>
    /// <returns>The value.</returns>
    object GetValue(object target);
  }
}
