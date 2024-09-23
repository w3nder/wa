// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.TransferMonitor
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using Microsoft.Phone.BackgroundTransfer;
using Microsoft.Phone.Controls.LocalizedResources;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class TransferMonitor : INotifyPropertyChanged
  {
    private readonly BackgroundTransferRequest _request;
    private string _name;
    private TransferRequestState _state;
    private string _statusText;
    private long _bytesTransferred;
    private long _totalBytesToTransfer;
    private bool _isProgressIndeterminate;

    public event EventHandler<BackgroundTransferEventArgs> ProgressChanged;

    public event EventHandler<BackgroundTransferEventArgs> Started;

    public event EventHandler<BackgroundTransferEventArgs> Failed;

    public event EventHandler<BackgroundTransferEventArgs> Complete;

    public TransferType TransferType { get; private set; }

    public string ErrorMessage { get; private set; }

    public string Name
    {
      get
      {
        if (this._name != null && this._name.Trim().Length != 0)
          return this._name;
        string path = this.TransferType == TransferType.Upload ? this._request.UploadLocation.ToString() : this._request.DownloadLocation.ToString();
        try
        {
          return HttpUtility.UrlDecode(Path.GetFileNameWithoutExtension(path));
        }
        catch (ArgumentException ex)
        {
          return path;
        }
      }
      set
      {
        this._name = value;
        this.OnPropertyChanged(nameof (Name));
      }
    }

    public double PercentComplete
    {
      get
      {
        return this.TotalBytesToTransfer == 0L ? 0.0 : (double) this.BytesTransferred / (double) this.TotalBytesToTransfer;
      }
    }

    public TransferRequestState State
    {
      get => this._state;
      protected set
      {
        if (this._state == value)
          return;
        if (this.Started != null && this._state == TransferRequestState.Pending && (value != TransferRequestState.Complete || value != TransferRequestState.Failed))
          this.Started((object) this, new BackgroundTransferEventArgs(this._request));
        if (this.Complete != null && value == TransferRequestState.Complete)
          this.Complete((object) this, new BackgroundTransferEventArgs(this._request));
        if (this.Failed != null && value == TransferRequestState.Failed)
          this.Failed((object) this, new BackgroundTransferEventArgs(this._request));
        this._state = value;
        this.OnPropertyChanged(nameof (State));
      }
    }

    public string StatusText
    {
      get => this._statusText;
      protected set
      {
        this._statusText = value;
        this.OnPropertyChanged(nameof (StatusText));
      }
    }

    public long BytesTransferred
    {
      get => this._bytesTransferred;
      protected set
      {
        this._bytesTransferred = value;
        this.OnPropertyChanged(nameof (BytesTransferred));
        this.OnPropertyChanged("PercentComplete");
      }
    }

    public long TotalBytesToTransfer
    {
      get => this._totalBytesToTransfer;
      protected set
      {
        this._totalBytesToTransfer = value;
        this.OnPropertyChanged(nameof (TotalBytesToTransfer));
        this.OnPropertyChanged("PercentComplete");
      }
    }

    public bool IsProgressIndeterminate
    {
      get => this._isProgressIndeterminate;
      protected set
      {
        this._isProgressIndeterminate = value;
        this.OnPropertyChanged(nameof (IsProgressIndeterminate));
      }
    }

    public TransferMonitor(BackgroundTransferRequest request)
      : this(request, "")
    {
    }

    public TransferMonitor(BackgroundTransferRequest request, string name)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      this._request = request;
      this._name = name;
      this.TransferType = this._request.DownloadLocation == (Uri) null ? TransferType.Upload : TransferType.Download;
      this._request.TransferStatusChanged -= new EventHandler<BackgroundTransferEventArgs>(this.RequestStateChanged);
      this._request.TransferStatusChanged += new EventHandler<BackgroundTransferEventArgs>(this.RequestStateChanged);
      this._request.TransferProgressChanged -= new EventHandler<BackgroundTransferEventArgs>(this.RequestProgressChanged);
      this._request.TransferProgressChanged += new EventHandler<BackgroundTransferEventArgs>(this.RequestProgressChanged);
      this.RequestStateChanged((object) this._request, new BackgroundTransferEventArgs(this._request));
    }

    public void RequestStart()
    {
      try
      {
        BackgroundTransferService.Add(this._request);
      }
      catch (ArgumentNullException ex)
      {
        this.ErrorMessage = "Invalid request";
        this.State = TransferRequestState.Failed;
        this.StatusText = ControlResources.StatusFailed;
      }
      catch (InvalidOperationException ex)
      {
        this.ErrorMessage = "The request has already been submitted.";
        this.State = TransferRequestState.Failed;
      }
      catch (SystemException ex)
      {
        this.ErrorMessage = "The maximum number of requests on the device has been reached.";
        this.State = TransferRequestState.Failed;
        this.StatusText = ControlResources.StatusFailed;
      }
    }

    public void RequestCancel()
    {
      try
      {
        BackgroundTransferService.Remove(this._request);
      }
      catch (InvalidOperationException ex)
      {
        this.ErrorMessage = "The request has already been canceled.";
        this.State = TransferRequestState.Failed;
        this.StatusText = ControlResources.StatusFailed;
      }
      catch (ArgumentNullException ex)
      {
        this.ErrorMessage = "Invalid request";
        this.State = TransferRequestState.Failed;
        this.StatusText = ControlResources.StatusFailed;
      }
    }

    protected void RequestStateChanged(object sender, BackgroundTransferEventArgs args)
    {
      if (args == null)
        return;
      switch (args.Request.TransferStatus)
      {
        case TransferStatus.None:
          this.State = TransferRequestState.Pending;
          this.StatusText = ControlResources.StatusPending;
          break;
        case TransferStatus.Transferring:
          this.State = this.BytesTransferred > 0L ? (this.TransferType == TransferType.Upload ? TransferRequestState.Uploading : TransferRequestState.Downloading) : TransferRequestState.Pending;
          this.StatusText = this.TransferringStatusText();
          break;
        case TransferStatus.Waiting:
          this.State = TransferRequestState.Waiting;
          this.StatusText = ControlResources.StatusWaiting;
          break;
        case TransferStatus.WaitingForWiFi:
          this.State = TransferRequestState.Waiting;
          this.StatusText = ControlResources.StatusWaitingForWiFi;
          break;
        case TransferStatus.WaitingForExternalPower:
          this.State = TransferRequestState.Waiting;
          this.StatusText = ControlResources.StatusWaitingForExternalPower;
          break;
        case TransferStatus.WaitingForExternalPowerDueToBatterySaverMode:
          this.State = TransferRequestState.Waiting;
          this.StatusText = ControlResources.StatusWaitingForExternalPowerDueToBatterySaverMode;
          break;
        case TransferStatus.WaitingForNonVoiceBlockingNetwork:
          this.State = TransferRequestState.Waiting;
          this.StatusText = ControlResources.StatusWaitingForNonVoiceBlockingNetwork;
          break;
        case TransferStatus.Paused:
          this.State = TransferRequestState.Paused;
          this.StatusText = ControlResources.StatusPaused;
          break;
        case TransferStatus.Completed:
          if (this._request.TransferError != null)
          {
            this.ErrorMessage = this._request.TransferError.Message;
            this.State = TransferRequestState.Failed;
            this.StatusText = this.ErrorMessage.Contains("canceled") ? ControlResources.StatusCancelled : ControlResources.StatusFailed;
            break;
          }
          this.State = TransferRequestState.Complete;
          this.StatusText = ControlResources.StatusComplete;
          break;
        case TransferStatus.Unknown:
          this.State = TransferRequestState.Unknown;
          this.StatusText = ControlResources.StatusCancelled;
          break;
      }
    }

    private void RequestProgressChanged(object sender, BackgroundTransferEventArgs e)
    {
      if (this.ProgressChanged != null)
        this.ProgressChanged((object) this, e);
      this.State = this.TransferType == TransferType.Upload ? TransferRequestState.Uploading : TransferRequestState.Downloading;
      this.StatusText = this.TransferringStatusText();
    }

    protected string TransferringStatusText()
    {
      this.IsProgressIndeterminate = this.TransferType == TransferType.Upload ? this._request.TotalBytesToSend <= 0L : this._request.TotalBytesToReceive <= 0L;
      long bytes1 = this.TransferType == TransferType.Upload ? this._request.TotalBytesToSend : this._request.TotalBytesToReceive;
      long bytes2 = this.TransferType == TransferType.Upload ? this._request.BytesSent : this._request.BytesReceived;
      if (bytes2 <= 0L)
        return ControlResources.StatusPending;
      string str1 = this.TransferType == TransferType.Upload ? ControlResources.StatusUploading : ControlResources.StatusDownloading;
      string str2 = !this.IsProgressIndeterminate ? string.Format(ControlResources.PartOfWhole, (object) TransferMonitor.BytesToString(bytes2), (object) TransferMonitor.BytesToString(bytes1)) : (bytes2 <= 0L ? "" : TransferMonitor.BytesToString(bytes2));
      this.TotalBytesToTransfer = bytes1;
      this.BytesTransferred = bytes2;
      return string.Format("{0} {1}", (object) str1, (object) str2);
    }

    protected static string BytesToString(long bytes)
    {
      string[] strArray = new string[4]
      {
        ControlResources.Byte,
        ControlResources.Kilobyte,
        ControlResources.Megabyte,
        ControlResources.Gigabyte
      };
      if (bytes <= 0L)
        return "0 " + strArray[0];
      int int32 = Convert.ToInt32(Math.Min(3.0, Math.Floor(Math.Log((double) bytes, 1024.0))));
      return string.Format("{0} {1}", (object) Math.Round((double) bytes / Math.Pow(1024.0, (double) int32), 1), (object) strArray[int32]);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged == null)
        return;
      propertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
