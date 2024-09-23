// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.SqlitePreparedStatement
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [MarshalingBehavior]
  public sealed class SqlitePreparedStatement : ISqlitePreparedStatement
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Dispose();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool Step();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Reset();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Bind([In] int Index, [In] object Value);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern int GetCount();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern object GetColumn([In] int Col);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern string GetColumnName([In] int Col);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern string GetError();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern string GetSql();
  }
}
