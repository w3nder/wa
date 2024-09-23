// Decompiled with JetBrains decompiler
// Type: WhatsApp.ParticipantSort
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;


namespace WhatsApp
{
  public class ParticipantSort
  {
    public static int CompareParticipants(
      UserStatus u1,
      UserStatus u2,
      bool compareFirstName,
      bool putSelfToBegin = false)
    {
      Func<UserStatus, string> defaultGenSortKeyFunc = ParticipantSort.GetDefaultGenSortKeyFunc(compareFirstName);
      string myJid = putSelfToBegin ? Settings.MyJid : (string) null;
      return ParticipantSort.SortState.CompareFunc(ParticipantSort.SortState.Create(u1, defaultGenSortKeyFunc, myJid), ParticipantSort.SortState.Create(u2, defaultGenSortKeyFunc, myJid));
    }

    private static Func<UserStatus, string> GetDefaultGenSortKeyFunc(bool sortByFirstName)
    {
      return (Func<UserStatus, string>) (u => u.GetDisplayName(sortByFirstName, false) ?? u.Jid);
    }

    public static void Sort(List<UserStatus> users, bool sortByFirstName, bool putSelfToFirst)
    {
      ParticipantSort.Sort(users, putSelfToFirst, ParticipantSort.GetDefaultGenSortKeyFunc(sortByFirstName));
    }

    public static void Sort(
      List<UserStatus> users,
      bool putSelfToBegin,
      Func<UserStatus, string> genSortKeyFunc = null)
    {
      if (users == null || users.Count < 2)
        return;
      if (genSortKeyFunc == null)
        genSortKeyFunc = ParticipantSort.GetDefaultGenSortKeyFunc(false);
      string ownJid = putSelfToBegin ? Settings.MyJid : (string) null;
      List<ParticipantSort.SortState> list = users.Select<UserStatus, ParticipantSort.SortState>((Func<UserStatus, ParticipantSort.SortState>) (us => ParticipantSort.SortState.Create(us, genSortKeyFunc, ownJid))).ToList<ParticipantSort.SortState>();
      list.Sort(new Comparison<ParticipantSort.SortState>(ParticipantSort.SortState.CompareFunc));
      int num = 0;
      foreach (ParticipantSort.SortState sortState in list)
        users[num++] = sortState.Status;
    }

    private class SortState
    {
      private string SortKey;
      private bool IsSelf;
      private bool IsNumber;

      public UserStatus Status { get; private set; }

      public static ParticipantSort.SortState Create(
        UserStatus u,
        Func<UserStatus, string> getKey,
        string ownJid)
      {
        if (u != null)
          return new ParticipantSort.SortState()
          {
            Status = u,
            SortKey = getKey(u) ?? "",
            IsNumber = u.ContactName == null,
            IsSelf = ownJid != null && StringComparer.OrdinalIgnoreCase.Equals(u.Jid, ownJid)
          };
        return new ParticipantSort.SortState()
        {
          Status = u,
          SortKey = "",
          IsNumber = false,
          IsSelf = false
        };
      }

      public static int CompareFunc(ParticipantSort.SortState s1, ParticipantSort.SortState s2)
      {
        if (s1.Status == null)
          return 1;
        if (s2.Status == null)
          return -1;
        if (s1.Status == s2.Status)
          return 0;
        if (s1.IsSelf)
          return -1;
        if (s2.IsSelf)
          return 1;
        if (!s1.IsNumber && s2.IsNumber)
          return -1;
        if (s1.IsNumber && !s2.IsNumber || string.IsNullOrEmpty(s1.SortKey) && !string.IsNullOrEmpty(s2.SortKey))
          return 1;
        return !string.IsNullOrEmpty(s1.SortKey) && string.IsNullOrEmpty(s2.SortKey) ? -1 : string.Compare(s1.SortKey, s2.SortKey, StringComparison.CurrentCulture);
      }
    }
  }
}
