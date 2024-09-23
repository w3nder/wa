// Decompiled with JetBrains decompiler
// Type: System.Net.Mail.SR
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Globalization;
using System.Net.Http;
using System.Resources;

#nullable disable
namespace System.Net.Mail
{
  internal static class SR
  {
    public const string InvalidHeaderName = "InvalidHeaderName";
    public const string MailAddressInvalidFormat = "MailAddressInvalidFormat";
    public const string MailHeaderFieldInvalidCharacter = "MailHeaderFieldInvalidCharacter";
    public const string MailHeaderFieldMalformedHeader = "MailHeaderFieldMalformedHeader";

    public static string GetString(string id, params object[] args)
    {
      ResourceManager resourceManager = SysSR.ResourceManager;
      if (resourceManager == null)
        return (string) null;
      string format = resourceManager.GetString(id, SysSR.Culture);
      if (args == null || args.Length <= 0)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is string str && str.Length > 1024)
          args[index] = (object) (str.Substring(0, 1021) + "...");
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }
  }
}
