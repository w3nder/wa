// Decompiled with JetBrains decompiler
// Type: WhatsApp.BackupProperties
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;


namespace WhatsApp
{
  public class BackupProperties
  {
    public string BackupId { get; set; }

    public DateTime StartTime { get; set; }

    public long Size { get; set; }

    public string LastBackupId { get; set; }

    public DateTime LastStartTime { get; set; }

    public long LastSize { get; set; }

    public long IncrementalSize { get; set; }

    public BackupSettings Settings { get; set; }

    public long IncompleteSize { get; set; }

    public long? RestoreSizeEstimate { get; set; }

    public long? RestoreCountEstimate { get; set; }

    public long RestoredSize { get; set; }

    public bool InProgress { get; set; }
  }
}
