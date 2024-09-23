// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.Camera
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class Camera : WamEvent
  {
    public long? cameraPresentationT;
    public long? cameraActionPhotoT;
    public long? cameraActionVideoStartT;
    public long? cameraActionVideoEndT;
    public wam_enum_camera_origin? cameraOrigin;
    public wam_enum_camera_export_media_type? cameraExportMediaType;

    public void Reset()
    {
      this.cameraPresentationT = new long?();
      this.cameraActionPhotoT = new long?();
      this.cameraActionVideoStartT = new long?();
      this.cameraActionVideoEndT = new long?();
      this.cameraOrigin = new wam_enum_camera_origin?();
      this.cameraExportMediaType = new wam_enum_camera_export_media_type?();
    }

    public override uint GetCode() => 1536;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.cameraPresentationT);
      Wam.MaybeSerializeField(2, this.cameraActionPhotoT);
      Wam.MaybeSerializeField(3, this.cameraActionVideoStartT);
      Wam.MaybeSerializeField(4, this.cameraActionVideoEndT);
      Wam.MaybeSerializeField(5, Wam.EnumToLong<wam_enum_camera_origin>(this.cameraOrigin));
      Wam.MaybeSerializeField(6, Wam.EnumToLong<wam_enum_camera_export_media_type>(this.cameraExportMediaType));
    }
  }
}
