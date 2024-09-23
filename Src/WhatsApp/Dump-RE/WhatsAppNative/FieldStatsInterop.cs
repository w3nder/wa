// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.FieldStatsInterop
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [MarshalingBehavior]
  [Version(100794368)]
  [Activatable(100794368)]
  public sealed class FieldStatsInterop : IFieldStats
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern FieldStatsInterop();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetAttributeDouble([In] int idx, [In] double value);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetAttributeString([In] int idx, [In] string str);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetAttributeNull([In] int idx);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void MaybeSerializeDouble([In] int idx, [In] double value);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void MaybeSerializeString([In] int idx, [In] string str);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SaveEvent([In] uint Code, [In] uint Weight, [In] IAction serialize);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Start([In] IFSConfig Config);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Stop();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool IsReadyToSend();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IByteBuffer LoadFile();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SendAck();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void RotateBuffer();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SubmitVoipRating([In] byte[] Cookie, [In] int Rating, [In] string Description);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SubmitVoipNullRating([In] byte[] Cookie);
  }
}
