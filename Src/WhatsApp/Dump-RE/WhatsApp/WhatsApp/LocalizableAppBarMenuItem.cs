﻿// Decompiled with JetBrains decompiler
// Type: WhatsApp.LocalizableAppBarMenuItem
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Shell;

#nullable disable
namespace WhatsApp
{
  public class LocalizableAppBarMenuItem : ApplicationBarMenuItem
  {
    public LocalizableAppBarMenuItem(string text, string[] args)
      : base(text)
    {
      this.Args = args;
    }

    public string[] Args { get; set; }
  }
}
