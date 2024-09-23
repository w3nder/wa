// Decompiled with JetBrains decompiler
// Type: WhatsApp.DeviceProfile
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Info;
using System;
using System.IO;

#nullable disable
namespace WhatsApp
{
  public class DeviceProfile
  {
    private string friendlyName;
    private static DeviceProfile instance;

    public string Manufacturer { get; private set; }

    public string Model { get; private set; }

    public string FriendlyName
    {
      get => this.friendlyName ?? this.Model;
      private set => this.friendlyName = value;
    }

    public DeviceProfile.Flags DeviceFlags { get; private set; }

    public static DeviceProfile Instance
    {
      get
      {
        return Utils.LazyInit<DeviceProfile>(ref DeviceProfile.instance, (Func<DeviceProfile>) (() => new DeviceProfile(DeviceStatus.DeviceManufacturer, DeviceStatus.DeviceName)));
      }
    }

    public bool HasSoftNavBar => (this.DeviceFlags & DeviceProfile.Flags.SoftNavBar) != 0;

    private string Deref(string[] values, int idx)
    {
      string str = (string) null;
      if (values != null && values.Length > idx && !string.IsNullOrEmpty(values[idx]))
        str = values[idx];
      return str;
    }

    public DeviceProfile(string mfgr, string model)
    {
      string[] values = (string[]) null;
      if (mfgr == null)
        mfgr = "";
      if (model == null)
        model = "";
      Log.l("device list", "looking up device | mfgr:[{0}], model:[{1}]", (object) mfgr, (object) model);
      mfgr = mfgr.ToUpperInvariant();
      using (Stream stream = AppState.OpenFromXAP("deviceinfo.csv"))
      {
        using (StreamReader streamReader = new StreamReader(stream))
        {
          streamReader.ReadLine();
          string str;
          while ((str = streamReader.ReadLine()) != null)
          {
            string[] strArray = str.Split(',');
            if (str.Length >= 2)
            {
              StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase;
              if (this.VendorEquals(strArray[0], mfgr, stringComparison) && model.StartsWith(strArray[1], stringComparison) && (strArray[1].Length <= model.Length || !char.IsDigit(strArray[1][model.Length])))
              {
                values = strArray;
                break;
              }
            }
          }
        }
      }
      string str1;
      this.Manufacturer = (str1 = this.Deref(values, 0)) == null ? mfgr : str1;
      string str2;
      this.Model = (str2 = this.Deref(values, 1)) == null ? model : str2;
      string str3;
      if ((str3 = this.Deref(values, 2)) != null)
        this.FriendlyName = str3;
      string str4;
      if ((str4 = this.Deref(values, 3)) != null)
      {
        string str5 = str4;
        char[] chArray = new char[1]{ '|' };
        foreach (string str6 in str5.Split(chArray))
        {
          switch (str6)
          {
            case "echo":
              this.DeviceFlags |= DeviceProfile.Flags.NeedsSoftwareEchoCancellation;
              break;
            case "noproximity":
              this.DeviceFlags |= DeviceProfile.Flags.NoProximitySensor;
              break;
            case "softnav":
              this.DeviceFlags |= DeviceProfile.Flags.SoftNavBar;
              break;
            case "slowproximity":
              this.DeviceFlags |= DeviceProfile.Flags.SlowProximitySensor;
              break;
          }
        }
      }
      string str7 = "not found in known device list";
      if (values != null)
        str7 = string.Format("flags = [{0}]", (object) this.DeviceFlags.ToString());
      Log.l("device list", "mfgr:[{0}], model:[{1}], {2}", (object) this.Manufacturer, (object) this.FriendlyName, (object) str7);
    }

    private bool VendorEquals(string modelA, string modelB, StringComparison cmp)
    {
      if (modelA.Equals("MICROSOFT", cmp))
        modelA = "NOKIA";
      if (modelB.Equals("MICROSOFT", cmp))
        modelB = "NOKIA";
      return modelA.Equals(modelB, cmp);
    }

    [System.Flags]
    public enum Flags
    {
      NeedsSoftwareEchoCancellation = 1,
      NoProximitySensor = 2,
      SoftNavBar = 4,
      SlowProximitySensor = 8,
    }
  }
}
