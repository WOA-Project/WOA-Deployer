using System;
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

            if (ps.HadErrors)
            {
                throw new ApplicationException($"The execution of the script '{script}' failed");
            }

            return results;
        }

        public static async Task<PSDataCollection<PSObject>> ExecuteCommand(this PowerShell ps, string commandText, params (string, object)[] parameters)
        {
            ps.Commands.Clear();

            var command = ps.AddCommand(commandText);
            
            foreach (var (arg, v) in parameters)
            {
                if (v == null)
                {
                    command.AddParameter(arg);
                }
                else
                {
                    command.AddParameter(arg, v);
                }                
            }

            var psDataCollection = await Task.Factory.FromAsync(ps.BeginInvoke(), ps.EndInvoke);

            if (ps.HadErrors)
            {
                throw new ApplicationException($"The execution of the command '{commandText}' failed");
            }

            return psDataCollection;
        }

        public static async Task<PSDataCollection<PSObject>> ExecuteCommand(this PowerShell ps, string commandText, IEnumerable<object> arguments, params (string, object)[] parameters)
        {
            ps.Commands.Clear();

            var command = ps.AddCommand(commandText);
            
            foreach (var (arg, v) in parameters)
            {
                command.AddParameter(arg, v);
            }

            var psDataCollection = await Task.Factory.FromAsync(ps.BeginInvoke(), ps.EndInvoke);

            if (ps.HadErrors)
            {
                throw new ApplicationException($"The execution of the command '{commandText}' failed");
            }

            return psDataCollection;
        }
    }
}