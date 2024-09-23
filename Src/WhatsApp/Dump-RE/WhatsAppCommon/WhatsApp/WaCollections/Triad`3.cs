// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaCollections.Triad`3
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.WaCollections
{
  public class Triad<T, U, V> : Pair<T, U>
  {
    public V Third;

    public Triad()
    {
    }

    public Triad(T first, U second, V third)
      : base(first, second)
    {
      this.Third = third;
    }

    public override bool Equals(object obj)
    {
      if (!(obj is Triad<T, U, V> triad) || !base.Equals((object) triad))
        return false;
      return (object) this.Third != null ? this.Third.Equals((object) triad.Third) : (object) triad.Third == null;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode() * 1000003 ^ ((object) this.Third == null ? 0 : this.Third.GetHashCode());
    }
  }
}
