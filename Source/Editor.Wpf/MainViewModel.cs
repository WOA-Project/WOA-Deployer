using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using Deployer.Core.Compiler;
using Deployer.Core.Requirements;
using Iridio.Binding.Model;
using Iridio.Common;
using MediatR;
using ReactiveUI;
using Zafiro.Core;
using Zafiro.Core.Files;
using Zafiro.Core.Mixins;
using Zafiro.Core.Patterns.Either;
using Zafiro.Core.UI;
using Unit = System.Reactive.Unit;

namespace Editor.Wpf
{
    public class MainViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<ValidationResult> validate;

        public MainViewModel(IDeployerCompiler compiler, IOpenFilePicker picker, IRequirementsAnalyzer requirementsAnalyzer, IMediator mediator)
        {
            OpenFile = ReactiveCommand.CreateFromObservable(() =>
                picker.Picks(new[] { new FileTypeFilter("Text files", "*.txt") }, () => null,
                    s => { }));

            file = OpenFile
                .SubscribeOnDispatcher()
                .ToProperty(this, model => model.File);
            var openFileLoader = OpenFile
                .SelectMany(async z =>
                {
                    await using var openForRead = await z.OpenForRead();
                    using var stream = new StreamReader(openForRead);
                    var readToEndAsync = await stream.ReadToEndAsync();
                    return readToEndAsync;
                });

            var hasFile = this.WhenAnyValue(v => v.File).Select(z => z != null);

            Save = ReactiveCommand.CreateFromTask(async () =>
            {
                using (var stream = await File.OpenForWrite())
                {
                    using (var r = new StreamWriter(stream, Encoding.Default) { AutoFlush = true })
                    {
                        await r.WriteAsync(SourceCode);
                    }
                }
            }, hasFile);


            Compile = ReactiveCommand.CreateFromTask(async () =>
            {
                string contents;
                using (var openForRead = await File.OpenForRead())
                {
                    contents = await openForRead.ReadToEnd();
                }

                var requirements = requirementsAnalyzer.GetRequirements(contents);
                var missing = requirements.Handle(list => Enumerable.Empty<MissingRequirement>());
                var responses = await missing.Select(r =>
                {
                    if (r.Kind == RequirementKind.WimFile)
                    {
                        return (RequirementRequest)new WimFileRequest(){Index = 0, Path = "", Key =  r.Key};
                    } 

                    if (r.Kind == RequirementKind.Disk)
                    {
                        return (RequirementRequest)new DiskRequest(){Index = 0, Key = r.Key};
                    } 

                    throw new ArgumentOutOfRangeException();
                    
                }).AsyncSelect(async re =>
                {
                    var send = await mediator.Send(re);
                    return TurnIntoAssignments((RequirementResponse) send);
                });

                return compiler.Compile(File.Source.OriginalString, responses.SelectMany(x => x));
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
            subscription = SaveAndCompile.InvokeCommand(Compile);
        }

        private IEnumerable<Assignment> TurnIntoAssignments(RequirementResponse responses)
        {
            return responses.Select(r => new Assignment(r.Key, "#placeholder#"));
        }

        public ReactiveCommand<Unit, Unit> SaveAndCompile { get; set; }


        public ReactiveCommand<Unit, Unit> Save { get; set; }


        public string FileSource => fileSource.Value;

        public IZafiroFile File => file.Value;

        public ReactiveCommand<Unit, IZafiroFile> OpenFile { get; set; }

        public ValidationResult ValidationResult => validate.Value;

        public ReactiveCommand<Unit, Either<Errors, CompilationUnit>> Compile { get; }

        public string Path { get; set; }

        private string sourceCode;
        private ObservableAsPropertyHelper<IZafiroFile> file;
        private ObservableAsPropertyHelper<string> fileSource;
        private IDisposable subscription;

        public string SourceCode
        {
            get => sourceCode;
            set => this.RaiseAndSetIfChanged(ref sourceCode, value);
        }
    }

    public class ValidationResult
    {
        public IEnumerable<string> Messages { get; }

        public ValidationResult(CompilationUnit unit)
        {
            Messages = new[] { "Compile operation successful" };
        }

        public ValidationResult(Errors messages)
        {
            Messages = messages.Select(e => e.ErrorKind + ": " + e.AdditionalData);
        }
    }
}