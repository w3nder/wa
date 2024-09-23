// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaCollections.Pair`2
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.WaCollections
{
  public class Pair<T, U>
  {
    public T First;
    public U Second;

    public Pair()
    {
    }

    public Pair(T first, U second)
    {
      this.First = first;
      this.Second = second;
    }

    public override bool Equals(object obj)
    {
      if (!(obj is Pair<T, U> pair) || ((object) this.First == null ? ((object) pair.First == null ? 1 : 0) : (this.First.Equals((object) pair.First) ? 1 : 0)) == 0)
        return false;
      return (object) this.Second != null ? this.Second.Equals((object) pair.Second) : (object) pair.Second == null;
    }

    public override int GetHashCode()
    {
      return (-1660579480 ^ ((object) this.First == null ? 0 : this.First.GetHashCode())) * 1000003 ^ ((object) this.Second == null ? 0 : this.Second.GetHashCode());
    }
  }
}
