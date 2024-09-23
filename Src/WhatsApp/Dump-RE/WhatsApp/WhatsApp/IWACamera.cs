// Decompiled with JetBrains decompiler
// Type: WhatsApp.IWACamera
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Devices;
using System.Threading.Tasks;

#nullable disable
namespace WhatsApp
{
  public interface IWACamera
  {
    Task<bool> InitializeCamera();

    Task CaptureImage();

    Task FocusAtPoint(double x, double y);

    Task Focus();

    void Dispose();

    FlashMode? SetFlash(FlashMode? mode);

    void ToggleFlash();
  }
}
