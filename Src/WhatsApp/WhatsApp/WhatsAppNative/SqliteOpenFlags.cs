// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.SqliteOpenFlags
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Version(100794368)]
  public enum SqliteOpenFlags
  {
    READONLY = 1,
    READWRITE = 2,
    CREATE = 4,
    Defaults = 6,
    DELETEONCLOSE = 8,
    EXCLUSIVE = 16, // 0x00000010
    AUTOPROXY = 32, // 0x00000020
    URI = 64, // 0x00000040
    MEMORY = 128, // 0x00000080
    MAIN_DB = 256, // 0x00000100
    TEMP_DB = 512, // 0x00000200
    TRANSIENT_DB = 1024, // 0x00000400
    MAIN_JOURNAL = 2048, // 0x00000800
    TEMP_JOURNAL = 4096, // 0x00001000
    SUBJOURNAL = 8192, // 0x00002000
    MASTER_JOURNAL = 16384, // 0x00004000
    NOMUTEX = 32768, // 0x00008000
    OPEN_FULLMUTEX = 65536, // 0x00010000
    SHAREDCACHE = 131072, // 0x00020000
    PRIVATECACHE = 262144, // 0x00040000
    WAL = 524288, // 0x00080000
  }
}
