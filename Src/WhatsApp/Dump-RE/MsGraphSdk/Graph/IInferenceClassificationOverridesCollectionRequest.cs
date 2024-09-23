// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IInferenceClassificationOverridesCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IInferenceClassificationOverridesCollectionRequest : IBaseRequest
  {
    Task<InferenceClassificationOverride> AddAsync(
      InferenceClassificationOverride inferenceClassificationOverride);

    Task<InferenceClassificationOverride> AddAsync(
      InferenceClassificationOverride inferenceClassificationOverride,
      CancellationToken cancellationToken);

    Task<IInferenceClassificationOverridesCollectionPage> GetAsync();

    Task<IInferenceClassificationOverridesCollectionPage> GetAsync(
      CancellationToken cancellationToken);

    IInferenceClassificationOverridesCollectionRequest Expand(string value);

    IInferenceClassificationOverridesCollectionRequest Select(string value);

    IInferenceClassificationOverridesCollectionRequest Top(int value);

    IInferenceClassificationOverridesCollectionRequest Filter(string value);

    IInferenceClassificationOverridesCollectionRequest Skip(int value);

    IInferenceClassificationOverridesCollectionRequest OrderBy(string value);
  }
}
