// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class UserRequest : BaseRequest, IUserRequest, IBaseRequest
  {
    public UserRequest(string requestUrl, IBaseClient client, IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
    }

    public Task<User> CreateAsync(User userToCreate)
    {
      return this.CreateAsync(userToCreate, CancellationToken.None);
    }

    public async Task<User> CreateAsync(User userToCreate, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PUT";
      User userToInitialize = await this.SendAsync<User>((object) userToCreate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(userToInitialize);
      return userToInitialize;
    }

    public Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
      this.Method = "DELETE";
      User user = await this.SendAsync<User>((object) null, cancellationToken).ConfigureAwait(false);
    }

    public Task<User> GetAsync() => this.GetAsync(CancellationToken.None);

    public async Task<User> GetAsync(CancellationToken cancellationToken)
    {
      this.Method = "GET";
      User userToInitialize = await this.SendAsync<User>((object) null, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(userToInitialize);
      return userToInitialize;
    }

    public Task<User> UpdateAsync(User userToUpdate)
    {
      return this.UpdateAsync(userToUpdate, CancellationToken.None);
    }

    public async Task<User> UpdateAsync(User userToUpdate, CancellationToken cancellationToken)
    {
      this.ContentType = "application/json";
      this.Method = "PATCH";
      User userToInitialize = await this.SendAsync<User>((object) userToUpdate, cancellationToken).ConfigureAwait(false);
      this.InitializeCollectionProperties(userToInitialize);
      return userToInitialize;
    }

    private void InitializeCollectionProperties(User userToInitialize)
    {
      if (userToInitialize == null || userToInitialize.AdditionalData == null)
        return;
      if (userToInitialize.OwnedDevices != null && userToInitialize.OwnedDevices.CurrentPage != null)
      {
        userToInitialize.OwnedDevices.AdditionalData = userToInitialize.AdditionalData;
        object obj;
        userToInitialize.AdditionalData.TryGetValue("ownedDevices@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          userToInitialize.OwnedDevices.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (userToInitialize.RegisteredDevices != null && userToInitialize.RegisteredDevices.CurrentPage != null)
      {
        userToInitialize.RegisteredDevices.AdditionalData = userToInitialize.AdditionalData;
        object obj;
        userToInitialize.AdditionalData.TryGetValue("registeredDevices@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          userToInitialize.RegisteredDevices.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (userToInitialize.DirectReports != null && userToInitialize.DirectReports.CurrentPage != null)
      {
        userToInitialize.DirectReports.AdditionalData = userToInitialize.AdditionalData;
        object obj;
        userToInitialize.AdditionalData.TryGetValue("directReports@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          userToInitialize.DirectReports.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (userToInitialize.MemberOf != null && userToInitialize.MemberOf.CurrentPage != null)
      {
        userToInitialize.MemberOf.AdditionalData = userToInitialize.AdditionalData;
        object obj;
        userToInitialize.AdditionalData.TryGetValue("memberOf@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          userToInitialize.MemberOf.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (userToInitialize.CreatedObjects != null && userToInitialize.CreatedObjects.CurrentPage != null)
      {
        userToInitialize.CreatedObjects.AdditionalData = userToInitialize.AdditionalData;
        object obj;
        userToInitialize.AdditionalData.TryGetValue("createdObjects@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          userToInitialize.CreatedObjects.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (userToInitialize.OwnedObjects != null && userToInitialize.OwnedObjects.CurrentPage != null)
      {
        userToInitialize.OwnedObjects.AdditionalData = userToInitialize.AdditionalData;
        object obj;
        userToInitialize.AdditionalData.TryGetValue("ownedObjects@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          userToInitialize.OwnedObjects.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (userToInitialize.Messages != null && userToInitialize.Messages.CurrentPage != null)
      {
        userToInitialize.Messages.AdditionalData = userToInitialize.AdditionalData;
        object obj;
        userToInitialize.AdditionalData.TryGetValue("messages@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          userToInitialize.Messages.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (userToInitialize.MailFolders != null && userToInitialize.MailFolders.CurrentPage != null)
      {
        userToInitialize.MailFolders.AdditionalData = userToInitialize.AdditionalData;
        object obj;
        userToInitialize.AdditionalData.TryGetValue("mailFolders@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          userToInitialize.MailFolders.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (userToInitialize.Calendars != null && userToInitialize.Calendars.CurrentPage != null)
      {
        userToInitialize.Calendars.AdditionalData = userToInitialize.AdditionalData;
        object obj;
        userToInitialize.AdditionalData.TryGetValue("calendars@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          userToInitialize.Calendars.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (userToInitialize.CalendarGroups != null && userToInitialize.CalendarGroups.CurrentPage != null)
      {
        userToInitialize.CalendarGroups.AdditionalData = userToInitialize.AdditionalData;
        object obj;
        userToInitialize.AdditionalData.TryGetValue("calendarGroups@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          userToInitialize.CalendarGroups.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (userToInitialize.CalendarView != null && userToInitialize.CalendarView.CurrentPage != null)
      {
        userToInitialize.CalendarView.AdditionalData = userToInitialize.AdditionalData;
        object obj;
        userToInitialize.AdditionalData.TryGetValue("calendarView@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          userToInitialize.CalendarView.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (userToInitialize.Events != null && userToInitialize.Events.CurrentPage != null)
      {
        userToInitialize.Events.AdditionalData = userToInitialize.AdditionalData;
        object obj;
        userToInitialize.AdditionalData.TryGetValue("events@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          userToInitialize.Events.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (userToInitialize.Contacts != null && userToInitialize.Contacts.CurrentPage != null)
      {
        userToInitialize.Contacts.AdditionalData = userToInitialize.AdditionalData;
        object obj;
        userToInitialize.AdditionalData.TryGetValue("contacts@odata.nextLink", out obj);
        string nextPageLinkString = obj as string;
        if (!string.IsNullOrEmpty(nextPageLinkString))
          userToInitialize.Contacts.InitializeNextPageRequest(this.Client, nextPageLinkString);
      }
      if (userToInitialize.ContactFolders == null || userToInitialize.ContactFolders.CurrentPage == null)
        return;
      userToInitialize.ContactFolders.AdditionalData = userToInitialize.AdditionalData;
      object obj1;
      userToInitialize.AdditionalData.TryGetValue("contactFolders@odata.nextLink", out obj1);
      string nextPageLinkString1 = obj1 as string;
      if (string.IsNullOrEmpty(nextPageLinkString1))
        return;
      userToInitialize.ContactFolders.InitializeNextPageRequest(this.Client, nextPageLinkString1);
    }
  }
}
