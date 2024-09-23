// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.CalendarColor
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using Newtonsoft.Json;

#nullable disable
namespace Microsoft.Graph
{
  [JsonConverter(typeof (EnumConverter))]
  public enum CalendarColor
  {
    LightBlue,
    LightGreen,
    LightOrange,
    LightGray,
    LightYellow,
    LightTeal,
    LightPink,
    LightBrown,
    LightRed,
    MaxColor,
    Auto,
  }
}
