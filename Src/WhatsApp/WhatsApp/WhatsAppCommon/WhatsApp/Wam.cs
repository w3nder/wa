// Decompiled with JetBrains decompiler
// Type: WhatsApp.Wam
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using WhatsApp.Events;
using WhatsAppNative;


namespace WhatsApp
{
  public static class Wam
  {
    private static Wam.AttributeWrapper attrib = new Wam.AttributeWrapper();
    private static Wam.SerializeWrapper serializer = new Wam.SerializeWrapper();

    public static void LogEvent(WamEvent ev, uint weight)
    {
      FieldStatsRunner.FieldStatsAction((Action<IFieldStats>) (instance => instance.SaveEvent(ev.GetCode(), weight, new Action(ev.SerializeFields).AsComAction())));
    }

    public static int UniformRandom(uint n) => NativeInterfaces.Misc.UniformRandom((int) n);

    public static long? EnumToLong<T>(T? value) where T : struct
    {
      if (!typeof (T).IsEnum)
        throw new InvalidOperationException("This method is meant for enums.");
      return value.HasValue ? new long?((long) (int) (ValueType) value.Value) : new long?();
    }

    public static void SetAttribute(int code, bool? arg) => Wam.attrib.Value(code, arg);

    public static void SetAttribute(int code, long? arg) => Wam.attrib.Value(code, arg);

    public static void SetAttribute(int code, double? arg) => Wam.attrib.Value(code, arg);

    public static void SetAttribute(int code, string arg) => Wam.attrib.Value(code, arg);

    public static void MaybeSerializeField(int code, bool? arg) => Wam.serializer.Value(code, arg);

    public static void MaybeSerializeField(int code, long? arg) => Wam.serializer.Value(code, arg);

    public static void MaybeSerializeField(int code, double? arg)
    {
      Wam.serializer.Value(code, arg);
    }

    public static void MaybeSerializeField(int code, string arg) => Wam.serializer.Value(code, arg);

    public static void SetAbKey(string value) => Wam.SetAttribute(15, value);

    public static void SetAppBuild(wam_enum_app_build_type? value)
    {
      Wam.SetAttribute(11, Wam.EnumToLong<wam_enum_app_build_type>(value));
    }

    public static void SetAppDistribution(wam_enum_app_distribution_type? value)
    {
      Wam.SetAttribute(12, Wam.EnumToLong<wam_enum_app_distribution_type>(value));
    }

    public static void SetAppIsBetaRelease(bool? value) => Wam.SetAttribute(6, value);

    public static void SetAppVersion(string value) => Wam.SetAttribute(5, value);

    public static void SetDatacenter(string value) => Wam.SetAttribute(16, value);

    public static void SetDeviceName(string value) => Wam.SetAttribute(3, value);

    public static void SetMcc(long? value) => Wam.SetAttribute(1, value);

    public static void SetMnc(long? value) => Wam.SetAttribute(0, value);

    public static void SetNetworkIsWifi(bool? value) => Wam.SetAttribute(7, value);

    public static void SetNetworkRadioType(wam_enum_radio_type? value)
    {
      Wam.SetAttribute(10, Wam.EnumToLong<wam_enum_radio_type>(value));
    }

    public static void SetNetworkRadioTypeS(string value) => Wam.SetAttribute(8, value);

    public static void SetOsVersion(string value) => Wam.SetAttribute(4, value);

    public static void SetPlatform(wam_enum_platform_type? value)
    {
      Wam.SetAttribute(2, Wam.EnumToLong<wam_enum_platform_type>(value));
    }

    public static void SetUserActivityLoggingMethod(wam_enum_user_activity_logging_method? value)
    {
      Wam.SetAttribute(13, Wam.EnumToLong<wam_enum_user_activity_logging_method>(value));
    }

    public static void SetWpProcess(wam_enum_wp_process? value)
    {
      Wam.SetAttribute(14, Wam.EnumToLong<wam_enum_wp_process>(value));
    }

    private abstract class FieldWrapper
    {
      protected abstract void Null(int code);

      protected abstract void ValueImpl(int code, double d);

      protected abstract void ValueImpl(int code, string str);

      public void Value(int code, bool? value)
      {
        if (value.HasValue)
          this.ValueImpl(code, value.Value ? 1.0 : 0.0);
        else
          this.Null(code);
      }

      public void Value(int code, long? value)
      {
        if (value.HasValue)
          this.ValueImpl(code, (double) value.Value);
        else
          this.Null(code);
      }

      public void Value(int code, double? value)
      {
        if (value.HasValue)
          this.ValueImpl(code, value.Value);
        else
          this.Null(code);
      }

      public void Value(int code, string value)
      {
        if (value != null)
          this.ValueImpl(code, value);
        else
          this.Null(code);
      }
    }

    private class AttributeWrapper : Wam.FieldWrapper
    {
      protected override void Null(int code) => FieldStatsRunner.Instance.SetAttributeNull(code);

      protected override void ValueImpl(int code, string str)
      {
        FieldStatsRunner.Instance.SetAttributeString(code, str);
      }

      protected override void ValueImpl(int code, double d)
      {
        FieldStatsRunner.Instance.SetAttributeDouble(code, d);
      }
    }

    private class SerializeWrapper : Wam.FieldWrapper
    {
      protected override void Null(int code)
      {
      }

      protected override void ValueImpl(int code, string str)
      {
        FieldStatsRunner.Instance.MaybeSerializeString(code, str);
      }

      protected override void ValueImpl(int code, double d)
      {
        FieldStatsRunner.Instance.MaybeSerializeDouble(code, d);
      }
    }
  }
}
