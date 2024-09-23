// Decompiled with JetBrains decompiler
// Type: WhatsApp.VerifiedNameRules
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll


namespace WhatsApp
{
  public static class VerifiedNameRules
  {
    public static string GetFirstChatName(UserStatus user, out bool checkMark)
    {
      checkMark = false;
      if (user.VerifiedLevel != VerifiedLevel.high)
        return user.GetDisplayName();
      string verifiedName = user.VerifiedNameCertificateDetails?.VerifiedName;
      if (user.IsInDeviceContactList && verifiedName != user.ContactName)
        return user.ContactName;
      checkMark = true;
      return verifiedName;
    }

    public static string GetSecondChatName(UserStatus user, out bool checkMark)
    {
      checkMark = false;
      if (user.VerifiedLevel != VerifiedLevel.high)
        return (string) null;
      string verifiedName = user.VerifiedNameCertificateDetails?.VerifiedName;
      if (!user.IsInDeviceContactList || verifiedName == user.ContactName)
        return (string) null;
      checkMark = true;
      return verifiedName;
    }

    public static string GetFirstInfoName(UserStatus user, out bool checkMark)
    {
      checkMark = false;
      if (user.VerifiedLevel != VerifiedLevel.high)
        return user.GetDisplayName();
      string verifiedName = user.VerifiedNameCertificateDetails?.VerifiedName;
      if (user.IsInDeviceContactList && verifiedName != user.ContactName)
        return user.ContactName;
      checkMark = true;
      return verifiedName;
    }

    public static string GetSecondInfoName(UserStatus user, out bool checkMark)
    {
      checkMark = false;
      string verifiedName = user.VerifiedNameCertificateDetails?.VerifiedName;
      if (user.VerifiedLevel == VerifiedLevel.high)
      {
        if (!user.IsInDeviceContactList || verifiedName == user.ContactName)
          return (string) null;
        checkMark = true;
        return verifiedName;
      }
      return user.IsInDevicePhonebook && verifiedName == user.ContactName ? (string) null : verifiedName;
    }

    public static bool IsApplicable(UserStatus user) => user != null && user.VerifiedLevel != 0;
  }
}
