// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaSelfStatusItemControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class WaSelfStatusItemControl : StatusItemControl
  {
    private StackPanel actionsPanel;

    protected override void InitComponents()
    {
      base.InitComponents();
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Auto)
      });
      this.actionsPanel = new StackPanel()
      {
        Orientation = Orientation.Horizontal
      };
      this.detailsPanel.Tap += new EventHandler<GestureEventArgs>(this.DetailsPanel_Tap);
      this.iconPanel.Tap += new EventHandler<GestureEventArgs>(this.IconPanel_Tap);
    }

    protected void IconPanel_Tap(object sender, EventArgs e)
    {
      if (!(this.ViewModel is SelfStatusThreadViewModel viewModel) || !viewModel.IsRecipientMode)
        return;
      this.ToggleSelection();
    }

    protected void DetailsPanel_Tap(object sender, EventArgs e)
    {
      this.IconPanel_Tap((object) null, (EventArgs) null);
    }

    public override void UpdateSelection()
    {
      if (this.ViewModel is SelfStatusThreadViewModel viewModel && viewModel.IsRecipientMode)
      {
        this.iconPanel.Background = viewModel.IsSelected ? (Brush) UIUtils.AccentBrush : UIUtils.PhoneChromeBrush;
        this.icon.Source = viewModel.IsSelected ? (System.Windows.Media.ImageSource) AssetStore.StatusIconWhite : (System.Windows.Media.ImageSource) AssetStore.StatusIcon;
      }
      else
        this.iconPanel.Background = UIUtils.PhoneChromeBrush;
    }
  }
}
