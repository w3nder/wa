// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GroupRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class GroupRequestBuilder : 
    DirectoryObjectRequestBuilder,
    IGroupRequestBuilder,
    IDirectoryObjectRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public GroupRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IGroupRequest Request() => this.Request((IEnumerable<Option>) null);

    public IGroupRequest Request(IEnumerable<Option> options)
    {
      return (IGroupRequest) new GroupRequest(this.RequestUrl, this.Client, options);
    }

    public IGroupMembersCollectionWithReferencesRequestBuilder Members
    {
      get
      {
        return (IGroupMembersCollectionWithReferencesRequestBuilder) new GroupMembersCollectionWithReferencesRequestBuilder(this.AppendSegmentToRequestUrl("members"), this.Client);
      }
    }

    public IGroupMemberOfCollectionWithReferencesRequestBuilder MemberOf
    {
      get
      {
        return (IGroupMemberOfCollectionWithReferencesRequestBuilder) new GroupMemberOfCollectionWithReferencesRequestBuilder(this.AppendSegmentToRequestUrl("memberOf"), this.Client);
      }
    }

    public IDirectoryObjectWithReferenceRequestBuilder CreatedOnBehalfOf
    {
      get
      {
        return (IDirectoryObjectWithReferenceRequestBuilder) new DirectoryObjectWithReferenceRequestBuilder(this.AppendSegmentToRequestUrl("createdOnBehalfOf"), this.Client);
      }
    }

    public IGroupOwnersCollectionWithReferencesRequestBuilder Owners
    {
      get
      {
        return (IGroupOwnersCollectionWithReferencesRequestBuilder) new GroupOwnersCollectionWithReferencesRequestBuilder(this.AppendSegmentToRequestUrl("owners"), this.Client);
      }
    }

    public IGroupThreadsCollectionRequestBuilder Threads
    {
      get
      {
        return (IGroupThreadsCollectionRequestBuilder) new GroupThreadsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("threads"), this.Client);
      }
    }

    public ICalendarRequestBuilder Calendar
    {
      get
      {
        return (ICalendarRequestBuilder) new CalendarRequestBuilder(this.AppendSegmentToRequestUrl("calendar"), this.Client);
      }
    }

    public IGroupCalendarViewCollectionRequestBuilder CalendarView
    {
      get
      {
        return (IGroupCalendarViewCollectionRequestBuilder) new GroupCalendarViewCollectionRequestBuilder(this.AppendSegmentToRequestUrl("calendarView"), this.Client);
      }
    }

    public IGroupEventsCollectionRequestBuilder Events
    {
      get
      {
        return (IGroupEventsCollectionRequestBuilder) new GroupEventsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("events"), this.Client);
      }
    }

    public IGroupConversationsCollectionRequestBuilder Conversations
    {
      get
      {
        return (IGroupConversationsCollectionRequestBuilder) new GroupConversationsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("conversations"), this.Client);
      }
    }

    public IProfilePhotoRequestBuilder Photo
    {
      get
      {
        return (IProfilePhotoRequestBuilder) new ProfilePhotoRequestBuilder(this.AppendSegmentToRequestUrl("photo"), this.Client);
      }
    }

    public IGroupAcceptedSendersCollectionRequestBuilder AcceptedSenders
    {
      get
      {
        return (IGroupAcceptedSendersCollectionRequestBuilder) new GroupAcceptedSendersCollectionRequestBuilder(this.AppendSegmentToRequestUrl("acceptedSenders"), this.Client);
      }
    }

    public IGroupRejectedSendersCollectionRequestBuilder RejectedSenders
    {
      get
      {
        return (IGroupRejectedSendersCollectionRequestBuilder) new GroupRejectedSendersCollectionRequestBuilder(this.AppendSegmentToRequestUrl("rejectedSenders"), this.Client);
      }
    }

    public IDriveRequestBuilder Drive
    {
      get
      {
        return (IDriveRequestBuilder) new DriveRequestBuilder(this.AppendSegmentToRequestUrl("drive"), this.Client);
      }
    }

    public IGroupSubscribeByMailRequestBuilder SubscribeByMail()
    {
      return (IGroupSubscribeByMailRequestBuilder) new GroupSubscribeByMailRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.subscribeByMail"), this.Client);
    }

    public IGroupUnsubscribeByMailRequestBuilder UnsubscribeByMail()
    {
      return (IGroupUnsubscribeByMailRequestBuilder) new GroupUnsubscribeByMailRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.unsubscribeByMail"), this.Client);
    }

    public IGroupAddFavoriteRequestBuilder AddFavorite()
    {
      return (IGroupAddFavoriteRequestBuilder) new GroupAddFavoriteRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.addFavorite"), this.Client);
    }

    public IGroupRemoveFavoriteRequestBuilder RemoveFavorite()
    {
      return (IGroupRemoveFavoriteRequestBuilder) new GroupRemoveFavoriteRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.removeFavorite"), this.Client);
    }

    public IGroupResetUnseenCountRequestBuilder ResetUnseenCount()
    {
      return (IGroupResetUnseenCountRequestBuilder) new GroupResetUnseenCountRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.resetUnseenCount"), this.Client);
    }
  }
}
