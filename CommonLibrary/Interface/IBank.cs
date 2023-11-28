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
    public interface IBank : IService
    {
        [OperationContract]
        Task<List<string>> ListClients();

        [OperationContract]
        Task EnlistMoneyTransfer(long userID, double amount);

        [OperationContract]
        Task<string> GetValidClient(string user);

    }
}
