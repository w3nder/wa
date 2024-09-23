// Decompiled with JetBrains decompiler
// Type: WhatsApp.Resolvers.SocketExtensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace WhatsApp.Resolvers
{
  public static class SocketExtensions
  {
    public static void Connect(this Socket s, EndPoint ep)
    {
      Exception ex = (Exception) null;
      using (ManualResetEvent ev = new ManualResetEvent(false))
      {
        SocketAsyncEventArgs e = new SocketAsyncEventArgs();
        e.Completed += (EventHandler<SocketAsyncEventArgs>) ((sender, argsIn) =>
        {
          if (argsIn.SocketError != SocketError.Success)
            ex = (Exception) new SocketException((int) argsIn.SocketError);
          ev.Set();
        });
        e.RemoteEndPoint = ep;
        s.ConnectAsync(e);
        ev.WaitOne();
      }
      if (ex != null)
        throw ex;
    }
  }
}
