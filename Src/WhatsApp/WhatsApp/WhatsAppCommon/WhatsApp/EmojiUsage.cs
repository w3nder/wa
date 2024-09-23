// Decompiled with JetBrains decompiler
// Type: WhatsApp.EmojiUsage
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Data.Linq.Mapping;
using System.Data.Linq.Mapping;


namespace WhatsApp
{
  [Table]
  [Index(Columns = "EmojiCode", IsUnique = true)]
  public class EmojiUsage : PropChangingBase
  {
    private string emojiCode_;
    private int usageWeight_;

    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int EmojiID { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string EmojiCode
    {
      get => this.emojiCode_;
      set
      {
        if (!(this.emojiCode_ != value))
          return;
        this.NotifyPropertyChanging(nameof (EmojiCode));
        this.emojiCode_ = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int UsageWeight
    {
      get => this.usageWeight_;
      set
      {
        if (this.usageWeight_ == value)
          return;
        this.NotifyPropertyChanging(nameof (UsageWeight));
        this.usageWeight_ = value;
      }
    }
  }
}
