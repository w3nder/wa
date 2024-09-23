// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.ObjectCollection`1
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Collections.ObjectModel;

#nullable disable
namespace System.Net.Http.Headers
{
  internal class ObjectCollection<T> : Collection<T> where T : class
  {
    private static readonly Action<T> defaultValidator = new Action<T>(ObjectCollection<T>.CheckNotNull);
    private Action<T> validator;

    public ObjectCollection()
      : this(ObjectCollection<T>.defaultValidator)
    {
    }

    public ObjectCollection(Action<T> validator) => this.validator = validator;

    protected override void InsertItem(int index, T item)
    {
      if (this.validator != null)
        this.validator(item);
      base.InsertItem(index, item);
    }

    protected override void SetItem(int index, T item)
    {
      if (this.validator != null)
        this.validator(item);
      base.SetItem(index, item);
    }

    private static void CheckNotNull(T item)
    {
      if ((object) item == null)
        throw new ArgumentNullException(nameof (item));
    }
  }
}
