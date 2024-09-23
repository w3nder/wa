// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.Misc
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
  public sealed class Misc : IMisc
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern Misc();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IByteBuffer GetToken([In] IByteBuffer a);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern CELL_INFO GetCellInfo([In] CellInfoFlags flags);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern string GetString([In] int idx);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern string DumpLog();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern uint GetProcessId();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern uint GetThreadId();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetBackground();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Log([In] string String, [In] bool NoWrite);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetLogUserConnection([In] ConnectionType connection);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetLogSink([In] ILogSink LogSink);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetCancelEvent([In] string EventName);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void DiscardLog();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SubmitLog([In] object stream);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern float GetVolume();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern ulong GetTickCount();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void RemoveDirectoryRecursiveImpl([In] string Path);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void LowerPriority([In] IAction action);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IAction SetAudioSink([In] object AudioDevice, [In] ISampleSink sink);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern INativeMediaStorage GetFilesystem();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern DiskSpace GetDiskSpace([In] string Path);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern string GetAppInstallDir();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void LaunchSession([In] string Url, [In] ICallbackResult Callback);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool IsBackgroundDataDisabled();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool IsBackgroundDataDisabledNearLimit();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void OpenRegLog();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void CloseRegLog();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void LogStackInfo();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IVoip GetVoipInstance();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetVoipFactory([In] IVoipFactory Fac);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetFieldStatsCallbacks([In] IFieldStatsManaged callbacks);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern ISoundSource CreateSoundSource([In] SoundPlaybackCodec codec, [In] IWAStream stream);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern string NormalizeUnicodeString([In] string Input, [In] bool Expand);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void EnumerateFilesInRange(
      [In] string Path,
      [In] long StartTime,
      [In] long EndTime,
      [In] IStringEnumerationCallback cb);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SquelchLogs_AddRef();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SquelchLogs_Release();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern string ComErrorToWinRtString([In] HResult Error);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IClosable ProximitySensorLcdSubscribe();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern object WaStreamToWinRtStream([In] IWAStream stream);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern string IdnToAscii([In] uint Flags, [In] string InputString);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern string IdnToUnicode([In] uint Flags, [In] string InputString);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern long GetFileSizeFast([In] string FilePath);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IClosable RegisterShellButtonCallback(
      [In] ShellButton buttonType,
      [In] IShellButtonCallback callbackObj);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetLogInfo([In] string LogInfo);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetVoipExperiment([In] string Experiment);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetIcuDirectory([In] string icuDirectory);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern string FormatLongIcu([In] string localeCode, [In] long number);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern string FormatDateTimeIcu([In] string localeCode, [In] string skeleton, [In] long dateTimeUtcSec);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern string FormatCurrencyIcu(
      [In] string localeCode,
      [In] string currencyCode,
      [In] long amount_x_1000);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void GetMemoryUsage(out long CurrentUsage, out long PeakUsage, out long Maximum);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern JpegEncoderResult EncodeJpeg(
      [In] IByteBuffer Buffer,
      [In] JpegEncoderParams @params,
      [In] IJpegWriteCallback writer);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern int UniformRandom([In] int n);
  }
}
