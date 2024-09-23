// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IMp4Utils
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(3007013376, 27014, 17347, 189, 155, 32, 40, 21, 87, 10, 137)]
  public interface IMp4Utils
  {
    int MapStream([In] IWAStream stream);

    void UnmapStream([In] int integer);

    void MapNamedStream([In] string Path, [In] IWAStream stream);

    void UnmapNamedStream([In] string Path);

    MediaType ExtractStreamInformation([In] string Path);

    void TrimMp4File([In] string InputPath, [In] string OutputPath, [In] long DesiredSize);

    void ExtractAVStreams([In] string InputFilename, [In] string OutputDirectory);

    void ExtractAVStreamsForStreaming(
      [In] string InputFilename,
      [In] float fps,
      [In] IMp4UtilsMetadataReceiver metadata,
      [In] IMp4UtilsWriteCallback videoWriter,
      [In] IMp4UtilsWriteCallback audioWriter);

    void MuxAVStreams(
      [In] string AudioFilename,
      [In] string VideoFilename,
      [In] string OutputFilename,
      [In] float StartTime,
      [In] float Duration,
      [In] float TargetFramerate);

    void GetStreamMetadata([In] string Filename, [In] IMp4UtilsMetadataReceiver metadata);

    ulong[] GetEditPoints([In] string Filename);

    CheckAndRepairResult CheckAndRepair(
      [In] string InputFilename,
      [In] string OutputFilename,
      [In] bool DownloadScenario,
      [In] bool BetaMode,
      [In] IAction OnPreliminaryCheckCompleted);

    bool IsRecoverableError([In] HResult hr);

    void DumpForensics([In] string FileName, [In] HResult ErrorCode, [In] string OutputFileName);

    void TagWaAnimatedGif([In] string InputFile, [In] string OutputFile);

    bool IsWaAnimGif([In] string InputFile);

    IMp4TrackRemover OpenTrackRemover([In] string InputFile);

    IClosable AddOpenCallback([In] IMp4UtilsOpenCallback cb);

    void DeleteThreadLocalStorage();
  }
}
