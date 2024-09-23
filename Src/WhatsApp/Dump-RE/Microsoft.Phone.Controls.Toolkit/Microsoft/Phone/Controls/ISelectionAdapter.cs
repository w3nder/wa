// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.ISelectionAdapter
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System.Collections;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public interface ISelectionAdapter
  {
    object SelectedItem { get; set; }

    event SelectionChangedEventHandler SelectionChanged;

    IEnumerable ItemsSource { get; set; }

    event RoutedEventHandler Commit;

    event RoutedEventHandler Cancel;

    void HandleKeyDown(KeyEventArgs e);

    AutomationPeer CreateAutomationPeer();
  }
}
