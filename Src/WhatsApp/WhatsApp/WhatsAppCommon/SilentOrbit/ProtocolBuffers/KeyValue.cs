// Decompiled with JetBrains decompiler
// Type: SilentOrbit.ProtocolBuffers.KeyValue
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace SilentOrbit.ProtocolBuffers
{
  public class KeyValue
  {
    public Key Key { get; set; }

    public byte[] Value { get; set; }

    public KeyValue(Key key, byte[] value)
    {
      this.Key = key;
      this.Value = value;
    }

    public override string ToString()
    {
      return string.Format("[KeyValue: {0}, {1}, {2} bytes]", (object) this.Key.Field, (object) this.Key.WireType, (object) this.Value.Length);
    }
  }
}
