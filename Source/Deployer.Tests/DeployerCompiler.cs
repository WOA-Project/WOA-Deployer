using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Iridio.Binding;
using Iridio.Binding.Model;
using Iridio.Common;
using Iridio.Parsing;
using Iridio.Parsing.Model;
using Zafiro.Core;
using Zafiro.Core.FileSystem;
using Zafiro.Core.Patterns.Either;
using Optional;
using Optional.Collections;
using Xunit;

namespace Deployer.Tests
{
    public class DeployerCompilerTests
    {
        [Fact]
        public void Injection()
        {

            var sut = new DeployerCompiler(new Preprocessor(new FileFileSystemOperations() ), new FakeParser(), new FakeBinder());
            var compilation = sut.Compile("pepito",
                new InjectableVariableDeclaration[] {new InjectableVariableDeclaration("Test", 123),});
            
            compilation
                .MapRight(unit => unit.ToString())
                .Should()
                .Be(Either.Success<Errors, string>(""));
        }
    }

    public class FileFileSystemOperations : IFileSystemOperations
    {
        public Task Copy(string source, string destination, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task CopyDirectory(string source, string destination, string fileSearchPattern = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task DeleteDirectory(string path)
        {
            throw new NotImplementedException();
        }

        public bool DirectoryExists(string path)
        {
            throw new NotImplementedException();
        }

        public bool FileExists(string path)
        {
            throw new NotImplementedException();
        }

        public void CreateDirectory(string path)
        {
            throw new NotImplementedException();
        }

        public void EnsureDirectoryExists(string directoryPath)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFile(string filePath)
        {
            throw new NotImplementedException();
        }

        public string GetTempFileName()
        {
            throw new NotImplementedException();
        }

        public string ReadAllText(string path)
        {
            throw new NotImplementedException();
        }

        public void WriteAllText(string path, string text)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> QueryDirectory(string root, Func<string, bool> selector = null)
        {
            throw new NotImplementedException();
        }

        public Stream OpenForWrite(string path)
        {
            throw new NotImplementedException();
        }

        public string GetTempDirectoryName()
        {
            throw new NotImplementedException();
        }

        public Stream OpenForRead(string path)
        {
            throw new NotImplementedException();
        }

        public string WorkingDirectory { get; set; }
    }


    public class FakeParser : IParser
    {
        public Either<ParsingError, EnhancedScript> Parse(string source)
        {
            throw new NotImplementedException();
        }
    }

    public class FakeBinder : IBinder
    {
        public Either<Errors, CompilationUnit> Bind(EnhancedScript script)
        {
            throw new NotImplementedException();
        }
    }

    public class DeployerCompiler
    {
        private readonly IParser parser;
        private readonly IBinder binder;
        private readonly IEqualityComparer<ProcedureDeclaration> procedureComparer = new LambdaComparer<ProcedureDeclaration>((a, b) => Equals(a.Name, b.Name));
        private readonly Preprocessor preprocessor;

        public DeployerCompiler(Preprocessor preprocessor, IParser parser, IBinder binder)
        {
            this.parser = parser;
            this.binder = binder;
            this.preprocessor = preprocessor;
        }

        public Either<Errors, CompilationUnit> Compile(string path, IEnumerable<InjectableVariableDeclaration> providedRequirements)
        {
            var input = preprocessor.Process(path);
            var parsed = parser.Parse(input);

            return parsed
                .MapLeft(error => new Errors(new Error(ErrorKind.UnableToParse, error.Message)))
                .MapRight(script => InjectRequirements(script, providedRequirements))
                .MapRight(script => binder.Bind(script));
        }

        private Either<Errors, EnhancedScript> InjectRequirements(EnhancedScript script, IEnumerable<InjectableVariableDeclaration> providedRequirements)
        {
            return GetMain(script)
                .Match(main =>
                {
                    ProcedureDeclaration modifiedMain = ModifyMain(main, providedRequirements);
                    var modifiedProcedures = script.Procedures.Except(new[] { modifiedMain }, procedureComparer);
                    var modifiedScript = new EnhancedScript(script.Header, modifiedProcedures.ToArray());
                    return modifiedScript;
                }, () => Either.Error<Errors, EnhancedScript>(new Errors(new InjectError())));
        }

        private ProcedureDeclaration ModifyMain(ProcedureDeclaration main,
            IEnumerable<InjectableVariableDeclaration> injectableVariableDeclarations)
        {
            return new ProcedureDeclaration(main.Name, ModifiedBlock(main.Block, injectableVariableDeclarations));
        }

        private Block ModifiedBlock(Block mainBlock,
            IEnumerable<InjectableVariableDeclaration> injectableVariableDeclarations)
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
            return OptionCollectionExtensions
                .FirstOrNone(script.Procedures, d => d.Name.Equals("Main", StringComparison.InvariantCulture));
        }
    }
}