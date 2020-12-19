using System;
using System.Threading.Tasks;
using Deployer.Core.Deployers.Errors.Deployer;
using Deployer.Core.Scripting;
using Iridio.Runtime.ReturnValues;
using Zafiro.Core;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Core
{
    public interface IWoaDeployer
    {
        IOperationProgress OperationProgress { get; }
        IOperationContext OperationContext { get; }
        IObservable<string> Messages { get; }
        Task<Either<DeployerError, Success>> Run(string scriptPath);
    }
}