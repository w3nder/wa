// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.WebRequest
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Activatable(100794368)]
  [MarshalingBehavior]
  [Version(100794368)]
  public sealed class WebRequest : IWebRequest, IWebWriter
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern WebRequest();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void OpenImpl(
      [In] string Url,
      [In] string Method,
      [In] string UserAgent,
      [In] string Headers,
      [In] IWebCallback Callback);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Dispose();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Cancel();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetCertificatePinning([In] bool Enabled);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetResolver([In] IHostResolver Resolver);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetIgnoreCertificateRevocations([In] bool Ignore);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetHandleRedirects([In] bool HandleRedirects);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetTrustCache([In] bool TrustCache);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern string GetDefaultUserAgent();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetKeepAlive([In] bool Enabled);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetTryOriginalHost([In] bool Enabled);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void ClearIpCache();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Write([In] IByteBuffer Buf);
  }
}
