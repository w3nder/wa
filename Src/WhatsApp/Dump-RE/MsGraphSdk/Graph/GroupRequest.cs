// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GroupRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class GroupRequest : BaseRequest, IGroupRequest, IBaseRequest
  {
    public GroupRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<Group> CreateAsync(Group groupToCreate)
    {
      return this.CreateAsync(groupToCreate, CancellationToken.None);
    }

    public async Task<Group> CreateAsync(Group groupToCreate, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      Group groupToInitialize = await this.SendAsync<Group>((object) groupToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(groupToInitialize);
      return groupToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      Group group = await this.SendAsync<Group>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<Group> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<Group> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      Group groupToInitialize = await this.SendAsync<Group>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(groupToInitialize);
      return groupToInitialize;
    }

    public Task<Group> UpdateAsync(Group groupToUpdate)
    {
      return this.UpdateAsync(groupToUpdate, CancellationToken.None);
    }

    public async Task<Group> UpdateAsync(Group groupToUpdate, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      Group groupToInitialize = await this.SendAsync<Group>((object) groupToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(groupToInitialize);
      return groupToInitialize;
    }

    private void InitializeCollectionProperties(Group groupToInitialize)
    {
      if (groupToInitialize == null || groupToInitialize.AdditionalData == null)
        return;
      if (groupToInitialize.Members != null && groupToInitialize.Members.CurrentPage != null)
      {
        groupToInitialize.Members.AdditionalData = groupToInitialize.AdditionalData;
        object obj;
        groupToInitialize.AdditionalData.TryGetValue("members@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          groupToInitialize.Members.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (groupToInitialize.MemberOf != null && groupToInitialize.MemberOf.CurrentPage != null)
      {
        groupToInitialize.MemberOf.AdditionalData = groupToInitialize.AdditionalData;
        object obj;
        groupToInitialize.AdditionalData.TryGetValue("memberOf@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          groupToInitialize.MemberOf.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (groupToInitialize.Owners != null && groupToInitialize.Owners.CurrentPage != null)
      {
        groupToInitialize.Owners.AdditionalData = groupToInitialize.AdditionalData;
        object obj;
        groupToInitialize.AdditionalData.TryGetValue("owners@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          groupToInitialize.Owners.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (groupToInitialize.Threads != null && groupToInitialize.Threads.CurrentPage != null)
      {
        groupToInitialize.Threads.AdditionalData = groupToInitialize.AdditionalData;
        object obj;
        groupToInitialize.AdditionalData.TryGetValue("threads@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          groupToInitialize.Threads.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (groupToInitialize.CalendarView != null && groupToInitialize.CalendarView.CurrentPage != null)
      {
        groupToInitialize.CalendarView.AdditionalData = groupToInitialize.AdditionalData;
        object obj;
        groupToInitialize.AdditionalData.TryGetValue("calendarView@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          groupToInitialize.CalendarView.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (groupToInitialize.Events != null && groupToInitialize.Events.CurrentPage != null)
      {
        groupToInitialize.Events.AdditionalData = groupToInitialize.AdditionalData;
        object obj;
        groupToInitialize.AdditionalData.TryGetValue("events@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          groupToInitialize.Events.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (groupToInitialize.Conversations != null && groupToInitialize.Conversations.CurrentPage != null)
      {
        groupToInitialize.Conversations.AdditionalData = groupToInitialize.AdditionalData;
        object obj;
        groupToInitialize.AdditionalData.TryGetValue("conversations@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          groupToInitialize.Conversations.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (groupToInitialize.AcceptedSenders != null && groupToInitialize.AcceptedSenders.CurrentPage != null)
      {
        groupToInitialize.AcceptedSenders.AdditionalData = groupToInitialize.AdditionalData;
        object obj;
        groupToInitialize.AdditionalData.TryGetValue("acceptedSenders@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          groupToInitialize.AcceptedSenders.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (groupToInitialize.RejectedSenders == null || groupToInitialize.RejectedSenders.CurrentPage == null)
        return;
      groupToInitialize.RejectedSenders.AdditionalData = groupToInitialize.AdditionalData;
      object obj1;
      groupToInitialize.AdditionalData.TryGetValue("rejectedSenders@odata.nextLink", out obj1);
      string nextPageLinkString1 = obj1 as string;
      if (string.IsNullOrEmpty(nextPageLinkString1))
        return;
      groupToInitialize.RejectedSenders.InitializeNextPageRequest(this.Client, nextPageLinkString1);
    }
  }
}
