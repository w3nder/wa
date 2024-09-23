// Decompiled with JetBrains decompiler
// Type: WhatsApp.Mms4FibonacciBackoffCalculator
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;


namespace WhatsApp
{
  public class Mms4FibonacciBackoffCalculator
  {
    private object calcLock = new object();
    private Random random = new Random();
    private const int START_N = 0;
    private const int START_N_PLUS_1 = 1000;
    private static int MAX_BACKOFF = 987000;
    private long currN;
    private long currNPlus1 = 1000;

    public Mms4FibonacciBackoffCalculator()
    {
      try
      {
        string mms4FibBackoffState = Settings.Mms4FibBackoffState;
        if (string.IsNullOrEmpty(mms4FibBackoffState))
          return;
        string[] strArray = mms4FibBackoffState.Split(';');
        if (strArray.Length != 2)
          return;
        this.currN = (long) int.Parse(strArray[0]);
        this.currNPlus1 = (long) int.Parse(strArray[0]);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Failed to restore mms fib backoff settings");
      }
    }

    public long GetSleepTimeMs()
    {
      long num = this.NextBackOff();
      long sleepTimeMs = 3L * num / 4L + (long) (this.random.NextDouble() * (double) (num / 2L));
      Log.d("fib", "sleep {0} milliseconds", (object) sleepTimeMs);
      return sleepTimeMs;
    }

    public void Reset()
    {
      lock (this.calcLock)
      {
        Log.d("fib", "reset");
        this.currN = 0L;
        this.currNPlus1 = 1000L;
        this.UpdateSettings();
      }
    }

    private long NextBackOff()
    {
      lock (this.calcLock)
      {
        if (this.currN > (long) Mms4FibonacciBackoffCalculator.MAX_BACKOFF)
          return (long) Mms4FibonacciBackoffCalculator.MAX_BACKOFF;
        this.currNPlus1 += this.currN;
        this.currN = this.currNPlus1 - this.currN;
        this.UpdateSettings();
        return this.currNPlus1 - this.currN;
      }
    }

    private void UpdateSettings()
    {
      try
      {
        if (this.currN == 0L && this.currNPlus1 == 1000L)
          Settings.Mms4FibBackoffState = (string) null;
        else
          Settings.Mms4FibBackoffState = this.currN.ToString() + ";" + this.currNPlus1.ToString();
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Failed to save mms fib backoff settings");
      }
    }
  }
}
