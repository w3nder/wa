// Decompiled with JetBrains decompiler
// Type: WhatsApp.RegularExpressions.Lazy`1
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;


namespace WhatsApp.RegularExpressions
{
  internal class Lazy<T>
  {
    private T value;
    private Func<T> valueFunc;

    public T Value
    {
      get
      {
        if ((object) this.value == null && this.valueFunc != null)
        {
          this.value = this.valueFunc();
          this.valueFunc = (Func<T>) null;
        }
        return this.value;
      }
      set
      {
        this.valueFunc = (Func<T>) null;
        this.value = value;
      }
    }

    public void SetValueLazy(Func<T> func)
    {
      this.valueFunc = func;
      this.value = default (T);
    }
  }
}
