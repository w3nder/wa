// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IDriveItemRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public interface IDriveItemRequestBuilder : IEntityRequestBuilder, IBaseRequestBuilder
  {
    IDriveItemRequestBuilder ItemWithPath(string path);

    IDriveItemRequest Request();

    IDriveItemRequest Request(IEnumerable<Option> options);

    IUserWithReferenceRequestBuilder CreatedByUser { get; }

    IUserWithReferenceRequestBuilder LastModifiedByUser { get; }

    IDriveItemPermissionsCollectionRequestBuilder Permissions { get; }

    IDriveItemChildrenCollectionRequestBuilder Children { get; }

    IDriveItemThumbnailsCollectionRequestBuilder Thumbnails { get; }

    IDriveItemContentRequestBuilder Content { get; }

    IDriveItemCreateLinkRequestBuilder CreateLink(string type, string scope = null);

    IDriveItemCopyRequestBuilder Copy(string name = null, ItemReference parentReference = null);

    IDriveItemSearchRequestBuilder Search(string q = null);

    IDriveItemDeltaRequestBuilder Delta(string token = null);

    IDriveItemDeltaRequestBuilder Delta();
  }
}
