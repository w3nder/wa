// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.SpeechTextBox
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Phone.Speech.Recognition;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class SpeechTextBox : PhoneTextBox
  {
    private bool _focused;
    private bool _useSelectedTextReplacement;
    private int _selectionStart;
    private int _selectionEnd;
    private bool _handlingSpeech;

    public SpeechTextBox()
    {
      this.ActionIcon = (ImageSource) new BitmapImage(new Uri("/Microsoft.Phone.Controls.Toolkit;Component/images/microphone.png", UriKind.Relative));
      this.ActionIconTapped += new EventHandler(this.Mic_ActionIconTapped);
      this.GotFocus += new RoutedEventHandler(this.PhraseBoxFocused);
      this.LostFocus += new RoutedEventHandler(this.PhraseBoxUnFocused);
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      if (this.ActionIconBorder == null)
        return;
      this.ActionIconBorder.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.MicButtonManipulationStarted);
    }

    public event EventHandler<SpeechRecognizedEventArgs> SpeechRecognized;

    private void Mic_ActionIconTapped(object sender, EventArgs e) => this.HandleSpeech();

    private async void HandleSpeech()
    {
      if (this._handlingSpeech)
        return;
      this._handlingSpeech = true;
      try
      {
        SpeechRecognizerUI recognizer = new SpeechRecognizerUI();
        SpeechRecognitionUIResult result = (SpeechRecognitionUIResult) null;
        if (this.InputScope != null)
        {
          if ((this.InputScope.Names[0] as InputScopeName).NameValue.Equals((object) InputScopeNameValue.Search))
            recognizer.Recognizer.Grammars.AddGrammarFromPredefinedType("WebSearchGrammar", (SpeechPredefinedGrammar) 2);
        }
        try
        {
          result = await recognizer.RecognizeWithUIAsync();
        }
        catch (OperationCanceledException ex)
        {
          return;
        }
        catch (Exception ex)
        {
          if (ex.HResult == -2147199736)
            return;
          int num = (int) MessageBox.Show("An error occured. \n" + ex.Message);
          return;
        }
        if (result.ResultStatus != null)
          return;
        EventHandler<SpeechRecognizedEventArgs> speechRecognized = this.SpeechRecognized;
        SpeechRecognizedEventArgs e = new SpeechRecognizedEventArgs(result.RecognitionResult);
        if (speechRecognized != null)
        {
          speechRecognized((object) this, e);
          if (e.Canceled)
            return;
        }
        string text = this.Text;
        if (this._useSelectedTextReplacement)
        {
          this.Text = text.Substring(0, this._selectionStart) + result.RecognitionResult.Text + text.Substring(this._selectionEnd + 1);
          this.Select(this._selectionStart, result.RecognitionResult.Text.Length);
        }
        else
        {
          this.Text = result.RecognitionResult.Text;
          this.Focus();
          this.Select(this._selectionStart, result.RecognitionResult.Text.Length);
        }
      }
      finally
      {
        this._handlingSpeech = false;
      }
    }

    private void MicButtonManipulationStarted(object sender, EventArgs e)
    {
      this._useSelectedTextReplacement = this._focused;
      this._selectionStart = this.SelectionStart;
      this._selectionEnd = this._selectionStart + this.SelectionLength - 1;
    }

    private void PhraseBoxFocused(object sender, RoutedEventArgs e) => this._focused = true;

    private void PhraseBoxUnFocused(object sender, RoutedEventArgs e) => this._focused = false;
  }
}
