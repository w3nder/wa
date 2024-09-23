// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.Mp4Utils
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [MarshalingBehavior]
  [Version(100794368)]
  [Activatable(100794368)]
  public sealed class Mp4Utils : IMp4Utils
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern Mp4Utils();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern int MapStream([In] IWAStream stream);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void UnmapStream([In] int integer);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void MapNamedStream([In] string Path, [In] IWAStream stream);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void UnmapNamedStream([In] string Path);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern MediaType ExtractStreamInformation([In] string Path);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void TrimMp4File([In] string InputPath, [In] string OutputPath, [In] long DesiredSize);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void ExtractAVStreams([In] string InputFilename, [In] string OutputDirectory);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void ExtractAVStreamsForStreaming(
      [In] string InputFilename,
      [In] float fps,
      [In] IMp4UtilsMetadataReceiver metadata,
      [In] IMp4UtilsWriteCallback videoWriter,
      [In] IMp4UtilsWriteCallback audioWriter);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void MuxAVStreams(
      [In] string AudioFilename,
      [In] string VideoFilename,
      [In] string OutputFilename,
      [In] float StartTime,
      [In] float Duration,
      [In] float TargetFramerate);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void GetStreamMetadata([In] string Filename, [In] IMp4UtilsMetadataReceiver metadata);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern ulong[] GetEditPoints([In] string Filename);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern CheckAndRepairResult CheckAndRepair(
      [In] string InputFilename,
      [In] string OutputFilename,
      [In] bool DownloadScenario,
      [In] bool BetaMode,
      [In] IAction OnPreliminaryCheckCompleted);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool IsRecoverableError([In] HResult hr);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void DumpForensics([In] string FileName, [In] HResult ErrorCode, [In] string OutputFileName);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void TagWaAnimatedGif([In] string InputFile, [In] string OutputFile);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool IsWaAnimGif([In] string InputFile);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IMp4TrackRemover OpenTrackRemover([In] string InputFile);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IClosable AddOpenCallback([In] IMp4UtilsOpenCallback cb);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void DeleteThreadLocalStorage();
  }
}
