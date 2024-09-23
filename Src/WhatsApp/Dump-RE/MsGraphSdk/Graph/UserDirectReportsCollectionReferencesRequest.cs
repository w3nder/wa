// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserDirectReportsCollectionReferencesRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class UserDirectReportsCollectionReferencesRequest : 
    BaseRequest,
    IUserDirectReportsCollectionReferencesRequest,
    IBaseRequest
  {
    public UserDirectReportsCollectionReferencesRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task AddAsync(DirectoryObject directoryObject)
    {
      return this.AddAsync(directoryObject, CancellationToken.None);
    }

    public Task AddAsync(DirectoryObject directoryObject, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "POST";
      if (string.IsNullOrEmpty(directoryObject.Id))
        throw new ServiceException(new Error()
        {
          Code = "invalidRequest",
          Message = "ID is required to add a reference."
        });
      return this.SendAsync((object) new ReferenceRequestBody()
      {
        ODataId = string.Format("{0}/directoryObjects/{1}", (object) this.Client.BaseUrl, (object) directoryObject.Id)
      }, cancellationToken);
    }
  }
}
