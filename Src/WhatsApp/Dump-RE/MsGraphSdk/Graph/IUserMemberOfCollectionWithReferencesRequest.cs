﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IUserMemberOfCollectionWithReferencesRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IUserMemberOfCollectionWithReferencesRequest : IBaseRequest
  {
    Task<IUserMemberOfCollectionWithReferencesPage> GetAsync();

    Task<IUserMemberOfCollectionWithReferencesPage> GetAsync(CancellationToken cancellationToken);

    IUserMemberOfCollectionWithReferencesRequest Top(int value);

    IUserMemberOfCollectionWithReferencesRequest OrderBy(string value);
  }
}
