// Decompiled with JetBrains decompiler
// Type: System.PlatformHelper
// Assembly: Portable.ConcurrentDictionary, Version=1.0.2.0, Culture=neutral, PublicKeyToken=null
// MVID: DB56BACC-BDC4-4C60-BF1D-8E1E2F27714A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Portable.ConcurrentDictionary.dll

#nullable disable
namespace System
{
  internal static class PlatformHelper
  {
    public static int ProcessorCount => Environment.ProcessorCount;
  }
}
