// Decompiled with JetBrains decompiler
// Type: WhatsApp.StickerManager
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class StickerManager
  {
    private static string SERVER_ENDPOINT = "https://static.whatsapp.net/sticker?";
    private static string ALL = "cat=all";
    private static string IMG = "img=";
    private static string LogHeader = nameof (StickerManager);
    private bool processing;
    private bool complete;
    private static StickerManager stickerManagerInstance;
    private static object createLock = new object();
    private List<StickerPack> stickerPacks;
    private static WorkQueue requestStickerPacksThread;

    public StickerRequestInfo RequestInfo { get; set; }

    public List<StickerPack> StickerPacks
    {
      get => this.stickerPacks ?? (this.stickerPacks = new List<StickerPack>());
      set => this.stickerPacks = value;
    }

    public static StickerManager GetInstance()
    {
      if (StickerManager.stickerManagerInstance == null)
      {
        lock (StickerManager.createLock)
        {
          if (StickerManager.stickerManagerInstance == null)
            StickerManager.stickerManagerInstance = new StickerManager();
        }
      }
      return StickerManager.stickerManagerInstance;
    }

    public IEnumerable<StickerPack> GetStickerPacks()
    {
      if (this.complete)
        return (IEnumerable<StickerPack>) SqliteStickerPacks.GetSavedStickerPacks();
      if (!this.processing)
        this.TryDownloadStickerPacks();
      return Enumerable.Empty<StickerPack>();
    }

    private static WorkQueue RequestStickerPacksThread
    {
      get
      {
        return Utils.LazyInit<WorkQueue>(ref StickerManager.requestStickerPacksThread, (Func<WorkQueue>) (() => new WorkQueue()));
      }
    }

    public void TryDownloadStickerPacks()
    {
      if (this.processing)
        return;
      StickerManager.RequestStickerPacksThread.Enqueue((Action) (() =>
      {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        dictionary.Add("Content-Type", "application/json");
        StringBuilder stringBuilder = new StringBuilder();
        foreach (KeyValuePair<string, string> keyValuePair in dictionary)
        {
          if (stringBuilder.Length > 0)
            stringBuilder.Append("\r\n");
          stringBuilder.Append(string.Format("{0}: {1}", (object) keyValuePair.Key, (object) string.Join(" ", new string[1]
          {
            keyValuePair.Value
          })));
        }
        string headers = stringBuilder.ToString();
        StickerManager.GetStickerPacks(StickerManager.SERVER_ENDPOINT + StickerManager.ALL, headers).Subscribe<StickerManager.StickerPacksDownloadDetails>(new Action<StickerManager.StickerPacksDownloadDetails>(this.OnStickerPacksDownloadDetails), (Action<Exception>) (ex => { }), (Action) (() =>
        {
          this.processing = false;
          this.complete = true;
        }));
      }));
    }

    private void OnStickerPacksDownloadDetails(
      StickerManager.StickerPacksDownloadDetails downloadDetails)
    {
      int responseCode = downloadDetails.ResponseCode;
      string responseEncoding = downloadDetails.ResponseEncoding;
      string etag = downloadDetails.ETag;
      Stream stickerPacksStream = (Stream) downloadDetails.StickerPacksStream;
      Log.l(StickerManager.LogHeader, "processing response {0}, {1}, {2} {3}", (object) responseCode, (object) (responseEncoding ?? "null"), (object) (etag ?? "null"), (object) (stickerPacksStream != null));
      if (responseCode == 304 || responseCode == 404 || responseCode != 200 || stickerPacksStream == null)
        return;
      if (stickerPacksStream.Length == 0L)
        return;
      try
      {
        stickerPacksStream.Position = 0L;
        List<StickerPack> fromJsonStream = StickerManager.CreateFromJsonStream<List<StickerPack>>(stickerPacksStream);
        if (fromJsonStream.Count <= 0)
        {
          ArgumentException e = new ArgumentException("Unexpected null data");
          Log.SendCrashLog((Exception) e, "no useful data returned from sticker pack request", logOnlyForRelease: true);
          throw e;
        }
        foreach (StickerPack stickerPack in fromJsonStream)
          this.StickerPacks.Add(stickerPack);
      }
      catch (Exception ex)
      {
      }
    }

    private static T CreateFromJsonStream<T>(Stream stream)
    {
      JsonSerializer jsonSerializer = new JsonSerializer();
      using (stream)
      {
        using (StreamReader reader = new StreamReader(stream))
          return (T) jsonSerializer.Deserialize((TextReader) reader, typeof (T));
      }
    }

    private static IObservable<StickerManager.StickerPacksDownloadDetails> GetStickerPacks(
      string url,
      string headers)
    {
      return NativeWeb.Create<StickerManager.StickerPacksDownloadDetails>(NativeWeb.Options.Default | NativeWeb.Options.KeepAlive, (Action<IWebRequest, IObserver<StickerManager.StickerPacksDownloadDetails>>) ((req, observer) =>
      {
        MemoryStream mem = new MemoryStream();
        int responseCode = -1;
        bool onCompletedFired = false;
        string responseETag = (string) null;
        string responseEncoding = (string) null;
        IWebRequest req1 = req;
        string url1 = url;
        NativeWeb.Callback callbackObject = new NativeWeb.Callback();
        callbackObject.OnBeginResponse = (Action<int, string>) ((code, returnedHeaders) =>
        {
          responseCode = code;
          foreach (KeyValuePair<string, string> header in NativeWeb.ParseHeaders(headers))
          {
            string key = header.Key;
            if (key == "ETag")
              responseETag = header.Value;
            if (key == "Accept-Encoding")
              responseEncoding = header.Value;
          }
          switch (code)
          {
            case 200:
              break;
            case 304:
            case 404:
              observer.OnNext(new StickerManager.StickerPacksDownloadDetails()
              {
                ResponseCode = responseCode,
                ResponseEncoding = (string) null,
                ETag = responseETag,
                StickerPacksStream = (MemoryStream) null
              });
              observer.OnCompleted();
              onCompletedFired = true;
              break;
            default:
              observer.OnError(new Exception(string.Format("GetStickerPacks Unexpected response {0}", (object) code)));
              observer.OnCompleted();
              onCompletedFired = true;
              break;
          }
        });
        callbackObject.OnBytesIn = (Action<byte[]>) (b => mem.Write(b, 0, b.Length));
        callbackObject.OnEndResponse = (Action) (() =>
        {
          if (!onCompletedFired)
          {
            mem.Position = 0L;
            observer.OnNext(new StickerManager.StickerPacksDownloadDetails()
            {
              ResponseCode = responseCode,
              ResponseEncoding = responseEncoding,
              ETag = responseETag,
              StickerPacksStream = mem
            });
            observer.OnCompleted();
          }
          else
            mem.SafeDispose();
        });
        string headers1 = headers;
        req1.Open(url1, (IWebCallback) callbackObject, headers: headers1);
      }));
    }

    public void TryDownloadPreviewsForPack(StickerPack stickerPack)
    {
      if (this.processing)
        return;
      StickerManager.RequestStickerPacksThread.Enqueue((Action) (() =>
      {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        dictionary.Add("Content-Type", "image/png");
        StringBuilder stringBuilder = new StringBuilder();
        foreach (KeyValuePair<string, string> keyValuePair in dictionary)
        {
          if (stringBuilder.Length > 0)
            stringBuilder.Append("\r\n");
          stringBuilder.Append(string.Format("{0}: {1}", (object) keyValuePair.Key, (object) string.Join(" ", new string[1]
          {
            keyValuePair.Value
          })));
        }
        string headers = stringBuilder.ToString();
        this.GetStickerPreview(StickerManager.SERVER_ENDPOINT + StickerManager.IMG + stickerPack.TrayImageId, headers).Subscribe<StickerManager.StickerPreviewDownloadDetails>((Action<StickerManager.StickerPreviewDownloadDetails>) (details => this.OnStickerPacksDownloadDetails(details, stickerPack)), (Action<Exception>) (ex => { }), (Action) (() =>
        {
          this.processing = false;
          this.complete = true;
        }));
      }));
    }

    private void OnStickerPacksDownloadDetails(
      StickerManager.StickerPreviewDownloadDetails downloadDetails,
      StickerPack stickerPack)
    {
      int responseCode = downloadDetails.ResponseCode;
      string responseEncoding = downloadDetails.ResponseEncoding;
      byte[] previewDataBytes = downloadDetails.PreviewDataBytes;
      Log.l(StickerManager.LogHeader, "processing response {0}, {1}, {2}", (object) responseCode, (object) (responseEncoding ?? "null"), (object) (previewDataBytes != null));
      if (responseCode == 304 || responseCode == 404 || responseCode != 200 || previewDataBytes == null)
        return;
      if (previewDataBytes.Length == 0)
        return;
      try
      {
        StickerManager.SavePreviewFile(stickerPack, previewDataBytes);
      }
      catch (Exception ex)
      {
      }
    }

    private IObservable<StickerManager.StickerPreviewDownloadDetails> GetStickerPreview(
      string url,
      string headers)
    {
      return NativeWeb.Create<StickerManager.StickerPreviewDownloadDetails>(NativeWeb.Options.Default | NativeWeb.Options.KeepAlive, (Action<IWebRequest, IObserver<StickerManager.StickerPreviewDownloadDetails>>) ((req, observer) =>
      {
        MemoryStream mem = new MemoryStream();
        int responseCode = -1;
        bool onCompletedFired = false;
        string responseEncoding = (string) null;
        IWebRequest req1 = req;
        string url1 = url;
        NativeWeb.Callback callbackObject = new NativeWeb.Callback();
        callbackObject.OnBeginResponse = (Action<int, string>) ((code, returnedHeaders) =>
        {
          responseCode = code;
          foreach (KeyValuePair<string, string> header in NativeWeb.ParseHeaders(headers))
          {
            if (header.Key == "Accept-Encoding")
              responseEncoding = header.Value;
          }
          switch (code)
          {
            case 200:
              break;
            case 304:
            case 404:
              observer.OnNext(new StickerManager.StickerPreviewDownloadDetails()
              {
                ResponseCode = responseCode,
                ResponseEncoding = (string) null,
                PreviewDataBytes = (byte[]) null
              });
              observer.OnCompleted();
              onCompletedFired = true;
              break;
            default:
              observer.OnError(new Exception(string.Format("GetStickerPacks Unexpected response {0}", (object) code)));
              observer.OnCompleted();
              onCompletedFired = true;
              break;
          }
        });
        callbackObject.OnBytesIn = (Action<byte[]>) (b => mem.Write(b, 0, b.Length));
        callbackObject.OnEndResponse = (Action) (() =>
        {
          if (!onCompletedFired)
          {
            mem.Position = 0L;
            using (BinaryReader binaryReader = new BinaryReader((Stream) mem))
            {
              byte[] numArray = binaryReader.ReadBytes((int) mem.Length);
              observer.OnNext(new StickerManager.StickerPreviewDownloadDetails()
              {
                ResponseCode = responseCode,
                ResponseEncoding = responseEncoding,
                PreviewDataBytes = numArray
              });
            }
            observer.OnCompleted();
          }
          else
            mem.SafeDispose();
        });
        string headers1 = headers;
        req1.Open(url1, (IWebCallback) callbackObject, headers: headers1);
      }));
    }

    public static void SavePreviewFile(StickerPack stickerPack, byte[] data)
    {
      SqliteStickerPacks.SaveStickerPreviewImage(stickerPack, data);
    }

    private class StickerPacksDownloadDetails
    {
      public int ResponseCode = -1;
      public string ETag;
      public string ResponseEncoding;
      public MemoryStream StickerPacksStream;
    }

    private class StickerPreviewDownloadDetails
    {
      public int ResponseCode = -1;
      public string ResponseEncoding;
      public byte[] PreviewDataBytes;
    }
  }
}
