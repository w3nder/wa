// Decompiled with JetBrains decompiler
// Type: WhatsApp.TwoFactorAuthentication
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;


namespace WhatsApp
{
  public class TwoFactorAuthentication
  {
    private static readonly double[] BackoffTable = new double[6]
    {
      0.25,
      0.5,
      1.0,
      1.0,
      3.0,
      7.0
    };
    public static bool NaggedForCodeEntry = false;

    public static void SendSetupCode(
      string code,
      string email,
      Action<bool> onSuccess,
      Action<int> onError)
    {
      AppState.GetConnection().SendSetSecurityCode(code, email, (Action) (() =>
      {
        bool factorPromptReset = Settings.TwoFactorPromptReset;
        Settings.TwoFactorAuthEnabled = true;
        Settings.TwoFactorNextPrompt = new DateTime?(FunRunner.CurrentServerTimeUtc.AddDays(TwoFactorAuthentication.BackoffTable[0]));
        Settings.TwoFactorPromptReset = false;
        Settings.TwoFactorPromptBackoff = 0;
        Settings.TwoFactorAuthCodeLocal = code;
        Settings.TwoFactorAuthEmail = email;
        onSuccess(factorPromptReset);
      }), (Action<int>) (err => onError(err)));
    }

    public static void RemoveSetupCode(Action<bool> onSuccess, Action<int> onError)
    {
      AppState.GetConnection().SendDeleteSecurityCode((Action) (() =>
      {
        bool factorPromptReset = Settings.TwoFactorPromptReset;
        Settings.TwoFactorAuthEnabled = false;
        Settings.TwoFactorPromptReset = false;
        Settings.TwoFactorPromptBackoff = 0;
        Settings.TwoFactorAuthCodeLocal = string.Empty;
        Settings.TwoFactorAuthEmail = string.Empty;
        onSuccess(factorPromptReset);
      }), (Action<int>) (err => onError(err)));
    }

    public static bool ShouldPromptCodeEntry
    {
      get
      {
        if (!Settings.TwoFactorAuthEnabled || TwoFactorAuthentication.NaggedForCodeEntry)
          return false;
        return !Settings.TwoFactorNextPrompt.HasValue || Settings.TwoFactorNextPrompt.Value < FunRunner.CurrentServerTimeUtc;
      }
    }

    public static void ValidateCode(string code, Action<bool> onSuccess, Action<int> onError = null)
    {
      AppState.GetConnection().SendGetValidateSecurityCode(code, (Action<bool, bool>) ((correctCode, hasEmail) =>
      {
        if (!correctCode)
          Settings.TwoFactorPromptReset = true;
        onSuccess(correctCode);
      }), (Action<int>) (err =>
      {
        if (onError == null)
          return;
        onError(err);
      }));
    }

    public static void CodeValidated(string code)
    {
      Settings.TwoFactorAuthEnabled = true;
      Settings.TwoFactorPromptReset = false;
      Settings.TwoFactorAuthCodeLocal = code;
      Settings.TwoFactorAuthEmail = string.Empty;
      int index = TwoFactorAuthentication.BackoffTable.Length - 1;
      Settings.TwoFactorPromptBackoff = index;
      Settings.TwoFactorNextPrompt = new DateTime?(FunRunner.CurrentServerTimeUtc.AddDays(TwoFactorAuthentication.BackoffTable[index]));
      Settings.TwoFactorWipePollInterval = 0;
      Settings.TwoFactorEmailWipePollExpiryTime = new DateTime?(DateTime.UtcNow);
    }

    public static void CodeRemoved()
    {
      Settings.TwoFactorAuthEnabled = false;
      Settings.TwoFactorPromptReset = false;
      Settings.TwoFactorPromptBackoff = 0;
      Settings.TwoFactorAuthCodeLocal = string.Empty;
      Settings.TwoFactorAuthEmail = string.Empty;
      Settings.TwoFactorWipePollInterval = 5;
      Settings.TwoFactorEmailWipePollExpiryTime = new DateTime?(DateTime.UtcNow);
    }

    public static TimeSpan GetNextBackoff()
    {
      int factorPromptBackoff = Settings.TwoFactorPromptBackoff;
      Settings.TwoFactorPromptBackoff = Math.Min(TwoFactorAuthentication.BackoffTable.Length - 1, factorPromptBackoff + 1);
      return TimeSpan.FromDays(TwoFactorAuthentication.BackoffTable[factorPromptBackoff]);
    }

    public static void DecrementBackoff()
    {
      Settings.TwoFactorPromptBackoff = Math.Max(0, Settings.TwoFactorPromptBackoff - 1);
    }
  }
}
