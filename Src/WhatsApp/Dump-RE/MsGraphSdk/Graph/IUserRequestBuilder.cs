// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IUserRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public interface IUserRequestBuilder : 
    IDirectoryObjectRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    IDriveItemRequestBuilder ItemWithPath(string path);

    IUserRequest Request();

    IUserRequest Request(IEnumerable<Option> options);

    IUserOwnedDevicesCollectionWithReferencesRequestBuilder OwnedDevices { get; }

    IUserRegisteredDevicesCollectionWithReferencesRequestBuilder RegisteredDevices { get; }

    IDirectoryObjectWithReferenceRequestBuilder Manager { get; }

    IUserDirectReportsCollectionWithReferencesRequestBuilder DirectReports { get; }

    IUserMemberOfCollectionWithReferencesRequestBuilder MemberOf { get; }

    IUserCreatedObjectsCollectionWithReferencesRequestBuilder CreatedObjects { get; }

    IUserOwnedObjectsCollectionWithReferencesRequestBuilder OwnedObjects { get; }

    IUserMessagesCollectionRequestBuilder Messages { get; }

    IUserMailFoldersCollectionRequestBuilder MailFolders { get; }

    ICalendarRequestBuilder Calendar { get; }

    IUserCalendarsCollectionRequestBuilder Calendars { get; }

    IUserCalendarGroupsCollectionRequestBuilder CalendarGroups { get; }

    IUserCalendarViewCollectionRequestBuilder CalendarView { get; }

    IUserEventsCollectionRequestBuilder Events { get; }

    IUserContactsCollectionRequestBuilder Contacts { get; }

    IUserContactFoldersCollectionRequestBuilder ContactFolders { get; }

    IInferenceClassificationRequestBuilder InferenceClassification { get; }

    IProfilePhotoRequestBuilder Photo { get; }

    IDriveRequestBuilder Drive { get; }

    IUserAssignLicenseRequestBuilder AssignLicense(
      IEnumerable<AssignedLicense> addLicenses,
      IEnumerable<Guid> removeLicenses);

    IUserChangePasswordRequestBuilder ChangePassword(string currentPassword = null, string newPassword = null);

    IUserSendMailRequestBuilder SendMail(Message Message, bool? SaveToSentItems = null);

    IUserReminderViewRequestBuilder ReminderView(string StartDateTime, string EndDateTime = null);
  }
}
