// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.DocumentDetection
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class DocumentDetection : WamEvent
  {
    public long? algorithmT;
    public long? systemAlgorithmT;
    public long? decodeBitmapT;
    public bool? isImageLikeDocument;
    public bool? isImageLikeDocumentBySystem;
    public bool? isImageLikeDocumentByWa;
    public double? documentImageMaxEdge;
    public double? documentImageQuality;
    public double? imageWidthForAlgorithm;
    public double? imageHeightForAlgorithm;
    public double? imageWidthForSending;
    public double? imageHeightForSending;
    public double? originalImageWidth;
    public double? originalImageHeight;
    public double? marginX;
    public double? marginY;
    public wam_enum_algorithm_type? algorithmType;

    public void Reset()
    {
      this.algorithmT = new long?();
      this.systemAlgorithmT = new long?();
      this.decodeBitmapT = new long?();
      this.isImageLikeDocument = new bool?();
      this.isImageLikeDocumentBySystem = new bool?();
      this.isImageLikeDocumentByWa = new bool?();
      this.documentImageMaxEdge = new double?();
      this.documentImageQuality = new double?();
      this.imageWidthForAlgorithm = new double?();
      this.imageHeightForAlgorithm = new double?();
      this.imageWidthForSending = new double?();
      this.imageHeightForSending = new double?();
      this.originalImageWidth = new double?();
      this.originalImageHeight = new double?();
      this.marginX = new double?();
      this.marginY = new double?();
      this.algorithmType = new wam_enum_algorithm_type?();
    }

    public override uint GetCode() => 1436;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.algorithmT);
      Wam.MaybeSerializeField(14, this.systemAlgorithmT);
      Wam.MaybeSerializeField(2, this.decodeBitmapT);
      Wam.MaybeSerializeField(3, this.isImageLikeDocument);
      Wam.MaybeSerializeField(15, this.isImageLikeDocumentBySystem);
      Wam.MaybeSerializeField(16, this.isImageLikeDocumentByWa);
      Wam.MaybeSerializeField(4, this.documentImageMaxEdge);
      Wam.MaybeSerializeField(5, this.documentImageQuality);
      Wam.MaybeSerializeField(6, this.imageWidthForAlgorithm);
      Wam.MaybeSerializeField(7, this.imageHeightForAlgorithm);
      Wam.MaybeSerializeField(8, this.imageWidthForSending);
      Wam.MaybeSerializeField(9, this.imageHeightForSending);
      Wam.MaybeSerializeField(10, this.originalImageWidth);
      Wam.MaybeSerializeField(11, this.originalImageHeight);
      Wam.MaybeSerializeField(12, this.marginX);
      Wam.MaybeSerializeField(13, this.marginY);
      Wam.MaybeSerializeField(17, Wam.EnumToLong<wam_enum_algorithm_type>(this.algorithmType));
    }
  }
}
