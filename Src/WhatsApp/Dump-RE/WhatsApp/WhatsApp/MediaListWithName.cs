// Decompiled with JetBrains decompiler
// Type: WhatsApp.MediaListWithName
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class MediaListWithName : IDisposable
  {
    private const int albumThumbnailSearchLimit = 20;
    private static bool sentClbForThumbnailFail;

    public string HeaderName { get; private set; }

    public string TileName { get; private set; }

    public IMediaItem Item { get; private set; }

    public IMediaList List { get; private set; }

    public bool MostRecentAsFirst { get; set; }

    public string Guid { get; set; }

    public byte[] GetThumbnail()
    {
      int itemCount = this.List.GetItemCount();
      if (itemCount <= 0)
        return (byte[]) null;
      uint num1 = 0;
      int num2 = 0;
      int num3 = Math.Min(19, itemCount - 1);
      int Index = num2;
      int num4 = 1;
      if (this.MostRecentAsFirst)
      {
        num2 = Math.Max(0, itemCount - 20);
        num3 = itemCount - 1;
        Index = num3;
        num4 = -1;
      }
      for (; Index >= num2; Index += num4)
      {
        if (Index <= num3)
        {
          try
          {
            return this.List.GetItem(Index).GetThumbnail().Get();
          }
          catch (Exception ex)
          {
            if ((int) num1 == (int) ex.GetHResult())
            {
              Log.l("get thumbnail for media list | {0}, ex: '{1}' ", (object) Index, (object) ex.GetFriendlyMessage());
            }
            else
            {
              num1 = ex.GetHResult();
              Log.LogException(ex, string.Format("get thumbnail for media list | {0}", (object) Index));
            }
          }
        }
        else
          break;
      }
      return (byte[]) null;
    }

    public void Dispose()
    {
      if (this.Item != null)
      {
        this.Item.Dispose();
        this.Item = (IMediaItem) null;
      }
      if (this.List == null)
        return;
      this.List.Dispose();
      this.List = (IMediaList) null;
    }

    public MediaListWithName(
      IMediaItem item,
      string headerName,
      string tileName,
      MediaItemTypes? filter)
    {
      this.Item = item;
      this.HeaderName = headerName;
      this.TileName = tileName;
      this.List = this.Item.GetChildren(MediaListTypes.FolderPictureContents);
      this.MostRecentAsFirst = true;
      if (!filter.HasValue)
        return;
      this.List.FilterItems(filter.Value);
    }
  }
}
