// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IGraphServiceClient
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

#nullable disable
namespace Microsoft.Graph
{
  public interface IGraphServiceClient : IBaseClient
  {
    IGraphServiceDirectoryObjectsCollectionRequestBuilder DirectoryObjects { get; }

    IGraphServiceDevicesCollectionRequestBuilder Devices { get; }

    IGraphServiceGroupsCollectionRequestBuilder Groups { get; }

    IGraphServiceDirectoryRolesCollectionRequestBuilder DirectoryRoles { get; }

    IGraphServiceDirectoryRoleTemplatesCollectionRequestBuilder DirectoryRoleTemplates { get; }

    IGraphServiceOrganizationCollectionRequestBuilder Organization { get; }

    IGraphServiceSubscribedSkusCollectionRequestBuilder SubscribedSkus { get; }

    IGraphServiceUsersCollectionRequestBuilder Users { get; }

    IGraphServiceDrivesCollectionRequestBuilder Drives { get; }

    IGraphServiceSubscriptionsCollectionRequestBuilder Subscriptions { get; }

    IUserRequestBuilder Me { get; }

    IDriveRequestBuilder Drive { get; }
  }
}
