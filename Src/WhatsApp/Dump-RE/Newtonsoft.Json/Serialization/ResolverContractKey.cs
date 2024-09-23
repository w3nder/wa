// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.ResolverContractKey
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;

#nullable disable
namespace Newtonsoft.Json.Serialization
{
  internal struct ResolverContractKey : IEquatable<ResolverContractKey>
  {
    private readonly Type _resolverType;
    private readonly Type _contractType;

    public ResolverContractKey(Type resolverType, Type contractType)
    {
      this._resolverType = resolverType;
      this._contractType = contractType;
    }

    public override int GetHashCode()
    {
      return this._resolverType.GetHashCode() ^ this._contractType.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      return obj is ResolverContractKey other && this.Equals(other);
    }

    public bool Equals(ResolverContractKey other)
    {
      return this._resolverType == other._resolverType && this._contractType == other._contractType;
    }
  }
}
