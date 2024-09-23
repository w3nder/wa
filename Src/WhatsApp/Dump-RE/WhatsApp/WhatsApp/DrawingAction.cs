// Decompiled with JetBrains decompiler
// Type: WhatsApp.DrawingAction
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

#nullable disable
namespace WhatsApp
{
  public class DrawingAction
  {
    public object item { get; set; }

    public object previousState { get; set; }

    public DrawingAction.DrawingActionType type { get; set; }

    public DrawingAction(object item)
    {
      this.item = item;
      this.previousState = (object) null;
      this.type = DrawingAction.DrawingActionType.Add;
    }

    public DrawingAction(object item, object previousState, DrawingAction.DrawingActionType type)
    {
      this.item = item;
      this.previousState = previousState;
      this.type = type;
    }

    public enum DrawingActionType
    {
      Add,
      Transform,
      AlterText,
    }
  }
}
