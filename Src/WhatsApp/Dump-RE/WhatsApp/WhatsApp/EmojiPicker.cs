// Decompiled with JetBrains decompiler
// Type: WhatsApp.EmojiPicker
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#nullable disable
namespace WhatsApp
{
  public class EmojiPicker : UserControl
  {
    private EmojiSearch EmojiSearcher;
    private string searchLanguage;
    internal Grid LayoutRoot;
    internal Grid CategoryRow;
    internal Button RecentButton;
    internal Button SmileysAndPeopleButton;
    internal Button AnimalsAndNatureButton;
    internal Button FoodAndDrinkButton;
    internal Button ActivityButton;
    internal Button TravelAndPlacesButton;
    internal Button ObjectsButton;
    internal Button SymbolsButton;
    internal Button FlagsButton;
    private bool _contentLoaded;

    public EmojiPickerViewModel ViewModel { get; private set; }

    public EmojiPickerModel Model { get; private set; }

    public bool IsSearchSupported { get; set; }

    public EmojiPicker(
      Action<Emoji.EmojiChar> EmojiAction = null,
      Button BackspaceButton = null,
      TextBox insertionTextBox = null)
    {
      this.InitializeComponent();
      this.InsertionTextBox = insertionTextBox;
      this.ViewModel = new EmojiPickerViewModel(this, BackspaceButton);
      this.Model = new EmojiPickerModel(this, this.ViewModel, EmojiAction);
      this.DataContext = (object) this.ViewModel;
      this.Model.InitGroupContainer();
      this.ViewModel.InitCurrentGroupIndex();
      string lang;
      string locale;
      AppState.GetLangAndLocale(out lang, out locale);
      this.searchLanguage = lang + "-" + locale;
      if (!Settings.EmojiSearchEnabled)
        return;
      this.EmojiSearcher = EmojiSearch.GetInstance();
      this.IsSearchSupported = this.EmojiSearcher.IsEmojiSearchSupportedForLanguage(this.searchLanguage);
      if (!this.IsSearchSupported)
        return;
      this.Search((string) null);
    }

    public static Brush ActiveBrush => EmojiPickerViewModel.ActiveBrush;

    public static Brush InactiveBrush => EmojiPickerViewModel.InactiveBrush;

    public PageOrientation Orientation
    {
      get => this.ViewModel.Orientation;
      set => this.ViewModel.Orientation = value;
    }

    public TextBox InsertionTextBox { get; set; }

    public TextBox SearchTermTextBox
    {
      get => this.ViewModel.SearchTermTextBox;
      set => this.ViewModel.SearchTermTextBox = value;
    }

    public void ApplyPendingEmojiUsages() => this.Model.ApplyPendingEmojiUsages();

    public void ApplyPendingEmojiSelectedIndexes()
    {
      this.ViewModel.ApplyPendingEmojiSelectedIndexes();
    }

    private void EmojiPicker_Unloaded(object sender, EventArgs e)
    {
      this.ViewModel.SaveCurrentGroupFirstVisibleOffset();
      this.Model.SaveEmojiPagePositions();
    }

    public void Search(string searchTerm)
    {
      this.ViewModel.SearchTerm = searchTerm != null ? searchTerm.ToLangFriendlyLower().Trim() : (string) null;
      List<string> stringList = new List<string>();
      if (this.ViewModel.SearchTerm == null)
        Log.d(nameof (EmojiPicker), "Reset search");
      else if (string.IsNullOrEmpty(this.ViewModel.SearchTerm))
      {
        Log.d(nameof (EmojiPicker), "Searching empty string");
        stringList = this.GetRecentOrTopEmojis();
      }
      else
      {
        List<string> matchingEmoji1 = this.EmojiSearcher.GetMatchingEmoji(this.searchLanguage, this.ViewModel.SearchTerm, true);
        List<string> matchingEmoji2 = this.EmojiSearcher.GetMatchingEmoji(this.searchLanguage, this.ViewModel.SearchTerm);
        foreach (string str in matchingEmoji1)
          matchingEmoji2.Remove(str);
        List<string> recentOrTopEmojis = this.GetRecentOrTopEmojis();
        List<string> collection = new List<string>();
        HashSet<string> stringSet1 = new HashSet<string>((IEnumerable<string>) matchingEmoji1);
        HashSet<string> stringSet2 = new HashSet<string>((IEnumerable<string>) matchingEmoji2);
        foreach (string str in recentOrTopEmojis)
        {
          if (stringSet1.Contains(str))
          {
            stringList.Add(str);
            matchingEmoji1.Remove(str);
          }
          else if (stringSet2.Contains(str))
          {
            collection.Add(str);
            matchingEmoji2.Remove(str);
          }
        }
        stringList.AddRange((IEnumerable<string>) matchingEmoji1);
        stringList.AddRange((IEnumerable<string>) collection);
        stringList.AddRange((IEnumerable<string>) matchingEmoji2);
        Log.d(nameof (EmojiPicker), "Searching returned {0} results", (object) stringList.Count);
      }
      this.Model.EmojiSearchGroup.Values = stringList.ToArray();
      this.ViewModel.ResetEmojiGrid();
    }

    private List<string> GetRecentOrTopEmojis()
    {
      this.Model.UpdateRecentEmojis();
      string[] values = this.Model.getEmojiGroup(Emoji.PickerCategory.Recent).Values;
      List<string> source = this.EmojiSearcher.GetTopEmoji();
      if (!source.Any<string>())
        source = Emoji.TopFifty;
      HashSet<string> recentSet = new HashSet<string>((IEnumerable<string>) values);
      List<string> recentOrTopEmojis = new List<string>((IEnumerable<string>) values);
      recentOrTopEmojis.AddRange(source.Where<string>((Func<string, bool>) (s => !recentSet.Contains(s))));
      return recentOrTopEmojis;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/EmojiPicker.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.CategoryRow = (Grid) this.FindName("CategoryRow");
      this.RecentButton = (Button) this.FindName("RecentButton");
      this.SmileysAndPeopleButton = (Button) this.FindName("SmileysAndPeopleButton");
      this.AnimalsAndNatureButton = (Button) this.FindName("AnimalsAndNatureButton");
      this.FoodAndDrinkButton = (Button) this.FindName("FoodAndDrinkButton");
      this.ActivityButton = (Button) this.FindName("ActivityButton");
      this.TravelAndPlacesButton = (Button) this.FindName("TravelAndPlacesButton");
      this.ObjectsButton = (Button) this.FindName("ObjectsButton");
      this.SymbolsButton = (Button) this.FindName("SymbolsButton");
      this.FlagsButton = (Button) this.FindName("FlagsButton");
    }
  }
}
