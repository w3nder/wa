// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Core.NameFilter
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Core
{
  public class NameFilter : IScanFilter
  {
    private readonly List<Regex> exclusions_;
    private readonly string filter_;
    private readonly List<Regex> inclusions_;

    public NameFilter(string filter)
    {
      this.filter_ = filter;
      this.inclusions_ = new List<Regex>();
      this.exclusions_ = new List<Regex>();
      this.Compile();
    }

    public bool IsMatch(string name) => this.IsIncluded(name) && !this.IsExcluded(name);

    public static bool IsValidExpression(string expression)
    {
      bool flag = true;
      try
      {
        Regex regex = new Regex(expression, RegexOptions.IgnoreCase | RegexOptions.Singleline);
      }
      catch
      {
        flag = false;
      }
      return flag;
    }

    public static bool IsValidFilterExpression(string toTest)
    {
      if (toTest == null)
        throw new ArgumentNullException(nameof (toTest));
      bool flag = true;
      try
      {
        string[] strArray = toTest.Split(';');
        for (int index = 0; index < strArray.Length; ++index)
        {
          if (!string.IsNullOrEmpty(strArray[index]))
          {
            string pattern;
            switch (strArray[index][0])
            {
              case '+':
                pattern = strArray[index].Substring(1, strArray[index].Length - 1);
                break;
              case '-':
                pattern = strArray[index].Substring(1, strArray[index].Length - 1);
                break;
              default:
                pattern = strArray[index];
                break;
            }
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
          }
        }
      }
      catch (Exception ex)
      {
        flag = false;
      }
      return flag;
    }

    public override string ToString() => this.filter_;

    public bool IsIncluded(string name)
    {
      bool flag = false;
      if (this.inclusions_.Count == 0)
      {
        flag = true;
      }
      else
      {
        foreach (Regex inclusion in this.inclusions_)
        {
          if (inclusion.IsMatch(name))
          {
            flag = true;
            break;
          }
        }
      }
      return flag;
    }

    public bool IsExcluded(string name)
    {
      bool flag = false;
      foreach (Regex exclusion in this.exclusions_)
      {
        if (exclusion.IsMatch(name))
        {
          flag = true;
          break;
        }
      }
      return flag;
    }

    private void Compile()
    {
      if (this.filter_ == null)
        return;
      string[] strArray = this.filter_.Split(';');
      for (int index = 0; index < strArray.Length; ++index)
      {
        if (!string.IsNullOrEmpty(strArray[index]))
        {
          bool flag = strArray[index][0] != '-';
          string pattern;
          switch (strArray[index][0])
          {
            case '+':
              pattern = strArray[index].Substring(1, strArray[index].Length - 1);
              break;
            case '-':
              pattern = strArray[index].Substring(1, strArray[index].Length - 1);
              break;
            default:
              pattern = strArray[index];
              break;
          }
          if (flag)
            this.inclusions_.Add(new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline));
          else
            this.exclusions_.Add(new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline));
        }
      }
    }
  }
}
