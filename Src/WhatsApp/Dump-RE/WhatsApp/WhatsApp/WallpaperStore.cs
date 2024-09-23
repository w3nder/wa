// Decompiled with JetBrains decompiler
// Type: WhatsApp.WallpaperStore
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WhatsApp.RegularExpressions;

#nullable disable
namespace WhatsApp
{
  public static class WallpaperStore
  {
    public static Regex WallpaperWithFgPrefRegex = new Regex("\\s*(.+)\\s+#([dlen]+)", RegexOptions.IgnoreCase);
    public const string XapWallpaperDir = "Images/Wallpapers";
    private const string WallpaperDir = "wallpapers";
    private const string GlobalWallpaperIsoPath = "wallpapers/global";
    private static Subject<string> wallpaperChangedSubject_ = new Subject<string>();
    private static WallpaperStore.WallpaperState cachedDefaultWallpaper = (WallpaperStore.WallpaperState) null;

    public static Subject<string> WallpaperChangedSubject
    {
      get => WallpaperStore.wallpaperChangedSubject_;
    }

    private static Size TargetWallpaperSize => ResolutionHelper.GetRenderSize();

    public static WallpaperStore.WallpaperState DefaultWallpaper
    {
      get
      {
        if (WallpaperStore.cachedDefaultWallpaper == null && Settings.GlobalWallpaper != null)
          WallpaperStore.cachedDefaultWallpaper = new WallpaperStore.WallpaperState(Settings.GlobalWallpaper);
        return WallpaperStore.cachedDefaultWallpaper;
      }
    }

    public static WallpaperStore.WallpaperState Get(
      MessagesContext db,
      string jid,
      bool fallbackToDefault)
    {
      WallpaperStore.WallpaperState wallpaperState = (WallpaperStore.WallpaperState) null;
      if (string.IsNullOrEmpty(jid))
        return wallpaperState;
      JidInfo jidInfo = db.GetJidInfo(jid, CreateOptions.None);
      if (jidInfo != null && jidInfo.Wallpaper != null)
        wallpaperState = new WallpaperStore.WallpaperState(jidInfo.Wallpaper);
      if (((wallpaperState == null ? 1 : (!wallpaperState.HasWallpaper ? 1 : 0)) & (fallbackToDefault ? 1 : 0)) != 0)
      {
        wallpaperState = WallpaperStore.DefaultWallpaper;
        if (wallpaperState != null && !wallpaperState.HasWallpaper)
          wallpaperState = (WallpaperStore.WallpaperState) null;
      }
      return wallpaperState;
    }

    private static bool IsBuiltInWallpaper(string filepath)
    {
      return filepath != null && filepath.Contains("Images/Wallpapers");
    }

    public static Color? TryParseColor(string val)
    {
      Color? color = new Color?();
      if (WallpaperStore.IsSolidColorWallpaper(val))
      {
        int startIndex1 = 1;
        int colorHex1 = WallpaperStore.TryParseColorHex(val.Substring(startIndex1, 2));
        int startIndex2 = startIndex1 + 2;
        int colorHex2 = WallpaperStore.TryParseColorHex(val.Substring(startIndex2, 2));
        int startIndex3 = startIndex2 + 2;
        int colorHex3 = WallpaperStore.TryParseColorHex(val.Substring(startIndex3, 2));
        int startIndex4 = startIndex3 + 2;
        int colorHex4 = WallpaperStore.TryParseColorHex(val.Substring(startIndex4, 2));
        if (colorHex1 >= 0 && colorHex2 >= 0 && colorHex3 >= 0 && colorHex4 >= 0)
          color = new Color?(Color.FromArgb((byte) colorHex1, (byte) colorHex2, (byte) colorHex3, (byte) colorHex4));
      }
      return color;
    }

    private static int TryParseColorHex(string hex)
    {
      int result = -1;
      if (!int.TryParse(hex, NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture, out result) || result < 0 || result > (int) byte.MaxValue)
        result = -1;
      return result;
    }

    private static bool IsSolidColorWallpaper(string val)
    {
      return val != null && val.StartsWith("#") && val.Length >= 9;
    }

    private static string GetCustomWallpaperFilepath(string jid)
    {
      return string.Format("{0}/{1}", (object) "wallpapers", (object) jid);
    }

    public static bool Set(MessagesContext db, string jid, Color color)
    {
      string newVal = string.Format("#{0}{1}{2}{3}", (object) color.A.ToString("x2"), (object) color.R.ToString("x2"), (object) color.G.ToString("x2"), (object) color.B.ToString("x2"));
      return WallpaperStore.SetImpl(db, jid, newVal, false);
    }

    public static bool Set(MessagesContext db, string jid, WriteableBitmap bitmap)
    {
      string str = jid == null ? "wallpapers/global" : WallpaperStore.GetCustomWallpaperFilepath(jid);
      return WallpaperStore.SaveWallpaperToFile(bitmap, str) && WallpaperStore.SetImpl(db, jid, str, true);
    }

    public static bool Set(MessagesContext db, string jid, string val)
    {
      return WallpaperStore.SetImpl(db, jid, val, false);
    }

    private static bool SetImpl(MessagesContext db, string jid, string newVal, bool skipDelete)
    {
      JidInfo ji = (JidInfo) null;
      bool dbDirty = false;
      if (!string.IsNullOrEmpty(jid))
      {
        CreateResult result = CreateResult.None;
        ji = db.GetJidInfo(jid, CreateOptions.CreateToDbIfNotFound, out result);
        if ((result & CreateResult.CreatedToDb) != CreateResult.None)
          dbDirty = true;
      }
      if (!skipDelete)
        WallpaperStore.Delete(ji, false, out dbDirty);
      if (string.IsNullOrEmpty(jid))
      {
        Settings.GlobalWallpaper = newVal;
        WallpaperStore.cachedDefaultWallpaper = (WallpaperStore.WallpaperState) null;
      }
      else if (ji != null)
      {
        ji.Wallpaper = newVal;
        dbDirty = true;
      }
      if (dbDirty)
        db.SubmitChanges();
      WallpaperStore.WallpaperChangedSubject.OnNext(jid);
      return true;
    }

    public static void Delete(MessagesContext db, string jid)
    {
      bool dbDirty = false;
      if (string.IsNullOrEmpty(jid))
      {
        WallpaperStore.Delete((JidInfo) null, true, out dbDirty);
      }
      else
      {
        JidInfo jidInfo = db.GetJidInfo(jid, CreateOptions.None);
        if (jidInfo != null)
          WallpaperStore.Delete(jidInfo, true, out dbDirty);
      }
      if (!dbDirty)
        return;
      db.SubmitChanges();
    }

    private static void Delete(JidInfo ji, bool notify, out bool dbDirty)
    {
      string str = ji == null ? Settings.GlobalWallpaper : ji.Wallpaper ?? WallpaperStore.GetCustomWallpaperFilepath(ji.Jid);
      dbDirty = false;
      try
      {
        using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
        {
          if (!string.IsNullOrEmpty(str))
          {
            if (storeForApplication.FileExists(str))
              storeForApplication.DeleteFile(str);
          }
        }
      }
      catch (Exception ex)
      {
        string context = string.Format("delete background for {0}", ji == null ? (object) "global" : (object) ji.Jid);
        Log.LogException(ex, context);
      }
      if (ji == null)
      {
        Settings.GlobalWallpaper = (string) null;
        WallpaperStore.cachedDefaultWallpaper = (WallpaperStore.WallpaperState) null;
      }
      else
      {
        ji.Wallpaper = (string) null;
        dbDirty = true;
      }
      if (!notify)
        return;
      WallpaperStore.WallpaperChangedSubject.OnNext(ji == null ? (string) null : ji.Jid);
    }

    private static bool SaveWallpaperToFile(WriteableBitmap bitmap, string filepath)
    {
      try
      {
        Size targetWallpaperSize = WallpaperStore.TargetWallpaperSize;
        using (MemoryStream targetStream = new MemoryStream())
        {
          bitmap.SaveJpeg((Stream) targetStream, (int) targetWallpaperSize.Width, (int) targetWallpaperSize.Height, 0, 90);
          targetStream.Position = 0L;
          using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
          {
            if (!storeForApplication.DirectoryExists("wallpapers"))
              storeForApplication.CreateDirectory("wallpapers");
            using (IsolatedStorageFileStream destination = storeForApplication.OpenFile(filepath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete))
              targetStream.CopyTo((Stream) destination);
          }
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "save wallpaper to file");
        return false;
      }
      return true;
    }

    private static BitmapSource LoadWallpaperFromFile(IsolatedStorageFile store, string filepath)
    {
      if (WallpaperStore.IsBuiltInWallpaper(filepath))
        return WallpaperStore.LoadWallpaperFromXapFile(filepath);
      if (filepath == null || !store.FileExists(filepath))
        return (BitmapSource) null;
      using (IsolatedStorageFileStream streamSource = store.OpenFile(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
      {
        BitmapImage bitmapImage = new BitmapImage();
        bitmapImage.CreateOptions = BitmapCreateOptions.None;
        bitmapImage.SetSource((Stream) streamSource);
        return (BitmapSource) bitmapImage;
      }
    }

    private static BitmapSource LoadWallpaperFromXapFile(string filepath)
    {
      if (!WallpaperStore.IsBuiltInWallpaper(filepath))
        return (BitmapSource) null;
      BitmapSource bitmapSource = (BitmapSource) null;
      try
      {
        Stream streamSource = AppState.OpenFromXAP(filepath);
        if (streamSource != null)
        {
          using (streamSource)
          {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.CreateOptions = BitmapCreateOptions.None;
            bitmapImage.SetSource(streamSource);
            bitmapSource = (BitmapSource) bitmapImage;
          }
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "load wallpaper from xap");
        bitmapSource = (BitmapSource) null;
      }
      return bitmapSource;
    }

    public class WallpaperState
    {
      private string filepath;
      private WeakReference<System.Windows.Media.ImageSource> cached;
      private Color? solidColor;
      private bool? needsFgProtection;
      private WallpaperStore.WallpaperState.ForegroundPreferences? fgPref;
      private string wallpaperVal;

      public System.Windows.Media.ImageSource Image
      {
        get
        {
          System.Windows.Media.ImageSource target = (System.Windows.Media.ImageSource) null;
          if (this.cached == null || !this.cached.TryGetTarget(out target))
          {
            if (!WallpaperStore.IsSolidColorWallpaper(this.wallpaperVal))
            {
              try
              {
                if (this.filepath == null)
                  this.Parse();
                if (this.filepath != null)
                {
                  using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
                    target = (System.Windows.Media.ImageSource) WallpaperStore.LoadWallpaperFromFile(storeForApplication, this.filepath);
                }
              }
              catch (Exception ex)
              {
                Log.LogException(ex, "load wallpaper");
                target = (System.Windows.Media.ImageSource) null;
              }
              this.Image = target;
            }
          }
          return target;
        }
        set
        {
          if (this.cached == null)
          {
            if (value == null)
              return;
            this.cached = new WeakReference<System.Windows.Media.ImageSource>(value);
          }
          else
            this.cached.SetTarget(value);
        }
      }

      public Color? SolidColor
      {
        get
        {
          return this.solidColor ?? (this.solidColor = WallpaperStore.TryParseColor(this.wallpaperVal));
        }
        private set => this.solidColor = value;
      }

      public bool HasWallpaper
      {
        get
        {
          if (this.wallpaperVal == null)
            return false;
          return this.SolidColor.HasValue || this.Image != null;
        }
      }

      public bool NeedsForegroundProtection
      {
        get
        {
          if (this.SolidColor.HasValue)
            return false;
          if (!this.needsFgProtection.HasValue)
            this.Parse();
          return this.needsFgProtection ?? true;
        }
      }

      public WallpaperStore.WallpaperState.ForegroundPreferences ForegroundPreference
      {
        get
        {
          if (!this.fgPref.HasValue)
            this.Parse();
          return this.fgPref ?? WallpaperStore.WallpaperState.ForegroundPreferences.LightOnly;
        }
      }

      public SolidColorBrush PreferredForegroundBrush
      {
        get => new SolidColorBrush(this.GetPreferredForegroundColor());
      }

      public WallpaperState(string wallpaper) => this.wallpaperVal = wallpaper;

      public string Get() => this.wallpaperVal;

      private void Parse()
      {
        WhatsApp.RegularExpressions.Match match = WallpaperStore.WallpaperWithFgPrefRegex.Match(this.wallpaperVal ?? "");
        if (match.Success)
        {
          string val = match.Groups[1].Value;
          if (!WallpaperStore.TryParseColor(val).HasValue)
            this.filepath = val;
          string str = match.Groups[2].Value;
          if (!string.IsNullOrEmpty(str))
          {
            foreach (char ch in str)
            {
              switch (ch)
              {
                case 'd':
                  this.fgPref = new WallpaperStore.WallpaperState.ForegroundPreferences?(WallpaperStore.WallpaperState.ForegroundPreferences.DarkOnly);
                  break;
                case 'e':
                  this.fgPref = new WallpaperStore.WallpaperState.ForegroundPreferences?(WallpaperStore.WallpaperState.ForegroundPreferences.Either);
                  break;
                case 'l':
                  this.fgPref = new WallpaperStore.WallpaperState.ForegroundPreferences?(WallpaperStore.WallpaperState.ForegroundPreferences.LightOnly);
                  break;
                case 'n':
                  this.needsFgProtection = new bool?(false);
                  break;
              }
            }
          }
        }
        if (!this.fgPref.HasValue)
          this.fgPref = new WallpaperStore.WallpaperState.ForegroundPreferences?(WallpaperStore.WallpaperState.ForegroundPreferences.LightOnly);
        if (!this.needsFgProtection.HasValue)
          this.needsFgProtection = new bool?(true);
        if (this.filepath != null)
          return;
        this.filepath = this.wallpaperVal;
      }

      public Color GetPreferredForegroundColor(bool forSysTray = false)
      {
        switch (this.ForegroundPreference)
        {
          case WallpaperStore.WallpaperState.ForegroundPreferences.LightOnly:
            return !forSysTray ? Constants.SysTrayOffWhite : Colors.White;
          case WallpaperStore.WallpaperState.ForegroundPreferences.DarkOnly:
            return Colors.Black;
          default:
            return UIUtils.ForegroundBrush.Color;
        }
      }

      public enum ForegroundPreferences
      {
        LightOnly,
        DarkOnly,
        Either,
      }
    }
  }
}
