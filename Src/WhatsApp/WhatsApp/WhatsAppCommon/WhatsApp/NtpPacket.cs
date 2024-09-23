// Decompiled with JetBrains decompiler
// Type: WhatsApp.NtpPacket
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp
{
  public struct NtpPacket
  {
    public byte Flags;
    public byte Stratum;
    public byte Poll;
    public byte Precision;
    public uint RootDelay;
    public uint RootDispersion;
    public uint ReferenceIdentifier;
    public NtpTimestamp ReferenceTimestamp;
    public NtpTimestamp OriginTimestamp;
    public NtpTimestamp ReceiveTimestamp;
    public NtpTimestamp TransmitTimestamp;
    public static int ModeClient = 3;
    public static int ModeServer = 4;

    public static byte MakeFlags(int li, int vn, int mode) => (byte) (mode | vn << 3 | li << 6);

    private static void Swap(ref uint l)
    {
      l = (uint) ((int) l << 24 | ((int) l & 65280) << 8) | (l & 16711680U) >> 8 | (l & 4278190080U) >> 24;
    }

    private static void Swap(ref NtpTimestamp t)
    {
      NtpPacket.Swap(ref t.Seconds);
      NtpPacket.Swap(ref t.Fraction);
    }

    public static void Swap(ref NtpPacket p)
    {
      NtpPacket.Swap(ref p.RootDelay);
      NtpPacket.Swap(ref p.RootDispersion);
      NtpPacket.Swap(ref p.ReferenceIdentifier);
      NtpPacket.Swap(ref p.ReferenceTimestamp);
      NtpPacket.Swap(ref p.OriginTimestamp);
      NtpPacket.Swap(ref p.ReceiveTimestamp);
      NtpPacket.Swap(ref p.TransmitTimestamp);
    }
  }
}
