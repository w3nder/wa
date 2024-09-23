// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IContactFolderRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public interface IContactFolderRequestBuilder : IEntityRequestBuilder, IBaseRequestBuilder
  {
    IContactFolderRequest Request();

    IContactFolderRequest Request(IEnumerable<Option> options);

    IContactFolderContactsCollectionRequestBuilder Contacts { get; }

    IContactFolderChildFoldersCollectionRequestBuilder ChildFolders { get; }
  }
}
