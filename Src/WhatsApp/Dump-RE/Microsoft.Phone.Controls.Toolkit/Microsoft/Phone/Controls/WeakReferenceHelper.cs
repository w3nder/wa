// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.WeakReferenceHelper
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public static class WeakReferenceHelper
  {
    public static bool ContainsTarget(IEnumerable<WeakReference> references, object target)
    {
      if (references == null)
        return false;
      foreach (WeakReference reference in references)
      {
        if (target == reference.Target)
          return true;
      }
      return false;
    }

    public static bool TryRemoveTarget(IList<WeakReference> references, object target)
    {
      if (references == null)
        return false;
      for (int index = 0; index < references.Count; ++index)
      {
        if (references[index].Target == target)
        {
          references.RemoveAt(index);
          return true;
        }
      }
      return false;
    }

    public static void RemoveNullTargetReferences(IList<WeakReference> references)
    {
      if (references == null)
        return;
      for (int index = 0; index < references.Count; ++index)
      {
        if (references[index].Target == null)
        {
          references.RemoveAt(index);
          --index;
        }
      }
    }
  }
}
