// Decompiled with JetBrains decompiler
// Type: WhatsApp.MessageSearchResult
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class MessageSearchResult
  {
    public Message Message { get; private set; }

    public Pair<int, int>[] DataOffsets { get; set; }

    public Pair<int, int>[] MediaNameOffsets { get; set; }

    public Pair<int, int>[] MediaCaptionOffsets { get; set; }

    public Pair<int, int>[] LocationDetailsOffsets { get; set; }

    public MessageSearchResult(Message m) => this.Message = m;
  }
}
