// Decompiled with JetBrains decompiler
// Type: WhatsApp.NtpClient
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using WhatsApp.Resolvers;


namespace WhatsApp
{
  public class NtpClient
  {
    private string hostname;
    private int port;
    private IPEndPoint endPoint;
    private static readonly DateTime Epoch = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public NtpClient(string hostname, int port = 123)
    {
      this.hostname = hostname;
      this.port = port;
    }

    private IObservable<IPEndPoint> LookupHost()
    {
      return this.endPoint != null ? Observable.Return<IPEndPoint>(this.endPoint) : Observable.Create<IPEndPoint>((Func<IObserver<IPEndPoint>, Action>) (observer =>
      {
        Resolver.Instance.Resolve(this.hostname, (Action<IEnumerable<ResolveResult>>) (r =>
        {
          if (r == null || !r.Any<ResolveResult>())
          {
            observer.OnError(new Exception("Failed to look up " + this.hostname));
          }
          else
          {
            IPAddress address = (IPAddress) null;
            IPAddress.TryParse(r.First<ResolveResult>().Address, out address);
            if (address == null)
              observer.OnError(new Exception("Failed to look up " + this.hostname));
            else
              observer.OnNext(new IPEndPoint(address, this.port));
          }
        }), (Action) (() => observer.OnError(new Exception("Failed to look up " + this.hostname))));
        return (Action) (() => { });
      })).Do<IPEndPoint>((Action<IPEndPoint>) (_ => this.endPoint = _));
    }

    private IObservable<Socket> CreateSocket()
    {
      return this.LookupHost().Select<IPEndPoint, Socket>((Func<IPEndPoint, Socket>) (endPoint =>
      {
        Socket s = new Socket(endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        s.Connect((EndPoint) endPoint);
        return s;
      }));
    }

    private IObservable<NtpPacket> PacketStream(Socket socket)
    {
      return Observable.Create<NtpPacket>((Func<IObserver<NtpPacket>, Action>) (observer =>
      {
        byte[] readBuffer = new byte[SerializeStruct.SizeOf<NtpPacket>()];
        bool cancel = false;
        Action readOne = (Action) null;
        readOne = (Action) (() =>
        {
          EventHandler<SocketAsyncEventArgs> eventHandler = (EventHandler<SocketAsyncEventArgs>) ((sender, respArgs) =>
          {
            if (respArgs.SocketError != SocketError.Success)
            {
              Log.l("ntp", "recv: hit socket error {0}", (object) respArgs.SocketError);
              observer.OnError((Exception) new SocketException((int) respArgs.SocketError));
            }
            else
            {
              NtpPacket p = SerializeStruct.Read<NtpPacket>(respArgs.Buffer, 0, Math.Min(readBuffer.Length, respArgs.BytesTransferred));
              NtpPacket.Swap(ref p);
              observer.OnNext(p);
              if (cancel)
                return;
              readOne();
            }
          });
          SocketAsyncEventArgs e = new SocketAsyncEventArgs();
          e.SetBuffer(readBuffer, 0, readBuffer.Length);
          e.Completed += eventHandler;
          socket.ReceiveFromAsync(e);
        });
        readOne();
        return (Action) (() => cancel = true);
      }));
    }

    private void SendPacket(Socket socket)
    {
      NtpPacket p = new NtpPacket();
      p.Flags = NtpPacket.MakeFlags(0, 3, NtpPacket.ModeClient);
      MemoryStream stream = new MemoryStream();
      NtpPacket.Swap(ref p);
      SerializeStruct.Write<NtpPacket>(p, stream);
      SocketAsyncEventArgs e = new SocketAsyncEventArgs();
      EventHandler<SocketAsyncEventArgs> eventHandler = (EventHandler<SocketAsyncEventArgs>) ((sender, respArgs) =>
      {
        if (respArgs.SocketError == SocketError.Success)
          return;
        Log.l("ntp", "send: hit socket error {0}", (object) respArgs.SocketError);
      });
      e.RemoteEndPoint = (EndPoint) this.endPoint;
      e.SetBuffer(stream.GetBuffer(), 0, (int) stream.Length);
      e.Completed += eventHandler;
      socket.SendToAsync(e);
    }

    public IObservable<DateTime> GetCurrentTime()
    {
      Socket s = (Socket) null;
      IObservable<DateTime> obs = this.CreateSocket().SelectMany<Socket, NtpPacket, NtpPacket>((Func<Socket, IObservable<NtpPacket>>) (socket => Observable.Defer<NtpPacket>((Func<IObservable<NtpPacket>>) (() =>
      {
        s = socket;
        return Observable.CreateWithDisposable<NtpPacket>((Func<IObserver<NtpPacket>, IDisposable>) (observer =>
        {
          IDisposable currentTime = this.PacketStream(socket).Subscribe(observer);
          this.SendPacket(socket);
          return currentTime;
        }));
      }))), (Func<Socket, NtpPacket, NtpPacket>) ((socket, pkt) => pkt)).Select<NtpPacket, DateTime>((Func<NtpPacket, DateTime>) (pkt => NtpClient.Epoch.AddSeconds((double) pkt.ReceiveTimestamp.Seconds)));
      return Observable.Create<DateTime>((Func<IObserver<DateTime>, Action>) (observer =>
      {
        IDisposable d = obs.Subscribe(observer);
        return (Action) (() =>
        {
          d.SafeDispose();
          s.SafeDispose();
        });
      })).Take<DateTime>(1).Timeout<DateTime>(TimeSpan.FromSeconds(10.0));
    }
  }
}
