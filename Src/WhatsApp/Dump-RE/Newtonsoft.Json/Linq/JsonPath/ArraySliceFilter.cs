// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JsonPath.ArraySliceFilter
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace Newtonsoft.Json.Linq.JsonPath
{
  internal class ArraySliceFilter : PathFilter
  {
    public int? Start { get; set; }

    public int? End { get; set; }

    public int? Step { get; set; }

    public override IEnumerable<JToken> ExecuteFilter(
      IEnumerable<JToken> current,
      bool errorWhenNoMatch)
    {
      int? step = this.Step;
      if ((step.GetValueOrDefault() != 0 ? 0 : (step.HasValue ? 1 : 0)) != 0)
        throw new JsonException("Step cannot be zero.");
      foreach (JToken t in current)
      {
        if (t is JArray a)
        {
          int stepCount = this.Step ?? 1;
          int startIndex = this.Start ?? (stepCount > 0 ? 0 : a.Count - 1);
          int stopIndex = this.End ?? (stepCount > 0 ? a.Count : -1);
          int? start = this.Start;
          if ((start.GetValueOrDefault() >= 0 ? 0 : (start.HasValue ? 1 : 0)) != 0)
            startIndex = a.Count + startIndex;
          int? end = this.End;
          if ((end.GetValueOrDefault() >= 0 ? 0 : (end.HasValue ? 1 : 0)) != 0)
            stopIndex = a.Count + stopIndex;
          startIndex = Math.Max(startIndex, stepCount > 0 ? 0 : int.MinValue);
          startIndex = Math.Min(startIndex, stepCount > 0 ? a.Count : a.Count - 1);
          stopIndex = Math.Max(stopIndex, -1);
          stopIndex = Math.Min(stopIndex, a.Count);
          bool positiveStep = stepCount > 0;
          if (this.IsValid(startIndex, stopIndex, positiveStep))
          {
            for (int i = startIndex; this.IsValid(i, stopIndex, positiveStep); i += stepCount)
              yield return a[i];
          }
          else if (errorWhenNoMatch)
            throw new JsonException("Array slice of {0} to {1} returned no results.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, this.Start.HasValue ? (object) this.Start.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture) : (object) "*", this.End.HasValue ? (object) this.End.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture) : (object) "*"));
        }
        else if (errorWhenNoMatch)
          throw new JsonException("Array slice is not valid on {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) t.GetType().Name));
      }
    }

    private bool IsValid(int index, int stopIndex, bool positiveStep)
    {
      return positiveStep ? index < stopIndex : index > stopIndex;
    }
  }
}
