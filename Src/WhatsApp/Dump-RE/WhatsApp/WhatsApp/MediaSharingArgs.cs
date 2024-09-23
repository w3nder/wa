// Decompiled with JetBrains decompiler
// Type: WhatsApp.MediaSharingArgs
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Windows.Navigation;

#nullable disable
namespace WhatsApp
{
  public class MediaSharingArgs
  {
    public MediaSharingState SharingState { get; private set; }

    public MediaSharingArgs.SharingStatus Status { get; set; }

    public NavigationService NavService { get; private set; }

    public Action<Action> NavTransition { get; set; }

    public MediaSharingArgs(
      MediaSharingState state,
      MediaSharingArgs.SharingStatus status,
      NavigationService nav)
    {
      this.SharingState = state;
      this.Status = status;
      this.NavService = nav;
      this.NavTransition = (Action<Action>) null;
    }

    public enum SharingStatus
    {
      None,
      Submitted,
      Canceled,
    }
  }
}
