// Decompiled with JetBrains decompiler
// Type: WhatsApp.TranscodeArgs
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Runtime.Serialization;


namespace WhatsApp
{
  public class TranscodeArgs : JsonBase
  {
    [DataMember(Name = "x")]
    public int? XOffset { get; set; }

    [DataMember(Name = "y")]
    public int? YOffset { get; set; }

    [DataMember(Name = "w")]
    public int? Width { get; set; }

    [DataMember(Name = "h")]
    public int? Height { get; set; }

    [DataMember(Name = "s")]
    public int? StartMilliseconds { get; set; }

    [DataMember(Name = "d")]
    public int? DurationMilliseconds { get; set; }

    [DataMember(Name = "r")]
    public int? Rotation { get; set; }

    [DataMember(Name = "f")]
    public int? FlagsInteger { get; set; }

    public TranscodeReason Flags
    {
      get
      {
        int? flagsInteger = this.FlagsInteger;
        return !flagsInteger.HasValue ? TranscodeReason.None : (TranscodeReason) flagsInteger.Value;
      }
      set => this.FlagsInteger = new int?((int) value);
    }
  }
}
