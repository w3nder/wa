// Decompiled with JetBrains decompiler
// Type: WhatsApp.AttachPanelViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;


namespace WhatsApp
{
  public class AttachPanelViewModel : PropChangedBase
  {
    private PageOrientation orientation_;
    private List<AttachButtonViewModel> buttonsSrc;
    private Dictionary<AttachPanel.ActionType, AttachButtonViewModel> buttonVmsDict;

    public PageOrientation Orientation
    {
      get => this.orientation_;
      set
      {
        if (this.orientation_ == value)
          return;
        this.orientation_ = value;
        this.NotifyPropertyChanged("HeaderVisibility");
        this.buttonsSrc = (List<AttachButtonViewModel>) null;
        this.NotifyPropertyChanged("ButtonsSource");
      }
    }

    public List<AttachButtonViewModel> ButtonsSource
    {
      get
      {
        if (this.buttonsSrc != null)
          return this.buttonsSrc;
        AttachButtonViewModel[] vmsInLogicOrder = ((IEnumerable<AttachPanel.ActionType>) this.GetActionTypes()).Select<AttachPanel.ActionType, AttachButtonViewModel>((Func<AttachPanel.ActionType, AttachButtonViewModel>) (actType => this.buttonVmsDict.GetValueOrDefault<AttachPanel.ActionType, AttachButtonViewModel>(actType))).ToArray<AttachButtonViewModel>();
        int[] source;
        switch (this.orientation_)
        {
          case PageOrientation.Landscape:
          case PageOrientation.LandscapeLeft:
            source = new int[6]{ 2, 5, 1, 4, 0, 3 };
            break;
          case PageOrientation.LandscapeRight:
            source = new int[6]{ 3, 0, 4, 1, 5, 2 };
            break;
          default:
            source = new int[6]{ 0, 1, 2, 3, 4, 5 };
            break;
        }
        return this.buttonsSrc = ((IEnumerable<int>) source).Select<int, AttachButtonViewModel>((Func<int, AttachButtonViewModel>) (i => ((IEnumerable<AttachButtonViewModel>) vmsInLogicOrder).ElementAtOrDefault<AttachButtonViewModel>(i))).ToList<AttachButtonViewModel>();
      }
    }

    public AttachPanelViewModel()
    {
      this.buttonVmsDict = ((IEnumerable<AttachPanel.ActionType>) this.GetActionTypes()).Select<AttachPanel.ActionType, AttachButtonViewModel>((Func<AttachPanel.ActionType, AttachButtonViewModel>) (actType => new AttachButtonViewModel(actType))).ToDictionary<AttachButtonViewModel, AttachPanel.ActionType>((Func<AttachButtonViewModel, AttachPanel.ActionType>) (vm => vm.ActionType));
    }

    private AttachPanel.ActionType[] GetActionTypes()
    {
      return new AttachPanel.ActionType[6]
      {
        AttachPanel.ActionType.TakePictureOrVideo,
        AttachPanel.ActionType.ChoosePictureAndVideo,
        AttachPanel.ActionType.ChooseDocument,
        AttachPanel.ActionType.ShareContact,
        AttachPanel.ActionType.ChooseAudio,
        AttachPanel.ActionType.ShareLocation
      };
    }
  }
}
