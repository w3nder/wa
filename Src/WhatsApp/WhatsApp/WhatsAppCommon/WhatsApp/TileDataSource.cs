// Decompiled with JetBrains decompiler
// Type: WhatsApp.TileDataSource
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;


namespace WhatsApp
{
  public class TileDataSource
  {
    public string LogIdentifier { get; private set; }

    public string Jid { get; private set; }

    public Func<string[]> GetContents { get; private set; }

    public Func<string> GetSecondaryContents { get; private set; }

    public Func<string> GetSenderDisplayName { get; private set; }

    private TileDataSource()
    {
    }

    public static TileDataSource CreateForMessage(Message m)
    {
      return new TileDataSource()
      {
        LogIdentifier = m.KeyId,
        Jid = m.KeyRemoteJid,
        GetContents = (Func<string[]>) (() => NotificationString.GetTwoLineForMessage(m, true)),
        GetSecondaryContents = (Func<string>) (() => TileHelper.FormatSecondaryTileContent(m)),
        GetSenderDisplayName = (Func<string>) (() => m.GetSenderDisplayName(!JidHelper.IsUserJid(m.KeyRemoteJid)))
      };
    }

    public static TileDataSource CreateForMissedCall(string jid, string displayName)
    {
      return new TileDataSource()
      {
        LogIdentifier = string.Format("missed[jid={0}]", (object) jid),
        Jid = (string) null,
        GetContents = (Func<string[]>) (() => new string[2]
        {
          AppResources.MissedCallNotification,
          displayName
        }),
        GetSecondaryContents = (Func<string>) (() => (string) null),
        GetSenderDisplayName = (Func<string>) (() => displayName)
      };
    }
  }
}
