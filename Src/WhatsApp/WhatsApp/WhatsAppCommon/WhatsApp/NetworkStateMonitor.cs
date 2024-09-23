// Decompiled with JetBrains decompiler
// Type: WhatsApp.NetworkStateMonitor
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WhatsAppNative;
using Windows.Networking.Connectivity;


namespace WhatsApp
{
  public class NetworkStateMonitor
  {
    private uint lastMnc;
    private string lastSsid;
    private NetworkStateChange lastNetworkState;
    private Subject<NetworkStateChange> subj = new Subject<NetworkStateChange>();
    private static NetworkStateMonitor instance;
    private static NetworkInterfaceSubType[] nonCellularSubTypes = new NetworkInterfaceSubType[3]
    {
      NetworkInterfaceSubType.Desktop_PassThru,
      NetworkInterfaceSubType.Unknown,
      NetworkInterfaceSubType.WiFi
    };

    private NetworkStateMonitor()
    {
      this.Observe();
      DeviceNetworkInformation.NetworkAvailabilityChanged += (EventHandler<NetworkNotificationEventArgs>) ((sender, args) =>
      {
        NetworkStateChange flags = NetworkStateChange.None;
        if (args.NetworkInterface.InterfaceType == NetworkInterfaceType.Wireless80211)
        {
          string str = (string) null;
          if (args.NotificationType != NetworkNotificationType.InterfaceDisconnected)
            str = args.NetworkInterface.InterfaceName;
          if (str != this.lastSsid)
          {
            flags |= NetworkStateChange.WifiNetworkChanged;
            this.lastSsid = str;
          }
          if (NetworkStateMonitor.IsWifiDataConnected())
            flags |= NetworkStateChange.WifiInternetConnected;
          if (NetworkStateMonitor.IsCellularDataConnected())
            flags |= NetworkStateChange.CellularInternetConnected;
        }
        else
        {
          flags |= NetworkStateChange.DataNetworkChanged;
          this.lastMnc = this.GetMnc();
          if (NetworkStateMonitor.IsWifiDataConnected())
            flags |= NetworkStateChange.WifiInternetConnected;
          if (NetworkStateMonitor.IsCellularDataConnected())
            flags |= NetworkStateChange.CellularInternetConnected;
        }
        this.NotifyFlags(flags);
      });
      WindowsRuntimeMarshal.AddEventHandler<NetworkStatusChangedEventHandler>(new Func<NetworkStatusChangedEventHandler, EventRegistrationToken>(Windows.Networking.Connectivity.NetworkInformation.add_NetworkStatusChanged), new Action<EventRegistrationToken>(Windows.Networking.Connectivity.NetworkInformation.remove_NetworkStatusChanged), (NetworkStatusChangedEventHandler) (sender => this.Observe()));
    }

    public void Observe()
    {
      uint mnc = this.GetMnc();
      string ssid = this.GetSsid();
      NetworkStateChange flags = NetworkStateChange.None;
      if (NetworkStateMonitor.IsCellularDataConnected())
        flags |= NetworkStateChange.CellularInternetConnected;
      if (NetworkStateMonitor.IsWifiDataConnected())
        flags |= NetworkStateChange.WifiInternetConnected;
      if ((int) mnc != (int) this.lastMnc)
      {
        flags |= NetworkStateChange.DataNetworkChanged;
        this.lastMnc = mnc;
      }
      if (ssid != this.lastSsid)
      {
        flags |= NetworkStateChange.WifiNetworkChanged;
        this.lastSsid = ssid;
      }
      this.NotifyFlags(flags);
    }

    public IObservable<NetworkStateChange> Observable
    {
      get => (IObservable<NetworkStateChange>) this.subj;
    }

    private void NotifyFlags(NetworkStateChange flags)
    {
      if (flags == this.lastNetworkState)
        return;
      this.lastNetworkState = flags;
      this.subj.OnNext(flags);
    }

    public static NetworkStateMonitor Instance
    {
      get
      {
        return Utils.LazyInit<NetworkStateMonitor>(ref NetworkStateMonitor.instance, (Func<NetworkStateMonitor>) (() => new NetworkStateMonitor()));
      }
    }

    private string GetSsid()
    {
      try
      {
        return new NetworkInterfaceList().Where<NetworkInterfaceInfo>((Func<NetworkInterfaceInfo, bool>) (iface => iface.InterfaceType == NetworkInterfaceType.Wireless80211)).Where<NetworkInterfaceInfo>((Func<NetworkInterfaceInfo, bool>) (iface => iface.InterfaceState == ConnectState.Connected)).Select<NetworkInterfaceInfo, string>((Func<NetworkInterfaceInfo, string>) (iface => iface.InterfaceName)).FirstOrDefault<string>();
      }
      catch (Exception ex)
      {
        return (string) null;
      }
    }

    private uint GetMnc()
    {
      try
      {
        return NativeInterfaces.Misc.GetCellInfo(CellInfoFlags.MccMnc).Mnc;
      }
      catch (Exception ex)
      {
        return 0;
      }
    }

    public static wam_enum_radio_type? GetConnectedCellularFSType()
    {
      try
      {
        ConnectionProfile connectionProfile = Windows.Networking.Connectivity.NetworkInformation.GetConnectionProfiles().Where<ConnectionProfile>((Func<ConnectionProfile, bool>) (p => p.GetNetworkConnectivityLevel() == 3 && p.IsWwanConnectionProfile)).FirstOrDefault<ConnectionProfile>();
        if (connectionProfile != null)
        {
          WwanDataClass currentDataClass = connectionProfile.WwanConnectionProfileDetails.GetCurrentDataClass();
          if (currentDataClass <= 131072)
          {
            if (currentDataClass <= 16)
            {
              switch ((int) currentDataClass)
              {
                case 0:
                  return new wam_enum_radio_type?();
                case 1:
                  return new wam_enum_radio_type?(wam_enum_radio_type.CELLULAR_GPRS);
                case 2:
                  return new wam_enum_radio_type?(wam_enum_radio_type.CELLULAR_EDGE);
                case 3:
                case 5:
                case 6:
                case 7:
                  goto label_24;
                case 4:
                  return new wam_enum_radio_type?(wam_enum_radio_type.CELLULAR_UMTS);
                case 8:
                  return new wam_enum_radio_type?(wam_enum_radio_type.CELLULAR_HSDPA);
                default:
                  if (currentDataClass == 16)
                    return new wam_enum_radio_type?(wam_enum_radio_type.CELLULAR_HSUPA);
                  goto label_24;
              }
            }
            else
            {
              if (currentDataClass == 32)
                return new wam_enum_radio_type?(wam_enum_radio_type.CELLULAR_LTE);
              if (currentDataClass == 65536)
                return new wam_enum_radio_type?(wam_enum_radio_type.CELLULAR_1XRTT);
              if (currentDataClass != 131072)
                goto label_24;
            }
          }
          else if (currentDataClass <= 524288)
          {
            if (currentDataClass != 262144 && currentDataClass != 524288)
              goto label_24;
          }
          else
          {
            if (currentDataClass == 1048576)
              return new wam_enum_radio_type?(wam_enum_radio_type.CELLULAR_CDMA);
            if (currentDataClass != 2097152)
            {
              if (currentDataClass == 4194304)
                return new wam_enum_radio_type?(wam_enum_radio_type.CELLULAR_EVDO);
              goto label_24;
            }
          }
          return new wam_enum_radio_type?(wam_enum_radio_type.CELLULAR_EVDO);
label_24:
          return new wam_enum_radio_type?(wam_enum_radio_type.CELLULAR_UNKNOWN);
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception checking for cellular fs type using connection profile");
      }
      try
      {
        NetworkInterfaceInfo networkInterfaceInfo = new NetworkInterfaceList().Where<NetworkInterfaceInfo>((Func<NetworkInterfaceInfo, bool>) (ni => !((IEnumerable<NetworkInterfaceSubType>) NetworkStateMonitor.nonCellularSubTypes).Contains<NetworkInterfaceSubType>(ni.InterfaceSubtype) && ni.InterfaceState == ConnectState.Connected)).FirstOrDefault<NetworkInterfaceInfo>();
        if (networkInterfaceInfo != null)
        {
          switch (networkInterfaceInfo.InterfaceSubtype)
          {
            case NetworkInterfaceSubType.Cellular_GPRS:
              return new wam_enum_radio_type?(wam_enum_radio_type.CELLULAR_GPRS);
            case NetworkInterfaceSubType.Cellular_1XRTT:
              return new wam_enum_radio_type?(wam_enum_radio_type.CELLULAR_1XRTT);
            case NetworkInterfaceSubType.Cellular_EVDO:
              return new wam_enum_radio_type?(wam_enum_radio_type.CELLULAR_EVDO);
            case NetworkInterfaceSubType.Cellular_EDGE:
              return new wam_enum_radio_type?(wam_enum_radio_type.CELLULAR_EDGE);
            case NetworkInterfaceSubType.Cellular_HSPA:
              return new wam_enum_radio_type?(wam_enum_radio_type.CELLULAR_HSPA);
            case NetworkInterfaceSubType.Cellular_EVDV:
              return new wam_enum_radio_type?(wam_enum_radio_type.CELLULAR_EVDO);
            case NetworkInterfaceSubType.WiFi:
              return new wam_enum_radio_type?(wam_enum_radio_type.WIFI_UNKNOWN);
            case NetworkInterfaceSubType.Cellular_LTE:
              return new wam_enum_radio_type?(wam_enum_radio_type.CELLULAR_LTE);
            case NetworkInterfaceSubType.Cellular_EHRPD:
              return new wam_enum_radio_type?(wam_enum_radio_type.CELLULAR_EHRPD);
            default:
              return new wam_enum_radio_type?(wam_enum_radio_type.CELLULAR_UNKNOWN);
          }
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception checking for data using NetworkInterface");
      }
      return new wam_enum_radio_type?();
    }

    public static bool IsCellularDataConnected()
    {
      try
      {
        return Windows.Networking.Connectivity.NetworkInformation.GetConnectionProfiles().Where<ConnectionProfile>((Func<ConnectionProfile, bool>) (p => p.GetNetworkConnectivityLevel() == 3 && p.IsWwanConnectionProfile)).Any<ConnectionProfile>();
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception checking for data using connection profile");
      }
      try
      {
        return new NetworkInterfaceList().Where<NetworkInterfaceInfo>((Func<NetworkInterfaceInfo, bool>) (ni => !((IEnumerable<NetworkInterfaceSubType>) NetworkStateMonitor.nonCellularSubTypes).Contains<NetworkInterfaceSubType>(ni.InterfaceSubtype) && ni.InterfaceState == ConnectState.Connected)).Any<NetworkInterfaceInfo>();
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception checking for data using NetworkInterface");
      }
      return false;
    }

    public static bool IsWifiDataConnected()
    {
      try
      {
        return Windows.Networking.Connectivity.NetworkInformation.GetConnectionProfiles().Where<ConnectionProfile>((Func<ConnectionProfile, bool>) (p => p.GetNetworkConnectivityLevel() == 3 && p.IsWlanConnectionProfile)).Any<ConnectionProfile>();
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception checking for wifi using connection profile");
      }
      try
      {
        return new NetworkInterfaceList().Where<NetworkInterfaceInfo>((Func<NetworkInterfaceInfo, bool>) (ni => ni.InterfaceType == NetworkInterfaceType.Wireless80211 && ni.InterfaceState == ConnectState.Connected)).Any<NetworkInterfaceInfo>();
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception checking for wifi using NetworkInterface");
      }
      return false;
    }

    public static bool Is2GConnection()
    {
      if (NetworkStateMonitor.IsWifiDataConnected())
        return false;
      try
      {
        return NativeInterfaces.Misc.GetCellInfo(CellInfoFlags.NetworkInfo).Is2G;
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    public static bool Is3GOrBetter()
    {
      if (NetworkStateMonitor.IsWifiDataConnected())
        return true;
      try
      {
        return NativeInterfaces.Misc.GetCellInfo(CellInfoFlags.NetworkInfo).Is3GPlus;
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    public static bool IsDataConnected()
    {
      return NetworkStateMonitor.IsWifiDataConnected() || NetworkStateMonitor.IsCellularDataConnected();
    }
  }
}
