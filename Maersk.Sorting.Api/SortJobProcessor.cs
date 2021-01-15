using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public class SortJobProcessor : ISortJobProcessor
    {
        private readonly ILogger<SortJobProcessor> _logger;
        private List<SortJob> _listSortJob;
        public SortJobProcessor(ILogger<SortJobProcessor> logger)
        {
            _logger = logger;
            _listSortJob = new List<SortJob>();
        }

        public async Task<SortJob> Process(SortJob job)
        {
            _logger.LogInformation("Processing job with ID '{JobId}'.", job.Id);
            _listSortJob.Add(job);
            var stopwatch = Stopwatch.StartNew();

            var output = job.Input.OrderBy(n => n).ToArray();
            await Task.Delay(50000); // NOTE: This is just to simulate a more expensive operation

            var duration = stopwatch.Elapsed;

            _logger.LogInformation("Completed processing job with ID '{JobId}'. Duration: '{Duration}'.", job.Id, duration);
            var item = _listSortJob.Remove(_listSortJob.Where(x=>x.Id == job.Id).FirstOrDefault());
            var completedJob = new SortJob(
                id: job.Id,
                status: SortJobStatus.Completed,
                duration: duration,
                input: job.Input,
                output: output);
            _listSortJob.Add(completedJob);
            return completedJob;
        }

        public async Task<SortJob []> GetJobs()
        {
            var result = _listSortJob.ToArray();
            return await Task.FromResult(result);
        }
        public async Task<SortJob> GetJob(Guid jobId)
        {
            return await Task.FromResult(_listSortJob.Where(x => x.Id == jobId).FirstOrDefault());
        }
    }
}
