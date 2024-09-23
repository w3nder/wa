// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.Sqlite
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Activatable(100794368)]
  [Version(100794368)]
  [MarshalingBehavior]
  public sealed class Sqlite : ISqlite
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern Sqlite();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Open([In] string Filename, [In] SqliteOpenFlags Flags, [In] string Vfs);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetBusyTimeout([In] int Ms);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void PrepareStatement(
      [In] string Sql,
      [In] int SqlLen,
      out int TailOffset,
      out ISqlitePreparedStatement ReturnedObject);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Dispose();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern string GetError();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Interrupt();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void RegisterTokenizer();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool IsTokenizerRegistered();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern int GetChangeCount();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern long GetLastRowId();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern ISqliteBackup InitializeBackup(
      [In] string myDbName,
      [In] ISqlite foreignDb,
      [In] string foreignDbName);
  }
}
