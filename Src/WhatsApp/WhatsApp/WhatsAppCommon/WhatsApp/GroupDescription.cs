// Decompiled with JetBrains decompiler
// Type: WhatsApp.GroupDescription
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;


namespace WhatsApp
{
  public class GroupDescription
  {
    public string Id { get; }

    public int? Error { get; set; }

    public string Body { get; }

    public DateTime? CreateTime { get; set; }

    public string PreviousId { get; set; }

    public string Owner { get; set; }

    public GroupDescription(string body, string id = null)
    {
      this.Id = id ?? Guid.NewGuid().ToString();
      this.Body = body;
    }
  }
}
