// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChatPageWrapper
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;


namespace WhatsApp
{
  public class ChatPageWrapper : PageWrapper
  {
    private ChatPage chatPage_;
    private ChatPage.InputMode inputModePreRotation_ = ChatPage.InputMode.None;

    public ChatPageWrapper(ChatPage page)
      : base((PhoneApplicationPage) page)
    {
      this.chatPage_ = page;
    }

    protected override bool OnRotationTransitionStarting(PageOrientation newOrientation)
    {
      if (this.chatPage_ != null)
      {
        this.inputModePreRotation_ = this.chatPage_.CurrentInputMode;
        if (!this.chatPage_.OnRotationTransitionStarting(newOrientation))
          return false;
      }
      return true;
    }

    protected override void OnRotationTransitionFinished()
    {
      if (this.chatPage_ != null)
        this.chatPage_.OnRotationTransitionFinished(this.inputModePreRotation_);
      base.OnRotationTransitionFinished();
    }

    protected override bool ShouldDelayRotationTransition()
    {
      return this.inputModePreRotation_ == ChatPage.InputMode.Keyboard || base.ShouldDelayRotationTransition();
    }
  }
}
