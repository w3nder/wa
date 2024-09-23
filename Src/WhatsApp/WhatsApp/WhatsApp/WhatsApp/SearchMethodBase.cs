// Decompiled with JetBrains decompiler
// Type: WhatsApp.SearchMethodBase
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Linq;


namespace WhatsApp
{
  public abstract class SearchMethodBase
  {
    protected object accessLock_ = new object();
    private int idCounter_;
    protected LinkedList<SearchMethodBase.Node> nodes_ = new LinkedList<SearchMethodBase.Node>();
    private bool isReady_ = true;

    public bool IsReady
    {
      get => this.isReady_;
      set => this.isReady_ = value;
    }

    public void Init(KeyValuePair<string, object>[] items)
    {
      lock (this.accessLock_)
      {
        this.nodes_ = new LinkedList<SearchMethodBase.Node>(((IEnumerable<KeyValuePair<string, object>>) items).Select<KeyValuePair<string, object>, SearchMethodBase.Node>((Func<KeyValuePair<string, object>, SearchMethodBase.Node>) (p => new SearchMethodBase.Node(++this.idCounter_, p.Key, p.Value))));
        foreach (SearchMethodBase.Node node in this.nodes_)
          this.ProcessNewNode(node);
      }
    }

    public void Add(string text, object tag)
    {
      lock (this.accessLock_)
      {
        SearchMethodBase.Node node = new SearchMethodBase.Node(++this.idCounter_, text, tag);
        this.nodes_.AddLast(node);
        this.ProcessNewNode(node);
      }
    }

    protected virtual void ProcessNewNode(SearchMethodBase.Node node)
    {
    }

    private static string Normalize(string term)
    {
      return NativeInterfaces.Misc.NormalizeUnicodeString(term ?? "", true).ToLower();
    }

    public IEnumerable<SearchMethodBase.Match> Lookup(string term)
    {
      return this.LookupImpl(SearchMethodBase.Normalize(term));
    }

    public IEnumerable<SearchMethodBase.Match> Lookup(string[] keywords)
    {
      return this.LookupImpl(((IEnumerable<string>) keywords).Select<string, string>(new Func<string, string>(SearchMethodBase.Normalize)).ToArray<string>());
    }

    public abstract IEnumerable<SearchMethodBase.Match> LookupImpl(string term);

    public abstract IEnumerable<SearchMethodBase.Match> LookupImpl(string[] keywords);

    public class Match
    {
      public int Start;
      public int End;

      public object Tag { get; set; }
    }

    protected class Node
    {
      public string Text;
      public object Tag;
      public int NodeId;

      public Node(int nodeId, string text, object tag)
      {
        this.NodeId = nodeId;
        this.Text = SearchMethodBase.Normalize(text);
        this.Tag = tag;
      }
    }
  }
}
