// Decompiled with JetBrains decompiler
// Type: WhatsApp.TreeInForestSubject`1
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
  public class TreeInForestSubject<T> : ISubject<T>, ISubject<T, T>, IObserver<T>, IObservable<T>
  {
    private object @lock = new object();
    private LinkedList<IObserver<T>> observers = new LinkedList<IObserver<T>>();
    private LinkedList<Action<IObserver<T>>> savedValues = new LinkedList<Action<IObserver<T>>>();
    private bool complete;
    private static IDisposable NoCancel = (IDisposable) new DisposableAction((Action) (() => { }));

    private void ProcessObserver(Action<IObserver<T>> onObserver)
    {
      IObserver<T> observer = (IObserver<T>) null;
      lock (this.@lock)
      {
        if (!this.complete)
        {
          if (this.observers.Count != 0)
          {
            LinkedListNode<IObserver<T>> first = this.observers.First;
            observer = first.Value;
            first.List.Remove(first);
          }
          else
            this.savedValues.AddLast(onObserver);
        }
      }
      if (observer == null)
        return;
      onObserver(observer);
    }

    public void OnNext(T value)
    {
      this.ProcessObserver((Action<IObserver<T>>) (obs =>
      {
        obs.OnNext(value);
        obs.OnCompleted();
      }));
    }

    public void OnError(Exception ex)
    {
      this.ProcessObserver((Action<IObserver<T>>) (obs =>
      {
        obs.OnError(ex);
        obs.OnCompleted();
      }));
    }

    public void OnCompleted()
    {
      IObserver<T>[] observerArray = (IObserver<T>[]) null;
      lock (this.@lock)
      {
        this.complete = true;
        observerArray = this.observers.ToArray<IObserver<T>>();
        this.observers.Clear();
      }
      if (observerArray == null)
        return;
      foreach (IObserver<T> observer in observerArray)
        observer.OnCompleted();
    }

    public IDisposable Subscribe(IObserver<T> obs)
    {
      LinkedListNode<IObserver<T>> node = (LinkedListNode<IObserver<T>>) null;
      Action<IObserver<T>> action = (Action<IObserver<T>>) null;
      lock (this.@lock)
      {
        if (this.complete)
        {
          obs.OnCompleted();
          return TreeInForestSubject<T>.NoCancel;
        }
        if (this.savedValues.Count != 0)
        {
          LinkedListNode<Action<IObserver<T>>> first = this.savedValues.First;
          action = first.Value;
          first.List.Remove(first);
        }
        else
          node = this.observers.AddLast(obs);
      }
      if (action == null)
        return (IDisposable) new DisposableAction((Action) (() =>
        {
          lock (this.@lock)
          {
            if (node == null)
              return;
            if (node.List != null)
              node.List.Remove(node);
            node = (LinkedListNode<IObserver<T>>) null;
          }
        }));
      action(obs);
      return TreeInForestSubject<T>.NoCancel;
    }
  }
}
