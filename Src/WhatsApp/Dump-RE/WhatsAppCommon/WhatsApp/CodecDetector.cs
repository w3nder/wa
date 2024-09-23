// Decompiled with JetBrains decompiler
// Type: WhatsApp.CodecDetector
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class CodecDetector
  {
    public static string GetVideoMimeType(string path)
    {
      switch (CodecDetector.DetectMp4Codecs(path).Container)
      {
        case MediaContainerType.IsoMp4:
          return "video/mp4";
        case MediaContainerType.Iso3gp:
          return "video/3gpp";
        case MediaContainerType.QuickTime:
          return "video/quicktime";
        case MediaContainerType.Ogg:
          return "video/ogg";
        default:
          return (string) null;
      }
    }

    public static CodecInfo GetCodecInfo(string path, bool checkDecoderPresent = true)
    {
      MediaType mediaType = CodecDetector.DetectMp4Codecs(path);
      if (mediaType.VideoStreamType == Mp4VideoStreamType.HEVC)
        mediaType.VideoStreamType = Mp4VideoStreamType.Unknown;
      switch (mediaType.AudioStreamType)
      {
        case Mp4AudioStreamType.DolbyEac3:
        case Mp4AudioStreamType.MultipleAudioTracks:
          mediaType.AudioStreamType = Mp4AudioStreamType.Unknown;
          break;
      }
      CodecInfo codecInfo = mediaType.Container == MediaContainerType.Unknown || mediaType.VideoStreamType == Mp4VideoStreamType.Unknown || mediaType.AudioStreamType == Mp4AudioStreamType.Unknown ? CodecInfo.NeedsTranscode : CodecInfo.SupportedCodec;
      if (codecInfo == CodecInfo.SupportedCodec)
      {
        switch (mediaType.Container)
        {
          case MediaContainerType.IsoMp4:
          case MediaContainerType.Iso3gp:
            break;
          case MediaContainerType.QuickTime:
            codecInfo = CodecInfo.NeedsRemux;
            break;
          default:
            codecInfo = CodecInfo.NeedsRemux;
            break;
        }
      }
      if (((IEnumerable<MediaContainerType>) new MediaContainerType[3]
      {
        MediaContainerType.IsoMp4,
        MediaContainerType.Iso3gp,
        MediaContainerType.QuickTime
      }).Contains<MediaContainerType>(mediaType.Container) && mediaType.VideoStreamType == Mp4VideoStreamType.NotFound && mediaType.AudioStreamType == Mp4AudioStreamType.NotFound)
        codecInfo = CodecInfo.Unsupported;
      if (codecInfo == CodecInfo.NeedsTranscode & checkDecoderPresent)
      {
        bool flag = true;
        Log.l("codec info", "Trying media decoders to confirm transcode will work");
        try
        {
          if (mediaType.Container == MediaContainerType.Unknown || mediaType.VideoStreamType != Mp4VideoStreamType.NotFound)
          {
            using (new VideoFrameGrabber(path))
              ;
          }
          if (mediaType.Container != MediaContainerType.Unknown)
          {
            if (mediaType.AudioStreamType == Mp4AudioStreamType.NotFound)
              goto label_33;
          }
          try
          {
            string absolutePath = MediaStorage.GetAbsolutePath(path);
            using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
            {
              using (NativeStream nativeStream = (NativeStream) nativeMediaStorage.OpenFile(absolutePath, FileMode.Open, FileAccess.Read))
                Marshal.ReleaseComObject((object) NativeInterfaces.Misc.CreateSoundSource(SoundPlaybackCodec.MediaFoundation, nativeStream.GetNative()));
            }
          }
          catch (Exception ex)
          {
            if (mediaType.Container != MediaContainerType.Unknown)
              throw;
          }
        }
        catch (Exception ex)
        {
          Log.l("codec info", "Codec lib hit exception: {0}", (object) ex.GetSynopsis());
          flag = false;
        }
label_33:
        if (!flag)
          codecInfo = CodecInfo.Unsupported;
      }
      Log.l("codec info", "returning {0}", (object) codecInfo.ToString());
      return codecInfo;
    }

    public static MediaType DetectMp4Codecs(string path, bool sanitize = true)
    {
      MediaType streamInformation = NativeInterfaces.Mp4Utils.ExtractStreamInformation(sanitize ? MediaStorage.GetAbsolutePath(path) : path);
      Log.l("codec info", "container={0}, vcodec={1}, acodec={2}, subtype={3}", (object) streamInformation.Container.ToString(), (object) streamInformation.VideoStreamType.ToString(), (object) streamInformation.AudioStreamType.ToString(), (object) streamInformation.AudioSubtype.ToString());
      return streamInformation;
    }

    public static CodecDetector.AudioCodecResult DetectAudioCodec(string path)
    {
      using (IMediaStorage mediaStorage = MediaStorage.Create(path))
      {
        using (Stream str = mediaStorage.OpenFile(path))
          return CodecDetector.DetectAudioCodec(str);
      }
    }

    public static CodecDetector.AudioCodecResult DetectAudioCodec(Stream str)
    {
      List<CodecDetector.Detectoid> source = new List<CodecDetector.Detectoid>();
      byte[] oggMagic = Encoding.UTF8.GetBytes("OggS");
      byte[] opusMagic = Encoding.UTF8.GetBytes("OpusHead");
      source.Add(new CodecDetector.Detectoid()
      {
        BytesRequired = 282 + opusMagic.Length,
        CodecDriver = SoundPlaybackCodec.OpusFile,
        Detect = (Func<byte[], bool>) (buffer =>
        {
          for (int index = 0; index < oggMagic.Length; ++index)
          {
            if ((int) oggMagic[index] != (int) buffer[index])
              return false;
          }
          int num = (int) buffer[26] + 27;
          for (int index = 0; index < opusMagic.Length; ++index)
          {
            if ((int) opusMagic[index] != (int) buffer[num + index])
              return false;
          }
          return true;
        }),
        MimeType = "audio/ogg; codecs=opus"
      });
      byte[] amrMagic = Encoding.UTF8.GetBytes("#!AMR");
      source.Add(new CodecDetector.Detectoid()
      {
        BytesRequired = amrMagic.Length,
        CodecDriversPlural = new SoundPlaybackCodec[2]
        {
          SoundPlaybackCodec.MediaFoundation,
          SoundPlaybackCodec.Amr
        },
        Detect = (Func<byte[], bool>) (buffer =>
        {
          for (int index = 0; index < amrMagic.Length; ++index)
          {
            if ((int) amrMagic[index] != (int) buffer[index])
              return false;
          }
          return true;
        }),
        MimeType = "audio/amr"
      });
      source.Add(new CodecDetector.Detectoid()
      {
        BytesRequired = 0,
        CodecDriver = SoundPlaybackCodec.MediaFoundation,
        Detect = (Func<byte[], bool>) (b => true)
      });
      long position = str.Position;
      byte[] bytes = new byte[source.Select<CodecDetector.Detectoid, int>((Func<CodecDetector.Detectoid, int>) (d => d.BytesRequired)).Max()];
      int bytesIn = str.Read(bytes, 0, bytes.Length);
      str.Position = position;
      CodecDetector.Detectoid detectoid = source.Where<CodecDetector.Detectoid>((Func<CodecDetector.Detectoid, bool>) (d => bytesIn >= d.BytesRequired && d.Detect(bytes))).FirstOrDefault<CodecDetector.Detectoid>();
      CodecDetector.AudioCodecResult audioCodecResult = new CodecDetector.AudioCodecResult();
      if (detectoid != null)
      {
        audioCodecResult.Codecs = detectoid.CodecDriversPlural;
        audioCodecResult.MimeType = detectoid.MimeType;
        if (audioCodecResult.MimeType == null)
          audioCodecResult.MimeType = CodecDetector.GuessAudioMimeType(str, bytes, bytesIn);
      }
      else
        audioCodecResult.Codecs = new SoundPlaybackCodec[0];
      return audioCodecResult;
    }

    private static string GuessAudioMimeType(Stream str, byte[] bytes, int bytesIn)
    {
      if (bytesIn > 2 && bytes[0] == byte.MaxValue && ((int) bytes[1] & 224) == 224)
        return ((int) bytes[1] & 6) == 0 ? "audio/aac" : "audio/mpeg";
      if (bytesIn > 12 && bytes[4] == (byte) 102 && bytes[5] == (byte) 116 && bytes[6] == (byte) 121 && bytes[7] == (byte) 112 && bytes[8] == (byte) 77 && bytes[9] == (byte) 52 && bytes[10] == (byte) 65)
        return "audio/mp4";
      if (bytes[0] == (byte) 73 && bytes[1] == (byte) 68 && bytes[2] == (byte) 51)
      {
        uint num = (uint) ((int) bytes[9] & (int) sbyte.MaxValue | ((int) bytes[8] & (int) sbyte.MaxValue) << 7 | ((int) bytes[7] & (int) sbyte.MaxValue) << 14 | ((int) bytes[6] & (int) sbyte.MaxValue) << 21);
        long position = str.Position;
        try
        {
          str.Position = position + 10L + (long) num;
          bytesIn = str.Read(bytes, 0, bytes.Length);
        }
        finally
        {
          str.Position = position;
        }
        if (CodecDetector.IsMp3Packet(bytes, bytesIn))
          return "audio/mpeg";
      }
      return (string) null;
    }

    private static bool IsMp3Packet(byte[] bytes, int bytesIn)
    {
      return bytesIn > 8 && bytes[0] == byte.MaxValue && ((int) bytes[1] & 224) == 224;
    }

    private class Detectoid
    {
      public int BytesRequired;
      public Func<byte[], bool> Detect;
      public string MimeType;

      public SoundPlaybackCodec CodecDriver
      {
        get
        {
          return ((IEnumerable<SoundPlaybackCodec>) this.CodecDriversPlural).FirstOrDefault<SoundPlaybackCodec>();
        }
        set
        {
          this.CodecDriversPlural = new SoundPlaybackCodec[1]
          {
            value
          };
        }
      }

      public SoundPlaybackCodec[] CodecDriversPlural { get; set; }
    }

    public struct AudioCodecResult
    {
      public SoundPlaybackCodec[] Codecs;
      public string MimeType;
    }
  }
}
