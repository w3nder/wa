// Decompiled with JetBrains decompiler
// Type: WhatsApp.BusinessHoursDetails
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class BusinessHoursDetails
  {
    private const byte BD_DETAILS_FORMAT_V1 = 1;

    public string TimeZone { get; private set; }

    public string Note { get; private set; }

    public List<BusinessHoursDetails.BusinessHoursPeriodDetails> OpenPeriods { get; private set; }

    public BusinessHoursDetails(
      string timeZone,
      string note,
      List<BusinessHoursDetails.BusinessHoursPeriodDetails> openPeriods)
    {
      this.TimeZone = timeZone;
      this.Note = note;
      this.OpenPeriods = openPeriods;
    }

    public static BusinessHoursDetails ParseBusinessHours(FunXMPP.ProtocolTreeNode node)
    {
      if (node == null)
        return (BusinessHoursDetails) null;
      BusinessHoursDetails businessHours = (BusinessHoursDetails) null;
      try
      {
        List<BusinessHoursDetails.BusinessHoursPeriodDetails> openPeriods = new List<BusinessHoursDetails.BusinessHoursPeriodDetails>();
        string note = (string) null;
        string attributeValue1 = node.GetAttributeValue("timezone");
        FunXMPP.ProtocolTreeNode child = node.GetChild("business_hours_note");
        if (child != null && child.GetDataString() != null)
          note = child.GetDataString();
        foreach (FunXMPP.ProtocolTreeNode allChild in node.GetAllChildren("business_hours_config"))
        {
          string attributeValue2 = allChild.GetAttributeValue("day_of_week");
          string attributeValue3 = allChild.GetAttributeValue("mode");
          int? openTime = new int?();
          string attributeValue4 = allChild.GetAttributeValue("open_time");
          if (attributeValue4 != null)
            openTime = new int?(int.Parse(attributeValue4));
          int? closeTime = new int?();
          string attributeValue5 = allChild.GetAttributeValue("close_time");
          if (attributeValue5 != null)
            closeTime = new int?(int.Parse(attributeValue5));
          openPeriods.Add(new BusinessHoursDetails.BusinessHoursPeriodDetails(BusinessHoursDetails.BusinessHoursPeriodDetails.ParseWeekDay(attributeValue2), BusinessHoursDetails.BusinessHoursPeriodDetails.ParseOpenMode(attributeValue3), openTime, closeTime));
        }
        businessHours = new BusinessHoursDetails(attributeValue1, note, openPeriods);
      }
      catch (Exception ex)
      {
        string context = BizProfileHelper.LogHdr + " Exception parsing Business hours";
        Log.LogException(ex, context);
      }
      return businessHours;
    }

    public byte[] Serialize()
    {
      byte[] numArray = (byte[]) null;
      try
      {
        BinaryData bd = new BinaryData();
        bd.AppendByte((byte) 1);
        bd.AppendStrWithLengthPrefix(this.TimeZone);
        bd.AppendStrWithLengthPrefix(this.Note ?? "");
        if (this.OpenPeriods != null && this.OpenPeriods.Any<BusinessHoursDetails.BusinessHoursPeriodDetails>())
        {
          bd.AppendInt32(this.OpenPeriods.Count);
          foreach (BusinessHoursDetails.BusinessHoursPeriodDetails openPeriod in this.OpenPeriods)
            openPeriod.SerialiseInto(bd);
        }
        else
          bd.AppendInt32(0);
        numArray = bd.Get();
      }
      catch (Exception ex)
      {
        string context = BizProfileHelper.LogHdr + " Exception serializing Business hours details";
        Log.LogException(ex, context);
      }
      return numArray;
    }

    public static BusinessHoursDetails Deserialize(byte[] binaryData)
    {
      BusinessHoursDetails businessHoursDetails = (BusinessHoursDetails) null;
      try
      {
        BinaryData bd = new BinaryData(binaryData);
        int offset1 = 0;
        byte num = bd.ReadByte(offset1);
        int newOffset = offset1 + 1;
        if (num != (byte) 1)
        {
          Log.l(BizProfileHelper.LogHdr, "Deserialize expected {0} found {1} at offset {2}", (object) (byte) 1, (object) num, (object) newOffset);
          throw new InvalidOperationException("Failure converting binary data to business hours");
        }
        string timeZone = bd.ReadStrWithLengthPrefix(newOffset, out newOffset);
        string note = bd.ReadStrWithLengthPrefix(newOffset, out newOffset);
        int capacity = bd.ReadInt32(newOffset);
        int offset2 = newOffset + 4;
        List<BusinessHoursDetails.BusinessHoursPeriodDetails> openPeriods = (List<BusinessHoursDetails.BusinessHoursPeriodDetails>) null;
        if (capacity > 0)
        {
          openPeriods = new List<BusinessHoursDetails.BusinessHoursPeriodDetails>(capacity);
          for (int index = 0; index < capacity; ++index)
            openPeriods.Add(BusinessHoursDetails.BusinessHoursPeriodDetails.DeserializeOutOf(bd, ref offset2));
        }
        businessHoursDetails = new BusinessHoursDetails(timeZone, note, openPeriods);
      }
      catch (Exception ex)
      {
        string context = BizProfileHelper.LogHdr + " Exception deserializing Business hours details";
        Log.LogException(ex, context);
      }
      return businessHoursDetails;
    }

    public class BusinessHoursPeriodDetails
    {
      private const string OPEN_MODE_24_HOURS = "open_24h";
      private const string OPEN_MODE_APPOINTMENT_ONLY = "appointment_only";
      private const string OPEN_MODE_SPECIFIC_HOURS = "specific_hours";
      private const string DAY_OF_WEEK_MONDAY = "mon";
      private const string DAY_OF_WEEK_TUESDAY = "tue";
      private const string DAY_OF_WEEK_WEDNESDAY = "wed";
      private const string DAY_OF_WEEK_THURSDAY = "thu";
      private const string DAY_OF_WEEK_FRIDAY = "fri";
      private const string DAY_OF_WEEK_SATURDAY = "sat";
      private const string DAY_OF_WEEK_SUNDAY = "sun";
      private int? openTimeInMins;
      private int? closeTimeInMins;
      private const byte BD_PERIOD_V1 = 1;

      public DayOfWeek Day { get; private set; }

      public BusinessHoursDetails.BusinessHoursPeriodDetails.OpenMode Mode { get; private set; }

      public TimeSpan? OpenTime
      {
        get
        {
          return !this.openTimeInMins.HasValue ? new TimeSpan?() : new TimeSpan?(TimeSpan.FromMinutes((double) this.openTimeInMins.Value));
        }
      }

      public TimeSpan? CloseTime
      {
        get
        {
          return !this.closeTimeInMins.HasValue ? new TimeSpan?() : new TimeSpan?(TimeSpan.FromMinutes((double) this.closeTimeInMins.Value));
        }
      }

      public BusinessHoursPeriodDetails(
        DayOfWeek dayOfWeek,
        BusinessHoursDetails.BusinessHoursPeriodDetails.OpenMode mode,
        int? openTime,
        int? closeTime)
      {
        this.Day = dayOfWeek;
        this.Mode = mode;
        this.openTimeInMins = openTime;
        this.closeTimeInMins = closeTime;
      }

      public static BusinessHoursDetails.BusinessHoursPeriodDetails.OpenMode ParseOpenMode(
        string openMode)
      {
        BusinessHoursDetails.BusinessHoursPeriodDetails.OpenMode openMode1 = BusinessHoursDetails.BusinessHoursPeriodDetails.OpenMode.None;
        switch (openMode)
        {
          case "specific_hours":
            openMode1 = BusinessHoursDetails.BusinessHoursPeriodDetails.OpenMode.SpecificHours;
            break;
          case "open_24h":
            openMode1 = BusinessHoursDetails.BusinessHoursPeriodDetails.OpenMode.Open24;
            break;
          case "appointment_only":
            openMode1 = BusinessHoursDetails.BusinessHoursPeriodDetails.OpenMode.AppointmentOnly;
            break;
        }
        return openMode1 != BusinessHoursDetails.BusinessHoursPeriodDetails.OpenMode.None ? openMode1 : throw new InvalidOperationException("Unrecognized mode: " + openMode);
      }

      public static DayOfWeek ParseWeekDay(string day)
      {
        switch (day)
        {
          case "fri":
            return DayOfWeek.Friday;
          case "mon":
            return DayOfWeek.Monday;
          case "sat":
            return DayOfWeek.Saturday;
          case "sun":
            return DayOfWeek.Sunday;
          case "thu":
            return DayOfWeek.Thursday;
          case "tue":
            return DayOfWeek.Tuesday;
          case "wed":
            return DayOfWeek.Wednesday;
          default:
            throw new ArgumentException("Unrecognized week day: " + day);
        }
      }

      public void SerialiseInto(BinaryData bd)
      {
        bd.AppendByte((byte) 1);
        bd.AppendByte((byte) this.Day);
        bd.AppendByte((byte) this.Mode);
        BinaryData binaryData1 = bd;
        int? nullable = this.openTimeInMins;
        int val1 = nullable ?? -1;
        binaryData1.AppendInt32(val1);
        BinaryData binaryData2 = bd;
        nullable = this.closeTimeInMins;
        int val2 = nullable ?? -1;
        binaryData2.AppendInt32(val2);
      }

      public static BusinessHoursDetails.BusinessHoursPeriodDetails DeserializeOutOf(
        BinaryData bd,
        ref int offset)
      {
        byte num1 = bd.ReadByte(offset);
        ++offset;
        if (num1 != (byte) 1)
        {
          Log.l(BizProfileHelper.LogHdr, "Deserialize expected {0} found {1} at offset {2}", (object) (byte) 1, (object) num1, (object) offset);
          throw new InvalidOperationException("Failure converting binary data for hours");
        }
        int num2 = (int) bd.ReadByte(offset);
        ++offset;
        if (num2 >= 7)
          num2 = 0;
        byte mode = bd.ReadByte(offset);
        ++offset;
        int? openTime = new int?();
        int num3 = bd.ReadInt32(offset);
        offset += 4;
        if (num3 > 0)
          openTime = new int?(num3);
        int? closeTime = new int?();
        int num4 = bd.ReadInt32(offset);
        offset += 4;
        if (num4 > 0)
          closeTime = new int?(num4);
        return new BusinessHoursDetails.BusinessHoursPeriodDetails((DayOfWeek) num2, (BusinessHoursDetails.BusinessHoursPeriodDetails.OpenMode) mode, openTime, closeTime);
      }

      public enum OpenMode
      {
        None,
        Open24,
        SpecificHours,
        AppointmentOnly,
      }
    }
  }
}
