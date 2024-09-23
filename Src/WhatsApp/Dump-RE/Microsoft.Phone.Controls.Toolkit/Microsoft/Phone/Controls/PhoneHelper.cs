// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.PhoneHelper
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

#nullable disable
namespace Microsoft.Phone.Controls
{
  internal static class PhoneHelper
  {
    public const double SipLandscapeHeight = 259.0;
    public const double SipPortraitHeight = 339.0;
    public const double SipTextCompletionHeight = 62.0;

    public static bool TryGetPhoneApplicationFrame(out PhoneApplicationFrame phoneApplicationFrame)
    {
      phoneApplicationFrame = Application.Current.RootVisual as PhoneApplicationFrame;
      return phoneApplicationFrame != null;
    }

    public static bool IsPortrait(this PhoneApplicationFrame phoneApplicationFrame)
    {
      return ((PageOrientation) 13 & phoneApplicationFrame.Orientation) == phoneApplicationFrame.Orientation;
    }

    public static double GetUsefulWidth(this PhoneApplicationFrame phoneApplicationFrame)
    {
      return !phoneApplicationFrame.IsPortrait() ? phoneApplicationFrame.ActualHeight : phoneApplicationFrame.ActualWidth;
    }

    public static double GetUsefulHeight(this PhoneApplicationFrame phoneApplicationFrame)
    {
      return !phoneApplicationFrame.IsPortrait() ? phoneApplicationFrame.ActualWidth : phoneApplicationFrame.ActualHeight;
    }

    public static Size GetUsefulSize(this PhoneApplicationFrame phoneApplicationFrame)
    {
      return new Size(phoneApplicationFrame.GetUsefulWidth(), phoneApplicationFrame.GetUsefulHeight());
    }

    private static bool TryGetFocusedTextBox(out TextBox textBox)
    {
      textBox = FocusManager.GetFocusedElement() as TextBox;
      return textBox != null;
    }

    public static bool IsSipShown() => PhoneHelper.TryGetFocusedTextBox(out TextBox _);

    public static bool IsSipTextCompletionShown(this TextBox textBox)
    {
      if (textBox.InputScope == null)
        return false;
      foreach (InputScopeName name in (IEnumerable) textBox.InputScope.Names)
      {
        switch (name.NameValue)
        {
          case InputScopeNameValue.Text:
          case InputScopeNameValue.Chat:
            return true;
          default:
            continue;
        }
      }
      return false;
    }

    public static Size GetSipCoveredSize(this PhoneApplicationFrame phoneApplicationFrame)
    {
      if (!PhoneHelper.IsSipShown())
        return new Size(0.0, 0.0);
      double usefulWidth = phoneApplicationFrame.GetUsefulWidth();
      double height = phoneApplicationFrame.IsPortrait() ? 339.0 : 259.0;
      TextBox textBox;
      if (PhoneHelper.TryGetFocusedTextBox(out textBox) && textBox.IsSipTextCompletionShown())
        height += 62.0;
      return new Size(usefulWidth, height);
    }

    public static Size GetSipUncoveredSize(this PhoneApplicationFrame phoneApplicationFrame)
    {
      return new Size(phoneApplicationFrame.GetUsefulWidth(), phoneApplicationFrame.GetUsefulHeight() - phoneApplicationFrame.GetSipCoveredSize().Height);
    }
  }
}
