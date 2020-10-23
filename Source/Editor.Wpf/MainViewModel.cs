using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using Deployer.Core.Compiler;
using ICSharpCode.AvalonEdit;
using Iridio.Binding.Model;
using Iridio.Common;
using Microsoft.Xaml.Behaviors;
using ReactiveUI;
using Zafiro.Core;
using Zafiro.Core.Files;
using Zafiro.Core.Mixins;
using Zafiro.Core.Patterns.Either;
using Zafiro.Core.UI;

namespace Editor.Wpf
{
    public sealed class AvalonEditBehaviour : Behavior<TextEditor> 
    {
        public static readonly DependencyProperty GiveMeTheTextProperty =
            DependencyProperty.Register("GiveMeTheText", typeof(string), typeof(AvalonEditBehaviour), 
                new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback));

        public string GiveMeTheText
        {
            get { return (string)GetValue(GiveMeTheTextProperty); }
            set { SetValue(GiveMeTheTextProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
                AssociatedObject.TextChanged += AssociatedObjectOnTextChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null)
                AssociatedObject.TextChanged -= AssociatedObjectOnTextChanged;
        }

        private void AssociatedObjectOnTextChanged(object sender, EventArgs eventArgs)
        {
            var textEditor = sender as TextEditor;
            if (textEditor != null)
            {
                if (textEditor.Document != null)
                    GiveMeTheText = textEditor.Document.Text;
            }
        }

        private static void PropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var behavior = dependencyObject as AvalonEditBehaviour;
            if (behavior.AssociatedObject!= null)
            {
                var editor = behavior.AssociatedObject as TextEditor;
                if (editor.Document != null)
                {
                    var caretOffset = editor.CaretOffset;
                    editor.Document.Text = dependencyPropertyChangedEventArgs.NewValue.ToString();
                    editor.CaretOffset = caretOffset;
                }
            }
        }
    }

    public class MainViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<ValidationResult> validate;

        public MainViewModel(IDeployerCompiler compiler, IOpenFilePicker picker, IDialogService dialogService)
        {
            OpenFile = ReactiveCommand.CreateFromObservable(() =>
                picker.Picks(new[] {new FileTypeFilter("Text files", "*.txt")}, () => null,
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
                    using (var r = new StreamWriter(stream, Encoding.Default){ AutoFlush = true })
                    {
                        await r.WriteAsync(SourceCode);
                    }
                }
            }, hasFile);

            
            Compile = ReactiveCommand.Create(() => compiler.Compile(File.Source.OriginalString, new List<Assignment>()), hasFile);
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