// Decompiled with JetBrains decompiler
// Type: WhatsApp.NetworkStateChange
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;

#nullable disable
namespace WhatsApp
{
  [Flags]
  public enum NetworkStateChange
  {
    None = 0,
    DataNetworkChanged = 1,
    WifiNetworkChanged = 2,
    CellularInternetConnected = 4,
    WifiInternetConnected = 8,
  }
}
