// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.ConfigService.MapControlConfigurationServiceClient
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.ConfigService
{
  [GeneratedCode("System.ServiceModel", "4.0.0.0")]
  [DebuggerStepThrough]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal class MapControlConfigurationServiceClient : 
    ClientBase<IMapControlConfigurationService>,
    IMapControlConfigurationService
  {
    private ClientBase<IMapControlConfigurationService>.BeginOperationDelegate onBeginGetConfigurationDelegate;
    private ClientBase<IMapControlConfigurationService>.EndOperationDelegate onEndGetConfigurationDelegate;
    private SendOrPostCallback onGetConfigurationCompletedDelegate;
    private ClientBase<IMapControlConfigurationService>.BeginOperationDelegate onBeginOpenDelegate;
    private ClientBase<IMapControlConfigurationService>.EndOperationDelegate onEndOpenDelegate;
    private SendOrPostCallback onOpenCompletedDelegate;
    private ClientBase<IMapControlConfigurationService>.BeginOperationDelegate onBeginCloseDelegate;
    private ClientBase<IMapControlConfigurationService>.EndOperationDelegate onEndCloseDelegate;
    private SendOrPostCallback onCloseCompletedDelegate;

    public MapControlConfigurationServiceClient()
    {
    }

    public MapControlConfigurationServiceClient(string endpointConfigurationName)
      : base(endpointConfigurationName)
    {
    }

    public MapControlConfigurationServiceClient(
      string endpointConfigurationName,
      string remoteAddress)
      : base(endpointConfigurationName, remoteAddress)
    {
    }

    public MapControlConfigurationServiceClient(
      string endpointConfigurationName,
      EndpointAddress remoteAddress)
      : base(endpointConfigurationName, remoteAddress)
    {
    }

    public MapControlConfigurationServiceClient(Binding binding, EndpointAddress remoteAddress)
      : base(binding, remoteAddress)
    {
    }

    public CookieContainer CookieContainer
    {
      get => this.InnerChannel.GetProperty<IHttpCookieContainerManager>()?.CookieContainer;
      set
      {
        IHttpCookieContainerManager property = this.InnerChannel.GetProperty<IHttpCookieContainerManager>();
        if (property == null)
          throw new InvalidOperationException("Unable to set the CookieContainer. Please make sure the binding contains an HttpCookieContainerBindingElement.");
        property.CookieContainer = value;
      }
    }

    public event EventHandler<GetConfigurationCompletedEventArgs> GetConfigurationCompleted;

    public event EventHandler<AsyncCompletedEventArgs> OpenCompleted;

    public event EventHandler<AsyncCompletedEventArgs> CloseCompleted;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    IAsyncResult IMapControlConfigurationService.BeginGetConfiguration(
      MapControlConfigurationRequest request,
      AsyncCallback callback,
      object asyncState)
    {
      return this.Channel.BeginGetConfiguration(request, callback, asyncState);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    MapControlConfigurationResponse IMapControlConfigurationService.EndGetConfiguration(
      IAsyncResult result)
    {
      return this.Channel.EndGetConfiguration(result);
    }

    private IAsyncResult OnBeginGetConfiguration(
      object[] inValues,
      AsyncCallback callback,
      object asyncState)
    {
      return ((IMapControlConfigurationService) this).BeginGetConfiguration((MapControlConfigurationRequest) inValues[0], callback, asyncState);
    }

    private object[] OnEndGetConfiguration(IAsyncResult result)
    {
      return new object[1]
      {
        (object) ((IMapControlConfigurationService) this).EndGetConfiguration(result)
      };
    }

    private void OnGetConfigurationCompleted(object state)
    {
      if (this.GetConfigurationCompleted == null)
        return;
      ClientBase<IMapControlConfigurationService>.InvokeAsyncCompletedEventArgs completedEventArgs = (ClientBase<IMapControlConfigurationService>.InvokeAsyncCompletedEventArgs) state;
      this.GetConfigurationCompleted((object) this, new GetConfigurationCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    public void GetConfigurationAsync(MapControlConfigurationRequest request)
    {
      this.GetConfigurationAsync(request, (object) null);
    }

    public void GetConfigurationAsync(MapControlConfigurationRequest request, object userState)
    {
      if (this.onBeginGetConfigurationDelegate == null)
        this.onBeginGetConfigurationDelegate = new ClientBase<IMapControlConfigurationService>.BeginOperationDelegate(this.OnBeginGetConfiguration);
      if (this.onEndGetConfigurationDelegate == null)
        this.onEndGetConfigurationDelegate = new ClientBase<IMapControlConfigurationService>.EndOperationDelegate(this.OnEndGetConfiguration);
      if (this.onGetConfigurationCompletedDelegate == null)
        this.onGetConfigurationCompletedDelegate = new SendOrPostCallback(this.OnGetConfigurationCompleted);
      this.InvokeAsync(this.onBeginGetConfigurationDelegate, new object[1]
      {
        (object) request
      }, this.onEndGetConfigurationDelegate, this.onGetConfigurationCompletedDelegate, userState);
    }

    private IAsyncResult OnBeginOpen(object[] inValues, AsyncCallback callback, object asyncState)
    {
      return ((ICommunicationObject) this).BeginOpen(callback, asyncState);
    }

    private object[] OnEndOpen(IAsyncResult result)
    {
      ((ICommunicationObject) this).EndOpen(result);
      return (object[]) null;
    }

    private void OnOpenCompleted(object state)
    {
      if (this.OpenCompleted == null)
        return;
      ClientBase<IMapControlConfigurationService>.InvokeAsyncCompletedEventArgs completedEventArgs = (ClientBase<IMapControlConfigurationService>.InvokeAsyncCompletedEventArgs) state;
      this.OpenCompleted((object) this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    public void OpenAsync() => this.OpenAsync((object) null);

    public void OpenAsync(object userState)
    {
      if (this.onBeginOpenDelegate == null)
        this.onBeginOpenDelegate = new ClientBase<IMapControlConfigurationService>.BeginOperationDelegate(this.OnBeginOpen);
      if (this.onEndOpenDelegate == null)
        this.onEndOpenDelegate = new ClientBase<IMapControlConfigurationService>.EndOperationDelegate(this.OnEndOpen);
      if (this.onOpenCompletedDelegate == null)
        this.onOpenCompletedDelegate = new SendOrPostCallback(this.OnOpenCompleted);
      this.InvokeAsync(this.onBeginOpenDelegate, (object[]) null, this.onEndOpenDelegate, this.onOpenCompletedDelegate, userState);
    }

    private IAsyncResult OnBeginClose(object[] inValues, AsyncCallback callback, object asyncState)
    {
      return ((ICommunicationObject) this).BeginClose(callback, asyncState);
    }

    private object[] OnEndClose(IAsyncResult result)
    {
      ((ICommunicationObject) this).EndClose(result);
      return (object[]) null;
    }

    private void OnCloseCompleted(object state)
    {
      if (this.CloseCompleted == null)
        return;
      ClientBase<IMapControlConfigurationService>.InvokeAsyncCompletedEventArgs completedEventArgs = (ClientBase<IMapControlConfigurationService>.InvokeAsyncCompletedEventArgs) state;
      this.CloseCompleted((object) this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    public void CloseAsync() => this.CloseAsync((object) null);

    public void CloseAsync(object userState)
    {
      if (this.onBeginCloseDelegate == null)
        this.onBeginCloseDelegate = new ClientBase<IMapControlConfigurationService>.BeginOperationDelegate(this.OnBeginClose);
      if (this.onEndCloseDelegate == null)
        this.onEndCloseDelegate = new ClientBase<IMapControlConfigurationService>.EndOperationDelegate(this.OnEndClose);
      if (this.onCloseCompletedDelegate == null)
        this.onCloseCompletedDelegate = new SendOrPostCallback(this.OnCloseCompleted);
      this.InvokeAsync(this.onBeginCloseDelegate, (object[]) null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
    }

    protected override IMapControlConfigurationService CreateChannel()
    {
      return (IMapControlConfigurationService) new MapControlConfigurationServiceClient.MapControlConfigurationServiceClientChannel((ClientBase<IMapControlConfigurationService>) this);
    }

    [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
    private class MapControlConfigurationServiceClientChannel : 
      ClientBase<IMapControlConfigurationService>.ChannelBase<IMapControlConfigurationService>,
      IMapControlConfigurationService
    {
      public MapControlConfigurationServiceClientChannel(
        ClientBase<IMapControlConfigurationService> client)
        : base(client)
      {
      }

      public IAsyncResult BeginGetConfiguration(
        MapControlConfigurationRequest request,
        AsyncCallback callback,
        object asyncState)
      {
        return this.BeginInvoke("GetConfiguration", new object[1]
        {
          (object) request
        }, callback, asyncState);
      }

      public MapControlConfigurationResponse EndGetConfiguration(IAsyncResult result)
      {
        return (MapControlConfigurationResponse) this.EndInvoke("GetConfiguration", new object[0], result);
      }
    }
  }
}
