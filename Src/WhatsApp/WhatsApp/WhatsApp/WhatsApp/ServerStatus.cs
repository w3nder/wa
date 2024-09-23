// Decompiled with JetBrains decompiler
// Type: WhatsApp.ServerStatus
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;


namespace WhatsApp
{
  [DataContract]
  public class ServerStatus
  {
    private const string StatusURL = "http://www.whatsapp.com/status.php?v=2";

    [DataMember(Name = "email")]
    public Availability Email { get; set; }

    [DataMember(Name = "last")]
    public Availability Last { get; set; }

    [DataMember(Name = "sync")]
    public Availability Sync { get; set; }

    [DataMember(Name = "chat")]
    public Availability Chat { get; set; }

    [DataMember(Name = "group")]
    public Availability Group { get; set; }

    [DataMember(Name = "multimedia")]
    public Availability Multimedia { get; set; }

    [DataMember(Name = "online")]
    public Availability Online { get; set; }

    [DataMember(Name = "profile")]
    public Availability Profile { get; set; }

    [DataMember(Name = "push")]
    public Availability Push { get; set; }

    [DataMember(Name = "registration")]
    public Availability Registration { get; set; }

    [DataMember(Name = "status")]
    public Availability Status { get; set; }

    [DataMember(Name = "broadcast")]
    public Availability Broadcast { get; set; }

    [DataMember(Name = "version")]
    public Availability Version { get; set; }

    public int FailureCount()
    {
      int num = 0;
      if (!this.Version.Available)
        ++num;
      if (!this.Last.Available)
        ++num;
      if (!this.Sync.Available)
        ++num;
      if (!this.Chat.Available)
        ++num;
      if (!this.Group.Available)
        ++num;
      if (!this.Multimedia.Available)
        ++num;
      if (!this.Online.Available)
        ++num;
      if (!this.Profile.Available)
        ++num;
      if (!this.Push.Available)
        ++num;
      if (!this.Status.Available)
        ++num;
      if (!this.Broadcast.Available)
        ++num;
      if (!this.Registration.Available)
        ++num;
      return num;
    }

    public void GetStatusMessage(
      out string header,
      out List<string> failureList,
      out string footer)
    {
      header = (string) null;
      failureList = (List<string>) null;
      footer = (string) null;
      if (!this.Version.Available)
      {
        header = this.Email.Available ? AppResources.ContactSupportVersionAndContact : AppResources.ContactSupportVersion;
      }
      else
      {
        List<string> source = new List<string>();
        if (Settings.PhoneNumberVerificationState == PhoneNumberVerificationState.Verified)
        {
          if (!this.Last.Available)
            source.Add(AppResources.ContactSupportLastSeen);
          if (!this.Sync.Available)
            source.Add(AppResources.ContactSupportSync);
          if (!this.Chat.Available)
            source.Add("");
          if (!this.Group.Available)
            source.Add(AppResources.ContactSupportGroup);
          if (!this.Multimedia.Available)
            source.Add(AppResources.ContactSupportMultimedia);
          if (!this.Online.Available)
            source.Add(AppResources.ContactSupportOnline);
          if (!this.Profile.Available)
            source.Add(AppResources.ContactSupportProfile);
          if (!this.Push.Available)
            source.Add(AppResources.ContactSupportPush);
          if (!this.Status.Available)
            source.Add(AppResources.ContactSupportRevivedStatusV2);
          if (!this.Broadcast.Available)
            source.Add(AppResources.ContactSupportBroadcast);
        }
        else if (!this.Registration.Available)
          source.Add(AppResources.VerificationTitle);
        if (source.Count == 1 || source.Count == 2 && !this.Email.Available)
        {
          string format = this.Email.Available ? AppResources.ContactSupportExperiencedProblemsSingle : AppResources.ContactSupportExperiencingProblemsSingle;
          if (!this.Chat.Available)
            header = this.Email.Available ? AppResources.ContactSupportRecentChatProblems : AppResources.ContactSupportCurrentChatProblems;
          else if (!this.Last.Available)
            header = string.Format(format, (object) AppResources.ContactSupportLastSeen);
          else if (!this.Sync.Available)
            header = string.Format(format, (object) AppResources.ContactSupportSync);
          else if (!this.Group.Available)
            header = string.Format(format, (object) AppResources.ContactSupportGroup);
          else if (!this.Multimedia.Available)
            header = string.Format(format, (object) AppResources.ContactSupportMultimedia);
          else if (!this.Online.Available)
            header = string.Format(format, (object) AppResources.ContactSupportOnline);
          else if (!this.Profile.Available)
            header = string.Format(format, (object) AppResources.ContactSupportProfile);
          else if (!this.Push.Available)
            header = string.Format(format, (object) AppResources.ContactSupportPush);
          else if (!this.Status.Available)
            header = string.Format(format, (object) AppResources.ContactSupportRevivedStatusV2);
          else if (!this.Broadcast.Available)
          {
            header = string.Format(format, (object) AppResources.ContactSupportBroadcast);
          }
          else
          {
            if (this.Registration.Available)
              return;
            header = string.Format(format, (object) AppResources.ContactSupportRegistration);
          }
        }
        else
        {
          if (source.Count <= 1)
            return;
          header = this.Email.Available ? AppResources.ContactSupportExperiencedProblemsMultiple : AppResources.ContactSupportExperiencingProblemsMultiple;
          failureList = new List<string>();
          failureList = source.Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x))).ToList<string>();
          footer = this.Email.Available ? AppResources.ContactSupportStillHavingProblemsPluralFeatures : AppResources.ContactSupportSorryForInconvenience;
        }
      }
    }

    public bool CanContinue() => this.Email.Available;

    public static IObservable<ServerStatus> GetStatus()
    {
      return Observable.CreateWithDisposable<byte[]>((Func<IObserver<byte[]>, IDisposable>) (observer =>
      {
        WebRequest that = WebRequest.Create("http://www.whatsapp.com/status.php?v=2");
        that.Headers["User-Agent"] = AppState.GetUserAgent();
        return that.GetResponseBytesAync().Subscribe(observer);
      })).Select<byte[], ServerStatus>((Func<byte[], ServerStatus>) (bytes => new DataContractJsonSerializer(typeof (ServerStatus)).ReadObject((Stream) new MemoryStream(bytes, false)) as ServerStatus));
    }
  }
}
