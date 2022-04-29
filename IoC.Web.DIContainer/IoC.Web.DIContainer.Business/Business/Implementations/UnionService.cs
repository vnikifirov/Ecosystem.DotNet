using System;
using IoC.Web.DIContainer.Business.Interfaces;
using Microsoft.Extensions.Configuration;

namespace IoC.Web.DIContainer.Business.Implementations
{
    public class UnionService : IUnionService
    {
        public readonly IConfiguration _config;
        public readonly ITransientService _transientService1;
        public readonly ITransientService _transientService2;
        public readonly IScopedService _scopedService1;
        public readonly IScopedService _scopedService2;
        public readonly ISingletonService _singletonService1;
        public readonly ISingletonService _singletonService2;
        public UnionService(ITransientService transientService1,
                ITransientService transientService2,
                IScopedService scopedService1,
                IScopedService scopedService2,
                ISingletonService singletonService1,
                ISingletonService singletonService2,
                IConfiguration configuration)
        {
            _transientService1 = transientService1;
            _transientService2 = transientService2;
            _scopedService1 = scopedService1;
            _scopedService2 = scopedService2;
            _singletonService1 = singletonService1;
            _singletonService2 = singletonService2;

            _config = configuration;
        }
    }
}
