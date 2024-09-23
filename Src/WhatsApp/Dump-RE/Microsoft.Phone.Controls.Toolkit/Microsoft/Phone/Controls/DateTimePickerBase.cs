// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.DateTimePickerBase
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using Microsoft.Phone.Controls.Primitives;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;

#nullable disable
namespace Microsoft.Phone.Controls
{
  [TemplatePart(Name = "DateTimeButton", Type = typeof (ButtonBase))]
  public class DateTimePickerBase : Control
  {
    private const string ButtonPartName = "DateTimeButton";
    private ButtonBase _dateButtonPart;
    private PhoneApplicationFrame _frame;
    private object _frameContentWhenOpened;
    private NavigationInTransition _savedNavigationInTransition;
    private NavigationOutTransition _savedNavigationOutTransition;
    private IDateTimePickerPage _dateTimePickerPage;
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof (Value), typeof (DateTime?), typeof (DateTimePickerBase), new PropertyMetadata((object) null, new PropertyChangedCallback(DateTimePickerBase.OnValueChanged)));
    public static readonly DependencyProperty ValueStringProperty = DependencyProperty.Register(nameof (ValueString), typeof (string), typeof (DateTimePickerBase), (PropertyMetadata) null);
    public static readonly DependencyProperty ValueStringFormatProperty = DependencyProperty.Register(nameof (ValueStringFormat), typeof (string), typeof (DateTimePickerBase), new PropertyMetadata((object) null, new PropertyChangedCallback(DateTimePickerBase.OnValueStringFormatChanged)));
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof (Header), typeof (object), typeof (DateTimePickerBase), (PropertyMetadata) null);
    public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(nameof (HeaderTemplate), typeof (DataTemplate), typeof (DateTimePickerBase), (PropertyMetadata) null);
    public static readonly DependencyProperty PickerPageUriProperty = DependencyProperty.Register(nameof (PickerPageUri), typeof (Uri), typeof (DateTimePickerBase), (PropertyMetadata) null);

    public event EventHandler<DateTimeValueChangedEventArgs> ValueChanged;

    [TypeConverter(typeof (TimeTypeConverter))]
    public DateTime? Value
    {
      get => (DateTime?) this.GetValue(DateTimePickerBase.ValueProperty);
      set => this.SetValue(DateTimePickerBase.ValueProperty, (object) value);
    }

    private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      ((DateTimePickerBase) o).OnValueChanged((DateTime?) e.OldValue, (DateTime?) e.NewValue);
    }

    private void OnValueChanged(DateTime? oldValue, DateTime? newValue)
    {
      this.UpdateValueString();
      this.OnValueChanged(new DateTimeValueChangedEventArgs(oldValue, newValue));
    }

    protected virtual void OnValueChanged(DateTimeValueChangedEventArgs e)
    {
      EventHandler<DateTimeValueChangedEventArgs> valueChanged = this.ValueChanged;
      if (valueChanged == null)
        return;
      valueChanged((object) this, e);
    }

    public string ValueString
    {
      get => (string) this.GetValue(DateTimePickerBase.ValueStringProperty);
      private set => this.SetValue(DateTimePickerBase.ValueStringProperty, (object) value);
    }

    public string ValueStringFormat
    {
      get => (string) this.GetValue(DateTimePickerBase.ValueStringFormatProperty);
      set => this.SetValue(DateTimePickerBase.ValueStringFormatProperty, (object) value);
    }

    private static void OnValueStringFormatChanged(
      DependencyObject o,
      DependencyPropertyChangedEventArgs e)
    {
      ((DateTimePickerBase) o).OnValueStringFormatChanged();
    }

    private void OnValueStringFormatChanged() => this.UpdateValueString();

    public object Header
    {
      get => this.GetValue(DateTimePickerBase.HeaderProperty);
      set => this.SetValue(DateTimePickerBase.HeaderProperty, value);
    }

    public DataTemplate HeaderTemplate
    {
      get => (DataTemplate) this.GetValue(DateTimePickerBase.HeaderTemplateProperty);
      set => this.SetValue(DateTimePickerBase.HeaderTemplateProperty, (object) value);
    }

    public Uri PickerPageUri
    {
      get => (Uri) this.GetValue(DateTimePickerBase.PickerPageUriProperty);
      set => this.SetValue(DateTimePickerBase.PickerPageUriProperty, (object) value);
    }

    protected virtual string ValueStringFormatFallback => "{0}";

    public override void OnApplyTemplate()
    {
      if (this._dateButtonPart != null)
        this._dateButtonPart.Click -= new RoutedEventHandler(this.OnDateButtonClick);
      base.OnApplyTemplate();
      this._dateButtonPart = this.GetTemplateChild("DateTimeButton") as ButtonBase;
      if (this._dateButtonPart == null)
        return;
      this._dateButtonPart.Click += new RoutedEventHandler(this.OnDateButtonClick);
    }

    internal static bool DateShouldFlowRTL()
    {
      string letterIsoLanguageName = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
      return letterIsoLanguageName == "ar" || letterIsoLanguageName == "fa";
    }

    internal static bool IsRTLLanguage()
    {
      string letterIsoLanguageName = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
      return letterIsoLanguageName == "ar" || letterIsoLanguageName == "he" || letterIsoLanguageName == "fa";
    }

    private void OnDateButtonClick(object sender, RoutedEventArgs e) => this.OpenPickerPage();

    private void UpdateValueString()
    {
      this.ValueString = string.Format((IFormatProvider) CultureInfo.CurrentCulture, this.ValueStringFormat ?? this.ValueStringFormatFallback, (object) this.Value);
    }

    private void OpenPickerPage()
    {
      if ((Uri) null == this.PickerPageUri)
        throw new ArgumentException("PickerPageUri property must not be null.");
      if (this._frame != null)
        return;
      this._frame = Application.Current.RootVisual as PhoneApplicationFrame;
      if (this._frame == null)
        return;
      this._frameContentWhenOpened = this._frame.Content;
      if (this._frameContentWhenOpened is UIElement contentWhenOpened)
      {
        this._savedNavigationInTransition = TransitionService.GetNavigationInTransition(contentWhenOpened);
        TransitionService.SetNavigationInTransition(contentWhenOpened, (NavigationInTransition) null);
        this._savedNavigationOutTransition = TransitionService.GetNavigationOutTransition(contentWhenOpened);
        TransitionService.SetNavigationOutTransition(contentWhenOpened, (NavigationOutTransition) null);
      }
      this._frame.Navigated += new NavigatedEventHandler(this.OnFrameNavigated);
      this._frame.NavigationStopped += new NavigationStoppedEventHandler(this.OnFrameNavigationStoppedOrFailed);
      this._frame.NavigationFailed += new NavigationFailedEventHandler(this.OnFrameNavigationStoppedOrFailed);
      this._frame.Navigate(this.PickerPageUri);
    }

    private void ClosePickerPage()
    {
      if (this._frame != null)
      {
        this._frame.Navigated -= new NavigatedEventHandler(this.OnFrameNavigated);
        this._frame.NavigationStopped -= new NavigationStoppedEventHandler(this.OnFrameNavigationStoppedOrFailed);
        this._frame.NavigationFailed -= new NavigationFailedEventHandler(this.OnFrameNavigationStoppedOrFailed);
        if (this._frameContentWhenOpened is UIElement contentWhenOpened)
        {
          TransitionService.SetNavigationInTransition(contentWhenOpened, this._savedNavigationInTransition);
          this._savedNavigationInTransition = (NavigationInTransition) null;
          TransitionService.SetNavigationOutTransition(contentWhenOpened, this._savedNavigationOutTransition);
          this._savedNavigationOutTransition = (NavigationOutTransition) null;
        }
        this._frame = (PhoneApplicationFrame) null;
        this._frameContentWhenOpened = (object) null;
      }
      if (this._dateTimePickerPage == null)
        return;
      if (this._dateTimePickerPage.Value.HasValue)
        this.Value = new DateTime?(this._dateTimePickerPage.Value.Value);
      this._dateTimePickerPage = (IDateTimePickerPage) null;
    }

    private void OnFrameNavigated(object sender, NavigationEventArgs e)
    {
      if (e.Content == this._frameContentWhenOpened)
      {
        this.ClosePickerPage();
      }
      else
      {
        if (this._dateTimePickerPage != null || !(e.Content is IDateTimePickerPage content))
          return;
        this._dateTimePickerPage = content;
        this._dateTimePickerPage.Value = new DateTime?(this.Value.GetValueOrDefault(DateTime.Now));
        content.SetFlowDirection(this.FlowDirection);
      }
    }

    private void OnFrameNavigationStoppedOrFailed(object sender, EventArgs e)
    {
      this.ClosePickerPage();
    }
  }
}
