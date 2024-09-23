// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Design.ApplicationIdCredentialsProviderConverter
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.ComponentModel;
using System.Globalization;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Design
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class ApplicationIdCredentialsProviderConverter : TypeConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof (string);
    }

    public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value)
    {
      return value is string applicationId ? (object) new ApplicationIdCredentialsProvider(applicationId) : throw new NotSupportedException(ExceptionStrings.TypeConverter_InvalidApplicationIdCredentialsProvider);
    }
  }
}
