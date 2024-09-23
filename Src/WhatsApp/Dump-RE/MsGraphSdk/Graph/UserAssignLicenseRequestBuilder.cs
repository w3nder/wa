// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserAssignLicenseRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class UserAssignLicenseRequestBuilder : 
    BasePostMethodRequestBuilder<IUserAssignLicenseRequest>,
    IUserAssignLicenseRequestBuilder
  {
    public UserAssignLicenseRequestBuilder(
      string requestUrl,
      IBaseClient client,
      IEnumerable<AssignedLicense> addLicenses,
      IEnumerable<Guid> removeLicenses)
      : base(requestUrl, client)
    {
      this.SetParameter<IEnumerable<AssignedLicense>>(nameof (addLicenses), addLicenses, false);
      this.SetParameter<IEnumerable<Guid>>(nameof (removeLicenses), removeLicenses, false);
    }

    protected override IUserAssignLicenseRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      UserAssignLicenseRequest request = new UserAssignLicenseRequest(functionUrl, this.Client, options);
      if (this.HasParameter("addLicenses"))
        request.RequestBody.AddLicenses = this.GetParameter<IEnumerable<AssignedLicense>>("addLicenses");
      if (this.HasParameter("removeLicenses"))
        request.RequestBody.RemoveLicenses = this.GetParameter<IEnumerable<Guid>>("removeLicenses");
      return (IUserAssignLicenseRequest) request;
    }
  }
}
