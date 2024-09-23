// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IGroupRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public interface IGroupRequestBuilder : 
    IDirectoryObjectRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    IGroupRequest Request();

    IGroupRequest Request(IEnumerable<Option> options);

    IGroupMembersCollectionWithReferencesRequestBuilder Members { get; }

    IGroupMemberOfCollectionWithReferencesRequestBuilder MemberOf { get; }

    IDirectoryObjectWithReferenceRequestBuilder CreatedOnBehalfOf { get; }

    IGroupOwnersCollectionWithReferencesRequestBuilder Owners { get; }

    IGroupThreadsCollectionRequestBuilder Threads { get; }

    ICalendarRequestBuilder Calendar { get; }

    IGroupCalendarViewCollectionRequestBuilder CalendarView { get; }

    IGroupEventsCollectionRequestBuilder Events { get; }

    IGroupConversationsCollectionRequestBuilder Conversations { get; }

    IProfilePhotoRequestBuilder Photo { get; }

    IGroupAcceptedSendersCollectionRequestBuilder AcceptedSenders { get; }

    IGroupRejectedSendersCollectionRequestBuilder RejectedSenders { get; }

    IDriveRequestBuilder Drive { get; }

    IGroupSubscribeByMailRequestBuilder SubscribeByMail();

    IGroupUnsubscribeByMailRequestBuilder UnsubscribeByMail();

    IGroupAddFavoriteRequestBuilder AddFavorite();

    IGroupRemoveFavoriteRequestBuilder RemoveFavorite();

    IGroupResetUnseenCountRequestBuilder ResetUnseenCount();
  }
}
