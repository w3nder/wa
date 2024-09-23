// Decompiled with JetBrains decompiler
// Type: System.Net.ServicePoint
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

#nullable disable
namespace System.Net
{
  internal class ServicePoint
  {
    public static readonly ServicePoint Instance = new ServicePoint();

    public bool Expect100Continue
    {
      set
      {
      }
    }
  }
}
