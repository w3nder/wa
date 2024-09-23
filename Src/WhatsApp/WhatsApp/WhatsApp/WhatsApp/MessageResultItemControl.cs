// Decompiled with JetBrains decompiler
// Type: WhatsApp.MessageResultItemControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WhatsApp.WaCollections;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class MessageResultItemControl : ItemControlBase
  {
    protected TextBlock timestampBlock;
    protected Image mediaIcon;
    protected TextBlock starBlock;

    protected override void InitComponents()
    {
      base.InitComponents();
      this.ShowIconPanel(false);
      double maxHeight = this.subtitleRow.MaxHeight;
      this.detailsPanel.MaxHeight += maxHeight;
      this.subtitleRow.MaxHeight += maxHeight;
      this.subtitleBlock.TextWrapping = TextWrapping.Wrap;
      this.titleRow.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Auto)
      });
      TextBlock textBlock = new TextBlock();
      textBlock.HorizontalAlignment = HorizontalAlignment.Right;
      textBlock.VerticalAlignment = VerticalAlignment.Bottom;
      textBlock.FontSize = UIUtils.FontSizeSmall;
      textBlock.Margin = new Thickness(18.0, 0.0, 0.0, 4.0);
      textBlock.Style = UIUtils.TextStyleNormal;
      this.timestampBlock = textBlock;
      this.titleRow.Children.Add((UIElement) this.timestampBlock);
      Grid.SetColumn((FrameworkElement) this.timestampBlock, 1);
      this.subtitleRow.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Auto)
      });
      this.subtitleRow.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Star)
      });
      this.subtitleRow.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Auto)
      });
      Grid.SetColumn((FrameworkElement) this.subtitleBlock, 1);
    }

    protected override void UpdateComponents(JidItemViewModel vm)
    {
      base.UpdateComponents(vm);
      this.subtitleBlock.Foreground = vm.SubtitleBrush;
      this.subtitleBlock.FontWeight = vm.SubtitleWeight;
      if (!(vm is MessageResultViewModel messageResultViewModel))
        return;
      if (messageResultViewModel.ShowTimestamp)
      {
        this.timestampBlock.Text = messageResultViewModel.TimestampStr;
        this.timestampBlock.Visibility = Visibility.Visible;
      }
      else
        this.timestampBlock.Visibility = Visibility.Collapsed;
      this.subtitleRow.Opacity = 1.0;
      this.UpdateSubtitleRowImpl(this.ViewModel as MessageResultViewModel);
    }

    private void UpdateSubtitleRowImpl(MessageResultViewModel vm)
    {
      if (vm == null)
        return;
      if (vm.ShowMediaIcon)
      {
        if (this.mediaIcon == null)
        {
          Image image = new Image();
          image.Margin = new Thickness(0.0, 2.0, 6.0, -2.0);
          image.Height = 24.0;
          image.Stretch = Stretch.UniformToFill;
          this.mediaIcon = image;
          this.subtitleRow.Children.Add((UIElement) this.mediaIcon);
          Grid.SetColumn((FrameworkElement) this.mediaIcon, 0);
        }
        this.mediaIcon.Visibility = Visibility.Visible;
        this.mediaIcon.Source = vm.MediaIconSource;
      }
      else if (this.mediaIcon != null)
        this.mediaIcon.Visibility = Visibility.Collapsed;
      StringBuilder stringBuilder = new StringBuilder();
      if (vm.ShowSender)
        stringBuilder.Append(Emoji.ConvertToUnicode(vm.SenderStr));
      int offsetFromSender = stringBuilder.Length;
      Pair<int, int>[] source = (Pair<int, int>[]) null;
      if (vm.SearchResult.DataOffsets != null)
      {
        stringBuilder.Append(Emoji.ConvertToUnicode(vm.SearchResult.Message.Data));
        source = vm.SearchResult.DataOffsets;
      }
      else if (vm.SearchResult.MediaNameOffsets != null)
      {
        stringBuilder.Append(Emoji.ConvertToUnicode(vm.SearchResult.Message.MediaName));
        source = vm.SearchResult.MediaNameOffsets;
      }
      else if (vm.SearchResult.MediaCaptionOffsets != null)
      {
        stringBuilder.Append(Emoji.ConvertToUnicode(vm.SearchResult.Message.MediaCaption));
        source = vm.SearchResult.MediaCaptionOffsets;
      }
      else if (vm.SearchResult.LocationDetailsOffsets != null)
      {
        stringBuilder.Append(Emoji.ConvertToUnicode(vm.SearchResult.Message.LocationDetails));
        source = vm.SearchResult.LocationDetailsOffsets;
      }
      else
        stringBuilder.Append(vm.GetSubtitle()?.Text ?? "");
      string str = stringBuilder.ToString();
      WaRichText.Chunk[] chunkArray = (WaRichText.Chunk[]) null;
      if (source != null)
        chunkArray = ((IEnumerable<Pair<int, int>>) source).Select<Pair<int, int>, WaRichText.Chunk>((Func<Pair<int, int>, WaRichText.Chunk>) (p => new WaRichText.Chunk(p.First + offsetFromSender, p.Second, WaRichText.Formats.Foreground, UIUtils.AccentColorCode))).ToArray<WaRichText.Chunk>();
      LinkDetector.Result[] array = ((IEnumerable<LinkDetector.Result>) (vm.SearchResult.Message.GetRichTextFormattings() ?? new LinkDetector.Result[0])).Select<LinkDetector.Result, LinkDetector.Result>((Func<LinkDetector.Result, LinkDetector.Result>) (chunk => new LinkDetector.Result(chunk)
      {
        Index = chunk.Index + offsetFromSender,
        OriginalStr = str
      })).ToArray<LinkDetector.Result>();
      this.subtitleBlock.Text = new RichTextBlock.TextSet()
      {
        Text = str,
        SerializedFormatting = (IEnumerable<LinkDetector.Result>) array,
        PartialFormattings = (IEnumerable<WaRichText.Chunk>) chunkArray
      };
      if (vm.SearchResult.Message.IsStarred)
      {
        if (this.starBlock == null)
        {
          TextBlock textBlock = new TextBlock();
          textBlock.FontSize = 22.0;
          textBlock.Text = "★";
          textBlock.VerticalAlignment = VerticalAlignment.Top;
          textBlock.Margin = new Thickness(12.0, 4.0, 0.0, 0.0);
          this.starBlock = textBlock;
          this.subtitleRow.Children.Add((UIElement) this.starBlock);
          Grid.SetColumn((FrameworkElement) this.starBlock, 2);
        }
        this.starBlock.Visibility = Visibility.Visible;
      }
      else
      {
        if (this.starBlock == null)
          return;
        this.starBlock.Visibility = Visibility.Collapsed;
      }
    }
  }
}
