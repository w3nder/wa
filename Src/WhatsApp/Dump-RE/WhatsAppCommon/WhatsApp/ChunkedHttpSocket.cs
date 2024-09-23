// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChunkedHttpSocket
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using WhatsApp.Events;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class ChunkedHttpSocket : IWASocket
  {
    private IWASocket nativeSocket;
    private ChunkedHttpSocket.ChunkedHttpSocketHandler httpHandler;
    private static readonly byte[] crlf = Encoding.UTF8.GetBytes("\r\n");
    private static readonly int crlfLength = ChunkedHttpSocket.crlf.Length;
    private static readonly string connectHeader = "POST /chat HTTP/1.1\r\nHost: c.whatsapp.net\r\nUser-Agent: Mozilla/5.0 (compatible; WAChat/1.2; +http://www.whatsapp.com/contact)\r\nTransfer-Encoding: chunked\r\n\r\n";
    private static readonly byte[] headerBytes = Encoding.UTF8.GetBytes(ChunkedHttpSocket.connectHeader);
    private static readonly byte[] emptyChunk = Encoding.UTF8.GetBytes("0\r\n\r\n");

    public ChunkedHttpSocket()
    {
      this.nativeSocket = (IWASocket) NativeInterfaces.CreateInstance<WASocket>();
      this.nativeSocket.SetPort((ushort) 80);
      this.httpHandler = new ChunkedHttpSocket.ChunkedHttpSocketHandler(this.nativeSocket);
      this.nativeSocket.SetHandler((IWASocketHandler) this.httpHandler);
      this.httpHandler.fsSentOverheadTimer.Start();
      IByteBuffer instance = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
      instance.Put(ChunkedHttpSocket.headerBytes);
      this.nativeSocket.Send(instance);
      instance.Reset();
      this.httpHandler.fsTotalBytesSent += ChunkedHttpSocket.headerBytes.Length;
      this.httpHandler.fsOverheadBytesSent += ChunkedHttpSocket.headerBytes.Length;
      this.httpHandler.fsSentOverheadTimer.Stop();
    }

    public void Close()
    {
      this.httpHandler.fsSentOverheadTimer.Start();
      IByteBuffer instance = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
      instance.Put(ChunkedHttpSocket.emptyChunk, 0, ChunkedHttpSocket.emptyChunk.Length);
      this.nativeSocket.Send(instance);
      instance.Reset();
      this.nativeSocket.Close();
      Log.l(nameof (ChunkedHttpSocket), "Closed");
      this.httpHandler.fsTotalBytesSent += ChunkedHttpSocket.emptyChunk.Length;
      this.httpHandler.fsOverheadBytesSent += ChunkedHttpSocket.emptyChunk.Length;
      this.httpHandler.fsSentOverheadTimer.Stop();
    }

    public void Connect(IWAScheduler scheduler) => this.nativeSocket.Connect(scheduler);

    public void Dispose() => this.nativeSocket.Dispose();

    public void Send(IByteBuffer buffer)
    {
      this.httpHandler.fsSentOverheadTimer.Start();
      byte[] bytes = Encoding.UTF8.GetBytes(buffer.GetLength().ToString("X"));
      byte[] numArray = new byte[bytes.Length + ChunkedHttpSocket.crlfLength + buffer.Get().Length + ChunkedHttpSocket.crlfLength];
      System.Buffer.BlockCopy((Array) bytes, 0, (Array) numArray, 0, bytes.Length);
      System.Buffer.BlockCopy((Array) ChunkedHttpSocket.crlf, 0, (Array) numArray, bytes.Length, ChunkedHttpSocket.crlfLength);
      System.Buffer.BlockCopy((Array) buffer.Get(), 0, (Array) numArray, bytes.Length + ChunkedHttpSocket.crlfLength, buffer.Get().Length);
      System.Buffer.BlockCopy((Array) ChunkedHttpSocket.crlf, 0, (Array) numArray, bytes.Length + ChunkedHttpSocket.crlfLength + buffer.Get().Length, ChunkedHttpSocket.crlfLength);
      IByteBuffer instance = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
      instance.Put(numArray);
      this.nativeSocket.Send(instance);
      instance.Reset();
      this.httpHandler.fsTotalBytesSent += numArray.Length;
      this.httpHandler.fsOverheadBytesSent += bytes.Length + ChunkedHttpSocket.crlfLength + ChunkedHttpSocket.crlfLength;
      this.httpHandler.fsSentOverheadTimer.Stop();
    }

    public void SetHandler(IWASocketHandler handler) => this.httpHandler.ClientHandler = handler;

    public void SetHost(IHost host) => this.nativeSocket.SetHost(host);

    public void SetPort(ushort port)
    {
    }

    public void SetTimeoutMilliseconds(int milliseconds, bool cumulative)
    {
      this.nativeSocket.SetTimeoutMilliseconds(milliseconds, cumulative);
    }

    private class ChunkedHttpSocketHandler : IWASocketHandler
    {
      private char[] colon = new char[1]{ ':' };
      private IWASocket nativeSocket;
      private MemoryStream input;
      private bool statusRead;
      private bool headersRead;
      private bool sawChunkedHeader;
      private int bytesToRead;
      public int fsTotalBytesRead;
      public int fsTotalBytesSent;
      public int fsOverheadBytesRead;
      public int fsOverheadBytesSent;
      public Stopwatch fsSentOverheadTimer;
      public Stopwatch fsReadOverheadTimer;

      public ChunkedHttpSocketHandler(IWASocket nativeSocket)
      {
        this.nativeSocket = nativeSocket;
        this.fsReadOverheadTimer = new Stopwatch();
        this.fsSentOverheadTimer = new Stopwatch();
        this.Reset();
      }

      public IWASocketHandler ClientHandler { get; set; }

      private void Reset()
      {
        this.input = new MemoryStream();
        this.statusRead = false;
        this.headersRead = false;
        this.sawChunkedHeader = false;
        this.bytesToRead = 0;
        this.fsTotalBytesRead = 0;
        this.fsTotalBytesSent = 0;
        this.fsOverheadBytesRead = 0;
        this.fsOverheadBytesSent = 0;
        this.fsSentOverheadTimer.Reset();
        this.fsReadOverheadTimer.Reset();
      }

      private void ShrinkInputStream(int amountToShrink)
      {
        if (amountToShrink <= 0)
          return;
        MemoryStream memoryStream = new MemoryStream();
        if ((long) amountToShrink < this.input.Length)
          memoryStream.Write(this.input.GetBuffer(), amountToShrink, (int) this.input.Length - amountToShrink);
        this.input.Dispose();
        this.input = memoryStream;
      }

      private void OnClientBytesIn(byte[] buffer, int start, int count)
      {
        if (count <= 0)
          return;
        IByteBuffer instance = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
        instance.Put(buffer, start, count);
        this.fsReadOverheadTimer.Stop();
        this.ClientHandler.BytesIn(instance);
        this.fsReadOverheadTimer.Start();
        instance.Reset();
      }

      public void BytesIn(IByteBuffer buffer)
      {
        this.fsReadOverheadTimer.Start();
        this.fsTotalBytesRead += buffer.GetLength();
        if (buffer.GetLength() <= this.bytesToRead)
        {
          this.OnClientBytesIn(buffer.Get(), 0, Math.Min(buffer.GetLength(), this.bytesToRead - ChunkedHttpSocket.crlfLength));
          this.bytesToRead -= buffer.GetLength();
        }
        else
        {
          if (this.bytesToRead > ChunkedHttpSocket.crlfLength)
            this.OnClientBytesIn(buffer.Get(), 0, this.bytesToRead - ChunkedHttpSocket.crlfLength);
          int length = (int) this.input.Length;
          int num1 = 0;
          this.input.Write(buffer.Get(), this.bytesToRead, buffer.GetLength() - this.bytesToRead);
          this.bytesToRead = 0;
          try
          {
            while ((long) length < this.input.Length - 1L)
            {
              if ((int) this.input.GetBuffer()[length] == (int) ChunkedHttpSocket.crlf[0] && (int) this.input.GetBuffer()[length + 1] == (int) ChunkedHttpSocket.crlf[1])
              {
                string s = Encoding.UTF8.GetString(this.input.GetBuffer(), num1, length - num1);
                length += ChunkedHttpSocket.crlfLength;
                this.fsOverheadBytesRead += s.Length + ChunkedHttpSocket.crlfLength;
                if (!this.statusRead)
                  this.statusRead = true;
                else if (!this.headersRead)
                {
                  if (s.Length == 0)
                  {
                    if (!this.sawChunkedHeader)
                    {
                      Log.l(nameof (ChunkedHttpSocket), "No 'Transfer-Encoding: Chunked' header in response from server");
                      this.nativeSocket.Close();
                      return;
                    }
                    this.headersRead = true;
                  }
                  else
                  {
                    string[] strArray = s.Split(this.colon, StringSplitOptions.RemoveEmptyEntries);
                    if (strArray.Length < 2)
                      throw new IndexOutOfRangeException("HTTP header failed to split into at least two pieces");
                    if (strArray[0].Trim().Equals("Transfer-Encoding"))
                    {
                      if (strArray[1].Trim().Equals("chunked"))
                      {
                        this.sawChunkedHeader = true;
                      }
                      else
                      {
                        Log.l(nameof (ChunkedHttpSocket), "Transfer-Encoding header not set to chunked in response from server");
                        this.nativeSocket.Close();
                        return;
                      }
                    }
                  }
                }
                else
                {
                  this.fsOverheadBytesRead += ChunkedHttpSocket.crlfLength;
                  this.bytesToRead = int.Parse(s, NumberStyles.HexNumber) + ChunkedHttpSocket.crlfLength;
                  if (this.bytesToRead == ChunkedHttpSocket.crlfLength)
                  {
                    this.nativeSocket.Close();
                    return;
                  }
                  this.OnClientBytesIn(this.input.GetBuffer(), length, Math.Min((int) this.input.Length - length, this.bytesToRead - ChunkedHttpSocket.crlfLength));
                  int num2 = length;
                  length += this.bytesToRead;
                  this.bytesToRead -= Math.Min(this.bytesToRead, (int) this.input.Length - num2);
                }
                num1 = length;
              }
              else
                ++length;
            }
          }
          catch (Exception ex)
          {
            Log.l(nameof (ChunkedHttpSocket), "Malformed chunked data from server");
            Log.l(nameof (ChunkedHttpSocket), ex.ToString());
            Log.l(nameof (ChunkedHttpSocket), "Saved input:\n{0}", (object) Encoding.UTF8.GetString(this.input.GetBuffer(), 0, this.input.GetBuffer().Length));
            Log.l(nameof (ChunkedHttpSocket), "Current input:\n{0}", (object) Encoding.UTF8.GetString(buffer.Get(), 0, buffer.GetLength()));
            Log.l(nameof (ChunkedHttpSocket), "i: {0}; lineStart: {1}", (object) length, (object) num1);
            Log.l(nameof (ChunkedHttpSocket), "Current line: {0}", (object) Encoding.UTF8.GetString(this.input.GetBuffer(), num1, length - num1));
            this.nativeSocket.Close();
            throw;
          }
          this.ShrinkInputStream(num1);
          this.fsReadOverheadTimer.Stop();
        }
      }

      public void Connected()
      {
        Log.l(nameof (ChunkedHttpSocketHandler), "Got Connected Event");
        if (this.ClientHandler == null)
          return;
        this.ClientHandler.Connected();
      }

      public void Disconnected(uint hr)
      {
        Log.l(nameof (ChunkedHttpSocketHandler), "Got Disconnected Event");
        new PseudoHttpSession()
        {
          pseudoHttpTotalBytesReceived = new double?((double) this.fsTotalBytesRead),
          pseudoHttpTotalBytesSent = new double?((double) this.fsTotalBytesSent),
          pseudoHttpHeaderBytesReceived = new double?((double) this.fsOverheadBytesRead),
          pseudoHttpHeaderBytesSent = new double?((double) this.fsOverheadBytesSent),
          pseudoHttpReceiveOverheadT = new long?(this.fsReadOverheadTimer.ElapsedMilliseconds),
          pseudoHttpSendOverheadT = new long?(this.fsSentOverheadTimer.ElapsedMilliseconds)
        }.SaveEventSampled(20U);
        this.Reset();
        if (this.ClientHandler == null)
          return;
        this.ClientHandler.Disconnected(hr);
      }

      public void WriteBufferDrained()
      {
        if (this.ClientHandler == null)
          return;
        this.ClientHandler.WriteBufferDrained();
      }
    }
  }
}
