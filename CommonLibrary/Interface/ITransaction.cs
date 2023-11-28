using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Interface
{
    [ServiceContract]
    public interface ITransaction : IService
    {
        [OperationContract]
        Task<bool> Prepare();

        [OperationContract]
        Task<bool> Commit();

        [OperationContract]
        Task  Rollback();

    }
}
