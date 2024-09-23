// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.HttpHeaderValueCollection`1
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

#nullable disable
namespace System.Net.Http.Headers
{
  /// <summary>Represents a collection of header values.</summary>
  /// <typeparam name="T"></typeparam>
  public sealed class HttpHeaderValueCollection<T> : ICollection<T>, IEnumerable<T>, IEnumerable where T : class
  {
    private string headerName;
    private HttpHeaders store;
    private T specialValue;
    private Action<HttpHeaderValueCollection<T>, T> validator;

    /// <returns>Returns <see cref="T:System.Int32" />.</returns>
    public int Count => this.GetCount();

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public bool IsReadOnly => false;

    internal bool IsSpecialValueSet
    {
      get
      {
        return (object) this.specialValue != null && this.store.ContainsParsedValue(this.headerName, (object) this.specialValue);
      }
    }

    internal HttpHeaderValueCollection(string headerName, HttpHeaders store)
      : this(headerName, store, default (T), (Action<HttpHeaderValueCollection<T>, T>) null)
    {
    }

    internal HttpHeaderValueCollection(
      string headerName,
      HttpHeaders store,
      Action<HttpHeaderValueCollection<T>, T> validator)
      : this(headerName, store, default (T), validator)
    {
    }

    internal HttpHeaderValueCollection(string headerName, HttpHeaders store, T specialValue)
      : this(headerName, store, specialValue, (Action<HttpHeaderValueCollection<T>, T>) null)
    {
    }

    internal HttpHeaderValueCollection(
      string headerName,
      HttpHeaders store,
      T specialValue,
      Action<HttpHeaderValueCollection<T>, T> validator)
    {
      Contract.Requires(headerName != null);
      Contract.Requires(store != null);
      this.store = store;
      this.headerName = headerName;
      this.specialValue = specialValue;
      this.validator = validator;
    }

    public void Add(T item)
    {
      this.CheckValue(item);
      this.store.AddParsedValue(this.headerName, (object) item);
    }

    public void ParseAdd(string input) => this.store.Add(this.headerName, input);

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public bool TryParseAdd(string input) => this.store.TryParseAndAddValue(this.headerName, input);

    public void Clear() => this.store.Remove(this.headerName);

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public bool Contains(T item)
    {
      this.CheckValue(item);
      return this.store.ContainsParsedValue(this.headerName, (object) item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      if (array == null)
        throw new ArgumentNullException(nameof (array));
      if (arrayIndex < 0 || arrayIndex > array.Length)
        throw new System.Net.Http.ArgumentOutOfRangeException(nameof (arrayIndex));
      object parsedValues = this.store.GetParsedValues(this.headerName);
      if (parsedValues == null)
        return;
      if (!(parsedValues is List<object> objectList))
      {
        Contract.Assert(parsedValues is T);
        if (arrayIndex == array.Length)
          throw new ArgumentException(SR.net_http_copyto_array_too_small);
        array[arrayIndex] = parsedValues as T;
      }
      else
        objectList.CopyTo((object[]) array, arrayIndex);
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public bool Remove(T item)
    {
      this.CheckValue(item);
      return this.store.RemoveParsedValue(this.headerName, (object) item);
    }

    /// <returns>Returns <see cref="T:System.Collections.Generic.IEnumerator`1" />.</returns>
    public IEnumerator<T> GetEnumerator()
    {
      object storeValue = this.store.GetParsedValues(this.headerName);
      if (storeValue != null)
      {
        if (!(storeValue is List<object> storeValues))
        {
          Contract.Assert(storeValue is T);
          yield return storeValue as T;
        }
        else
        {
          foreach (object item in storeValues)
          {
            Contract.Assert(item is T);
            yield return item as T;
          }
        }
      }
    }

    /// <returns>Returns <see cref="T:System.Collections.IEnumerator" />.</returns>
    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public override string ToString() => this.store.GetHeaderString(this.headerName);

    internal string GetHeaderStringWithoutSpecial()
    {
      return !this.IsSpecialValueSet ? this.ToString() : this.store.GetHeaderString(this.headerName, (object) this.specialValue);
    }

    internal void SetSpecialValue()
    {
      Contract.Assert((object) this.specialValue != null, "This method can only be used if the collection has a 'special value' set.");
      if (this.store.ContainsParsedValue(this.headerName, (object) this.specialValue))
        return;
      this.store.AddParsedValue(this.headerName, (object) this.specialValue);
    }

    internal void RemoveSpecialValue()
    {
      Contract.Assert((object) this.specialValue != null, "This method can only be used if the collection has a 'special value' set.");
      this.store.RemoveParsedValue(this.headerName, (object) this.specialValue);
    }

    private void CheckValue(T item)
    {
      if ((object) item == null)
        throw new ArgumentNullException(nameof (item));
      if (this.validator == null)
        return;
      this.validator(this, item);
    }

    private int GetCount()
    {
      object parsedValues = this.store.GetParsedValues(this.headerName);
      if (parsedValues == null)
        return 0;
      return !(parsedValues is List<object> objectList) ? 1 : objectList.Count;
    }
  }
}
