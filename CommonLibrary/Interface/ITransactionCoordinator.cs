﻿using CommonLibrary.Model;
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
    public interface ITransactionCoordinator : IService
    {
        [OperationContract]
        Task<List<string>> GetAllBooks();

        [OperationContract]
        Task<List<string>> GetAllClients();

        [OperationContract]
        Task<string?> GetBook(long bookID);

        [OperationContract]
        Task<string> GetValidClient(string client);

        [OperationContract]
        Task<bool> Prepare(long bookID, long clientID, uint count);

        [OperationContract]
        Task<bool> Commit();

        [OperationContract]
        Task<bool> Rollback();
    }
}
