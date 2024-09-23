// Decompiled with JetBrains decompiler
// Type: WhatsApp.VoipCallParams
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.Media.Effects;


namespace WhatsApp
{
  public static class VoipCallParams
  {
    private static bool? msftSoftwareEcCache;

    public static bool MsftSoftwareECNeeded
    {
      get
      {
        if (VoipCallParams.msftSoftwareEcCache.HasValue)
          return VoipCallParams.msftSoftwareEcCache.Value;
        VoipCallParams.msftSoftwareEcCache = new bool?(VoipCallParams.GetSoftwareECNeeded());
        return VoipCallParams.msftSoftwareEcCache.Value;
      }
    }

    private static bool GetSoftwareECNeeded()
    {
      bool softwareEcNeeded = true;
      try
      {
        foreach (AudioEffect audioCaptureEffect in (IEnumerable<AudioEffect>) AudioEffectsManager.CreateAudioCaptureEffectsManager(MediaDevice.GetDefaultAudioCaptureId((AudioDeviceRole) 1), (MediaCategory) 1).GetAudioCaptureEffects())
        {
          if (audioCaptureEffect.AudioEffectType == 1)
          {
            softwareEcNeeded = false;
            break;
          }
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "voip GetSoftwareECNeeded");
      }
      return softwareEcNeeded;
    }
  }
}
