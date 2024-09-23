// Decompiled with JetBrains decompiler
// Type: WhatsApp.Test.TestData
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable disable
namespace WhatsApp.Test
{
  public static class TestData
  {
    private static Message LoadMessage(
      SqliteMessagesContext db,
      bool fromMe,
      bool getFromGroupChats,
      FunXMPP.FMessage.Type msgType,
      IEnumerable<string> additionalWhereClauses = null)
    {
      StringBuilder stringBuilder = new StringBuilder("SELECT * FROM Messages WHERE\n");
      stringBuilder.AppendFormat("KeyRemoteJid LIKE ?\n");
      stringBuilder.AppendFormat("AND KeyFromMe = ?\n");
      stringBuilder.AppendFormat("AND MediaWaType = ?\n");
      if (additionalWhereClauses != null)
      {
        foreach (string additionalWhereClause in additionalWhereClauses)
          stringBuilder.AppendFormat("AND {0}\n", (object) additionalWhereClause);
      }
      stringBuilder.Append("ORDER BY MessageID DESC LIMIT 1");
      using (Sqlite.PreparedStatement stmt = db.PrepareStatement(stringBuilder.ToString()))
      {
        stmt.Bind(0, string.Format("%{0}", getFromGroupChats ? (object) "@g.us" : (object) "@s.whatsapp.net"));
        stmt.Bind(1, fromMe);
        stmt.Bind(2, (int) msgType, false);
        return db.ParseTable<Message>(stmt, "Messages").FirstOrDefault<Message>();
      }
    }

    public static Message[] LoadAssortedMessages(SqliteMessagesContext db, bool getFromGroups)
    {
      LinkedList<Message> source = new LinkedList<Message>();
      source.AddLast(TestData.LoadMessage(db, true, getFromGroups, FunXMPP.FMessage.Type.Undefined));
      source.AddLast(TestData.LoadMessage(db, false, getFromGroups, FunXMPP.FMessage.Type.Undefined));
      source.AddLast(TestData.LoadMessage(db, true, getFromGroups, FunXMPP.FMessage.Type.Image));
      source.AddLast(TestData.LoadMessage(db, false, getFromGroups, FunXMPP.FMessage.Type.Image));
      source.AddLast(TestData.LoadMessage(db, true, (getFromGroups ? 1 : 0) != 0, FunXMPP.FMessage.Type.Image, (IEnumerable<string>) new string[1]
      {
        "MediaCaption IS NOT NULL"
      }));
      source.AddLast(TestData.LoadMessage(db, false, (getFromGroups ? 1 : 0) != 0, FunXMPP.FMessage.Type.Image, (IEnumerable<string>) new string[1]
      {
        "MediaCaption IS NOT NULL"
      }));
      source.AddLast(TestData.LoadMessage(db, true, getFromGroups, FunXMPP.FMessage.Type.Video));
      source.AddLast(TestData.LoadMessage(db, false, getFromGroups, FunXMPP.FMessage.Type.Video));
      source.AddLast(TestData.LoadMessage(db, true, (getFromGroups ? 1 : 0) != 0, FunXMPP.FMessage.Type.Video, (IEnumerable<string>) new string[1]
      {
        "MediaCaption IS NOT NULL"
      }));
      source.AddLast(TestData.LoadMessage(db, false, (getFromGroups ? 1 : 0) != 0, FunXMPP.FMessage.Type.Video, (IEnumerable<string>) new string[1]
      {
        "MediaCaption IS NOT NULL"
      }));
      source.AddLast(TestData.LoadMessage(db, true, (getFromGroups ? 1 : 0) != 0, FunXMPP.FMessage.Type.Audio, (IEnumerable<string>) new string[1]
      {
        "MediaOrigin IS NULL"
      }));
      source.AddLast(TestData.LoadMessage(db, false, (getFromGroups ? 1 : 0) != 0, FunXMPP.FMessage.Type.Audio, (IEnumerable<string>) new string[1]
      {
        "MediaOrigin IS NULL"
      }));
      source.AddLast(TestData.LoadMessage(db, true, (getFromGroups ? 1 : 0) != 0, FunXMPP.FMessage.Type.Audio, (IEnumerable<string>) new string[1]
      {
        "MediaOrigin IS NOT NULL"
      }));
      source.AddLast(TestData.LoadMessage(db, false, (getFromGroups ? 1 : 0) != 0, FunXMPP.FMessage.Type.Audio, (IEnumerable<string>) new string[1]
      {
        "MediaOrigin IS NOT NULL"
      }));
      source.AddLast(TestData.LoadMessage(db, true, (getFromGroups ? 1 : 0) != 0, FunXMPP.FMessage.Type.Location, (IEnumerable<string>) new string[1]
      {
        "LocationDetails IS NULL"
      }));
      source.AddLast(TestData.LoadMessage(db, false, (getFromGroups ? 1 : 0) != 0, FunXMPP.FMessage.Type.Location, (IEnumerable<string>) new string[1]
      {
        "LocationDetails IS NULL"
      }));
      source.AddLast(TestData.LoadMessage(db, true, (getFromGroups ? 1 : 0) != 0, FunXMPP.FMessage.Type.Location, (IEnumerable<string>) new string[1]
      {
        "LocationDetails IS NOT NULL"
      }));
      source.AddLast(TestData.LoadMessage(db, false, (getFromGroups ? 1 : 0) != 0, FunXMPP.FMessage.Type.Location, (IEnumerable<string>) new string[1]
      {
        "LocationDetails IS NOT NULL"
      }));
      source.AddLast(TestData.LoadMessage(db, true, getFromGroups, FunXMPP.FMessage.Type.Contact));
      source.AddLast(TestData.LoadMessage(db, false, getFromGroups, FunXMPP.FMessage.Type.Contact));
      source.AddLast(TestData.LoadMessage(db, false, true, FunXMPP.FMessage.Type.System));
      return source.Where<Message>((Func<Message, bool>) (m => m != null)).ToArray<Message>();
    }
  }
}
