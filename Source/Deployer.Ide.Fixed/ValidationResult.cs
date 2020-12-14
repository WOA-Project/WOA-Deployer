using System;
using System.Collections.Generic;
using System.Linq;
using Deployer.Core.Deployers;
using Deployer.Core.Deployers.Errors;
using Deployer.Core.Deployers.Errors.Compiler;
using Deployer.Core.Deployers.Errors.Deployer;
using Iridio.Binding.Model;
using Iridio.Common;
using Iridio.Runtime;

namespace Deployer.Ide
{
    public class ValidationResult
    {
        public IEnumerable<IdeMessage> Messages { get; }

        public ValidationResult(Script unit)
        {
            Messages = new[] { new IdeMessage("Success!"), };
        }

        public ValidationResult(DeployerCompilerError error)
        {
            switch (error)
            {
                case UnableToCompile unableToCompile:
                    break;
                case UnableToSatisfyRequirements unableToSatisfyRequirements:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(error));
            }
        }

        public ValidationResult(Success error)
        {
            Messages = new[] { new IdeMessage("Execution successful") };
        }

        public ValidationResult(DeployerError error)
        {
            Messages = GetMessages(error);
        }

        private IEnumerable<IdeMessage> GetMessages(DeployerError error)
        {
            return error switch
            {
                CompilationFailed compilationFailed => new[]
                {
                    new IdeMessage("Compilation failed: " + compilationFailed.Error),
                },
                ExecutionFailed executionFailed => GetMessages(executionFailed.Errors),
                RequirementsFailed requirementsFailed => new[]
                {
                    new IdeMessage("Compilation failed: " + requirementsFailed.RequirementsError),
                },
                _ => throw new ArgumentOutOfRangeException(nameof(error))
            };
        }

        private IEnumerable<IdeMessage> GetMessages(RuntimeErrors executionFailedErrors)
        {
            return executionFailedErrors.SelectMany(error =>
            {
                return error switch
                {
                    IntegratedFunctionFailed integratedFunctionFailed => new[]
                    {
                        new IdeMessage(
                            $"Integrated function '{integratedFunctionFailed.Function}' has thrown an exception: {integratedFunctionFailed.Exception?.InnerException?.Message}"),
                    },
                    TypeMismatch typeMismatch => new[] {new IdeMessage($"Type mismatch"),},
                    UndeclaredFunction undeclaredFunction => new[]
                    {
                        new IdeMessage($"Function '{undeclaredFunction.FunctionName}' is missing"),
                    },
                    UserCanceledExecution _ => new[]
                    {
                        new IdeMessage($"The user has canceled the execution"),
                    },
                    _ => throw new ArgumentOutOfRangeException(nameof(error))
                };
            });
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