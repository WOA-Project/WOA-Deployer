using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Deployer.Core.Requirements;
using FluentAssertions;
using Xunit;
using Zafiro.Core;
using Zafiro.Core.Patterns.Either;
using Zafiro.Core.UI.Interaction;
using Option = Zafiro.Core.UI.Interaction.Option;
using Unit = System.Reactive.Unit;

namespace Deployer.Tests
{
    public class RequirementSupplierTests
    {
        [Fact]
        public async Task Supply()
        {
            var requirements = new[]
            {
                new FulfilledRequirement("wimFilePath", RequirementKind.Disk,  "path"),
                new FulfilledRequirement("wimFileIndex", RequirementKind.Disk, 1),
            };

            var testShell = new TestShell(options => options[0].Command.Execute(null));
            var sut = new RequirementSupplier(new Dictionary<RequirementKind, RequirementSolver>()
            {
                { RequirementKind.WimFile, new TestRequirementSolver(requirements)}
            }, testShell, () => null);

            var missingRequirements = new List<MissingRequirement>()
            {
                new MissingRequirement("wimFile", RequirementKind.WimFile, "")
            };

            var satisfyResult = await sut.Satisfy(missingRequirements);
           
            var expected = Either.Success<Error, IEnumerable<FulfilledRequirement>>(requirements);

            satisfyResult.Should().BeEquivalentTo(expected);
        }
    }

    public class TestRequirementSolver : RequirementSolver
    {
        private IEnumerable<FulfilledRequirement> list;

        public TestRequirementSolver(IEnumerable<FulfilledRequirement> list)
        {
            this.list = list;
        }

        public override IObservable<bool> IsValid => Observable.Return(true);
        public override IEnumerable<FulfilledRequirement> FulfilledRequirements()
        {
            return list;
        }
    }

    public class TestShell: IShell
    {
        private readonly Action<Collection<Option>> afterConfigure;

        public TestShell(Action<Collection<Option>> afterConfigure)
        {
            this.afterConfigure = afterConfigure;
        }

        public Task Popup<T>(IContextualizable content, T viewModel, Action<PopupConfiguration<T>> configure)
        {
            var config = new PopupConfiguration<T>(new TestPopup(), viewModel);
            configure(config);
            afterConfigure(config.Options);
            return Task.CompletedTask;
        }
    }

    public class Content : IContextualizable
    {
        public void SetContext(object o)
        {
        }

        public object Object { get; }
    }

    public class TestPopup : IPopup
    {
        private TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

        public void SetContext(object o)
        {
        }

        public object Object { get; }
        public void Close()
        {
            tcs.SetResult(null);
        }

        public Task Show()
        {
            return tcs.Task;
        }

        public string Title { get; set; }
        public IObservable<Unit> Shown { get; }
    }



    public class WimPickRequirementSolver : RequirementSolver
    {
        public override IObservable<bool> IsValid => Observable.Return(false);
        public override IEnumerable<FulfilledRequirement> FulfilledRequirements()
        {
            throw new NotImplementedException();
        }
    }
}