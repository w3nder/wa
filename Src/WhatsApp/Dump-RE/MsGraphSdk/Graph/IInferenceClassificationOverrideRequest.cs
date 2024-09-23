// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IInferenceClassificationOverrideRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IInferenceClassificationOverrideRequest : IBaseRequest
  {
    Task<InferenceClassificationOverride> CreateAsync(
      InferenceClassificationOverride inferenceClassificationOverrideToCreate);

    Task<InferenceClassificationOverride> CreateAsync(
      InferenceClassificationOverride inferenceClassificationOverrideToCreate,
      CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<InferenceClassificationOverride> GetAsync();

    Task<InferenceClassificationOverride> GetAsync(CancellationToken cancellationToken);

    Task<InferenceClassificationOverride> UpdateAsync(
      InferenceClassificationOverride inferenceClassificationOverrideToUpdate);

    Task<InferenceClassificationOverride> UpdateAsync(
      InferenceClassificationOverride inferenceClassificationOverrideToUpdate,
      CancellationToken cancellationToken);

    IInferenceClassificationOverrideRequest Expand(string value);

    IInferenceClassificationOverrideRequest Select(string value);
  }
}
