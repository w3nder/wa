// Decompiled with JetBrains decompiler
// Type: WhatsApp.ITileExtensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Collections.Generic;


namespace WhatsApp
{
  public static class ITileExtensions
  {
    public static void SetWideContent(this ITile tile, string str)
    {
      tile.SetWideContent((IEnumerable<string>) new string[1]
      {
        str
      });
    }
  }
}
