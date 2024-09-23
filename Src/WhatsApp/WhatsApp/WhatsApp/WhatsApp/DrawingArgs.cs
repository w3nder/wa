// Decompiled with JetBrains decompiler
// Type: WhatsApp.DrawingArgs
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Collections.Generic;
using System.Windows.Controls;


namespace WhatsApp
{
  public class DrawingArgs
  {
    public Grid DrawGrid { get; set; }

    public InkPresenter Canvas { get; set; }

    public Stack<DrawingAction> UndoList { get; set; }

    public bool HasDrawing { get; set; }

    public int OriginalHeight { get; set; }

    public int OriginalWidth { get; set; }
  }
}
