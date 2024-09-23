// Decompiled with JetBrains decompiler
// Type: WhatsApp.FileRef
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Text;


namespace WhatsApp
{
  public struct FileRef
  {
    public FileRoot Root;
    public string Subdir;
    public string FilePart;

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append('{').Append(this.Root.ToString()).Append('}');
      if (!string.IsNullOrEmpty(this.Subdir))
        stringBuilder.Append('[').Append(this.Subdir).Append(']');
      if (!string.IsNullOrEmpty(this.FilePart))
        stringBuilder.Append('"').Append(this.FilePart).Append('"');
      return stringBuilder.ToString();
    }
  }
}
