using System;
using System.Reactive.Subjects;

namespace Deployer
{
    public interface IDownloadProgress
    {
        ISubject<double> Percentage { get; set; }
        ISubject<long> BytesDownloaded { get; set; }
    }
}