// Decompiled with JetBrains decompiler
// Type: WhatsApp.UserStatusSearchResult
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using WhatsApp.WaCollections;


namespace WhatsApp
{
  public struct UserStatusSearchResult
  {
    public UserStatus UserStatus;
    public bool JidMatched;
    public Pair<int, int>[] ContactNameOffsets;
    public Pair<int, int>[] StatusOffsets;
  }
}
