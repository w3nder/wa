// Decompiled with JetBrains decompiler
// Type: ZXing.Common.DecodingOptions
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

#nullable disable
namespace ZXing.Common
{
  /// <summary>Defines an container for encoder options</summary>
  [Serializable]
  public class DecodingOptions
  {
    /// <summary>Gets the data container for all options</summary>
    [Browsable(false)]
    public IDictionary<DecodeHintType, object> Hints { get; private set; }

    [field: NonSerialized]
    public event Action<object, EventArgs> ValueChanged;

    /// <summary>
    /// Gets or sets a flag which cause a deeper look into the bitmap
    /// </summary>
    /// <value>
    ///   <c>true</c> if [try harder]; otherwise, <c>false</c>.
    /// </value>
    public bool TryHarder
    {
      get
      {
        return this.Hints.ContainsKey(DecodeHintType.TRY_HARDER) && (bool) this.Hints[DecodeHintType.TRY_HARDER];
      }
      set
      {
        if (value)
        {
          this.Hints[DecodeHintType.TRY_HARDER] = (object) true;
        }
        else
        {
          if (!this.Hints.ContainsKey(DecodeHintType.TRY_HARDER))
            return;
          this.Hints.Remove(DecodeHintType.TRY_HARDER);
        }
      }
    }

    /// <summary>Image is a pure monochrome image of a barcode.</summary>
    /// <value>
    ///   <c>true</c> if monochrome image of a barcode; otherwise, <c>false</c>.
    /// </value>
    public bool PureBarcode
    {
      get
      {
        return this.Hints.ContainsKey(DecodeHintType.PURE_BARCODE) && (bool) this.Hints[DecodeHintType.PURE_BARCODE];
      }
      set
      {
        if (value)
        {
          this.Hints[DecodeHintType.PURE_BARCODE] = (object) true;
        }
        else
        {
          if (!this.Hints.ContainsKey(DecodeHintType.PURE_BARCODE))
            return;
          this.Hints.Remove(DecodeHintType.PURE_BARCODE);
        }
      }
    }

    /// <summary>
    /// Specifies what character encoding to use when decoding, where applicable (type String)
    /// </summary>
    /// <value>The character set.</value>
    public string CharacterSet
    {
      get
      {
        return this.Hints.ContainsKey(DecodeHintType.CHARACTER_SET) ? (string) this.Hints[DecodeHintType.CHARACTER_SET] : (string) null;
      }
      set
      {
        if (value != null)
        {
          this.Hints[DecodeHintType.CHARACTER_SET] = (object) value;
        }
        else
        {
          if (!this.Hints.ContainsKey(DecodeHintType.CHARACTER_SET))
            return;
          this.Hints.Remove(DecodeHintType.CHARACTER_SET);
        }
      }
    }

    /// <summary>
    /// Image is known to be of one of a few possible formats.
    /// Maps to a {@link java.util.List} of {@link BarcodeFormat}s.
    /// </summary>
    /// <value>The possible formats.</value>
    public IList<BarcodeFormat> PossibleFormats
    {
      get
      {
        return this.Hints.ContainsKey(DecodeHintType.POSSIBLE_FORMATS) ? (IList<BarcodeFormat>) this.Hints[DecodeHintType.POSSIBLE_FORMATS] : (IList<BarcodeFormat>) null;
      }
      set
      {
        if (value != null)
        {
          this.Hints[DecodeHintType.POSSIBLE_FORMATS] = (object) value;
        }
        else
        {
          if (!this.Hints.ContainsKey(DecodeHintType.POSSIBLE_FORMATS))
            return;
          this.Hints.Remove(DecodeHintType.POSSIBLE_FORMATS);
        }
      }
    }

    /// <summary>
    /// if Code39 could be detected try to use extended mode for full ASCII character set
    /// </summary>
    public bool UseCode39ExtendedMode
    {
      get
      {
        return this.Hints.ContainsKey(DecodeHintType.USE_CODE_39_EXTENDED_MODE) && (bool) this.Hints[DecodeHintType.USE_CODE_39_EXTENDED_MODE];
      }
      set
      {
        if (value)
        {
          this.Hints[DecodeHintType.USE_CODE_39_EXTENDED_MODE] = (object) true;
        }
        else
        {
          if (!this.Hints.ContainsKey(DecodeHintType.USE_CODE_39_EXTENDED_MODE))
            return;
          this.Hints.Remove(DecodeHintType.USE_CODE_39_EXTENDED_MODE);
        }
      }
    }

    /// <summary>
    /// Don't fail if a Code39 is detected but can't be decoded in extended mode.
    /// Return the raw Code39 result instead. Maps to <see cref="T:System.Boolean" />.
    /// </summary>
    public bool UseCode39RelaxedExtendedMode
    {
      get
      {
        return this.Hints.ContainsKey(DecodeHintType.RELAXED_CODE_39_EXTENDED_MODE) && (bool) this.Hints[DecodeHintType.RELAXED_CODE_39_EXTENDED_MODE];
      }
      set
      {
        if (value)
        {
          this.Hints[DecodeHintType.RELAXED_CODE_39_EXTENDED_MODE] = (object) true;
        }
        else
        {
          if (!this.Hints.ContainsKey(DecodeHintType.RELAXED_CODE_39_EXTENDED_MODE))
            return;
          this.Hints.Remove(DecodeHintType.RELAXED_CODE_39_EXTENDED_MODE);
        }
      }
    }

    /// <summary>
    /// If true, return the start and end digits in a Codabar barcode instead of stripping them. They
    /// are alpha, whereas the rest are numeric. By default, they are stripped, but this causes them
    /// to not be. Doesn't matter what it maps to; use <see cref="T:System.Boolean" />.
    /// </summary>
    public bool ReturnCodabarStartEnd
    {
      get
      {
        return this.Hints.ContainsKey(DecodeHintType.RETURN_CODABAR_START_END) && (bool) this.Hints[DecodeHintType.RETURN_CODABAR_START_END];
      }
      set
      {
        if (value)
        {
          this.Hints[DecodeHintType.RETURN_CODABAR_START_END] = (object) true;
        }
        else
        {
          if (!this.Hints.ContainsKey(DecodeHintType.RETURN_CODABAR_START_END))
            return;
          this.Hints.Remove(DecodeHintType.RETURN_CODABAR_START_END);
        }
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.Common.EncodingOptions" /> class.
    /// </summary>
    public DecodingOptions()
    {
      DecodingOptions.ChangeNotifyDictionary<DecodeHintType, object> notifyDictionary = new DecodingOptions.ChangeNotifyDictionary<DecodeHintType, object>();
      this.Hints = (IDictionary<DecodeHintType, object>) notifyDictionary;
      this.UseCode39ExtendedMode = true;
      this.UseCode39RelaxedExtendedMode = true;
      notifyDictionary.ValueChanged += (Action<object, EventArgs>) ((o, args) =>
      {
        if (this.ValueChanged == null)
          return;
        this.ValueChanged((object) this, EventArgs.Empty);
      });
    }

    [Serializable]
    private class ChangeNotifyDictionary<TKey, TValue> : 
      IDictionary<TKey, TValue>,
      ICollection<KeyValuePair<TKey, TValue>>,
      IEnumerable<KeyValuePair<TKey, TValue>>,
      IEnumerable
    {
      private readonly IDictionary<TKey, TValue> values;

      [field: NonSerialized]
      public event Action<object, EventArgs> ValueChanged;

      public ChangeNotifyDictionary()
      {
        this.values = (IDictionary<TKey, TValue>) new Dictionary<TKey, TValue>();
      }

      private void OnValueChanged()
      {
        if (this.ValueChanged == null)
          return;
        this.ValueChanged((object) this, EventArgs.Empty);
      }

      public void Add(TKey key, TValue value)
      {
        this.values.Add(key, value);
        this.OnValueChanged();
      }

      public bool ContainsKey(TKey key) => this.values.ContainsKey(key);

      public ICollection<TKey> Keys => this.values.Keys;

      public bool Remove(TKey key)
      {
        bool flag = this.values.Remove(key);
        this.OnValueChanged();
        return flag;
      }

      public bool TryGetValue(TKey key, out TValue value)
      {
        return this.values.TryGetValue(key, out value);
      }

      public ICollection<TValue> Values => this.values.Values;

      public TValue this[TKey key]
      {
        get => this.values[key];
        set
        {
          this.values[key] = value;
          this.OnValueChanged();
        }
      }

      public void Add(KeyValuePair<TKey, TValue> item)
      {
        this.values.Add(item);
        this.OnValueChanged();
      }

      public void Clear()
      {
        this.values.Clear();
        this.OnValueChanged();
      }

      public bool Contains(KeyValuePair<TKey, TValue> item) => this.values.Contains(item);

      public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
      {
        this.values.CopyTo(array, arrayIndex);
      }

      public int Count => this.values.Count;

      public bool IsReadOnly => this.values.IsReadOnly;

      public bool Remove(KeyValuePair<TKey, TValue> item)
      {
        bool flag = ((ICollection<KeyValuePair<TKey, TValue>>) this.values).Remove(item);
        this.OnValueChanged();
        return flag;
      }

      public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => this.values.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => this.values.GetEnumerator();
    }
  }
}
