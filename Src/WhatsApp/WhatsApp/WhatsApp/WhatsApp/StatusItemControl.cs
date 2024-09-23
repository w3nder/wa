// Decompiled with JetBrains decompiler
// Type: WhatsApp.StatusItemControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class StatusItemControl : ItemControlBase
  {
    protected SegmentedCircle viewProgressCircle;
    protected RichTextBlock textPreview;

    protected override void InitComponents()
    {
      base.InitComponents();
      double num = 5.0;
      SegmentedCircle segmentedCircle = new SegmentedCircle();
      segmentedCircle.Radius = (double) (this.ItemHeight / 2) + num;
      segmentedCircle.Margin = new Thickness(-num, -num, 0.0, 0.0);
      segmentedCircle.Fill = (Brush) new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 100, (byte) 100, (byte) 100));
      segmentedCircle.Background = (Brush) UIUtils.AccentBrush;
      segmentedCircle.Visibility = Visibility.Collapsed;
      this.viewProgressCircle = segmentedCircle;
      Grid.SetRow((FrameworkElement) this.viewProgressCircle, 0);
      Grid.SetColumn((FrameworkElement) this.viewProgressCircle, 0);
      this.Children.Insert(0, (UIElement) this.viewProgressCircle);
      this.detailsPanel.Margin = new Thickness(18.0, 0.0, 0.0, 0.0);
    }

    protected void ToggleSelection()
    {
      StatusItemViewModel viewModel = this.ViewModel as StatusItemViewModel;
      viewModel.IsSelected = !viewModel.IsSelected;
    }

    protected override void UpdateComponents(JidItemViewModel vm)
    {
      base.UpdateComponents(vm);
      if (!(vm is StatusItemViewModel vm1))
        return;
      this.UpdateSegmentedCircle(vm1);
      this.UpdateTextPreview(vm1);
      if (vm1.ShowSubtitle)
      {
        this.UpdateSubtitleRow((JidItemViewModel) vm1);
        this.subtitleRow.Visibility = Visibility.Visible;
        this.titleRow.VerticalAlignment = VerticalAlignment.Stretch;
      }
      else
      {
        this.subtitleRow.Visibility = Visibility.Collapsed;
        this.titleRow.VerticalAlignment = VerticalAlignment.Center;
      }
      this.Opacity = vm1.IsDimmed ? 0.5 : 1.0;
    }

    protected void UpdateTextPreview(StatusItemViewModel vm)
    {
      Message msg = (Message) null;
      bool isTextStatus = false;
      if (vm?.Status != null)
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          msg = db.GetMessageById(vm.Status.MessageId);
          Message m = msg;
          isTextStatus = m != null && m.IsTextStatus();
        }));
      bool flag = false;
      if (isTextStatus)
      {
        double maxHeight = this.iconPanel.Height - 12.0;
        if (this.textPreview == null)
        {
          RichTextBlock richTextBlock = new RichTextBlock();
          richTextBlock.TextWrapping = TextWrapping.Wrap;
          richTextBlock.Foreground = (Brush) UIUtils.WhiteBrush;
          richTextBlock.VerticalAlignment = VerticalAlignment.Center;
          richTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
          richTextBlock.Margin = new Thickness(6.0, 0.0, 6.0, 0.0);
          richTextBlock.TextAlignment = TextAlignment.Center;
          this.textPreview = richTextBlock;
          AdaptiveRichTextBlockWrapper textBlockWrapper = new AdaptiveRichTextBlockWrapper(this.textPreview);
          textBlockWrapper.CacheMode = (CacheMode) new BitmapCache();
          textBlockWrapper.Margin = new Thickness(0.0);
          textBlockWrapper.MaxHeight = maxHeight;
          textBlockWrapper.VerticalAlignment = VerticalAlignment.Center;
          textBlockWrapper.HorizontalAlignment = HorizontalAlignment.Center;
          this.iconPanel.Children.Add((UIElement) textBlockWrapper);
        }
        uint? backgroundArgb = (uint?) msg.InternalProperties?.ExtendedTextPropertiesField?.BackgroundArgb;
        Color? nullable = new Color?();
        if (backgroundArgb.HasValue)
          nullable = new Color?(Color.FromArgb((byte) (backgroundArgb.Value >> 24 & (uint) byte.MaxValue), (byte) (backgroundArgb.Value >> 16 & (uint) byte.MaxValue), (byte) (backgroundArgb.Value >> 8 & (uint) byte.MaxValue), (byte) (backgroundArgb.Value & (uint) byte.MaxValue)));
        WaStatusViewControl.RenderTextStatus(this.textPreview, msg, msg.GetTextForDisplay(), nullable ?? Color.FromArgb((byte) 176, byte.MaxValue, byte.MaxValue, byte.MaxValue), 10.0, maxHeight, (Action<Message>) (_ => { }), (Action<string>) (_ => { }));
        this.textPreview.Visibility = Visibility.Visible;
        flag = true;
      }
      if (flag || this.textPreview == null)
        return;
      this.textPreview.Visibility = Visibility.Collapsed;
    }

    protected virtual void UpdateSegmentedCircle(StatusItemViewModel vm)
    {
      if (vm.ShowViewProgress)
      {
        if (vm is StatusThreadItemViewModel threadItemViewModel)
        {
          this.viewProgressCircle.SegmentCount = threadItemViewModel.Count;
          this.viewProgressCircle.FillCount = threadItemViewModel.ViewedCount;
        }
        this.viewProgressCircle.Visibility = Visibility.Visible;
      }
      else
        this.viewProgressCircle.Visibility = Visibility.Collapsed;
    }

    protected override void UpdateSubtitleRow(JidItemViewModel vm)
    {
      this.subtitleBlock.Foreground = vm.SubtitleBrush;
      this.subtitleBlock.Text = vm.GetSubtitle();
      base.UpdateSubtitleRow(vm);
    }
  }
}
