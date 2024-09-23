// Decompiled with JetBrains decompiler
// Type: WhatsApp.MediaLibraryExtensions
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using WhatsAppNative;


namespace WhatsApp
{
  public static class MediaLibraryExtensions
  {
    private static Dictionary<string, string> FilteredCustomAlbums = ((IEnumerable<string>) new string[4]
    {
      "Camera Roll",
      "Sample Pictures",
      "Saved Pictures",
      "Screenshots"
    }).Select<string, string>((Func<string, string>) (_ => _.ToLower())).ToDictionary<string, string>((Func<string, string>) (_ => _));

    public static void Using(this IMediaList d, Action<IMediaList> a)
    {
      ((Action) (() => a(d))).Finally(new Action(d.Dispose));
    }

    public static void Using(this IMediaItem d, Action<IMediaItem> a)
    {
      ((Action) (() => a(d))).Finally(new Action(d.Dispose));
    }

    public static IMediaList CreateList(this IMediaLibrary lib, MediaListTypes type)
    {
      return lib.CreateList(type, 0U);
    }

    private static bool GetFilteredNames(
      string name,
      out string tileName,
      out string headerName,
      out string guid)
    {
      string lowerInvariant = name.Trim().ToLowerInvariant();
      bool filteredNames = true;
      guid = lowerInvariant;
      switch (lowerInvariant)
      {
        case "{9ae241c6-e6cc-4080-a2ba-245e0f7c47c5}":
          tileName = AppResources.AlbumTileCameraRoll;
          headerName = AppResources.AlbumHeaderCameraRoll;
          break;
        case "{9ae241c6-e6cc-4080-a2ba-245e0f7c47c6}":
          tileName = headerName = AppResources.FavoritesHeader;
          break;
        case "{1e544589-04c4-492f-87ca-294a52149279}":
          tileName = AppResources.AlbumTileSavedPictures;
          headerName = AppResources.AlbumHeaderSavedPictures;
          break;
        case "{4915925e-fb2a-11de-ae1c-dd6355d89593}":
          tileName = headerName = "8";
          break;
        case "{c9ab957d-a9c1-48f3-b0bd-ce27fab279e7}":
          tileName = AppResources.AlbumTileScreenshots;
          headerName = AppResources.AlbumHeaderScreenshots;
          break;
        default:
          filteredNames = false;
          tileName = headerName = guid = (string) null;
          break;
      }
      return filteredNames;
    }

    private static int GetSpecialSortIndex(string guid)
    {
      return Array.IndexOf<string>(MediaLibrarySpecialFolders.SortOrder, guid);
    }

    private static int CompareAlbums(MediaListWithName a, MediaListWithName b)
    {
      return a.Guid != null ? (b.Guid == null ? -1 : MediaLibraryExtensions.GetSpecialSortIndex(a.Guid).CompareTo(MediaLibraryExtensions.GetSpecialSortIndex(b.Guid))) : (b.Guid != null ? -MediaLibraryExtensions.CompareAlbums(b, a) : a.TileName.CompareTo(b.TileName));
    }

    public static IEnumerable<MediaListWithName> GetAlbums(
      this IMediaLibrary lib,
      MediaItemTypes? filter = null)
    {
      List<MediaListWithName> list = ((IEnumerable<MediaListWithName>) new MediaListWithName[0]).Concat<MediaListWithName>(MediaLibraryExtensions.GetAlbumsNewStyle(lib, filter).Where<MediaListWithName>((Func<MediaListWithName, bool>) (mediaList =>
      {
        int num = mediaList.Guid != null || mediaList.TileName == AppResources.AlbumTitleOtherPictures ? 1 : (mediaList.List.GetItemCount() != 0 ? 1 : 0);
        if (num != 0)
          return num != 0;
        mediaList.Dispose();
        return num != 0;
      }))).ToList<MediaListWithName>();
      list.Sort(new Comparison<MediaListWithName>(MediaLibraryExtensions.CompareAlbums));
      return (IEnumerable<MediaListWithName>) list;
    }

    private static IEnumerable<MediaListWithName> GetAlbumSubItems(
      IMediaList list,
      MediaItemTypes? filter)
    {
      int i = 0;
      for (int count = list.GetItemCount(); i < count; ++i)
      {
        IMediaItem item = list.GetItem(i);
        try
        {
          string stringAttribute = item.GetStringAttribute(MediaItemStrings.Name);
          bool flag = false;
          string tileName;
          string headerName;
          string guid;
          MediaLibraryExtensions.GetFilteredNames(stringAttribute, out tileName, out headerName, out guid);
          switch (guid)
          {
            case "{4915925e-fb2a-11de-ae1c-dd6355d89593}":
              continue;
            case "{1e544589-04c4-492f-87ca-294a52149279}":
              flag = true;
              goto default;
            default:
              MediaListWithName albumSubItem = new MediaListWithName(item, headerName ?? stringAttribute, tileName ?? stringAttribute, filter)
              {
                Guid = guid
              };
              item = (IMediaItem) null;
              if (flag && albumSubItem.List.GetItemCount() == 0)
              {
                albumSubItem.Dispose();
                goto case "{4915925e-fb2a-11de-ae1c-dd6355d89593}";
              }
              else
              {
                yield return albumSubItem;
                break;
              }
          }
        }
        finally
        {
          item?.Dispose();
        }
        item = (IMediaItem) null;
      }
    }

    private static IEnumerable<MediaListWithName> GetCustomAlbums(
      IMediaLibrary lib,
      MediaItemTypes? filter)
    {
      IMediaList list = lib.CreateList(MediaListTypes.AllFolders);
      List<IMediaList> sublists = new List<IMediaList>();
      try
      {
        int Index = 0;
        for (int itemCount = list.GetItemCount(); Index < itemCount; ++Index)
        {
          IMediaItem mediaItem = list.GetItem(Index);
          try
          {
            if (mediaItem.GetStringAttribute(MediaItemStrings.Name) == "Pictures")
              sublists.Add(mediaItem.GetChildren(MediaListTypes.Folders));
          }
          finally
          {
            mediaItem?.Dispose();
          }
        }
        foreach (IMediaList list2 in sublists)
        {
          int i = 0;
          for (int count = list2.GetItemCount(); i < count; ++i)
          {
            IMediaItem item = list2.GetItem(i);
            try
            {
              string stringAttribute = item.GetStringAttribute(MediaItemStrings.Name);
              if (!MediaLibraryExtensions.FilteredCustomAlbums.ContainsKey(stringAttribute.Trim().ToLower()))
              {
                MediaListWithName customAlbum = new MediaListWithName(item, stringAttribute, stringAttribute, filter);
                item = (IMediaItem) null;
                yield return customAlbum;
              }
              else
                continue;
            }
            finally
            {
              item?.Dispose();
            }
            item = (IMediaItem) null;
          }
        }
      }
      finally
      {
        if (list != null)
        {
          list.Dispose();
          list = (IMediaList) null;
        }
        sublists.ForEach((Action<IMediaList>) (l => l.Dispose()));
        sublists.Clear();
      }
    }

    private static IEnumerable<MediaListWithName> GetAlbumsNewStyle(
      IMediaLibrary lib,
      MediaItemTypes? filter)
    {
      uint ZMEDIAFOLDER_TYPE_PICTURES_ROOT = 1;
      IMediaItem root = (IMediaItem) null;
      IMediaList list = (IMediaList) null;
      try
      {
        root = lib.GetSpecialFolder(ZMEDIAFOLDER_TYPE_PICTURES_ROOT);
        list = lib.CreateList(MediaListTypes.AllFolders, root.GetId());
        foreach (MediaListWithName albumSubItem in MediaLibraryExtensions.GetAlbumSubItems(list, filter))
          yield return albumSubItem;
      }
      finally
      {
        list?.Dispose();
        root?.Dispose();
      }
      yield return new MediaListWithName(lib.GetSpecialFolder(ZMEDIAFOLDER_TYPE_PICTURES_ROOT), AppResources.AlbumHeaderOtherPictures, AppResources.AlbumTitleOtherPictures, filter);
    }
  }
}
