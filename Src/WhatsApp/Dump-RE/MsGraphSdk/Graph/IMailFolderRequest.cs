// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IMailFolderRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IMailFolderRequest : IBaseRequest
  {
    Task<MailFolder> CreateAsync(MailFolder mailFolderToCreate);

    Task<MailFolder> CreateAsync(MailFolder mailFolderToCreate, CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<MailFolder> GetAsync();

    Task<MailFolder> GetAsync(CancellationToken cancellationToken);

    Task<MailFolder> UpdateAsync(MailFolder mailFolderToUpdate);

    Task<MailFolder> UpdateAsync(MailFolder mailFolderToUpdate, CancellationToken cancellationToken);

    IMailFolderRequest Expand(string value);

    IMailFolderRequest Select(string value);
  }
}
