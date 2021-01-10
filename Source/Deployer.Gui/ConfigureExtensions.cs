using System;
using Deployer.Core;
using Deployer.Core.Requirements;
using Deployer.Core.Requirements.WimFile;
using Grace.DependencyInjection;
using MediatR;

namespace Deployer.Gui
{
    public static class ConfigureExtensions
    {
        public static void ConfigureMediator(this IExportRegistrationBlock block)
        {
            block.ExportFactory<Func<Type, object>, Mediator>(locate => new Mediator(x => locate(x)))
                .As<ISender>()
                .As<IMediator>();
            block.ExportAssembly(typeof(WimFileHandler).Assembly).BasedOn(typeof(IRequestHandler<,>)).ByInterfaces();
        }
    }
}