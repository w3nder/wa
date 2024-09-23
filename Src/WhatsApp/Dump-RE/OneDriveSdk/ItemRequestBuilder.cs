// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ItemRequestBuilder
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public class ItemRequestBuilder : BaseRequestBuilder, IItemRequestBuilder, IBaseRequestBuilder
  {
    public IItemRequestBuilder ItemWithPath(string path)
    {
      if (!string.IsNullOrEmpty(path) && !path.StartsWith("/"))
        path = string.Format("/{0}", (object) path);
      return (IItemRequestBuilder) new ItemRequestBuilder(string.Format("{0}:{1}:", (object) this.RequestUrl, (object) path), this.Client);
    }

    public ItemRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IItemRequest Request() => this.Request((IEnumerable<Option>) null);

    public IItemRequest Request(IEnumerable<Option> options)
    {
      return (IItemRequest) new ItemRequest(this.RequestUrl, this.Client, options);
    }

    public IItemPermissionsCollectionRequestBuilder Permissions
    {
      get
      {
        return (IItemPermissionsCollectionRequestBuilder) new ItemPermissionsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("permissions"), this.Client);
      }
    }

    public IItemVersionsCollectionRequestBuilder Versions
    {
      get
      {
        return (IItemVersionsCollectionRequestBuilder) new ItemVersionsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("versions"), this.Client);
      }
    }

    public IItemChildrenCollectionRequestBuilder Children
    {
      get
      {
        return (IItemChildrenCollectionRequestBuilder) new ItemChildrenCollectionRequestBuilder(this.AppendSegmentToRequestUrl("children"), this.Client);
      }
    }

    public IItemThumbnailsCollectionRequestBuilder Thumbnails
    {
      get
      {
        return (IItemThumbnailsCollectionRequestBuilder) new ItemThumbnailsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("thumbnails"), this.Client);
      }
    }

    public IItemContentRequestBuilder Content
    {
      get
      {
        return (IItemContentRequestBuilder) new ItemContentRequestBuilder(this.AppendSegmentToRequestUrl("content"), this.Client);
      }
    }

    public IItemCreateSessionRequestBuilder CreateSession(ChunkedUploadSessionDescriptor item = null)
    {
      return (IItemCreateSessionRequestBuilder) new ItemCreateSessionRequestBuilder(this.AppendSegmentToRequestUrl("oneDrive.createSession"), this.Client, item);
    }

    public IItemCopyRequestBuilder Copy(string name = null, ItemReference parentReference = null)
    {
      return (IItemCopyRequestBuilder) new ItemCopyRequestBuilder(this.AppendSegmentToRequestUrl("oneDrive.copy"), this.Client, name, parentReference);
    }

    public IItemCreateLinkRequestBuilder CreateLink(string type)
    {
      return (IItemCreateLinkRequestBuilder) new ItemCreateLinkRequestBuilder(this.AppendSegmentToRequestUrl("oneDrive.createLink"), this.Client, type);
    }

    public IItemDeltaRequestBuilder Delta(string token = null)
    {
      return (IItemDeltaRequestBuilder) new ItemDeltaRequestBuilder(this.AppendSegmentToRequestUrl("oneDrive.delta"), this.Client, token);
    }

    public IItemSearchRequestBuilder Search(string q = null)
    {
      return (IItemSearchRequestBuilder) new ItemSearchRequestBuilder(this.AppendSegmentToRequestUrl("oneDrive.search"), this.Client, q);
    }
  }
}
