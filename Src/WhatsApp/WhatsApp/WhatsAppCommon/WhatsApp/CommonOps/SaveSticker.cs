// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.SaveSticker
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;


namespace WhatsApp.CommonOps
{
  public static class SaveSticker
  {
    public static void UnsaveSticker(Sticker sticker)
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        sticker.DateTimeStarred = new DateTime?();
        db.UnsaveSticker(sticker);
        db.LocalFileRelease(sticker.LocalFileUri, LocalFileType.Sticker);
        db.SubmitChanges(true);
      }));
    }

    public static Sticker GetExistingSticker(this Message m, MessagesContext db)
    {
      byte[] mediaHash = m.MediaHash;
      Sticker stickerByFileHash = db.GetStickerByFileHash(mediaHash);
      bool flag = false;
      if (stickerByFileHash != null && stickerByFileHash.LocalFileUri != null)
      {
        using (IMediaStorage mediaStorage = MediaStorage.Create(stickerByFileHash.LocalFileUri))
        {
          flag = mediaStorage.FileExists(stickerByFileHash.LocalFileUri);
          if (!flag)
          {
            db.LocalFileRelease(stickerByFileHash.LocalFileUri, LocalFileType.Sticker);
            db.ClearStickerLocalFileUri(stickerByFileHash);
          }
        }
      }
      return !flag ? (Sticker) null : stickerByFileHash;
    }

    public static Sticker GetSavedSticker(this Message m, MessagesContext db)
    {
      Sticker existingSticker = m.GetExistingSticker(db);
      return existingSticker == null || !existingSticker.DateTimeStarred.HasValue ? (Sticker) null : existingSticker;
    }
  }
}
