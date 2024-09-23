// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IFieldStats
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Guid(3287938058, 30557, 18109, 176, 232, 67, 232, 200, 20, 24, 80)]
  [Version(100794368)]
  public interface IFieldStats
  {
    void SetAttributeDouble([In] int idx, [In] double value);

    void SetAttributeString([In] int idx, [In] string str);

    void SetAttributeNull([In] int idx);

    void MaybeSerializeDouble([In] int idx, [In] double value);

    void MaybeSerializeString([In] int idx, [In] string str);

    void SaveEvent([In] uint Code, [In] uint Weight, [In] IAction serialize);

    void Start([In] IFSConfig Config);

    void Stop();

    bool IsReadyToSend();

    IByteBuffer LoadFile();

    void SendAck();

    void RotateBuffer();

    void SubmitVoipRating([In] byte[] Cookie, [In] int Rating, [In] string Description);

    void SubmitVoipNullRating([In] byte[] Cookie);
  }
}
