﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.MeetingMessageType
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using Newtonsoft.Json;

#nullable disable
namespace Microsoft.Graph
{
  [JsonConverter(typeof (EnumConverter))]
  public enum MeetingMessageType
  {
    None,
    MeetingRequest,
    MeetingCancelled,
    MeetingAccepted,
    MeetingTenativelyAccepted,
    MeetingDeclined,
  }
}
