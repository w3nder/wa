// Decompiled with JetBrains decompiler
// Type: WhatsApp.PageArgs
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Windows.Navigation;

#nullable disable
namespace WhatsApp
{
  public class PageArgs
  {
    public NavigationService NavService { get; private set; }

    public GlobalProgressIndicator ProgressIndicator { get; private set; }

    public object Tag { get; set; }

    public PageArgs(NavigationService navService, GlobalProgressIndicator progressIndicator = null)
    {
      this.NavService = navService;
      this.ProgressIndicator = progressIndicator;
    }
  }
}
