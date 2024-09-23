// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.ISqlitePreparedStatement
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Guid(2410853660, 17876, 18742, 138, 90, 68, 47, 14, 86, 232, 232)]
  [Version(100794368)]
  public interface ISqlitePreparedStatement
  {
    void Dispose();

    bool Step();

    void Reset();

    void Bind([In] int Index, [In] object Value);

    int GetCount();

    object GetColumn([In] int Col);

    string GetColumnName([In] int Col);

    string GetError();

    string GetSql();
  }
}
