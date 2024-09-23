// Decompiled with JetBrains decompiler
// Type: WhatsApp.DeviceSpecificSampleRates
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll


namespace WhatsApp
{
  public static class DeviceSpecificSampleRates
  {
    private static DeviceSpecificSampleRates.SampleRates? cached;

    public static DeviceSpecificSampleRates.SampleRates Get()
    {
      if (DeviceSpecificSampleRates.cached.HasValue)
        return DeviceSpecificSampleRates.cached.Value;
      DeviceSpecificSampleRates.SampleRates sampleRates = new DeviceSpecificSampleRates.SampleRates()
      {
        SampleRateFromDriver = 48000,
        ResampleRate = 16000
      };
      DeviceSpecificSampleRates.cached = new DeviceSpecificSampleRates.SampleRates?(sampleRates);
      return sampleRates;
    }

    public struct SampleRates
    {
      public int SampleRateFromDriver;
      public int ResampleRate;
    }
  }
}
