// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GraphServiceClient
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

#nullable disable
namespace Microsoft.Graph
{
  public class GraphServiceClient : BaseClient, IGraphServiceClient, IBaseClient
  {
    public GraphServiceClient(
      IAuthenticationProvider authenticationProvider,
      IHttpProvider httpProvider = null)
      : this("https://graph.microsoft.com/v1.0", authenticationProvider, httpProvider)
    {
    }

    public GraphServiceClient(
      string baseUrl,
      IAuthenticationProvider authenticationProvider,
      IHttpProvider httpProvider = null)
      : base(baseUrl, authenticationProvider, httpProvider)
    {
    }

    public IGraphServiceDirectoryObjectsCollectionRequestBuilder DirectoryObjects
    {
      get
      {
        return (IGraphServiceDirectoryObjectsCollectionRequestBuilder) new GraphServiceDirectoryObjectsCollectionRequestBuilder(this.BaseUrl + "/directoryObjects", (IBaseClient) this);
      }
    }

    public IGraphServiceDevicesCollectionRequestBuilder Devices
    {
      get
      {
        return (IGraphServiceDevicesCollectionRequestBuilder) new GraphServiceDevicesCollectionRequestBuilder(this.BaseUrl + "/devices", (IBaseClient) this);
      }
    }

    public IGraphServiceGroupsCollectionRequestBuilder Groups
    {
      get
      {
        return (IGraphServiceGroupsCollectionRequestBuilder) new GraphServiceGroupsCollectionRequestBuilder(this.BaseUrl + "/groups", (IBaseClient) this);
      }
    }

    public IGraphServiceDirectoryRolesCollectionRequestBuilder DirectoryRoles
    {
      get
      {
        return (IGraphServiceDirectoryRolesCollectionRequestBuilder) new GraphServiceDirectoryRolesCollectionRequestBuilder(this.BaseUrl + "/directoryRoles", (IBaseClient) this);
      }
    }

    public IGraphServiceDirectoryRoleTemplatesCollectionRequestBuilder DirectoryRoleTemplates
    {
      get
      {
        return (IGraphServiceDirectoryRoleTemplatesCollectionRequestBuilder) new GraphServiceDirectoryRoleTemplatesCollectionRequestBuilder(this.BaseUrl + "/directoryRoleTemplates", (IBaseClient) this);
      }
    }

    public IGraphServiceOrganizationCollectionRequestBuilder Organization
    {
      get
      {
        return (IGraphServiceOrganizationCollectionRequestBuilder) new GraphServiceOrganizationCollectionRequestBuilder(this.BaseUrl + "/organization", (IBaseClient) this);
      }
    }

    public IGraphServiceSubscribedSkusCollectionRequestBuilder SubscribedSkus
    {
      get
      {
        return (IGraphServiceSubscribedSkusCollectionRequestBuilder) new GraphServiceSubscribedSkusCollectionRequestBuilder(this.BaseUrl + "/subscribedSkus", (IBaseClient) this);
      }
    }

    public IGraphServiceUsersCollectionRequestBuilder Users
    {
      get
      {
        return (IGraphServiceUsersCollectionRequestBuilder) new GraphServiceUsersCollectionRequestBuilder(this.BaseUrl + "/users", (IBaseClient) this);
      }
    }

    public IGraphServiceDrivesCollectionRequestBuilder Drives
    {
      get
      {
        return (IGraphServiceDrivesCollectionRequestBuilder) new GraphServiceDrivesCollectionRequestBuilder(this.BaseUrl + "/drives", (IBaseClient) this);
      }
    }

    public IGraphServiceSubscriptionsCollectionRequestBuilder Subscriptions
    {
      get
      {
        return (IGraphServiceSubscriptionsCollectionRequestBuilder) new GraphServiceSubscriptionsCollectionRequestBuilder(this.BaseUrl + "/subscriptions", (IBaseClient) this);
      }
    }

    public IUserRequestBuilder Me
    {
      get => (IUserRequestBuilder) new UserRequestBuilder(this.BaseUrl + "/me", (IBaseClient) this);
    }

    public IDriveRequestBuilder Drive
    {
      get
      {
        return (IDriveRequestBuilder) new DriveRequestBuilder(this.BaseUrl + "/drive", (IBaseClient) this);
      }
    }
  }
}
