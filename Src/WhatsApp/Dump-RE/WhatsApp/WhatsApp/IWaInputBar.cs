// Decompiled with JetBrains decompiler
// Type: WhatsApp.IWaInputBar
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Windows.Controls;

#nullable disable
namespace WhatsApp
{
  public interface IWaInputBar
  {
    void SetOrientation(PageOrientation pageOrientation);

    void SetText(string text);

    ExtendedTextInputData GetInputData();

    string GetText();

    WebPageMetadata GetLinkPreviewData();

    bool IsTextEmptyOrWhiteSpace();

    bool IsEmojiKeyboardOpen();

    void OpenKeyboard();

    void OpenEmojiKeyboard();

    void CloseEmojiKeyboard(SIPStates nextState = SIPStates.Undefined);

    void Enable(bool enable);

    void Clear();

    void Dispose();

    IObservable<TextChangedEventArgs> TextChangedObservable();

    double GetSIPHeight();
  }
}
