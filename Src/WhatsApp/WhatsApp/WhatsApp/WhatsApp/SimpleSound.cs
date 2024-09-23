// Decompiled with JetBrains decompiler
// Type: WhatsApp.SimpleSound
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using WhatsAppNative;


namespace WhatsApp
{
  public class SimpleSound : SimpleSoundBase<SoundEffect>
  {
    private SoundEffectInstance instance;
    private IDisposable instanceSub;
    private static Dictionary<string, RefCountedObject<SoundEffectInstance>> instanceCache;
    private static object instanceCacheLock;

    public SimpleSound(string path, float volume = 1f)
      : base(path, volume)
    {
    }

    protected override SoundEffect LoadObject()
    {
      Stream stream = (Stream) null;
      try
      {
        NativeStream nativeStream = (NativeStream) (stream = AppState.OpenFromXAP(this.path));
        if (!this.path.EndsWith(".wav", StringComparison.InvariantCultureIgnoreCase))
        {
          ISoundSource soundSource = NativeInterfaces.Misc.CreateSoundSource((IEnumerable<SoundPlaybackCodec>) CodecDetector.DetectAudioCodec(stream).Codecs, nativeStream.GetNative());
          AudioMetadata metadata = soundSource.GetMetadata();
          byte[] numArray = new byte[(int) (soundSource.GetDuration() * (long) metadata.SampleRate * (long) metadata.Channels * (long) metadata.BitsPerSample / 8L / 1000L)];
          IByteBuffer instance = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
          instance.Put(numArray);
          soundSource.FillBuffer(instance);
          instance.Reset();
          stream.Dispose();
          stream = (Stream) new MemoryStream();
          new WavWriter(stream, metadata.SampleRate, (Channels) metadata.Channels, (ushort) metadata.BitsPerSample).AddSamples(numArray);
          stream.Position = 0L;
        }
        return SoundEffect.FromStream(stream);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "load sound instance");
      }
      finally
      {
        stream.SafeDispose();
      }
      return (SoundEffect) null;
    }

    private void TryGetCachedInstance()
    {
      RefCountedObject<SoundEffectInstance> refCountedObject;
      SimpleSound.instanceCache.TryGetValue(this.path, out refCountedObject);
      if (refCountedObject == null)
        return;
      this.instanceSub = refCountedObject.Get(out this.instance);
    }

    public override void Play()
    {
      if (this.instance == null)
      {
        Utils.LazyInit<Dictionary<string, RefCountedObject<SoundEffectInstance>>>(ref SimpleSound.instanceCache, (Func<Dictionary<string, RefCountedObject<SoundEffectInstance>>>) (() => new Dictionary<string, RefCountedObject<SoundEffectInstance>>()));
        Utils.LazyInit<object>(ref SimpleSound.instanceCacheLock, (Func<object>) (() => new object()));
        lock (SimpleSound.instanceCacheLock)
          this.TryGetCachedInstance();
        this.Load();
        if (this.effect != null)
        {
          if (this.instance == null)
          {
            lock (SimpleSound.instanceCacheLock)
            {
              this.TryGetCachedInstance();
              if (this.instance == null)
              {
                RefCountedObject<SoundEffectInstance> refCountedObject = new RefCountedObject<SoundEffectInstance>((Action<SoundEffectInstance>) (d => d.Dispose()));
                refCountedObject.Set(this.effect.CreateInstance());
                this.instanceSub = refCountedObject.Get(out this.instance);
                SimpleSound.instanceCache[this.path] = refCountedObject;
              }
            }
          }
          if (this.instance != null)
            this.instance.Volume = this.volume;
        }
        if (this.instance == null)
          return;
      }
      try
      {
        FrameworkDispatcher.Update();
      }
      catch
      {
      }
      this.instance.Play();
    }

    protected override void DisposeManagedResources()
    {
      base.DisposeManagedResources();
      if (this.instance != null)
      {
        lock (SimpleSound.instanceCacheLock)
        {
          RefCountedObject<SoundEffectInstance> refCountedObject;
          SimpleSound.instanceCache.TryGetValue(this.path, out refCountedObject);
          refCountedObject?.Set((SoundEffectInstance) null);
        }
      }
      this.instanceSub.SafeDispose();
    }
  }
}
