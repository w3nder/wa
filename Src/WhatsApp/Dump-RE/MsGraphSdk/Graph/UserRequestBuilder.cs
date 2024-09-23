// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class UserRequestBuilder : 
    DirectoryObjectRequestBuilder,
    IUserRequestBuilder,
    IDirectoryObjectRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public IDriveItemRequestBuilder ItemWithPath(string path)
    {
      if (!string.IsNullOrEmpty(path))
        path = path.TrimStart('/');
      return (IDriveItemRequestBuilder) new DriveItemRequestBuilder(string.Format("{0}/{1}:", (object) this.RequestUrl, (object) path), this.Client);
    }

    public UserRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IUserRequest Request() => this.Request((IEnumerable<Option>) null);

    public IUserRequest Request(IEnumerable<Option> options)
    {
      return (IUserRequest) new UserRequest(this.RequestUrl, this.Client, options);
    }

    public IUserOwnedDevicesCollectionWithReferencesRequestBuilder OwnedDevices
    {
      get
      {
        return (IUserOwnedDevicesCollectionWithReferencesRequestBuilder) new UserOwnedDevicesCollectionWithReferencesRequestBuilder(this.AppendSegmentToRequestUrl("ownedDevices"), this.Client);
      }
    }

    public IUserRegisteredDevicesCollectionWithReferencesRequestBuilder RegisteredDevices
    {
      get
      {
        return (IUserRegisteredDevicesCollectionWithReferencesRequestBuilder) new UserRegisteredDevicesCollectionWithReferencesRequestBuilder(this.AppendSegmentToRequestUrl("registeredDevices"), this.Client);
      }
    }

    public IDirectoryObjectWithReferenceRequestBuilder Manager
    {
      get
      {
        return (IDirectoryObjectWithReferenceRequestBuilder) new DirectoryObjectWithReferenceRequestBuilder(this.AppendSegmentToRequestUrl("manager"), this.Client);
      }
    }

    public IUserDirectReportsCollectionWithReferencesRequestBuilder DirectReports
    {
      get
      {
        return (IUserDirectReportsCollectionWithReferencesRequestBuilder) new UserDirectReportsCollectionWithReferencesRequestBuilder(this.AppendSegmentToRequestUrl("directReports"), this.Client);
      }
    }

    public IUserMemberOfCollectionWithReferencesRequestBuilder MemberOf
    {
      get
      {
        return (IUserMemberOfCollectionWithReferencesRequestBuilder) new UserMemberOfCollectionWithReferencesRequestBuilder(this.AppendSegmentToRequestUrl("memberOf"), this.Client);
      }
    }

    public IUserCreatedObjectsCollectionWithReferencesRequestBuilder CreatedObjects
    {
      get
      {
        return (IUserCreatedObjectsCollectionWithReferencesRequestBuilder) new UserCreatedObjectsCollectionWithReferencesRequestBuilder(this.AppendSegmentToRequestUrl("createdObjects"), this.Client);
      }
    }

    public IUserOwnedObjectsCollectionWithReferencesRequestBuilder OwnedObjects
    {
      get
      {
        return (IUserOwnedObjectsCollectionWithReferencesRequestBuilder) new UserOwnedObjectsCollectionWithReferencesRequestBuilder(this.AppendSegmentToRequestUrl("ownedObjects"), this.Client);
      }
    }

    public IUserMessagesCollectionRequestBuilder Messages
    {
      get
      {
        return (IUserMessagesCollectionRequestBuilder) new UserMessagesCollectionRequestBuilder(this.AppendSegmentToRequestUrl("messages"), this.Client);
      }
    }

    public IUserMailFoldersCollectionRequestBuilder MailFolders
    {
      get
      {
        return (IUserMailFoldersCollectionRequestBuilder) new UserMailFoldersCollectionRequestBuilder(this.AppendSegmentToRequestUrl("mailFolders"), this.Client);
      }
    }

    public ICalendarRequestBuilder Calendar
    {
      get
      {
        return (ICalendarRequestBuilder) new CalendarRequestBuilder(this.AppendSegmentToRequestUrl("calendar"), this.Client);
      }
    }

    public IUserCalendarsCollectionRequestBuilder Calendars
    {
      get
      {
        return (IUserCalendarsCollectionRequestBuilder) new UserCalendarsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("calendars"), this.Client);
      }
    }

    public IUserCalendarGroupsCollectionRequestBuilder CalendarGroups
    {
      get
      {
        return (IUserCalendarGroupsCollectionRequestBuilder) new UserCalendarGroupsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("calendarGroups"), this.Client);
      }
    }

    public IUserCalendarViewCollectionRequestBuilder CalendarView
    {
      get
      {
        return (IUserCalendarViewCollectionRequestBuilder) new UserCalendarViewCollectionRequestBuilder(this.AppendSegmentToRequestUrl("calendarView"), this.Client);
      }
    }

    public IUserEventsCollectionRequestBuilder Events
    {
      get
      {
        return (IUserEventsCollectionRequestBuilder) new UserEventsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("events"), this.Client);
      }
    }

    public IUserContactsCollectionRequestBuilder Contacts
    {
      get
      {
        return (IUserContactsCollectionRequestBuilder) new UserContactsCollectionRequestBuilder(this.AppendSegmentToRequestUrl("contacts"), this.Client);
      }
    }

    public IUserContactFoldersCollectionRequestBuilder ContactFolders
    {
      get
      {
        return (IUserContactFoldersCollectionRequestBuilder) new UserContactFoldersCollectionRequestBuilder(this.AppendSegmentToRequestUrl("contactFolders"), this.Client);
      }
    }

    public IInferenceClassificationRequestBuilder InferenceClassification
    {
      get
      {
        return (IInferenceClassificationRequestBuilder) new InferenceClassificationRequestBuilder(this.AppendSegmentToRequestUrl("inferenceClassification"), this.Client);
      }
    }

    public IProfilePhotoRequestBuilder Photo
    {
      get
      {
        return (IProfilePhotoRequestBuilder) new ProfilePhotoRequestBuilder(this.AppendSegmentToRequestUrl("photo"), this.Client);
      }
    }

    public IDriveRequestBuilder Drive
    {
      get
      {
        return (IDriveRequestBuilder) new DriveRequestBuilder(this.AppendSegmentToRequestUrl("drive"), this.Client);
      }
    }

    public IUserAssignLicenseRequestBuilder AssignLicense(
      IEnumerable<AssignedLicense> addLicenses,
      IEnumerable<Guid> removeLicenses)
    {
      return (IUserAssignLicenseRequestBuilder) new UserAssignLicenseRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.assignLicense"), this.Client, addLicenses, removeLicenses);
    }

    public IUserChangePasswordRequestBuilder ChangePassword(
      string currentPassword = null,
      string newPassword = null)
    {
      return (IUserChangePasswordRequestBuilder) new UserChangePasswordRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.changePassword"), this.Client, currentPassword, newPassword);
    }

    public IUserSendMailRequestBuilder SendMail(Message Message, bool? SaveToSentItems = null)
    {
      return (IUserSendMailRequestBuilder) new UserSendMailRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.sendMail"), this.Client, Message, SaveToSentItems);
    }

    public IUserReminderViewRequestBuilder ReminderView(string StartDateTime, string EndDateTime = null)
    {
      return (IUserReminderViewRequestBuilder) new UserReminderViewRequestBuilder(this.AppendSegmentToRequestUrl("microsoft.graph.reminderView"), this.Client, StartDateTime, EndDateTime);
    }
  }
}
