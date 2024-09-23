// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.ISqlite
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Guid(4243948536, 60328, 19645, 157, 137, 149, 27, 116, 93, 62, 241)]
  [Version(100794368)]
  public interface ISqlite
  {
    void Open([In] string Filename, [In] SqliteOpenFlags Flags, [In] string Vfs);

    void SetBusyTimeout([In] int Ms);

    void PrepareStatement(
      [In] string Sql,
      [In] int SqlLen,
      out int TailOffset,
      out ISqlitePreparedStatement ReturnedObject);

    void Dispose();

    string GetError();

    void Interrupt();

    void RegisterTokenizer();

    bool IsTokenizerRegistered();

    int GetChangeCount();

    long GetLastRowId();

    ISqliteBackup InitializeBackup([In] string myDbName, [In] ISqlite foreignDb, [In] string foreignDbName);
  }
}
