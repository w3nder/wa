// Decompiled with JetBrains decompiler
// Type: WhatsApp.EmojiViewModelGroup
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;


namespace WhatsApp
{
  public class EmojiViewModelGroup
  {
    private string[] values;

    public EmojiPickerViewModel ViewModel { get; set; }

    public EmojiPickerModel Model { get; set; }

    public Emoji.PickerCategory? GroupName { set; get; }

    public int FirstVisibleEmoji { get; set; }

    public int LastEmojiPagePosition { get; set; }

    public string[] Values
    {
      get
      {
        if (this.values == null && this.GroupName.HasValue)
        {
          Emoji.PickerCategory? groupName1 = this.GroupName;
          this.values = !groupName1.HasValue || groupName1.GetValueOrDefault() != Emoji.PickerCategory.Recent ? ((IEnumerable<EmojiModel>) EmojiModel.GetInstance()).Where<EmojiModel>((Func<EmojiModel, bool>) (m =>
          {
            int category = (int) m.Category;
            Emoji.PickerCategory? groupName2 = this.GroupName;
            int valueOrDefault = (int) groupName2.GetValueOrDefault();
            return category == valueOrDefault && groupName2.HasValue;
          })).Single<EmojiModel>().Values : new string[0];
        }
        return this.values;
      }
      set => this.values = value;
    }

    public Button PickerButton { get; set; }

    public Action<Emoji.EmojiChar> OnEmojiSelectedAction { get; set; }

    public EmojiViewModelGroup(
      EmojiPickerViewModel viewModel,
      EmojiPickerModel model,
      Emoji.PickerCategory? groupName = null,
      Button pickerButton = null)
    {
      this.ViewModel = viewModel;
      this.Model = model;
      if (!groupName.HasValue || pickerButton == null)
        return;
      this.GroupName = groupName;
      this.PickerButton = pickerButton;
      this.PickerButton.Tap += (EventHandler<GestureEventArgs>) ((sender, e) => this.ViewModel.CurrentEmojiGroupIndex = this.GroupName.Value);
    }
  }
}
