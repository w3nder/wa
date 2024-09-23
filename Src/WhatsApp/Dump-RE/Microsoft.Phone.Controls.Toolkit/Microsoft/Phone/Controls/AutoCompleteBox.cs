// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.AutoCompleteBox
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

#nullable disable
namespace Microsoft.Phone.Controls
{
  [TemplateVisualState(Name = "InvalidUnfocused", GroupName = "ValidationStates")]
  [TemplatePart(Name = "Popup", Type = typeof (Popup))]
  [StyleTypedProperty(Property = "TextBoxStyle", StyleTargetType = typeof (TextBox))]
  [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof (ListBox))]
  [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
  [TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
  [TemplateVisualState(Name = "PopupClosed", GroupName = "PopupStates")]
  [TemplateVisualState(Name = "PopupOpened", GroupName = "PopupStates")]
  [TemplateVisualState(Name = "Valid", GroupName = "ValidationStates")]
  [TemplateVisualState(Name = "InvalidFocused", GroupName = "ValidationStates")]
  [TemplatePart(Name = "Text", Type = typeof (TextBox))]
  [ContentProperty("ItemsSource")]
  [TemplatePart(Name = "SelectionAdapter", Type = typeof (ISelectionAdapter))]
  [TemplatePart(Name = "Selector", Type = typeof (Selector))]
  public class AutoCompleteBox : Control, IUpdateVisualState
  {
    private const string ElementSelectionAdapter = "SelectionAdapter";
    private const string ElementSelector = "Selector";
    private const string ElementPopup = "Popup";
    private const string ElementTextBox = "Text";
    private const string ElementTextBoxStyle = "TextBoxStyle";
    private const string ElementItemContainerStyle = "ItemContainerStyle";
    private List<object> _items;
    private ObservableCollection<object> _view;
    private int _ignoreTextPropertyChange;
    private bool _ignorePropertyChange;
    private bool _ignoreTextSelectionChange;
    private bool _skipSelectedItemTextUpdate;
    private int _textSelectionStart;
    private bool _inputtingText;
    private bool _userCalledPopulate;
    private bool _popupHasOpened;
    private DispatcherTimer _delayTimer;
    private bool _allowWrite;
    private BindingEvaluator<string> _valueBindingEvaluator;
    private WeakEventListener<AutoCompleteBox, object, NotifyCollectionChangedEventArgs> _collectionChangedWeakEventListener;
    private PhoneApplicationFrame _frame;
    public static readonly DependencyProperty MinimumPrefixLengthProperty = DependencyProperty.Register(nameof (MinimumPrefixLength), typeof (int), typeof (AutoCompleteBox), new PropertyMetadata((object) 1, new PropertyChangedCallback(AutoCompleteBox.OnMinimumPrefixLengthPropertyChanged)));
    public static readonly DependencyProperty MinimumPopulateDelayProperty = DependencyProperty.Register(nameof (MinimumPopulateDelay), typeof (int), typeof (AutoCompleteBox), new PropertyMetadata(new PropertyChangedCallback(AutoCompleteBox.OnMinimumPopulateDelayPropertyChanged)));
    public static readonly DependencyProperty IsTextCompletionEnabledProperty = DependencyProperty.Register(nameof (IsTextCompletionEnabled), typeof (bool), typeof (AutoCompleteBox), new PropertyMetadata((object) false, (PropertyChangedCallback) null));
    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof (ItemTemplate), typeof (DataTemplate), typeof (AutoCompleteBox), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register(nameof (ItemContainerStyle), typeof (Style), typeof (AutoCompleteBox), new PropertyMetadata((object) null, (PropertyChangedCallback) null));
    public static readonly DependencyProperty TextBoxStyleProperty = DependencyProperty.Register(nameof (TextBoxStyle), typeof (Style), typeof (AutoCompleteBox), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty MaxDropDownHeightProperty = DependencyProperty.Register(nameof (MaxDropDownHeight), typeof (double), typeof (AutoCompleteBox), new PropertyMetadata((object) double.PositiveInfinity, new PropertyChangedCallback(AutoCompleteBox.OnMaxDropDownHeightPropertyChanged)));
    public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register(nameof (IsDropDownOpen), typeof (bool), typeof (AutoCompleteBox), new PropertyMetadata((object) false, new PropertyChangedCallback(AutoCompleteBox.OnIsDropDownOpenPropertyChanged)));
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof (ItemsSource), typeof (IEnumerable), typeof (AutoCompleteBox), new PropertyMetadata(new PropertyChangedCallback(AutoCompleteBox.OnItemsSourcePropertyChanged)));
    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof (SelectedItem), typeof (object), typeof (AutoCompleteBox), new PropertyMetadata(new PropertyChangedCallback(AutoCompleteBox.OnSelectedItemPropertyChanged)));
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof (Text), typeof (string), typeof (AutoCompleteBox), new PropertyMetadata((object) string.Empty, new PropertyChangedCallback(AutoCompleteBox.OnTextPropertyChanged)));
    public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register(nameof (SearchText), typeof (string), typeof (AutoCompleteBox), new PropertyMetadata((object) string.Empty, new PropertyChangedCallback(AutoCompleteBox.OnSearchTextPropertyChanged)));
    public static readonly DependencyProperty FilterModeProperty = DependencyProperty.Register(nameof (FilterMode), typeof (AutoCompleteFilterMode), typeof (AutoCompleteBox), new PropertyMetadata((object) AutoCompleteFilterMode.StartsWith, new PropertyChangedCallback(AutoCompleteBox.OnFilterModePropertyChanged)));
    public static readonly DependencyProperty ItemFilterProperty = DependencyProperty.Register(nameof (ItemFilter), typeof (AutoCompleteFilterPredicate<object>), typeof (AutoCompleteBox), new PropertyMetadata(new PropertyChangedCallback(AutoCompleteBox.OnItemFilterPropertyChanged)));
    public static readonly DependencyProperty TextFilterProperty = DependencyProperty.Register(nameof (TextFilter), typeof (AutoCompleteFilterPredicate<string>), typeof (AutoCompleteBox), new PropertyMetadata((object) AutoCompleteSearch.GetFilter(AutoCompleteFilterMode.StartsWith)));
    public static readonly DependencyProperty InputScopeProperty = DependencyProperty.Register(nameof (InputScope), typeof (InputScope), typeof (AutoCompleteBox), (PropertyMetadata) null);
    private TextBox _text;
    private ISelectionAdapter _adapter;

    internal InteractionHelper Interaction { get; set; }

    public int MinimumPrefixLength
    {
      get => (int) this.GetValue(AutoCompleteBox.MinimumPrefixLengthProperty);
      set => this.SetValue(AutoCompleteBox.MinimumPrefixLengthProperty, (object) value);
    }

    private static void OnMinimumPrefixLengthPropertyChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      int newValue = (int) e.NewValue;
      if (newValue < 0 && newValue != -1)
        throw new ArgumentOutOfRangeException("MinimumPrefixLength");
    }

    public int MinimumPopulateDelay
    {
      get => (int) this.GetValue(AutoCompleteBox.MinimumPopulateDelayProperty);
      set => this.SetValue(AutoCompleteBox.MinimumPopulateDelayProperty, (object) value);
    }

    private static void OnMinimumPopulateDelayPropertyChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      AutoCompleteBox autoCompleteBox = d as AutoCompleteBox;
      if (autoCompleteBox._ignorePropertyChange)
      {
        autoCompleteBox._ignorePropertyChange = false;
      }
      else
      {
        int newValue = (int) e.NewValue;
        if (newValue < 0)
        {
          autoCompleteBox._ignorePropertyChange = true;
          d.SetValue(e.Property, e.OldValue);
        }
        if (autoCompleteBox._delayTimer != null)
        {
          autoCompleteBox._delayTimer.Stop();
          if (newValue == 0)
            autoCompleteBox._delayTimer = (DispatcherTimer) null;
        }
        if (newValue > 0 && autoCompleteBox._delayTimer == null)
        {
          autoCompleteBox._delayTimer = new DispatcherTimer();
          autoCompleteBox._delayTimer.Tick += new EventHandler(autoCompleteBox.PopulateDropDown);
        }
        if (newValue <= 0 || autoCompleteBox._delayTimer == null)
          return;
        autoCompleteBox._delayTimer.Interval = TimeSpan.FromMilliseconds((double) newValue);
      }
    }

    public bool IsTextCompletionEnabled
    {
      get => (bool) this.GetValue(AutoCompleteBox.IsTextCompletionEnabledProperty);
      set => this.SetValue(AutoCompleteBox.IsTextCompletionEnabledProperty, (object) value);
    }

    public DataTemplate ItemTemplate
    {
      get => this.GetValue(AutoCompleteBox.ItemTemplateProperty) as DataTemplate;
      set => this.SetValue(AutoCompleteBox.ItemTemplateProperty, (object) value);
    }

    public Style ItemContainerStyle
    {
      get => this.GetValue(AutoCompleteBox.ItemContainerStyleProperty) as Style;
      set => this.SetValue(AutoCompleteBox.ItemContainerStyleProperty, (object) value);
    }

    public Style TextBoxStyle
    {
      get => this.GetValue(AutoCompleteBox.TextBoxStyleProperty) as Style;
      set => this.SetValue(AutoCompleteBox.TextBoxStyleProperty, (object) value);
    }

    public double MaxDropDownHeight
    {
      get => (double) this.GetValue(AutoCompleteBox.MaxDropDownHeightProperty);
      set => this.SetValue(AutoCompleteBox.MaxDropDownHeightProperty, (object) value);
    }

    private static void OnMaxDropDownHeightPropertyChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      AutoCompleteBox autoCompleteBox = d as AutoCompleteBox;
      if (autoCompleteBox._ignorePropertyChange)
      {
        autoCompleteBox._ignorePropertyChange = false;
      }
      else
      {
        double newValue = (double) e.NewValue;
        if (newValue < 0.0)
        {
          autoCompleteBox._ignorePropertyChange = true;
          autoCompleteBox.SetValue(e.Property, e.OldValue);
        }
        autoCompleteBox.OnMaxDropDownHeightChanged(newValue);
      }
    }

    public bool IsDropDownOpen
    {
      get => (bool) this.GetValue(AutoCompleteBox.IsDropDownOpenProperty);
      set => this.SetValue(AutoCompleteBox.IsDropDownOpenProperty, (object) value);
    }

    private static void OnIsDropDownOpenPropertyChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      AutoCompleteBox autoCompleteBox = d as AutoCompleteBox;
      if (autoCompleteBox._ignorePropertyChange)
      {
        autoCompleteBox._ignorePropertyChange = false;
      }
      else
      {
        bool oldValue = (bool) e.OldValue;
        if ((bool) e.NewValue)
          autoCompleteBox.TextUpdated(autoCompleteBox.Text, true);
        else
          autoCompleteBox.ClosingDropDown(oldValue);
        autoCompleteBox.UpdateVisualState(true);
      }
    }

    public IEnumerable ItemsSource
    {
      get => this.GetValue(AutoCompleteBox.ItemsSourceProperty) as IEnumerable;
      set => this.SetValue(AutoCompleteBox.ItemsSourceProperty, (object) value);
    }

    private static void OnItemsSourcePropertyChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      (d as AutoCompleteBox).OnItemsSourceChanged((IEnumerable) e.OldValue, (IEnumerable) e.NewValue);
    }

    public object SelectedItem
    {
      get => this.GetValue(AutoCompleteBox.SelectedItemProperty);
      set => this.SetValue(AutoCompleteBox.SelectedItemProperty, value);
    }

    private static void OnSelectedItemPropertyChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      AutoCompleteBox autoCompleteBox = d as AutoCompleteBox;
      if (autoCompleteBox._ignorePropertyChange)
      {
        autoCompleteBox._ignorePropertyChange = false;
      }
      else
      {
        if (autoCompleteBox._skipSelectedItemTextUpdate)
          autoCompleteBox._skipSelectedItemTextUpdate = false;
        else
          autoCompleteBox.OnSelectedItemChanged(e.NewValue);
        List<object> removedItems = new List<object>();
        if (e.OldValue != null)
          removedItems.Add(e.OldValue);
        List<object> addedItems = new List<object>();
        if (e.NewValue != null)
          addedItems.Add(e.NewValue);
        autoCompleteBox.OnSelectionChanged(new SelectionChangedEventArgs((IList) removedItems, (IList) addedItems));
      }
    }

    private void OnSelectedItemChanged(object newItem)
    {
      this.UpdateTextValue(newItem != null ? this.FormatValue(newItem, true) : this.SearchText);
      if (this.TextBox == null || this.Text == null)
        return;
      this.TextBox.SelectionStart = this.Text.Length;
    }

    public string Text
    {
      get => this.GetValue(AutoCompleteBox.TextProperty) as string;
      set => this.SetValue(AutoCompleteBox.TextProperty, (object) value);
    }

    private static void OnTextPropertyChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      (d as AutoCompleteBox).TextUpdated((string) e.NewValue, false);
    }

    public string SearchText
    {
      get => (string) this.GetValue(AutoCompleteBox.SearchTextProperty);
      private set
      {
        try
        {
          this._allowWrite = true;
          this.SetValue(AutoCompleteBox.SearchTextProperty, (object) value);
        }
        finally
        {
          this._allowWrite = false;
        }
      }
    }

    private static void OnSearchTextPropertyChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      AutoCompleteBox autoCompleteBox = d as AutoCompleteBox;
      if (autoCompleteBox._ignorePropertyChange)
      {
        autoCompleteBox._ignorePropertyChange = false;
      }
      else
      {
        if (autoCompleteBox._allowWrite)
          return;
        autoCompleteBox._ignorePropertyChange = true;
        autoCompleteBox.SetValue(e.Property, e.OldValue);
      }
    }

    public AutoCompleteFilterMode FilterMode
    {
      get => (AutoCompleteFilterMode) this.GetValue(AutoCompleteBox.FilterModeProperty);
      set => this.SetValue(AutoCompleteBox.FilterModeProperty, (object) value);
    }

    private static void OnFilterModePropertyChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      AutoCompleteBox autoCompleteBox = d as AutoCompleteBox;
      switch ((AutoCompleteFilterMode) e.NewValue)
      {
        case AutoCompleteFilterMode.None:
        case AutoCompleteFilterMode.StartsWith:
        case AutoCompleteFilterMode.StartsWithCaseSensitive:
        case AutoCompleteFilterMode.StartsWithOrdinal:
        case AutoCompleteFilterMode.StartsWithOrdinalCaseSensitive:
        case AutoCompleteFilterMode.Contains:
        case AutoCompleteFilterMode.ContainsCaseSensitive:
        case AutoCompleteFilterMode.ContainsOrdinal:
        case AutoCompleteFilterMode.ContainsOrdinalCaseSensitive:
        case AutoCompleteFilterMode.Equals:
        case AutoCompleteFilterMode.EqualsCaseSensitive:
        case AutoCompleteFilterMode.EqualsOrdinal:
        case AutoCompleteFilterMode.EqualsOrdinalCaseSensitive:
        case AutoCompleteFilterMode.Custom:
          AutoCompleteFilterMode newValue = (AutoCompleteFilterMode) e.NewValue;
          autoCompleteBox.TextFilter = AutoCompleteSearch.GetFilter(newValue);
          break;
        default:
          autoCompleteBox.SetValue(e.Property, e.OldValue);
          goto case AutoCompleteFilterMode.None;
      }
    }

    public AutoCompleteFilterPredicate<object> ItemFilter
    {
      get
      {
        return this.GetValue(AutoCompleteBox.ItemFilterProperty) as AutoCompleteFilterPredicate<object>;
      }
      set => this.SetValue(AutoCompleteBox.ItemFilterProperty, (object) value);
    }

    private static void OnItemFilterPropertyChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      AutoCompleteBox autoCompleteBox = d as AutoCompleteBox;
      if (!(e.NewValue is AutoCompleteFilterPredicate<object>))
      {
        autoCompleteBox.FilterMode = AutoCompleteFilterMode.None;
      }
      else
      {
        autoCompleteBox.FilterMode = AutoCompleteFilterMode.Custom;
        autoCompleteBox.TextFilter = (AutoCompleteFilterPredicate<string>) null;
      }
    }

    public AutoCompleteFilterPredicate<string> TextFilter
    {
      get
      {
        return this.GetValue(AutoCompleteBox.TextFilterProperty) as AutoCompleteFilterPredicate<string>;
      }
      set => this.SetValue(AutoCompleteBox.TextFilterProperty, (object) value);
    }

    public InputScope InputScope
    {
      get => (InputScope) this.GetValue(AutoCompleteBox.InputScopeProperty);
      set => this.SetValue(AutoCompleteBox.InputScopeProperty, (object) value);
    }

    private PopupHelper DropDownPopup { get; set; }

    private static bool IsCompletionEnabled
    {
      get
      {
        PhoneApplicationFrame phoneApplicationFrame;
        return PhoneHelper.TryGetPhoneApplicationFrame(out phoneApplicationFrame) && phoneApplicationFrame.IsPortrait();
      }
    }

    internal TextBox TextBox
    {
      get => this._text;
      set
      {
        if (this._text != null)
        {
          this._text.SelectionChanged -= new RoutedEventHandler(this.OnTextBoxSelectionChanged);
          this._text.TextChanged -= new TextChangedEventHandler(this.OnTextBoxTextChanged);
        }
        this._text = value;
        if (this._text == null)
          return;
        this._text.SelectionChanged += new RoutedEventHandler(this.OnTextBoxSelectionChanged);
        this._text.TextChanged += new TextChangedEventHandler(this.OnTextBoxTextChanged);
        if (this.Text == null)
          return;
        this.UpdateTextValue(this.Text);
      }
    }

    protected internal ISelectionAdapter SelectionAdapter
    {
      get => this._adapter;
      set
      {
        if (this._adapter != null)
        {
          this._adapter.SelectionChanged -= new SelectionChangedEventHandler(this.OnAdapterSelectionChanged);
          this._adapter.Commit -= new RoutedEventHandler(this.OnAdapterSelectionComplete);
          this._adapter.Cancel -= new RoutedEventHandler(this.OnAdapterSelectionCanceled);
          this._adapter.Cancel -= new RoutedEventHandler(this.OnAdapterSelectionComplete);
          this._adapter.ItemsSource = (IEnumerable) null;
        }
        this._adapter = value;
        if (this._adapter == null)
          return;
        this._adapter.SelectionChanged += new SelectionChangedEventHandler(this.OnAdapterSelectionChanged);
        this._adapter.Commit += new RoutedEventHandler(this.OnAdapterSelectionComplete);
        this._adapter.Cancel += new RoutedEventHandler(this.OnAdapterSelectionCanceled);
        this._adapter.Cancel += new RoutedEventHandler(this.OnAdapterSelectionComplete);
        this._adapter.ItemsSource = (IEnumerable) this._view;
      }
    }

    public event RoutedEventHandler TextChanged;

    public event PopulatingEventHandler Populating;

    public event PopulatedEventHandler Populated;

    public event RoutedPropertyChangingEventHandler<bool> DropDownOpening;

    public event RoutedPropertyChangedEventHandler<bool> DropDownOpened;

    public event RoutedPropertyChangingEventHandler<bool> DropDownClosing;

    public event RoutedPropertyChangedEventHandler<bool> DropDownClosed;

    public event SelectionChangedEventHandler SelectionChanged;

    public Binding ValueMemberBinding
    {
      get
      {
        return this._valueBindingEvaluator == null ? (Binding) null : this._valueBindingEvaluator.ValueBinding;
      }
      set => this._valueBindingEvaluator = new BindingEvaluator<string>(value);
    }

    public string ValueMemberPath
    {
      get => this.ValueMemberBinding == null ? (string) null : this.ValueMemberBinding.Path.Path;
      set => this.ValueMemberBinding = value == null ? (Binding) null : new Binding(value);
    }

    public AutoCompleteBox()
    {
      this.DefaultStyleKey = (object) typeof (AutoCompleteBox);
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
      this.Unloaded += new RoutedEventHandler(this.OnUnloaded);
      this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.ControlIsEnabledChanged);
      this.Interaction = new InteractionHelper((Control) this);
      this.ClearView();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      this.ApplyTemplate();
      if (this._frame != null || !PhoneHelper.TryGetPhoneApplicationFrame(out this._frame))
        return;
      this._frame.OrientationChanged += new EventHandler<OrientationChangedEventArgs>(this.OnOrientationChanged);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
      if (this._frame == null)
        return;
      this._frame.OrientationChanged -= new EventHandler<OrientationChangedEventArgs>(this.OnOrientationChanged);
      this._frame = (PhoneApplicationFrame) null;
    }

    private void OnOrientationChanged(object sender, OrientationChangedEventArgs e)
    {
      this.IsDropDownOpen = false;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      Size size = base.ArrangeOverride(finalSize);
      if (this.DropDownPopup != null)
        this.DropDownPopup.Arrange(new Size?(finalSize));
      return size;
    }

    public override void OnApplyTemplate()
    {
      if (this.TextBox != null)
      {
        this.TextBox.RemoveHandler(UIElement.KeyDownEvent, (Delegate) new KeyEventHandler(this.OnUIElementKeyDown));
        this.TextBox.RemoveHandler(UIElement.KeyUpEvent, (Delegate) new KeyEventHandler(this.OnUIElementKeyUp));
      }
      if (this.DropDownPopup != null)
      {
        this.DropDownPopup.Closed -= new EventHandler(this.DropDownPopup_Closed);
        this.DropDownPopup.FocusChanged -= new EventHandler(this.OnDropDownFocusChanged);
        this.DropDownPopup.UpdateVisualStates -= new EventHandler(this.OnDropDownPopupUpdateVisualStates);
        this.DropDownPopup.BeforeOnApplyTemplate();
        this.DropDownPopup = (PopupHelper) null;
      }
      base.OnApplyTemplate();
      if (this.GetTemplateChild("Popup") is Popup templateChild)
      {
        this.DropDownPopup = new PopupHelper((Control) this, templateChild);
        this.DropDownPopup.MaxDropDownHeight = this.MaxDropDownHeight;
        this.DropDownPopup.AfterOnApplyTemplate();
        this.DropDownPopup.Closed += new EventHandler(this.DropDownPopup_Closed);
        this.DropDownPopup.FocusChanged += new EventHandler(this.OnDropDownFocusChanged);
        this.DropDownPopup.UpdateVisualStates += new EventHandler(this.OnDropDownPopupUpdateVisualStates);
      }
      this.SelectionAdapter = this.GetSelectionAdapterPart();
      this.TextBox = this.GetTemplateChild("Text") as TextBox;
      if (this.TextBox != null)
      {
        this.TextBox.AddHandler(UIElement.KeyDownEvent, (Delegate) new KeyEventHandler(this.OnUIElementKeyDown), true);
        this.TextBox.AddHandler(UIElement.KeyUpEvent, (Delegate) new KeyEventHandler(this.OnUIElementKeyUp), true);
      }
      this.Interaction.OnApplyTemplateBase();
      if (!this.IsDropDownOpen || this.DropDownPopup == null || this.DropDownPopup.IsOpen)
        return;
      this.OpeningDropDown(false);
    }

    private void OnDropDownPopupUpdateVisualStates(object sender, EventArgs e)
    {
      this.UpdateVisualState(true);
    }

    private void OnDropDownFocusChanged(object sender, EventArgs e)
    {
      this.FocusChanged(this.HasFocus());
    }

    private void ClosingDropDown(bool oldValue)
    {
      bool flag = false;
      if (this.DropDownPopup != null)
        flag = this.DropDownPopup.UsesClosingVisualState;
      RoutedPropertyChangingEventArgs<bool> e = new RoutedPropertyChangingEventArgs<bool>(AutoCompleteBox.IsDropDownOpenProperty, oldValue, false, true);
      this.OnDropDownClosing(e);
      if (this._view == null || this._view.Count == 0)
        flag = false;
      if (e.Cancel)
      {
        this._ignorePropertyChange = true;
        this.SetValue(AutoCompleteBox.IsDropDownOpenProperty, (object) oldValue);
      }
      else if (!flag)
        this.CloseDropDown(oldValue, false);
      this.UpdateVisualState(true);
    }

    private void OpeningDropDown(bool oldValue)
    {
      if (!AutoCompleteBox.IsCompletionEnabled)
        return;
      RoutedPropertyChangingEventArgs<bool> e = new RoutedPropertyChangingEventArgs<bool>(AutoCompleteBox.IsDropDownOpenProperty, oldValue, true, true);
      this.OnDropDownOpening(e);
      if (e.Cancel)
      {
        this._ignorePropertyChange = true;
        this.SetValue(AutoCompleteBox.IsDropDownOpenProperty, (object) oldValue);
      }
      else
        this.OpenDropDown(oldValue, true);
      this.UpdateVisualState(true);
    }

    private void DropDownPopup_Closed(object sender, EventArgs e)
    {
      if (this.IsDropDownOpen)
        this.IsDropDownOpen = false;
      if (!this._popupHasOpened)
        return;
      this.OnDropDownClosed(new RoutedPropertyChangedEventArgs<bool>(true, false));
    }

    private void FocusChanged(bool hasFocus)
    {
      if (hasFocus)
      {
        if (this.TextBox == null || this.TextBox.SelectionLength != 0)
          return;
        this.TextBox.Focus();
      }
      else
      {
        this.IsDropDownOpen = false;
        this._userCalledPopulate = false;
      }
    }

    protected bool HasFocus()
    {
      DependencyObject parent;
      for (DependencyObject dependencyObject = FocusManager.GetFocusedElement() as DependencyObject; dependencyObject != null; dependencyObject = parent)
      {
        if (object.ReferenceEquals((object) dependencyObject, (object) this))
          return true;
        parent = VisualTreeHelper.GetParent(dependencyObject);
        if (parent == null && dependencyObject is FrameworkElement frameworkElement)
          parent = frameworkElement.Parent;
      }
      return false;
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
      base.OnGotFocus(e);
      this.FocusChanged(this.HasFocus());
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
      base.OnLostFocus(e);
      this.FocusChanged(this.HasFocus());
    }

    private void ControlIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if ((bool) e.NewValue)
        return;
      this.IsDropDownOpen = false;
    }

    protected virtual ISelectionAdapter GetSelectionAdapterPart()
    {
      selectionAdapterPart = (ISelectionAdapter) null;
      if (this.GetTemplateChild("Selector") is Selector templateChild && !(templateChild is ISelectionAdapter selectionAdapterPart))
        selectionAdapterPart = (ISelectionAdapter) new SelectorSelectionAdapter(templateChild);
      if (selectionAdapterPart == null)
        selectionAdapterPart = this.GetTemplateChild("SelectionAdapter") as ISelectionAdapter;
      return selectionAdapterPart;
    }

    private void PopulateDropDown(object sender, EventArgs e)
    {
      if (this._delayTimer != null)
        this._delayTimer.Stop();
      this.SearchText = this.Text;
      PopulatingEventArgs e1 = new PopulatingEventArgs(this.SearchText);
      this.OnPopulating(e1);
      if (e1.Cancel)
        return;
      this.PopulateComplete();
    }

    protected virtual void OnPopulating(PopulatingEventArgs e)
    {
      PopulatingEventHandler populating = this.Populating;
      if (populating == null)
        return;
      populating((object) this, e);
    }

    protected virtual void OnPopulated(PopulatedEventArgs e)
    {
      PopulatedEventHandler populated = this.Populated;
      if (populated == null)
        return;
      populated((object) this, e);
    }

    protected virtual void OnSelectionChanged(SelectionChangedEventArgs e)
    {
      SelectionChangedEventHandler selectionChanged = this.SelectionChanged;
      if (selectionChanged == null)
        return;
      selectionChanged((object) this, e);
    }

    protected virtual void OnDropDownOpening(RoutedPropertyChangingEventArgs<bool> e)
    {
      RoutedPropertyChangingEventHandler<bool> dropDownOpening = this.DropDownOpening;
      if (dropDownOpening == null)
        return;
      dropDownOpening((object) this, e);
    }

    protected virtual void OnDropDownOpened(RoutedPropertyChangedEventArgs<bool> e)
    {
      RoutedPropertyChangedEventHandler<bool> dropDownOpened = this.DropDownOpened;
      if (dropDownOpened == null)
        return;
      dropDownOpened((object) this, e);
    }

    protected virtual void OnDropDownClosing(RoutedPropertyChangingEventArgs<bool> e)
    {
      RoutedPropertyChangingEventHandler<bool> dropDownClosing = this.DropDownClosing;
      if (dropDownClosing == null)
        return;
      dropDownClosing((object) this, e);
    }

    protected virtual void OnDropDownClosed(RoutedPropertyChangedEventArgs<bool> e)
    {
      RoutedPropertyChangedEventHandler<bool> dropDownClosed = this.DropDownClosed;
      if (dropDownClosed == null)
        return;
      dropDownClosed((object) this, e);
    }

    private string FormatValue(object value, bool clearDataContext)
    {
      string str = this.FormatValue(value);
      if (clearDataContext && this._valueBindingEvaluator != null)
        this._valueBindingEvaluator.ClearDataContext();
      return str;
    }

    protected virtual string FormatValue(object value)
    {
      if (this._valueBindingEvaluator != null)
        return this._valueBindingEvaluator.GetDynamicValue(value) ?? string.Empty;
      return value != null ? value.ToString() : string.Empty;
    }

    protected virtual void OnTextChanged(RoutedEventArgs e)
    {
      RoutedEventHandler textChanged = this.TextChanged;
      if (textChanged == null)
        return;
      textChanged((object) this, e);
    }

    private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
    {
      this.TextUpdated(this._text.Text, true);
    }

    private void OnTextBoxSelectionChanged(object sender, RoutedEventArgs e)
    {
      if (this._ignoreTextSelectionChange || this._inputtingText)
        return;
      this._textSelectionStart = this._text.SelectionStart;
    }

    private void OnUIElementKeyDown(object sender, KeyEventArgs e) => this._inputtingText = true;

    private void OnUIElementKeyUp(object sender, KeyEventArgs e) => this._inputtingText = false;

    private void UpdateTextValue(string value) => this.UpdateTextValue(value, new bool?());

    private void UpdateTextValue(string value, bool? userInitiated)
    {
      if (userInitiated.HasValue)
      {
        bool? nullable = userInitiated;
        if ((!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? 1 : 0)) == 0)
          goto label_4;
      }
      if (this.Text != value)
      {
        ++this._ignoreTextPropertyChange;
        this.Text = value;
        this.OnTextChanged(new RoutedEventArgs());
      }
label_4:
      if (userInitiated.HasValue)
      {
        bool? nullable = userInitiated;
        if ((nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? 1 : 0)) == 0)
          return;
      }
      if (this.TextBox == null || !(this.TextBox.Text != value))
        return;
      ++this._ignoreTextPropertyChange;
      this.TextBox.Text = value ?? string.Empty;
      if (!(this.Text == value) && this.Text != null)
        return;
      this.OnTextChanged(new RoutedEventArgs());
    }

    private void TextUpdated(string newText, bool userInitiated)
    {
      if (this._ignoreTextPropertyChange > 0)
      {
        --this._ignoreTextPropertyChange;
      }
      else
      {
        if (newText == null)
          newText = string.Empty;
        if (this.IsTextCompletionEnabled && this.TextBox != null && this.TextBox.SelectionLength > 0 && this.TextBox.SelectionStart != this.TextBox.Text.Length)
          return;
        bool flag = newText.Length >= this.MinimumPrefixLength && this.MinimumPrefixLength >= 0;
        this._userCalledPopulate = flag && userInitiated;
        this.UpdateTextValue(newText, new bool?(userInitiated));
        if (flag)
        {
          this._ignoreTextSelectionChange = true;
          if (this._delayTimer != null)
            this._delayTimer.Start();
          else
            this.PopulateDropDown((object) this, EventArgs.Empty);
        }
        else
        {
          this.SearchText = string.Empty;
          if (this.SelectedItem != null)
            this._skipSelectedItemTextUpdate = true;
          this.SelectedItem = (object) null;
          if (!this.IsDropDownOpen)
            return;
          this.IsDropDownOpen = false;
        }
      }
    }

    public void PopulateComplete()
    {
      this.RefreshView();
      this.OnPopulated(new PopulatedEventArgs((IEnumerable) new ReadOnlyCollection<object>((IList<object>) this._view)));
      if (this.SelectionAdapter != null && this.SelectionAdapter.ItemsSource != this._view)
        this.SelectionAdapter.ItemsSource = (IEnumerable) this._view;
      bool flag = this._userCalledPopulate && this._view.Count > 0;
      if (flag != this.IsDropDownOpen)
      {
        this._ignorePropertyChange = true;
        this.IsDropDownOpen = flag;
      }
      if (this.IsDropDownOpen)
      {
        this.OpeningDropDown(false);
        if (this.DropDownPopup != null)
          this.DropDownPopup.Arrange(new Size?());
      }
      else
        this.ClosingDropDown(true);
      this.UpdateTextCompletion(this._userCalledPopulate);
    }

    private void UpdateTextCompletion(bool userInitiated)
    {
      object obj1 = (object) null;
      string text = this.Text;
      if (this._view.Count > 0)
      {
        if (this.IsTextCompletionEnabled && this.TextBox != null && userInitiated)
        {
          int length1 = this.TextBox.Text.Length;
          int selectionStart = this.TextBox.SelectionStart;
          if (selectionStart == text.Length && selectionStart > this._textSelectionStart)
          {
            object obj2 = this.FilterMode == AutoCompleteFilterMode.StartsWith || this.FilterMode == AutoCompleteFilterMode.StartsWithCaseSensitive ? this._view[0] : this.TryGetMatch(text, this._view, AutoCompleteSearch.GetFilter(AutoCompleteFilterMode.StartsWith));
            if (obj2 != null)
            {
              obj1 = obj2;
              string str = this.FormatValue(obj2, true);
              int length2 = Math.Min(str.Length, this.Text.Length);
              if (AutoCompleteSearch.Equals(this.Text.Substring(0, length2), str.Substring(0, length2)))
              {
                this.UpdateTextValue(str);
                this.TextBox.SelectionStart = length1;
                this.TextBox.SelectionLength = str.Length - length1;
              }
            }
          }
        }
        else
          obj1 = this.TryGetMatch(text, this._view, AutoCompleteSearch.GetFilter(AutoCompleteFilterMode.EqualsCaseSensitive));
      }
      if (this.SelectedItem != obj1)
        this._skipSelectedItemTextUpdate = true;
      this.SelectedItem = obj1;
      if (!this._ignoreTextSelectionChange)
        return;
      this._ignoreTextSelectionChange = false;
      if (this.TextBox == null || this._inputtingText)
        return;
      this._textSelectionStart = this.TextBox.SelectionStart;
    }

    private object TryGetMatch(
      string searchText,
      ObservableCollection<object> view,
      AutoCompleteFilterPredicate<string> predicate)
    {
      if (view != null && view.Count > 0)
      {
        foreach (object match in (Collection<object>) view)
        {
          if (predicate(searchText, this.FormatValue(match)))
            return match;
        }
      }
      return (object) null;
    }

    private void ClearView()
    {
      if (this._view == null)
        this._view = new ObservableCollection<object>();
      else
        this._view.Clear();
    }

    private void RefreshView()
    {
      if (this._items == null)
      {
        this.ClearView();
      }
      else
      {
        string search = this.Text ?? string.Empty;
        bool flag1 = this.TextFilter != null;
        bool flag2 = this.FilterMode == AutoCompleteFilterMode.Custom && this.TextFilter == null;
        int index = 0;
        int count = this._view.Count;
        foreach (object obj in this._items)
        {
          bool flag3 = !flag1 && !flag2;
          if (!flag3)
            flag3 = flag1 ? this.TextFilter(search, this.FormatValue(obj)) : this.ItemFilter(search, obj);
          if (count > index && flag3 && this._view[index] == obj)
            ++index;
          else if (flag3)
          {
            if (count > index && this._view[index] != obj)
            {
              this._view.RemoveAt(index);
              this._view.Insert(index, obj);
              ++index;
            }
            else
            {
              if (index == count)
                this._view.Add(obj);
              else
                this._view.Insert(index, obj);
              ++index;
              ++count;
            }
          }
          else if (count > index && this._view[index] == obj)
          {
            this._view.RemoveAt(index);
            --count;
          }
        }
        if (this._valueBindingEvaluator == null)
          return;
        this._valueBindingEvaluator.ClearDataContext();
      }
    }

    private void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
    {
      if (oldValue is INotifyCollectionChanged && this._collectionChangedWeakEventListener != null)
      {
        this._collectionChangedWeakEventListener.Detach();
        this._collectionChangedWeakEventListener = (WeakEventListener<AutoCompleteBox, object, NotifyCollectionChangedEventArgs>) null;
      }
      INotifyCollectionChanged newValueINotifyCollectionChanged = newValue as INotifyCollectionChanged;
      if (newValueINotifyCollectionChanged != null)
      {
        this._collectionChangedWeakEventListener = new WeakEventListener<AutoCompleteBox, object, NotifyCollectionChangedEventArgs>(this);
        this._collectionChangedWeakEventListener.OnEventAction = (Action<AutoCompleteBox, object, NotifyCollectionChangedEventArgs>) ((instance, source, eventArgs) => instance.ItemsSourceCollectionChanged(source, eventArgs));
        this._collectionChangedWeakEventListener.OnDetachAction = (Action<WeakEventListener<AutoCompleteBox, object, NotifyCollectionChangedEventArgs>>) (weakEventListener => newValueINotifyCollectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(weakEventListener.OnEvent));
        newValueINotifyCollectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(this._collectionChangedWeakEventListener.OnEvent);
      }
      this._items = newValue == null ? (List<object>) null : new List<object>((IEnumerable<object>) newValue.Cast<object>().ToList<object>());
      this.ClearView();
      if (this.SelectionAdapter != null && this.SelectionAdapter.ItemsSource != this._view)
        this.SelectionAdapter.ItemsSource = (IEnumerable) this._view;
      if (!this.IsDropDownOpen)
        return;
      this.RefreshView();
    }

    private void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
      {
        for (int index = 0; index < e.OldItems.Count; ++index)
          this._items.RemoveAt(e.OldStartingIndex);
      }
      if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null && this._items.Count >= e.NewStartingIndex)
      {
        for (int index = 0; index < e.NewItems.Count; ++index)
          this._items.Insert(e.NewStartingIndex + index, e.NewItems[index]);
      }
      if (e.Action == NotifyCollectionChangedAction.Replace && e.NewItems != null && e.OldItems != null)
      {
        for (int index = 0; index < e.NewItems.Count; ++index)
          this._items[e.NewStartingIndex] = e.NewItems[index];
      }
      if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
      {
        for (int index = 0; index < e.OldItems.Count; ++index)
          this._view.Remove(e.OldItems[index]);
      }
      if (e.Action == NotifyCollectionChangedAction.Reset)
      {
        this.ClearView();
        if (this.ItemsSource != null)
          this._items = new List<object>((IEnumerable<object>) this.ItemsSource.Cast<object>().ToList<object>());
      }
      this.RefreshView();
    }

    private void OnAdapterSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      this.SelectedItem = this._adapter.SelectedItem;
    }

    private void OnAdapterSelectionComplete(object sender, RoutedEventArgs e)
    {
      this.IsDropDownOpen = false;
      this.UpdateTextCompletion(false);
      if (this.TextBox == null)
        return;
      this.TextBox.Select(this.TextBox.Text.Length, 0);
      this.TextBox.Focus();
    }

    private void OnAdapterSelectionCanceled(object sender, RoutedEventArgs e)
    {
      this.UpdateTextValue(this.SearchText);
      this.UpdateTextCompletion(false);
    }

    private void OnMaxDropDownHeightChanged(double newValue)
    {
      if (this.DropDownPopup != null)
      {
        this.DropDownPopup.MaxDropDownHeight = newValue;
        this.DropDownPopup.Arrange(new Size?());
      }
      this.UpdateVisualState(true);
    }

    private void OpenDropDown(bool oldValue, bool newValue)
    {
      if (this.DropDownPopup != null)
        this.DropDownPopup.IsOpen = true;
      this._popupHasOpened = true;
      this.OnDropDownOpened(new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue));
    }

    private void CloseDropDown(bool oldValue, bool newValue)
    {
      if (!this._popupHasOpened)
        return;
      if (this.SelectionAdapter != null)
        this.SelectionAdapter.SelectedItem = (object) null;
      if (this.DropDownPopup != null)
        this.DropDownPopup.IsOpen = false;
      this.OnDropDownClosed(new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue));
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException(nameof (e));
      base.OnKeyDown(e);
      if (e.Handled || !this.IsEnabled)
        return;
      if (this.IsDropDownOpen)
      {
        if (this.SelectionAdapter != null)
        {
          this.SelectionAdapter.HandleKeyDown(e);
          if (e.Handled)
            return;
        }
        if (e.Key == Key.Escape)
        {
          this.OnAdapterSelectionCanceled((object) this, new RoutedEventArgs());
          e.Handled = true;
        }
      }
      else if (e.Key == Key.Down)
      {
        this.IsDropDownOpen = true;
        e.Handled = true;
      }
      switch (e.Key)
      {
        case Key.Enter:
          this.OnAdapterSelectionComplete((object) this, new RoutedEventArgs());
          e.Handled = true;
          break;
        case Key.F4:
          this.IsDropDownOpen = !this.IsDropDownOpen;
          e.Handled = true;
          break;
      }
    }

    void IUpdateVisualState.UpdateVisualState(bool useTransitions)
    {
      this.UpdateVisualState(useTransitions);
    }

    internal virtual void UpdateVisualState(bool useTransitions)
    {
      VisualStateManager.GoToState((Control) this, this.IsDropDownOpen ? "PopupOpened" : "PopupClosed", useTransitions);
      this.Interaction.UpdateVisualStateBase(useTransitions);
    }
  }
}
