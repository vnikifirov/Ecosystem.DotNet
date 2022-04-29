using System;
namespace IoC.Web.DIContainer.Business.Interfaces
{
    public interface IScopedService
    {
        Guid OperationID { get; }
    }
}
