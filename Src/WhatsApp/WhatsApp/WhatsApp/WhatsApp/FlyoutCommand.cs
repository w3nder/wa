// Decompiled with JetBrains decompiler
// Type: WhatsApp.FlyoutCommand
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;


namespace WhatsApp
{
  public class FlyoutCommand
  {
    public string Title;
    public bool IsEnabled;
    public Action ClickAction;
    public string AutomationId;

    public FlyoutCommand(string title, bool isEnabled, Action clickAction, string automationId = null)
    {
      this.Title = title;
      this.IsEnabled = isEnabled;
      this.ClickAction = clickAction;
      this.AutomationId = automationId;
    }
  }
}
