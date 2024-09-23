// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Maps.Toolkit.MapItemsSourceChangeManager
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System.Collections.Specialized;

#nullable disable
namespace Microsoft.Phone.Maps.Toolkit
{
  internal class MapItemsSourceChangeManager : CollectionChangeListener<object>
  {
    public MapItemsSourceChangeManager(INotifyCollectionChanged sourceCollection)
    {
      this.SourceCollection = sourceCollection;
      this.SourceCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(((CollectionChangeListener<object>) this).CollectionChanged);
    }

    public MapChildCollection Items { get; set; }

    private INotifyCollectionChanged SourceCollection { get; set; }

    public void Disconnect()
    {
      this.SourceCollection.CollectionChanged -= new NotifyCollectionChangedEventHandler(((CollectionChangeListener<object>) this).CollectionChanged);
      this.SourceCollection = (INotifyCollectionChanged) null;
    }

    protected override void InsertItemInternal(int index, object obj)
    {
      this.Items.InsertInternal(index, obj);
    }

    protected override void RemoveItemInternal(object obj)
    {
      this.Items.RemoveInternal(this.Items.IndexOf(obj));
    }

    protected override void ResetInternal() => this.Items.ClearInternal();

    protected override void AddInternal(object obj) => this.Items.AddInternal(obj);

    protected override void MoveInternal(object obj, int newIndex)
    {
      this.Items.MoveInternal(this.Items.IndexOf(obj), newIndex);
    }
  }
}
