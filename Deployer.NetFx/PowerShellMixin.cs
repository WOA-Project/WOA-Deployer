using System.Collections.Generic;
using System.Management.Automation;
using System.Threading.Tasks;

namespace Deployer.NetFx
{
    public static class PowerShellMixin
    {
        public static async Task<PSDataCollection<PSObject>> ExecuteScript(this PowerShell ps, string script)
        {
            ps.Commands.Clear();
            ps.AddScript(script, true);
            var results = await Task.Factory.FromAsync(ps.BeginInvoke(), ps.EndInvoke);
            return results;
        }

        public static async Task<PSDataCollection<PSObject>> ExecuteCommand(this PowerShell ps, string commandText, params (string, object)[] parameters)
        {
            ps.Commands.Clear();

            var command = ps.AddCommand(commandText);
            
            foreach (var (arg, v) in parameters)
            {
                command.AddParameter(arg, v);
            }

            return await Task.Factory.FromAsync(ps.BeginInvoke(), ps.EndInvoke);
        }
    }
}