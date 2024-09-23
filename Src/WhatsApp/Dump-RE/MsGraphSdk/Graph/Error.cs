// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.Error
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

#nullable disable
namespace Microsoft.Graph
{
  [DataContract]
  public class Error
  {
    [DataMember(Name = "code", IsRequired = false, EmitDefaultValue = false)]
    public string Code { get; set; }

    [DataMember(Name = "innererror", IsRequired = false, EmitDefaultValue = false)]
    public Error InnerError { get; set; }

    [DataMember(Name = "message", IsRequired = false, EmitDefaultValue = false)]
    public string Message { get; set; }

    [DataMember(Name = "throwSite", IsRequired = false, EmitDefaultValue = false)]
    public string ThrowSite { get; set; }

    [JsonExtensionData(ReadData = true)]
    public IDictionary<string, object> AdditionalData { get; set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (!string.IsNullOrEmpty(this.Code))
      {
        stringBuilder.AppendFormat("Code: {0}", (object) this.Code);
        stringBuilder.Append(Environment.NewLine);
      }
      if (!string.IsNullOrEmpty(this.ThrowSite))
      {
        stringBuilder.AppendFormat("Throw site: {0}", (object) this.ThrowSite);
        stringBuilder.Append(Environment.NewLine);
      }
      if (!string.IsNullOrEmpty(this.Message))
      {
        stringBuilder.AppendFormat("Message: {0}", (object) this.Message);
        stringBuilder.Append(Environment.NewLine);
      }
      if (this.InnerError != null)
      {
        stringBuilder.Append(Environment.NewLine);
        stringBuilder.Append("Inner error");
        stringBuilder.Append(Environment.NewLine);
        stringBuilder.Append(this.InnerError.ToString());
      }
      return stringBuilder.ToString();
    }
  }
}
