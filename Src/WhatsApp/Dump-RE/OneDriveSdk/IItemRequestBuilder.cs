// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.IItemRequestBuilder
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public interface IItemRequestBuilder : IBaseRequestBuilder
  {
    IItemRequestBuilder ItemWithPath(string path);

    IItemRequest Request();

    IItemRequest Request(IEnumerable<Option> options);

    IItemPermissionsCollectionRequestBuilder Permissions { get; }

    IItemVersionsCollectionRequestBuilder Versions { get; }

    IItemChildrenCollectionRequestBuilder Children { get; }

    IItemThumbnailsCollectionRequestBuilder Thumbnails { get; }

    IItemContentRequestBuilder Content { get; }

    IItemCreateSessionRequestBuilder CreateSession(ChunkedUploadSessionDescriptor item = null);

    IItemCopyRequestBuilder Copy(string name = null, ItemReference parentReference = null);

    IItemCreateLinkRequestBuilder CreateLink(string type);

    IItemDeltaRequestBuilder Delta(string token = null);

    IItemSearchRequestBuilder Search(string q = null);
  }
}
