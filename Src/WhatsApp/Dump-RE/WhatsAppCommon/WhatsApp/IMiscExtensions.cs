// Decompiled with JetBrains decompiler
// Type: WhatsApp.IMiscExtensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public static class IMiscExtensions
  {
    public static CELL_INFO GetCellInfo(this IMisc misc) => misc.GetCellInfo(CellInfoFlags.All);

    public static ISoundSource CreateSoundSource(
      this IMisc misc,
      IEnumerable<SoundPlaybackCodec> codecs,
      IWAStream stream)
    {
      return Utils.TakeFirstNonThrowing<SoundPlaybackCodec, ISoundSource>(codecs, (Func<SoundPlaybackCodec, ISoundSource>) (codec =>
      {
        Log.l("SoundSource", "trying decoder: {0}", (object) codec.ToString());
        try
        {
          stream.Seek(0L, 0U);
          return misc.CreateSoundSource(codec, stream);
        }
        catch (Exception ex)
        {
          string context = string.Format("SoundSource > decoder[{0}]", (object) codec.ToString());
          Log.LogException(ex, context);
          throw;
        }
      }));
    }

    public static void EnumerateFilesInRange(
      this IMisc misc,
      string path,
      DateTime? dtMin,
      DateTime? dtMax,
      Action<string> callback)
    {
      misc.EnumerateFilesInRange(path, dtMin.HasValue ? dtMin.GetValueOrDefault().ToFileTimeUtc() : 0L, dtMax.HasValue ? dtMax.GetValueOrDefault().ToFileTimeUtc() : 0L, (IStringEnumerationCallback) new IMiscExtensions.StringEnumerationCallback(callback));
    }

    public static IDisposable SquelchLogs(this IMisc misc)
    {
      misc.SquelchLogs_AddRef();
      return (IDisposable) new DisposableAction(new Action(misc.SquelchLogs_Release));
    }

    public static void RemoveDirectoryRecursive(
      this IMisc misc,
      string path,
      bool swallowError = false,
      bool renameFirst = false,
      string renameTarget = null)
    {
      Action action = (Action) (() =>
      {
        if (renameFirst || renameTarget != null)
        {
          renameTarget = renameTarget != null ? MediaStorage.GetAbsolutePath(path) : path + "_d" + DateTime.UtcNow.ToUnixTime().ToString();
          using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
          {
            try
            {
              nativeMediaStorage.MoveFile(path, renameTarget);
              path = renameTarget;
            }
            catch (Exception ex)
            {
              Log.LogException(ex, "failed to rename target out of the way");
            }
          }
        }
        misc.RemoveDirectoryRecursiveImpl(path);
      });
      if (swallowError)
      {
        try
        {
          using (misc.SquelchLogs())
            action();
        }
        catch (Exception ex)
        {
        }
      }
      else
        action();
    }

    private class StringEnumerationCallback : IStringEnumerationCallback
    {
      private Action<string> onNext;

      public StringEnumerationCallback(Action<string> onNext) => this.onNext = onNext;

      public void OnNext(string s) => this.onNext(s);
    }
  }
}
