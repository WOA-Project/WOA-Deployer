using System;
using System.Collections.Generic;
using System.Linq;
using Iridio.Binding;
using Iridio.Binding.Model;
using Iridio.Common;
using Iridio.Parsing;
using Iridio.Parsing.Model;
using Optional;
using Optional.Collections;
using Zafiro.Core;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Core.Compiler
{
    public class DeployerCompiler : IDeployerCompiler
    {
        private readonly IParser parser;
        private readonly IBinder binder;
        private readonly IEqualityComparer<ProcedureDeclaration> procedureComparer = new LambdaComparer<ProcedureDeclaration>((a, b) => Equals(a.Name, b.Name));
        private readonly IPreprocessor preprocessor;

        public DeployerCompiler(IPreprocessor preprocessor, IParser parser, IBinder binder)
        {
            this.parser = parser;
            this.binder = binder;
            this.preprocessor = preprocessor;
        }

        public Either<Errors, CompilationUnit> Compile(string path, IEnumerable<Assignment> toInject)
        {
            var input = preprocessor.Process(path);
            var parsed = parser.Parse(input);

            return parsed
                .MapLeft(error => new Errors(new Error(ErrorKind.UnableToParse, error.Message)))
                .MapRight(script => Inject(script, toInject))
                .MapRight(script => binder.Bind(script));
        }

        private Either<Errors, EnhancedScript> Inject(EnhancedScript script, IEnumerable<Assignment> toInject)
        {
            return GetMain(script)
                .Match(main =>
                {
                    ProcedureDeclaration modifiedMain = ModifyMain(main, toInject);
                    var modifiedProcedures = script.Procedures.Except(new[] { modifiedMain }, procedureComparer).Concat(new[] {modifiedMain});
                    var modifiedScript = new EnhancedScript(script.Header, modifiedProcedures.ToArray());
                    return modifiedScript;
                }, () => Either.Error<Errors, EnhancedScript>(new Errors(new InjectError())));
        }

        private ProcedureDeclaration ModifyMain(ProcedureDeclaration main,
            IEnumerable<Assignment> injectableVariableDeclarations)
        {
            return new ProcedureDeclaration(main.Name, ModifiedBlock(main.Block, injectableVariableDeclarations));
        }

        private Block ModifiedBlock(Block mainBlock,
            IEnumerable<Assignment> injectableVariableDeclarations)
        {
            var declarations = injectableVariableDeclarations
                .Select(assignment => new AssignmentStatement(assignment.Name, ValueToExpression(assignment.Value)));
            var statements = declarations.Concat(mainBlock.Statements);
            return new Block(statements.ToArray());
        }

        private static Expression ValueToExpression(object assignmentValue)
        {
            switch (assignmentValue)
            {
                case int i:
                    return new NumericExpression(i);
                case string s:
                    return new StringExpression(s);
            }

            throw new NotSupportedException();
        }

        private static Option<ProcedureDeclaration> GetMain(EnhancedScript script)
        {
            return script
                .Procedures
                .FirstOrNone(d => d.Name.Equals("Main", StringComparison.InvariantCulture));
        }
    }
}