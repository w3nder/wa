// Decompiled with JetBrains decompiler
// Type: WhatsApp.WACamera
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Devices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Windows.Phone.Media.Capture;


namespace WhatsApp
{
  public class WACamera : IWACamera
  {
    public PhotoCaptureDevice cam;
    public MemoryStream captureStream = new MemoryStream();
    public CameraLoadedState cameraState;
    public bool IsInitialized;
    public bool Enabled;
    public int OrientationOfImage;
    public int CameraRotationOffset;
    public bool FlipImage;
    public int EncodedImageOrientation;
    public CameraType CameraType = CameraType.FrontFacing;
    private static IReadOnlyList<object> flashProperties;
    private CameraCaptureSequence seq;

    public WACamera(CameraType cameraType)
    {
      this.CameraType = cameraType;
      if (cameraType == CameraType.FrontFacing)
      {
        this.CameraRotationOffset = 270;
        this.FlipImage = true;
      }
      else
      {
        this.CameraRotationOffset = 90;
        this.FlipImage = false;
      }
    }

    public async Task<bool> InitializeCamera()
    {
      this.cameraState = CameraLoadedState.Initializing;
      if (PhotoCaptureDevice.AvailableSensorLocations.Contains<CameraSensorLocation>((CameraSensorLocation) 0) || PhotoCaptureDevice.AvailableSensorLocations.Contains<CameraSensorLocation>((CameraSensorLocation) 1))
      {
        if (this.cam != null)
        {
          this.cam.Dispose();
          this.cam = (PhotoCaptureDevice) null;
        }
        CameraSensorLocation cameraLocation = this.CameraType != CameraType.FrontFacing || !PhotoCaptureDevice.AvailableSensorLocations.Contains<CameraSensorLocation>((CameraSensorLocation) 1) ? (CameraSensorLocation) 0 : (CameraSensorLocation) 1;
        try
        {
          IReadOnlyList<Windows.Foundation.Size> captureResolutions = PhotoCaptureDevice.GetAvailableCaptureResolutions(cameraLocation);
          IReadOnlyList<Windows.Foundation.Size> previewResolutions = PhotoCaptureDevice.GetAvailablePreviewResolutions(cameraLocation);
          Windows.Foundation.Size captureResolution = captureResolutions.FirstOrDefault<Windows.Foundation.Size>();
          Windows.Foundation.Size previewResolution = previewResolutions.FirstOrDefault<Windows.Foundation.Size>();
          Windows.Foundation.Size ThreeByFour1;
          Windows.Foundation.Size NineBySixteen1;
          this.getHighestResolutions(captureResolutions, out ThreeByFour1, out NineBySixteen1);
          Windows.Foundation.Size ThreeByFour2;
          Windows.Foundation.Size NineBySixteen2;
          this.getHighestResolutions(previewResolutions, out ThreeByFour2, out NineBySixteen2);
          if (NineBySixteen1.Height != 0.0 && NineBySixteen2.Height != 0.0)
          {
            captureResolution = NineBySixteen1;
            previewResolution = NineBySixteen2;
            Log.l("Resolution 6/19.  Preview: {0} Capture: {1}", (object) previewResolution, (object) captureResolution);
          }
          else if (ThreeByFour1.Height != 0.0 && ThreeByFour2.Height != 0.0)
          {
            captureResolution = ThreeByFour1;
            previewResolution = ThreeByFour2;
            Log.l("Resolution 3/4.  Preview: {0} Capture: {1}", (object) previewResolution, (object) captureResolution);
          }
          else
            Log.l("Resolution default.  Preview: {0} Capture: {1}", (object) previewResolution, (object) captureResolution);
          TaskCompletionSource<Task> source = new TaskCompletionSource<Task>();
          Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (async () =>
          {
            try
            {
              WACamera waCamera;
              PhotoCaptureDevice cam = waCamera.cam;
              PhotoCaptureDevice photoCaptureDevice = await PhotoCaptureDevice.OpenAsync(cameraLocation, captureResolution);
              waCamera.cam = photoCaptureDevice;
              waCamera = (WACamera) null;
            }
            catch (Exception ex)
            {
              Log.SendCrashLog(ex, "Camera OpenAsync failure", logOnlyForRelease: true);
              this.cam = (PhotoCaptureDevice) null;
            }
            finally
            {
              source.SetResult((Task) null);
            }
          }));
          Task task = await source.Task;
          if (this.cam == null)
            return false;
          await this.cam.SetPreviewResolutionAsync(previewResolution);
          previewResolution = new Windows.Foundation.Size();
        }
        catch (Exception ex)
        {
          string context = "Camera initialization failed: " + (object) cameraLocation;
          Log.LogException(ex, context);
          return false;
        }
      }
      this.cam.SetProperty(KnownCameraGeneralProperties.PlayShutterSoundOnCapture, (object) true);
      if (this.CameraType != CameraType.FrontFacing)
        Settings.UserFlashSetting = this.SetFlash(Settings.UserFlashSetting);
      this.seq = this.cam.CreateCaptureSequence(1U);
      this.seq.Frames[0].put_CaptureStream(this.captureStream.AsOutputStream());
      await this.cam.PrepareCaptureSequenceAsync(this.seq);
      return true;
    }

    public void getHighestResolutions(
      IReadOnlyList<Windows.Foundation.Size> resolutions,
      out Windows.Foundation.Size ThreeByFour,
      out Windows.Foundation.Size NineBySixteen)
    {
      NineBySixteen = new Windows.Foundation.Size(0.0, 0.0);
      ThreeByFour = new Windows.Foundation.Size(0.0, 0.0);
      for (int index = 0; index < resolutions.Count; ++index)
      {
        Windows.Foundation.Size resolution = resolutions[index];
        double height1 = resolution.Height;
        resolution = resolutions[index];
        double width1 = resolution.Width;
        if (height1 / width1 == 9.0 / 16.0)
        {
          resolution = resolutions[index];
          if (resolution.Height > NineBySixteen.Height)
            NineBySixteen = resolutions[index];
        }
        resolution = resolutions[index];
        double height2 = resolution.Height;
        resolution = resolutions[index];
        double width2 = resolution.Width;
        if (height2 / width2 == 0.75)
        {
          resolution = resolutions[index];
          if (resolution.Height > ThreeByFour.Height)
            ThreeByFour = resolutions[index];
        }
      }
    }

    public void Dispose()
    {
      this.Enabled = false;
      Log.d("camera", "WACamera dispose {0} {1}", (object) this.IsInitialized, (object) (this.cam != null));
      if (!this.IsInitialized || this.cam == null)
        return;
      this.cam.Dispose();
      this.IsInitialized = false;
    }

    private bool IsFlashStateSupported(FlashState flashState)
    {
      try
      {
        if (WACamera.flashProperties == null)
          WACamera.flashProperties = PhotoCaptureDevice.GetSupportedPropertyValues((CameraSensorLocation) 0, KnownCameraPhotoProperties.FlashMode);
        if (WACamera.flashProperties != null)
        {
          if (WACamera.flashProperties.Count > 0)
          {
            foreach (uint flashProperty in (IEnumerable<object>) WACamera.flashProperties)
            {
              if ((int) flashProperty == flashState)
                return true;
            }
          }
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception getting supported flash states");
      }
      return false;
    }

    public FlashMode? SetFlash(FlashMode? mode)
    {
      try
      {
        if (mode.HasValue)
        {
          if (this.CameraType != CameraType.FrontFacing)
          {
            FlashState flashState = (FlashState) 0;
            FlashMode? nullable = mode;
            if (nullable.HasValue)
            {
              switch (nullable.GetValueOrDefault())
              {
                case FlashMode.On:
                  if (this.IsFlashStateSupported((FlashState) 3))
                  {
                    flashState = (FlashState) 3;
                    break;
                  }
                  mode = new FlashMode?(FlashMode.Auto);
                  goto case FlashMode.Auto;
                case FlashMode.Off:
                  flashState = (FlashState) 0;
                  break;
                case FlashMode.Auto:
                  if (this.IsFlashStateSupported((FlashState) 1))
                  {
                    flashState = (FlashState) 1;
                    break;
                  }
                  mode = new FlashMode?(FlashMode.Off);
                  goto case FlashMode.Off;
              }
            }
            this.cam.SetProperty(KnownCameraPhotoProperties.FlashMode, (object) flashState);
            return mode;
          }
        }
      }
      catch (Exception ex)
      {
        Log.l("Camera", "Failed to set flash to {0}, ex: {1}", (object) mode, (object) ex.GetFriendlyMessage());
      }
      try
      {
        switch ((uint) this.cam.GetProperty(KnownCameraPhotoProperties.FlashMode))
        {
          case 0:
            return new FlashMode?(FlashMode.Off);
          case 1:
            return new FlashMode?(FlashMode.Auto);
          case 3:
            return new FlashMode?(FlashMode.On);
        }
      }
      catch (Exception ex)
      {
      }
      return new FlashMode?(FlashMode.Off);
    }

    public void ToggleFlash()
    {
      uint property = (uint) this.cam.GetProperty(KnownCameraPhotoProperties.FlashMode);
      FlashMode flashMode = FlashMode.Off;
      switch (property)
      {
        case 0:
          flashMode = FlashMode.On;
          break;
        case 1:
          flashMode = FlashMode.Off;
          break;
        case 3:
          flashMode = FlashMode.Auto;
          break;
      }
      Settings.UserFlashSetting = this.SetFlash(new FlashMode?(flashMode));
    }

    public async Task CaptureImage()
    {
      Log.WriteLineDebug("Start Capture");
      await this.seq.StartCaptureAsync();
      Log.WriteLineDebug("End");
      this.captureStream.Seek(0L, SeekOrigin.Begin);
    }

    public async Task FocusAtPoint(double x, double y)
    {
      if (!PhotoCaptureDevice.IsFocusSupported(this.CameraType == CameraType.Primary ? (CameraSensorLocation) 0 : (CameraSensorLocation) 1))
        return;
      this.cam.put_FocusRegion(new Windows.Foundation.Rect?(new Windows.Foundation.Rect(x, y, 0.0, 0.0)));
      CameraFocusStatus cameraFocusStatus = await this.cam.FocusAsync();
    }

    public async Task Focus()
    {
      if (!PhotoCaptureDevice.IsFocusSupported(this.CameraType == CameraType.Primary ? (CameraSensorLocation) 0 : (CameraSensorLocation) 1))
        return;
      CameraFocusStatus cameraFocusStatus = await this.cam.FocusAsync();
    }
  }
}
