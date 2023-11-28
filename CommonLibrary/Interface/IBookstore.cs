using CommonLibrary.Model;
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
    public interface IBookstore : ITransaction
    {

        [OperationContract]
        Task<List<string>> ListAvailableItems();

        [OperationContract]
        Task EnlistPurchase(long bookID, uint count);

        [OperationContract]
        Task <double> GetItemPrice(long bookID);

        [OperationContract]
        Task<string?> GetBook(long bookID);
    }
}
