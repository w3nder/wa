// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.ValidationUtils
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;
using System.Globalization;

#nullable disable
namespace Newtonsoft.Json.Utilities
{
  internal static class ValidationUtils
  {
    public static void ArgumentNotNullOrEmpty(string value, string parameterName)
    {
      switch (value)
      {
        case null:
          throw new ArgumentNullException(parameterName);
        case "":
          throw new ArgumentException("'{0}' cannot be empty.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) parameterName), parameterName);
      }
    }

    public static void ArgumentTypeIsEnum(Type enumType, string parameterName)
    {
      ValidationUtils.ArgumentNotNull((object) enumType, nameof (enumType));
      if (!enumType.IsEnum())
        throw new ArgumentException("Type {0} is not an Enum.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) enumType), parameterName);
    }

    public static void ArgumentNotNull(object value, string parameterName)
    {
      if (value == null)
        throw new ArgumentNullException(parameterName);
    }
  }
}
