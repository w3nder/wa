// Decompiled with JetBrains decompiler
// Type: WhatsApp.CountryInfoViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Windows.Media;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class CountryInfoViewModel : WaViewModelBase
  {
    public bool IsTop;
    public const double ItemHeight = 72.0;
    private CountryInfoItem item_;
    private bool isSelected_;

    public CountryInfoViewModel(CountryInfoItem item) => this.item_ = item;

    public CountryInfoItem Item => this.item_;

    public string FullName => this.item_.FullName;

    public string PhoneCountryCode => this.item_.PhoneCountryCode;

    public SolidColorBrush ForegroundBrush
    {
      get => !this.isSelected_ ? UIUtils.ForegroundBrush : UIUtils.AccentBrush;
    }

    public bool IsSelected
    {
      get => this.isSelected_;
      set
      {
        if (this.isSelected_ == value)
          return;
        this.isSelected_ = value;
        this.NotifyPropertyChanged("ForegroundBrush");
      }
    }

    public double Height => 72.0;
  }
}
