// Decompiled with JetBrains decompiler
// Type: WhatsApp.DocumentMessageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public class DocumentMessageViewModel : FileMessageViewModelBase
  {
    private DocumentMessageWrapper docMsg;

    public override string FooterInfoStr
    {
      get
      {
        int pageCount = this.docMsg.PageCount;
        string str1 = pageCount > 0 ? Plurals.Instance.GetString(AppResources.DocPagesPlural, pageCount) : (string) null;
        string fileExtension = this.docMsg.GetFileExtension();
        string str2 = Utils.FileSizeFormatter.Format(this.Message.MediaSize);
        return string.Join(string.Format("{0}•{1}", (object) ' ', (object) ' '), ((IEnumerable<string>) new string[3]
        {
          str1,
          fileExtension,
          str2
        }).Where<string>((Func<string, bool>) (s => !string.IsNullOrEmpty(s))));
      }
    }

    public override Thickness FooterMargin
    {
      get
      {
        return new Thickness(12.0 * this.zoomMultiplier, 0.0, 12.0 * this.zoomMultiplier, 6.0 * this.zoomMultiplier);
      }
    }

    public override Thickness ViewPanelMargin
    {
      get
      {
        double num = 12.0 * this.zoomMultiplier;
        return new Thickness(num, this.ViewPanelTopMargin, num, 32.0 * this.zoomMultiplier);
      }
    }

    public override bool ShouldShowActionButton
    {
      get => !this.Message.TransferInProgress && this.Message.LocalFileUri == null;
    }

    public override Thickness ForwardedRowMargin
    {
      get
      {
        return base.ForwardedRowMargin with
        {
          Bottom = this.Message.KeyFromMe ? -4.0 : 6.0
        };
      }
    }

    public override bool ShouldShowFooterInfo => true;

    public System.Windows.Media.ImageSource DocListIconSource => this.GetDocListTypeIcon();

    public System.Windows.Media.ImageSource DocListPreviewSource => (System.Windows.Media.ImageSource) null;

    public DocumentMessageViewModel(Message m)
      : base(m)
    {
      this.docMsg = new DocumentMessageWrapper(m);
    }

    protected override string GetTextStr() => this.docMsg.Title;

    protected override Size GetTargetThumbnailSizeImpl()
    {
      return new Size(MessageViewModel.DefaultContentWidth, MessageViewModel.DefaultContentWidth / 3.0);
    }

    protected override IObservable<WriteableBitmap> GenerateLargeThumbnailObservable(int targetWidth)
    {
      return base.GenerateLargeThumbnailObservable(targetWidth);
    }

    public override Set<string> GetTrackedProperties()
    {
      Set<string> trackedProperties = base.GetTrackedProperties();
      trackedProperties.Add("MediaSize");
      return trackedProperties;
    }

    protected override bool OnMessagePropertyChanged(string prop)
    {
      if (base.OnMessagePropertyChanged(prop))
        return true;
      bool flag = false;
      if (prop == "MediaSize")
      {
        this.Notify("FooterInfoChanged");
        flag = true;
      }
      return flag;
    }

    public System.Windows.Media.ImageSource GetDocumentTypeIcon()
    {
      System.Windows.Media.ImageSource imageSource = (System.Windows.Media.ImageSource) null;
      switch (this.Message.MediaMimeType)
      {
        case "application/msword":
        case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
          imageSource = (System.Windows.Media.ImageSource) AssetStore.DocTypeIconDoc;
          break;
        case "application/pdf":
          imageSource = (System.Windows.Media.ImageSource) AssetStore.DocTypeIconPdf;
          break;
        case "application/vnd.ms-excel":
        case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
          imageSource = (System.Windows.Media.ImageSource) AssetStore.DocTypeIconXls;
          break;
        case "application/vnd.ms-powerpoint":
        case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
          imageSource = (System.Windows.Media.ImageSource) AssetStore.DocTypeIconPpt;
          break;
      }
      return imageSource ?? (System.Windows.Media.ImageSource) AssetStore.DocTypeIconDefault;
    }

    public System.Windows.Media.ImageSource GetDocListTypeIcon()
    {
      System.Windows.Media.ImageSource imageSource = (System.Windows.Media.ImageSource) null;
      switch (this.Message.MediaMimeType)
      {
        case "application/msword":
        case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
          imageSource = (System.Windows.Media.ImageSource) AssetStore.DocListDocIcon;
          break;
        case "application/pdf":
          imageSource = (System.Windows.Media.ImageSource) AssetStore.DocListPdfIcon;
          break;
        case "application/vnd.ms-excel":
        case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
          imageSource = (System.Windows.Media.ImageSource) AssetStore.DocListXlsIcon;
          break;
        case "application/vnd.ms-powerpoint":
        case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
          imageSource = (System.Windows.Media.ImageSource) AssetStore.DocListPptIcon;
          break;
      }
      return imageSource ?? (System.Windows.Media.ImageSource) AssetStore.DocListFileIcon;
    }
  }
}
