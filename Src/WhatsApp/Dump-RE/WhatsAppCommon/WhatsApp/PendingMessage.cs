// Decompiled with JetBrains decompiler
// Type: WhatsApp.PendingMessage
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Data.Linq.Mapping;
using System;
using System.Data.Linq.Mapping;

#nullable disable
namespace WhatsApp
{
  [Table]
  [Index(Columns = "KeyRemoteJid")]
  public class PendingMessage : PropChangingBase
  {
    private string keyRemoteJid;
    private string keyId;
    private DateTime? timestamp;
    private byte[] protobufMessage;
    private string remoteResource;
    private byte[] pendingMsgsPropertiesProtoBuf;

    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int PendingMessagesId { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string KeyRemoteJid
    {
      get => this.keyRemoteJid;
      set
      {
        if (!(this.keyRemoteJid != value))
          return;
        this.NotifyPropertyChanging(nameof (KeyRemoteJid));
        this.keyRemoteJid = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string KeyId
    {
      get => this.keyId;
      set
      {
        if (!(this.keyId != value))
          return;
        this.NotifyPropertyChanging(nameof (KeyId));
        this.keyId = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public DateTime? Timestamp
    {
      get => this.timestamp;
      set
      {
        DateTime? nullable1 = value;
        if (nullable1.HasValue && nullable1.Value.Kind == DateTimeKind.Local)
          nullable1 = new DateTime?(nullable1.Value.ToUniversalTime());
        DateTime? timestamp = this.timestamp;
        DateTime? nullable2 = nullable1;
        if ((timestamp.HasValue == nullable2.HasValue ? (timestamp.HasValue ? (timestamp.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (Timestamp));
        this.timestamp = nullable1;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public byte[] ProtobufMessage
    {
      get => this.protobufMessage;
      set
      {
        if (this.protobufMessage == value)
          return;
        this.NotifyPropertyChanging(nameof (ProtobufMessage));
        this.protobufMessage = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string RemoteResource
    {
      get => this.remoteResource;
      set
      {
        if (!(this.remoteResource != value))
          return;
        this.NotifyPropertyChanging(nameof (RemoteResource));
        this.remoteResource = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public byte[] PendingMsgPropertiesProtobuf
    {
      get => this.pendingMsgsPropertiesProtoBuf;
      set
      {
        if (this.pendingMsgsPropertiesProtoBuf == value)
          return;
        this.NotifyPropertyChanging(nameof (PendingMsgPropertiesProtobuf));
        this.pendingMsgsPropertiesProtoBuf = value;
      }
    }

    public PendingMsgProperties InternalProperties
    {
      get
      {
        return this.PendingMsgPropertiesProtobuf == null ? (PendingMsgProperties) null : PendingMsgProperties.Deserialize(this.PendingMsgPropertiesProtobuf);
      }
      set
      {
        this.PendingMsgPropertiesProtobuf = value != null ? PendingMsgProperties.SerializeToBytes(value) : (byte[]) null;
      }
    }

    public PendingMessage()
    {
    }

    public PendingMessage(
      string keyRemoteJid,
      string keyId,
      string remoteResource,
      DateTime timestamp,
      byte[] serializedMessage)
    {
      this.keyRemoteJid = keyRemoteJid;
      this.keyId = keyId;
      this.remoteResource = remoteResource;
      this.timestamp = new DateTime?(timestamp);
      this.protobufMessage = serializedMessage;
    }
  }
}
