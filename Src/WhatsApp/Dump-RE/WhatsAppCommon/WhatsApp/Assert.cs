// Decompiled with JetBrains decompiler
// Type: WhatsApp.Assert
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;

#nullable disable
namespace WhatsApp
{
  public class Assert
  {
    private static bool Ensure(bool condition, string msg = null) => condition;

    public static bool IsTrue(bool condition, string msg = null) => Assert.Ensure(condition, msg);

    public static bool IsFalse(bool condition, string msg = null) => Assert.Ensure(!condition, msg);

    public static bool Failed(string msg = null) => Assert.Ensure(false, msg);

    public class AssertFailureException : Exception
    {
      public AssertFailureException()
      {
      }

      public AssertFailureException(string msg)
        : base(msg)
      {
      }
    }
  }
}
