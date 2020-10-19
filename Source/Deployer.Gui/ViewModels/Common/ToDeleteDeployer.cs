using System;
using System.Reactive;
using System.Threading.Tasks;
using Deployer.Core;

namespace Deployer.Gui.ViewModels.Common
{
    public class ToDeleteDeployer
    {
        public IObservable<string> Messages { get; set; }

        public Task<Unit> RunScript(string sourceLocalPath)
        {
            throw new NotImplementedException();
        }

        public Task<Unit> Deploy(string deploymentScriptPath, Device first)
        {
            throw new NotImplementedException();
        }
    }
}