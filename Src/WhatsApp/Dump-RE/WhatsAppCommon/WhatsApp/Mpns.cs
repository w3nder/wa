// Decompiled with JetBrains decompiler
// Type: WhatsApp.Mpns
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace WhatsApp
{
  public class Mpns : IPushSystem
  {
    public void OnAppReset()
    {
    }

    public void OnPushRegistered()
    {
    }

    private static string FilterContent(string str, int charLimit = 64)
    {
      if (str.Length > charLimit)
        str = Utils.TruncateAtIndex(str, charLimit - 1) + "…";
      return str;
    }

    public void ShellToastEx(
      string[] content,
      string group,
      string uri,
      bool muted,
      string tone,
      string tag)
    {
      string str1 = Mpns.FilterContent(content[0]);
      string str2 = content.Length > 1 ? Mpns.FilterContent(content[1]) : "";
      ShellToast shellToast = new ShellToast()
      {
        Title = str1,
        Content = str2,
        NavigationUri = new Uri(uri, UriKind.Relative)
      };
      if (tone != null)
        shellToast.GetType().GetProperty("Sound").SetMethod.Invoke((object) shellToast, new object[1]
        {
          (object) new Uri(tone.Replace('\\', '/'), UriKind.Relative)
        });
      shellToast.Show();
    }

    public void ClearToastHistoryGroup(string group)
    {
      Log.l("MPNS", "Clear toast group | group={0}", (object) group);
    }

    public void ClearToastHistoryMessage(string tag, string group)
    {
      Log.l("MPNS", "Clear toast | tag={0}, group={1}", (object) tag, (object) group);
    }

    public string PushState
    {
      get
      {
        string pushState = (string) null;
        if (!AppState.IsBackgroundAgent)
          pushState = PushSystem.ForegroundInstance.PushState;
        return pushState;
      }
    }

    public ITile PrimaryTile
    {
      get
      {
        ShellTile nativeTile;
        try
        {
          nativeTile = ShellTile.ActiveTiles.FirstOrDefault<ShellTile>();
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "shell tile enumerate");
          return (ITile) null;
        }
        if (nativeTile != null)
          return (ITile) new Mpns.PrimaryTileWrapper(nativeTile);
        Log.l("MPNS", "Unexpected: primary tile is null");
        return (ITile) null;
      }
    }

    private ShellTile GetSecondaryTileImpl(Func<Uri, bool> selector, string key)
    {
      try
      {
        return ShellTile.ActiveTiles.FirstOrDefault<ShellTile>((Func<ShellTile, bool>) (t => selector(t.NavigationUri)));
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "get chat tile");
        return (ShellTile) null;
      }
    }

    public bool SecondaryTileExists()
    {
      try
      {
        return ShellTile.ActiveTiles.Count<ShellTile>() > 1;
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "shell tile enumerate");
        return false;
      }
    }

    public bool SecondaryTileExists(Func<Uri, bool> selector, string key)
    {
      return this.GetSecondaryTileImpl(selector, key) != null;
    }

    public ITile GetSecondaryTile(Func<Uri, bool> selector, string key)
    {
      ShellTile secondaryTileImpl = this.GetSecondaryTileImpl(selector, key);
      return secondaryTileImpl != null ? (ITile) new Mpns.SecondaryTileWrapper(secondaryTileImpl) : (ITile) null;
    }

    public Dictionary<string, string> ClientConfig
    {
      get
      {
        Dictionary<string, string> clientConfig = new Dictionary<string, string>();
        string lang;
        string locale;
        AppState.GetLangAndLocale(out lang, out locale);
        string str = AppState.IsVoipScheduled() ? "3" : "2";
        string[] strArray = new string[16]
        {
          "platform",
          "microsoft",
          "lg",
          lang,
          "lc",
          locale,
          "clear",
          "0",
          "preview",
          Settings.PreviewEnabled ? "1" : "0",
          "default",
          "1",
          "groups",
          "1",
          "version",
          str
        };
        for (int index = 0; index + 1 < strArray.Length; index += 2)
          clientConfig[strArray[index]] = strArray[index + 1];
        return clientConfig;
      }
    }

    private class PrimaryTileWrapper : ITile
    {
      private IconicTileData tileData = new IconicTileData();
      private ShellTile nativeTile;

      public PrimaryTileWrapper(ShellTile nativeTile) => this.nativeTile = nativeTile;

      public void Clear()
      {
        this.tileData.Count = new int?(0);
        this.tileData.WideContent1 = this.tileData.WideContent2 = this.tileData.WideContent3 = "";
      }

      public void SetWideContent(IEnumerable<string> strs)
      {
        strs = PushSystem.SanitizeWideContent(strs).Select<string, string>((Func<string, string>) (s => s.EscapeForTile()));
        this.tileData.SetWideContent(strs);
      }

      public void SetCount(int? count) => this.tileData.Count = count;

      public void Update()
      {
        try
        {
          this.nativeTile.Update((ShellTileData) this.tileData);
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "shell tile update");
        }
      }

      public void SetTitle(string title, bool back) => throw new NotImplementedException();

      public void SetBackgroundImage(Uri uri) => throw new NotImplementedException();

      public void SetSmallBackgroundImage(Uri uri) => throw new NotImplementedException();

      public void Delete() => throw new NotImplementedException();
    }

    private class SecondaryTileWrapper : ITile
    {
      private FlipTileData tileData = new FlipTileData();
      private ShellTile nativeTile;

      public SecondaryTileWrapper(ShellTile nativeTile) => this.nativeTile = nativeTile;

      public void Clear() => this.tileData.Count = new int?(0);

      public void SetWideContent(IEnumerable<string> strs)
      {
        strs = PushSystem.SanitizeWideContent(strs).Select<string, string>((Func<string, string>) (s => s.EscapeForTile()));
        this.tileData.BackContent = string.Join("\n", strs.ToArray<string>());
      }

      public void SetCount(int? count) => this.tileData.Count = count;

      public void SetTitle(string title, bool back)
      {
        title = title.EscapeForTile();
        if (back)
          this.tileData.BackTitle = title;
        else
          this.tileData.Title = title;
      }

      public void SetBackgroundImage(Uri uri) => this.tileData.BackgroundImage = uri;

      public void SetSmallBackgroundImage(Uri uri) => this.tileData.SmallBackgroundImage = uri;

      public void Update()
      {
        try
        {
          this.nativeTile.Update((ShellTileData) this.tileData);
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "shell tile update");
        }
      }

      public void Delete() => this.nativeTile.Delete();
    }
  }
}
