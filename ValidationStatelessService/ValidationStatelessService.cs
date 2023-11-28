using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CommonLibrary.Interface;
using CommonLibrary.Model;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Newtonsoft.Json;

namespace ValidationStatelessService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class ValidationStatelessService : StatelessService, IValidation
    {
        public ValidationStatelessService(StatelessServiceContext context)
            : base(context)
        { }

        public async Task<List<string>> GetAllBooks()
        {
            ITransactionCoordinator proxy = ServiceProxy.Create<ITransactionCoordinator>(new Uri("fabric:/BookstoreTransaction/TransactionCoordinator"), new ServicePartitionKey(1));

            return await proxy.GetAllBooks();
        }

        public async Task<List<string>> GetAllClients()
        {
            ITransactionCoordinator proxy = ServiceProxy.Create<ITransactionCoordinator>(new Uri("fabric:/BookstoreTransaction/TransactionCoordinator"), new ServicePartitionKey(1));

            return await proxy.GetAllClients();
        }

        public async Task<string?> GetBook(long bookID)
        {
            ITransactionCoordinator proxy = ServiceProxy.Create<ITransactionCoordinator>(new Uri("fabric:/BookstoreTransaction/TransactionCoordinator"), new ServicePartitionKey(1));
            return await proxy.GetBook(bookID);
        }

        public async Task<string> GetValidClient(Customer user)
        {
            var isStringPropNull = user.GetType().GetProperties().Where(pi => pi.PropertyType == typeof(string)).Select(pi => (string)pi.GetValue(user)).Any(value => string.IsNullOrEmpty(value));
            var isNumPropNull = user.BankCardNumber.Equals(0) || user.BankCardNumber.Equals(null);

            //Fields are fine, check if the customer has a bank acc
            if (!isNumPropNull && !isNumPropNull)
            {
                ITransactionCoordinator proxy = ServiceProxy.Create<ITransactionCoordinator>(new Uri("fabric:/BookstoreTransaction/TransactionCoordinator"), new ServicePartitionKey(1));
                var convertedCustomer = JsonConvert.SerializeObject(user);

                //return bank client acc with money
                return await proxy.GetValidClient(convertedCustomer);
            }

            return null;
        }

        public async Task<bool> Validation(Book book)
        {
            var isStringPropNull = book.GetType().GetProperties().Where(pi => pi.PropertyType == typeof(string)).Select(pi => (string)pi.GetValue(book)).Any(value => string.IsNullOrEmpty(value));
            var isNumPropNull = book.Quantity.Equals(0) || book.Quantity.Equals(null);
            return !isNumPropNull && !isNumPropNull;
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            //return new ServiceInstanceListener[0];
            return this.CreateServiceRemotingInstanceListeners();
        }


        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.ServiceMessage(this.Context, $"Working-{iterations}, {this.Context.InstanceId}", ++iterations);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
