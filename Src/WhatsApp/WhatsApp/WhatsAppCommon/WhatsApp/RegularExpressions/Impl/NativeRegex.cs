// Decompiled with JetBrains decompiler
// Type: WhatsApp.RegularExpressions.Impl.NativeRegex
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using WhatsAppNative;


namespace WhatsApp.RegularExpressions.Impl
{
  public class NativeRegex
  {
    private IRegex native;

    public NativeRegex(string expr, WhatsApp.RegularExpressions.RegexOptions options)
    {
      this.native = (IRegex) NativeInterfaces.CreateInstance<WhatsAppNative.NativeRegex>();
      WhatsAppNative.RegexOptions Options = (WhatsAppNative.RegexOptions) 0;
      if ((options & WhatsApp.RegularExpressions.RegexOptions.IgnoreCase) != WhatsApp.RegularExpressions.RegexOptions.Default)
        Options |= WhatsAppNative.RegexOptions.IgnoreCase;
      this.native.Initialize(expr, Options);
    }

    ~NativeRegex()
    {
      if (this.native == null)
        return;
      this.native.Dispose();
      this.native = (IRegex) null;
    }

    public WhatsApp.RegularExpressions.Match Match(string input, int offset, int length)
    {
      WhatsApp.RegularExpressions.Match r = new WhatsApp.RegularExpressions.Match();
      IMatch match = this.native.GetMatch(input, offset, length);
      if (match == null)
        return r;
      try
      {
        r.Success = true;
        WhatsAppNative.Range range = match.GetRange();
        r.Index = (int) range.Index;
        r.Length = (int) range.Length;
        r.valueObject.SetValueLazy((Func<string>) (() => input.Substring(r.Index, r.Length)));
        uint groupCount = match.GetGroupCount();
        if (groupCount != 0U)
        {
          List<Group> groupList = new List<Group>();
          for (int Idx = 0; (long) Idx < (long) groupCount; ++Idx)
          {
            WhatsAppNative.Range group1 = match.GetGroup(Idx);
            int snapIdx = (int) group1.Index;
            int snapLength = (int) group1.Length;
            Group group2 = new Group();
            group2.valueObject.SetValueLazy((Func<string>) (() => snapIdx >= 0 ? input.Substring(snapIdx, snapLength) : (string) null));
            groupList.Add(group2);
          }
          r.Groups = groupList.ToArray();
        }
      }
      finally
      {
        match.Dispose();
      }
      return r;
    }
  }
}
