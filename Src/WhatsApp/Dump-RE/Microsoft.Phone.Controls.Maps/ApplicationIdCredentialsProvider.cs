// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.ApplicationIdCredentialsProvider
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.ComponentModel;

#nullable disable
namespace Microsoft.Phone.Controls.Maps
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public sealed class ApplicationIdCredentialsProvider : CredentialsProvider, INotifyPropertyChanged
  {
    private string applicationId;

    public ApplicationIdCredentialsProvider()
      : this(string.Empty)
    {
    }

    public ApplicationIdCredentialsProvider(string applicationId)
    {
      this.ApplicationId = applicationId;
    }

    public string ApplicationId
    {
      get => this.applicationId;
      set
      {
        this.applicationId = value;
        this.OnPropertyChanged(nameof (ApplicationId));
      }
    }

    public override void GetCredentials(Action<Credentials> callback)
    {
      callback(new Credentials()
      {
        ApplicationId = this.ApplicationId
      });
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged == null)
        return;
      PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
      propertyChanged((object) this, e);
    }
  }
}
