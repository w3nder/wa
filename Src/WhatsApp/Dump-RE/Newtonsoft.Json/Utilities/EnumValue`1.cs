// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.EnumValue`1
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

#nullable disable
namespace Newtonsoft.Json.Utilities
{
  internal class EnumValue<T> where T : struct
  {
    private readonly string _name;
    private readonly T _value;

    public string Name => this._name;

    public T Value => this._value;

    public EnumValue(string name, T value)
    {
      this._name = name;
      this._value = value;
    }
  }
}
