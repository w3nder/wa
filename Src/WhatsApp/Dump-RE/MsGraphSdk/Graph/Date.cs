// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.Date
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using Newtonsoft.Json;
using System;

#nullable disable
namespace Microsoft.Graph
{
  [JsonConverter(typeof (DateConverter))]
  public class Date
  {
    internal Date(DateTime dateTime) => this.DateTime = dateTime;

    public Date(int year, int month, int day)
      : this(new DateTime(year, month, day))
    {
    }

    internal DateTime DateTime { get; set; }

    public int Year => this.DateTime.Year;

    public int Month => this.DateTime.Month;

    public int Day => this.DateTime.Day;

    public override string ToString() => this.DateTime.ToString("yyyy-MM-dd");
  }
}
