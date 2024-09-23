// Decompiled with JetBrains decompiler
// Type: WhatsApp.SimpleSoundBase`1
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Collections.Generic;


namespace WhatsApp
{
  public abstract class SimpleSoundBase<T> : WaDisposable
  {
    protected string path;
    protected float volume;
    private static object @lock = new object();
    private static Dictionary<string, T> pathToObject = new Dictionary<string, T>();
    protected T effect;

    public SimpleSoundBase(string path, float volume = 1f)
    {
      this.path = path;
      this.volume = volume;
    }

    protected void Load()
    {
      if ((object) this.effect != null)
        return;
      lock (SimpleSoundBase<T>.@lock)
      {
        if (SimpleSoundBase<T>.pathToObject.TryGetValue(this.path, out this.effect))
          return;
        this.effect = this.LoadObject();
        if ((object) this.effect == null)
          return;
        SimpleSoundBase<T>.pathToObject[this.path] = this.effect;
      }
    }

    protected abstract T LoadObject();

    public abstract void Play();
  }
}
