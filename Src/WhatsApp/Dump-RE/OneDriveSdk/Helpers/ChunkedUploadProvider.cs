// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.Helpers.ChunkedUploadProvider
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk.Helpers
{
  public class ChunkedUploadProvider
  {
    private const int DefaultMaxChunkSize = 5242880;
    private const int RequiredChunkSizeIncrement = 327680;
    private IBaseClient client;
    private Stream uploadStream;
    private readonly int maxChunkSize;
    private List<Tuple<long, long>> rangesRemaining;

    public UploadSession Session { get; private set; }

    private long totalUploadLength => this.uploadStream.Length;

    public ChunkedUploadProvider(
      UploadSession session,
      IBaseClient client,
      Stream uploadStream,
      int maxChunkSize = -1)
    {
      if (!uploadStream.CanRead || !uploadStream.CanSeek)
        throw new ArgumentException("Must provide stream that can read and seek");
      this.Session = session;
      this.client = client;
      this.uploadStream = uploadStream;
      this.rangesRemaining = this.GetRangesRemaining(session);
      this.maxChunkSize = maxChunkSize < 0 ? 5242880 : maxChunkSize;
      if (this.maxChunkSize % 327680 != 0)
        throw new ArgumentException("Max chunk size must be a multiple of 320 KiB", nameof (maxChunkSize));
    }

    public virtual IEnumerable<UploadChunkRequest> GetUploadChunkRequests(
      IEnumerable<Option> options = null)
    {
      int nextChunkSize;
      foreach (Tuple<long, long> range in this.rangesRemaining)
      {
        for (long currentRangeBegins = range.Item1; currentRangeBegins <= range.Item2; currentRangeBegins += (long) nextChunkSize)
        {
          nextChunkSize = this.NextChunkSize(currentRangeBegins, range.Item2);
          yield return new UploadChunkRequest(this.Session.UploadUrl, this.client, options, currentRangeBegins, currentRangeBegins + (long) nextChunkSize - 1L, this.totalUploadLength);
        }
      }
    }

    public virtual async Task<UploadSession> UpdateSessionStatusAsync()
    {
      UploadSession async = await new UploadSessionRequest(this.Session, this.client, (IEnumerable<Option>) null).GetAsync();
      this.rangesRemaining = this.GetRangesRemaining(async);
      async.UploadUrl = this.Session.UploadUrl;
      this.Session = async;
      return async;
    }

    public async Task DeleteSession()
    {
      await new UploadSessionRequest(this.Session, this.client, (IEnumerable<Option>) null).DeleteAsync();
    }

    public async Task<Item> UploadAsync(int maxTries = 3, IEnumerable<Option> options = null)
    {
      int uploadTries = 0;
      byte[] readBuffer = new byte[this.maxChunkSize];
      List<Exception> trackedExceptions = new List<Exception>();
      while (uploadTries < maxTries)
      {
        IEnumerator<UploadChunkRequest> enumerator = this.GetUploadChunkRequests(options).GetEnumerator();
        Item itemResponse;
        try
        {
          UploadChunkResult requestResponseAsync;
          do
          {
            if (enumerator.MoveNext())
              requestResponseAsync = await this.GetChunkRequestResponseAsync(enumerator.Current, readBuffer, (ICollection<Exception>) trackedExceptions);
            else
              goto label_9;
          }
          while (!requestResponseAsync.UploadSucceeded);
          itemResponse = requestResponseAsync.ItemResponse;
          goto label_14;
        }
        finally
        {
          enumerator?.Dispose();
        }
label_9:
        enumerator = (IEnumerator<UploadChunkRequest>) null;
        UploadSession uploadSession = await this.UpdateSessionStatusAsync();
        ++uploadTries;
        if (uploadTries < maxTries)
        {
          await Task.Delay(2000 * uploadTries * uploadTries).ConfigureAwait(false);
          continue;
        }
        continue;
label_14:
        return itemResponse;
      }
      throw new TaskCanceledException("Upload failed too many times. See InnerException for list of exceptions that occured.", (Exception) new AggregateException(trackedExceptions.ToArray()));
    }

    public virtual async Task<UploadChunkResult> GetChunkRequestResponseAsync(
      UploadChunkRequest request,
      byte[] readBuffer,
      ICollection<Exception> exceptionTrackingList)
    {
      bool firstAttempt = true;
      this.uploadStream.Seek(request.RangeBegin, SeekOrigin.Begin);
      int num = await this.uploadStream.ReadAsync(readBuffer, 0, request.RangeLength).ConfigureAwait(false);
      while (true)
      {
        using (MemoryStream requestBodyStream = new MemoryStream(request.RangeLength))
        {
          await requestBodyStream.WriteAsync(readBuffer, 0, request.RangeLength).ConfigureAwait(false);
          requestBodyStream.Seek(0L, SeekOrigin.Begin);
          try
          {
            return await request.PutAsync((Stream) requestBodyStream).ConfigureAwait(false);
          }
          catch (ServiceException ex)
          {
            if (ex.IsMatch("generalException") || ex.IsMatch("timeout"))
            {
              if (firstAttempt)
              {
                firstAttempt = false;
                exceptionTrackingList.Add((Exception) ex);
              }
              else
                throw;
            }
            else
            {
              if (ex.IsMatch("invalidRange"))
                return new UploadChunkResult();
              throw;
            }
          }
        }
      }
    }

    internal List<Tuple<long, long>> GetRangesRemaining(UploadSession session)
    {
      List<Tuple<long, long>> rangesRemaining = new List<Tuple<long, long>>();
      foreach (string nextExpectedRange in session.NextExpectedRanges)
      {
        string[] strArray = nextExpectedRange.Split('-');
        rangesRemaining.Add(new Tuple<long, long>(long.Parse(strArray[0]), string.IsNullOrEmpty(strArray[1]) ? this.totalUploadLength - 1L : long.Parse(strArray[1])));
      }
      return rangesRemaining;
    }

    private int NextChunkSize(long rangeBegin, long rangeEnd)
    {
      int num = (int) (rangeEnd - rangeBegin) + 1;
      return num <= this.maxChunkSize ? num : this.maxChunkSize;
    }
  }
}
