// Decompiled with JetBrains decompiler
// Type: WhatsApp.SelectableMessageListItem
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WhatsApp.Controls;


namespace WhatsApp
{
  public class SelectableMessageListItem : LongListMultiSelectorItem
  {
    private const string LayoutRootName = "LayoutRoot";
    private const string SelectBoxTranformName = "SelectBoxTransform";
    private Grid layoutRoot;
    private CompositeTransform selectBoxTransform;
    private bool visualStateGroupsAdjustmentAttempted;

    protected override double SideMargin => 24.0 * ResolutionHelper.ZoomMultiplier;

    private void AdjustVisualStateGroups()
    {
      if (this.layoutRoot == null || this.visualStateGroupsAdjustmentAttempted)
        return;
      this.visualStateGroupsAdjustmentAttempted = true;
      if (!(VisualStateManager.GetVisualStateGroups((FrameworkElement) this.layoutRoot)[0] is VisualStateGroup visualStateGroup))
        return;
      VisualTransition[] array = visualStateGroup.Transitions.Cast<VisualTransition>().ToArray<VisualTransition>();
      double sideMargin = this.SideMargin;
      VisualTransition visualTransition1 = ((IEnumerable<VisualTransition>) array).ElementAtOrDefault<VisualTransition>(2);
      if (visualTransition1 != null)
      {
        Storyboard storyboard = visualTransition1.Storyboard;
        if (storyboard != null && storyboard.Children.ElementAtOrDefault<Timeline>(1) is DoubleAnimationUsingKeyFrames animationUsingKeyFrames && animationUsingKeyFrames.KeyFrames[0] is EasingDoubleKeyFrame keyFrame)
          keyFrame.Value = sideMargin;
      }
      VisualTransition visualTransition2 = ((IEnumerable<VisualTransition>) array).ElementAtOrDefault<VisualTransition>(3);
      if (visualTransition2 != null)
      {
        Storyboard storyboard = visualTransition2.Storyboard;
        if (storyboard != null && storyboard.Children.ElementAtOrDefault<Timeline>(1) is DoubleAnimationUsingKeyFrames animationUsingKeyFrames && animationUsingKeyFrames.KeyFrames[0] is EasingDoubleKeyFrame keyFrame)
          keyFrame.Value = sideMargin;
      }
      VisualTransition visualTransition3 = ((IEnumerable<VisualTransition>) array).ElementAtOrDefault<VisualTransition>(4);
      if (visualTransition3 == null)
        return;
      Storyboard storyboard1 = visualTransition3.Storyboard;
      if (storyboard1 == null || !(storyboard1.Children.ElementAtOrDefault<Timeline>(1) is DoubleAnimationUsingKeyFrames animationUsingKeyFrames1) || !(animationUsingKeyFrames1.KeyFrames[1] is EasingDoubleKeyFrame keyFrame1))
        return;
      keyFrame1.Value = sideMargin;
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.layoutRoot = this.GetTemplateChild("LayoutRoot") as Grid;
      this.selectBoxTransform = this.GetTemplateChild("SelectBoxTransform") as CompositeTransform;
      this.AdjustVisualStateGroups();
      double sideMargin = this.SideMargin;
      if (this.presenter != null)
      {
        this.presenter.Margin = new Thickness(0.0, 0.0, sideMargin, 0.0);
        if (this.presenter.RenderTransform is CompositeTransform renderTransform)
          renderTransform.TranslateX = sideMargin;
      }
      if (this.innerHintPanel != null)
      {
        this.innerHintPanel.Width = sideMargin;
        if (this.innerHintPanel.RenderTransform is CompositeTransform renderTransform)
          renderTransform.TranslateX = sideMargin;
      }
      if (this.outerHintPanel == null)
        return;
      this.outerHintPanel.Width = sideMargin;
    }

    public void OnItemRealized(MessageViewModel vm)
    {
      if (vm == null)
        return;
      if (vm.IsSelectable)
      {
        this.SelectionMode = LongListMultiSelectorItem.SelectionModes.Normal;
        if (this.selectBoxTransform != null)
          this.selectBoxTransform.TranslateY = vm.Message.KeyFromMe ? 9.0 : 9.0;
        if (MessageViewModel.IsOverWallpaper)
          this.selectBox.Foreground = (Brush) UIUtils.WhiteBrush;
        else
          this.selectBox.Foreground = (Brush) UIUtils.ForegroundBrush;
        bool flag = true;
        if (vm.Message.MediaWaType == FunXMPP.FMessage.Type.Undefined)
          flag = vm.MergedPosition == MessageViewModel.GroupingPosition.None || vm.MergedPosition == MessageViewModel.GroupingPosition.Top;
        if (this.selectBox == null)
          return;
        this.selectBox.Opacity = flag ? 1.0 : 0.0;
        this.selectBox.IsHitTestVisible = flag;
      }
      else
        this.SelectionMode = LongListMultiSelectorItem.SelectionModes.Hidden;
    }
  }
}
