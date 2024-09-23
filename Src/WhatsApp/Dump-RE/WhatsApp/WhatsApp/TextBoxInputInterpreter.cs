// Decompiled with JetBrains decompiler
// Type: WhatsApp.TextBoxInputInterpreter
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class TextBoxInputInterpreter
  {
    private const string LogHeader = "textbox interp";
    private CircularStack<TextBoxInputInterpreter.TextBoxEvent> pendingEvents = new CircularStack<TextBoxInputInterpreter.TextBoxEvent>(5);

    public void Clear() => this.pendingEvents.Clear();

    public TextBoxInputInterpreter.TextBoxDelta PushSelectionChangedEvent(
      int currSelStart,
      int currSelLen)
    {
      this.pendingEvents.Push(new TextBoxInputInterpreter.TextBoxEvent()
      {
        EventType = TextBoxInputInterpreter.TextBoxEvent.EventTypes.SelectionChanged,
        SelectionStart = currSelStart,
        SelectionLength = currSelLen
      });
      return this.TryInterpret();
    }

    public TextBoxInputInterpreter.TextBoxDelta PushTextChangedEvent(int lengthDelta)
    {
      this.pendingEvents.Push(new TextBoxInputInterpreter.TextBoxEvent()
      {
        EventType = TextBoxInputInterpreter.TextBoxEvent.EventTypes.TextChanged,
        TextLengthDelta = lengthDelta
      });
      return this.TryInterpret();
    }

    private TextBoxInputInterpreter.TextBoxDelta TryInterpret()
    {
      TextBoxInputInterpreter.TextBoxDelta textBoxDelta = (TextBoxInputInterpreter.TextBoxDelta) null;
      int count = this.pendingEvents.Count;
      if (count < 2)
        return textBoxDelta;
      TextBoxInputInterpreter.TextBoxEvent textBoxEvent1 = this.pendingEvents.PeekAtOrDefault();
      TextBoxInputInterpreter.TextBoxEvent textBoxEvent2 = this.pendingEvents.PeekAtOrDefault(1);
      TextBoxInputInterpreter.TextBoxEvent textBoxEvent3 = (TextBoxInputInterpreter.TextBoxEvent) null;
      if (textBoxEvent2.EventType == TextBoxInputInterpreter.TextBoxEvent.EventTypes.TextChanged && textBoxEvent1.EventType == TextBoxInputInterpreter.TextBoxEvent.EventTypes.SelectionChanged && textBoxEvent1.SelectionLength == 0)
      {
        if (count > 3)
        {
          textBoxEvent3 = this.pendingEvents.PeekAtOrDefault(2);
          TextBoxInputInterpreter.TextBoxEvent textBoxEvent4 = this.pendingEvents.PeekAtOrDefault(3);
          if (textBoxEvent4.EventType == TextBoxInputInterpreter.TextBoxEvent.EventTypes.SelectionChanged && textBoxEvent4.SelectionLength > 0 && textBoxEvent3.EventType == TextBoxInputInterpreter.TextBoxEvent.EventTypes.SelectionChanged && textBoxEvent3.SelectionLength == 0)
          {
            int selectionStart = textBoxEvent4.SelectionStart;
            int selectionLength = textBoxEvent4.SelectionLength;
            int textLengthDelta = textBoxEvent2.TextLengthDelta;
            int num = selectionStart + selectionLength + textLengthDelta;
            if (textBoxEvent3.SelectionStart == num && textBoxEvent1.SelectionStart == num)
              textBoxDelta = new TextBoxInputInterpreter.TextBoxDelta(selectionStart, selectionLength, selectionStart, selectionLength + textLengthDelta);
          }
        }
        if (textBoxDelta == null && count > 2)
        {
          TextBoxInputInterpreter.TextBoxEvent textBoxEvent5 = textBoxEvent3 ?? this.pendingEvents.PeekAtOrDefault(2);
          if (textBoxEvent5.EventType == TextBoxInputInterpreter.TextBoxEvent.EventTypes.SelectionChanged && textBoxEvent5.SelectionLength > 0)
          {
            int selectionStart = textBoxEvent5.SelectionStart;
            int selectionLength = textBoxEvent5.SelectionLength;
            int textLengthDelta = textBoxEvent2.TextLengthDelta;
            if (textBoxEvent1.SelectionStart == selectionStart + selectionLength + textLengthDelta)
              textBoxDelta = new TextBoxInputInterpreter.TextBoxDelta(selectionStart, selectionLength, selectionStart, selectionLength + textLengthDelta);
          }
        }
        if (textBoxDelta == null)
        {
          int selectionStart = textBoxEvent1.SelectionStart;
          int textLengthDelta = textBoxEvent2.TextLengthDelta;
          textBoxDelta = textLengthDelta >= 0 ? new TextBoxInputInterpreter.TextBoxDelta(selectionStart - textLengthDelta, 0, selectionStart - textLengthDelta, textLengthDelta) : new TextBoxInputInterpreter.TextBoxDelta(selectionStart, -textLengthDelta, selectionStart, 0);
        }
      }
      if (textBoxDelta != null)
      {
        this.pendingEvents.Clear();
        if (textBoxDelta.OldSegmentStart >= 0 && textBoxDelta.NewSegmentStart >= 0 && textBoxDelta.OldSegmentLen >= 0 && textBoxDelta.NewSegmentLen >= 0)
          return textBoxDelta;
      }
      return (TextBoxInputInterpreter.TextBoxDelta) null;
    }

    public class TextBoxDelta
    {
      public int OldSegmentStart;
      public int OldSegmentLen;
      public int NewSegmentStart;
      public int NewSegmentLen;

      public int LengthDelta => this.NewSegmentLen - this.OldSegmentLen;

      public TextBoxDelta(int oldSegStart, int oldSegLen, int newSegStart, int newSegLen)
      {
        this.OldSegmentStart = oldSegStart;
        this.OldSegmentLen = oldSegLen;
        this.NewSegmentStart = newSegStart;
        this.NewSegmentLen = newSegLen;
      }
    }

    private class TextBoxEvent
    {
      public TextBoxInputInterpreter.TextBoxEvent.EventTypes EventType;
      public int SelectionStart = -1;
      public int SelectionLength = -1;
      public int TextLengthDelta;

      public enum EventTypes
      {
        Undefined,
        SelectionChanged,
        TextChanged,
      }
    }
  }
}
