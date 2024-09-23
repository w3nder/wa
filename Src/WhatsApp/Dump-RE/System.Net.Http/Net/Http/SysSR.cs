// Decompiled with JetBrains decompiler
// Type: System.Net.Http.SysSR
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace System.Net.Http
{
  [DebuggerNonUserCode]
  [CompilerGenerated]
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  internal class SysSR
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal SysSR()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (object.ReferenceEquals((object) SysSR.resourceMan, (object) null))
          SysSR.resourceMan = new ResourceManager("System.Net.Http.SysSR", typeof (SysSR).Assembly);
        return SysSR.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => SysSR.resourceCulture;
      set => SysSR.resourceCulture = value;
    }

    internal static string http_capabilitynotsupported
    {
      get
      {
        return SysSR.ResourceManager.GetString(nameof (http_capabilitynotsupported), SysSR.resourceCulture);
      }
    }

    internal static string InvalidHeaderName
    {
      get => SysSR.ResourceManager.GetString(nameof (InvalidHeaderName), SysSR.resourceCulture);
    }

    internal static string MailAddressInvalidFormat
    {
      get
      {
        return SysSR.ResourceManager.GetString(nameof (MailAddressInvalidFormat), SysSR.resourceCulture);
      }
    }

    internal static string MailHeaderFieldInvalidCharacter
    {
      get
      {
        return SysSR.ResourceManager.GetString(nameof (MailHeaderFieldInvalidCharacter), SysSR.resourceCulture);
      }
    }

    internal static string MailHeaderFieldMalformedHeader
    {
      get
      {
        return SysSR.ResourceManager.GetString(nameof (MailHeaderFieldMalformedHeader), SysSR.resourceCulture);
      }
    }

    internal static string net_connarg
    {
      get => SysSR.ResourceManager.GetString(nameof (net_connarg), SysSR.resourceCulture);
    }

    internal static string net_fromto
    {
      get => SysSR.ResourceManager.GetString(nameof (net_fromto), SysSR.resourceCulture);
    }

    internal static string net_invalid_host
    {
      get => SysSR.ResourceManager.GetString(nameof (net_invalid_host), SysSR.resourceCulture);
    }

    internal static string net_rangetoosmall
    {
      get => SysSR.ResourceManager.GetString(nameof (net_rangetoosmall), SysSR.resourceCulture);
    }

    internal static string net_rangetype
    {
      get => SysSR.ResourceManager.GetString(nameof (net_rangetype), SysSR.resourceCulture);
    }

    internal static string net_writestarted
    {
      get => SysSR.ResourceManager.GetString(nameof (net_writestarted), SysSR.resourceCulture);
    }

    internal static string net_wrongversion
    {
      get => SysSR.ResourceManager.GetString(nameof (net_wrongversion), SysSR.resourceCulture);
    }
  }
}
