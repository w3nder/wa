// Decompiled with JetBrains decompiler
// Type: WhatsApp.QrScanner
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Windows.Phone.Media.Capture;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;


namespace WhatsApp
{
  public class QrScanner : UserControl
  {
    private static readonly string LogHdr = nameof (QrScanner);
    private PhotoCaptureDevice camera;
    private DispatcherTimer timer;
    private QRCodeReader reader;
    private RGBLuminanceSource source;
    private QrScanner.CameraLoadedState cameraState;
    private bool scanStarted;
    private Dictionary<DecodeHintType, object> decodeHints = new Dictionary<DecodeHintType, object>();
    private int processCount;
    private DateTime? nextFocusTime;
    private const int timerIntervalMs = 100;
    private const int timeBetweenFocusesMs = 401;
    private const int processTicksPerBustedCheck = 10;
    private const int processTicksB4BustedNotification = 50;
    private const int processTicksPerLog = 50;
    private bool processing;
    private int exceptionCount;
    private bool waiting_;
    private byte? luminance;
    private bool seenDifferentLuminance;
    internal Storyboard CompleteAnimation;
    internal Storyboard QrDemoStoryboard;
    internal Rectangle PreviewRect;
    internal VideoBrush PreviewBrush;
    internal Image ScanFrame;
    internal Rectangle ShadeBackground;
    internal Image Checkmark;
    internal ProgressBar QrWait;
    internal Grid QrDemo;
    internal Image Computer;
    internal CompositeTransform ComputerXForm;
    internal Grid PhoneContainer;
    internal CompositeTransform PhoneContainerXForm;
    internal Grid QrContainer;
    internal Image QrCode;
    internal CompositeTransform QrXForm;
    internal Image QrNormal;
    internal Image SmallCheck;
    internal CompositeTransform SmallCheckXForm;
    private bool _contentLoaded;

    public event EventHandler<QrScanner.QrScannerEventArgs> QrScanned;

    public QrScanner() => this.InitializeComponent();

    private void CameraInitialized()
    {
      Log.d(QrScanner.LogHdr, "CameraInitialized | {0}", (object) (this.camera != null));
      if (this.camera == null)
        return;
      this.PreviewBrush.SetSource((object) this.camera);
      Windows.Foundation.Size previewResolution = this.camera.PreviewResolution;
      this.source = new RGBLuminanceSource(new byte[(int) previewResolution.Width * (int) previewResolution.Height * 3], (int) previewResolution.Width, (int) previewResolution.Height);
      if (this.reader == null)
        this.reader = new QRCodeReader();
      else
        this.reader.reset();
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        if (this.camera == null)
          return;
        this.cameraState = QrScanner.CameraLoadedState.Loaded;
        if (!this.scanStarted)
          return;
        this.StartScanner();
      }));
    }

    public void ScanAsync()
    {
      this.Load();
      if (!AppState.GetConnection().EventHandler.Qr.Session.QrDemoShown && !this.SkipDemo)
        Storyboarder.Perform(this.QrDemoStoryboard);
      this.scanStarted = true;
      if (this.cameraState != QrScanner.CameraLoadedState.Loaded)
        return;
      this.StartScanner();
    }

    private void StartScanner()
    {
      Log.l(QrScanner.LogHdr, nameof (StartScanner));
      this.processCount = 0;
      this.exceptionCount = 0;
      this.nextFocusTime = PhotoCaptureDevice.IsFocusSupported((CameraSensorLocation) 0) ? new DateTime?(DateTime.UtcNow) : new DateTime?();
      this.decodeHints[DecodeHintType.CHARACTER_SET] = (object) "ISO-8859-1";
      this.decodeHints[DecodeHintType.POSSIBLE_FORMATS] = (object) new List<BarcodeFormat>()
      {
        BarcodeFormat.QR_CODE
      };
      this.timer.Start();
    }

    public void StopScan()
    {
      Log.l(QrScanner.LogHdr, nameof (StopScan));
      this.scanStarted = false;
      this.luminance = new byte?();
      this.seenDifferentLuminance = false;
      this.QrDemoStoryboard.Stop();
      if (this.timer == null || !this.timer.IsEnabled)
        return;
      this.timer.Stop();
    }

    private async void Timer_Tick(object sender, EventArgs e)
    {
      if (this.cameraState != QrScanner.CameraLoadedState.Loaded)
        return;
      lock (this)
      {
        if (this.processing)
          return;
        ++this.processCount;
        this.processing = true;
      }
      try
      {
        this.camera.GetPreviewBufferY(this.source.Matrix);
        ZXing.Result result = this.reader.decode(new BinaryBitmap((Binarizer) new HybridBinarizer((LuminanceSource) this.source)), (IDictionary<DecodeHintType, object>) this.decodeHints);
        if (result != null && result.Text != null)
        {
          EventHandler<QrScanner.QrScannerEventArgs> qrScanned = this.QrScanned;
          if (qrScanned != null)
          {
            byte[] numArray1 = (byte[]) null;
            if (result.ResultMetadata[ResultMetadataType.BYTE_SEGMENTS] is List<byte[]> source)
            {
              if (source.Count > 1)
              {
                int length = 0;
                foreach (byte[] numArray2 in source)
                  length += numArray2.Length;
                numArray1 = new byte[length];
                int destinationIndex = 0;
                foreach (byte[] sourceArray in source)
                {
                  Array.Copy((Array) sourceArray, 0, (Array) numArray1, destinationIndex, sourceArray.Length);
                  destinationIndex += sourceArray.Length;
                }
              }
              else
                numArray1 = source.FirstOrDefault<byte[]>();
            }
            qrScanned((object) this, new QrScanner.QrScannerEventArgs(result.Text, numArray1));
          }
        }
        if (this.nextFocusTime.HasValue)
        {
          DateTime? nextFocusTime = this.nextFocusTime;
          DateTime utcNow = DateTime.UtcNow;
          if ((nextFocusTime.HasValue ? (nextFocusTime.GetValueOrDefault() < utcNow ? 1 : 0) : 0) != 0)
          {
            CameraFocusStatus cameraFocusStatus = await this.camera.FocusAsync();
            this.nextFocusTime = new DateTime?(DateTime.UtcNow.AddMilliseconds(401.0));
          }
        }
        if (this.processCount % 10 == 0)
          this.VerifyBusted(this.source.Matrix);
        if (this.processCount % 50 != 0)
          return;
        Log.d(QrScanner.LogHdr, nameof (Timer_Tick));
      }
      catch (Exception ex)
      {
        ++this.exceptionCount;
        if (this.exceptionCount < 3)
        {
          Log.l(ex, "Procesing scanner input");
        }
        else
        {
          if (this.exceptionCount != 3)
            return;
          Log.SendCrashLog(ex, "Repeated exceptions while scanning", logOnlyForRelease: true);
        }
      }
      finally
      {
        this.processing = false;
      }
    }

    public async void Load()
    {
      this.cameraState = QrScanner.CameraLoadedState.Initializing;
      if (PhotoCaptureDevice.AvailableSensorLocations.Contains<CameraSensorLocation>((CameraSensorLocation) 0) || PhotoCaptureDevice.AvailableSensorLocations.Contains<CameraSensorLocation>((CameraSensorLocation) 1))
      {
        if (this.camera != null)
        {
          this.camera.Dispose();
          this.camera = (PhotoCaptureDevice) null;
        }
        QrScanner qrScanner;
        if (PhotoCaptureDevice.AvailableSensorLocations.Contains<CameraSensorLocation>((CameraSensorLocation) 0))
        {
          Windows.Foundation.Size previewRes = PhotoCaptureDevice.GetAvailablePreviewResolutions((CameraSensorLocation) 0)[0];
          Windows.Foundation.Size[] array = PhotoCaptureDevice.GetAvailableCaptureResolutions((CameraSensorLocation) 0).ToArray<Windows.Foundation.Size>();
          Windows.Foundation.Size size = previewRes;
          if (!((IEnumerable<Windows.Foundation.Size>) array).Contains<Windows.Foundation.Size>(previewRes))
            size = array[0];
          Log.l(QrScanner.LogHdr, "using back camera, res: {0}x{1}, preview res: {2}x{3}", (object) size.Width, (object) size.Height, (object) previewRes.Width, (object) previewRes.Height);
          qrScanner = this;
          PhotoCaptureDevice camera = qrScanner.camera;
          PhotoCaptureDevice photoCaptureDevice = await PhotoCaptureDevice.OpenAsync((CameraSensorLocation) 0, size);
          qrScanner.camera = photoCaptureDevice;
          qrScanner = (QrScanner) null;
          this.camera.SetPreviewResolutionAsync(previewRes);
          previewRes = new Windows.Foundation.Size();
        }
        else
        {
          Windows.Foundation.Size previewRes = PhotoCaptureDevice.GetAvailablePreviewResolutions((CameraSensorLocation) 1)[0];
          Windows.Foundation.Size[] array = PhotoCaptureDevice.GetAvailableCaptureResolutions((CameraSensorLocation) 0).ToArray<Windows.Foundation.Size>();
          Windows.Foundation.Size size = previewRes;
          if (!((IEnumerable<Windows.Foundation.Size>) array).Contains<Windows.Foundation.Size>(previewRes))
            size = array[0];
          Log.l(QrScanner.LogHdr, "using front camera, res: {0}x{1}, preview res: {2}x{3}", (object) size.Width, (object) size.Height, (object) previewRes.Width, (object) previewRes.Height);
          qrScanner = this;
          PhotoCaptureDevice camera = qrScanner.camera;
          PhotoCaptureDevice photoCaptureDevice = await PhotoCaptureDevice.OpenAsync((CameraSensorLocation) 1, size);
          qrScanner.camera = photoCaptureDevice;
          qrScanner = (QrScanner) null;
          this.camera.SetPreviewResolutionAsync(previewRes);
          previewRes = new Windows.Foundation.Size();
        }
        this.camera.SetProperty(KnownCameraPhotoProperties.FlashMode, (object) (FlashState) 0);
        if (this.cameraState == QrScanner.CameraLoadedState.Unloaded)
        {
          this.camera.Dispose();
          this.camera = (PhotoCaptureDevice) null;
        }
        else
          this.CameraInitialized();
      }
      if (this.timer == null)
      {
        this.timer = new DispatcherTimer();
        this.timer.Interval = TimeSpan.FromMilliseconds(100.0);
        this.timer.Tick += new EventHandler(this.Timer_Tick);
      }
      this.seenDifferentLuminance = false;
    }

    public void Unload()
    {
      this.StopScan();
      this.cameraState = QrScanner.CameraLoadedState.Unloaded;
      ((IDisposable) this.camera).SafeDispose();
      this.camera = (PhotoCaptureDevice) null;
      if (this.timer != null)
        this.timer.Tick -= new EventHandler(this.Timer_Tick);
      this.timer = (DispatcherTimer) null;
    }

    public bool Complete
    {
      set
      {
        this.Waiting = false;
        this.CompleteAnimation.Begin();
      }
    }

    public bool Waiting
    {
      set
      {
        if (this.waiting_ == value)
          return;
        this.waiting_ = value;
        this.ShadeBackground.Visibility = this.waiting_ ? Visibility.Visible : Visibility.Collapsed;
        this.QrWait.Visibility = this.waiting_ ? Visibility.Visible : Visibility.Collapsed;
        this.QrWait.IsIndeterminate = this.waiting_;
      }
    }

    public void DemoCollapse(object sender, EventArgs args) => this.QrDemoStoryboard.Stop();

    public void VerifyBusted(byte[] buffer)
    {
      if (this.seenDifferentLuminance || this.cameraState != QrScanner.CameraLoadedState.Loaded)
        return;
      Log.l(QrScanner.LogHdr, "Checking if camera is busted");
      if (!this.luminance.HasValue)
        this.luminance = new byte?(buffer[0]);
      Windows.Foundation.Size previewResolution = this.camera.PreviewResolution;
      for (int index = 0; (double) index < previewResolution.Width * previewResolution.Height; index += 10)
      {
        int num = (int) buffer[index];
        byte? luminance = this.luminance;
        int? nullable = luminance.HasValue ? new int?((int) luminance.GetValueOrDefault()) : new int?();
        int valueOrDefault = nullable.GetValueOrDefault();
        if ((num == valueOrDefault ? (!nullable.HasValue ? 1 : 0) : 1) != 0)
        {
          this.seenDifferentLuminance = true;
          break;
        }
      }
      if (this.processCount <= 50 || this.seenDifferentLuminance)
        return;
      int num1 = (int) MessageBox.Show(AppResources.WebScannerStartFailDescription, AppResources.WebScannerStartFail, MessageBoxButton.OK);
      this.seenDifferentLuminance = true;
    }

    public bool SkipDemo { get; set; }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/QrScanner.xaml", UriKind.Relative));
      this.CompleteAnimation = (Storyboard) this.FindName("CompleteAnimation");
      this.QrDemoStoryboard = (Storyboard) this.FindName("QrDemoStoryboard");
      this.PreviewRect = (Rectangle) this.FindName("PreviewRect");
      this.PreviewBrush = (VideoBrush) this.FindName("PreviewBrush");
      this.ScanFrame = (Image) this.FindName("ScanFrame");
      this.ShadeBackground = (Rectangle) this.FindName("ShadeBackground");
      this.Checkmark = (Image) this.FindName("Checkmark");
      this.QrWait = (ProgressBar) this.FindName("QrWait");
      this.QrDemo = (Grid) this.FindName("QrDemo");
      this.Computer = (Image) this.FindName("Computer");
      this.ComputerXForm = (CompositeTransform) this.FindName("ComputerXForm");
      this.PhoneContainer = (Grid) this.FindName("PhoneContainer");
      this.PhoneContainerXForm = (CompositeTransform) this.FindName("PhoneContainerXForm");
      this.QrContainer = (Grid) this.FindName("QrContainer");
      this.QrCode = (Image) this.FindName("QrCode");
      this.QrXForm = (CompositeTransform) this.FindName("QrXForm");
      this.QrNormal = (Image) this.FindName("QrNormal");
      this.SmallCheck = (Image) this.FindName("SmallCheck");
      this.SmallCheckXForm = (CompositeTransform) this.FindName("SmallCheckXForm");
    }

    public enum CameraLoadedState
    {
      Unloaded,
      Initializing,
      Loaded,
    }

    public class QrScannerEventArgs : EventArgs
    {
      public string Data;
      public byte[] Bytes;

      public QrScannerEventArgs(string s, byte[] b)
      {
        this.Data = s;
        this.Bytes = b;
      }
    }
  }
}
