// Decompiled with JetBrains decompiler
// Type: WhatsApp.UriMessageWrapper
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp
{
  public class UriMessageWrapper
  {
    private Message msg;
    private FunXMPP.FMessage fmsg;

    public string Text
    {
      get
      {
        if (this.msg != null)
          return this.msg.Data;
        return this.fmsg != null ? this.fmsg.data : (string) null;
      }
      set
      {
        if (this.msg != null)
          this.msg.Data = value;
        if (this.fmsg == null)
          return;
        this.fmsg.data = value;
      }
    }

    public string MatchedText
    {
      get
      {
        if (this.msg != null)
          return this.msg.LocationDetails;
        return this.fmsg != null ? this.fmsg.details : (string) null;
      }
      set
      {
        if (this.msg != null)
          this.msg.LocationDetails = value;
        if (this.fmsg == null)
          return;
        this.fmsg.details = value;
      }
    }

    public string CanonicalUrl
    {
      get
      {
        if (this.msg != null)
          return this.msg.LocationUrl;
        return this.fmsg != null ? this.fmsg.location_url : (string) null;
      }
      set
      {
        if (this.msg != null)
          this.msg.LocationUrl = value;
        if (this.fmsg == null)
          return;
        this.fmsg.location_url = value;
      }
    }

    public string Description
    {
      get
      {
        if (this.msg != null)
          return this.msg.MediaCaption;
        return this.fmsg != null ? this.fmsg.media_caption : (string) null;
      }
      set
      {
        if (this.msg != null)
          this.msg.MediaCaption = value;
        if (this.fmsg == null)
          return;
        this.fmsg.media_caption = value;
      }
    }

    public string Title
    {
      get
      {
        if (this.msg != null)
          return this.msg.MediaName;
        return this.fmsg != null ? this.fmsg.media_name : (string) null;
      }
      set
      {
        if (this.msg != null)
          this.msg.MediaName = value;
        if (this.fmsg == null)
          return;
        this.fmsg.media_name = value;
      }
    }

    public UriMessageWrapper(Message m) => this.msg = m;

    public UriMessageWrapper(FunXMPP.FMessage m) => this.fmsg = m;
  }
}
