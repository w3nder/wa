// Decompiled with JetBrains decompiler
// Type: WhatsApp.Resolvers.DnsResolver
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
using System.Text;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp.Resolvers
{
  public class DnsResolver : ResolverBase
  {
    private IPEndPoint endPoint;
    private const byte PointerMask = 192;
    private Socket socket;
    private object sockInitLock = new object();
    private object observerLock = new object();
    private LinkedList<IObserver<DnsResolver.DnsResult>> ResultObservers = new LinkedList<IObserver<DnsResolver.DnsResult>>();
    private RefCountAction readLoop;

    public DnsResolver(string ip, int port = 53)
    {
      this.endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
    }

    public override string DisplayName
    {
      get
      {
        return string.Format("{0}[ip={1}]", (object) base.DisplayName, (object) this.endPoint.Address.ToString());
      }
    }

    private ushort Read16(Stream input)
    {
      return (ushort) ((uint) ((int) this.ToByte(input.ReadByte()) << 8) | (uint) this.ToByte(input.ReadByte()));
    }

    private uint Read32(Stream input)
    {
      return (uint) ((int) this.Read16(input) << 16) | (uint) this.Read16(input);
    }

    private byte ToByte(int readByteOutput)
    {
      return readByteOutput >= 0 ? (byte) readByteOutput : throw new IOException("Unexpected EOF");
    }

    private DnsResolver.RequestHeader ReadHeader(Stream input)
    {
      DnsResolver.RequestHeader requestHeader = new DnsResolver.RequestHeader();
      requestHeader.Id = this.Read16(input);
      ushort num = this.Read16(input);
      requestHeader.Qr = ((uint) num & 32768U) > 0U;
      requestHeader.AuthoritativeAnswer = ((uint) num & 1024U) > 0U;
      requestHeader.Truncation = ((uint) num & 512U) > 0U;
      requestHeader.RecursionDesired = ((uint) num & 256U) > 0U;
      requestHeader.RecursionAvailable = ((uint) num & 128U) > 0U;
      requestHeader.OpCode = (DnsResolver.OpCode) ((int) num >> 11 & 15);
      requestHeader.ResponseCode = (DnsResolver.ResponseCode) ((int) num & 15);
      requestHeader.QuestionCount = this.Read16(input);
      requestHeader.AnswerCount = this.Read16(input);
      requestHeader.NsCount = this.Read16(input);
      requestHeader.AdditionalCount = this.Read16(input);
      return requestHeader;
    }

    private string ReadHostname(Stream input)
    {
      StringBuilder stringBuilder = new StringBuilder();
      byte[] numArray = new byte[256];
      int num1 = 0;
      long? nullable = new long?();
      byte count1;
      while ((count1 = this.ToByte(input.ReadByte())) != (byte) 0)
      {
        if (((int) count1 & 192) == 192)
        {
          if (num1 > (int) byte.MaxValue)
            throw new Exception("Large chain of pointers in name; giving up");
          byte num2 = this.ToByte(input.ReadByte());
          ushort num3 = (ushort) ((uint) (((int) count1 & -193) << 8) | (uint) num2);
          if (!nullable.HasValue)
            nullable = new long?(input.Position);
          input.Position = (long) num3;
          ++num1;
        }
        else
        {
          int count2 = input.Read(numArray, 0, (int) count1);
          if (count2 != (int) count1)
            throw new IOException("unexpected EOF");
          string str = Encoding.UTF8.GetString(numArray, 0, count2);
          if (stringBuilder.Length != 0)
            stringBuilder.Append('.');
          stringBuilder.Append(str.ToLower());
        }
      }
      if (nullable.HasValue)
        input.Position = nullable.Value;
      return stringBuilder.ToString();
    }

    private DnsResolver.Question ReadQuestion(Stream input)
    {
      return new DnsResolver.Question()
      {
        Name = this.ReadHostname(input),
        Type = (DnsResolver.Type) this.Read16(input),
        Class = (DnsResolver.Class) this.Read16(input)
      };
    }

    private DnsResolver.Answer ReadAnswer(Stream input)
    {
      DnsResolver.Answer answer = new DnsResolver.Answer();
      answer.Name = this.ReadHostname(input);
      answer.Type = (DnsResolver.Type) this.Read16(input);
      answer.Class = (DnsResolver.Class) this.Read16(input);
      answer.Ttl = this.Read32(input);
      ushort count = this.Read16(input);
      answer.Rdl = new byte[(int) count];
      if (input.Read(answer.Rdl, 0, (int) count) != (int) count)
        throw new IOException("unexpected EOF");
      return answer;
    }

    private static Pair<StringBuilder, int> ReadLabel(byte[] data, int offset)
    {
      if (data == null || offset < 0 || offset >= data.Length)
      {
        Log.l("DecodeLabel", "data length:{0}, offset:{1}", data == null ? (object) "null" : (object) data.Length.ToString(), (object) offset);
        throw new InvalidDataException("offset is outside of the data array");
      }
      int second = -1;
      StringBuilder first = new StringBuilder();
      byte count;
      while (true)
      {
        count = data[offset];
        ++offset;
        if (((int) count >> 6 & 3) != 3)
        {
          if (count != (byte) 0)
          {
            if (offset + (int) count < data.Length)
            {
              string str;
              try
              {
                str = Encoding.UTF8.GetString(data, offset, (int) count);
              }
              catch (Exception ex)
              {
                Log.l("DecodeLabel", "data length:{0}, offset:{1}, current Byte:{2}", (object) data.Length.ToString(), (object) offset, (object) count);
                Log.l("DecodeLabel", "cname record", data);
                throw new InvalidDataException("failed to parse canonical name");
              }
              first = first.Append(str).Append(".");
              offset += (int) count;
            }
            else
              goto label_9;
          }
          else
            goto label_13;
        }
        else
          break;
      }
      int num = ((int) count & 63) << 8;
      if (offset >= data.Length)
      {
        Log.l("DecodeLabel", "data length:{0}, offset:{1}, current Byte:{2}", (object) data.Length.ToString(), (object) offset, (object) count);
        throw new InvalidDataException("offset is outside of the data array, when getting a pointer");
      }
      second = num + (int) data[offset];
      goto label_13;
label_9:
      Log.l("DecodeLabel", "data length:{0}, offset:{1}, current Byte:{2}", (object) data.Length.ToString(), (object) offset, (object) count);
      Log.l("DecodeLabel", "cname record", data);
      throw new InvalidDataException("failed to parse canonical name");
label_13:
      return new Pair<StringBuilder, int>(first, second);
    }

    private void Write16(Stream output, ushort value)
    {
      output.WriteByte((byte) ((uint) value >> 8));
      output.WriteByte((byte) ((uint) value & (uint) byte.MaxValue));
    }

    private void WriteRequestHeader(Stream output, DnsResolver.RequestHeader header)
    {
      this.Write16(output, header.Id);
      ushort num = (ushort) ((DnsResolver.ResponseCode) ((header.Qr ? 32768 : 0) | (header.AuthoritativeAnswer ? 1024 : 0) | (header.Truncation ? 512 : 0) | (header.RecursionDesired ? 256 : 0) | (header.RecursionAvailable ? 128 : 0) | (int) header.OpCode << 11) | header.ResponseCode);
      this.Write16(output, num);
      this.Write16(output, header.QuestionCount);
      this.Write16(output, header.AnswerCount);
      this.Write16(output, header.NsCount);
      this.Write16(output, header.AdditionalCount);
    }

    private void WriteQuestion(Stream output, DnsResolver.Question q)
    {
      string str = q.Name + ".";
      char[] chArray = new char[1]{ '.' };
      foreach (string s in str.Split(chArray))
      {
        byte[] numArray = Encoding.UTF8.GetBytes(s);
        if (numArray.Length > 63)
          numArray = ((IEnumerable<byte>) numArray).Take<byte>(63).ToArray<byte>();
        output.WriteByte((byte) numArray.Length);
        output.Write(numArray, 0, numArray.Length);
      }
      this.Write16(output, (ushort) q.Type);
      this.Write16(output, (ushort) q.Class);
    }

    private RefCountAction ReadLoop
    {
      get
      {
        return Utils.LazyInit<RefCountAction>(ref this.readLoop, new Func<RefCountAction>(this.GetReadLoop), this.sockInitLock);
      }
    }

    private RefCountAction GetReadLoop()
    {
      Action disposeReadHandler = (Action) (() => { });
      return new RefCountAction((Action) (() =>
      {
        bool localCancel = false;
        this.socket = new Socket(this.endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        this.socket.Connect((EndPoint) this.endPoint);
        byte[] buffer = new byte[512];
        SocketAsyncEventArgs readArgs = new SocketAsyncEventArgs();
        readArgs.SetBuffer(buffer, 0, buffer.Length);
        EventHandler<SocketAsyncEventArgs> readHandler = (EventHandler<SocketAsyncEventArgs>) ((sender, respArgs) =>
        {
          if (respArgs.SocketError != SocketError.Success)
          {
            Log.WriteLineDebug("{0}: recv: hit socket error {1}", (object) this.DisplayName, (object) respArgs.SocketError);
            SocketException ex = new SocketException((int) respArgs.SocketError);
            this.ForEachObserver((Action<IObserver<DnsResolver.DnsResult>>) (obs => obs.OnError((Exception) ex)));
          }
          else
          {
            DnsResolver.DnsResult res = (DnsResolver.DnsResult) null;
            using (MemoryStream input = new MemoryStream(respArgs.Buffer, 0, respArgs.BytesTransferred, false, true))
            {
              try
              {
                res = this.ParseResponse(input);
              }
              catch (Exception ex)
              {
                string context = string.Format("{0}: parse response", (object) this.DisplayName);
                Log.LogException(ex, context);
              }
            }
            if (res != null)
              this.ForEachObserver((Action<IObserver<DnsResolver.DnsResult>>) (obs =>
              {
                try
                {
                  obs.OnNext(res);
                }
                catch (Exception ex)
                {
                  obs.OnError(ex);
                }
              }));
            this.readLoop.Synchronize((Action) (() =>
            {
              if (!this.readLoop.InEffect)
                return;
              if (localCancel)
                return;
              try
              {
                this.socket.ReceiveFromAsync(readArgs);
              }
              catch (Exception ex)
              {
                this.ForEachObserver((Action<IObserver<DnsResolver.DnsResult>>) (obs =>
                {
                  try
                  {
                    obs.OnError(ex);
                  }
                  catch (Exception ex2)
                  {
                  }
                }));
              }
            }));
          }
        });
        readArgs.Completed += readHandler;
        disposeReadHandler = (Action) (() =>
        {
          readArgs.Completed -= readHandler;
          localCancel = true;
        });
        this.socket.ReceiveFromAsync(readArgs);
      }), (Action) (() =>
      {
        this.socket.Dispose();
        this.socket = (Socket) null;
        disposeReadHandler();
        disposeReadHandler = (Action) null;
      }));
    }

    private void ForEachObserver(
      Action<IObserver<DnsResolver.DnsResult>> onObserver)
    {
      lock (this.observerLock)
      {
        foreach (IObserver<DnsResolver.DnsResult> observer in this.ResultObservers.AsRemoveSafeEnumerator<IObserver<DnsResolver.DnsResult>>())
        {
          try
          {
            onObserver(observer);
          }
          catch (Exception ex)
          {
          }
        }
      }
    }

    private IObservable<DnsResolver.DnsResult> GetResponseObservable()
    {
      return Observable.Create<DnsResolver.DnsResult>((Func<IObserver<DnsResolver.DnsResult>, Action>) (observer =>
      {
        LinkedListNode<IObserver<DnsResolver.DnsResult>> node = (LinkedListNode<IObserver<DnsResolver.DnsResult>>) null;
        lock (this.observerLock)
          node = this.ResultObservers.AddLast(observer);
        IDisposable eventSub = this.ReadLoop.Subscribe();
        return (Action) (() =>
        {
          lock (this.observerLock)
          {
            if (node != null)
            {
              node.List.Remove(node);
              node = (LinkedListNode<IObserver<DnsResolver.DnsResult>>) null;
            }
            eventSub.SafeDispose();
            eventSub = (IDisposable) null;
          }
        });
      }));
    }

    private DnsResolver.DnsResult ParseResponse(MemoryStream input)
    {
      DnsResolver.RequestHeader requestHeader = this.ReadHeader((Stream) input);
      List<DnsResolver.Question> source1 = new List<DnsResolver.Question>();
      List<DnsResolver.Answer> source2 = new List<DnsResolver.Answer>();
      for (int index = 0; index < (int) requestHeader.QuestionCount; ++index)
        source1.Add(this.ReadQuestion((Stream) input));
      for (int index = 0; index < (int) requestHeader.AnswerCount; ++index)
        source2.Add(this.ReadAnswer((Stream) input));
      DnsResolver.DnsResult response = new DnsResolver.DnsResult();
      DnsResolver.Answer cnameAnswer = (DnsResolver.Answer) null;
      response.Host = source1.Select<DnsResolver.Question, string>((Func<DnsResolver.Question, string>) (q => q.Name)).Where<string>((Func<string, bool>) (n => n != null)).FirstOrDefault<string>();
      response.Addresses = (IEnumerable<ResolveResult>) source2.Where<DnsResolver.Answer>((Func<DnsResolver.Answer, bool>) (a => a.Type == DnsResolver.Type.A || a.Type == DnsResolver.Type.AAAA || a.Type == DnsResolver.Type.CNAME)).Select<DnsResolver.Answer, ResolveResult>((Func<DnsResolver.Answer, ResolveResult>) (a =>
      {
        try
        {
          if (a.Type == DnsResolver.Type.CNAME)
          {
            Log.d(this.DisplayName, "found CNAME {0}", (object) a.Name);
            cnameAnswer = a;
            return (ResolveResult) null;
          }
          IPAddress ipAddress = new IPAddress(a.Rdl);
          return new ResolveResult()
          {
            Address = ipAddress.ToString(),
            Ttl = new uint?(a.Ttl)
          };
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "parse IP");
          return (ResolveResult) null;
        }
      })).Where<ResolveResult>((Func<ResolveResult, bool>) (a => a != null)).ToArray<ResolveResult>();
      response.ResponseCode = requestHeader.ResponseCode;
      response.IsResponse = requestHeader.Qr;
      response.IsTruncated = requestHeader.Truncation;
      if ((response.Addresses == null || response.Addresses.Count<ResolveResult>() == 0) && cnameAnswer != null)
      {
        Log.d(this.DisplayName, "processing CNAME {0}", (object) cnameAnswer.Name);
        try
        {
          Pair<StringBuilder, int> pair = DnsResolver.ReadLabel(cnameAnswer.Rdl, 0);
          StringBuilder first = pair.First;
          if (pair.Second != -1)
          {
            input.Position = (long) pair.Second;
            first.Append((object) DnsResolver.ReadLabel(input.GetBuffer(), pair.Second).First);
          }
          ResolveResult resolveResult = new ResolveResult()
          {
            Address = first.ToString(),
            Ttl = new uint?(cnameAnswer.Ttl)
          };
          response.Addresses = (IEnumerable<ResolveResult>) new ResolveResult[1]
          {
            resolveResult
          };
          response.IsCname = true;
        }
        catch (Exception ex)
        {
          Log.l(this.DisplayName, "Exception processing CNAME {0}", (object) ex.GetFriendlyMessage());
          response.ResponseCode = DnsResolver.ResponseCode.NameError;
        }
      }
      if (string.IsNullOrEmpty(response.Host))
        response = (DnsResolver.DnsResult) null;
      return response;
    }

    public override void ResolveImpl(
      string host,
      Action<IEnumerable<ResolveResult>> onResults,
      Action onError)
    {
      this.ResolveImplInner(host, onResults, onError, 0);
    }

    public void ResolveImplInner(
      string host,
      Action<IEnumerable<ResolveResult>> callerOnResults,
      Action callerOnError,
      int depth)
    {
      bool complete = false;
      List<Action> onComplete = new List<Action>();
      object ocLock = new object();
      Action callOnComplete = (Action) (() =>
      {
        lock (ocLock)
        {
          List<Action> actionList = onComplete;
          onComplete.Clear();
          actionList.ForEach((Action<Action>) (a => a()));
          complete = true;
        }
      });
      Action<Action> action = (Action<Action>) (a =>
      {
        lock (ocLock)
        {
          if (complete)
            a();
          else
            onComplete.Add(a);
        }
      });
      Action<IEnumerable<ResolveResult>> innerRes = callerOnResults;
      Action<IEnumerable<ResolveResult>> onResults = (Action<IEnumerable<ResolveResult>>) (res =>
      {
        innerRes(res);
        callOnComplete();
      });
      Action<IEnumerable<ResolveResult>> onCname = (Action<IEnumerable<ResolveResult>>) (res =>
      {
        string address = res.FirstOrDefault<ResolveResult>().Address;
        Log.l(this.DisplayName, "Cname found {0} at {1}", (object) address, (object) depth);
        callOnComplete();
        this.ResolveImplInner(address, callerOnResults, callerOnError, depth + 1);
      });
      Action innerError = callerOnError;
      Action onError = (Action) (() =>
      {
        innerError();
        callOnComplete();
      });
      using (RefCountDisposable refCountDisposable = new RefCountDisposable(this.ReadLoop.Subscribe()))
      {
        host = host.ToLower();
        lock (ocLock)
        {
          IDisposable disp = this.GetResponseObservable().Where<DnsResolver.DnsResult>((Func<DnsResolver.DnsResult, bool>) (r => r.Host == host)).Take<DnsResolver.DnsResult>(1).Subscribe<DnsResolver.DnsResult>((Action<DnsResolver.DnsResult>) (res =>
          {
            if (res.ResponseCode != DnsResolver.ResponseCode.Success)
              Log.WriteLineDebug("Host lookup returned {0}", (object) res.ResponseCode);
            else if (!res.IsResponse)
              Log.WriteLineDebug("DNS response did not have response bit!");
            else if (res.IsTruncated)
            {
              Log.WriteLineDebug("DNS response was truncated!");
            }
            else
            {
              if (res.IsCname && depth < 3)
              {
                onCname(res.Addresses);
                return;
              }
              if (!res.IsCname && res.Addresses != null && res.Addresses.Any<ResolveResult>())
              {
                onResults(res.Addresses);
                return;
              }
            }
            onError();
          }), (Action<Exception>) (ex => onError()));
          action((Action) (() => disp.Dispose()));
        }
        List<DnsResolver.Question> questionList1 = new List<DnsResolver.Question>();
        List<DnsResolver.Question> questionList2 = questionList1;
        DnsResolver.Question question1 = new DnsResolver.Question();
        question1.Name = host;
        question1.Type = DnsResolver.Type.A;
        question1.Class = DnsResolver.Class.IN;
        DnsResolver.Question question2 = question1;
        questionList2.Add(question2);
        List<DnsResolver.Question> questionList3 = questionList1;
        question1 = new DnsResolver.Question();
        question1.Name = host;
        question1.Type = DnsResolver.Type.AAAA;
        question1.Class = DnsResolver.Class.IN;
        DnsResolver.Question question3 = question1;
        questionList3.Add(question3);
        DnsResolver.RequestHeader header = new DnsResolver.RequestHeader()
        {
          Id = (ushort) new Random().Next((int) ushort.MaxValue),
          OpCode = DnsResolver.OpCode.Query,
          QuestionCount = (ushort) questionList1.Count,
          RecursionDesired = true
        };
        MemoryStream mem = new MemoryStream();
        this.WriteRequestHeader((Stream) mem, header);
        questionList1.ForEach((Action<DnsResolver.Question>) (q => this.WriteQuestion((Stream) mem, q)));
        IDisposable disposable1 = this.WriteRequest(mem.GetBuffer(), 0, (int) mem.Length, onError, refCountDisposable.GetDisposable());
        action(new Action(disposable1.Dispose));
        IDisposable disposable2 = PooledTimer.Instance.Schedule(TimeSpan.FromSeconds(20.0), (Action) (() => onError()));
        action(new Action(disposable2.Dispose));
      }
    }

    private IDisposable WriteRequest(
      byte[] buffer,
      int offset,
      int len,
      Action onError,
      IDisposable socketSub)
    {
      Action write = (Action) null;
      Action cancelWrite = (Action) null;
      bool cancel = false;
      object @lock = new object();
      int[] retryConstants = new int[5]
      {
        1000,
        1000,
        2000,
        4000,
        4000
      };
      int retryIdx = 0;
      write = (Action) (() =>
      {
        lock (@lock)
        {
          if (cancel)
            return;
        }
        SocketAsyncEventArgs writeArgs = new SocketAsyncEventArgs();
        writeArgs.RemoteEndPoint = (EndPoint) this.endPoint;
        writeArgs.SetBuffer(buffer, offset, len);
        Action removeWriteHandler = (Action) null;
        EventHandler<SocketAsyncEventArgs> writeHandler = (EventHandler<SocketAsyncEventArgs>) ((sender, respArgs) =>
        {
          lock (@lock)
          {
            removeWriteHandler();
            cancelWrite = (Action) null;
          }
          if (respArgs.SocketError != SocketError.Success)
          {
            Log.WriteLineDebug("{0}: send: hit socket error {1}", (object) this.DisplayName, (object) respArgs.SocketError);
            onError();
          }
          else
          {
            if (retryIdx >= retryConstants.Length)
              return;
            PooledTimer.Instance.Schedule(TimeSpan.FromMilliseconds((double) retryConstants[retryIdx++]), write);
          }
        });
        removeWriteHandler = (Action) (() => writeArgs.Completed -= writeHandler);
        lock (@lock)
        {
          if (cancel)
            return;
          cancelWrite = removeWriteHandler;
          writeArgs.Completed += writeHandler;
          this.socket.SendToAsync(writeArgs);
        }
      });
      write();
      return (IDisposable) new DisposableAction((Action) (() =>
      {
        lock (@lock)
        {
          cancel = true;
          if (cancelWrite != null)
          {
            cancelWrite();
            cancelWrite = (Action) null;
          }
          socketSub.SafeDispose();
          socketSub = (IDisposable) null;
        }
      }));
    }

    private enum OpCode
    {
      Query,
      InverseQuery,
      ServerStatusRequest,
    }

    private enum ResponseCode
    {
      Success = 0,
      FormatError = 1,
      ServerFailure = 2,
      NameError = 3,
      NotImplemented = 4,
      Refused = 5,
      Mask = 31, // 0x0000001F
    }

    private enum RequestFlags
    {
      Ra = 128, // 0x00000080
      Rd = 256, // 0x00000100
      Tc = 512, // 0x00000200
      Aa = 1024, // 0x00000400
      Qr = 32768, // 0x00008000
      Mask = 34688, // 0x00008780
    }

    private enum Type
    {
      A = 1,
      CNAME = 5,
      AAAA = 28, // 0x0000001C
      ANY = 255, // 0x000000FF
    }

    private enum Class
    {
      IN = 1,
      ANY = 255, // 0x000000FF
    }

    private struct RequestHeader
    {
      public ushort Id;
      public bool Qr;
      public DnsResolver.OpCode OpCode;
      public bool AuthoritativeAnswer;
      public bool Truncation;
      public bool RecursionDesired;
      public bool RecursionAvailable;
      public DnsResolver.ResponseCode ResponseCode;
      public ushort QuestionCount;
      public ushort AnswerCount;
      public ushort NsCount;
      public ushort AdditionalCount;
    }

    private struct Question
    {
      public string Name;
      public DnsResolver.Type Type;
      public DnsResolver.Class Class;
    }

    private class Answer
    {
      public string Name;
      public DnsResolver.Type Type;
      public DnsResolver.Class Class;
      public uint Ttl;
      public byte[] Rdl;
    }

    private class DnsResult
    {
      public string Host;
      public IEnumerable<ResolveResult> Addresses;
      public DnsResolver.ResponseCode ResponseCode;
      public bool IsResponse;
      public bool IsTruncated;
      public bool IsCname;
    }
  }
}
