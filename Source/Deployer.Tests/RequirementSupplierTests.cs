using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Deployer.Gui.Services;
using FluentAssertions;
using Grace.DependencyInjection;
using MediatR;
using Xunit;
using Zafiro.Core;
using Zafiro.Core.UI;

namespace Deployer.Tests
{
    public class RequirementSupplierTests
    {
        [Fact]
        public async Task Supply()
        {
            var sut = new RequirementSupplier(new Dictionary<RequirementKind, RequirementSolver>()
            {
                { RequirementKind.WimFile, new WimPickRequirementSolver()}
            }, new TestInteraction());

            var satisfy = await sut.Satisfy(new List<MissingRequirement>() {new MissingRequirement("wimFile", RequirementKind.WimFile)});
            satisfy.Should().BeEquivalentTo(new[]
            {
                new FulfilledRequirement("wimFilePath", RequirementKind.Disk, "path"),
                new FulfilledRequirement("wimFileIndex", RequirementKind.Disk, 1),
            });
        }
    }

    public class TestInteraction : ISimpleInteraction
    {
        public void Register(string token, Type viewType)
        {
            throw new NotImplementedException();
        }

        public Task Interact(string key, string title, object vm, ICollection<DialogButton> buttons)
        {
            throw new NotImplementedException();
        }
    }

    public class RequirementSolver
    {
    }

    class WimPickRequirementSolver : RequirementSolver
    {
    }

    public class WimRequirementHandler : IRequestHandler<WimRequest, WimResponse>
    {
        public Task<WimResponse> Handle(WimRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new WimResponse("c:\\file.wim", 1));
        }
    }

    public class WimRequest : IRequest<WimResponse>
    {
    }

    public class WimResponse
    {
        public string Path { get; }
        public int ImageIndex { get; }

        public WimResponse(string path, int imageIndex)
        {
            Path = path;
            ImageIndex = imageIndex;
        }
    }
}