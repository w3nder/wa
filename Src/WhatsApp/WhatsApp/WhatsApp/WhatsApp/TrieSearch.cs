// Decompiled with JetBrains decompiler
// Type: WhatsApp.TrieSearch
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public class TrieSearch : SearchMethodBase
  {
    private TrieSearch.TrieNode searchRoot = new TrieSearch.TrieNode();
    private int itemsBuiltIntoTrie;

    protected override void ProcessNewNode(SearchMethodBase.Node node)
    {
      this.BuildIndex(node.NodeId, node.Text, node.Tag);
    }

    public override IEnumerable<SearchMethodBase.Match> LookupImpl(string term)
    {
      if (!this.IsReady)
      {
        Log.p("item search", "not ready yet | items built into trie = {0}", (object) this.itemsBuiltIntoTrie);
        throw new InvalidOperationException("trie not ready yet");
      }
      TrieSearch.TrieNode searchRoot = this.searchRoot;
      for (int index = 0; index < term.Length && searchRoot != null; ++index)
      {
        char minValue = term[index];
        if (char.IsWhiteSpace(minValue))
          minValue = char.MinValue;
        searchRoot.Children.TryGetValue(minValue, out searchRoot);
      }
      return searchRoot != null ? (IEnumerable<SearchMethodBase.Match>) searchRoot.Matches : Enumerable.Empty<SearchMethodBase.Match>();
    }

    public override IEnumerable<SearchMethodBase.Match> LookupImpl(string[] keywords)
    {
      return this.Lookup(string.Join(" ", keywords));
    }

    private void BuildIndex(int nodeId, string text, object tag)
    {
      List<TrieSearch.SearchIndexBuilder> searchIndexBuilderList = new List<TrieSearch.SearchIndexBuilder>();
      char[] charArray = (text ?? "").ToLower().ToCharArray();
      bool flag = true;
      int length = charArray.Length;
      for (int index = 0; index < length; ++index)
      {
        char minValue = charArray[index];
        if (char.IsWhiteSpace(minValue) || minValue == char.MinValue)
        {
          if (!flag)
          {
            charArray[index] = minValue = char.MinValue;
            flag = true;
          }
          else
            continue;
        }
        else if (flag)
        {
          searchIndexBuilderList.Add(new TrieSearch.SearchIndexBuilder()
          {
            Last = this.searchRoot,
            StartingIndex = index
          });
          flag = false;
        }
        foreach (TrieSearch.SearchIndexBuilder searchIndexBuilder in searchIndexBuilderList)
        {
          TrieSearch.TrieNode last = searchIndexBuilder.Last;
          TrieSearch.TrieNode trieNode = (TrieSearch.TrieNode) null;
          if (!last.Children.TryGetValue(minValue, out trieNode))
          {
            trieNode = new TrieSearch.TrieNode();
            last.Children.Add(minValue, trieNode);
          }
          if (!trieNode.DeDup.Contains(nodeId))
          {
            trieNode.Matches.Add(new SearchMethodBase.Match()
            {
              Start = searchIndexBuilder.StartingIndex,
              End = index + 1,
              Tag = tag
            });
            trieNode.DeDup.Add(nodeId);
          }
          searchIndexBuilder.Last = trieNode;
        }
      }
      ++this.itemsBuiltIntoTrie;
    }

    public class TrieNode
    {
      public Dictionary<char, TrieSearch.TrieNode> Children = new Dictionary<char, TrieSearch.TrieNode>();
      public List<SearchMethodBase.Match> Matches = new List<SearchMethodBase.Match>();
      public Set<int> DeDup = new Set<int>();
    }

    public class SearchIndexBuilder
    {
      public int StartingIndex;
      public TrieSearch.TrieNode Last;
    }
  }
}
