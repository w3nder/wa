// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.Data.PhoneHelper
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System;
using System.Xml;

#nullable disable
namespace Coding4Fun.Phone.Controls.Data
{
  public class PhoneHelper
  {
    private const string AppManifestName = "WMAppManifest.xml";
    private const string AppNodeName = "App";

    public static string GetAppAttribute(string attributeName)
    {
      try
      {
        using (XmlReader xmlReader = XmlReader.Create("WMAppManifest.xml", new XmlReaderSettings()
        {
          XmlResolver = (XmlResolver) new XmlXapResolver()
        }))
        {
          xmlReader.ReadToDescendant("App");
          return xmlReader.IsStartElement() ? xmlReader.GetAttribute(attributeName) : throw new FormatException("WMAppManifest.xml is missing App");
        }
      }
      catch (Exception ex)
      {
        return "";
      }
    }
  }
}
