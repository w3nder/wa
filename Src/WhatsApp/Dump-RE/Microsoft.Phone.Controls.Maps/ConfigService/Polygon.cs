// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.ConfigService.Polygon
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.ConfigService
{
  [DebuggerStepThrough]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [DataContract(Name = "Polygon", Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal class Polygon : ShapeBase
  {
    private ObservableCollection<Location> VerticesField;

    [DataMember]
    internal ObservableCollection<Location> Vertices
    {
      get => this.VerticesField;
      set
      {
        if (object.ReferenceEquals((object) this.VerticesField, (object) value))
          return;
        this.VerticesField = value;
        this.RaisePropertyChanged(nameof (Vertices));
      }
    }
  }
}
