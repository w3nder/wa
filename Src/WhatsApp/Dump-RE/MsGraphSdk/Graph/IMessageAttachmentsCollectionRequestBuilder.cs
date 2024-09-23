// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IMessageAttachmentsCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public interface IMessageAttachmentsCollectionRequestBuilder
  {
    IMessageAttachmentsCollectionRequest Request();

    IMessageAttachmentsCollectionRequest Request(IEnumerable<Option> options);

    IAttachmentRequestBuilder this[string id] { get; }
  }
}
