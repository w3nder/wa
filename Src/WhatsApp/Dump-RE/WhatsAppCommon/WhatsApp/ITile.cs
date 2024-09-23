// Decompiled with JetBrains decompiler
// Type: WhatsApp.ITile
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace WhatsApp
{
  public interface ITile
  {
    void Clear();

    void SetTitle(string title, bool back = false);

    void SetWideContent(IEnumerable<string> strs);

    void SetCount(int? count);

    void SetBackgroundImage(Uri uri);

    void SetSmallBackgroundImage(Uri uri);

    void Update();

    void Delete();
  }
}
