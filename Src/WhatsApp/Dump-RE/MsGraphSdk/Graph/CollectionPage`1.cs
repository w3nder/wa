// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.CollectionPage`1
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class CollectionPage<T> : 
    ICollectionPage<T>,
    IList<T>,
    ICollection<T>,
    IEnumerable<T>,
    IEnumerable
  {
    public CollectionPage() => this.CurrentPage = (IList<T>) new List<T>();

    public CollectionPage(IList<T> currentPage) => this.CurrentPage = currentPage;

    public IList<T> CurrentPage { get; private set; }

    public int IndexOf(T item) => this.CurrentPage.IndexOf(item);

    public void Insert(int index, T item) => this.CurrentPage.Insert(index, item);

    public void RemoveAt(int index) => this.CurrentPage.RemoveAt(index);

    public T this[int index]
    {
      get => this.CurrentPage[index];
      set => this.CurrentPage[index] = value;
    }

    public void Add(T item) => this.CurrentPage.Add(item);

    public void Clear() => this.CurrentPage.Clear();

    public bool Contains(T item) => this.CurrentPage.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => this.CurrentPage.CopyTo(array, arrayIndex);

    public int Count => this.CurrentPage.Count;

    public bool IsReadOnly => this.CurrentPage.IsReadOnly;

    public bool Remove(T item) => this.CurrentPage.Remove(item);

    public IEnumerator<T> GetEnumerator() => this.CurrentPage.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.CurrentPage.GetEnumerator();

    public IDictionary<string, object> AdditionalData { get; set; }
  }
}
