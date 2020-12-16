using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Deployer.Core.Compiler;
using Deployer.Core.Deployers.Errors.Compiler;
using Deployer.Core.Deployers.Errors.Deployer;
using Deployer.Core.Requirements;
using Deployer.Net4x;
using DynamicData;
using Iridio.Binding.Model;
using Iridio.Runtime;
using MediatR;
using ReactiveUI;
using Zafiro.Core;
using Zafiro.Core.Files;
using Zafiro.Core.Patterns.Either;
using Zafiro.UI;
using Unit = System.Reactive.Unit;

namespace Deployer.Ide
{
    public class MainViewModel : ReactiveObject
    {
        private readonly WoaDeployerBase deployer;
        private readonly ObservableAsPropertyHelper<IZafiroFile> file;
        private readonly ObservableAsPropertyHelper<string> fileSource;

        private string sourceCode;
        private CompositeDisposable disposables = new CompositeDisposable();
        private ReadOnlyObservableCollection<string> output;
        private ReadOnlyObservableCollection<string> runtimeMessages;
        private readonly SourceList<string> outputList;
        private SourceList<string> buildList;

        public MainViewModel(WoaDeployerBase deployer, IIdeDeployerCompiler compiler, IOpenFilePicker picker)
        {
            this.deployer = deployer;
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

            openFileLoader
                .ObserveOnDispatcher()
                .Subscribe(s => SourceCode = s);

            Compile = ReactiveCommand.CreateFromTask(async () =>
            {
                await Save.Execute();
                return await CompileCore.Execute();
            }, hasFile);

            fileSource = openFileLoader.ToProperty(this, model => model.FileSource);

            Run = ReactiveCommand.CreateFromTask(async () =>
            {
                await Save.Execute();
                var p = await CompileCore.Execute();
                var l = p.MapRight(async script => await RunCore.Execute());
                await l.RightTask();
            }, hasFile);

            RunCore = ReactiveCommand.CreateFromTask(() => deployer.Run(File.Source.OriginalString));
            CompileCore = ReactiveCommand.CreateFromTask(() => compiler.Compile(File.Source.OriginalString));

            deployer.Messages
                .ToObservableChangeSet()
                .Bind(out output)
                .ObserveOnDispatcher()
                .DisposeMany()
                .Subscribe()
                .DisposeWith(disposables);

            buildList = new SourceList<string>();
            buildList.Connect()
                .Bind(out runtimeMessages)
                .ObserveOnDispatcher()
                .DisposeMany()
                .Subscribe()
                .DisposeWith(disposables);

            CompileCore.Subscribe(either => buildList.AddRange(Extract(either)));
            RunCore.Subscribe(either => buildList.AddRange(Extract(either)));

            ResetBuild = ReactiveCommand.Create(() => buildList.Clear());
        }

        public ReactiveCommand<Unit, Unit> ResetBuild { get; set; }

        private IEnumerable<string> Extract(Either<DeployerError, Success> either)
        {
            return either.MapRight(s => (IEnumerable<string>)new[] {"Execution finished successfully"})
                .Handle(s => s.Items);
        }

        public ReactiveCommand<Unit, Either<DeployerCompilerError, Script>> CompileCore { get; }

        public ReactiveCommand<Unit, Either<DeployerError, Success>> RunCore { get; }

        private IEnumerable<string> Extract(Either<DeployerCompilerError, Script> either)
        {
            return either
                .MapRight(s => (IEnumerable<string>)new[] { "Static analysis is OK" })
                .Handle(s => s.Items);
        }

        public ReactiveCommand<Unit, Unit> Run { get; }
        
        public ReactiveCommand<Unit, Unit> Save { get; }
        
        public string FileSource => fileSource.Value;

        public IZafiroFile File => file.Value;

        public ReactiveCommand<Unit, IZafiroFile> OpenFile { get; }

        public ReactiveCommand<Unit, Either<DeployerCompilerError, Script>> Compile { get; }

        public string SourceCode
        {
            get => sourceCode;
            set => this.RaiseAndSetIfChanged(ref sourceCode, value);
        }

        public ReadOnlyObservableCollection<string> Output => output;
        public ReadOnlyObservableCollection<string> RuntimeMessages => runtimeMessages;

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