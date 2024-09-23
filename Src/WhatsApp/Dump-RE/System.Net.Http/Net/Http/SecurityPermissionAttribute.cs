// Decompiled with JetBrains decompiler
// Type: System.Net.Http.SecurityPermissionAttribute
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

#nullable disable
namespace System.Net.Http
{
  internal class SecurityPermissionAttribute : Attribute
  {
    public SecurityPermissionAttribute(SecurityAction action)
    {
    }

    public SecurityPermissionFlag Flags { get; set; }
  }
}
