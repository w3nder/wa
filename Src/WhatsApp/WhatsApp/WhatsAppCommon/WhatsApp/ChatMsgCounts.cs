// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChatMsgCounts
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WhatsApp.Events;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public static class ChatMsgCounts
  {
    private const string logHeader = "cmc";
    private static int bytesQueuedToPromptProcessing = AppState.IsBackgroundAgent ? 128 : 1024;
    private static int maxBytesQueuedToPromptProcessing = AppState.IsBackgroundAgent ? 2048 : 8192;
    private static bool initiatedProcessingForQueueLength = false;
    private static object cmcLock = new object();
    private static long startUnixTime = -1;
    private const int secondsInADay = 86400;
    private const byte CmcStoredDetFmt = 1;
    private const byte CmcStoredDetRecv = 1;
    private const byte CmcStoredDetSend = 0;
    private static object cmcProcessLock = new object();
    private const byte CmcDailyFmt = 2;
    private const int numberOfChatBytes = 11;
    private const int offsetToSendCount = 0;
    private const int offsetToRecvCount = 4;
    private const int offsetToGroupFlag = 8;
    private const int offsetToContactFlag = 9;
    private const int offsetToTypeFlag = 10;

    public static void QueueToPending(string chatid, long? unixTimestamp, bool receivedMsg)
    {
      if (JidHelper.IsStatusJid(chatid))
        return;
      try
      {
        int pendingImpl = ChatMsgCounts.QueueToPendingImpl(chatid, unixTimestamp, receivedMsg);
        if (pendingImpl > ChatMsgCounts.bytesQueuedToPromptProcessing)
        {
          if (ChatMsgCounts.initiatedProcessingForQueueLength)
            return;
          Log.l("cmc", "initating processing - queued bytes {0}", (object) pendingImpl);
          ChatMsgCounts.initiatedProcessingForQueueLength = true;
          ChatMsgCounts.ProcessPending();
        }
        else
          ChatMsgCounts.initiatedProcessingForQueueLength = false;
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "exception queuing chat message", logOnlyForRelease: true);
      }
    }

    public static void ProcessPending()
    {
      WAThreadPool.QueueUserWorkItem((Action) (() =>
      {
        try
        {
          byte[] cmcPendingDetails = Settings.CmcPendingDetails;
          if (cmcPendingDetails != null && cmcPendingDetails.Length > ChatMsgCounts.maxBytesQueuedToPromptProcessing)
          {
            Log.d("cmc", "too many bytes to process at once: {0}, limit {1}", (object) cmcPendingDetails.Length, (object) ChatMsgCounts.maxBytesQueuedToPromptProcessing);
            Log.SendCrashLog((Exception) new InvalidOperationException("cmc too many queued bytes"), "too many queued bytes");
            ChatMsgCounts.StripProcessedBytesFromPending(cmcPendingDetails.Length);
          }
          else
            ChatMsgCounts.ProcessPendingImpl();
        }
        catch (Exception ex)
        {
          Log.SendCrashLog(ex, "exception sending chat message counts");
        }
        finally
        {
          ChatMsgCounts.initiatedProcessingForQueueLength = false;
        }
      }));
    }

    private static long GetStartUnixTime()
    {
      if (ChatMsgCounts.startUnixTime == -1L)
      {
        lock (ChatMsgCounts.cmcLock)
        {
          ChatMsgCounts.startUnixTime = Settings.CmcStartTs;
          if (ChatMsgCounts.startUnixTime <= 0L)
          {
            long unixTime;
            long num = (long) new Random((int) (unixTime = FunRunner.CurrentServerTimeUtc.ToUnixTime())).Next(0, 86400);
            ChatMsgCounts.startUnixTime = unixTime - num;
            Settings.CmcStartTs = ChatMsgCounts.startUnixTime;
            Settings.CmcPendingDetails = (byte[]) null;
            Settings.CmcDailyDetails = (byte[]) null;
          }
        }
      }
      return ChatMsgCounts.startUnixTime;
    }

    private static int ConvertToDay(long unixTime)
    {
      return (int) ((unixTime - ChatMsgCounts.GetStartUnixTime()) / 86400L);
    }

    private static long ConvertFromDay(int day)
    {
      return (long) day * 86400L + ChatMsgCounts.GetStartUnixTime();
    }

    private static int QueueToPendingImpl(string chatid, long? unixTimestamp, bool receivedMsg)
    {
      if (string.IsNullOrEmpty(chatid))
        return 0;
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 1);
      binaryData.AppendByte(receivedMsg ? (byte) 1 : (byte) 0);
      binaryData.AppendInt32(ChatMsgCounts.ConvertToDay(unixTimestamp ?? FunRunner.CurrentServerTimeUtc.ToUnixTime()));
      binaryData.AppendStrWithLengthPrefix(chatid);
      byte[] sourceArray = binaryData.Get();
      byte[] array = (byte[]) null;
      lock (ChatMsgCounts.cmcLock)
      {
        array = Settings.CmcPendingDetails ?? new byte[0];
        int length = array.Length;
        Log.d("cmc", "Adding data for {0}", (object) chatid);
        Array.Resize<byte>(ref array, length + sourceArray.Length);
        Array.Copy((Array) sourceArray, 0, (Array) array, length, sourceArray.Length);
        Settings.CmcPendingDetails = array;
      }
      return array == null ? 0 : array.Length;
    }

    private static void StripProcessedBytesFromPending(int stripCount)
    {
      if (stripCount <= 0)
        return;
      lock (ChatMsgCounts.cmcLock)
      {
        byte[] sourceArray = Settings.CmcPendingDetails ?? new byte[0];
        int length = sourceArray.Length;
        if (length < stripCount)
        {
          Log.l("cmc", "Stripping more bytes {0} than present {1} in pending", (object) stripCount, (object) length);
          Log.SendCrashLog((Exception) new InvalidOperationException("stripping too many bytes"), "cmc", logOnlyForRelease: true);
          stripCount = length;
        }
        else
          Log.d("cmc", "Stripping bytes {0} from {1} in pending", (object) stripCount, (object) length);
        byte[] destinationArray = new byte[length - stripCount];
        if (destinationArray.Length != 0)
          Array.Copy((Array) sourceArray, stripCount, (Array) destinationArray, 0, destinationArray.Length);
        Settings.CmcPendingDetails = destinationArray;
      }
    }

    private static void ProcessPendingImpl()
    {
      DateTime? start = PerformanceTimer.Start(PerformanceTimer.Mode.DebugAndBeta);
      lock (ChatMsgCounts.cmcProcessLock)
      {
        int day = ChatMsgCounts.ConvertToDay(FunRunner.CurrentServerTimeUtc.ToUnixTime());
        byte[] bytes = Settings.CmcDailyDetails;
        if (bytes != null && bytes.Length >= 5 && new BinaryData(bytes).ReadByte(0) != (byte) 2)
        {
          Log.l("cmc", "Invalid data found processing daily collection - ignoring");
          bytes = (byte[]) null;
        }
        if (bytes == null || bytes.Length < 5)
        {
          BinaryData binaryData = new BinaryData();
          binaryData.AppendByte((byte) 2);
          binaryData.AppendInt32(day);
          bytes = binaryData.Get();
        }
        BinaryData binaryData1 = new BinaryData(bytes);
        int dailyCollectionDay = binaryData1.ReadInt32(1);
        byte[] cmcPendingDetails = Settings.CmcPendingDetails;
        Log.d("cmc", "ProcessPending {0}, {1}", (object) (cmcPendingDetails != null ? cmcPendingDetails.Length : -1), (object) (dailyCollectionDay == day));
        if ((cmcPendingDetails == null || cmcPendingDetails.Length == 0) && dailyCollectionDay == day)
          return;
        int offset1 = 5;
        Dictionary<long, byte[]> dictionary = new Dictionary<long, byte[]>();
        try
        {
          while (offset1 < binaryData1.Length())
          {
            long key = binaryData1.ReadLong64(offset1);
            int offset2 = offset1 + 8;
            byte[] numArray = binaryData1.ReadBytes(offset2, 11);
            offset1 = offset2 + 11;
            dictionary[key] = numArray;
          }
        }
        catch (Exception ex)
        {
          Log.l("cmc", "exception processing daily counts, ignoring currently persisted data");
          Log.SendCrashLog(ex, "processing daily data", logOnlyForRelease: true);
        }
        Log.d("cmc", "Found {0} entries in daily collection", (object) dictionary.Count<KeyValuePair<long, byte[]>>());
        int newOffset = 0;
        int stripCount = newOffset;
        BinaryData binaryData2 = new BinaryData(cmcPendingDetails == null ? new byte[0] : cmcPendingDetails);
        while (newOffset < binaryData2.Length())
        {
          if (binaryData2.ReadByte(newOffset) != (byte) 1)
          {
            Log.l("cmc", "Invalid pending data found - ignoring");
            newOffset = binaryData2.Length();
            break;
          }
          ++newOffset;
          byte num1 = binaryData2.ReadByte(newOffset);
          ++newOffset;
          int num2 = binaryData2.ReadInt32(newOffset);
          if (num2 != dailyCollectionDay)
          {
            ChatMsgCounts.SendDailyCollection(dailyCollectionDay, dictionary);
            dictionary.Clear();
            dailyCollectionDay = num2;
          }
          newOffset += 4;
          string chatId = binaryData2.ReadStrWithLengthPrefix(newOffset, out newOffset);
          ChatMsgCounts.UpdateDailyCollection(dictionary, chatId, num1 == (byte) 1);
          stripCount = newOffset;
        }
        if (dailyCollectionDay != day)
        {
          ChatMsgCounts.SendDailyCollection(dailyCollectionDay, dictionary);
          dictionary.Clear();
          dailyCollectionDay = day;
        }
        Log.d("cmc", "processing leaves {0} entries in daily collection for {1}", (object) dictionary.Count<KeyValuePair<long, byte[]>>(), (object) dailyCollectionDay);
        BinaryData binaryData3 = new BinaryData();
        binaryData3.AppendByte((byte) 2);
        binaryData3.AppendInt32(day);
        foreach (KeyValuePair<long, byte[]> keyValuePair in dictionary)
        {
          binaryData3.AppendLong64(keyValuePair.Key);
          binaryData3.AppendBytes((IEnumerable<byte>) keyValuePair.Value);
        }
        Settings.CmcDailyDetails = binaryData3.Get();
        ChatMsgCounts.StripProcessedBytesFromPending(stripCount);
      }
      PerformanceTimer.End("cmc ProcessPendingImpl", start);
    }

    private static void UpdateDailyCollection(
      Dictionary<long, byte[]> dailyCollection,
      string chatId,
      bool receivedMessage)
    {
      JidHelper.JidTypes jidType = JidHelper.GetJidType(chatId);
      switch (jidType)
      {
        case JidHelper.JidTypes.User:
        case JidHelper.JidTypes.Group:
        case JidHelper.JidTypes.Broadcast:
          Conversation convo = (Conversation) null;
          if (jidType == JidHelper.JidTypes.Broadcast || jidType != JidHelper.JidTypes.Group)
          {
            MessagesContext.Run((MessagesContext.MessagesCallback) (db2 => convo = db2.GetConversation(chatId, CreateOptions.None)));
            if (convo == null)
              break;
          }
          string[] strArray;
          if (jidType == JidHelper.JidTypes.Broadcast && !receivedMessage)
            strArray = convo.GetParticipantJids(true);
          else
            strArray = new string[1]{ chatId };
          foreach (string jid1 in strArray)
          {
            long hashForJid = ChatMsgCounts.CreateHashForJid(jid1);
            byte[] chatData = (byte[]) null;
            if (!dailyCollection.TryGetValue(hashForJid, out chatData))
            {
              Log.d("cmc", "Creating dictionary entry for {0}", (object) jid1);
              chatData = new byte[11];
              ChatMsgCounts.SetSendCount(chatData, 0);
              ChatMsgCounts.SetRecvCount(chatData, 0);
              ChatMsgCounts.SetGroupFlag(chatData, false);
              ChatMsgCounts.SetContactFlag(chatData, false);
              ChatMsgCounts.SetChatType(chatData, wam_enum_chat_type.INDIVIDUAL);
              string jid2 = (string) null;
              switch (jidType)
              {
                case JidHelper.JidTypes.User:
                  jid2 = jid1;
                  break;
                case JidHelper.JidTypes.Group:
                  ChatMsgCounts.SetGroupFlag(chatData, true);
                  jid2 = convo?.GroupOwner;
                  if (jid2 == Settings.MyJid)
                  {
                    ChatMsgCounts.SetContactFlag(chatData, true);
                    break;
                  }
                  break;
              }
              if (!string.IsNullOrEmpty(jid2))
              {
                UserStatus user = UserCache.Get(jid2, false);
                if (user != null)
                {
                  if (user.IsInDeviceContactList)
                    ChatMsgCounts.SetContactFlag(chatData, true);
                  if (user.IsEnterprise())
                    ChatMsgCounts.SetChatType(chatData, wam_enum_chat_type.ENT);
                  else if (user.IsSmb())
                    ChatMsgCounts.SetChatType(chatData, wam_enum_chat_type.SMB);
                }
              }
              dailyCollection[hashForJid] = chatData;
            }
            else
              Log.d("cmc", "Updating dictionary entry for {0}", (object) jid1);
            if (receivedMessage)
            {
              int count = ChatMsgCounts.GetRecvCount(chatData) + 1;
              ChatMsgCounts.SetRecvCount(chatData, count);
            }
            else
            {
              int count = ChatMsgCounts.GetSendCount(chatData) + 1;
              ChatMsgCounts.SetSendCount(chatData, count);
            }
          }
          break;
      }
    }

    private static long CreateHashForJid(string jid)
    {
      byte[] numArray = !string.IsNullOrEmpty(jid) ? MD5Core.GetHash(Encoding.UTF8.GetBytes(jid)) : throw new InvalidOperationException("Can't hash an empty string");
      return BitConverter.ToInt64(numArray, 0) ^ BitConverter.ToInt64(numArray, 8);
    }

    private static void SendDailyCollection(
      int dailyCollectionDay,
      Dictionary<long, byte[]> dailyCollection)
    {
      long num = ChatMsgCounts.ConvertFromDay(dailyCollectionDay);
      Log.l("cmc", "Sending {0} Entries for {1} - {2}", (object) dailyCollection.Count<KeyValuePair<long, byte[]>>(), (object) dailyCollectionDay, (object) num);
      foreach (byte[] chatData in dailyCollection.Values)
      {
        ChatMessageCounts chatMessageCounts = new ChatMessageCounts();
        chatMessageCounts.startTime = new long?(num);
        chatMessageCounts.chatTypeInd = new wam_enum_chat_type?(ChatMsgCounts.GetChatType(chatData));
        chatMessageCounts.messagesSent = new long?((long) ChatMsgCounts.GetSendCount(chatData));
        chatMessageCounts.messagesReceived = new long?((long) ChatMsgCounts.GetRecvCount(chatData));
        chatMessageCounts.isAContact = new bool?(ChatMsgCounts.IsContact(chatData));
        chatMessageCounts.isAGroup = new bool?(ChatMsgCounts.IsGroup(chatData));
        Log.d("cmc", "Sent: {0}, Recv: {1}, Group: {2}, Contact: {3}", (object) chatMessageCounts.messagesSent, (object) chatMessageCounts.messagesReceived, (object) chatMessageCounts.isAContact, (object) chatMessageCounts.isAGroup);
        chatMessageCounts.SaveEvent();
      }
    }

    private static int GetSendCount(byte[] chatData)
    {
      if (BitConverter.IsLittleEndian)
        return BitConverter.ToInt32(chatData, 0);
      byte[] destinationArray = new byte[4];
      Array.Copy((Array) chatData, 0, (Array) destinationArray, 0, destinationArray.Length);
      return BitConverter.ToInt32(destinationArray, 0);
    }

    private static void SetSendCount(byte[] chatData, int count)
    {
      byte[] bytes = BitConverter.GetBytes(count);
      if (!BitConverter.IsLittleEndian)
        Array.Reverse((Array) bytes);
      Array.Copy((Array) bytes, 0, (Array) chatData, 0, bytes.Length);
    }

    private static int GetRecvCount(byte[] chatData)
    {
      if (BitConverter.IsLittleEndian)
        return BitConverter.ToInt32(chatData, 4);
      byte[] destinationArray = new byte[4];
      Array.Copy((Array) chatData, 4, (Array) destinationArray, 0, destinationArray.Length);
      return BitConverter.ToInt32(destinationArray, 0);
    }

    private static void SetRecvCount(byte[] chatData, int count)
    {
      byte[] bytes = BitConverter.GetBytes(count);
      if (!BitConverter.IsLittleEndian)
        Array.Reverse((Array) bytes);
      Array.Copy((Array) bytes, 0, (Array) chatData, 4, bytes.Length);
    }

    private static bool IsGroup(byte[] chatData) => chatData[8] == (byte) 1;

    private static void SetGroupFlag(byte[] chatData, bool isGroup)
    {
      chatData[8] = isGroup ? (byte) 1 : (byte) 0;
    }

    private static bool IsContact(byte[] chatData) => chatData[9] == (byte) 1;

    private static void SetContactFlag(byte[] chatData, bool isContact)
    {
      chatData[9] = isContact ? (byte) 1 : (byte) 0;
    }

    private static wam_enum_chat_type GetChatType(byte[] chatData)
    {
      switch (chatData[10])
      {
        case 2:
          return wam_enum_chat_type.SMB;
        case 3:
          return wam_enum_chat_type.ENT;
        default:
          return wam_enum_chat_type.INDIVIDUAL;
      }
    }

    private static void SetChatType(byte[] chatData, wam_enum_chat_type type)
    {
      chatData[10] = (byte) type;
    }
  }
}
