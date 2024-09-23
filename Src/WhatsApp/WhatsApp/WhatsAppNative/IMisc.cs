// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IMisc
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(1403299506, 16973, 16792, 136, 123, 158, 141, 104, 252, 169, 194)]
  public interface IMisc
  {
    IByteBuffer GetToken([In] IByteBuffer a);

    CELL_INFO GetCellInfo([In] CellInfoFlags flags);

    string GetString([In] int idx);

    string DumpLog();

    uint GetProcessId();

    uint GetThreadId();

    void SetBackground();

    void Log([In] string String, [In] bool NoWrite);

    void SetLogUserConnection([In] ConnectionType connection);

    void SetLogSink([In] ILogSink LogSink);

    void SetCancelEvent([In] string EventName);

    void DiscardLog();

    void SubmitLog([In] object stream);

    float GetVolume();

    ulong GetTickCount();

    void RemoveDirectoryRecursiveImpl([In] string Path);

    void LowerPriority([In] IAction action);

    IAction SetAudioSink([In] object AudioDevice, [In] ISampleSink sink);

    INativeMediaStorage GetFilesystem();

    DiskSpace GetDiskSpace([In] string Path);

    string GetAppInstallDir();

    void LaunchSession([In] string Url, [In] ICallbackResult Callback);

    bool IsBackgroundDataDisabled();

    bool IsBackgroundDataDisabledNearLimit();

    void OpenRegLog();

    void CloseRegLog();

    void LogStackInfo();

    IVoip GetVoipInstance();

    void SetVoipFactory([In] IVoipFactory Fac);

    void SetFieldStatsCallbacks([In] IFieldStatsManaged callbacks);

    ISoundSource CreateSoundSource([In] SoundPlaybackCodec codec, [In] IWAStream stream);

    string NormalizeUnicodeString([In] string Input, [In] bool Expand);

    void EnumerateFilesInRange(
      [In] string Path,
      [In] long StartTime,
      [In] long EndTime,
      [In] IStringEnumerationCallback cb);

    void SquelchLogs_AddRef();

    void SquelchLogs_Release();

    string ComErrorToWinRtString([In] HResult Error);

    IClosable ProximitySensorLcdSubscribe();

    object WaStreamToWinRtStream([In] IWAStream stream);

    string IdnToAscii([In] uint Flags, [In] string InputString);

    string IdnToUnicode([In] uint Flags, [In] string InputString);

    long GetFileSizeFast([In] string FilePath);

    IClosable RegisterShellButtonCallback([In] ShellButton buttonType, [In] IShellButtonCallback callbackObj);

    void SetLogInfo([In] string LogInfo);

    void SetVoipExperiment([In] string Experiment);

    void SetIcuDirectory([In] string icuDirectory);

    string FormatLongIcu([In] string localeCode, [In] long number);

    string FormatDateTimeIcu([In] string localeCode, [In] string skeleton, [In] long dateTimeUtcSec);

    string FormatCurrencyIcu([In] string localeCode, [In] string currencyCode, [In] long amount_x_1000);

    void GetMemoryUsage(out long CurrentUsage, out long PeakUsage, out long Maximum);

    JpegEncoderResult EncodeJpeg(
      [In] IByteBuffer Buffer,
      [In] JpegEncoderParams @params,
      [In] IJpegWriteCallback writer);

    int UniformRandom([In] int n);
  }
}
