// Decompiled with JetBrains decompiler
// Type: WhatsApp.BizProfileDetails
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Collections.Generic;

#nullable disable
namespace WhatsApp
{
  public class BizProfileDetails
  {
    public string Tag;
    public string Address;
    public List<string> Websites;
    public string Description;
    public string VerticalDescription;
    public string VerticalCanonical;
    public string Email;
    public double? Latitude;
    public double? Longitude;
    public BusinessHoursDetails Hours;

    public static BizProfileDetails ExtractProfileDetails(FunXMPP.ProtocolTreeNode profNode)
    {
      BizProfileDetails profileDetails = new BizProfileDetails();
      profileDetails.Tag = profNode.GetAttributeValue("tag");
      profileDetails.Address = profNode.GetChild("address")?.GetDataString();
      profileDetails.Websites = new List<string>();
      foreach (FunXMPP.ProtocolTreeNode allChild in profNode.GetAllChildren("website"))
        profileDetails.Websites.Add(allChild.GetDataString());
      profileDetails.Description = profNode.GetChild("description")?.GetDataString();
      FunXMPP.ProtocolTreeNode child1 = profNode.GetChild("vertical");
      if (child1 != null)
      {
        profileDetails.VerticalDescription = child1.GetDataString();
        profileDetails.VerticalCanonical = child1.GetAttributeValue("canonical");
      }
      profileDetails.Email = profNode.GetChild("email")?.GetDataString();
      string dataString1 = profNode.GetChild("latitude")?.GetDataString();
      if (dataString1 != null)
      {
        double result = 0.0;
        if (double.TryParse(dataString1, out result))
          profileDetails.Latitude = new double?(result);
      }
      string dataString2 = profNode.GetChild("longitude")?.GetDataString();
      if (dataString2 != null)
      {
        double result = 0.0;
        if (double.TryParse(dataString2, out result))
          profileDetails.Longitude = new double?(result);
      }
      FunXMPP.ProtocolTreeNode child2 = profNode.GetChild("business_hours");
      if (child2 != null)
        profileDetails.Hours = BusinessHoursDetails.ParseBusinessHours(child2);
      return profileDetails;
    }
  }
}
