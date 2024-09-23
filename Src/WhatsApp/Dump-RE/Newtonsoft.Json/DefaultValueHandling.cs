// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.DefaultValueHandling
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// Specifies default value handling options for the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
  /// </summary>
  /// <example>
  ///   <code lang="cs" source="..\Src\Newtonsoft.Json.Tests\Documentation\SerializationTests.cs" region="ReducingSerializedJsonSizeDefaultValueHandlingObject" title="DefaultValueHandling Class" />
  ///   <code lang="cs" source="..\Src\Newtonsoft.Json.Tests\Documentation\SerializationTests.cs" region="ReducingSerializedJsonSizeDefaultValueHandlingExample" title="DefaultValueHandling Ignore Example" />
  /// </example>
  [Flags]
  public enum DefaultValueHandling
  {
    /// <summary>
    /// Include members where the member value is the same as the member's default value when serializing objects.
    /// Included members are written to JSON. Has no effect when deserializing.
    /// </summary>
    Include = 0,
    /// <summary>
    /// Ignore members where the member value is the same as the member's default value when serializing objects
    /// so that is is not written to JSON.
    /// This option will ignore all default values (e.g. <c>null</c> for objects and nullable types; <c>0</c> for integers,
    /// decimals and floating point numbers; and <c>false</c> for booleans). The default value ignored can be changed by
    /// placing the <see cref="T:System.ComponentModel.DefaultValueAttribute" /> on the property.
    /// </summary>
    Ignore = 1,
    /// <summary>
    /// Members with a default value but no JSON will be set to their default value when deserializing.
    /// </summary>
    Populate = 2,
    /// <summary>
    /// Ignore members where the member value is the same as the member's default value when serializing objects
    /// and sets members to their default value when deserializing.
    /// </summary>
    IgnoreAndPopulate = Populate | Ignore, // 0x00000003
  }
}
