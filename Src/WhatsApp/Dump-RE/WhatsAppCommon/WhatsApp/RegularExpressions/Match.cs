// Decompiled with JetBrains decompiler
// Type: WhatsApp.RegularExpressions.Match
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.RegularExpressions
{
  public class Match
  {
    internal Lazy<string> valueObject = new Lazy<string>();

    public bool Success { get; internal set; }

    public int Index { get; internal set; }

    public int Length { get; internal set; }

    public string Value
    {
      get => this.valueObject.Value;
      internal set => this.valueObject.Value = value;
    }

    public Group[] Groups { get; internal set; }
  }
}
