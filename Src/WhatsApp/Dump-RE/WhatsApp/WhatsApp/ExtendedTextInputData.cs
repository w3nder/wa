// Decompiled with JetBrains decompiler
// Type: WhatsApp.ExtendedTextInputData
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

#nullable disable
namespace WhatsApp
{
  public class ExtendedTextInputData
  {
    public string Text { get; set; }

    public WebPageMetadata LinkPreviewData { get; set; }

    public Message QuotedMessage { get; set; }

    public string QuotedChat { get; set; }

    public string[] MentionedJids { get; set; }
  }
}
