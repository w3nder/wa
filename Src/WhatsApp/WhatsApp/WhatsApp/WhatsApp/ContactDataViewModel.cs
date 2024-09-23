// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactDataViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Windows;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class ContactDataViewModel : WaViewModelBase
  {
    private string titleStr;
    private string valueStr;
    private bool isSelectable = true;

    public ContactDataViewModel(
      string vCardType,
      string title,
      string value,
      bool selectable = true,
      Func<object, bool> shouldKeep = null)
    {
      this.VCardType = vCardType;
      this.titleStr = title;
      this.valueStr = value;
      this.isSelectable = selectable;
      this.ShouldKeepValue = shouldKeep;
    }

    public Func<object, bool> ShouldKeepValue { get; private set; }

    public string VCardType { get; private set; }

    public string Title => this.titleStr;

    public string Value => this.valueStr;

    public bool IsChecked { get; set; }

    public Visibility CheckBoxVisibility => this.isSelectable.ToVisibility();
  }
}
