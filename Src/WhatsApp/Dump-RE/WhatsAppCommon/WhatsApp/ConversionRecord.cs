// Decompiled with JetBrains decompiler
// Type: WhatsApp.ConversionRecord
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
  [Index(Columns = "ConversionJid")]
  public class ConversionRecord : PropChangingBase
  {
    private int recordId;
    private string conversionJid;

    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int RecordId
    {
      get => this.recordId;
      set => this.recordId = value;
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string ConversionJid
    {
      get => this.conversionJid;
      set => this.conversionJid = value;
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public DateTime Timestamp { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string PhoneNumber { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string Source { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public byte[] Data { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public DateTime LastActionTimestamp { get; set; }
  }
}
