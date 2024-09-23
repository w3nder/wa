// Decompiled with JetBrains decompiler
// Type: WhatsApp.DocumentMessageUtils
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

#nullable disable
namespace WhatsApp
{
  public static class DocumentMessageUtils
  {
    public static string GetMimeTypeFromExtension(string ext)
    {
      string typeFromExtension = (string) null;
      switch (ext)
      {
        case "doc":
          typeFromExtension = "application/msword";
          break;
        case "docx":
          typeFromExtension = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
          break;
        case "pdf":
          typeFromExtension = "application/pdf";
          break;
        case "ppt":
          typeFromExtension = "application/vnd.ms-powerpoint";
          break;
        case "pptx":
          typeFromExtension = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
          break;
        case "txt":
          typeFromExtension = "text/plain";
          break;
        case "xls":
          typeFromExtension = "application/vnd.ms-excel";
          break;
        case "xlsx":
          typeFromExtension = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
          break;
      }
      return typeFromExtension;
    }

    public static string GetUnsupportedErrorMessage(string targetJid, string[] unsupportedUserJids)
    {
      string unsupportedErrorMessage;
      if (JidHelper.IsUserJid(targetJid))
      {
        unsupportedErrorMessage = string.Format(AppResources.DocumentNotSupportedIndividual, (object) JidHelper.GetDisplayNameForContactJid(targetJid));
      }
      else
      {
        List<UserStatus> list = ((IEnumerable<string>) (unsupportedUserJids ?? new string[0])).Select<string, UserStatus>((Func<string, UserStatus>) (ujid => UserCache.Get(ujid, false))).Where<UserStatus>((Func<UserStatus, bool>) (u => u != null)).ToList<UserStatus>();
        ParticipantSort.Sort(list, true, false);
        string[] array = list.Select<UserStatus, string>((Func<UserStatus, string>) (u => u.GetDisplayName(true))).Where<string>((Func<string, bool>) (name => name != null)).ToArray<string>();
        if (!((IEnumerable<string>) array).Any<string>())
          unsupportedErrorMessage = "";
        else if (array.Length == 1)
          unsupportedErrorMessage = string.Format(AppResources.DocumentNotSupportedGroup1, (object) array[0]);
        else if (array.Length == 2)
          unsupportedErrorMessage = string.Format(AppResources.DocumentNotSupportedGroup2, (object) array[0], (object) array[1]);
        else if (array.Length == 3)
          unsupportedErrorMessage = string.Format(AppResources.DocumentNotSupportedGroup3, (object) array[0], (object) array[1], (object) array[2]);
        else
          unsupportedErrorMessage = Plurals.Instance.GetStringWithIndex(AppResources.DocumentNotSupportedGroupNPlural, 2, (object) array[0], (object) array[1], (object) (unsupportedUserJids.Length - 2));
      }
      return unsupportedErrorMessage;
    }

    public class DocumentData
    {
      public string MimeType { get; set; }

      public string Title { get; set; }

      public string FileExtension { get; set; }

      public WriteableBitmap Thumbnail { get; set; }

      public Stream Stream { get; set; }

      public int PageCount { get; set; }

      public string Filename { get; set; }
    }
  }
}
