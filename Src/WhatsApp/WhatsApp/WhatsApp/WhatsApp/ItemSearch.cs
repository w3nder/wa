// Decompiled with JetBrains decompiler
// Type: WhatsApp.ItemSearch
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;


namespace WhatsApp
{
  public class ItemSearch
  {
    private List<ItemSearch.SearchUnit> searchUnits_ = new List<ItemSearch.SearchUnit>();
    private ItemSearch.SearchType searchType_;
    private SearchMethodBase searchMethod_;
    private object loadingLock_ = new object();
    private bool isLoading_;
    private IDisposable loadingProgress_;

    public event EventHandler DataLoaded;

    protected void NotifyDataLoaded()
    {
      if (this.DataLoaded == null)
        return;
      this.DataLoaded((object) this, new EventArgs());
    }

    public Func<ItemSearch.SearchUnit, bool> EmptySearchTermResultsFilter { get; set; }

    public ItemSearch(ItemSearch.SearchType searchType = ItemSearch.SearchType.Trie)
    {
      this.searchType_ = searchType;
      this.ResetSearchMethod();
    }

    private void ResetSearchMethod()
    {
      switch (this.searchType_)
      {
        case ItemSearch.SearchType.Trie:
          this.searchMethod_ = (SearchMethodBase) new TrieSearch();
          break;
        default:
          this.searchMethod_ = (SearchMethodBase) new RegexSearch();
          break;
      }
    }

    public static string ProcessRawSearchTerm(string rawTerm, out string[] words)
    {
      string str = (rawTerm ?? "").Trim();
      ref string[] local = ref words;
      string[] strArray;
      if (!string.IsNullOrEmpty(str))
        strArray = ((IEnumerable<string>) str.ToLower().Split(' ')).Where<string>((Func<string, bool>) (part => !string.IsNullOrEmpty(part))).ToArray<string>();
      else
        strArray = new string[0];
      local = strArray;
      return words.Length == 0 ? "" : string.Join(" ", words);
    }

    public static string ProcessRawSearchTerm(string rawTerm)
    {
      string[] words = (string[]) null;
      return ItemSearch.ProcessRawSearchTerm(rawTerm, out words);
    }

    public void SetSearchUnits(IEnumerable<ItemSearch.SearchUnit> units)
    {
      this.SetSearchUnits(units.ToObservable<ItemSearch.SearchUnit>());
    }

    public void SetSearchUnits(IObservable<ItemSearch.SearchUnit> unitsObs)
    {
      lock (this.loadingLock_)
      {
        if (this.isLoading_)
          return;
        this.isLoading_ = true;
      }
      this.ClearData();
      if (unitsObs == null)
        unitsObs = Observable.Empty<ItemSearch.SearchUnit>();
      this.searchMethod_.IsReady = false;
      this.loadingProgress_ = unitsObs.ObserveOn<ItemSearch.SearchUnit>((IScheduler) AppState.Worker).Subscribe<ItemSearch.SearchUnit>((Action<ItemSearch.SearchUnit>) (unit => this.AddSearchUnitImpl(unit)), (Action) (() =>
      {
        this.searchMethod_.IsReady = true;
        lock (this.loadingLock_)
          this.isLoading_ = false;
        this.NotifyDataLoaded();
      }));
    }

    public void ClearData()
    {
      if (this.loadingProgress_ != null)
      {
        this.loadingProgress_.Dispose();
        this.loadingProgress_ = (IDisposable) null;
      }
      this.ResetSearchMethod();
      this.searchUnits_ = new List<ItemSearch.SearchUnit>();
    }

    public bool AddSearchUnit(ItemSearch.SearchUnit unit)
    {
      lock (this.loadingLock_)
      {
        if (this.isLoading_)
          return false;
        this.AddSearchUnitImpl(unit);
      }
      return true;
    }

    protected virtual void AddSearchUnitImpl(ItemSearch.SearchUnit unit)
    {
      this.searchUnits_.Add(unit);
      this.searchMethod_.Add(unit.SearchText, (object) unit);
    }

    public virtual void AbortCurrentSearch()
    {
      Log.p("item search", "abort current search | do nothing");
    }

    public virtual IObservable<ItemSearch.SearchResult> Lookup(string rawTerm)
    {
      return Observable.Create<ItemSearch.SearchResult>((Func<IObserver<ItemSearch.SearchResult>, Action>) (observer =>
      {
        Log.p("item search", "start | term={0}", (object) rawTerm);
        string[] words;
        string str = ItemSearch.ProcessRawSearchTerm(rawTerm, out words);
        ItemSearch.SearchResult searchResult = new ItemSearch.SearchResult()
        {
          RawSearchTerm = rawTerm,
          SearchTerm = str
        };
        try
        {
          searchResult.Matches = this.LookupCore(words);
          searchResult.Type = ItemSearch.SearchResult.ResultType.Success;
          searchResult.IsFinalResult = true;
        }
        catch (Exception ex)
        {
          searchResult.Type = ItemSearch.SearchResult.ResultType.Failed;
          searchResult.Matches = (IEnumerable<ItemSearch.Match>) new ItemSearch.Match[0];
        }
        Log.p("item search", "notify search results | type={0} matches={1}", (object) searchResult.Type, (object) searchResult.Matches.Count<ItemSearch.Match>());
        observer.OnNext(searchResult);
        Log.p("item search", "completed | term='{0}'", (object) rawTerm);
        observer.OnCompleted();
        return (Action) (() => Log.p("item search", "disposed | term='{0}'", (object) rawTerm));
      }));
    }

    private IEnumerable<ItemSearch.Match> LookupCore(string[] keywords)
    {
      if (keywords != null && keywords.Length != 0)
        return this.searchMethod_.Lookup(keywords).Where<SearchMethodBase.Match>((Func<SearchMethodBase.Match, bool>) (m => m != null && m.Tag is ItemSearch.SearchUnit)).Select<SearchMethodBase.Match, ItemSearch.Match>((Func<SearchMethodBase.Match, ItemSearch.Match>) (m => new ItemSearch.Match(m.Tag as ItemSearch.SearchUnit)
        {
          TextMatchStart = m.Start,
          TextMatchLength = m.End - m.Start
        }));
      return this.EmptySearchTermResultsFilter == null ? this.searchUnits_.Select<ItemSearch.SearchUnit, ItemSearch.Match>((Func<ItemSearch.SearchUnit, ItemSearch.Match>) (unit => new ItemSearch.Match(unit))) : this.searchUnits_.Where<ItemSearch.SearchUnit>((Func<ItemSearch.SearchUnit, bool>) (unit => this.EmptySearchTermResultsFilter(unit))).Select<ItemSearch.SearchUnit, ItemSearch.Match>((Func<ItemSearch.SearchUnit, ItemSearch.Match>) (unit => new ItemSearch.Match(unit)));
    }

    public enum SearchType
    {
      Trie,
      Regex,
    }

    public class SearchUnit
    {
      public string SearchText { get; private set; }

      public object Tag { get; set; }

      public SearchUnit(string searchText) => this.SearchText = searchText;
    }

    public class Match
    {
      public int TextMatchStart;
      public int TextMatchLength;
      public bool IsOriginal;

      public ItemSearch.SearchUnit SearchUnit { get; private set; }

      public Match(ItemSearch.SearchUnit unit) => this.SearchUnit = unit;
    }

    public class SearchResult
    {
      public ItemSearch.SearchResult.ResultType Type { get; set; }

      public IEnumerable<ItemSearch.Match> Matches { get; set; }

      public string SearchTerm { get; set; }

      public string RawSearchTerm { get; set; }

      public bool IsFinalResult { get; set; }

      public enum ResultType
      {
        Undefined,
        Success,
        Failed,
        Canceled,
      }
    }
  }
}
