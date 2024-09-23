// Decompiled with JetBrains decompiler
// Type: WhatsApp.FrequentChats
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;


namespace WhatsApp
{
  public static class FrequentChats
  {
    public static void CalculateScores()
    {
      DateTime? start = PerformanceTimer.Start(PerformanceTimer.Mode.DebugAndBeta);
      Dictionary<string, Dictionary<long, long>> jidsCounts = (Dictionary<string, Dictionary<long, long>>) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        jidsCounts = db.AccumulateRecentOutGoingMessages(FunRunner.CurrentServerTimeUtc.AddDays(-7.0), new int?(1000));
        db.ClearFrequentChats();
      }));
      Dictionary<string, FrequentChatScore> scoreTable = new Dictionary<string, FrequentChatScore>();
      foreach (KeyValuePair<string, Dictionary<long, long>> keyValuePair1 in jidsCounts)
      {
        if (!JidHelper.IsStatusJid(keyValuePair1.Key))
        {
          FrequentChatScore frequentChatScore = (FrequentChatScore) null;
          if (!scoreTable.TryGetValue(keyValuePair1.Key, out frequentChatScore))
          {
            frequentChatScore = new FrequentChatScore()
            {
              Jid = keyValuePair1.Key
            };
            scoreTable[keyValuePair1.Key] = frequentChatScore;
          }
          foreach (KeyValuePair<long, long> keyValuePair2 in keyValuePair1.Value)
          {
            long num = keyValuePair2.Value;
            frequentChatScore.DefaultScore += num;
            frequentChatScore.ImageScore += keyValuePair2.Key == 1L ? 10L * num : num;
            frequentChatScore.VideoScore += keyValuePair2.Key == 3L ? 10L * num : num;
          }
        }
      }
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        foreach (FrequentChatScore fcs in scoreTable.Values)
        {
          Log.d("freq chats", "{0} - {1}, {2}, {3}", (object) fcs.Jid, (object) fcs.DefaultScore, (object) fcs.ImageScore, (object) fcs.VideoScore);
          db.InsertFrequentChatScore(fcs);
        }
        db.SubmitChanges();
      }));
      Log.l(nameof (CalculateScores), "created {0} frequency counts", (object) scoreTable.Count);
      PerformanceTimer.End("frequent chat scores calculated", start);
    }
  }
}
