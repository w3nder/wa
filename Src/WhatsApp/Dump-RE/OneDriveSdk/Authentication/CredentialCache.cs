// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.Authentication.CredentialCache
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace Microsoft.OneDrive.Sdk.Authentication
{
  public class CredentialCache
  {
    internal readonly IDictionary<CredentialCacheKey, AccountSession> cacheDictionary = (IDictionary<CredentialCacheKey, AccountSession>) new ConcurrentDictionary<CredentialCacheKey, AccountSession>();
    private const int CacheVersion = 2;

    internal CredentialCacheKey MostRecentlyUsedKey { get; set; }

    public CredentialCache()
      : this((byte[]) null)
    {
    }

    public CredentialCache(ISerializer serializer)
      : this((byte[]) null, serializer)
    {
    }

    public CredentialCache(byte[] blob, ISerializer serializer = null)
    {
      this.Serializer = serializer ?? (ISerializer) new Microsoft.Graph.Serializer();
      this.InitializeCacheFromBlob(blob);
      this.MostRecentlyUsedKey = (CredentialCacheKey) null;
    }

    public virtual CredentialCacheNotification BeforeAccess { get; set; }

    public virtual CredentialCacheNotification BeforeWrite { get; set; }

    public virtual CredentialCacheNotification AfterAccess { get; set; }

    public virtual bool HasStateChanged { get; set; }

    protected ISerializer Serializer { get; private set; }

    public virtual byte[] GetCacheBlob()
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (BinaryReader binaryReader = new BinaryReader((Stream) memoryStream))
        {
          using (BinaryWriter binaryWriter = new BinaryWriter((Stream) memoryStream))
          {
            binaryWriter.Write(2);
            binaryWriter.Write(this.cacheDictionary.Count);
            foreach (KeyValuePair<CredentialCacheKey, AccountSession> cache in (IEnumerable<KeyValuePair<CredentialCacheKey, AccountSession>>) this.cacheDictionary)
            {
              binaryWriter.Write(this.Serializer.SerializeObject((object) cache.Key));
              binaryWriter.Write(this.Serializer.SerializeObject((object) cache.Value));
            }
            bool flag = this.MostRecentlyUsedKey != null;
            binaryWriter.Write(flag);
            if (flag)
              binaryWriter.Write(this.Serializer.SerializeObject((object) this.MostRecentlyUsedKey));
            int position = (int) memoryStream.Position;
            memoryStream.Position = 0L;
            return binaryReader.ReadBytes(position);
          }
        }
      }
    }

    public virtual void InitializeCacheFromBlob(byte[] cacheBytes)
    {
      if (cacheBytes == null)
      {
        this.cacheDictionary.Clear();
      }
      else
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          using (BinaryReader binaryReader = new BinaryReader((Stream) memoryStream))
          {
            using (BinaryWriter binaryWriter = new BinaryWriter((Stream) memoryStream))
            {
              binaryWriter.Write(cacheBytes);
              memoryStream.Position = 0L;
              this.cacheDictionary.Clear();
              if (binaryReader.ReadInt32() != 2)
                return;
              int num = binaryReader.ReadInt32();
              for (int index = 0; index < num; ++index)
              {
                string inputString1 = binaryReader.ReadString();
                string inputString2 = binaryReader.ReadString();
                if (!string.IsNullOrEmpty(inputString1) && !string.IsNullOrEmpty(inputString2))
                  this.cacheDictionary.Add(this.Serializer.DeserializeObject<CredentialCacheKey>(inputString1), this.Serializer.DeserializeObject<AccountSession>(inputString2));
              }
              this.MostRecentlyUsedKey = binaryReader.ReadBoolean() ? this.Serializer.DeserializeObject<CredentialCacheKey>(binaryReader.ReadString()) : (CredentialCacheKey) null;
            }
          }
        }
      }
    }

    public virtual void Clear()
    {
      CredentialCacheNotificationArgs args = new CredentialCacheNotificationArgs()
      {
        CredentialCache = this
      };
      this.OnBeforeAccess(args);
      this.OnBeforeWrite(args);
      this.cacheDictionary.Clear();
      this.HasStateChanged = true;
      this.OnAfterAccess(args);
    }

    public virtual void AddToCache(AccountSession accountSession)
    {
      CredentialCacheNotificationArgs args = new CredentialCacheNotificationArgs()
      {
        CredentialCache = this
      };
      this.OnBeforeAccess(args);
      this.OnBeforeWrite(args);
      CredentialCacheKey keyForAuthResult = this.GetKeyForAuthResult(accountSession);
      this.cacheDictionary[keyForAuthResult] = accountSession;
      this.MostRecentlyUsedKey = keyForAuthResult;
      this.HasStateChanged = true;
      this.OnAfterAccess(args);
    }

    public virtual void DeleteFromCache(AccountSession accountSession)
    {
      if (accountSession == null)
        return;
      CredentialCacheNotificationArgs args = new CredentialCacheNotificationArgs()
      {
        CredentialCache = this
      };
      this.OnBeforeAccess(args);
      this.OnBeforeWrite(args);
      CredentialCacheKey keyForAuthResult = this.GetKeyForAuthResult(accountSession);
      this.cacheDictionary.Remove(keyForAuthResult);
      if (keyForAuthResult.Equals((object) this.MostRecentlyUsedKey))
        this.MostRecentlyUsedKey = (CredentialCacheKey) null;
      this.HasStateChanged = true;
      this.OnAfterAccess(args);
    }

    public CredentialCacheKey GetKeyForAuthResult(AccountSession accountSession)
    {
      return new CredentialCacheKey()
      {
        ClientId = accountSession.ClientId,
        UserId = accountSession.UserId
      };
    }

    public virtual AccountSession GetResultFromCache(string clientId, string userId)
    {
      CredentialCacheKey credentialCacheKey = new CredentialCacheKey()
      {
        ClientId = clientId,
        UserId = userId
      };
      CredentialCacheNotificationArgs args = new CredentialCacheNotificationArgs()
      {
        CredentialCache = this
      };
      this.OnBeforeAccess(args);
      AccountSession resultFromCache = this.GetResultFromCache(credentialCacheKey);
      this.OnAfterAccess(args);
      return resultFromCache;
    }

    public virtual AccountSession GetMostRecentlyUsedResultFromCache()
    {
      CredentialCacheNotificationArgs args = new CredentialCacheNotificationArgs()
      {
        CredentialCache = this
      };
      this.OnBeforeAccess(args);
      if (this.MostRecentlyUsedKey == null)
        return (AccountSession) null;
      AccountSession resultFromCache = this.GetResultFromCache(this.MostRecentlyUsedKey);
      this.OnAfterAccess(args);
      return resultFromCache;
    }

    private AccountSession GetResultFromCache(CredentialCacheKey credentialCacheKey)
    {
      AccountSession resultFromCache = (AccountSession) null;
      if (this.cacheDictionary.TryGetValue(credentialCacheKey, out resultFromCache))
        this.MostRecentlyUsedKey = credentialCacheKey;
      return resultFromCache;
    }

    protected void OnAfterAccess(CredentialCacheNotificationArgs args)
    {
      if (this.AfterAccess == null)
        return;
      this.AfterAccess(args);
    }

    protected void OnBeforeAccess(CredentialCacheNotificationArgs args)
    {
      if (this.BeforeAccess == null)
        return;
      this.BeforeAccess(args);
    }

    protected void OnBeforeWrite(CredentialCacheNotificationArgs args)
    {
      if (this.BeforeWrite == null)
        return;
      this.BeforeWrite(args);
    }
  }
}
