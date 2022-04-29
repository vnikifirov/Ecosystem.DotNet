using System;
namespace IoC.Web.DIContainer.Business.Interfaces
{
    public interface ITransientService
    {
        Guid OperationID { get; }
    }
}
