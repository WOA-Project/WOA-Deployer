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
using Zafiro.UI;
using Option = Zafiro.UI.Option;
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
                new FulfilledRequirement("wimFilePath",  "path"),
                new FulfilledRequirement("wimFileIndex", 1),
            };

            var testShell = new TestPopup(options => options[0].Command.Execute(null));
            var sut = new RequirementSupplier(settings => new TestRequirementSolver(requirements), testShell, str => null);

            var missingRequirements = new List<MissingRequirement>()
            {
                new MissingRequirement("wimFile", RequirementKind.WimFile, "")
            };

            var satisfyResult = await sut.Satisfy(missingRequirements);
           
            var expected = Either.Success<Error, IEnumerable<FulfilledRequirement>>(requirements);

            satisfyResult.Should().BeEquivalentTo(expected);
        }
    }

    public class TestRequirementSolver : IRequirementSolver
    {
        private IEnumerable<FulfilledRequirement> list;

        public TestRequirementSolver(IEnumerable<FulfilledRequirement> list)
        {
            this.list = list;
        }

        public virtual IObservable<bool> IsValid => Observable.Return(true);

        public virtual IEnumerable<FulfilledRequirement> FulfilledRequirements()
        {
            return list;
        }
    }

    public class TestPopup: IPopup
    {
        private readonly Action<Collection<Option>> afterConfigure;

        public TestPopup(Action<Collection<Option>> afterConfigure)
        {
            this.afterConfigure = afterConfigure;
        }

        public Task ShowAsModal<T>(IHaveDataContext content, T viewModel, Action<ViewConfiguration<T>> configure)
        {
            var config = new ViewConfiguration<T>(new FakeView(), viewModel);
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

    public class FakeView : IView
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

        public Task ShowAsModal()
        {
            return tcs.Task;
        }

        public string Title { get; set; }
        public IObservable<Unit> Shown { get; }
    }
}