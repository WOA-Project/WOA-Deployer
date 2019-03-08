﻿using System;

namespace Deployer.Tasks
{
    [TaskDescription("Fetching from GitHub: {0} - {1}")]
    public class FetchGitHubBranch : FetchGitHubBase
    {
        public FetchGitHubBranch(string repoBaseUrl, string branch, IZipExtractor extractor, IObserver<double> progressObserver) :
            base(repoBaseUrl, branch, extractor, progressObserver)
        {
        }
    }
}