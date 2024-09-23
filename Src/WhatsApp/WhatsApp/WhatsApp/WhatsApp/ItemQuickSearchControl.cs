// Decompiled with JetBrains decompiler
// Type: WhatsApp.ItemQuickSearchControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WhatsApp.WaCollections;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class ItemQuickSearchControl : UserControl
  {
    private IEnumerable<JidItemViewModel> itemsSource_;
    private bool hasFocus_;
    private string currentSearchTerm_;
    private ItemSearch search_;
    private Set<string> matchedItems_;
    internal Grid LayoutRoot;
    internal AutoCompleteBox SearchField;
    internal TextBlock WatermarkBlock;
    private bool _contentLoaded;

    public event ItemQuickSearchControl.ItemSelectedHandler ItemSelected;

    protected void NotifyItemSelected(JidItemViewModel item)
    {
      if (this.ItemSelected == null || item == null)
        return;
      this.ItemSelected(item);
    }

    public IEnumerable<JidItemViewModel> ItemsSource
    {
      set
      {
        this.SearchField.ItemsSource = (IEnumerable) (this.itemsSource_ = (IEnumerable<JidItemViewModel>) value.ToArray<JidItemViewModel>());
        this.InitSearch();
      }
    }

    private bool HasFocus
    {
      get => this.hasFocus_;
      set
      {
        if (this.hasFocus_ == value)
          return;
        this.hasFocus_ = value;
        this.OnFocusChanged();
      }
    }

    private string CurrentSearchTerm
    {
      get => this.currentSearchTerm_;
      set
      {
        if (!(this.currentSearchTerm_ != value))
          return;
        this.currentSearchTerm_ = value;
        this.OnSearchTermChanged();
      }
    }

    public string WatermarkText
    {
      get => this.WatermarkBlock.Text;
      set => this.WatermarkBlock.Text = value;
    }

    public double MaxDropDownHeight
    {
      get => this.SearchField.MaxDropDownHeight;
      set => this.SearchField.MaxDropDownHeight = value;
    }

    public Func<JidItemViewModel, bool> ResultBlackListFilter { get; set; }

    public ItemQuickSearchControl()
    {
      this.InitializeComponent();
      this.SearchField.FilterMode = AutoCompleteFilterMode.Custom;
      this.SearchField.ItemFilter = new AutoCompleteFilterPredicate<object>(this.CustomItemFilterFunc);
      this.WatermarkBlock.Foreground = (Brush) new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 113, (byte) 113, (byte) 113));
    }

    private void InitSearch()
    {
      if (this.search_ != null)
      {
        this.search_.ClearData();
        this.search_ = (ItemSearch) null;
      }
      this.search_ = new ItemSearch();
      this.search_.SetSearchUnits(this.itemsSource_.Select<JidItemViewModel, ItemSearch.SearchUnit>((Func<JidItemViewModel, ItemSearch.SearchUnit>) (item => new ItemSearch.SearchUnit(item.TitleStr)
      {
        Tag = (object) item
      })));
    }

    private bool CustomItemFilterFunc(string searchTerm, object o)
    {
      if (!(o is JidItemViewModel jidItemViewModel) || this.ResultBlackListFilter != null && this.ResultBlackListFilter(jidItemViewModel))
        return false;
      return this.matchedItems_ != null ? this.matchedItems_.Contains(jidItemViewModel.TitleStr) : jidItemViewModel.TitleStr.ToLower().StartsWith(searchTerm.ToLower());
    }

    private void SearchField_TextChanged(object sender, EventArgs e)
    {
      this.CurrentSearchTerm = this.SearchField.Text;
    }

    private void SearchField_GotFocus(object sender, EventArgs e)
    {
      this.SearchField.Text = "";
      this.HasFocus = true;
    }

    private void SearchField_LostFocus(object sender, EventArgs e) => this.HasFocus = false;

    private void ResultList_SelectionChanged(object sender, EventArgs e)
    {
      this.SearchField.Text = "";
      JidItemViewModel selectedItem = this.SearchField.SelectedItem as JidItemViewModel;
      this.SearchField.SelectedItem = (object) null;
      if (selectedItem == null)
        return;
      this.NotifyItemSelected(selectedItem);
    }

    private void OnFocusChanged()
    {
      this.WatermarkBlock.Visibility = (!this.HasFocus && string.IsNullOrEmpty(this.SearchField.Text)).ToVisibility();
    }

    private void OnSearchTermChanged()
    {
      if (this.search_ == null)
        return;
      this.search_.Lookup(this.CurrentSearchTerm).Subscribe<ItemSearch.SearchResult>((Action<ItemSearch.SearchResult>) (result =>
      {
        if (result.Type == ItemSearch.SearchResult.ResultType.Success)
        {
          this.matchedItems_ = new Set<string>();
          foreach (ItemSearch.Match match in result.Matches)
            this.matchedItems_.Add(match.SearchUnit.SearchText);
        }
        else
          this.matchedItems_ = (Set<string>) null;
      }));
    }

    private void WatermarkBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.SearchField.Focus();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/ItemQuickSearchControl.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.SearchField = (AutoCompleteBox) this.FindName("SearchField");
      this.WatermarkBlock = (TextBlock) this.FindName("WatermarkBlock");
    }

    public delegate void ItemSelectedHandler(JidItemViewModel item);
  }
}
