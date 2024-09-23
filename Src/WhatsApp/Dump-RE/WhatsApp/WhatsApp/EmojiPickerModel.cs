// Decompiled with JetBrains decompiler
// Type: WhatsApp.EmojiPickerModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace WhatsApp
{
  public class EmojiPickerModel
  {
    private EmojiPicker Driver;
    public EmojiPickerViewModel ViewModel;
    private Dictionary<Emoji.PickerCategory, EmojiViewModelGroup> EmojiGroupContainer;
    private Emoji.PickerCategory[] GroupsOrder = new Emoji.PickerCategory[8]
    {
      Emoji.PickerCategory.SmileysAndPeople,
      Emoji.PickerCategory.AnimalsAndNature,
      Emoji.PickerCategory.FoodAndDrink,
      Emoji.PickerCategory.Activity,
      Emoji.PickerCategory.TravelAndPlaces,
      Emoji.PickerCategory.Objects,
      Emoji.PickerCategory.Symbols,
      Emoji.PickerCategory.Flags
    };
    public Action<Emoji.EmojiChar> onEmojiSelectedAction_;
    private EmojiViewModelGroup emojiSearchGroup;
    private Dictionary<Emoji.PickerCategory, int> lastEmojiPagePositions_;
    private LinkedList<string> pendingRecentUsedEmojis_ = new LinkedList<string>();
    private bool reloadRecentEmojisNeeded_ = true;
    private static int VisibleSlots = 50;

    public EmojiViewModelGroup EmojiSearchGroup
    {
      get
      {
        if (this.emojiSearchGroup == null)
        {
          this.emojiSearchGroup = new EmojiViewModelGroup(this.ViewModel, this);
          this.emojiSearchGroup.OnEmojiSelectedAction = (Action<Emoji.EmojiChar>) (emojiChar => this.onEmojiSelectedAction_(emojiChar));
        }
        return this.emojiSearchGroup;
      }
    }

    public EmojiViewModelGroup getEmojiGroup(Emoji.PickerCategory groupIndex)
    {
      EmojiViewModelGroup emojiGroup = this.EmojiGroupContainer[groupIndex];
      if (emojiGroup.OnEmojiSelectedAction == null)
      {
        int num = 0;
        if (this.lastEmojiPagePositions_ == null)
          this.LoadEmojiPagePositions();
        else
          this.lastEmojiPagePositions_.TryGetValue(groupIndex, out num);
        emojiGroup.OnEmojiSelectedAction = this.onEmojiSelectedAction_;
        emojiGroup.FirstVisibleEmoji = num;
      }
      return emojiGroup;
    }

    private void LoadEmojiPagePositions()
    {
      List<int> intList;
      try
      {
        intList = ((IEnumerable<string>) Settings.LastEmojiPagePositions.Split(",".ToArray<char>())).Select<string, int>((Func<string, int>) (s => int.Parse(s))).ToList<int>();
        if (intList.Count != this.GroupsOrder.Length)
          intList = (List<int>) null;
      }
      catch (Exception ex)
      {
        intList = (List<int>) null;
      }
      if (intList == null)
        intList = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0 };
      this.lastEmojiPagePositions_ = new Dictionary<Emoji.PickerCategory, int>();
      int num = 0;
      foreach (Emoji.PickerCategory key in this.GroupsOrder)
        this.lastEmojiPagePositions_[key] = intList[num++];
    }

    public void SaveEmojiPagePositions()
    {
      List<int> values = new List<int>();
      foreach (Emoji.PickerCategory key in this.GroupsOrder)
      {
        EmojiViewModelGroup emojiViewModelGroup = this.EmojiGroupContainer[key];
        values.Add(emojiViewModelGroup == null ? this.lastEmojiPagePositions_[key] : emojiViewModelGroup.FirstVisibleEmoji);
      }
      Settings.LastEmojiPagePositions = string.Join<int>(",", (IEnumerable<int>) values);
    }

    public void UpdateRecentEmojis()
    {
      if (this.pendingRecentUsedEmojis_.Count <= 0 && !this.reloadRecentEmojisNeeded_)
        return;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        if (this.pendingRecentUsedEmojis_.Count > 0)
          this.ApplyPendingEmojiUsages(db);
        if (!this.reloadRecentEmojisNeeded_)
          return;
        this.ReloadRecent(db);
        this.reloadRecentEmojisNeeded_ = false;
      }));
    }

    public void ReloadRecent(MessagesContext db)
    {
      this.getEmojiGroup(Emoji.PickerCategory.Recent).Values = db.GetAllEmojiUsages().Take<EmojiUsage>(EmojiPickerModel.VisibleSlots).Select<EmojiUsage, string>((Func<EmojiUsage, string>) (eu => eu.EmojiCode)).ToArray<string>();
    }

    public void ApplyPendingEmojiUsages()
    {
      if (this.pendingRecentUsedEmojis_.Count <= 0)
        return;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => this.ApplyPendingEmojiUsages(db)));
    }

    private void ApplyPendingEmojiUsages(MessagesContext db)
    {
      db.ProcessEmojiUsages((IEnumerable<string>) this.pendingRecentUsedEmojis_);
      this.pendingRecentUsedEmojis_.Clear();
    }

    private void RecordEmojiUsage(Emoji.EmojiChar emojiChar)
    {
      this.pendingRecentUsedEmojis_.AddLast(Emoji.GetBaseEmoji(emojiChar.codepoints));
      this.reloadRecentEmojisNeeded_ = true;
    }

    public EmojiPickerModel(
      EmojiPicker _base,
      EmojiPickerViewModel emojiPickerViewModel,
      Action<Emoji.EmojiChar> EmojiAction = null)
    {
      this.Driver = _base;
      this.ViewModel = emojiPickerViewModel;
      this.ViewModel.Model = this;
      if (EmojiAction == null)
        this.onEmojiSelectedAction_ = (Action<Emoji.EmojiChar>) (emojiChar =>
        {
          this.RecordEmojiUsage(emojiChar);
          this.ViewModel.InsertEmoji(emojiChar);
        });
      else
        this.onEmojiSelectedAction_ = EmojiAction;
    }

    public void InitGroupContainer()
    {
      this.EmojiGroupContainer = new Dictionary<Emoji.PickerCategory, EmojiViewModelGroup>();
      this.EmojiGroupContainer.Add(Emoji.PickerCategory.SmileysAndPeople, new EmojiViewModelGroup(this.ViewModel, this, new Emoji.PickerCategory?(Emoji.PickerCategory.SmileysAndPeople), this.Driver.SmileysAndPeopleButton));
      this.EmojiGroupContainer.Add(Emoji.PickerCategory.AnimalsAndNature, new EmojiViewModelGroup(this.ViewModel, this, new Emoji.PickerCategory?(Emoji.PickerCategory.AnimalsAndNature), this.Driver.AnimalsAndNatureButton));
      this.EmojiGroupContainer.Add(Emoji.PickerCategory.Activity, new EmojiViewModelGroup(this.ViewModel, this, new Emoji.PickerCategory?(Emoji.PickerCategory.Activity), this.Driver.ActivityButton));
      this.EmojiGroupContainer.Add(Emoji.PickerCategory.FoodAndDrink, new EmojiViewModelGroup(this.ViewModel, this, new Emoji.PickerCategory?(Emoji.PickerCategory.FoodAndDrink), this.Driver.FoodAndDrinkButton));
      this.EmojiGroupContainer.Add(Emoji.PickerCategory.TravelAndPlaces, new EmojiViewModelGroup(this.ViewModel, this, new Emoji.PickerCategory?(Emoji.PickerCategory.TravelAndPlaces), this.Driver.TravelAndPlacesButton));
      this.EmojiGroupContainer.Add(Emoji.PickerCategory.Objects, new EmojiViewModelGroup(this.ViewModel, this, new Emoji.PickerCategory?(Emoji.PickerCategory.Objects), this.Driver.ObjectsButton));
      this.EmojiGroupContainer.Add(Emoji.PickerCategory.Symbols, new EmojiViewModelGroup(this.ViewModel, this, new Emoji.PickerCategory?(Emoji.PickerCategory.Symbols), this.Driver.SymbolsButton));
      this.EmojiGroupContainer.Add(Emoji.PickerCategory.Flags, new EmojiViewModelGroup(this.ViewModel, this, new Emoji.PickerCategory?(Emoji.PickerCategory.Flags), this.Driver.FlagsButton));
      this.EmojiGroupContainer.Add(Emoji.PickerCategory.Recent, new EmojiViewModelGroup(this.ViewModel, this, new Emoji.PickerCategory?(Emoji.PickerCategory.Recent), this.Driver.RecentButton));
    }
  }
}
