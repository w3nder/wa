// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Zip.ZipNameTransform
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using ICSharpCode.SharpZipLib.Silverlight.Core;
using ICSharpCode.SharpZipLib.Silverlight.Zip;
using System;
using System.IO;
using System.Text;

#nullable disable
namespace ICSharpCode.SharpZipLib.Zip
{
  public class ZipNameTransform : INameTransform
  {
    private string trimPrefix_;
    private static readonly char[] InvalidEntryChars;
    private static readonly char[] InvalidEntryCharsRelaxed;

    public ZipNameTransform()
    {
    }

    public ZipNameTransform(string trimPrefix) => this.TrimPrefix = trimPrefix;

    static ZipNameTransform()
    {
      char[] invalidPathChars = Path.GetInvalidPathChars();
      int length1 = invalidPathChars.Length + 2;
      ZipNameTransform.InvalidEntryCharsRelaxed = new char[length1];
      Array.Copy((Array) invalidPathChars, 0, (Array) ZipNameTransform.InvalidEntryCharsRelaxed, 0, invalidPathChars.Length);
      ZipNameTransform.InvalidEntryCharsRelaxed[length1 - 1] = '*';
      ZipNameTransform.InvalidEntryCharsRelaxed[length1 - 2] = '?';
      int length2 = invalidPathChars.Length + 4;
      ZipNameTransform.InvalidEntryChars = new char[length2];
      Array.Copy((Array) invalidPathChars, 0, (Array) ZipNameTransform.InvalidEntryChars, 0, invalidPathChars.Length);
      ZipNameTransform.InvalidEntryChars[length2 - 1] = ':';
      ZipNameTransform.InvalidEntryChars[length2 - 2] = '\\';
      ZipNameTransform.InvalidEntryChars[length2 - 3] = '*';
      ZipNameTransform.InvalidEntryChars[length2 - 4] = '?';
    }

    public string TransformDirectory(string name)
    {
      name = this.TransformFile(name);
      if (name.Length <= 0)
        throw new ZipException("Cannot have an empty directory name");
      if (!name.EndsWith("/"))
        name += "/";
      return name;
    }

    public string TransformFile(string name)
    {
      if (name != null)
      {
        string lower = name.ToLower();
        if (this.trimPrefix_ != null && lower.IndexOf(this.trimPrefix_) == 0)
          name = name.Substring(this.trimPrefix_.Length);
        if (Path.IsPathRooted(name))
          name = name.Substring(Path.GetPathRoot(name).Length);
        name = name.Replace("\\", "/");
        while (name.Length > 0 && name[0] == '/')
          name = name.Remove(0, 1);
        name = ZipNameTransform.MakeValidName(name, '_');
      }
      else
        name = string.Empty;
      return name;
    }

    public string TrimPrefix
    {
      get => this.trimPrefix_;
      set
      {
        this.trimPrefix_ = value;
        if (this.trimPrefix_ == null)
          return;
        this.trimPrefix_ = this.trimPrefix_.ToLower();
      }
    }

    private static string MakeValidName(string name, char replacement)
    {
      int index = name.IndexOfAny(ZipNameTransform.InvalidEntryChars);
      if (index > 0)
      {
        StringBuilder stringBuilder = new StringBuilder(name);
        for (; index >= 0; index = index < name.Length ? name.IndexOfAny(ZipNameTransform.InvalidEntryChars, index + 1) : -1)
          stringBuilder[index] = replacement;
        name = stringBuilder.ToString();
      }
      return name;
    }

    public static bool IsValidName(string name, bool relaxed)
    {
      bool flag = name != null;
      if (flag)
        flag = !relaxed ? name.IndexOfAny(ZipNameTransform.InvalidEntryChars) < 0 && name.IndexOf('/') != 0 : name.IndexOfAny(ZipNameTransform.InvalidEntryCharsRelaxed) < 0;
      return flag;
    }

    public static bool IsValidName(string name)
    {
      return name != null && name.IndexOfAny(ZipNameTransform.InvalidEntryChars) < 0 && name.IndexOf('/') != 0;
    }
  }
}
