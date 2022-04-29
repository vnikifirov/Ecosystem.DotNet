using System;
using IoC.Web.DIContainer.Business.Interfaces;

namespace IoC.Web.DIContainer.Business.Implementations
{
    public class OperationService : ITransientService,
                                    IScopedService,
                                    ISingletonService
    {
        private Guid id;
        public OperationService()
        {
            id = Guid.NewGuid();
        }
        public Guid OperationID
        {
            get => id;
        }
    }
}
