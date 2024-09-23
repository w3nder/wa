// Decompiled with JetBrains decompiler
// Type: WhatsApp.CreateOptionsImpl
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp
{
  internal enum CreateOptionsImpl
  {
    None = 0,
    CreateIfNotFound = 1,
    InsertAfterCreated = 2,
    SubmitAfterInserted = 4,
    BypassCache = 8,
  }
}
