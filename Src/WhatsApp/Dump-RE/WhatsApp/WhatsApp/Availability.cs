// Decompiled with JetBrains decompiler
// Type: WhatsApp.Availability
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Runtime.Serialization;

#nullable disable
namespace WhatsApp
{
  [DataContract]
  public class Availability
  {
    [DataMember(Name = "available")]
    public bool Available { get; set; }
  }
}
