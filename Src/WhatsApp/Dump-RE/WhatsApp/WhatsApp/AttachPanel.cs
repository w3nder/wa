// Decompiled with JetBrains decompiler
// Type: WhatsApp.AttachPanel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class AttachPanel : UserControl
  {
    private static double? buttonSize = new double?();
    public static double ButtonGap = 12.0;
    internal StackPanel LayoutRoot;
    private bool _contentLoaded;

    public static double ButtonSize
    {
      get
      {
        if (!AttachPanel.buttonSize.HasValue)
        {
          Size renderSize = ResolutionHelper.GetRenderSize();
          AttachPanel.buttonSize = new double?((Math.Min(renderSize.Width, renderSize.Height) - 48.0 * ResolutionHelper.ZoomMultiplier - AttachPanel.ButtonGap * 2.0) / 3.0);
        }
        return AttachPanel.buttonSize.Value;
      }
    }

    public event AttachPanel.AttachmentActionChosenHandler AttachmentActionChosen;

    protected void NotifyAttachmentActionChosen(AttachPanel.ActionType actionChosen)
    {
      if (this.AttachmentActionChosen == null)
        return;
      this.AttachmentActionChosen((object) this, actionChosen);
    }

    private AttachPanelViewModel ViewModel { get; set; }

    public PageOrientation Orientation
    {
      get => this.ViewModel.Orientation;
      set => this.ViewModel.Orientation = value;
    }

    public AttachPanel()
    {
      this.InitializeComponent();
      this.ViewModel = new AttachPanelViewModel();
      this.DataContext = (object) this.ViewModel;
    }

    public void EnableActions(AttachPanel.ActionType[] actions, bool enable)
    {
      Set<AttachPanel.ActionType> set = new Set<AttachPanel.ActionType>((IEnumerable<AttachPanel.ActionType>) actions);
      foreach (AttachButtonViewModel attachButtonViewModel in this.ViewModel.ButtonsSource)
      {
        if (attachButtonViewModel != null && set.Contains(attachButtonViewModel.ActionType))
          attachButtonViewModel.IsEnabled = enable;
      }
    }

    public void EnableAction(AttachPanel.ActionType action, bool enable)
    {
      foreach (AttachButtonViewModel attachButtonViewModel in this.ViewModel.ButtonsSource)
      {
        if (attachButtonViewModel != null && attachButtonViewModel.ActionType == action)
          attachButtonViewModel.IsEnabled = enable;
      }
    }

    private void AttachButton_Tap(object sender, EventArgs e)
    {
      if (!(sender is FrameworkElement frameworkElement) || !(frameworkElement.DataContext is AttachButtonViewModel dataContext) || !dataContext.IsEnabled)
        return;
      Log.d(nameof (AttachButton_Tap), "action {0}", (object) dataContext.ActionType);
      this.NotifyAttachmentActionChosen(dataContext.ActionType);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/AttachPanel.xaml", UriKind.Relative));
      this.LayoutRoot = (StackPanel) this.FindName("LayoutRoot");
    }

    public enum ActionType
    {
      None,
      RecordVideo,
      TakePicture,
      TakePictureOrVideo,
      ChooseAudio,
      ChooseDocument,
      ChoosePicture,
      ChooseVideo,
      ChoosePictureAndVideo,
      ShareContact,
      ShareLocation,
    }

    public delegate void AttachmentActionChosenHandler(
      object sender,
      AttachPanel.ActionType actionChosen);
  }
}
