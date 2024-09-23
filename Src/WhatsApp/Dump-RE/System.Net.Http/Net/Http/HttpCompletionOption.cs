// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpCompletionOption
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

#nullable disable
namespace System.Net.Http
{
  /// <summary>Indicates if <see cref="T:System.Net.Http.HttpClient" /> operations should be considered completed either as soon as a response is available, or after reading the entire response message including the content. </summary>
  public enum HttpCompletionOption
  {
    /// <summary>The operation should complete after reading the entire response including the content.</summary>
    ResponseContentRead,
    /// <summary>The operation should complete as soon as a response is available and headers are read. The content is not read yet. </summary>
    ResponseHeadersRead,
  }
}
