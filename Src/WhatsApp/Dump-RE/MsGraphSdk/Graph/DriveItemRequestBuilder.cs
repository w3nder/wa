// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DriveItemRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class DriveItemRequestBuilder : 
    EntityRequestBuilder,
    IDriveItemRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public IDriveItemRequestBuilder ItemWithPath(string path)
    {
      if (!string.IsNullOrEmpty(path) && !path.StartsWith("/"))
        path = string.Format("/{0}", (object) path);
      return (IDriveItemRequestBuilder) new DriveItemRequestBuilder(string.Format("{0}:{1}:", (object) this.RequestUrl, (object) path), this.Client);
    }

    public DriveItemRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IDriveItemRequest Request() => this.Request((IEnumerable<Option>) null);

    public IDriveItemRequest Request(IEnumerable<Option> options)
    {
      return (IDriveItemRequest) new DriveItemRequest(this.RequestUrl, this.Client, options);
    }

    public IUserWithReferenceRequestBuilder CreatedByUser
    {
      get
      {
        return (IUserWithReferenceRequestBuilder) new UserWithReferenceRequestBuilder(this.AppendSegmentToRequestUrl("createdByUser"), this.Client);
      }
    }

    public IUserWithReferenceRequestBuilder LastModifiedByUser
    {
      get
      {
        return (IUserWithReferenceRequestBuilder) new UserWithReferenceRequestBuilder(this.AppendSegmentToRequestUrl("lastModifiedByUser"), this.Client);
      }
    }

    public IDriveItemPermissionsCollectionRequestBuilder Permissions
    {
      get
      {
        return (IDriveItemPermissionsCollectionRequestBuilder) new DriveItemPermissionsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("permissions"), this.Client);
      }
    }

    public IDriveItemChildrenCollectionRequestBuilder Children
    {
      get
      {
        return (IDriveItemChildrenCollectionRequestBuilder) new DriveItemChildrenCollectionRequestBuilder(this.AppendSegmentToRequestUrl("children"), this.Client);
      }
    }

    public IDriveItemThumbnailsCollectionRequestBuilder Thumbnails
    {
      get
      {
        return (IDriveItemThumbnailsCollectionRequestBuilder) new DriveItemThumbnailsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("thumbnails"), this.Client);
      }
    }

    public IDriveItemContentRequestBuilder Content
    {
      get
      {
        return (IDriveItemContentRequestBuilder) new DriveItemContentRequestBuilder(this.AppendSegmentToRequestUrl("content"), this.Client);
      }
    }

    public IDriveItemCreateLinkRequestBuilder CreateLink(string type, string scope = null)
    {
      return (IDriveItemCreateLinkRequestBuilder) new DriveItemCreateLinkRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.createLink"), this.Client, type, scope);
    }

    public IDriveItemCopyRequestBuilder Copy(string name = null, ItemReference parentReference = null)
    {
      return (IDriveItemCopyRequestBuilder) new DriveItemCopyRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.copy"), this.Client, name, parentReference);
    }

    public IDriveItemSearchRequestBuilder Search(string q = null)
    {
      return (IDriveItemSearchRequestBuilder) new DriveItemSearchRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.search"), this.Client, q);
    }

    public IDriveItemDeltaRequestBuilder Delta(string token = null)
    {
      return (IDriveItemDeltaRequestBuilder) new DriveItemDeltaRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.delta"), this.Client, token);
    }

    public IDriveItemDeltaRequestBuilder Delta()
    {
      return (IDriveItemDeltaRequestBuilder) new DriveItemDeltaRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.delta"), this.Client);
    }
  }
}
