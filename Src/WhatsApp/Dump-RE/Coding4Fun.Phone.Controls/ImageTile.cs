// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.ImageTile
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  public class ImageTile : Button
  {
    private readonly DispatcherTimer _changeImageTimer = new DispatcherTimer();
    private readonly Random _rand = new Random();
    private readonly Dictionary<int, Uri> _imageCurrentLocation = new Dictionary<int, Uri>();
    private readonly List<Uri> _imagesBeingShown = new List<Uri>();
    private readonly List<int> _availableSpotsOnGrid = new List<int>();
    private readonly List<ImageTileState> _animationTracking = new List<ImageTileState>();
    private int _largeImageIndex = -1;
    private bool _createAnimation = true;
    private ImageTileLayoutStates _imageTileLayoutState;
    private Grid _imageContainer;
    public static readonly DependencyProperty ColumnProperty = DependencyProperty.Register(nameof (Columns), typeof (int), typeof (ImageTile), new PropertyMetadata((object) 3, new PropertyChangedCallback(ImageTile.OnGridSizeChanged)));
    public static readonly DependencyProperty RowsProperty = DependencyProperty.Register(nameof (Rows), typeof (int), typeof (ImageTile), new PropertyMetadata((object) 3, new PropertyChangedCallback(ImageTile.OnGridSizeChanged)));
    public static readonly DependencyProperty LargeTileColumnsProperty = DependencyProperty.Register(nameof (LargeTileColumns), typeof (int), typeof (ImageTile), new PropertyMetadata((object) 2, new PropertyChangedCallback(ImageTile.OnLargeTileChanged)));
    public static readonly DependencyProperty LargeTileRowsProperty = DependencyProperty.Register(nameof (LargeTileRows), typeof (int), typeof (ImageTile), new PropertyMetadata((object) 2, new PropertyChangedCallback(ImageTile.OnLargeTileChanged)));
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof (ItemsSource), typeof (List<Uri>), typeof (ImageTile), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty AnimationTypesProperty = DependencyProperty.Register(nameof (AnimationType), typeof (ImageTileAnimationTypes), typeof (ImageTile), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty IsFrozenProperty = DependencyProperty.Register(nameof (IsFrozen), typeof (bool), typeof (ImageTile), new PropertyMetadata((object) false, new PropertyChangedCallback(ImageTile.OnIsFrozenPropertyChanged)));
    public static readonly DependencyProperty AnimationDurationProperty = DependencyProperty.Register(nameof (AnimationDuration), typeof (int), typeof (ImageTile), new PropertyMetadata((object) 500));
    public static readonly DependencyProperty ImageCycleIntervalProperty = DependencyProperty.Register(nameof (ImageCycleInterval), typeof (int), typeof (ImageTile), new PropertyMetadata((object) 1000, new PropertyChangedCallback(ImageTile.OnImageCycleIntervalPropertyChanged)));

    public event EventHandler<ExceptionRoutedEventArgs> ImageFailed;

    public ImageTile() => ((Control) this).DefaultStyleKey = (object) typeof (ImageTile);

    public virtual void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this._imageContainer = (Grid) ((Control) this).GetTemplateChild("ImageContainer");
      this.GridSizeChanged();
      this.ResetGridStateManagement();
      this._createAnimation = false;
      if (!DesignerProperties.IsInDesignTool)
      {
        for (int row = 0; row < this.Rows; ++row)
        {
          for (int col = 0; col < this.Columns; ++col)
            this.CycleImage(row, col);
        }
      }
      this._createAnimation = true;
      this.ImageCycleIntervalChanged();
      this.IsFrozenPropertyChanged();
      this._changeImageTimer.Tick += new EventHandler(this.ChangeImageTimerTick);
    }

    private int CalculateIndex(int row, int col) => row * this.Columns + col;

    private void ChangeImageTimerTick(object sender, EventArgs e) => this.CycleImage();

    public void CycleImage(int row = -1, int col = -1)
    {
      if (this._imageContainer == null || this.ItemsSource == null || this.ItemsSource.Count <= 0)
        return;
      int index;
      bool isLargeImage;
      this.CalculateNextValidItem(out index, ref row, ref col, out isLargeImage);
      Image image = this.CreateImage(row, col, index, isLargeImage);
      ((PresentationFrameworkCollection<UIElement>) ((Panel) this._imageContainer).Children).Add((UIElement) image);
      if (!this._createAnimation || this.AnimationType == ImageTileAnimationTypes.None)
        return;
      Storyboard sb = new Storyboard();
      this.TrackAnimationForImageRemoval(row, col, sb, isLargeImage);
      switch (this.AnimationType)
      {
        case ImageTileAnimationTypes.Fade:
          ImageTile.CreateDoubleAnimations(sb, (DependencyObject) image, "Opacity", toValue: 1.0, speed: this.AnimationDuration);
          break;
        case ImageTileAnimationTypes.HorizontalExpand:
          ((UIElement) image).Projection = (Projection) new PlaneProjection();
          ImageTile.CreateDoubleAnimations(sb, (DependencyObject) ((UIElement) image).Projection, "RotationY", 270.0, 360.0, this.AnimationDuration);
          break;
        case ImageTileAnimationTypes.VerticalExpand:
          ((UIElement) image).Projection = (Projection) new PlaneProjection();
          ImageTile.CreateDoubleAnimations(sb, (DependencyObject) ((UIElement) image).Projection, "RotationX", 270.0, 360.0, this.AnimationDuration);
          break;
      }
      ((Timeline) sb).Completed += new EventHandler(this.AnimationCompleted);
      sb.Begin();
    }

    private void CalculateNextValidItem(
      out int index,
      ref int row,
      ref int col,
      out bool isLargeImage)
    {
      isLargeImage = false;
      if (row == -1 && col == -1)
      {
        if (this._availableSpotsOnGrid.Count == 0)
        {
          this.ResetGridStateManagement();
          isLargeImage = this._imageTileLayoutState == ImageTileLayoutStates.BigImage;
        }
        List<int> list = this._availableSpotsOnGrid.Where<int>(new Func<int, bool>(this.IsValidLargeTilePosition)).ToList<int>();
        List<int> intList = isLargeImage ? list : this._availableSpotsOnGrid;
        int index1 = this._rand.Next(0, intList.Count);
        index = intList[index1];
        this.GetRowAndColumnForIndex(index, out row, out col);
      }
      else
        index = this.CalculateIndex(row, col);
      if (isLargeImage)
      {
        this._largeImageIndex = index;
        for (int index2 = 0; index2 < this.LargeTileRows; ++index2)
        {
          for (int index3 = 0; index3 < this.LargeTileColumns; ++index3)
            this._availableSpotsOnGrid.Remove(this.CalculateIndex(row + index2, col + index3));
        }
      }
      else
        this._availableSpotsOnGrid.Remove(index);
    }

    private bool IsValidLargeTilePosition(int index)
    {
      int row;
      int column;
      this.GetRowAndColumnForIndex(index, out row, out column);
      return column <= this.Columns - this.LargeTileColumns && row <= this.Rows - this.LargeTileRows;
    }

    private void ResetGridStateManagement()
    {
      if (this._availableSpotsOnGrid.Count != 0)
        return;
      bool isEnabled = this._changeImageTimer.IsEnabled;
      this._changeImageTimer.Stop();
      this.AlterCycleState();
      if (this._imageTileLayoutState == ImageTileLayoutStates.ForceOverwriteOfBigImage)
      {
        this._availableSpotsOnGrid.Add(this._largeImageIndex);
      }
      else
      {
        int num = this.Rows * this.Columns;
        for (int index = 0; index < num; ++index)
          this._availableSpotsOnGrid.Add(index);
        if (this._imageTileLayoutState == ImageTileLayoutStates.AllButBigImage)
          this._availableSpotsOnGrid.Remove(this._largeImageIndex);
      }
      if (!isEnabled)
        return;
      this._changeImageTimer.Start();
    }

    private void AlterCycleState()
    {
      switch (this._imageTileLayoutState)
      {
        case ImageTileLayoutStates.AllImages:
          this._imageTileLayoutState = ImageTileLayoutStates.BigImage;
          break;
        case ImageTileLayoutStates.BigImage:
          this._imageTileLayoutState = ImageTileLayoutStates.AllButBigImage;
          break;
        case ImageTileLayoutStates.AllButBigImage:
          this._imageTileLayoutState = ImageTileLayoutStates.ForceOverwriteOfBigImage;
          break;
        default:
          this._imageTileLayoutState = ImageTileLayoutStates.AllImages;
          break;
      }
    }

    private Image CreateImage(int row, int col, int index, bool isLargeImage)
    {
      Uri randomImageUri = this.GetRandomImageUri(index);
      Image image1 = new Image();
      ((FrameworkElement) image1).HorizontalAlignment = (HorizontalAlignment) 1;
      ((FrameworkElement) image1).VerticalAlignment = (VerticalAlignment) 1;
      image1.Stretch = (Stretch) 3;
      ((FrameworkElement) image1).Name = Guid.NewGuid().ToString();
      Image image2 = image1;
      ((DependencyObject) image2).SetValue(Grid.ColumnProperty, (object) col);
      ((DependencyObject) image2).SetValue(Grid.RowProperty, (object) row);
      if (isLargeImage)
      {
        ((DependencyObject) image2).SetValue(Grid.ColumnSpanProperty, (object) this.LargeTileColumns);
        ((DependencyObject) image2).SetValue(Grid.RowSpanProperty, (object) this.LargeTileRows);
      }
      image2.Source = (ImageSource) this.GetImage(randomImageUri);
      return image2;
    }

    private void TrackAnimationForImageRemoval(
      int row,
      int col,
      Storyboard sb,
      bool forceLargeImageCleanup)
    {
      this._animationTracking.Add(new ImageTileState()
      {
        Storyboard = sb,
        Row = row,
        Column = col,
        ForceLargeImageCleanup = forceLargeImageCleanup
      });
    }

    private static void CreateDoubleAnimations(
      Storyboard sb,
      DependencyObject target,
      string propertyPath,
      double fromValue = 0.0,
      double toValue = 0.0,
      int speed = 500)
    {
      DoubleAnimation doubleAnimation1 = new DoubleAnimation();
      doubleAnimation1.To = new double?(toValue);
      doubleAnimation1.From = new double?(fromValue);
      ((Timeline) doubleAnimation1).Duration = new Duration(TimeSpan.FromMilliseconds((double) speed));
      DoubleAnimation doubleAnimation2 = doubleAnimation1;
      Storyboard.SetTarget((Timeline) doubleAnimation2, target);
      Storyboard.SetTargetProperty((Timeline) doubleAnimation2, new PropertyPath(propertyPath, new object[0]));
      ((PresentationFrameworkCollection<Timeline>) sb.Children).Add((Timeline) doubleAnimation2);
    }

    private void AnimationCompleted(object sender, EventArgs e)
    {
      Storyboard itemStoryboard = sender as Storyboard;
      ImageTileState imageTileState = this._animationTracking.FirstOrDefault<ImageTileState>((Func<ImageTileState, bool>) (x => x.Storyboard == itemStoryboard));
      if (imageTileState.ForceLargeImageCleanup)
      {
        for (int index1 = 0; index1 < this.LargeTileRows; ++index1)
        {
          for (int index2 = 0; index2 < this.LargeTileColumns; ++index2)
          {
            if (index1 != 0 || index2 != 0)
              this.RemoveOldImagesFromGrid(index1 + imageTileState.Row, index2 + imageTileState.Column, true);
          }
        }
      }
      this.RemoveOldImagesFromGrid(imageTileState.Row, imageTileState.Column);
      this._animationTracking.Remove(imageTileState);
    }

    private void RemoveOldImagesFromGrid(int row, int col, bool forceRemoval = false)
    {
      UIElement[] array = ((IEnumerable<UIElement>) ((Panel) this._imageContainer).Children).Where<UIElement>((Func<UIElement, bool>) (x => (int) ((DependencyObject) x).GetValue(Grid.RowProperty) == row && (int) ((DependencyObject) x).GetValue(Grid.ColumnProperty) == col)).ToArray<UIElement>();
      int num = forceRemoval ? 0 : 1;
      for (int index = 0; index < array.Length - num; ++index)
      {
        if (array[index] is Image image)
        {
          if (image.Source is BitmapImage source)
            this._imagesBeingShown.Remove(source.UriSource);
          ((PresentationFrameworkCollection<UIElement>) ((Panel) this._imageContainer).Children).Remove((UIElement) image);
        }
      }
    }

    private Uri GetRandomImageUri(int index)
    {
      int count = this.ItemsSource.Count;
      int maxAvailableSlots = this.Rows * this.Columns;
      int maxLoopCounter = 0;
      Uri imgUri;
      do
      {
        imgUri = this.ItemsSource[this._rand.Next(count)];
        ++maxLoopCounter;
      }
      while (this.AllowRandomImageFetchToContinue(index, maxAvailableSlots, count, maxLoopCounter, imgUri));
      this._imageCurrentLocation[index] = imgUri;
      this._imagesBeingShown.Add(imgUri);
      return imgUri;
    }

    private void GetRowAndColumnForIndex(int index, out int row, out int column)
    {
      column = index % this.Columns;
      row = (index - column) / this.Rows;
    }

    private bool AllowRandomImageFetchToContinue(
      int targetIndex,
      int maxAvailableSlots,
      int imageSourceCount,
      int maxLoopCounter,
      Uri imgUri)
    {
      if (maxLoopCounter >= 10)
        return false;
      if (imageSourceCount > maxAvailableSlots)
        return this._imageCurrentLocation.ContainsValue(imgUri);
      return this._imageCurrentLocation.ContainsKey(targetIndex) && this._imageCurrentLocation[targetIndex] == imgUri || this._imagesBeingShown.Contains(imgUri);
    }

    private BitmapImage GetImage(Uri file)
    {
      BitmapImage image = new BitmapImage(file);
      image.ImageFailed += new EventHandler<ExceptionRoutedEventArgs>(this.ImageLoadFail);
      return image;
    }

    private void ImageLoadFail(object sender, ExceptionRoutedEventArgs e)
    {
      if (this.ImageFailed == null)
        return;
      this.ImageFailed(sender, e);
    }

    public int Columns
    {
      get => (int) ((DependencyObject) this).GetValue(ImageTile.ColumnProperty);
      set => ((DependencyObject) this).SetValue(ImageTile.ColumnProperty, (object) value);
    }

    public int Rows
    {
      get => (int) ((DependencyObject) this).GetValue(ImageTile.RowsProperty);
      set => ((DependencyObject) this).SetValue(ImageTile.RowsProperty, (object) value);
    }

    public int LargeTileColumns
    {
      get => (int) ((DependencyObject) this).GetValue(ImageTile.LargeTileColumnsProperty);
      set => ((DependencyObject) this).SetValue(ImageTile.LargeTileColumnsProperty, (object) value);
    }

    public int LargeTileRows
    {
      get => (int) ((DependencyObject) this).GetValue(ImageTile.LargeTileRowsProperty);
      set => ((DependencyObject) this).SetValue(ImageTile.LargeTileRowsProperty, (object) value);
    }

    public List<Uri> ItemsSource
    {
      get => (List<Uri>) ((DependencyObject) this).GetValue(ImageTile.ItemsSourceProperty);
      set => ((DependencyObject) this).SetValue(ImageTile.ItemsSourceProperty, (object) value);
    }

    public ImageTileAnimationTypes AnimationType
    {
      get
      {
        return (ImageTileAnimationTypes) ((DependencyObject) this).GetValue(ImageTile.AnimationTypesProperty);
      }
      set => ((DependencyObject) this).SetValue(ImageTile.AnimationTypesProperty, (object) value);
    }

    public bool IsFrozen
    {
      get => (bool) ((DependencyObject) this).GetValue(ImageTile.IsFrozenProperty);
      set => ((DependencyObject) this).SetValue(ImageTile.IsFrozenProperty, (object) value);
    }

    public int AnimationDuration
    {
      get => (int) ((DependencyObject) this).GetValue(ImageTile.AnimationDurationProperty);
      set
      {
        ((DependencyObject) this).SetValue(ImageTile.AnimationDurationProperty, (object) value);
      }
    }

    public int ImageCycleInterval
    {
      get => (int) ((DependencyObject) this).GetValue(ImageTile.ImageCycleIntervalProperty);
      set
      {
        ((DependencyObject) this).SetValue(ImageTile.ImageCycleIntervalProperty, (object) value);
      }
    }

    private static void OnIsFrozenPropertyChanged(
      DependencyObject dependencyObject,
      DependencyPropertyChangedEventArgs args)
    {
      if (!(dependencyObject is ImageTile imageTile) || imageTile._changeImageTimer == null)
        return;
      imageTile.IsFrozenPropertyChanged();
    }

    private void IsFrozenPropertyChanged()
    {
      if (this.IsFrozen)
        this._changeImageTimer.Stop();
      else
        this._changeImageTimer.Start();
    }

    private static void OnImageCycleIntervalPropertyChanged(
      DependencyObject dependencyObject,
      DependencyPropertyChangedEventArgs args)
    {
      if (!(dependencyObject is ImageTile imageTile) || imageTile._changeImageTimer == null)
        return;
      imageTile.ImageCycleIntervalChanged();
    }

    private void ImageCycleIntervalChanged()
    {
      bool isEnabled = this._changeImageTimer.IsEnabled;
      this._changeImageTimer.Stop();
      this._changeImageTimer.Interval = TimeSpan.FromMilliseconds((double) this.ImageCycleInterval);
      if (!isEnabled)
        return;
      this._changeImageTimer.Start();
    }

    private static void OnLargeTileChanged(
      DependencyObject dependencyObject,
      DependencyPropertyChangedEventArgs args)
    {
      if (!(dependencyObject is ImageTile imageTile) || args.NewValue == args.OldValue)
        return;
      imageTile.VerifyGridBounds();
    }

    private static void OnGridSizeChanged(
      DependencyObject dependencyObject,
      DependencyPropertyChangedEventArgs args)
    {
      if (!(dependencyObject is ImageTile imageTile) || args.NewValue == args.OldValue)
        return;
      imageTile.VerifyGridBounds();
      imageTile.GridSizeChanged();
    }

    private void GridSizeChanged()
    {
      if (this._imageContainer == null)
        return;
      int count1 = ((PresentationFrameworkCollection<ColumnDefinition>) this._imageContainer.ColumnDefinitions).Count;
      int count2 = ((PresentationFrameworkCollection<RowDefinition>) this._imageContainer.RowDefinitions).Count;
      if (count1 > this.Columns)
      {
        for (int col = count1 - 1; col >= this.Columns; --col)
        {
          ((PresentationFrameworkCollection<ColumnDefinition>) this._imageContainer.ColumnDefinitions).RemoveAt(col);
          this.KeepGridInSyncCol(col);
        }
      }
      else if (count1 < this.Columns)
      {
        for (int index = 0; index < this.Columns - count1; ++index)
          ((PresentationFrameworkCollection<ColumnDefinition>) this._imageContainer.ColumnDefinitions).Add(new ColumnDefinition());
      }
      if (count2 > this.Rows)
      {
        for (int row = count2 - 1; row >= this.Rows; --row)
        {
          ((PresentationFrameworkCollection<RowDefinition>) this._imageContainer.RowDefinitions).RemoveAt(row);
          this.KeepGridInSyncRow(row);
        }
      }
      else
      {
        if (count2 >= this.Rows)
          return;
        for (int index = 0; index < this.Rows - count2; ++index)
          ((PresentationFrameworkCollection<RowDefinition>) this._imageContainer.RowDefinitions).Add(new RowDefinition());
      }
    }

    private void VerifyGridBounds()
    {
      if (this.Rows < 1)
        throw new ArgumentOutOfRangeException("Rows", "Rows must be greater than 0");
      if (this.Columns < 1)
        throw new ArgumentOutOfRangeException("Columns", "Columns must be greater than 0");
      if (this.LargeTileRows < 1)
        throw new ArgumentOutOfRangeException("LargeTileRows", "LargeTileRows must be greater than 0");
      if (this.LargeTileRows > this.Rows)
        throw new ArgumentOutOfRangeException("LargeTileRows", "LargeTileRows must be less than or equal to Rows");
      if (this.LargeTileColumns < 1)
        throw new ArgumentOutOfRangeException("LargeTileColumns", "LargeTileColumns must be greater than 0");
      if (this.LargeTileColumns > this.Columns)
        throw new ArgumentOutOfRangeException("LargeTileColumns", "LargeTileColumns must be less than or equal to Columns");
    }

    private void KeepGridInSyncRow(int row)
    {
      for (int col = 0; col < this.Columns; ++col)
        this.KeepGridInSync(row, col);
    }

    private void KeepGridInSyncCol(int col)
    {
      for (int row = 0; row < this.Rows; ++row)
        this.KeepGridInSync(row, col);
    }

    private void KeepGridInSync(int row, int col)
    {
      int index = this.CalculateIndex(row, col);
      Uri uri;
      if (this._imageCurrentLocation.TryGetValue(index, out uri))
      {
        this._imagesBeingShown.Remove(uri);
        this._imageCurrentLocation.Remove(index);
      }
      this._availableSpotsOnGrid.Remove(index);
    }
  }
}
