// Decompiled with JetBrains decompiler
// Type: WhatsApp.PopupManager
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Navigation;


namespace WhatsApp
{
  public class PopupManager
  {
    private Popup popup;
    private PhoneApplicationPage page;
    private PageOrientation orientation;
    private Uri uri;
    private List<Action> dtors = new List<Action>();
    private static LinkedList<Popup> allPopups = new LinkedList<Popup>();
    private static readonly double screenWidth = Application.Current.Host.Content.ActualWidth;
    private static readonly double screenHeight = Application.Current.Host.Content.ActualHeight;

    public Popup Popup => this.popup;

    public PageOrientation Orientation
    {
      get => this.orientation;
      set
      {
        if (this.orientation == value)
          return;
        this.orientation = value;
        this.CalculateSize();
        this.NotifyOrientationChanged();
      }
    }

    public static IEnumerable<Popup> OpenPopups
    {
      get => ((IEnumerable<Popup>) PopupManager.allPopups.ToArray<Popup>()).AsEnumerable<Popup>();
    }

    public event EventHandler<EventArgs> OrientationChanged;

    protected void NotifyOrientationChanged()
    {
      if (this.OrientationChanged == null)
        return;
      this.OrientationChanged((object) this, new EventArgs());
    }

    public event EventHandler BackKeyPressed;

    protected void NotifyBackKeyPressed()
    {
      if (this.BackKeyPressed == null)
        return;
      this.BackKeyPressed((object) this, new EventArgs());
    }

    public event EventHandler<EventArgs> BackKeyHandled;

    protected void NotifyBackKeyHandled()
    {
      if (this.BackKeyHandled == null)
        return;
      this.BackKeyHandled((object) this, new EventArgs());
    }

    public event EventHandler<EventArgs> Closed;

    protected void NotifyClosed()
    {
      if (this.Closed == null)
        return;
      this.Closed((object) this, new EventArgs());
    }

    public static void Register(Popup pop)
    {
      LinkedListNode<Popup> node = PopupManager.allPopups.AddLast(pop);
      EventHandler eh = (EventHandler) null;
      eh = (EventHandler) ((sender, args) =>
      {
        PopupManager.allPopups.Remove(node);
        pop.Closed -= eh;
      });
      pop.Closed += eh;
    }

    public PopupManager(Popup pop, bool attachToBackStack)
    {
      this.popup = pop;
      PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
      if (frame != null)
      {
        this.page = frame.Content as PhoneApplicationPage;
        if (this.page != null)
        {
          if (attachToBackStack)
          {
            IDisposable sub = BackKeyBroker.Get(this.page, 0).Subscribe<CancelEventArgs>((Action<CancelEventArgs>) (_ => this.OnBackKeyPressed((object) this.page, _)));
            this.dtors.Add((Action) (() => sub.Dispose()));
            this.uri = this.page.NavigationService.Source;
            frame.JournalEntryRemoved += new EventHandler<JournalEntryRemovedEventArgs>(this.frame_JournalEntryRemoved);
            this.dtors.Add((Action) (() => frame.JournalEntryRemoved -= new EventHandler<JournalEntryRemovedEventArgs>(this.frame_JournalEntryRemoved)));
          }
          this.page.OrientationChanged += new EventHandler<OrientationChangedEventArgs>(this.OnPageOrientationChanged);
        }
      }
      LinkedListNode<Popup> node = PopupManager.allPopups.AddLast(pop);
      this.dtors.Add((Action) (() => PopupManager.allPopups.Remove(node)));
      this.dtors.Add((Action) (() => this.popup.Closed -= new EventHandler(this.Popup_Closed)));
      this.popup.Closed += new EventHandler(this.Popup_Closed);
    }

    private void frame_JournalEntryRemoved(object sender, JournalEntryRemovedEventArgs e)
    {
      if (!(this.uri == e.Entry.Source))
        return;
      this.popup.IsOpen = false;
    }

    public void Show()
    {
      if (this.page != null)
        this.Orientation = this.page.Orientation;
      this.popup.IsOpen = true;
    }

    private void Popup_Closed(object sender, EventArgs e)
    {
      if (this.page != null)
        this.page.OrientationChanged -= new EventHandler<OrientationChangedEventArgs>(this.OnPageOrientationChanged);
      this.dtors.ForEach((Action<Action>) (a => a()));
      this.dtors.Clear();
      this.NotifyClosed();
    }

    private void OnBackKeyPressed(object sender, CancelEventArgs e)
    {
      this.NotifyBackKeyPressed();
      e.Cancel = true;
      this.popup.IsOpen = false;
      this.NotifyBackKeyHandled();
    }

    private void OnPageOrientationChanged(object sender, OrientationChangedEventArgs e)
    {
      this.Orientation = e.Orientation;
    }

    private void CalculateSize()
    {
      double traySize = 0.0;
      Func<double> func = (Func<double>) (() => !SystemTray.IsVisible ? 0.0 : traySize);
      double num = this.page == null || this.page.ApplicationBar == null || !this.page.ApplicationBar.IsVisible ? 0.0 : this.page.ApplicationBar.DefaultSize;
      if (this.page != null && this.page.ApplicationBar == null)
        Deployment.Current.Dispatcher.RunAfterDelay(TimeSpan.FromMilliseconds(500.0), (Action) (() =>
        {
          if (!this.popup.IsOpen || this.page.ApplicationBar == null)
            return;
          this.CalculateSize();
          this.NotifyOrientationChanged();
        }));
      if ((this.Orientation & PageOrientation.Portrait) == PageOrientation.None)
      {
        traySize = 72.0;
        switch (this.Orientation)
        {
          case PageOrientation.Landscape:
          case PageOrientation.LandscapeLeft:
            this.popup.Child.RenderTransform = (Transform) new CompositeTransform()
            {
              Rotation = 90.0,
              TranslateX = PopupManager.screenWidth
            };
            this.popup.HorizontalOffset = 0.0;
            this.popup.VerticalOffset = func();
            break;
          case PageOrientation.LandscapeRight:
            this.popup.Child.RenderTransform = (Transform) new CompositeTransform()
            {
              Rotation = -90.0,
              TranslateY = PopupManager.screenHeight
            };
            this.popup.HorizontalOffset = 0.0;
            this.popup.VerticalOffset = -num;
            break;
        }
        this.popup.Height = PopupManager.screenHeight - num - func();
        this.popup.Width = PopupManager.screenWidth;
        FrameworkElement child = this.popup.Child as FrameworkElement;
        child.Width = this.popup.Height;
        child.Height = this.popup.Width;
      }
      else
      {
        traySize = 32.0;
        this.popup.Child.RenderTransform = (Transform) null;
        this.popup.HorizontalOffset = 0.0;
        this.popup.VerticalOffset = func();
        this.popup.Width = PopupManager.screenWidth;
        this.popup.Height = PopupManager.screenHeight - num - func();
        FrameworkElement child = this.popup.Child as FrameworkElement;
        child.Width = this.popup.Width;
        child.Height = this.popup.Height;
      }
    }
  }
}
