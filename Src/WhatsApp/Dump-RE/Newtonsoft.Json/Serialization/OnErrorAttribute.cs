// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.OnErrorAttribute
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;

#nullable disable
namespace Newtonsoft.Json.Serialization
{
  /// <summary>
  /// When applied to a method, specifies that the method is called when an error occurs serializing an object.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, Inherited = false)]
  public sealed class OnErrorAttribute : Attribute
  {
  }
}
