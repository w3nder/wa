// Decompiled with JetBrains decompiler
// Type: WhatsApp.IPushSystem
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;


namespace WhatsApp
{
  public interface IPushSystem
  {
    string PushState { get; }

    ITile PrimaryTile { get; }

    bool SecondaryTileExists();

    bool SecondaryTileExists(Func<Uri, bool> selector, string key);

    ITile GetSecondaryTile(Func<Uri, bool> selector, string key);

    Dictionary<string, string> ClientConfig { get; }

    void OnAppReset();

    void OnPushRegistered();

    void ShellToastEx(
      string[] content,
      string group,
      string uri,
      bool muted,
      string tone = null,
      string tag = null);

    void ClearToastHistoryGroup(string group);

    void ClearToastHistoryMessage(string tag, string group);
  }
}
