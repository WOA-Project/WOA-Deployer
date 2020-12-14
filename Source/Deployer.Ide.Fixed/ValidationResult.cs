using System;
using System.Collections.Generic;
using System.Linq;
using Deployer.Core.Deployers;
using Deployer.Core.Deployers.Errors;
using Deployer.Core.Deployers.Errors.Compiler;
using Iridio.Binding.Model;
using Iridio.Common;

namespace Deployer.Ide
{
    public class ValidationResult
    {
        public IEnumerable<IdeMessage> Messages { get; }

        public ValidationResult(Script unit)
        {
            Messages = new [] { new IdeMessage("Success!"), };
        }

        public ValidationResult(DeployerCompilerError error)
        {
            Messages = ToError(error);
        }

        private static IEnumerable<IdeMessage> ToError(DeployerCompilerError error)
        {
            switch (error)
            {
                case UnableToCompile compileError:
                    return compileError.Errors.Select(x => new IdeMessage(x.ToString()));
                case UnableToSatisfyRequirements requirementsError:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(error));
            }

            throw new NotSupportedException();
        }
    }
}