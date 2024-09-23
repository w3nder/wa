// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserChangePasswordRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class UserChangePasswordRequestBuilder : 
    BasePostMethodRequestBuilder<IUserChangePasswordRequest>,
    IUserChangePasswordRequestBuilder
  {
    public UserChangePasswordRequestBuilder(
      string requestUrl,
      IBaseClient client,
      string currentPassword,
      string newPassword)
      : base(requestUrl, client)
    {
      this.SetParameter<string>(nameof (currentPassword), currentPassword, true);
      this.SetParameter<string>(nameof (newPassword), newPassword, true);
    }

    protected override IUserChangePasswordRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      UserChangePasswordRequest request = new UserChangePasswordRequest(functionUrl, this.Client, options);
      if (this.HasParameter("currentPassword"))
        request.RequestBody.CurrentPassword = this.GetParameter<string>("currentPassword");
      if (this.HasParameter("newPassword"))
        request.RequestBody.NewPassword = this.GetParameter<string>("newPassword");
      return (IUserChangePasswordRequest) request;
    }
  }
}
