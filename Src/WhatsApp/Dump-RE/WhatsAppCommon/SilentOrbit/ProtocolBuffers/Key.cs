// Decompiled with JetBrains decompiler
// Type: SilentOrbit.ProtocolBuffers.Key
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace SilentOrbit.ProtocolBuffers
{
  public class Key
  {
    public uint Field { get; set; }

    public Wire WireType { get; set; }

    public Key(uint field, Wire wireType)
    {
      this.Field = field;
      this.WireType = wireType;
    }

    public override string ToString()
    {
      return string.Format("[Key: {0}, {1}]", (object) this.Field, (object) this.WireType);
    }
  }
}
