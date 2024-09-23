// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.Option
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

#nullable disable
namespace Microsoft.Graph
{
  public class Option
  {
    public Option(string name, string value)
    {
      this.Name = name;
      this.Value = value;
    }

    public string Name { get; private set; }

    public string Value { get; private set; }
  }
}
