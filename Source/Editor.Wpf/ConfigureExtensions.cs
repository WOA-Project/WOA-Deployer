using System;
using Deployer.Core;
using Grace.DependencyInjection;
using MediatR;

namespace Editor.Wpf
{
    public static class ConfigureExtensions
    {
        public static void ConfigureMediator(this IExportRegistrationBlock block)
        {
            block.ExportFactory<Func<Type, object>, Mediator>(locate => new Mediator(x => locate(x)))
                .As<ISender>()
                .As<IMediator>();	
            block.ExportAssembly(typeof(IAnchor).Assembly).BasedOn(typeof(IRequestHandler<,>)).ByInterfaces();
        }
    }
}