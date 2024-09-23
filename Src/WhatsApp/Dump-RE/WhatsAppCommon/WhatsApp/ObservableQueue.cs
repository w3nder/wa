// Decompiled with JetBrains decompiler
// Type: WhatsApp.ObservableQueue
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace WhatsApp
{
  public class ObservableQueue
  {
    private object @lock = new object();
    private LinkedList<ObservableQueue.Observer> PendingObservers = new LinkedList<ObservableQueue.Observer>();
    private ObservableQueue.Observer CurrentObserver;

    private void Attach(ObservableQueue.Observer obs)
    {
      if (this.CurrentObserver == null)
      {
        this.CurrentObserver = obs;
        this.Subscribe(obs);
      }
      else
        obs.Node = this.PendingObservers.AddLast(obs);
    }

    private void CallNext()
    {
      ObservableQueue.Observer obs = this.CurrentObserver = this.PendingObservers.FirstOrDefault<ObservableQueue.Observer>();
      if (obs == null)
        return;
      this.PendingObservers.Remove(obs);
      this.Subscribe(obs);
    }

    private void Subscribe(ObservableQueue.Observer obs)
    {
      obs.Node = (LinkedListNode<ObservableQueue.Observer>) null;
      WAThreadPool.QueueUserWorkItem((Action) (() =>
      {
        try
        {
          obs.Subscribe();
        }
        catch (Exception ex)
        {
          obs.OnError(ex);
        }
      }));
    }

    public IObservable<T> GetQueuedObservable<T>(IObservable<T> source)
    {
      return Observable.Create<T>((Func<IObserver<T>, Action>) (observer =>
      {
        ObservableQueue.Observer wrapperObject = (ObservableQueue.Observer) null;
        IDisposable disp = (IDisposable) null;
        object innerLock = new object();
        bool cancel = false;
        Action release = (Action) (() =>
        {
          wrapperObject.OnNext();
          observer.OnCompleted();
        });
        Action<Exception> onError = (Action<Exception>) (ex =>
        {
          try
          {
            observer.OnError(ex);
          }
          finally
          {
            release();
          }
        });
        wrapperObject = new ObservableQueue.Observer(this, (Action) (() =>
        {
          lock (innerLock)
          {
            if (cancel)
              return;
            disp = source.Subscribe<T>((Action<T>) (_ => observer.OnNext(_)), onError, release);
          }
        }), onError);
        lock (this.@lock)
          this.Attach(wrapperObject);
        return (Action) (() =>
        {
          release();
          lock (innerLock)
          {
            if (disp != null)
            {
              disp.Dispose();
              disp = (IDisposable) null;
            }
            cancel = true;
          }
        });
      }));
    }

    private class Observer
    {
      private ObservableQueue Queue;
      private Action subscribe;
      private Action<Exception> onError;
      public LinkedListNode<ObservableQueue.Observer> Node;

      public Observer(ObservableQueue queue, Action subscribe, Action<Exception> onError)
      {
        this.Queue = queue;
        this.subscribe = subscribe;
        this.onError = onError;
      }

      public void Subscribe() => this.subscribe();

      public void OnError(Exception ex) => this.onError(ex);

      public void OnNext()
      {
        lock (this.Queue.@lock)
        {
          if (this.Queue.CurrentObserver == this)
            this.Queue.CallNext();
          if (this.Node == null)
            return;
          this.Queue.PendingObservers.Remove(this.Node);
          this.Node = (LinkedListNode<ObservableQueue.Observer>) null;
        }
      }
    }
  }
}
