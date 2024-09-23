// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.MetroFlowData
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System;
using System.ComponentModel;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  public class MetroFlowData : INotifyPropertyChanged
  {
    private Uri _imageUri;
    private string _title;

    public Uri ImageUri
    {
      get => this._imageUri;
      set
      {
        this._imageUri = value;
        this.RaisePropertyChanged(nameof (ImageUri));
      }
    }

    public string Title
    {
      get => this._title;
      set
      {
        this._title = value;
        this.RaisePropertyChanged(nameof (Title));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void RaisePropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
