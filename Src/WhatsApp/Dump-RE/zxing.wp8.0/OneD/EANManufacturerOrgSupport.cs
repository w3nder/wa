// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.EANManufacturerOrgSupport
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;

#nullable disable
namespace ZXing.OneD
{
  /// <summary>
  /// Records EAN prefix to GS1 Member Organization, where the member organization
  /// correlates strongly with a country. This is an imperfect means of identifying
  /// a country of origin by EAN-13 barcode value. See
  /// <a href="http://en.wikipedia.org/wiki/List_of_GS1_country_codes">
  /// http://en.wikipedia.org/wiki/List_of_GS1_country_codes</a>.
  /// 
  /// <author>Sean Owen</author>
  /// </summary>
  internal sealed class EANManufacturerOrgSupport
  {
    private List<int[]> ranges = new List<int[]>();
    private List<string> countryIdentifiers = new List<string>();

    internal string lookupCountryIdentifier(string productCode)
    {
      this.initIfNeeded();
      int num1 = int.Parse(productCode.Substring(0, 3));
      int count = this.ranges.Count;
      for (int index = 0; index < count; ++index)
      {
        int[] range = this.ranges[index];
        int num2 = range[0];
        if (num1 < num2)
          return (string) null;
        int num3 = range.Length == 1 ? num2 : range[1];
        if (num1 <= num3)
          return this.countryIdentifiers[index];
      }
      return (string) null;
    }

    private void add(int[] range, string id)
    {
      this.ranges.Add(range);
      this.countryIdentifiers.Add(id);
    }

    private void initIfNeeded()
    {
      if (this.ranges.Count != 0)
        return;
      this.add(new int[2]{ 0, 19 }, "US/CA");
      this.add(new int[2]{ 30, 39 }, "US");
      this.add(new int[2]{ 60, 139 }, "US/CA");
      this.add(new int[2]{ 300, 379 }, "FR");
      this.add(new int[1]{ 380 }, "BG");
      this.add(new int[1]{ 383 }, "SI");
      this.add(new int[1]{ 385 }, "HR");
      this.add(new int[1]{ 387 }, "BA");
      this.add(new int[2]{ 400, 440 }, "DE");
      this.add(new int[2]{ 450, 459 }, "JP");
      this.add(new int[2]{ 460, 469 }, "RU");
      this.add(new int[1]{ 471 }, "TW");
      this.add(new int[1]{ 474 }, "EE");
      this.add(new int[1]{ 475 }, "LV");
      this.add(new int[1]{ 476 }, "AZ");
      this.add(new int[1]{ 477 }, "LT");
      this.add(new int[1]{ 478 }, "UZ");
      this.add(new int[1]{ 479 }, "LK");
      this.add(new int[1]{ 480 }, "PH");
      this.add(new int[1]{ 481 }, "BY");
      this.add(new int[1]{ 482 }, "UA");
      this.add(new int[1]{ 484 }, "MD");
      this.add(new int[1]{ 485 }, "AM");
      this.add(new int[1]{ 486 }, "GE");
      this.add(new int[1]{ 487 }, "KZ");
      this.add(new int[1]{ 489 }, "HK");
      this.add(new int[2]{ 490, 499 }, "JP");
      this.add(new int[2]{ 500, 509 }, "GB");
      this.add(new int[1]{ 520 }, "GR");
      this.add(new int[1]{ 528 }, "LB");
      this.add(new int[1]{ 529 }, "CY");
      this.add(new int[1]{ 531 }, "MK");
      this.add(new int[1]{ 535 }, "MT");
      this.add(new int[1]{ 539 }, "IE");
      this.add(new int[2]{ 540, 549 }, "BE/LU");
      this.add(new int[1]{ 560 }, "PT");
      this.add(new int[1]{ 569 }, "IS");
      this.add(new int[2]{ 570, 579 }, "DK");
      this.add(new int[1]{ 590 }, "PL");
      this.add(new int[1]{ 594 }, "RO");
      this.add(new int[1]{ 599 }, "HU");
      this.add(new int[2]{ 600, 601 }, "ZA");
      this.add(new int[1]{ 603 }, "GH");
      this.add(new int[1]{ 608 }, "BH");
      this.add(new int[1]{ 609 }, "MU");
      this.add(new int[1]{ 611 }, "MA");
      this.add(new int[1]{ 613 }, "DZ");
      this.add(new int[1]{ 616 }, "KE");
      this.add(new int[1]{ 618 }, "CI");
      this.add(new int[1]{ 619 }, "TN");
      this.add(new int[1]{ 621 }, "SY");
      this.add(new int[1]{ 622 }, "EG");
      this.add(new int[1]{ 624 }, "LY");
      this.add(new int[1]{ 625 }, "JO");
      this.add(new int[1]{ 626 }, "IR");
      this.add(new int[1]{ 627 }, "KW");
      this.add(new int[1]{ 628 }, "SA");
      this.add(new int[1]{ 629 }, "AE");
      this.add(new int[2]{ 640, 649 }, "FI");
      this.add(new int[2]{ 690, 695 }, "CN");
      this.add(new int[2]{ 700, 709 }, "NO");
      this.add(new int[1]{ 729 }, "IL");
      this.add(new int[2]{ 730, 739 }, "SE");
      this.add(new int[1]{ 740 }, "GT");
      this.add(new int[1]{ 741 }, "SV");
      this.add(new int[1]{ 742 }, "HN");
      this.add(new int[1]{ 743 }, "NI");
      this.add(new int[1]{ 744 }, "CR");
      this.add(new int[1]{ 745 }, "PA");
      this.add(new int[1]{ 746 }, "DO");
      this.add(new int[1]{ 750 }, "MX");
      this.add(new int[2]{ 754, 755 }, "CA");
      this.add(new int[1]{ 759 }, "VE");
      this.add(new int[2]{ 760, 769 }, "CH");
      this.add(new int[1]{ 770 }, "CO");
      this.add(new int[1]{ 773 }, "UY");
      this.add(new int[1]{ 775 }, "PE");
      this.add(new int[1]{ 777 }, "BO");
      this.add(new int[1]{ 779 }, "AR");
      this.add(new int[1]{ 780 }, "CL");
      this.add(new int[1]{ 784 }, "PY");
      this.add(new int[1]{ 785 }, "PE");
      this.add(new int[1]{ 786 }, "EC");
      this.add(new int[2]{ 789, 790 }, "BR");
      this.add(new int[2]{ 800, 839 }, "IT");
      this.add(new int[2]{ 840, 849 }, "ES");
      this.add(new int[1]{ 850 }, "CU");
      this.add(new int[1]{ 858 }, "SK");
      this.add(new int[1]{ 859 }, "CZ");
      this.add(new int[1]{ 860 }, "YU");
      this.add(new int[1]{ 865 }, "MN");
      this.add(new int[1]{ 867 }, "KP");
      this.add(new int[2]{ 868, 869 }, "TR");
      this.add(new int[2]{ 870, 879 }, "NL");
      this.add(new int[1]{ 880 }, "KR");
      this.add(new int[1]{ 885 }, "TH");
      this.add(new int[1]{ 888 }, "SG");
      this.add(new int[1]{ 890 }, "IN");
      this.add(new int[1]{ 893 }, "VN");
      this.add(new int[1]{ 896 }, "PK");
      this.add(new int[1]{ 899 }, "ID");
      this.add(new int[2]{ 900, 919 }, "AT");
      this.add(new int[2]{ 930, 939 }, "AU");
      this.add(new int[2]{ 940, 949 }, "AZ");
      this.add(new int[1]{ 955 }, "MY");
      this.add(new int[1]{ 958 }, "MO");
    }
  }
}
