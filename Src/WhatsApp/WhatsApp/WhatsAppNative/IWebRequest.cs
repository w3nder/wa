// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IWebRequest
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(3460879174, 63016, 18878, 157, 246, 99, 145, 217, 147, 164, 181)]
  public interface IWebRequest
  {
    void OpenImpl(
      [In] string Url,
      [In] string Method,
      [In] string UserAgent,
      [In] string Headers,
      [In] IWebCallback Callback);

    void Dispose();

    void Cancel();

    void SetCertificatePinning([In] bool Enabled);

    void SetResolver([In] IHostResolver Resolver);

    void SetIgnoreCertificateRevocations([In] bool Ignore);

    void SetHandleRedirects([In] bool HandleRedirects);

    void SetTrustCache([In] bool TrustCache);

    string GetDefaultUserAgent();

    void SetKeepAlive([In] bool Enabled);

    void SetTryOriginalHost([In] bool Enabled);

    void ClearIpCache();
  }
}
