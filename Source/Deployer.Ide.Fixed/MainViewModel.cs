using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Deployer.Core.Compiler;
using Deployer.Core.Deployers;
using Deployer.Core.Deployers.Errors;
using Deployer.Core.Deployers.Errors.Compiler;
using Deployer.Core.Deployers.Errors.Deployer;
using Deployer.Core.Requirements;
using Deployer.Net4x;
using Iridio.Binding.Model;
using Iridio.Common;
using Iridio.Runtime;
using MediatR;
using ReactiveUI;
using Zafiro.Core;
using Zafiro.Core.Files;
using Zafiro.Core.Patterns.Either;
using Zafiro.UI;
using Error = Iridio.Common.Error;
using Unit = System.Reactive.Unit;

namespace Deployer.Ide
{
    public class MainViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<IZafiroFile> file;
        private readonly ObservableAsPropertyHelper<string> fileSource;
        private readonly ObservableAsPropertyHelper<ValidationResult> validate;

        private string sourceCode;
        private CompositeDisposable disposables = new CompositeDisposable();

        public MainViewModel(WoaDeployerBase deployer, IIdeDeployerCompiler compiler, IOpenFilePicker picker)
        {
            OpenFile = ReactiveCommand.CreateFromObservable(() =>
                picker.Picks(new[] {new FileTypeFilter("Text files", "*.txt")}, () => null,
                    s => { }));

            file = OpenFile
                .SubscribeOnDispatcher()
                .ToProperty(this, model => model.File);
            var openFileLoader = OpenFile
                .SelectMany(async file =>
                {
                    using var openForRead = await file.OpenForRead();
                    using var stream = new StreamReader(openForRead);
                    var readToEndAsync = await stream.ReadToEndAsync();
                    return readToEndAsync;
                });

            var hasFile = this.WhenAnyValue(v => v.File).Select(z => z != null);

            Save = ReactiveCommand.CreateFromTask(SaveFile, hasFile);

            Compile = ReactiveCommand.CreateFromTask(async () =>
            {
                var compile = await compiler.Compile(File.Source.OriginalString);
                return compile;
            }, hasFile);
            validate = Compile
                .Select(e => e
                    .MapRight(unit => new ValidationResult(unit))
                    .Handle(errors => new ValidationResult(errors)))
                .ToProperty(this, model => model.ValidationResult);

            openFileLoader
                .ObserveOnDispatcher()
                .Subscribe(s => SourceCode = s);

            fileSource = openFileLoader.ToProperty(this, model => model.FileSource);

            SaveAndCompile = Save;
            SaveAndCompile.InvokeCommand(Compile).DisposeWith(disposables);

            Run = ReactiveCommand.CreateFromTask(() => deployer.Run(File.Source.OriginalString), hasFile);

            SaveAndRun = Save;
            SaveAndRun.InvokeCommand(Run).DisposeWith(disposables);
        }

        public ReactiveCommand<Unit, Unit> SaveAndRun { get; set; }

        public ReactiveCommand<Unit, Either<DeployerError, Success>> Run { get; }

        public ReactiveCommand<Unit, Unit> SaveAndCompile { get; }


        public ReactiveCommand<Unit, Unit> Save { get; }


        public string FileSource => fileSource.Value;

        public IZafiroFile File => file.Value;

        public ReactiveCommand<Unit, IZafiroFile> OpenFile { get; }

        public ValidationResult ValidationResult => validate.Value;

        public ReactiveCommand<Unit, Either<DeployerCompilerError, Script>> Compile { get; }

        public string SourceCode
        {
            get => sourceCode;
            set => this.RaiseAndSetIfChanged(ref sourceCode, value);
        }

        private async Task<IEnumerable<Assignment>> SatisfyRequirements(IRequirementsAnalyzer requirementsAnalyzer,
            ISender mediator)
        {
            var requirements = requirementsAnalyzer.GetRequirements(await File.ReadToEnd());
            var missing = requirements.Handle(list => Enumerable.Empty<MissingRequirement>());
            var responses = await missing.Select<MissingRequirement, RequirementRequest>(r =>
            {
                if (r.Kind == RequirementKind.WimFile) return new WimFileRequest {Index = 0, Path = "", Key = r.Key};

                if (r.Kind == RequirementKind.Disk) return new DiskRequest {Index = 0, Key = r.Key};

                throw new ArgumentOutOfRangeException();
            }).AsyncSelect(async re =>
            {
                var send = await mediator.Send(re);
                return TurnIntoAssignments((RequirementResponse) send);
            });

            var selectMany = responses.SelectMany(x => x);

            return selectMany;
        }

        private async Task SaveFile()
        {
            using (var stream = await File.OpenForWrite())
            {
                using (var r = new StreamWriter(stream, Encoding.Default) {AutoFlush = true})
                {
                    await r.WriteAsync(SourceCode);
                }
            }
        }

        private IEnumerable<Assignment> TurnIntoAssignments(RequirementResponse responses)
        {
            return responses.Select(r => new Assignment(r.Key, "#placeholder#"));
        }
    }
}