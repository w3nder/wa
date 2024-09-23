// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ServiceException
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System;

#nullable disable
namespace Microsoft.Graph
{
  public class ServiceException : Exception
  {
    public ServiceException(Error error, Exception innerException = null)
      : base((string) null, innerException)
    {
      this.Error = error;
    }

    public Error Error { get; private set; }

    public bool IsMatch(string errorCode)
    {
      if (string.IsNullOrEmpty(errorCode))
        throw new ArgumentException("errorCode cannot be null or empty", nameof (errorCode));
      for (Error error = this.Error; error != null; error = error.InnerError)
      {
        if (string.Equals(error.Code, errorCode, StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }

    public override string ToString() => this.Error != null ? this.Error.ToString() : (string) null;
  }
}
