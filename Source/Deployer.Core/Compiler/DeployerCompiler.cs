using System;
using System.Collections.Generic;
using System.Linq;
using Iridio;
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
        private readonly IEqualityComparer<Procedure> procedureComparer = new LambdaComparer<Procedure>((a, b) => Equals(a.Name, b.Name));
        private readonly IPreprocessor preprocessor;

        public DeployerCompiler(IPreprocessor preprocessor, IParser parser, IBinder binder)
        {
            this.parser = parser;
            this.binder = binder;
            this.preprocessor = preprocessor;
        }

        public Either<CompilerError, Script> Compile(string path, IEnumerable<Assignment> toInject)
        {
            var input = preprocessor.Process(path);
            var parsed = parser.Parse(input);

            return parsed
                .MapLeft(error => (CompilerError)new ParseError(error))
                .MapRight(script => TryInject(script, toInject).Match(syntax => syntax, () => script))
                .MapRight(script =>
                {
                    var either = binder.Bind(script)
                        .MapLeft(errors => (CompilerError)new BindError(errors));
                    return either;
                });
        }

        private Option<IridioSyntax> TryInject(IridioSyntax script, IEnumerable<Assignment> toInject)
        {
            return GetMain(script)
                .Map(main =>
                {
                    Procedure modifiedMain = ModifyMain(main, toInject);
                    var modifiedProcedures = script.Procedures.Except(new[] { modifiedMain }, procedureComparer).Concat(new[] {modifiedMain});
                    var modifiedScript = new IridioSyntax(modifiedProcedures.ToArray());
                    return modifiedScript;
                });
        }

        private Procedure ModifyMain(Procedure main,
            IEnumerable<Assignment> injectableVariableDeclarations)
        {
            return new Procedure(main.Name, ModifiedBlock(main.Block, injectableVariableDeclarations));
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
                case int n:
                    return new IntegerExpression(n);
                case uint n:
                    return new IntegerExpression((int) n);
                case string s:
                    return new StringExpression(s);
                case double d:
                    return new DoubleExpression(d);
            }

            throw new NotSupportedException();
        }

        private static Option<Procedure> GetMain(IridioSyntax script)
        {
            return OptionCollectionExtensions.FirstOrNone(script.Procedures,
                d => d.Name.Equals("Main", StringComparison.InvariantCulture));
        }
    }
}