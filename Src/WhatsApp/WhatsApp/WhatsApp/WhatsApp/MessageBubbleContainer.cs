// Decompiled with JetBrains decompiler
// Type: WhatsApp.MessageBubbleContainer
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;


namespace WhatsApp
{
  public class MessageBubbleContainer : Grid
  {
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.RegisterAttached(nameof (ViewModel), typeof (object), typeof (MessageBubbleContainer), new PropertyMetadata((object) null, new PropertyChangedCallback(MessageBubbleContainer.OnViewModelChanged)));
    protected MessageViewModel viewModel;
    private IDisposable vmSub;
    private ContextMenu contextMenu;
    protected MessageBubble msgBubble;
    private ColumnDefinition firstCol;
    private bool isSelectable = true;

    private string LogHeader
    {
      get
      {
        return string.Format("msgbubcont > curr key:{0}", this.viewModel == null ? (object) "n/a" : (object) this.viewModel.Message.KeyId);
      }
    }

    public object ViewModel
    {
      get => this.GetValue(MessageBubbleContainer.ViewModelProperty);
      set => this.SetValue(MessageBubbleContainer.ViewModelProperty, value);
    }

    public bool IsSelectable
    {
      get => this.isSelectable;
      set
      {
        if (this.isSelectable == value)
          return;
        this.isSelectable = value;
        double num = 24.0 * ResolutionHelper.ZoomMultiplier;
        this.firstCol.Width = new GridLength(this.isSelectable ? 0.0 : num, GridUnitType.Pixel);
      }
    }

    public MessageBubbleContainer()
    {
      double num = 24.0 * ResolutionHelper.ZoomMultiplier;
      ColumnDefinitionCollection columnDefinitions = this.ColumnDefinitions;
      ColumnDefinition columnDefinition1 = new ColumnDefinition();
      columnDefinition1.Width = new GridLength(this.IsSelectable ? 0.0 : num, GridUnitType.Pixel);
      ColumnDefinition columnDefinition2 = columnDefinition1;
      this.firstCol = columnDefinition1;
      ColumnDefinition columnDefinition3 = columnDefinition2;
      columnDefinitions.Add(columnDefinition3);
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(0.25, GridUnitType.Star)
      });
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(0.5, GridUnitType.Star)
      });
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(0.25, GridUnitType.Star)
      });
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(num, GridUnitType.Pixel)
      });
      this.msgBubble = new MessageBubble();
      this.Children.Add((UIElement) this.msgBubble);
      this.contextMenu = new ContextMenu()
      {
        IsZoomEnabled = false
      };
      this.contextMenu.Opened += new RoutedEventHandler(this.ContextMenu_Opened);
      this.contextMenu.Closed += new RoutedEventHandler(this.ContextMenu_Closed);
    }

    ~MessageBubbleContainer() => this.Cleanup();

    protected virtual void Cleanup()
    {
      this.vmSub.SafeDispose();
      this.vmSub = (IDisposable) null;
      if (this.msgBubble == null)
        return;
      this.msgBubble.Cleanup();
    }

    public void Refresh() => this.Render(this.viewModel, true);

    protected virtual void Render(MessageViewModel vm, bool forceUpdate = false)
    {
      if (vm == null)
        Log.l(this.LogHeader, "render | skip | null view model");
      else if (!forceUpdate && vm == this.viewModel)
      {
        Log.l(this.LogHeader, "render | skip | same view model");
      }
      else
      {
        if (vm.Message.MediaWaType == FunXMPP.FMessage.Type.System)
        {
          Grid.SetColumn((FrameworkElement) this.msgBubble, 1);
          Grid.SetColumnSpan((FrameworkElement) this.msgBubble, 3);
        }
        else if (vm.Message.MediaWaType == FunXMPP.FMessage.Type.Divider)
        {
          Grid.SetColumn((FrameworkElement) this.msgBubble, 0);
          Grid.SetColumnSpan((FrameworkElement) this.msgBubble, 5);
        }
        else
        {
          Grid.SetColumnSpan((FrameworkElement) this.msgBubble, 2);
          if (vm.IsPsaChat)
          {
            if (this.ColumnDefinitions.Count <= 5)
              this.ColumnDefinitions.Insert(2, new ColumnDefinition()
              {
                Width = new GridLength(1.0, GridUnitType.Star)
              });
            Grid.SetColumn((FrameworkElement) this.msgBubble, 2);
          }
          else if (vm.ShouldShowOnOutgoingSide)
          {
            Grid.SetColumn((FrameworkElement) this.msgBubble, 2);
          }
          else
          {
            Grid.SetColumn((FrameworkElement) this.msgBubble, 1);
            Grid.SetColumnSpan((FrameworkElement) this.msgBubble, 2);
          }
        }
        try
        {
          this.msgBubble.Render(vm);
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "render msg bubble");
        }
        this.vmSub.SafeDispose();
        this.vmSub = vm.GetObservable().ObserveOnDispatcherIfNeeded<KeyValuePair<string, object>>().Subscribe<KeyValuePair<string, object>>(new Action<KeyValuePair<string, object>>(this.OnViewModelNotified));
        Thickness bubbleMargin = vm.BubbleMargin;
        if (vm.Message.MediaWaType == FunXMPP.FMessage.Type.System && this.IsSelectable)
          bubbleMargin.Left += 24.0 * ResolutionHelper.ZoomMultiplier;
        this.msgBubble.Margin = bubbleMargin;
        this.viewModel = vm;
        if (vm.EnableContextMenu)
          ContextMenuService.SetContextMenu((DependencyObject) this.msgBubble, this.contextMenu);
        else
          ContextMenuService.SetContextMenu((DependencyObject) this.msgBubble, (ContextMenu) null);
      }
    }

    public static void OnViewModelChanged(
      DependencyObject sender,
      DependencyPropertyChangedEventArgs args)
    {
      if (!(sender is MessageBubbleContainer messageBubbleContainer))
        return;
      try
      {
        messageBubbleContainer.Render(args.NewValue as MessageViewModel);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "render new view model");
      }
    }

    protected virtual void OnViewModelNotified(KeyValuePair<string, object> p)
    {
      this.msgBubble.ProcessViewModelNotification(p);
    }

    private void ContextMenu_Opened(object sender, EventArgs e)
    {
      this.contextMenu.ItemsSource = (IEnumerable) MessageMenu.GetMessageMenuItems(this.viewModel);
    }

    private void ContextMenu_Closed(object sender, EventArgs e)
    {
      this.contextMenu.ItemsSource = (IEnumerable) null;
    }
  }
}
