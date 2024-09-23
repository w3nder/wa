// Decompiled with JetBrains decompiler
// Type: WhatsApp.AppDataExtensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public static class AppDataExtensions
  {
    public static bool IsTopTier(this UserStatus user)
    {
      return user.GetVerifiedTier() == VerifiedTier.Top;
    }

    public static VerifiedTier GetVerifiedTier(this UserStatus user)
    {
      return user != null ? VerifiedNamesCertifier.ConvertVerifiedLevel(user.VerifiedLevel) : VerifiedTier.NotApplicable;
    }

    public static bool IsVerified(this UserStatus user)
    {
      return user.VerifiedName == VerifiedNameState.Verified || user.VerifiedName == VerifiedNameState.PendingCertification;
    }

    public static bool IsVerified(this UserStatus user, VerifiedLevel verifiedLevel)
    {
      return user != null && user.IsVerified() && user.VerifiedLevel == verifiedLevel;
    }

    public static bool ShowVerifiedBadgeForWeb(this UserStatus user) => user.IsTopTier();

    public static bool VerifiedNameMatchesContactName(this UserStatus user)
    {
      bool flag = false;
      string lower = user?.VerifiedNameCertificateDetails?.VerifiedName?.ToLower();
      if (lower == null || !user.IsInDevicePhonebook || user.ContactName == null)
        return flag;
      string strB = user.ContactName.ToLower().Trim();
      if (string.Compare(lower, strB, StringComparison.InvariantCultureIgnoreCase) == 0)
        flag = true;
      return flag;
    }

    public static string GetVerifiedNameForDisplay(this UserStatus user)
    {
      return !user.IsVerified() ? (string) null : user.VerifiedNameCertificateDetails?.VerifiedName ?? user.PushName;
    }

    public static int? VerificationLevelForWeb(this UserStatus user)
    {
      if (!user.IsVerified())
        return new int?();
      switch (user.VerifiedLevel)
      {
        case VerifiedLevel.unknown:
          return new int?(0);
        case VerifiedLevel.low:
          return new int?(1);
        case VerifiedLevel.high:
          return new int?(2);
        default:
          return new int?();
      }
    }

    public static Triad<VerifiedLevel, string, VerifiedTier> GetVerifiedStateForDisplay(
      this UserStatus user)
    {
      int verifiedLevel = user.IsVerified() ? (int) user.VerifiedLevel : 0;
      return new Triad<VerifiedLevel, string, VerifiedTier>((VerifiedLevel) verifiedLevel, verifiedLevel == 0 ? (string) null : user.GetVerifiedNameForDisplay(), user.GetVerifiedTier());
    }

    public static Triad<VerifiedLevel, string, VerifiedTier> GetLastDisplayedVerifiedState(
      this UserStatus user)
    {
      UserStatusProperties.BusinessUserProperties userPropertiesField = UserStatusProperties.GetForUserStatus(user)?.BusinessUserPropertiesField;
      int? displayedVerifiedLevel = (int?) userPropertiesField?.LastDisplayedVerifiedLevel;
      int first = displayedVerifiedLevel.HasValue ? displayedVerifiedLevel.Value : 0;
      string displayedVerifiedName = first == 0 ? (string) null : userPropertiesField?.LastDisplayedVerifiedName;
      int? lastDisplayedTier = (int?) userPropertiesField?.LastDisplayedTier;
      VerifiedTier third = lastDisplayedTier.HasValue ? (VerifiedTier) lastDisplayedTier.Value : VerifiedTier.NotApplicable;
      return new Triad<VerifiedLevel, string, VerifiedTier>((VerifiedLevel) first, displayedVerifiedName, third);
    }

    public static bool IsEnterprise(this UserStatus user)
    {
      return user.VerifiedNameCertificateDetails?.Issuer?.StartsWith("ent:") ?? false;
    }

    public static bool IsSmb(this UserStatus user)
    {
      return user.VerifiedNameCertificateDetails?.Issuer?.StartsWith("smb:") ?? false;
    }

    public enum VerifiedNameMatch
    {
      NoMatch,
      FuzzyMatch,
      Match,
    }
  }
}
