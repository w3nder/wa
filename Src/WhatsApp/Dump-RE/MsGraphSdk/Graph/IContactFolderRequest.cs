// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IContactFolderRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IContactFolderRequest : IBaseRequest
  {
    Task<ContactFolder> CreateAsync(ContactFolder contactFolderToCreate);

    Task<ContactFolder> CreateAsync(
      ContactFolder contactFolderToCreate,
      CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<ContactFolder> GetAsync();

    Task<ContactFolder> GetAsync(CancellationToken cancellationToken);

    Task<ContactFolder> UpdateAsync(ContactFolder contactFolderToUpdate);

    Task<ContactFolder> UpdateAsync(
      ContactFolder contactFolderToUpdate,
      CancellationToken cancellationToken);

    IContactFolderRequest Expand(string value);

    IContactFolderRequest Select(string value);
  }
}
