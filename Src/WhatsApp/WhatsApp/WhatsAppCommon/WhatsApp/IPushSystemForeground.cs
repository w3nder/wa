// Decompiled with JetBrains decompiler
// Type: WhatsApp.IPushSystemForeground
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;


namespace WhatsApp
{
  public interface IPushSystemForeground
  {
    IObservable<Uri> UriObservable { get; }

    void BindPush();

    void RequestNewUri();

    string PushState { get; }

    bool IsHealthy { get; }

    void CreateTile(
      string key,
      string title,
      int initialCount,
      string initialContent,
      Uri uri,
      Uri backgroundImage,
      Uri smallBackgroundImage);
  }
}
