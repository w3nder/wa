// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.BasePostMethodRequestBuilder`1
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public abstract class BasePostMethodRequestBuilder<T> : BaseRequestBuilder where T : IBaseRequest
  {
    private Dictionary<string, object> _parameters = new Dictionary<string, object>();

    public BasePostMethodRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    protected abstract T CreateRequest(string functionUrl, IEnumerable<Option> options);

    public T Request(IEnumerable<Option> options = null)
    {
      return this.CreateRequest(this.RequestUrl, options);
    }

    protected void SetParameter<U>(string name, U value, bool nullable)
    {
      if ((object) value == null && !nullable)
        throw new ServiceException(new Error()
        {
          Code = "invalidRequest",
          Message = string.Format("{0} is a required parameter for this method request.", (object) name)
        });
      this._parameters.Add(name, (object) value);
    }

    protected bool HasParameter(string name) => this._parameters.ContainsKey(name);

    protected U GetParameter<U>(string name) => (U) this._parameters[name];
  }
}
