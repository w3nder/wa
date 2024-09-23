// Decompiled with JetBrains decompiler
// Type: WhatsApp.TokenDictionary
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Collections.Generic;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public class TokenDictionary
  {
    private string[] primaryStrings;
    private string[][] secondaryStrings;
    private int secondaryStringsStart = 236;
    private int dictionaryVersion;
    public int PrimaryTokenMax = 245;
    private Dictionary<string, int> primaryStringDict = new Dictionary<string, int>();
    private Dictionary<string, Pair<int, int>> secondaryStringDict = new Dictionary<string, Pair<int, int>>();

    public TokenDictionary(ITokenDictionary dict = null)
    {
      if (dict == null)
        dict = (ITokenDictionary) new WAPDefaultTokenDictionary();
      this.primaryStrings = dict.PrimaryStrings;
      this.secondaryStrings = dict.SecondaryStrings;
      this.secondaryStringsStart = dict.SecondaryStringsStart;
      this.dictionaryVersion = dict.DictionaryVersion;
      for (int index = 0; index < this.primaryStrings.Length; ++index)
      {
        string primaryString = this.primaryStrings[index];
        if (primaryString != null)
          this.primaryStringDict.Add(primaryString, index);
      }
      for (int index1 = 0; index1 < this.secondaryStrings.Length; ++index1)
      {
        string[] secondaryString = this.secondaryStrings[index1];
        for (int index2 = 0; index2 < secondaryString.Length; ++index2)
        {
          string key = secondaryString[index2];
          if (key != null)
            this.secondaryStringDict.Add(key, new Pair<int, int>()
            {
              First = index1 + this.secondaryStringsStart,
              Second = index2
            });
        }
      }
      this.PrimaryTokenMax = this.primaryStrings.Length + this.secondaryStrings.Length;
    }

    public bool TryGetToken(string str, ref int subdict, ref int token)
    {
      if (this.primaryStringDict.TryGetValue(str, out token))
        return true;
      Pair<int, int> pair;
      if (!this.secondaryStringDict.TryGetValue(str, out pair))
        return false;
      subdict = pair.First;
      token = pair.Second;
      return true;
    }

    public void GetToken(int token, ref int subdict, ref string str)
    {
      string[] strArray = (string[]) null;
      if (subdict >= 0)
      {
        if (subdict >= this.secondaryStrings.Length)
          throw new FunXMPP.CorruptStreamException("Invalid subdictionary " + (object) subdict);
        strArray = this.secondaryStrings[subdict];
      }
      else if (this.secondaryStringsStart >= 0 && token >= this.secondaryStringsStart && token < this.secondaryStringsStart + this.secondaryStrings.Length)
        subdict = token - this.secondaryStringsStart;
      else
        strArray = this.primaryStrings;
      if (strArray == null)
        return;
      if (token < 0 || token > strArray.Length)
        throw new FunXMPP.CorruptStreamException("Invalid token " + (object) token);
      str = strArray[token];
      if (str == null)
        throw new FunXMPP.CorruptStreamException("invalid token/length in getToken");
    }

    public int GetDictionaryVersion() => this.dictionaryVersion;
  }
}
