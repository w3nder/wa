// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpUtilities
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace System.Net.Http
{
  internal static class HttpUtilities
  {
    internal static readonly Version DefaultVersion = HttpVersion.Version11;
    internal static readonly byte[] EmptyByteArray = new byte[0];

    internal static bool IsHttpUri(Uri uri)
    {
      Contract.Assert(uri != (Uri) null);
      string scheme = uri.Scheme;
      return string.Compare("http", scheme, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare("https", scheme, StringComparison.OrdinalIgnoreCase) == 0;
    }

    internal static bool HandleFaultsAndCancelation<T>(Task task, TaskCompletionSource<T> tcs)
    {
      Contract.Assert(task.IsCompleted);
      if (task.IsFaulted)
      {
        tcs.TrySetException(((Exception) task.Exception).GetBaseException());
        return true;
      }
      if (!task.IsCanceled)
        return false;
      tcs.TrySetCanceled();
      return true;
    }

    internal static Task ContinueWithStandard(this Task task, Action<Task> continuation)
    {
      return task.ContinueWith(continuation, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
    }

    internal static Task ContinueWithStandard<T>(this Task<T> task, Action<Task<T>> continuation)
    {
      return task.ContinueWith(continuation, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
    }
  }
}
