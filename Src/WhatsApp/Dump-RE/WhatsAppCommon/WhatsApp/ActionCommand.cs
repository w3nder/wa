// Decompiled with JetBrains decompiler
// Type: WhatsApp.ActionCommand
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Windows.Input;

#nullable disable
namespace WhatsApp
{
  public class ActionCommand : ICommand
  {
    private Action a;

    public ActionCommand(Action a) => this.a = a;

    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object o) => true;

    public void Execute(object o) => this.a();
  }
}
