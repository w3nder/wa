// Decompiled with JetBrains decompiler
// Type: WhatsApp.BackKeyBroker
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

#nullable disable
namespace WhatsApp
{
  public class BackKeyBroker
  {
    private LinkedList<BackKeyBroker.ObserverState> Observers = new LinkedList<BackKeyBroker.ObserverState>();
    public static readonly DependencyProperty BackKeyBrokerProperty = DependencyProperty.RegisterAttached(nameof (BackKeyBroker), typeof (BackKeyBroker), typeof (BackKeyBroker), new PropertyMetadata((object) null, (PropertyChangedCallback) ((sender, args) => { })));

    private BackKeyBroker(PhoneApplicationPage page)
    {
      page.BackKeyPress += (EventHandler<CancelEventArgs>) ((sender, args) =>
      {
        for (LinkedListNode<BackKeyBroker.ObserverState> linkedListNode = this.Observers.First; linkedListNode != null && !args.Cancel; linkedListNode = linkedListNode.Next)
          linkedListNode.Value.Observer.OnNext(args);
      });
    }

    public IObservable<CancelEventArgs> GetObservable(PhoneApplicationPage page, int priority)
    {
      return Observable.Create<CancelEventArgs>((Func<IObserver<CancelEventArgs>, Action>) (observer =>
      {
        BackKeyBroker.ObserverState observerState = new BackKeyBroker.ObserverState()
        {
          Observer = observer,
          Priority = priority
        };
        LinkedListNode<BackKeyBroker.ObserverState> myNode = (LinkedListNode<BackKeyBroker.ObserverState>) null;
        for (LinkedListNode<BackKeyBroker.ObserverState> node = this.Observers.First; node != null; node = node.Next)
        {
          if (node.Value.Priority >= priority)
          {
            myNode = this.Observers.AddBefore(node, observerState);
            break;
          }
        }
        if (myNode == null)
          myNode = this.Observers.AddLast(observerState);
        return (Action) (() =>
        {
          if (myNode == null)
            return;
          this.Observers.Remove(myNode);
        });
      }));
    }

    public static IObservable<CancelEventArgs> Get(PhoneApplicationPage page, int priority)
    {
      if (!(page.GetValue(BackKeyBroker.BackKeyBrokerProperty) is BackKeyBroker backKeyBroker))
      {
        backKeyBroker = new BackKeyBroker(page);
        page.SetValue(BackKeyBroker.BackKeyBrokerProperty, (object) backKeyBroker);
      }
      return backKeyBroker.GetObservable(page, priority);
    }

    private class ObserverState
    {
      public IObserver<CancelEventArgs> Observer;
      public int Priority;
    }
  }
}
