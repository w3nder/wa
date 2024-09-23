// Decompiled with JetBrains decompiler
// Type: WhatsApp.AssetStore
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#nullable disable
namespace WhatsApp
{
  public class AssetStore
  {
    private const string LogHdr = "AssetStore";
    private static BitmapSource inlineClock;
    private static BitmapSource inlineError;
    private static BitmapSource inlineCheck;
    private static BitmapSource inlineDoubleChecks;
    private static BitmapSource inlineMic;
    private static BitmapSource inlineWhiteClock;
    private static BitmapSource inlineWhiteError;
    private static BitmapSource inlineWhiteCheck;
    private static BitmapSource inlineWhiteDoubleChecks;
    private static BitmapSource inlineWhiteMic;
    private static BitmapSource inlineSolidWhiteClock;
    private static BitmapSource inlineSolidWhiteError;
    private static BitmapSource inlineSolidWhiteCheck;
    private static BitmapSource inlineSolidWhiteDoubleChecks;
    private static BitmapSource inlineAccentClock;
    private static BitmapSource inlineAccentError;
    private static BitmapSource inlineAccentCheck;
    private static BitmapSource inlineAccentDoubleChecks;
    private static BitmapSource inlineBlueChecks;
    private static BitmapSource inlineContactWhite;
    private static BitmapSource inlineContactSubtle;
    private static BitmapSource inlineContactAccent;
    private static BitmapSource inlineContact;
    private static BitmapSource inlineMicWhite;
    private static BitmapSource inlineMicSubtle;
    private static BitmapSource inlineMicBlue;
    private static BitmapSource inlineMicGreen;
    private static BitmapSource inlineAudioWhite;
    private static BitmapSource inlineAudioSubtle;
    private static BitmapSource inlineAudioAccent;
    private static BitmapSource inlineDocWhite;
    private static BitmapSource inlineDocSubtle;
    private static BitmapSource inlineDocAccent;
    private static BitmapSource inlineDoc;
    private static BitmapSource inlineLiveLocationWhite;
    private static BitmapSource inlineLiveLocationSubtle;
    private static BitmapSource inlineLiveLocationAccent;
    private static BitmapSource inlineLiveLocation;
    private static BitmapSource liveLocationWhite;
    private static BitmapSource liveLocationSubtle;
    private static BitmapSource liveLocationAccent;
    private static BitmapSource liveLocation;
    private static BitmapSource inlineLocationWhite;
    private static BitmapSource inlineLocationSubtle;
    private static BitmapSource inlineLocationAccent;
    private static BitmapSource inlineLocation;
    private static BitmapSource inlineRevokeWhite;
    private static BitmapSource inlineRevokeSubtle;
    private static BitmapSource inlineRevokeAccent;
    private static BitmapSource inlineRevoke;
    private static BitmapSource whatsappAvatar;
    private static BitmapSource inlinePictureWhite;
    private static BitmapSource inlineGifWhite;
    private static BitmapSource inlineStickerWhite;
    private static BitmapSource inlineVerified;
    private static BitmapSource inlinePictureSubtle;
    private static BitmapSource inlinePictureAccent;
    private static BitmapSource inlineGifSubtle;
    private static BitmapSource inlineGifAccent;
    private static BitmapSource inlineStickerSubtle;
    private static BitmapSource inlineStickerAccent;
    private static BitmapSource inlinePicture;
    private static BitmapSource inlineVideoWhite;
    private static BitmapSource inlineVideoSubtle;
    private static BitmapSource inlineVideoAccent;
    private static BitmapSource inlineVideo;
    private static BitmapSource inlineMuteWhite;
    private static BitmapSource inlineMuteSubtle;
    private static BitmapSource inlineMuteAccent;
    private static BitmapSource inlinePinWhite;
    private static BitmapSource inlinePinSubtle;
    private static BitmapSource inlinePinAccent;
    private static BitmapSource inlineWhiteBroadcast;
    private static BitmapSource inlineAccentBroadcast;
    private static BitmapSource broadcast;
    private static BitmapSource defaultGroupIcon;
    private static BitmapSource defaultContactIcon;
    private static BitmapSource defaultContactIconLarge;
    private static BitmapSource defaultContactIconBlack;
    private static BitmapSource poweredByBingIcon;
    private static BitmapSource slideToCancelMaskDefault;
    private static BitmapSource slideToCancelMaskChrome;
    private static BitmapSource forwardArrow;
    private static BitmapSource locationButtonIcon;
    private static BitmapSource camcorderButtonIcon;
    private static BitmapSource cameraButtonIcon;
    private static BitmapSource pictureButtonIcon;
    private static BitmapSource videoButtonIcon;
    private static BitmapSource audioButtonIcon;
    private static BitmapSource contactButtonIcon;
    private static BitmapSource docButtonIcon;
    private static BitmapSource emojiRecentButtonIconDark;
    private static BitmapSource emojiRecentButtonIcon;
    private static BitmapSource emojiPeopleButtonIconDark;
    private static BitmapSource emojiPeopleButtonIcon;
    private static BitmapSource emojiNatureButtonIconDark;
    private static BitmapSource emojiNatureButtonIcon;
    private static BitmapSource emojiActivityButtonIconDark;
    private static BitmapSource emojiActivityButtonIcon;
    private static BitmapSource emojiFoodButtonIconDark;
    private static BitmapSource emojiFoodButtonIcon;
    private static BitmapSource emojiTravelButtonIconDark;
    private static BitmapSource emojiTravelButtonIcon;
    private static BitmapSource emojiObjectsButtonIconDark;
    private static BitmapSource emojiObjectsButtonIcon;
    private static BitmapSource emojiSymbolsButtonIconDark;
    private static BitmapSource emojiSymbolsButtonIcon;
    private static BitmapSource emojiFlagsButtonIconDark;
    private static BitmapSource emojiFlagsButtonIcon;
    private static BitmapSource keypadBackSpaceIcon;
    private static BitmapSource keypadSearchButtonIcon;
    private static BitmapSource searchButtonIcon;
    private static BitmapSource emojiCancelSearchButtonIcon;
    private static BitmapSource textboxEmojiIcon;
    private static BitmapSource textboxKeyboardIcon;
    private static BitmapSource phoneIconBlack;
    private static BitmapSource emojiIcon;
    private static BitmapSource keyboardIcon;
    private static BitmapSource dismissIconWhite;
    private static BitmapSource dismissIconBlack;
    private static BitmapSource deleteIconWhite;
    private static BitmapSource deleteIconBlack;
    private static BitmapSource forwardIconWhite;
    private static BitmapSource forwardIconBlack;
    private static BitmapSource inputBarAttachIcon;
    private static BitmapSource inputBarEmojiIcon;
    private static BitmapSource keypadEmojiIcon;
    private static BitmapSource keypadStickerIcon;
    private static BitmapSource keypadPlusIcon;
    private static BitmapSource inputBarKeyboardIcon;
    private static BitmapSource inputBarStatusEmojiIcon;
    private static BitmapSource inputBarStatusSendIcon;
    private static BitmapSource inputBarKeyboardIconWhite;
    private static BitmapSource inputBarMicIcon;
    private static BitmapSource inputBarMicIconRed;
    private static BitmapSource inputBarSendIconLight;
    private static BitmapSource inputBarSendIcon;
    private static BitmapSource inputBarColorIcon;
    private static BitmapSource inputBarFontIconBryndan;
    private static BitmapSource inputBarFontIconGeorgia;
    private static BitmapSource inputBarFontIconNorican;
    private static BitmapSource inputBarFontIconOswald;
    private static BitmapSource inputBarFontIconSegue;
    private static BitmapSource docListPdfIcon;
    private static BitmapSource docListDocIcon;
    private static BitmapSource docListPptIcon;
    private static BitmapSource docListXlsIcon;
    private static BitmapSource docListFileIcon;
    private static BitmapSource retryIconWhite;
    private static BitmapSource retryIcon;
    private static BitmapSource statusIconWhite;
    private static BitmapSource statusIcon;
    private static BitmapSource includeCheckIconWhite;
    private static BitmapSource includeCheckIcon;
    private static BitmapSource excludeCheckIconWhite;
    private static BitmapSource excludeCheckIcon;
    private static BitmapSource cancelIcon;
    private static BitmapSource cancelIconWhite;
    private static BitmapSource eyeIconWhite;
    private static BitmapSource chevronIconBlack;
    private static BitmapSource chevronIcon;
    private static BitmapSource doubleChevronIconBlack;
    private static BitmapSource doubleChevronIcon;
    private static BitmapSource keypadCancelIcon;
    private static BitmapSource keypadGifIcon;
    private static BitmapSource stickerThumbnailPlaceholder;
    private static BitmapSource stickerRecentIconDark;
    private static BitmapSource stickerRecentIcon;
    private static BitmapSource stickerSavedIconDark;
    private static BitmapSource stickerSavedIcon;

    public static BitmapImage LoadAssetByFilepath(string filepath)
    {
      BitmapImage r = (BitmapImage) null;
      Deployment.Current.Dispatcher.InvokeSynchronous((Action) (() => r = AssetStore.LoadBitmapImage(filepath)));
      return r;
    }

    public static BitmapImage LoadAsset(string filename, AssetStore.ThemeSetting themeSetting = AssetStore.ThemeSetting.ToBeDetermined)
    {
      DateTime? start = PerformanceTimer.Start();
      if (themeSetting == AssetStore.ThemeSetting.ToBeDetermined)
        themeSetting = ImageStore.IsDarkTheme() ? AssetStore.ThemeSetting.Dark : AssetStore.ThemeSetting.Light;
      string themeFolder = (string) null;
      themeFolder = themeSetting == AssetStore.ThemeSetting.Dark ? "dark" : (themeSetting != AssetStore.ThemeSetting.Light ? "notheme" : "light");
      BitmapImage r = (BitmapImage) null;
      Deployment.Current.Dispatcher.InvokeSynchronous((Action) (() => r = AssetStore.LoadBitmapImage(string.Format("/Images/assets/{0}/{1}", (object) themeFolder, (object) filename))));
      PerformanceTimer.End("LoadAsset: " + filename, start);
      return r;
    }

    public static BitmapImage LoadDarkThemeAsset(string filename)
    {
      return AssetStore.LoadAsset(filename, AssetStore.ThemeSetting.Dark);
    }

    public static BitmapImage LoadLightThemeAsset(string filename)
    {
      return AssetStore.LoadAsset(filename, AssetStore.ThemeSetting.Light);
    }

    private static BitmapImage LoadBitmapImage(string filepath)
    {
      BitmapImage bitmapImage;
      try
      {
        bitmapImage = new BitmapImage(new Uri(filepath, UriKind.Relative))
        {
          CreateOptions = BitmapCreateOptions.None
        };
        if (bitmapImage.PixelWidth != 0)
        {
          if (bitmapImage.PixelHeight != 0)
            goto label_5;
        }
        bitmapImage = (BitmapImage) null;
        Log.l(nameof (AssetStore), "bitmap size is zero: {0}", (object) filepath);
      }
      catch (Exception ex)
      {
        bitmapImage = (BitmapImage) null;
        string context = string.Format("load asset: {0}", (object) filepath);
        Log.LogException(ex, context);
      }
label_5:
      return bitmapImage;
    }

    private static BitmapSource LoadCached(
      ref BitmapSource cached,
      Func<BitmapSource> loadFunc,
      bool skipCache = true)
    {
      return !skipCache ? cached ?? (cached = loadFunc()) : loadFunc();
    }

    public static BitmapSource InlineClock
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineClock, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineWhiteClock = AssetStore.InlineWhiteClock;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineWhiteClock, UIUtils.SubtleBrush, new Size?(new Size((double) inlineWhiteClock.PixelWidth, (double) inlineWhiteClock.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineWhiteClock
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineWhiteClock, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inline-clock.png", AssetStore.ThemeSetting.Dark)));
      }
    }

    public static BitmapSource InlineSolidWhiteClock
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineSolidWhiteClock, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inline-100-clock.png", AssetStore.ThemeSetting.Dark)));
      }
    }

    public static BitmapSource InlineAccentClock
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineAccentClock, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineSolidWhiteClock = AssetStore.InlineSolidWhiteClock;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineSolidWhiteClock, (Brush) UIUtils.DarkAccentBrush, new Size?(new Size((double) inlineSolidWhiteClock.PixelWidth, (double) inlineSolidWhiteClock.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineError
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineError, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineWhiteError = AssetStore.InlineWhiteError;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineWhiteError, UIUtils.SubtleBrush, new Size?(new Size((double) inlineWhiteError.PixelWidth, (double) inlineWhiteError.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineWhiteError
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineWhiteError, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inline-error.png", AssetStore.ThemeSetting.Dark)));
      }
    }

    public static BitmapSource InlineSolidWhiteError
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineSolidWhiteError, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inline-100-error.png", AssetStore.ThemeSetting.Dark)));
      }
    }

    public static BitmapSource InlineAccentError
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineAccentError, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineSolidWhiteError = AssetStore.InlineSolidWhiteError;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineSolidWhiteError, (Brush) UIUtils.DarkAccentBrush, new Size?(new Size((double) inlineSolidWhiteError.PixelWidth, (double) inlineSolidWhiteError.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineCheck
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineCheck, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineWhiteCheck = AssetStore.InlineWhiteCheck;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineWhiteCheck, UIUtils.SubtleBrush, new Size?(new Size((double) inlineWhiteCheck.PixelWidth, (double) inlineWhiteCheck.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineWhiteCheck
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineWhiteCheck, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inline-check.png", AssetStore.ThemeSetting.Dark)));
      }
    }

    public static BitmapSource InlineSolidWhiteCheck
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineSolidWhiteCheck, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inline-100-check.png", AssetStore.ThemeSetting.Dark)));
      }
    }

    public static BitmapSource InlineAccentCheck
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineAccentCheck, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineSolidWhiteCheck = AssetStore.InlineSolidWhiteCheck;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineSolidWhiteCheck, (Brush) UIUtils.DarkAccentBrush, new Size?(new Size((double) inlineSolidWhiteCheck.PixelWidth, (double) inlineSolidWhiteCheck.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineDoubleChecks
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineDoubleChecks, (Func<BitmapSource>) (() =>
        {
          BitmapSource whiteDoubleChecks = AssetStore.InlineWhiteDoubleChecks;
          return (BitmapSource) IconUtils.CreateColorIcon(whiteDoubleChecks, UIUtils.SubtleBrush, new Size?(new Size((double) whiteDoubleChecks.PixelWidth, (double) whiteDoubleChecks.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineWhiteDoubleChecks
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineWhiteDoubleChecks, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inline-double-check.png", AssetStore.ThemeSetting.Dark)));
      }
    }

    public static BitmapSource InlineSolidWhiteDoubleChecks
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineSolidWhiteDoubleChecks, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inline-100-double-check.png", AssetStore.ThemeSetting.Dark)));
      }
    }

    public static BitmapSource InlineAccentDoubleChecks
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineAccentDoubleChecks, (Func<BitmapSource>) (() =>
        {
          BitmapSource whiteDoubleChecks = AssetStore.InlineSolidWhiteDoubleChecks;
          return (BitmapSource) IconUtils.CreateColorIcon(whiteDoubleChecks, (Brush) UIUtils.DarkAccentBrush, new Size?(new Size((double) whiteDoubleChecks.PixelWidth, (double) whiteDoubleChecks.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineBlueChecks
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineBlueChecks, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inline-blue-checks.png", AssetStore.ThemeSetting.NoTheme)));
      }
    }

    public static BitmapSource InlineContactWhite
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineContactWhite, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inline-contact.png", AssetStore.ThemeSetting.NoTheme)));
      }
    }

    public static BitmapSource InlineContactSubtle
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineContactSubtle, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineContactWhite = AssetStore.InlineContactWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineContactWhite, UIUtils.SubtleBrush, new Size?(new Size((double) inlineContactWhite.PixelWidth, (double) inlineContactWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineContactAccent
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineContactAccent, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineContactWhite = AssetStore.InlineContactWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineContactWhite, (Brush) UIUtils.AccentBrush, new Size?(new Size((double) inlineContactWhite.PixelWidth, (double) inlineContactWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineContact
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineContact, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineContactWhite = AssetStore.InlineContactWhite;
          return ImageStore.IsDarkTheme() ? inlineContactWhite : (BitmapSource) IconUtils.CreateColorIcon(inlineContactWhite, (Brush) UIUtils.BlackBrush, new Size?(new Size((double) inlineContactWhite.PixelWidth, (double) inlineContactWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineMicWhite
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineMicWhite, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inline-mic.png", AssetStore.ThemeSetting.NoTheme)));
      }
    }

    public static BitmapSource InlineMicSubtle
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineMicSubtle, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineMicWhite = AssetStore.InlineMicWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineMicWhite, UIUtils.SubtleBrush, new Size?(new Size((double) inlineMicWhite.PixelWidth, (double) inlineMicWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineMicBlue
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineMicBlue, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inline-blue-mic.png", AssetStore.ThemeSetting.NoTheme)));
      }
    }

    public static BitmapSource InlineMicGreen
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineMicGreen, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inline-green-mic.png", AssetStore.ThemeSetting.NoTheme)));
      }
    }

    private static BitmapSource InlineAudioWhite
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineAudioWhite, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inline-audio.png", AssetStore.ThemeSetting.NoTheme)));
      }
    }

    public static BitmapSource InlineAudioSubtle
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineAudioSubtle, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineAudioWhite = AssetStore.InlineAudioWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineAudioWhite, UIUtils.SubtleBrush, new Size?(new Size((double) inlineAudioWhite.PixelWidth, (double) inlineAudioWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineAudioAccent
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineAudioAccent, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineAudioWhite = AssetStore.InlineAudioWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineAudioWhite, (Brush) UIUtils.AccentBrush, new Size?(new Size((double) inlineAudioWhite.PixelWidth, (double) inlineAudioWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineDocWhite
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineDocWhite, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inline-doc.png", AssetStore.ThemeSetting.NoTheme)));
      }
    }

    public static BitmapSource InlineDocSubtle
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineDocSubtle, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineDocWhite = AssetStore.InlineDocWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineDocWhite, UIUtils.SubtleBrush, new Size?(new Size((double) inlineDocWhite.PixelWidth, (double) inlineDocWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineDocAccent
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineDocAccent, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineDocWhite = AssetStore.InlineDocWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineDocWhite, (Brush) UIUtils.AccentBrush, new Size?(new Size((double) inlineDocWhite.PixelWidth, (double) inlineDocWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineDoc
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineDoc, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineDocWhite = AssetStore.InlineDocWhite;
          return ImageStore.IsDarkTheme() ? inlineDocWhite : (BitmapSource) IconUtils.CreateColorIcon(inlineDocWhite, (Brush) UIUtils.BlackBrush, new Size?(new Size((double) inlineDocWhite.PixelWidth, (double) inlineDocWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineLiveLocationWhite
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineLiveLocationWhite, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inline-livelocation-white.png", AssetStore.ThemeSetting.NoTheme)));
      }
    }

    public static BitmapSource InlineLiveLocationSubtle
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineLiveLocationSubtle, (Func<BitmapSource>) (() =>
        {
          BitmapSource liveLocationWhite = AssetStore.InlineLiveLocationWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(liveLocationWhite, UIUtils.SubtleBrush, new Size?(new Size((double) liveLocationWhite.PixelWidth, (double) liveLocationWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineLiveLocationAccent
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineLiveLocationAccent, (Func<BitmapSource>) (() =>
        {
          BitmapSource liveLocationWhite = AssetStore.InlineLiveLocationWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(liveLocationWhite, (Brush) UIUtils.AccentBrush, new Size?(new Size((double) liveLocationWhite.PixelWidth, (double) liveLocationWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineLiveLocation
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineLiveLocation, (Func<BitmapSource>) (() =>
        {
          BitmapSource liveLocationWhite = AssetStore.InlineLiveLocationWhite;
          return ImageStore.IsDarkTheme() ? liveLocationWhite : (BitmapSource) IconUtils.CreateColorIcon(liveLocationWhite, (Brush) UIUtils.BlackBrush, new Size?(new Size((double) liveLocationWhite.PixelWidth, (double) liveLocationWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource LiveLocationWhite
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.liveLocationWhite, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("icon-livelocation.png", AssetStore.ThemeSetting.NoTheme)));
      }
    }

    public static BitmapSource LiveLocationSubtle
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.liveLocationSubtle, (Func<BitmapSource>) (() =>
        {
          BitmapSource liveLocationWhite = AssetStore.LiveLocationWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(liveLocationWhite, UIUtils.SubtleBrush, new Size?(new Size((double) liveLocationWhite.PixelWidth, (double) liveLocationWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource LiveLocationAccent
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.liveLocationAccent, (Func<BitmapSource>) (() =>
        {
          BitmapSource liveLocationWhite = AssetStore.LiveLocationWhite;
          if (liveLocationWhite != null)
            return (BitmapSource) IconUtils.CreateColorIcon(liveLocationWhite, (Brush) UIUtils.AccentBrush, new Size?(new Size((double) liveLocationWhite.PixelWidth, (double) liveLocationWhite.PixelHeight)));
          Log.SendCrashLog((Exception) new NullReferenceException("LiveLocationWhite is null"), "Fail gracefully", logOnlyForRelease: true);
          return (BitmapSource) null;
        }), false);
      }
    }

    public static BitmapSource LiveLocation
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.liveLocation, (Func<BitmapSource>) (() =>
        {
          BitmapSource liveLocationWhite = AssetStore.LiveLocationWhite;
          return ImageStore.IsDarkTheme() ? liveLocationWhite : (BitmapSource) IconUtils.CreateColorIcon(liveLocationWhite, (Brush) UIUtils.BlackBrush, new Size?(new Size((double) liveLocationWhite.PixelWidth, (double) liveLocationWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineLocationWhite
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineLocationWhite, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inline-location.png", AssetStore.ThemeSetting.NoTheme)));
      }
    }

    public static BitmapSource InlineLocationSubtle
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineLocationSubtle, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineLocationWhite = AssetStore.InlineLocationWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineLocationWhite, UIUtils.SubtleBrush, new Size?(new Size((double) inlineLocationWhite.PixelWidth, (double) inlineLocationWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineLocationAccent
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineLocationAccent, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineLocationWhite = AssetStore.InlineLocationWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineLocationWhite, (Brush) UIUtils.AccentBrush, new Size?(new Size((double) inlineLocationWhite.PixelWidth, (double) inlineLocationWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineLocation
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineLocation, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineLocationWhite = AssetStore.InlineLocationWhite;
          return ImageStore.IsDarkTheme() ? inlineLocationWhite : (BitmapSource) IconUtils.CreateColorIcon(inlineLocationWhite, (Brush) UIUtils.BlackBrush, new Size?(new Size((double) inlineLocationWhite.PixelWidth, (double) inlineLocationWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineRevokeWhite
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineRevokeWhite, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inline-revoke-white.png", AssetStore.ThemeSetting.NoTheme)));
      }
    }

    public static BitmapSource InlineRevokeSubtle
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineRevokeSubtle, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineRevokeWhite = AssetStore.InlineRevokeWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineRevokeWhite, UIUtils.SubtleBrush, new Size?(new Size((double) inlineRevokeWhite.PixelWidth, (double) inlineRevokeWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineRevokeAccent
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineRevokeAccent, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineRevokeWhite = AssetStore.InlineRevokeWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineRevokeWhite, (Brush) UIUtils.AccentBrush, new Size?(new Size((double) inlineRevokeWhite.PixelWidth, (double) inlineRevokeWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineRevoke
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineRevoke, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineRevokeWhite = AssetStore.InlineRevokeWhite;
          return ImageStore.IsDarkTheme() ? inlineRevokeWhite : (BitmapSource) IconUtils.CreateColorIcon(inlineRevokeWhite, (Brush) UIUtils.BlackBrush, new Size?(new Size((double) inlineRevokeWhite.PixelWidth, (double) inlineRevokeWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineStatusWhite
    {
      get => (BitmapSource) AssetStore.LoadAsset("inline-status.png", AssetStore.ThemeSetting.Dark);
    }

    public static BitmapSource InlineStatus
    {
      get => (BitmapSource) AssetStore.LoadAsset("inline-status.png");
    }

    public static BitmapSource WhatsAppAvatar
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.whatsappAvatar, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("whatsapp-avatar.png", AssetStore.ThemeSetting.NoTheme)));
      }
    }

    public static BitmapSource InlinePictureWhite
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlinePictureWhite, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inline-picture.png", AssetStore.ThemeSetting.NoTheme)));
      }
    }

    public static BitmapSource InlineGifWhite
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineGifWhite, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inline-gif.png", AssetStore.ThemeSetting.NoTheme)));
      }
    }

    public static BitmapSource InlineStickerWhite
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineStickerWhite, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inline-sticker.png", AssetStore.ThemeSetting.NoTheme)));
      }
    }

    public static BitmapSource InlineVerified
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineVerified, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("icon_verified.png", AssetStore.ThemeSetting.NoTheme)));
      }
    }

    public static BitmapSource InlinePictureSubtle
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlinePictureSubtle, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlinePictureWhite = AssetStore.InlinePictureWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(inlinePictureWhite, UIUtils.SubtleBrush, new Size?(new Size((double) inlinePictureWhite.PixelWidth, (double) inlinePictureWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlinePictureAccent
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlinePictureAccent, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlinePictureWhite = AssetStore.InlinePictureWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(inlinePictureWhite, (Brush) UIUtils.AccentBrush, new Size?(new Size((double) inlinePictureWhite.PixelWidth, (double) inlinePictureWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineGifSubtle
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineGifSubtle, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineGifWhite = AssetStore.InlineGifWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineGifWhite, UIUtils.SubtleBrush, new Size?(new Size((double) inlineGifWhite.PixelWidth, (double) inlineGifWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineGifAccent
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineGifAccent, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineGifWhite = AssetStore.InlineGifWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineGifWhite, (Brush) UIUtils.AccentBrush, new Size?(new Size((double) inlineGifWhite.PixelWidth, (double) inlineGifWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineStickerSubtle
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineStickerSubtle, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineStickerWhite = AssetStore.InlineStickerWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineStickerWhite, UIUtils.SubtleBrush, new Size?(new Size((double) inlineStickerWhite.PixelWidth, (double) inlineStickerWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineStickerAccent
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineStickerAccent, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineStickerWhite = AssetStore.InlineStickerWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineStickerWhite, (Brush) UIUtils.AccentBrush, new Size?(new Size((double) inlineStickerWhite.PixelWidth, (double) inlineStickerWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlinePicture
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlinePicture, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlinePictureWhite = AssetStore.InlinePictureWhite;
          return ImageStore.IsDarkTheme() ? inlinePictureWhite : (BitmapSource) IconUtils.CreateColorIcon(inlinePictureWhite, (Brush) UIUtils.BlackBrush, new Size?(new Size((double) inlinePictureWhite.PixelWidth, (double) inlinePictureWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineVideoWhite
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineVideoWhite, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inline-video.png", AssetStore.ThemeSetting.NoTheme)));
      }
    }

    public static BitmapSource InlineVideoSubtle
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineVideoSubtle, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineVideoWhite = AssetStore.InlineVideoWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineVideoWhite, UIUtils.SubtleBrush, new Size?(new Size((double) inlineVideoWhite.PixelWidth, (double) inlineVideoWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineVideoAccent
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineVideoAccent, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineVideoWhite = AssetStore.InlineVideoWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineVideoWhite, (Brush) UIUtils.AccentBrush, new Size?(new Size((double) inlineVideoWhite.PixelWidth, (double) inlineVideoWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineVideo
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineVideo, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineVideoWhite = AssetStore.InlineVideoWhite;
          return ImageStore.IsDarkTheme() ? inlineVideoWhite : (BitmapSource) IconUtils.CreateColorIcon(inlineVideoWhite, (Brush) UIUtils.BlackBrush, new Size?(new Size((double) inlineVideoWhite.PixelWidth, (double) inlineVideoWhite.PixelHeight)));
        }), false);
      }
    }

    private static BitmapSource InlineMuteWhite
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineMuteWhite, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inline-mute.png", AssetStore.ThemeSetting.NoTheme)));
      }
    }

    public static BitmapSource InlineMuteSubtle
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineMuteSubtle, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineMuteWhite = AssetStore.InlineMuteWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineMuteWhite, UIUtils.SubtleBrush, new Size?(new Size((double) inlineMuteWhite.PixelWidth, (double) inlineMuteWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineMuteAccent
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineMuteAccent, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineMuteWhite = AssetStore.InlineMuteWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineMuteWhite, (Brush) UIUtils.AccentBrush, new Size?(new Size((double) inlineMuteWhite.PixelWidth, (double) inlineMuteWhite.PixelHeight)));
        }), false);
      }
    }

    private static BitmapSource InlinePinWhite
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlinePinWhite, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inline-pin.png", AssetStore.ThemeSetting.Dark)));
      }
    }

    public static BitmapSource InlinePinSubtle
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlinePinSubtle, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlinePinWhite = AssetStore.InlinePinWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(inlinePinWhite, UIUtils.SubtleBrush, new Size?(new Size((double) inlinePinWhite.PixelWidth, (double) inlinePinWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlinePinAccent
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlinePinAccent, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlinePinWhite = AssetStore.InlinePinWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(inlinePinWhite, (Brush) UIUtils.AccentBrush, new Size?(new Size((double) inlinePinWhite.PixelWidth, (double) inlinePinWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource InlineWhiteBroadcast
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineWhiteBroadcast, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inline-broadcast.png", AssetStore.ThemeSetting.Dark)));
      }
    }

    public static BitmapSource InlineAccentBroadcast
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inlineAccentBroadcast, (Func<BitmapSource>) (() =>
        {
          BitmapSource inlineWhiteBroadcast = AssetStore.InlineWhiteBroadcast;
          return (BitmapSource) IconUtils.CreateColorIcon(inlineWhiteBroadcast, (Brush) UIUtils.DarkAccentBrush, new Size?(new Size((double) inlineWhiteBroadcast.PixelWidth, (double) inlineWhiteBroadcast.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource Broadcast
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.broadcast, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("broadcast.png", AssetStore.ThemeSetting.Dark)));
      }
    }

    public static BitmapSource DefaultGroupIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.defaultGroupIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("default-group-icon.png")));
      }
    }

    public static BitmapSource DefaultContactIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.defaultContactIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("default-contact-icon.png")));
      }
    }

    public static BitmapSource DefaultContactIconLarge
    {
      get
      {
        AssetStore.ThemeSetting themeSetting = ImageStore.IsDarkTheme() ? AssetStore.ThemeSetting.Dark : AssetStore.ThemeSetting.Light;
        return AssetStore.LoadCached(ref AssetStore.defaultContactIconLarge, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("default-contact-icon-480x480.jpg", themeSetting)));
      }
    }

    public static BitmapSource DefaultContactIconBlack
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.defaultContactIconBlack, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("default-contact-icon.png", AssetStore.ThemeSetting.Light)));
      }
    }

    public static BitmapSource PoweredByBingIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.poweredByBingIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("powered-by-bing.png")));
      }
    }

    public static BitmapSource SlideToCancelMaskDefault
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.slideToCancelMaskDefault, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("slide-to-cancel-mask.png")), false);
      }
    }

    public static BitmapSource SlideToCancelMaskChrome
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.slideToCancelMaskChrome, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("slide-to-cancel-mask-chrome.png")), false);
      }
    }

    public static BitmapSource ForwardArrow
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.forwardArrow, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("forward-arrow.png", AssetStore.ThemeSetting.NoTheme)));
      }
    }

    public static BitmapSource LocationButtonIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.locationButtonIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("attach-location.png", AssetStore.ThemeSetting.NoTheme)), false);
      }
    }

    public static BitmapSource CamcorderButtonIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.camcorderButtonIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("attach-camcorder.png", AssetStore.ThemeSetting.NoTheme)), false);
      }
    }

    public static BitmapSource CameraButtonIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.cameraButtonIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("attach-camera.png", AssetStore.ThemeSetting.NoTheme)), false);
      }
    }

    public static BitmapSource PictureButtonIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.pictureButtonIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("attach-picture.png", AssetStore.ThemeSetting.NoTheme)), false);
      }
    }

    public static BitmapSource VideoButtonIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.videoButtonIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("attach-video.png", AssetStore.ThemeSetting.NoTheme)), false);
      }
    }

    public static BitmapSource AudioButtonIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.audioButtonIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("attach-audio.png", AssetStore.ThemeSetting.NoTheme)), false);
      }
    }

    public static BitmapSource ContactButtonIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.contactButtonIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("attach-contact.png", AssetStore.ThemeSetting.NoTheme)), false);
      }
    }

    public static BitmapSource DocumentButtonIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.docButtonIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("attach-doc.png", AssetStore.ThemeSetting.NoTheme)), false);
      }
    }

    public static BitmapSource EmojiRecentButtonIcon(AssetStore.ThemeSetting requested)
    {
      BitmapSource cached = AssetStore.emojiRecentButtonIcon;
      if (requested == AssetStore.ThemeSetting.Dark || requested == AssetStore.ThemeSetting.ToBeDetermined && ImageStore.IsDarkTheme())
        cached = AssetStore.emojiRecentButtonIconDark;
      return AssetStore.LoadCached(ref cached, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("emoji-recent.png", requested)), false);
    }

    public static BitmapSource EmojiPeopleButtonIcon(AssetStore.ThemeSetting requested)
    {
      BitmapSource cached = AssetStore.emojiPeopleButtonIcon;
      if (requested == AssetStore.ThemeSetting.Dark || requested == AssetStore.ThemeSetting.ToBeDetermined && ImageStore.IsDarkTheme())
        cached = AssetStore.emojiPeopleButtonIconDark;
      return AssetStore.LoadCached(ref cached, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("emoji-people.png", requested)), false);
    }

    public static BitmapSource EmojiNatureButtonIcon(AssetStore.ThemeSetting requested)
    {
      BitmapSource cached = AssetStore.emojiNatureButtonIcon;
      if (requested == AssetStore.ThemeSetting.Dark || requested == AssetStore.ThemeSetting.ToBeDetermined && ImageStore.IsDarkTheme())
        cached = AssetStore.emojiNatureButtonIconDark;
      return AssetStore.LoadCached(ref cached, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("emoji-nature.png", requested)), false);
    }

    public static BitmapSource EmojiActivityButtonIcon(AssetStore.ThemeSetting requested)
    {
      BitmapSource cached = AssetStore.emojiActivityButtonIcon;
      if (requested == AssetStore.ThemeSetting.Dark || requested == AssetStore.ThemeSetting.ToBeDetermined && ImageStore.IsDarkTheme())
        cached = AssetStore.emojiActivityButtonIconDark;
      return AssetStore.LoadCached(ref cached, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("emoji-activity.png", requested)), false);
    }

    public static BitmapSource EmojiFoodButtonIcon(AssetStore.ThemeSetting requested)
    {
      BitmapSource cached = AssetStore.emojiFoodButtonIcon;
      if (requested == AssetStore.ThemeSetting.Dark || requested == AssetStore.ThemeSetting.ToBeDetermined && ImageStore.IsDarkTheme())
        cached = AssetStore.emojiFoodButtonIconDark;
      return AssetStore.LoadCached(ref cached, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("emoji-food.png", requested)), false);
    }

    public static BitmapSource EmojiTravelButtonIcon(AssetStore.ThemeSetting requested)
    {
      BitmapSource cached = AssetStore.emojiTravelButtonIcon;
      if (requested == AssetStore.ThemeSetting.Dark || requested == AssetStore.ThemeSetting.ToBeDetermined && ImageStore.IsDarkTheme())
        cached = AssetStore.emojiTravelButtonIconDark;
      return AssetStore.LoadCached(ref cached, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("emoji-travel.png", requested)), false);
    }

    public static BitmapSource EmojiObjectsButtonIcon(AssetStore.ThemeSetting requested)
    {
      BitmapSource cached = AssetStore.emojiObjectsButtonIcon;
      if (requested == AssetStore.ThemeSetting.Dark || requested == AssetStore.ThemeSetting.ToBeDetermined && ImageStore.IsDarkTheme())
        cached = AssetStore.emojiObjectsButtonIconDark;
      return AssetStore.LoadCached(ref cached, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("emoji-objects.png", requested)), false);
    }

    public static BitmapSource EmojiSymbolsButtonIcon(AssetStore.ThemeSetting requested)
    {
      BitmapSource cached = AssetStore.emojiSymbolsButtonIcon;
      if (requested == AssetStore.ThemeSetting.Dark || requested == AssetStore.ThemeSetting.ToBeDetermined && ImageStore.IsDarkTheme())
        cached = AssetStore.emojiSymbolsButtonIconDark;
      return AssetStore.LoadCached(ref cached, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("emoji-symbols.png", requested)), false);
    }

    public static BitmapSource EmojiFlagsButtonIcon(AssetStore.ThemeSetting requested)
    {
      BitmapSource cached = AssetStore.emojiFlagsButtonIcon;
      if (requested == AssetStore.ThemeSetting.Dark || requested == AssetStore.ThemeSetting.ToBeDetermined && ImageStore.IsDarkTheme())
        cached = AssetStore.emojiFlagsButtonIconDark;
      return AssetStore.LoadCached(ref cached, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("emoji-flags.png", requested)), false);
    }

    public static BitmapSource KeypadBackSpaceIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.keypadBackSpaceIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("keypad-backspace.png", AssetStore.ThemeSetting.Dark)), false);
      }
    }

    public static BitmapSource KeypadSearchButtonIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.keypadSearchButtonIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("keypad-search.png", AssetStore.ThemeSetting.Dark)), false);
      }
    }

    public static BitmapSource SearchButtonIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.searchButtonIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("keypad-search.png")), false);
      }
    }

    public static BitmapSource EmojiCancelSearchButtonIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.emojiCancelSearchButtonIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("emoji-back.png")), false);
      }
    }

    public static BitmapSource TextboxEmojiIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.textboxEmojiIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("textbox-action-emoji.png", AssetStore.ThemeSetting.NoTheme)), false);
      }
    }

    public static BitmapSource TextboxKeyboardIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.textboxKeyboardIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("textbox-action-keyboard.png", AssetStore.ThemeSetting.NoTheme)), false);
      }
    }

    public static BitmapSource VolumeIconWhite
    {
      get => (BitmapSource) AssetStore.LoadAsset("volume.png", AssetStore.ThemeSetting.Dark);
    }

    public static BitmapSource VolumeMuteIconWhite
    {
      get => (BitmapSource) AssetStore.LoadAsset("volume-mute.png", AssetStore.ThemeSetting.Dark);
    }

    public static BitmapSource MicIconWhite
    {
      get => (BitmapSource) AssetStore.LoadAsset("mic-white.png", AssetStore.ThemeSetting.NoTheme);
    }

    public static BitmapSource MicIconBlack
    {
      get => (BitmapSource) AssetStore.LoadAsset("mic-black.png", AssetStore.ThemeSetting.NoTheme);
    }

    public static BitmapSource MicIconRed
    {
      get => (BitmapSource) AssetStore.LoadAsset("mic-red.png", AssetStore.ThemeSetting.NoTheme);
    }

    public static BitmapSource MicIconSubtle
    {
      get => (BitmapSource) AssetStore.LoadAsset("mic-subtle.png", AssetStore.ThemeSetting.NoTheme);
    }

    public static BitmapSource ContactInfoIcon
    {
      get => (BitmapSource) AssetStore.LoadAsset("contact-info.png", AssetStore.ThemeSetting.Dark);
    }

    public static BitmapSource RecentCallsIcon
    {
      get => (BitmapSource) AssetStore.LoadAsset("recent-calls.png", AssetStore.ThemeSetting.Dark);
    }

    public static BitmapSource PhoneIconWhiteSolid
    {
      get => (BitmapSource) AssetStore.LoadAsset("phone-solid.png", AssetStore.ThemeSetting.Dark);
    }

    public static BitmapSource PhoneIconWhite
    {
      get => (BitmapSource) AssetStore.LoadAsset("phone.png", AssetStore.ThemeSetting.Dark);
    }

    public static BitmapSource PhoneIconBlack
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.phoneIconBlack, (Func<BitmapSource>) (() =>
        {
          BitmapSource phoneIconWhite = AssetStore.PhoneIconWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(phoneIconWhite, (Brush) UIUtils.BlackBrush, new Size?(new Size((double) phoneIconWhite.PixelWidth, (double) phoneIconWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource PhoneIcon
    {
      get => !ImageStore.IsDarkTheme() ? AssetStore.PhoneIconBlack : AssetStore.PhoneIconWhite;
    }

    public static BitmapSource VCardIcon
    {
      get => (BitmapSource) AssetStore.LoadAsset("vcard.png", AssetStore.ThemeSetting.Dark);
    }

    public static BitmapSource VideoCallIconWhiteSolid
    {
      get => (BitmapSource) AssetStore.LoadAsset("video-solid.png", AssetStore.ThemeSetting.Dark);
    }

    public static BitmapSource VideoCallIcon
    {
      get => (BitmapSource) AssetStore.LoadAsset("video.png", AssetStore.ThemeSetting.Dark);
    }

    public static BitmapSource StickerE022
    {
      get => (BitmapSource) AssetStore.LoadBitmapImage("/Images/emojis/sticker-E022.png");
    }

    public static BitmapSource EncryptedMessage
    {
      get
      {
        return (BitmapSource) AssetStore.LoadAsset("encrypted-message.png", AssetStore.ThemeSetting.NoTheme);
      }
    }

    public static BitmapSource UnsupportedMessage
    {
      get
      {
        return (BitmapSource) AssetStore.LoadAsset("unsupported-message.png", AssetStore.ThemeSetting.NoTheme);
      }
    }

    public static BitmapSource RevokedMessage
    {
      get
      {
        return (BitmapSource) AssetStore.LoadAsset("revoked-message.png", AssetStore.ThemeSetting.NoTheme);
      }
    }

    public static BitmapSource MessageIconWhite
    {
      get => (BitmapSource) AssetStore.LoadAsset("message.png", AssetStore.ThemeSetting.Dark);
    }

    public static BitmapSource LumiaCameraIcon
    {
      get => (BitmapSource) AssetStore.LoadBitmapImage("/Images/camera/icon-cam-capture.png");
    }

    public static BitmapSource LumiaCameraIconBlack
    {
      get => (BitmapSource) AssetStore.LoadBitmapImage("/Images/camera/icon-cam-capture-b.png");
    }

    public static BitmapSource FlashIcon
    {
      get => (BitmapSource) AssetStore.LoadBitmapImage("/Images/camera/icon-cam-flash.png");
    }

    public static BitmapSource FlashAutoIcon
    {
      get => (BitmapSource) AssetStore.LoadBitmapImage("/Images/camera/icon-cam-autoflash.png");
    }

    public static BitmapSource NoFlashIcon
    {
      get => (BitmapSource) AssetStore.LoadBitmapImage("/Images/camera/icon-cam-noflash.png");
    }

    public static BitmapSource DocTypeIconDefault
    {
      get
      {
        return (BitmapSource) AssetStore.LoadAsset("msgbubble-file-icon.png", AssetStore.ThemeSetting.NoTheme);
      }
    }

    public static BitmapSource DocTypeIconPdf
    {
      get
      {
        return (BitmapSource) AssetStore.LoadAsset("msgbubble-pdf-icon.png", AssetStore.ThemeSetting.NoTheme);
      }
    }

    public static BitmapSource DocTypeIconDoc
    {
      get
      {
        return (BitmapSource) AssetStore.LoadAsset("msgbubble-doc-icon.png", AssetStore.ThemeSetting.NoTheme);
      }
    }

    public static BitmapSource DocTypeIconPpt
    {
      get
      {
        return (BitmapSource) AssetStore.LoadAsset("msgbubble-ppt-icon.png", AssetStore.ThemeSetting.NoTheme);
      }
    }

    public static BitmapSource DocTypeIconXls
    {
      get
      {
        return (BitmapSource) AssetStore.LoadAsset("msgbubble-xls-icon.png", AssetStore.ThemeSetting.NoTheme);
      }
    }

    public static BitmapSource AttachIcon
    {
      get => (BitmapSource) AssetStore.LoadAsset("attach.png", AssetStore.ThemeSetting.NoTheme);
    }

    public static BitmapSource EmojiIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.emojiIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("emoji.png", AssetStore.ThemeSetting.NoTheme)), false);
      }
    }

    public static BitmapSource KeyboardIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.keyboardIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("keyboard.png", AssetStore.ThemeSetting.NoTheme)), false);
      }
    }

    public static BitmapSource DismissIconWhite
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.dismissIconWhite, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("x.png", AssetStore.ThemeSetting.NoTheme)), false);
      }
    }

    public static BitmapSource DismissIconBlack
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.dismissIconBlack, (Func<BitmapSource>) (() =>
        {
          BitmapSource dismissIconWhite = AssetStore.DismissIconWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(dismissIconWhite, (Brush) UIUtils.BlackBrush, new Size?(new Size((double) dismissIconWhite.PixelWidth, (double) dismissIconWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource DismissIcon
    {
      get => !ImageStore.IsDarkTheme() ? AssetStore.DismissIconBlack : AssetStore.DismissIconWhite;
    }

    public static BitmapSource DeleteIconWhite
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.deleteIconWhite, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("delete.png", AssetStore.ThemeSetting.Dark)), false);
      }
    }

    public static BitmapSource DeleteIconBlack
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.deleteIconBlack, (Func<BitmapSource>) (() =>
        {
          BitmapSource deleteIconWhite = AssetStore.DeleteIconWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(deleteIconWhite, (Brush) UIUtils.BlackBrush, new Size?(new Size((double) deleteIconWhite.PixelWidth, (double) deleteIconWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource DeleteIcon
    {
      get => !ImageStore.IsDarkTheme() ? AssetStore.DeleteIconBlack : AssetStore.DeleteIconWhite;
    }

    public static BitmapSource ForwardIconWhite
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.forwardIconWhite, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("forward.png", AssetStore.ThemeSetting.Dark)));
      }
    }

    public static BitmapSource ForwardIconBlack
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.forwardIconBlack, (Func<BitmapSource>) (() =>
        {
          BitmapSource forwardIconWhite = AssetStore.ForwardIconWhite;
          return (BitmapSource) IconUtils.CreateColorIcon(forwardIconWhite, (Brush) UIUtils.BlackBrush, new Size?(new Size((double) forwardIconWhite.PixelWidth, (double) forwardIconWhite.PixelHeight)));
        }), false);
      }
    }

    public static BitmapSource ForwardIcon
    {
      get => !ImageStore.IsDarkTheme() ? AssetStore.ForwardIconBlack : AssetStore.ForwardIconWhite;
    }

    public static BitmapSource InputBarAttachIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inputBarAttachIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inputbar-attach.png")), false);
      }
    }

    public static BitmapSource InputBarEmojiIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inputBarEmojiIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inputbar-emoji.png")), false);
      }
    }

    public static BitmapSource KeypadEmojiIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.keypadEmojiIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("keypad-emoji.png")), false);
      }
    }

    public static BitmapSource KeypadStickerIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.keypadStickerIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("keypad-sticker.png")), false);
      }
    }

    public static BitmapSource KeypadPlusIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.keypadPlusIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("keypad-custom.png", AssetStore.ThemeSetting.Dark)), false);
      }
    }

    public static BitmapSource InputBarKeyboardIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inputBarKeyboardIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inputbar-keyboard.png")), false);
      }
    }

    public static BitmapSource InputBarStatusEmojiIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inputBarStatusEmojiIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inputbar-status-emoji.png", AssetStore.ThemeSetting.Dark)), false);
      }
    }

    public static BitmapSource InputBarStatusSendIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inputBarStatusSendIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inputbar-status-send.png", AssetStore.ThemeSetting.Dark)), false);
      }
    }

    public static BitmapSource InputBarKeyboardIconWhite
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inputBarKeyboardIconWhite, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inputbar-keyboard.png", AssetStore.ThemeSetting.Dark)), false);
      }
    }

    public static BitmapSource InputBarMicIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inputBarMicIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inputbar-mic.png")), false);
      }
    }

    public static BitmapSource InputBarMicIconRed
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inputBarMicIconRed, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inputbar-mic-red.png", AssetStore.ThemeSetting.NoTheme)));
      }
    }

    public static BitmapSource InputBarSendIconLight
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inputBarSendIconLight, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inputbar-send.png", AssetStore.ThemeSetting.Light)), false);
      }
    }

    public static BitmapSource InputBarSendIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inputBarSendIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inputbar-send.png", AssetStore.ThemeSetting.Dark)), false);
      }
    }

    public static BitmapSource InputBarColorIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inputBarColorIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inputbar-color.png", AssetStore.ThemeSetting.Dark)), false);
      }
    }

    public static BitmapSource InputBarFontIconBryndan
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inputBarFontIconBryndan, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inputbar-font-bryndan.png", AssetStore.ThemeSetting.Dark)), false);
      }
    }

    public static BitmapSource InputBarFontIconGeorgia
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inputBarFontIconGeorgia, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inputbar-font-georgia.png", AssetStore.ThemeSetting.Dark)), false);
      }
    }

    public static BitmapSource InputBarFontIconNorican
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inputBarFontIconNorican, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inputbar-font-norican.png", AssetStore.ThemeSetting.Dark)), false);
      }
    }

    public static BitmapSource InputBarFontIconOswald
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inputBarFontIconOswald, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inputbar-font-oswald.png", AssetStore.ThemeSetting.Dark)), false);
      }
    }

    public static BitmapSource InputBarFontIconSegue
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.inputBarFontIconSegue, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("inputbar-font-segue.png", AssetStore.ThemeSetting.Dark)), false);
      }
    }

    public static BitmapSource ScanSuccessCheck
    {
      get => (BitmapSource) AssetStore.LoadBitmapImage("/Images/icon-verification-success.png");
    }

    public static BitmapSource ScanFailureBang
    {
      get => (BitmapSource) AssetStore.LoadBitmapImage("/Images/icon-verification-failure.png");
    }

    public static BitmapSource DocListPdfIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.docListPdfIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("doc-list-pdf.png", AssetStore.ThemeSetting.NoTheme)), false);
      }
    }

    public static BitmapSource DocListDocIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.docListDocIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("doc-list-doc.png", AssetStore.ThemeSetting.NoTheme)), false);
      }
    }

    public static BitmapSource DocListPptIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.docListPptIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("doc-list-ppt.png", AssetStore.ThemeSetting.NoTheme)), false);
      }
    }

    public static BitmapSource DocListXlsIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.docListXlsIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("doc-list-xls.png", AssetStore.ThemeSetting.NoTheme)), false);
      }
    }

    public static BitmapSource DocListFileIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.docListFileIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("doc-list-file.png")), false);
      }
    }

    public static BitmapSource RetryIconWhite
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.retryIconWhite, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("retry.png", AssetStore.ThemeSetting.Dark)), false);
      }
    }

    public static BitmapSource RetryIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.retryIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("retry.png")), false);
      }
    }

    public static BitmapSource StatusIconWhite
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.statusIconWhite, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("status.png", AssetStore.ThemeSetting.Dark)));
      }
    }

    public static BitmapSource StatusIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.statusIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("status.png")));
      }
    }

    public static BitmapSource IncludeCheckIconWhite
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.includeCheckIconWhite, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("include-check.png", AssetStore.ThemeSetting.Dark)));
      }
    }

    public static BitmapSource IncludeCheckIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.includeCheckIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("include-check.png")));
      }
    }

    public static BitmapSource ExcludeCheckIconWhite
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.excludeCheckIconWhite, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("exclude-check.png", AssetStore.ThemeSetting.Dark)));
      }
    }

    public static BitmapSource ExcludeCheckIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.excludeCheckIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("exclude-check.png")));
      }
    }

    public static BitmapSource CancelIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.cancelIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("cancel.png")), false);
      }
    }

    public static BitmapSource CancelIconWhite
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.cancelIconWhite, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("cancel.png", AssetStore.ThemeSetting.Dark)), false);
      }
    }

    public static BitmapSource EyeIconWhite
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.eyeIconWhite, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("eye.png", AssetStore.ThemeSetting.Dark)), false);
      }
    }

    public static BitmapSource ChevronIconBlack
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.chevronIconBlack, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("chevron.png", AssetStore.ThemeSetting.Light)));
      }
    }

    public static BitmapSource ChevronIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.chevronIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("chevron.png")), false);
      }
    }

    public static BitmapSource DoubleChevronIconBlack
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.doubleChevronIconBlack, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("double-chevron.png", AssetStore.ThemeSetting.Light)), false);
      }
    }

    public static BitmapSource DoubleChevronIcon
    {
      get
      {
        return AssetStore.LoadCached(ref AssetStore.doubleChevronIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("double-chevron.png")), false);
      }
    }

    public static BitmapSource BizVerifiedHighIcon
    {
      get => (BitmapSource) AssetStore.LoadAsset("biz-badge-verified.png");
    }

    public static BitmapSource BizVerifiedLowIcon
    {
      get => (BitmapSource) AssetStore.LoadAsset("biz-badge-confirmed.png");
    }

    public static BitmapSource BizVerifiedUnknownIcon
    {
      get => (BitmapSource) AssetStore.LoadAsset("biz-badge-unknown.png");
    }

    public static BitmapSource BizInfoIcon => (BitmapSource) AssetStore.LoadAsset("biz-info.png");

    public static BitmapSource BizEmailIcon => (BitmapSource) AssetStore.LoadAsset("biz-email.png");

    public static BitmapSource BizLocationIcon
    {
      get => (BitmapSource) AssetStore.LoadAsset("biz-location.png");
    }

    public static BitmapSource BizWebsiteIcon
    {
      get => (BitmapSource) AssetStore.LoadAsset("biz-website.png");
    }

    public static BitmapSource BizCategoryIcon
    {
      get => (BitmapSource) AssetStore.LoadAsset("biz-category.png");
    }

    public static BitmapSource BizHoursIcon => (BitmapSource) AssetStore.LoadAsset("biz-hours.png");

    public static BitmapSource BizInstagramIcon
    {
      get => (BitmapSource) AssetStore.LoadAsset("biz-insta.png");
    }

    private static BitmapSource LoadVoipIcon(string filename, bool isDarkTheme)
    {
      return (BitmapSource) AssetStore.LoadBitmapImage(string.Format("/Images/voip/{0}/{1}", isDarkTheme ? (object) "dark" : (object) "light", (object) filename));
    }

    public static BitmapSource CallScreenAddIcon
    {
      get => AssetStore.LoadVoipIcon("icon_add.png", ImageStore.IsDarkTheme());
    }

    public static BitmapSource CallScreenAnswerIcon
    {
      get => AssetStore.LoadVoipIcon("icon_answer-call.png", true);
    }

    public static BitmapSource CallScreenBluetoothIcon
    {
      get => AssetStore.LoadVoipIcon("icon_bluetooth.png", ImageStore.IsDarkTheme());
    }

    public static BitmapSource CallScreenCameraIcon
    {
      get => AssetStore.LoadVoipIcon("icon_camera.png", ImageStore.IsDarkTheme());
    }

    public static BitmapSource CallScreenDeclineIcon
    {
      get => AssetStore.LoadVoipIcon("icon_decline.png", ImageStore.IsDarkTheme());
    }

    public static BitmapSource CallScreenEndCallIcon
    {
      get => AssetStore.LoadVoipIcon("icon_call-end.png", true);
    }

    public static BitmapSource CallScreenCancelInvitationIcon
    {
      get => AssetStore.LoadVoipIcon("icon_call-end.png", ImageStore.IsDarkTheme());
    }

    public static BitmapSource CallScreenMessageIcon
    {
      get => AssetStore.LoadVoipIcon("icon_message.png", ImageStore.IsDarkTheme());
    }

    public static BitmapSource CallScreenMuteIcon
    {
      get => AssetStore.LoadVoipIcon("icon_mute.png", ImageStore.IsDarkTheme());
    }

    public static BitmapSource CallScreenSpeakerIcon
    {
      get => AssetStore.LoadVoipIcon("icon_speaker.png", ImageStore.IsDarkTheme());
    }

    public static BitmapSource CallScreenVideoOnIcon
    {
      get => AssetStore.LoadVoipIcon("icon_video.png", ImageStore.IsDarkTheme());
    }

    public static BitmapSource CallScreenVideoOffIcon
    {
      get => AssetStore.LoadVoipIcon("icon_video_off.png", ImageStore.IsDarkTheme());
    }

    public static BitmapSource CallScreenAddIconWhite
    {
      get => AssetStore.LoadVoipIcon("icon_add.png", true);
    }

    public static BitmapSource CallScreenAnswerIconWhite
    {
      get => AssetStore.LoadVoipIcon("icon_answer-call.png", true);
    }

    public static BitmapSource CallScreenBluetoothIconWhite
    {
      get => AssetStore.LoadVoipIcon("icon_bluetooth.png", true);
    }

    public static BitmapSource CallScreenCameraIconWhite
    {
      get => AssetStore.LoadVoipIcon("icon_camera.png", true);
    }

    public static BitmapSource CallScreenDeclineIconWhite
    {
      get => AssetStore.LoadVoipIcon("icon_decline.png", true);
    }

    public static BitmapSource CallScreenEndCallIconWhite
    {
      get => AssetStore.LoadVoipIcon("icon_call-end.png", true);
    }

    public static BitmapSource CallScreenCancelInvitationIconWhite
    {
      get => AssetStore.LoadVoipIcon("icon_call-end.png", true);
    }

    public static BitmapSource CallScreenMessageIconWhite
    {
      get => AssetStore.LoadVoipIcon("icon_message.png", true);
    }

    public static BitmapSource CallScreenMuteIconWhite
    {
      get => AssetStore.LoadVoipIcon("icon_mute.png", true);
    }

    public static BitmapSource CallScreenSpeakerIconWhite
    {
      get => AssetStore.LoadVoipIcon("icon_speaker.png", true);
    }

    public static BitmapSource CallScreenVideoOnIconWhite
    {
      get => AssetStore.LoadVoipIcon("icon_video.png", true);
    }

    public static BitmapSource CallScreenVideoOffIconWhite
    {
      get => AssetStore.LoadVoipIcon("icon_video_off.png", true);
    }

    public static System.Windows.Media.ImageSource KeypadCancelIcon
    {
      get
      {
        return (System.Windows.Media.ImageSource) AssetStore.LoadCached(ref AssetStore.keypadCancelIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("keypad-cancel.png", AssetStore.ThemeSetting.Light)), false);
      }
    }

    public static System.Windows.Media.ImageSource KeypadGifIcon
    {
      get
      {
        return (System.Windows.Media.ImageSource) AssetStore.LoadCached(ref AssetStore.keypadGifIcon, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("keypad-gif.png")), false);
      }
    }

    public static System.Windows.Media.ImageSource StickerThumbnailPlaceholder
    {
      get
      {
        return (System.Windows.Media.ImageSource) AssetStore.LoadCached(ref AssetStore.stickerThumbnailPlaceholder, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("sticker-error.png")), false);
      }
    }

    public static BitmapSource StickerRecentIcon(AssetStore.ThemeSetting requested)
    {
      BitmapSource cached = AssetStore.stickerRecentIcon;
      if (requested == AssetStore.ThemeSetting.Dark || requested == AssetStore.ThemeSetting.ToBeDetermined && ImageStore.IsDarkTheme())
        cached = AssetStore.stickerRecentIconDark;
      return AssetStore.LoadCached(ref cached, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("sticker-recent.png", requested)), false);
    }

    public static BitmapSource StickerSavedIcon(AssetStore.ThemeSetting requested)
    {
      BitmapSource cached = AssetStore.stickerSavedIcon;
      if (requested == AssetStore.ThemeSetting.Dark || requested == AssetStore.ThemeSetting.ToBeDetermined && ImageStore.IsDarkTheme())
        cached = AssetStore.stickerSavedIconDark;
      return AssetStore.LoadCached(ref cached, (Func<BitmapSource>) (() => (BitmapSource) AssetStore.LoadAsset("sticker-saved.png", requested)), false);
    }

    public static BitmapSource PaymentBackground
    {
      get => (BitmapSource) AssetStore.LoadAsset("payment-bg.png", AssetStore.ThemeSetting.NoTheme);
    }

    public static BitmapSource SecurityPadlockIcon
    {
      get => (BitmapSource) AssetStore.LoadAsset("security-settings-padlock.png");
    }

    public static BitmapSource GdprReportIcon
    {
      get => (BitmapSource) AssetStore.LoadAsset("gdpr-report-icon.png");
    }

    public static BitmapSource GdprTosBackground
    {
      get
      {
        return (BitmapSource) AssetStore.LoadAsset("gdpr-tos-bg.png", AssetStore.ThemeSetting.NoTheme);
      }
    }

    public static BitmapSource GetDefaultChatIcon(string jid)
    {
      BitmapSource defaultChatIcon = (BitmapSource) null;
      switch (JidHelper.GetJidType(jid))
      {
        case JidHelper.JidTypes.User:
          defaultChatIcon = AssetStore.DefaultContactIcon;
          break;
        case JidHelper.JidTypes.Group:
          defaultChatIcon = AssetStore.DefaultGroupIcon;
          break;
      }
      return defaultChatIcon;
    }

    public static BitmapSource GetMessageDefaultIcon(FunXMPP.FMessage.Type mediaType)
    {
      BitmapSource messageDefaultIcon = (BitmapSource) null;
      switch (mediaType)
      {
        case FunXMPP.FMessage.Type.Image:
        case FunXMPP.FMessage.Type.Gif:
        case FunXMPP.FMessage.Type.Sticker:
          messageDefaultIcon = AssetStore.PictureButtonIcon;
          break;
        case FunXMPP.FMessage.Type.Audio:
          messageDefaultIcon = AssetStore.AudioButtonIcon;
          break;
        case FunXMPP.FMessage.Type.Video:
          messageDefaultIcon = AssetStore.VideoButtonIcon;
          break;
        case FunXMPP.FMessage.Type.Contact:
          messageDefaultIcon = AssetStore.ContactButtonIcon;
          break;
        case FunXMPP.FMessage.Type.Location:
          messageDefaultIcon = AssetStore.LocationButtonIcon;
          break;
        case FunXMPP.FMessage.Type.LiveLocation:
          messageDefaultIcon = AssetStore.LocationButtonIcon;
          break;
      }
      return messageDefaultIcon;
    }

    public enum ThemeSetting
    {
      ToBeDetermined,
      NoTheme,
      Dark,
      Light,
    }
  }
}
