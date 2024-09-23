// Decompiled with JetBrains decompiler
// Type: WhatsApp.Mp4UtilsExtensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WhatsAppNative;


namespace WhatsApp
{
  public static class Mp4UtilsExtensions
  {
    public static Mp4MappedStream MapStream(this IMp4Utils utils, Stream stream)
    {
      return new Mp4MappedStream()
      {
        Id = new int?(utils.MapStream(stream.ToWaStream()))
      };
    }

    public static Mp4MappedStream MapStream(this IMp4Utils utils, string filename, Stream stream)
    {
      Mp4MappedStream mp4MappedStream = new Mp4MappedStream();
      mp4MappedStream.Filename = filename;
      utils.MapNamedStream(filename, stream.ToWaStream());
      return mp4MappedStream;
    }

    public static void CheckAndRepair(
      this IMp4Utils utils,
      Message m,
      ref string fileName,
      string newFileName,
      bool downloadScenario,
      List<string> oldFiles,
      List<Action<Message>> messageModifications,
      bool recomputeHash = true)
    {
      bool flag1 = false;
      switch (m.MediaWaType)
      {
        case FunXMPP.FMessage.Type.Audio:
        case FunXMPP.FMessage.Type.Video:
        case FunXMPP.FMessage.Type.Gif:
          try
          {
            flag1 = ((IEnumerable<MediaContainerType>) new MediaContainerType[3]
            {
              MediaContainerType.Iso3gp,
              MediaContainerType.IsoMp4,
              MediaContainerType.QuickTime
            }).Contains<MediaContainerType>(CodecDetector.DetectMp4Codecs(fileName).Container);
            break;
          }
          catch (Exception ex)
          {
            break;
          }
      }
      if (flag1)
      {
        bool flag2 = false;
        bool preliminaryStageCompleted = false;
        try
        {
          flag2 = utils.CheckAndRepair(MediaStorage.GetAbsolutePath(fileName), MediaStorage.GetAbsolutePath(newFileName), downloadScenario, true, ((Action) (() => preliminaryStageCompleted = true)).AsComAction()).RepairOccurred && !downloadScenario;
        }
        catch (Exception ex1)
        {
          if (utils.IsRecoverableError(ex1))
          {
            throw;
          }
          else
          {
            try
            {
              string fileNameSnap = fileName;
              Log.CreateForensicsFile(ex, (Action<Stream>) (forensicsStream =>
              {
                using (Mp4MappedStream mp4MappedStream = utils.MapStream(forensicsStream))
                  utils.DumpForensics(MediaStorage.GetAbsolutePath(fileNameSnap), ex, mp4MappedStream.Filename);
              }));
            }
            catch (Exception ex2)
            {
              Log.LogException(ex2, "forensics");
            }
            bool flag3 = true;
            if (downloadScenario & preliminaryStageCompleted)
              flag3 = false;
            if (flag3)
              throw new CheckAndRepairException("Check and repair failed", ex1);
          }
        }
        if (!flag2)
        {
          Log.l("check&repair", "Did not need repair.");
          oldFiles.Add(newFileName);
          recomputeHash = false;
        }
        else
        {
          Log.l("check&repair", "File was modified.");
          oldFiles.Add(fileName);
          fileName = newFileName;
        }
        if (!recomputeHash)
          return;
        using (Stream stream = MediaStorage.OpenFile(fileName))
        {
          byte[] newHash = MediaUpload.ComputeHash(stream);
          long lengthSnap = stream.Length;
          messageModifications.Add((Action<Message>) (m2 =>
          {
            m2.MediaHash = newHash;
            m2.MediaSize = lengthSnap;
            MessagesContext.Run((MessagesContext.MessagesCallback) (db => m2.ClearCipherMediaHash(db)));
          }));
        }
      }
      else
        Log.l("check&repair", "Not an MP4 file.");
    }

    public static IDisposable AddOpenCallback(this IMp4Utils utils, Func<string, Stream> callback)
    {
      return utils.AddOpenCallback((IMp4UtilsOpenCallback) new Mp4UtilsExtensions.OpenCallbackWrapper(callback));
    }

    public static Mp4UtilsMetadata GetStreamMetadata(this IMp4Utils utils, string path)
    {
      Mp4UtilsExtensions.Mp4UtilsMetadataReceiver metadata = new Mp4UtilsExtensions.Mp4UtilsMetadataReceiver();
      utils.GetStreamMetadata(path, (IMp4UtilsMetadataReceiver) metadata);
      return metadata.Result;
    }

    private class OpenCallbackWrapper : IMp4UtilsOpenCallback
    {
      private Func<string, Stream> callback;

      public OpenCallbackWrapper(Func<string, Stream> callback) => this.callback = callback;

      public IWAStream Open(string file)
      {
        Stream source = this.callback(file);
        return source != null ? source.ToWaStream() : (IWAStream) null;
      }
    }

    private class Mp4UtilsMetadataReceiver : IMp4UtilsMetadataReceiver
    {
      public Mp4UtilsMetadata Result;

      public void OnAudioMetadata(
        int sampleRate,
        int channels,
        int bitsPerSample,
        int bytesPerSecond,
        int blockAlign,
        float duration)
      {
        this.Result.Audio = new Mp4UtilsAudioMetdata?(new Mp4UtilsAudioMetdata()
        {
          SampleRate = sampleRate,
          Channels = channels,
          BitsPerSample = bitsPerSample,
          BytesPerSecond = bytesPerSecond,
          BlockAlign = blockAlign,
          Duration = duration
        });
      }

      public void OnVideoMetadata(
        int width,
        int height,
        float fps,
        float duration,
        int rotationAngle)
      {
        this.Result.Video = new Mp4UtilsVideoMetdata?(new Mp4UtilsVideoMetdata()
        {
          Width = width,
          Height = height,
          Fps = fps,
          Duration = duration,
          RotationAngle = rotationAngle
        });
      }

      public void OnMetadataComplete()
      {
      }
    }
  }
}
