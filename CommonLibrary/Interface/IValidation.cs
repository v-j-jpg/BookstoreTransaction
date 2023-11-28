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
    public interface IValidation : IService
    {
        [OperationContract]
        Task<bool> Validation(Book book);

        [OperationContract]
        Task<List<string>> GetAllBooks();

        [OperationContract]
        Task<string?> GetBook(long bookID);

        [OperationContract]
        Task<List<string>> GetAllClients();

        [OperationContract]
        Task<string> GetValidClient(Customer customer);

    }
}
