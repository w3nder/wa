// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.Helpers.ControlHelper
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

#nullable disable
namespace Coding4Fun.Phone.Controls.Helpers
{
  public class ControlHelper
  {
    public static double CheckBound(double value, double max)
    {
      return ControlHelper.CheckBound(value, 0.0, max);
    }

    public static double CheckBound(double value, double min, double max)
    {
      if (value <= min)
        value = min;
      else if (value >= max)
        value = max;
      return value;
    }
  }
}
