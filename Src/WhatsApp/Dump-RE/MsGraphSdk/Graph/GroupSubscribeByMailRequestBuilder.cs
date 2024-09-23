// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GroupSubscribeByMailRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class GroupSubscribeByMailRequestBuilder : 
    BaseGetMethodRequestBuilder<IGroupSubscribeByMailRequest>,
    IGroupSubscribeByMailRequestBuilder
  {
    public GroupSubscribeByMailRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    protected override IGroupSubscribeByMailRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      return (IGroupSubscribeByMailRequest) new GroupSubscribeByMailRequest(functionUrl, this.Client, options);
    }
  }
}
