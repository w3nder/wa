// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IInferenceClassificationRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IInferenceClassificationRequest : IBaseRequest
  {
    Task<InferenceClassification> CreateAsync(
      InferenceClassification inferenceClassificationToCreate);

    Task<InferenceClassification> CreateAsync(
      InferenceClassification inferenceClassificationToCreate,
      CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<InferenceClassification> GetAsync();

    Task<InferenceClassification> GetAsync(CancellationToken cancellationToken);

    Task<InferenceClassification> UpdateAsync(
      InferenceClassification inferenceClassificationToUpdate);

    Task<InferenceClassification> UpdateAsync(
      InferenceClassification inferenceClassificationToUpdate,
      CancellationToken cancellationToken);

    IInferenceClassificationRequest Expand(string value);

    IInferenceClassificationRequest Select(string value);
  }
}
