// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Transition
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class Transition : ITransition
  {
    private CacheMode _cacheMode;
    private UIElement _element;
    private bool _isHitTestVisible;
    private Storyboard _storyboard;

    protected Storyboard Storyboard
    {
      get => this._storyboard;
      set
      {
        if (value == this._storyboard)
          return;
        if (this._storyboard != null)
          this._storyboard.Completed -= new EventHandler(this.OnCompleted);
        this._storyboard = value;
        if (this._storyboard == null)
          return;
        this._storyboard.Completed += new EventHandler(this.OnCompleted);
      }
    }

    public event EventHandler Completed;

    public Transition(UIElement element, Storyboard storyboard)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      if (storyboard == null)
        throw new ArgumentNullException(nameof (storyboard));
      this._element = element;
      this.Storyboard = storyboard;
      this.Storyboard.Completed += new EventHandler(this.OnCompletedRestore);
    }

    public virtual void Begin()
    {
      this.Save();
      this.Storyboard.Begin();
    }

    private void OnCompletedRestore(object sender, EventArgs e) => this.Restore();

    public ClockState GetCurrentState() => this.Storyboard.GetCurrentState();

    public TimeSpan GetCurrentTime() => this.Storyboard.GetCurrentTime();

    public void Pause() => this.Storyboard.Pause();

    private void Restore()
    {
      if (!(this._cacheMode is BitmapCache))
        this._element.CacheMode = this._cacheMode;
      if (this._isHitTestVisible)
        this._element.IsHitTestVisible = this._isHitTestVisible;
      else
        this._element.IsHitTestVisible = true;
    }

    public void Resume() => this.Storyboard.Resume();

    private void Save()
    {
      this._cacheMode = this._element.CacheMode;
      if (!(this._cacheMode is BitmapCache))
        this._element.CacheMode = TransitionFrame.BitmapCacheMode;
      this._isHitTestVisible = this._element.IsHitTestVisible;
      if (!this._isHitTestVisible)
        return;
      this._element.IsHitTestVisible = false;
    }

    private void OnCompleted(object sender, EventArgs e)
    {
      EventHandler completed = this.Completed;
      if (completed == null)
        return;
      completed((object) this, e);
    }

    public void Seek(TimeSpan offset) => this.Storyboard.Seek(offset);

    public void SeekAlignedToLastTick(TimeSpan offset)
    {
      this.Storyboard.SeekAlignedToLastTick(offset);
    }

    public void SkipToFill() => this.Storyboard.SkipToFill();

    public void Stop()
    {
      this.Storyboard.Stop();
      this.Restore();
    }
  }
}
