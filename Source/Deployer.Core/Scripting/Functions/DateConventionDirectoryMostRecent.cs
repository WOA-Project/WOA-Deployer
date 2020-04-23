using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Deployer.Core.Scripting.Core;
using Zafiro.Core.FileSystem;

namespace Deployer.Core.Scripting.Functions
{
    public class DateConventionDirectoryMostRecent : DeployerFunction
    {
        public DateConventionDirectoryMostRecent(IFileSystemOperations fileSystemOperations, IOperationContext operationContext) : base(fileSystemOperations, operationContext)
        {
        }

        public Task<string> Execute(string rootToExamine)
        {
            var query = FileSystemOperations.QueryDirectory(rootToExamine)
                .Select(s =>
                {
                    TryParse(s, out var date);
                    return new {Dir = s, Date = date};
                })
                .OrderByDescending(arg => arg.Date)
                .Select(x => x.Dir);

            return Task.FromResult(query.First());
        }

        private static bool TryParse(string directory, out DateTime date)
        {
            var dirName = Path.GetFileName(directory);
            var candidate = dirName.Split('-');
            if (candidate.Length > 1)
            {
                var datePart = candidate[0];
                if (DateTime.TryParseExact(datePart, "yyyyMMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    return true;
                }
            }

            date = default;
            return false;
        }
    }
}