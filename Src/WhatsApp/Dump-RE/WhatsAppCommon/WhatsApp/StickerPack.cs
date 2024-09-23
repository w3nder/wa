// Decompiled with JetBrains decompiler
// Type: WhatsApp.StickerPack
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable disable
namespace WhatsApp
{
  public class StickerPack
  {
    [JsonProperty("sticker-pack-id")]
    public string PackId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("publisher")]
    public string Publisher { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("file-size")]
    public uint FileSize { get; set; }

    [JsonProperty("tray-image-id")]
    public string TrayImageId { get; set; }

    [JsonProperty("preview-main-image-id")]
    public string MainPreviewId { get; set; }

    [JsonProperty("preview-image-ids")]
    public List<string> PreviewImageIds { get; set; }

    [JsonProperty("stickers")]
    public List<Sticker> Stickers { get; set; }

    public DateTime? DownloadDateTime { get; set; }
  }
}
