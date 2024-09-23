// Decompiled with JetBrains decompiler
// Type: WhatsApp.HsmLocalePackInfo
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;


namespace WhatsApp
{
  public class HsmLocalePackInfo
  {
    public string Lg;
    public string Lc;
    public string Hash;

    public HsmLocalePackInfo(string lg, string lc)
    {
      this.Lg = !string.IsNullOrEmpty(lg) ? lg : throw new ArgumentOutOfRangeException("Base language not specified for Hsm Locale Pack info");
      this.Lc = lc;
    }

    public override string ToString()
    {
      return this.Lg + "_" + (this.Lc == null ? "" : this.Lc) + "_" + this.Hash;
    }
  }
}
