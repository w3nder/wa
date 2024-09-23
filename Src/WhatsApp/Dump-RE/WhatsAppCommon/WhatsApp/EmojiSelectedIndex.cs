// Decompiled with JetBrains decompiler
// Type: WhatsApp.EmojiSelectedIndex
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Data.Linq.Mapping;
using System.Data.Linq.Mapping;

#nullable disable
namespace WhatsApp
{
  [Table]
  [Index(Columns = "EmojiCode", IsUnique = true)]
  public class EmojiSelectedIndex : PropChangingBase
  {
    private string emojiCode_;
    private int selection_;

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
    public int Selection
    {
      get => this.selection_;
      set
      {
        if (this.selection_ == value)
          return;
        this.NotifyPropertyChanging(nameof (Selection));
        this.selection_ = value;
      }
    }
  }
}
