// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.ConfigService.SizeOfint
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.ConfigService
{
  [DebuggerStepThrough]
  [DataContract(Name = "SizeOfint", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  internal class SizeOfint : INotifyPropertyChanged
  {
    private int HeightField;
    private int WidthField;

    [DataMember]
    internal int Height
    {
      get => this.HeightField;
      set
      {
        if (this.HeightField.Equals(value))
          return;
        this.HeightField = value;
        this.RaisePropertyChanged(nameof (Height));
      }
    }

    [DataMember]
    internal int Width
    {
      get => this.WidthField;
      set
      {
        if (this.WidthField.Equals(value))
          return;
        this.WidthField = value;
        this.RaisePropertyChanged(nameof (Width));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void RaisePropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged == null)
        return;
      propertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
