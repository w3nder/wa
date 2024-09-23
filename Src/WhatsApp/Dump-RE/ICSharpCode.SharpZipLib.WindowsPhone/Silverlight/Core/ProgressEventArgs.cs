// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Core.ProgressEventArgs
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Core
{
  public class ProgressEventArgs : EventArgs
  {
    private readonly string name_;
    private readonly long processed_;
    private readonly long target_;
    private bool continueRunning_ = true;

    public ProgressEventArgs(string name, long processed, long target)
    {
      this.name_ = name;
      this.processed_ = processed;
      this.target_ = target;
    }

    public string Name => this.name_;

    public bool ContinueRunning
    {
      get => this.continueRunning_;
      set => this.continueRunning_ = value;
    }

    public float PercentComplete
    {
      get
      {
        return this.target_ <= 0L ? 0.0f : (float) ((double) this.processed_ / (double) this.target_ * 100.0);
      }
    }

    public long Processed => this.processed_;

    public long Target => this.target_;
  }
}
