// Decompiled with JetBrains decompiler
// Type: WhatsApp.DocumentMessageWrapper
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp
{
  public class DocumentMessageWrapper
  {
    private Message msg;
    private FunXMPP.FMessage fmsg;

    public string Title
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

    public int PageCount
    {
      get
      {
        if (this.msg != null)
          return this.msg.MediaDurationSeconds;
        return this.fmsg != null ? this.fmsg.media_duration_seconds : 0;
      }
      set
      {
        if (this.msg != null)
          this.msg.MediaDurationSeconds = value;
        if (this.fmsg == null)
          return;
        this.fmsg.media_duration_seconds = value;
      }
    }

    public string Filename
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

    public DocumentMessageWrapper(Message m) => this.msg = m;

    public DocumentMessageWrapper(FunXMPP.FMessage m) => this.fmsg = m;

    public string GetFileExtension()
    {
      string mimeType = (string) null;
      if (this.msg != null)
        mimeType = this.msg.MediaMimeType;
      else if (this.fmsg != null)
        mimeType = this.fmsg.media_mime_type;
      string fileExtension = Utils.InferFileExtensionFromMimeType(mimeType);
      if (fileExtension == null)
      {
        string filename = this.Filename;
        if (filename != null)
        {
          int num = filename.LastIndexOf('.');
          if (num >= 0)
            fileExtension = filename.Substring(num + 1);
        }
      }
      return fileExtension;
    }
  }
}
