// Decompiled with JetBrains decompiler
// Type: System.Net.Mail.MailAddressParser
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Collections.Generic;
using System.Net.Mime;

#nullable disable
namespace System.Net.Mail
{
  internal static class MailAddressParser
  {
    internal static MailAddress ParseAddress(string data)
    {
      int index = data.Length - 1;
      return MailAddressParser.ParseAddress(data, false, ref index);
    }

    internal static IList<MailAddress> ParseMultipleAddresses(string data)
    {
      IList<MailAddress> multipleAddresses = (IList<MailAddress>) new List<MailAddress>();
      for (int index = data.Length - 1; index >= 0; --index)
        multipleAddresses.Insert(0, MailAddressParser.ParseAddress(data, true, ref index));
      return multipleAddresses;
    }

    private static MailAddress ParseAddress(
      string data,
      bool expectMultipleAddresses,
      ref int index)
    {
      index = MailAddressParser.ReadCfwsAndThrowIfIncomplete(data, index);
      bool expectAngleBracket = false;
      if ((int) data[index] == (int) MailBnfHelper.EndAngleBracket)
      {
        expectAngleBracket = true;
        --index;
      }
      string domain = MailAddressParser.ParseDomain(data, ref index);
      if ((int) data[index] != (int) MailBnfHelper.At)
        throw new FormatException(SR.GetString("MailAddressInvalidFormat"));
      --index;
      string localPart = MailAddressParser.ParseLocalPart(data, ref index, expectAngleBracket, expectMultipleAddresses);
      if (expectAngleBracket)
      {
        if (index >= 0 && (int) data[index] == (int) MailBnfHelper.StartAngleBracket)
        {
          --index;
          index = WhitespaceReader.ReadFwsReverse(data, index);
        }
        else
          throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", (object) (char) (index >= 0 ? (int) data[index] : (int) MailBnfHelper.EndAngleBracket)));
      }
      return new MailAddress(index < 0 || expectMultipleAddresses && (int) data[index] == (int) MailBnfHelper.Comma ? string.Empty : MailAddressParser.ParseDisplayName(data, ref index, expectMultipleAddresses), localPart, domain);
    }

    private static int ReadCfwsAndThrowIfIncomplete(string data, int index)
    {
      index = WhitespaceReader.ReadCfwsReverse(data, index);
      return index >= 0 ? index : throw new FormatException(SR.GetString("MailAddressInvalidFormat"));
    }

    private static string ParseDomain(string data, ref int index)
    {
      index = MailAddressParser.ReadCfwsAndThrowIfIncomplete(data, index);
      int num = index;
      index = (int) data[index] != (int) MailBnfHelper.EndSquareBracket ? DotAtomReader.ReadReverse(data, index) : DomainLiteralReader.ReadReverse(data, index);
      string input = data.Substring(index + 1, num - index);
      index = MailAddressParser.ReadCfwsAndThrowIfIncomplete(data, index);
      return MailAddressParser.NormalizeOrThrow(input);
    }

    private static string ParseLocalPart(
      string data,
      ref int index,
      bool expectAngleBracket,
      bool expectMultipleAddresses)
    {
      index = MailAddressParser.ReadCfwsAndThrowIfIncomplete(data, index);
      int num = index;
      if ((int) data[index] == (int) MailBnfHelper.Quote)
      {
        index = QuotedStringFormatReader.ReadReverseQuoted(data, index, true);
      }
      else
      {
        index = DotAtomReader.ReadReverse(data, index);
        if (index >= 0 && !MailBnfHelper.Whitespace.Contains(data[index]) && (int) data[index] != (int) MailBnfHelper.EndComment && (!expectAngleBracket || (int) data[index] != (int) MailBnfHelper.StartAngleBracket) && (!expectMultipleAddresses || (int) data[index] != (int) MailBnfHelper.Comma) && (int) data[index] != (int) MailBnfHelper.Quote)
          throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", (object) data[index]));
      }
      string input = data.Substring(index + 1, num - index);
      index = WhitespaceReader.ReadCfwsReverse(data, index);
      return MailAddressParser.NormalizeOrThrow(input);
    }

    private static string ParseDisplayName(
      string data,
      ref int index,
      bool expectMultipleAddresses)
    {
      int index1 = WhitespaceReader.ReadCfwsReverse(data, index);
      string input;
      if (index1 >= 0 && (int) data[index1] == (int) MailBnfHelper.Quote)
      {
        index = QuotedStringFormatReader.ReadReverseQuoted(data, index1, true);
        int startIndex = index + 2;
        input = data.Substring(startIndex, index1 - startIndex);
        index = WhitespaceReader.ReadCfwsReverse(data, index);
        if (index >= 0 && (!expectMultipleAddresses || (int) data[index] != (int) MailBnfHelper.Comma))
          throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", (object) data[index]));
      }
      else
      {
        int num = index;
        index = QuotedStringFormatReader.ReadReverseUnQuoted(data, index, true, expectMultipleAddresses);
        input = data.Substring(index + 1, num - index).Trim();
      }
      return MailAddressParser.NormalizeOrThrow(input);
    }

    internal static string NormalizeOrThrow(string input)
    {
      try
      {
        return input.Normalize(NormalizationForm.FormC);
      }
      catch (ArgumentException ex)
      {
        throw new FormatException(SR.GetString("MailAddressInvalidFormat"), (Exception) ex);
      }
    }
  }
}
