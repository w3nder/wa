// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.PlatformServices.ImageryServiceClient
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
namespace Microsoft.Phone.Controls.Maps.PlatformServices
{
  [GeneratedCode("System.ServiceModel", "4.0.0.0")]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [DebuggerStepThrough]
  internal class ImageryServiceClient : ClientBase<IImageryService>, IImageryService
  {
    private ClientBase<IImageryService>.BeginOperationDelegate onBeginGetImageryMetadataDelegate;
    private ClientBase<IImageryService>.EndOperationDelegate onEndGetImageryMetadataDelegate;
    private SendOrPostCallback onGetImageryMetadataCompletedDelegate;
    private ClientBase<IImageryService>.BeginOperationDelegate onBeginGetMapUriDelegate;
    private ClientBase<IImageryService>.EndOperationDelegate onEndGetMapUriDelegate;
    private SendOrPostCallback onGetMapUriCompletedDelegate;
    private ClientBase<IImageryService>.BeginOperationDelegate onBeginOpenDelegate;
    private ClientBase<IImageryService>.EndOperationDelegate onEndOpenDelegate;
    private SendOrPostCallback onOpenCompletedDelegate;
    private ClientBase<IImageryService>.BeginOperationDelegate onBeginCloseDelegate;
    private ClientBase<IImageryService>.EndOperationDelegate onEndCloseDelegate;
    private SendOrPostCallback onCloseCompletedDelegate;

    public ImageryServiceClient()
    {
    }

    public ImageryServiceClient(string endpointConfigurationName)
      : base(endpointConfigurationName)
    {
    }

    public ImageryServiceClient(string endpointConfigurationName, string remoteAddress)
      : base(endpointConfigurationName, remoteAddress)
    {
    }

    public ImageryServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress)
      : base(endpointConfigurationName, remoteAddress)
    {
    }

    public ImageryServiceClient(Binding binding, EndpointAddress remoteAddress)
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

    public event EventHandler<GetImageryMetadataCompletedEventArgs> GetImageryMetadataCompleted;

    public event EventHandler<GetMapUriCompletedEventArgs> GetMapUriCompleted;

    public event EventHandler<AsyncCompletedEventArgs> OpenCompleted;

    public event EventHandler<AsyncCompletedEventArgs> CloseCompleted;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    IAsyncResult IImageryService.BeginGetImageryMetadata(
      ImageryMetadataRequest request,
      AsyncCallback callback,
      object asyncState)
    {
      return this.Channel.BeginGetImageryMetadata(request, callback, asyncState);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    ImageryMetadataResponse IImageryService.EndGetImageryMetadata(IAsyncResult result)
    {
      return this.Channel.EndGetImageryMetadata(result);
    }

    private IAsyncResult OnBeginGetImageryMetadata(
      object[] inValues,
      AsyncCallback callback,
      object asyncState)
    {
      return ((IImageryService) this).BeginGetImageryMetadata((ImageryMetadataRequest) inValues[0], callback, asyncState);
    }

    private object[] OnEndGetImageryMetadata(IAsyncResult result)
    {
      return new object[1]
      {
        (object) ((IImageryService) this).EndGetImageryMetadata(result)
      };
    }

    private void OnGetImageryMetadataCompleted(object state)
    {
      if (this.GetImageryMetadataCompleted == null)
        return;
      ClientBase<IImageryService>.InvokeAsyncCompletedEventArgs completedEventArgs = (ClientBase<IImageryService>.InvokeAsyncCompletedEventArgs) state;
      this.GetImageryMetadataCompleted((object) this, new GetImageryMetadataCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    public void GetImageryMetadataAsync(ImageryMetadataRequest request)
    {
      this.GetImageryMetadataAsync(request, (object) null);
    }

    public void GetImageryMetadataAsync(ImageryMetadataRequest request, object userState)
    {
      if (this.onBeginGetImageryMetadataDelegate == null)
        this.onBeginGetImageryMetadataDelegate = new ClientBase<IImageryService>.BeginOperationDelegate(this.OnBeginGetImageryMetadata);
      if (this.onEndGetImageryMetadataDelegate == null)
        this.onEndGetImageryMetadataDelegate = new ClientBase<IImageryService>.EndOperationDelegate(this.OnEndGetImageryMetadata);
      if (this.onGetImageryMetadataCompletedDelegate == null)
        this.onGetImageryMetadataCompletedDelegate = new SendOrPostCallback(this.OnGetImageryMetadataCompleted);
      this.InvokeAsync(this.onBeginGetImageryMetadataDelegate, new object[1]
      {
        (object) request
      }, this.onEndGetImageryMetadataDelegate, this.onGetImageryMetadataCompletedDelegate, userState);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    IAsyncResult IImageryService.BeginGetMapUri(
      MapUriRequest request,
      AsyncCallback callback,
      object asyncState)
    {
      return this.Channel.BeginGetMapUri(request, callback, asyncState);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    MapUriResponse IImageryService.EndGetMapUri(IAsyncResult result)
    {
      return this.Channel.EndGetMapUri(result);
    }

    private IAsyncResult OnBeginGetMapUri(
      object[] inValues,
      AsyncCallback callback,
      object asyncState)
    {
      return ((IImageryService) this).BeginGetMapUri((MapUriRequest) inValues[0], callback, asyncState);
    }

    private object[] OnEndGetMapUri(IAsyncResult result)
    {
      return new object[1]
      {
        (object) ((IImageryService) this).EndGetMapUri(result)
      };
    }

    private void OnGetMapUriCompleted(object state)
    {
      if (this.GetMapUriCompleted == null)
        return;
      ClientBase<IImageryService>.InvokeAsyncCompletedEventArgs completedEventArgs = (ClientBase<IImageryService>.InvokeAsyncCompletedEventArgs) state;
      this.GetMapUriCompleted((object) this, new GetMapUriCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    public void GetMapUriAsync(MapUriRequest request)
    {
      this.GetMapUriAsync(request, (object) null);
    }

    public void GetMapUriAsync(MapUriRequest request, object userState)
    {
      if (this.onBeginGetMapUriDelegate == null)
        this.onBeginGetMapUriDelegate = new ClientBase<IImageryService>.BeginOperationDelegate(this.OnBeginGetMapUri);
      if (this.onEndGetMapUriDelegate == null)
        this.onEndGetMapUriDelegate = new ClientBase<IImageryService>.EndOperationDelegate(this.OnEndGetMapUri);
      if (this.onGetMapUriCompletedDelegate == null)
        this.onGetMapUriCompletedDelegate = new SendOrPostCallback(this.OnGetMapUriCompleted);
      this.InvokeAsync(this.onBeginGetMapUriDelegate, new object[1]
      {
        (object) request
      }, this.onEndGetMapUriDelegate, this.onGetMapUriCompletedDelegate, userState);
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
      ClientBase<IImageryService>.InvokeAsyncCompletedEventArgs completedEventArgs = (ClientBase<IImageryService>.InvokeAsyncCompletedEventArgs) state;
      this.OpenCompleted((object) this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    public void OpenAsync() => this.OpenAsync((object) null);

    public void OpenAsync(object userState)
    {
      if (this.onBeginOpenDelegate == null)
        this.onBeginOpenDelegate = new ClientBase<IImageryService>.BeginOperationDelegate(this.OnBeginOpen);
      if (this.onEndOpenDelegate == null)
        this.onEndOpenDelegate = new ClientBase<IImageryService>.EndOperationDelegate(this.OnEndOpen);
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
      ClientBase<IImageryService>.InvokeAsyncCompletedEventArgs completedEventArgs = (ClientBase<IImageryService>.InvokeAsyncCompletedEventArgs) state;
      this.CloseCompleted((object) this, new AsyncCompletedEventArgs(completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    public void CloseAsync() => this.CloseAsync((object) null);

    public void CloseAsync(object userState)
    {
      if (this.onBeginCloseDelegate == null)
        this.onBeginCloseDelegate = new ClientBase<IImageryService>.BeginOperationDelegate(this.OnBeginClose);
      if (this.onEndCloseDelegate == null)
        this.onEndCloseDelegate = new ClientBase<IImageryService>.EndOperationDelegate(this.OnEndClose);
      if (this.onCloseCompletedDelegate == null)
        this.onCloseCompletedDelegate = new SendOrPostCallback(this.OnCloseCompleted);
      this.InvokeAsync(this.onBeginCloseDelegate, (object[]) null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
    }

    protected override IImageryService CreateChannel()
    {
      return (IImageryService) new ImageryServiceClient.ImageryServiceClientChannel((ClientBase<IImageryService>) this);
    }

    [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
    private class ImageryServiceClientChannel : 
      ClientBase<IImageryService>.ChannelBase<IImageryService>,
      IImageryService
    {
      public ImageryServiceClientChannel(ClientBase<IImageryService> client)
        : base(client)
      {
      }

      public IAsyncResult BeginGetImageryMetadata(
        ImageryMetadataRequest request,
        AsyncCallback callback,
        object asyncState)
      {
        return this.BeginInvoke("GetImageryMetadata", new object[1]
        {
          (object) request
        }, callback, asyncState);
      }

      public ImageryMetadataResponse EndGetImageryMetadata(IAsyncResult result)
      {
        return (ImageryMetadataResponse) this.EndInvoke("GetImageryMetadata", new object[0], result);
      }

      public IAsyncResult BeginGetMapUri(
        MapUriRequest request,
        AsyncCallback callback,
        object asyncState)
      {
        return this.BeginInvoke("GetMapUri", new object[1]
        {
          (object) request
        }, callback, asyncState);
      }

      public MapUriResponse EndGetMapUri(IAsyncResult result)
      {
        return (MapUriResponse) this.EndInvoke("GetMapUri", new object[0], result);
      }
    }
  }
}
