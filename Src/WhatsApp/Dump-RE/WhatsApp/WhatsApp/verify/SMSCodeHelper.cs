// Decompiled with JetBrains decompiler
// Type: WhatsApp.verify.SMSCodeHelper
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

#nullable disable
namespace WhatsApp.verify
{
  public static class SMSCodeHelper
  {
    private static string SavedSmsDetailsSeparatorString = ",";
    private static char SavedSmsDetailsSeparatorChar = ',';

    public static void SaveSMSCode(string smsCode)
    {
      if (smsCode == null)
      {
        Settings.RegistrationSmsDetails = (string) null;
      }
      else
      {
        if (Settings.CountryCode == null || Settings.PhoneNumber == null)
          return;
        Settings.RegistrationSmsDetails = string.Join(SMSCodeHelper.SavedSmsDetailsSeparatorString, new string[3]
        {
          Settings.CountryCode,
          Settings.PhoneNumber,
          smsCode
        });
      }
    }

    public static string GetSavedSMSCode()
    {
      string savedSmsCode = (string) null;
      string registrationSmsDetails = Settings.RegistrationSmsDetails;
      if (!string.IsNullOrEmpty(registrationSmsDetails))
      {
        string[] strArray = registrationSmsDetails.Split(SMSCodeHelper.SavedSmsDetailsSeparatorChar);
        if (strArray.Length == 3)
        {
          if (strArray[0] == Settings.CountryCode && strArray[1] == Settings.PhoneNumber)
            savedSmsCode = strArray[2];
          else
            Settings.RegistrationSmsDetails = (string) null;
        }
      }
      return savedSmsCode;
    }
  }
}
