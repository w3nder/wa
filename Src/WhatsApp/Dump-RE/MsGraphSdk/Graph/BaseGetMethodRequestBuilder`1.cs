// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.BaseGetMethodRequestBuilder`1
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace Microsoft.Graph
{
  public abstract class BaseGetMethodRequestBuilder<T> : BaseRequestBuilder where T : IBaseRequest
  {
    private List<string> _parameters = new List<string>();
    private List<QueryOption> _queryOptions = new List<QueryOption>();
    protected bool passParametersInQueryString;

    public BaseGetMethodRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    protected abstract T CreateRequest(string functionUrl, IEnumerable<Option> options);

    public T Request(IEnumerable<Option> options = null)
    {
      string functionUrl = this.RequestUrl;
      if (this.passParametersInQueryString && this._queryOptions.Count > 0)
      {
        if (options == null)
          options = (IEnumerable<Option>) this._queryOptions;
        else
          options.ToList<Option>().AddRange((IEnumerable<Option>) this._queryOptions);
      }
      else if (!this.passParametersInQueryString && this._parameters.Count > 0)
        functionUrl = string.Format("{0}({1})", (object) functionUrl, (object) string.Join(",", (IEnumerable<string>) this._parameters));
      return this.CreateRequest(functionUrl, options);
    }

    protected void SetParameter(string name, object value, bool nullable)
    {
      if (value == null && !nullable)
        throw new ServiceException(new Error()
        {
          Code = "invalidRequest",
          Message = string.Format("{0} is a required parameter for this method request.", (object) name)
        });
      if (this.passParametersInQueryString && value != null)
      {
        this._queryOptions.Add(new QueryOption(name, value.ToString()));
      }
      else
      {
        if (this.passParametersInQueryString)
          return;
        string str = value != null ? value.ToString() : "null";
        if (value != null && value is string)
          str = "'" + this.EscapeStringValue(str) + "'";
        this._parameters.Add(string.Format("{0}={1}", (object) name, (object) str));
      }
    }

    private string EscapeStringValue(string value) => value.Replace("'", "''");
  }
}
