// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JContainer
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

#nullable disable
namespace Newtonsoft.Json.Linq
{
  /// <summary>Represents a token that can contain other tokens.</summary>
  public abstract class JContainer : 
    JToken,
    IList<JToken>,
    ICollection<JToken>,
    IEnumerable<JToken>,
    IList,
    ICollection,
    IEnumerable
  {
    private object _syncRoot;

    /// <summary>Gets the container's children tokens.</summary>
    /// <value>The container's children tokens.</value>
    protected abstract IList<JToken> ChildrenTokens { get; }

    internal JContainer()
    {
    }

    internal JContainer(JContainer other)
      : this()
    {
      ValidationUtils.ArgumentNotNull((object) other, "c");
      int index = 0;
      foreach (JToken content in (IEnumerable<JToken>) other)
      {
        this.AddInternal(index, (object) content, false);
        ++index;
      }
    }

    internal void CheckReentrancy()
    {
    }

    internal virtual IList<JToken> CreateChildrenCollection() => (IList<JToken>) new List<JToken>();

    /// <summary>
    /// Gets a value indicating whether this token has child tokens.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this token has child values; otherwise, <c>false</c>.
    /// </value>
    public override bool HasValues => this.ChildrenTokens.Count > 0;

    internal bool ContentsEqual(JContainer container)
    {
      if (container == this)
        return true;
      IList<JToken> childrenTokens1 = this.ChildrenTokens;
      IList<JToken> childrenTokens2 = container.ChildrenTokens;
      if (childrenTokens1.Count != childrenTokens2.Count)
        return false;
      for (int index = 0; index < childrenTokens1.Count; ++index)
      {
        if (!childrenTokens1[index].DeepEquals(childrenTokens2[index]))
          return false;
      }
      return true;
    }

    /// <summary>Get the first child token of this token.</summary>
    /// <value>
    /// A <see cref="T:Newtonsoft.Json.Linq.JToken" /> containing the first child token of the <see cref="T:Newtonsoft.Json.Linq.JToken" />.
    /// </value>
    public override JToken First => this.ChildrenTokens.FirstOrDefault<JToken>();

    /// <summary>Get the last child token of this token.</summary>
    /// <value>
    /// A <see cref="T:Newtonsoft.Json.Linq.JToken" /> containing the last child token of the <see cref="T:Newtonsoft.Json.Linq.JToken" />.
    /// </value>
    public override JToken Last => this.ChildrenTokens.LastOrDefault<JToken>();

    /// <summary>
    /// Returns a collection of the child tokens of this token, in document order.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> containing the child tokens of this <see cref="T:Newtonsoft.Json.Linq.JToken" />, in document order.
    /// </returns>
    public override JEnumerable<JToken> Children()
    {
      return new JEnumerable<JToken>((IEnumerable<JToken>) this.ChildrenTokens);
    }

    /// <summary>
    /// Returns a collection of the child values of this token, in document order.
    /// </summary>
    /// <typeparam name="T">The type to convert the values to.</typeparam>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerable`1" /> containing the child values of this <see cref="T:Newtonsoft.Json.Linq.JToken" />, in document order.
    /// </returns>
    public override IEnumerable<T> Values<T>() => this.ChildrenTokens.Convert<JToken, T>();

    /// <summary>
    /// Returns a collection of the descendant tokens for this token in document order.
    /// </summary>
    /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> containing the descendant tokens of the <see cref="T:Newtonsoft.Json.Linq.JToken" />.</returns>
    public IEnumerable<JToken> Descendants() => this.GetDescendants(false);

    /// <summary>
    /// Returns a collection of the tokens that contain this token, and all descendant tokens of this token, in document order.
    /// </summary>
    /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> containing this token, and all the descendant tokens of the <see cref="T:Newtonsoft.Json.Linq.JToken" />.</returns>
    public IEnumerable<JToken> DescendantsAndSelf() => this.GetDescendants(true);

    internal IEnumerable<JToken> GetDescendants(bool self)
    {
      if (self)
        yield return (JToken) this;
      foreach (JToken o in (IEnumerable<JToken>) this.ChildrenTokens)
      {
        yield return o;
        if (o is JContainer c)
        {
          foreach (JToken d in c.Descendants())
            yield return d;
        }
      }
    }

    internal bool IsMultiContent(object content)
    {
      return content is IEnumerable && !(content is string) && !(content is JToken) && !(content is byte[]);
    }

    internal JToken EnsureParentToken(JToken item, bool skipParentCheck)
    {
      if (item == null)
        return (JToken) JValue.CreateNull();
      if (skipParentCheck || item.Parent == null && item != this && (!item.HasValues || this.Root != item))
        return item;
      item = item.CloneToken();
      return item;
    }

    internal int IndexOfItem(JToken item)
    {
      return this.ChildrenTokens.IndexOf<JToken>(item, (IEqualityComparer<JToken>) JContainer.JTokenReferenceEqualityComparer.Instance);
    }

    internal virtual void InsertItem(int index, JToken item, bool skipParentCheck)
    {
      if (index > this.ChildrenTokens.Count)
        throw new ArgumentOutOfRangeException(nameof (index), "Index must be within the bounds of the List.");
      this.CheckReentrancy();
      item = this.EnsureParentToken(item, skipParentCheck);
      JToken childrenToken1 = index == 0 ? (JToken) null : this.ChildrenTokens[index - 1];
      JToken childrenToken2 = index == this.ChildrenTokens.Count ? (JToken) null : this.ChildrenTokens[index];
      this.ValidateToken(item, (JToken) null);
      item.Parent = this;
      item.Previous = childrenToken1;
      if (childrenToken1 != null)
        childrenToken1.Next = item;
      item.Next = childrenToken2;
      if (childrenToken2 != null)
        childrenToken2.Previous = item;
      this.ChildrenTokens.Insert(index, item);
    }

    internal virtual void RemoveItemAt(int index)
    {
      if (index < 0)
        throw new ArgumentOutOfRangeException(nameof (index), "Index is less than 0.");
      if (index >= this.ChildrenTokens.Count)
        throw new ArgumentOutOfRangeException(nameof (index), "Index is equal to or greater than Count.");
      this.CheckReentrancy();
      JToken childrenToken1 = this.ChildrenTokens[index];
      JToken childrenToken2 = index == 0 ? (JToken) null : this.ChildrenTokens[index - 1];
      JToken childrenToken3 = index == this.ChildrenTokens.Count - 1 ? (JToken) null : this.ChildrenTokens[index + 1];
      if (childrenToken2 != null)
        childrenToken2.Next = childrenToken3;
      if (childrenToken3 != null)
        childrenToken3.Previous = childrenToken2;
      childrenToken1.Parent = (JContainer) null;
      childrenToken1.Previous = (JToken) null;
      childrenToken1.Next = (JToken) null;
      this.ChildrenTokens.RemoveAt(index);
    }

    internal virtual bool RemoveItem(JToken item)
    {
      int index = this.IndexOfItem(item);
      if (index < 0)
        return false;
      this.RemoveItemAt(index);
      return true;
    }

    internal virtual JToken GetItem(int index) => this.ChildrenTokens[index];

    internal virtual void SetItem(int index, JToken item)
    {
      if (index < 0)
        throw new ArgumentOutOfRangeException(nameof (index), "Index is less than 0.");
      if (index >= this.ChildrenTokens.Count)
        throw new ArgumentOutOfRangeException(nameof (index), "Index is equal to or greater than Count.");
      JToken childrenToken1 = this.ChildrenTokens[index];
      if (JContainer.IsTokenUnchanged(childrenToken1, item))
        return;
      this.CheckReentrancy();
      item = this.EnsureParentToken(item, false);
      this.ValidateToken(item, childrenToken1);
      JToken childrenToken2 = index == 0 ? (JToken) null : this.ChildrenTokens[index - 1];
      JToken childrenToken3 = index == this.ChildrenTokens.Count - 1 ? (JToken) null : this.ChildrenTokens[index + 1];
      item.Parent = this;
      item.Previous = childrenToken2;
      if (childrenToken2 != null)
        childrenToken2.Next = item;
      item.Next = childrenToken3;
      if (childrenToken3 != null)
        childrenToken3.Previous = item;
      this.ChildrenTokens[index] = item;
      childrenToken1.Parent = (JContainer) null;
      childrenToken1.Previous = (JToken) null;
      childrenToken1.Next = (JToken) null;
    }

    internal virtual void ClearItems()
    {
      this.CheckReentrancy();
      foreach (JToken childrenToken in (IEnumerable<JToken>) this.ChildrenTokens)
      {
        childrenToken.Parent = (JContainer) null;
        childrenToken.Previous = (JToken) null;
        childrenToken.Next = (JToken) null;
      }
      this.ChildrenTokens.Clear();
    }

    internal virtual void ReplaceItem(JToken existing, JToken replacement)
    {
      if (existing == null || existing.Parent != this)
        return;
      this.SetItem(this.IndexOfItem(existing), replacement);
    }

    internal virtual bool ContainsItem(JToken item) => this.IndexOfItem(item) != -1;

    internal virtual void CopyItemsTo(Array array, int arrayIndex)
    {
      if (array == null)
        throw new ArgumentNullException(nameof (array));
      if (arrayIndex < 0)
        throw new ArgumentOutOfRangeException(nameof (arrayIndex), "arrayIndex is less than 0.");
      if (arrayIndex >= array.Length && arrayIndex != 0)
        throw new ArgumentException("arrayIndex is equal to or greater than the length of array.");
      if (this.Count > array.Length - arrayIndex)
        throw new ArgumentException("The number of elements in the source JObject is greater than the available space from arrayIndex to the end of the destination array.");
      int num = 0;
      foreach (JToken childrenToken in (IEnumerable<JToken>) this.ChildrenTokens)
      {
        array.SetValue((object) childrenToken, arrayIndex + num);
        ++num;
      }
    }

    internal static bool IsTokenUnchanged(JToken currentValue, JToken newValue)
    {
      if (!(currentValue is JValue jvalue))
        return false;
      return jvalue.Type == JTokenType.Null && newValue == null || jvalue.Equals((object) newValue);
    }

    internal virtual void ValidateToken(JToken o, JToken existing)
    {
      ValidationUtils.ArgumentNotNull((object) o, nameof (o));
      if (o.Type == JTokenType.Property)
        throw new ArgumentException("Can not add {0} to {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) o.GetType(), (object) this.GetType()));
    }

    /// <summary>
    /// Adds the specified content as children of this <see cref="T:Newtonsoft.Json.Linq.JToken" />.
    /// </summary>
    /// <param name="content">The content to be added.</param>
    public virtual void Add(object content)
    {
      this.AddInternal(this.ChildrenTokens.Count, content, false);
    }

    internal void AddAndSkipParentCheck(JToken token)
    {
      this.AddInternal(this.ChildrenTokens.Count, (object) token, true);
    }

    /// <summary>
    /// Adds the specified content as the first children of this <see cref="T:Newtonsoft.Json.Linq.JToken" />.
    /// </summary>
    /// <param name="content">The content to be added.</param>
    public void AddFirst(object content) => this.AddInternal(0, content, false);

    internal void AddInternal(int index, object content, bool skipParentCheck)
    {
      if (this.IsMultiContent(content))
      {
        IEnumerable enumerable = (IEnumerable) content;
        int index1 = index;
        foreach (object content1 in enumerable)
        {
          this.AddInternal(index1, content1, skipParentCheck);
          ++index1;
        }
      }
      else
      {
        JToken fromContent = JContainer.CreateFromContent(content);
        this.InsertItem(index, fromContent, skipParentCheck);
      }
    }

    internal static JToken CreateFromContent(object content)
    {
      return content is JToken ? (JToken) content : (JToken) new JValue(content);
    }

    /// <summary>
    /// Creates an <see cref="T:Newtonsoft.Json.JsonWriter" /> that can be used to add tokens to the <see cref="T:Newtonsoft.Json.Linq.JToken" />.
    /// </summary>
    /// <returns>An <see cref="T:Newtonsoft.Json.JsonWriter" /> that is ready to have content written to it.</returns>
    public JsonWriter CreateWriter() => (JsonWriter) new JTokenWriter(this);

    /// <summary>
    /// Replaces the children nodes of this token with the specified content.
    /// </summary>
    /// <param name="content">The content.</param>
    public void ReplaceAll(object content)
    {
      this.ClearItems();
      this.Add(content);
    }

    /// <summary>Removes the child nodes from this token.</summary>
    public void RemoveAll() => this.ClearItems();

    internal abstract void MergeItem(object content, JsonMergeSettings settings);

    /// <summary>
    /// Merge the specified content into this <see cref="T:Newtonsoft.Json.Linq.JToken" />.
    /// </summary>
    /// <param name="content">The content to be merged.</param>
    public void Merge(object content) => this.MergeItem(content, new JsonMergeSettings());

    /// <summary>
    /// Merge the specified content into this <see cref="T:Newtonsoft.Json.Linq.JToken" /> using <see cref="T:Newtonsoft.Json.Linq.JsonMergeSettings" />.
    /// </summary>
    /// <param name="content">The content to be merged.</param>
    /// <param name="settings">The <see cref="T:Newtonsoft.Json.Linq.JsonMergeSettings" /> used to merge the content.</param>
    public void Merge(object content, JsonMergeSettings settings)
    {
      this.MergeItem(content, settings);
    }

    internal void ReadTokenFrom(JsonReader reader)
    {
      int depth = reader.Depth;
      if (!reader.Read())
        throw JsonReaderException.Create(reader, "Error reading {0} from JsonReader.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) this.GetType().Name));
      this.ReadContentFrom(reader);
      if (reader.Depth > depth)
        throw JsonReaderException.Create(reader, "Unexpected end of content while loading {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) this.GetType().Name));
    }

    internal void ReadContentFrom(JsonReader r)
    {
      ValidationUtils.ArgumentNotNull((object) r, nameof (r));
      IJsonLineInfo lineInfo = r as IJsonLineInfo;
      JContainer jcontainer = this;
      do
      {
        if (jcontainer is JProperty && ((JProperty) jcontainer).Value != null)
        {
          if (jcontainer == this)
            break;
          jcontainer = jcontainer.Parent;
        }
        switch (r.TokenType)
        {
          case JsonToken.None:
            continue;
          case JsonToken.StartObject:
            JObject content1 = new JObject();
            content1.SetLineInfo(lineInfo);
            jcontainer.Add((object) content1);
            jcontainer = (JContainer) content1;
            goto case JsonToken.None;
          case JsonToken.StartArray:
            JArray content2 = new JArray();
            content2.SetLineInfo(lineInfo);
            jcontainer.Add((object) content2);
            jcontainer = (JContainer) content2;
            goto case JsonToken.None;
          case JsonToken.StartConstructor:
            JConstructor content3 = new JConstructor(r.Value.ToString());
            content3.SetLineInfo(lineInfo);
            jcontainer.Add((object) content3);
            jcontainer = (JContainer) content3;
            goto case JsonToken.None;
          case JsonToken.PropertyName:
            string name = r.Value.ToString();
            JProperty content4 = new JProperty(name);
            content4.SetLineInfo(lineInfo);
            JProperty jproperty = ((JObject) jcontainer).Property(name);
            if (jproperty == null)
              jcontainer.Add((object) content4);
            else
              jproperty.Replace((JToken) content4);
            jcontainer = (JContainer) content4;
            goto case JsonToken.None;
          case JsonToken.Comment:
            JValue comment = JValue.CreateComment(r.Value.ToString());
            comment.SetLineInfo(lineInfo);
            jcontainer.Add((object) comment);
            goto case JsonToken.None;
          case JsonToken.Integer:
          case JsonToken.Float:
          case JsonToken.String:
          case JsonToken.Boolean:
          case JsonToken.Date:
          case JsonToken.Bytes:
            JValue content5 = new JValue(r.Value);
            content5.SetLineInfo(lineInfo);
            jcontainer.Add((object) content5);
            goto case JsonToken.None;
          case JsonToken.Null:
            JValue content6 = JValue.CreateNull();
            content6.SetLineInfo(lineInfo);
            jcontainer.Add((object) content6);
            goto case JsonToken.None;
          case JsonToken.Undefined:
            JValue undefined = JValue.CreateUndefined();
            undefined.SetLineInfo(lineInfo);
            jcontainer.Add((object) undefined);
            goto case JsonToken.None;
          case JsonToken.EndObject:
            if (jcontainer == this)
              return;
            jcontainer = jcontainer.Parent;
            goto case JsonToken.None;
          case JsonToken.EndArray:
            if (jcontainer == this)
              return;
            jcontainer = jcontainer.Parent;
            goto case JsonToken.None;
          case JsonToken.EndConstructor:
            if (jcontainer == this)
              return;
            jcontainer = jcontainer.Parent;
            goto case JsonToken.None;
          default:
            throw new InvalidOperationException("The JsonReader should not be on a token of type {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) r.TokenType));
        }
      }
      while (r.Read());
    }

    internal int ContentsHashCode()
    {
      int num = 0;
      foreach (JToken childrenToken in (IEnumerable<JToken>) this.ChildrenTokens)
        num ^= childrenToken.GetDeepHashCode();
      return num;
    }

    int IList<JToken>.IndexOf(JToken item) => this.IndexOfItem(item);

    void IList<JToken>.Insert(int index, JToken item) => this.InsertItem(index, item, false);

    void IList<JToken>.RemoveAt(int index) => this.RemoveItemAt(index);

    JToken IList<JToken>.this[int index]
    {
      get => this.GetItem(index);
      set => this.SetItem(index, value);
    }

    void ICollection<JToken>.Add(JToken item) => this.Add((object) item);

    void ICollection<JToken>.Clear() => this.ClearItems();

    bool ICollection<JToken>.Contains(JToken item) => this.ContainsItem(item);

    void ICollection<JToken>.CopyTo(JToken[] array, int arrayIndex)
    {
      this.CopyItemsTo((Array) array, arrayIndex);
    }

    bool ICollection<JToken>.IsReadOnly => false;

    bool ICollection<JToken>.Remove(JToken item) => this.RemoveItem(item);

    private JToken EnsureValue(object value)
    {
      if (value == null)
        return (JToken) null;
      return value is JToken ? (JToken) value : throw new ArgumentException("Argument is not a JToken.");
    }

    int IList.Add(object value)
    {
      this.Add((object) this.EnsureValue(value));
      return this.Count - 1;
    }

    void IList.Clear() => this.ClearItems();

    bool IList.Contains(object value) => this.ContainsItem(this.EnsureValue(value));

    int IList.IndexOf(object value) => this.IndexOfItem(this.EnsureValue(value));

    void IList.Insert(int index, object value)
    {
      this.InsertItem(index, this.EnsureValue(value), false);
    }

    bool IList.IsFixedSize => false;

    bool IList.IsReadOnly => false;

    void IList.Remove(object value) => this.RemoveItem(this.EnsureValue(value));

    void IList.RemoveAt(int index) => this.RemoveItemAt(index);

    object IList.this[int index]
    {
      get => (object) this.GetItem(index);
      set => this.SetItem(index, this.EnsureValue(value));
    }

    void ICollection.CopyTo(Array array, int index) => this.CopyItemsTo(array, index);

    /// <summary>Gets the count of child JSON tokens.</summary>
    /// <value>The count of child JSON tokens</value>
    public int Count => this.ChildrenTokens.Count;

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot
    {
      get
      {
        if (this._syncRoot == null)
          Interlocked.CompareExchange(ref this._syncRoot, new object(), (object) null);
        return this._syncRoot;
      }
    }

    internal static void MergeEnumerableContent(
      JContainer target,
      IEnumerable content,
      JsonMergeSettings settings)
    {
      switch (settings.MergeArrayHandling)
      {
        case MergeArrayHandling.Concat:
          IEnumerator enumerator1 = content.GetEnumerator();
          try
          {
            while (enumerator1.MoveNext())
            {
              JToken current = (JToken) enumerator1.Current;
              target.Add((object) current);
            }
            break;
          }
          finally
          {
            if (enumerator1 is IDisposable disposable)
              disposable.Dispose();
          }
        case MergeArrayHandling.Union:
          HashSet<JToken> jtokenSet = new HashSet<JToken>((IEnumerable<JToken>) target, (IEqualityComparer<JToken>) JToken.EqualityComparer);
          IEnumerator enumerator2 = content.GetEnumerator();
          try
          {
            while (enumerator2.MoveNext())
            {
              JToken current = (JToken) enumerator2.Current;
              if (jtokenSet.Add(current))
                target.Add((object) current);
            }
            break;
          }
          finally
          {
            if (enumerator2 is IDisposable disposable)
              disposable.Dispose();
          }
        case MergeArrayHandling.Replace:
          target.ClearItems();
          IEnumerator enumerator3 = content.GetEnumerator();
          try
          {
            while (enumerator3.MoveNext())
            {
              JToken current = (JToken) enumerator3.Current;
              target.Add((object) current);
            }
            break;
          }
          finally
          {
            if (enumerator3 is IDisposable disposable)
              disposable.Dispose();
          }
        case MergeArrayHandling.Merge:
          int key = 0;
          IEnumerator enumerator4 = content.GetEnumerator();
          try
          {
            while (enumerator4.MoveNext())
            {
              object current = enumerator4.Current;
              if (key < target.Count)
              {
                if (target[(object) key] is JContainer jcontainer)
                  jcontainer.Merge(current, settings);
                else if (current != null)
                {
                  JToken fromContent = JContainer.CreateFromContent(current);
                  if (fromContent.Type != JTokenType.Null)
                    target[(object) key] = fromContent;
                }
              }
              else
                target.Add(current);
              ++key;
            }
            break;
          }
          finally
          {
            if (enumerator4 is IDisposable disposable)
              disposable.Dispose();
          }
        default:
          throw new ArgumentOutOfRangeException(nameof (settings), "Unexpected merge array handling when merging JSON.");
      }
    }

    private class JTokenReferenceEqualityComparer : IEqualityComparer<JToken>
    {
      public static readonly JContainer.JTokenReferenceEqualityComparer Instance = new JContainer.JTokenReferenceEqualityComparer();

      public bool Equals(JToken x, JToken y) => object.ReferenceEquals((object) x, (object) y);

      public int GetHashCode(JToken obj) => obj == null ? 0 : obj.GetHashCode();
    }
  }
}
