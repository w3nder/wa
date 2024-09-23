// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.JsonPosition
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#nullable disable
namespace Newtonsoft.Json
{
  internal struct JsonPosition
  {
    private static readonly char[] SpecialCharacters = new char[6]
    {
      '.',
      ' ',
      '[',
      ']',
      '(',
      ')'
    };
    internal JsonContainerType Type;
    internal int Position;
    internal string PropertyName;
    internal bool HasIndex;

    public JsonPosition(JsonContainerType type)
    {
      this.Type = type;
      this.HasIndex = JsonPosition.TypeHasIndex(type);
      this.Position = -1;
      this.PropertyName = (string) null;
    }

    internal void WriteTo(StringBuilder sb)
    {
      switch (this.Type)
      {
        case JsonContainerType.Object:
          if (sb.Length > 0)
            sb.Append('.');
          string propertyName = this.PropertyName;
          if (propertyName.IndexOfAny(JsonPosition.SpecialCharacters) != -1)
          {
            sb.Append("['");
            sb.Append(propertyName);
            sb.Append("']");
            break;
          }
          sb.Append(propertyName);
          break;
        case JsonContainerType.Array:
        case JsonContainerType.Constructor:
          sb.Append('[');
          sb.Append(this.Position);
          sb.Append(']');
          break;
      }
    }

    internal static bool TypeHasIndex(JsonContainerType type)
    {
      return type == JsonContainerType.Array || type == JsonContainerType.Constructor;
    }

    internal static string BuildPath(IEnumerable<JsonPosition> positions)
    {
      StringBuilder sb = new StringBuilder();
      foreach (JsonPosition position in positions)
        position.WriteTo(sb);
      return sb.ToString();
    }

    internal static string FormatMessage(IJsonLineInfo lineInfo, string path, string message)
    {
      if (!message.EndsWith(Environment.NewLine, StringComparison.Ordinal))
      {
        message = message.Trim();
        if (!message.EndsWith('.'))
          message += ".";
        message += " ";
      }
      message += "Path '{0}'".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) path);
      if (lineInfo != null && lineInfo.HasLineInfo())
        message += ", line {0}, position {1}".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) lineInfo.LineNumber, (object) lineInfo.LinePosition);
      message += ".";
      return message;
    }
  }
}
