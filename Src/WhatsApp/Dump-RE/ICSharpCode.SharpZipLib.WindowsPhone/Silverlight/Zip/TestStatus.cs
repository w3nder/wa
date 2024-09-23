// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Zip.TestStatus
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Zip
{
  public class TestStatus
  {
    private ZipFile file_;
    private ZipEntry entry_;
    private bool entryValid_;
    private int errorCount_;
    private long bytesTested_;
    private TestOperation operation_;

    public TestStatus(ZipFile file) => this.file_ = file;

    public TestOperation Operation => this.operation_;

    public ZipFile File => this.file_;

    public ZipEntry Entry => this.entry_;

    public int ErrorCount => this.errorCount_;

    public long BytesTested => this.bytesTested_;

    public bool EntryValid => this.entryValid_;

    internal void AddError()
    {
      ++this.errorCount_;
      this.entryValid_ = false;
    }

    internal void SetOperation(TestOperation operation) => this.operation_ = operation;

    internal void SetEntry(ZipEntry entry)
    {
      this.entry_ = entry;
      this.entryValid_ = true;
      this.bytesTested_ = 0L;
    }

    internal void SetBytesTested(long value) => this.bytesTested_ = value;
  }
}
