using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Data;
using CommonLibrary.Interface;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using CommonLibrary.Model;
using System.Text.Json.Nodes;
using Newtonsoft.Json;
using CommonLibrary;
using System.Transactions;
using Microsoft.ServiceFabric.Services.Client;
using System.Net;

namespace TransactionCoordinator
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class TransactionCoordinator : StatefulService, ITransactionCoordinator
    {

        public TransactionCoordinator(StatefulServiceContext context)
            : base(context)
        { }

        public async Task<List<string>> GetAllBooks()
        {
            IBookstore proxy = ServiceProxy.Create<IBookstore>(new Uri("fabric:/BookstoreTransaction/BookstoreService"), new ServicePartitionKey(1));

            return await proxy.ListAvailableItems();
        }

        public async Task<List<string>> GetAllClients()
        {
            IBank proxy = ServiceProxy.Create<IBank>(new Uri("fabric:/BookstoreTransaction/Bank"), new ServicePartitionKey(1));
            return await proxy.ListClients();
        }

        public async Task<string?> GetBook(long bookID)
        {
            IBookstore proxy = ServiceProxy.Create<IBookstore>(new Uri("fabric:/BookstoreTransaction/BookstoreService"), new ServicePartitionKey(1));
            return await proxy.GetBook(bookID);
        }

        public async Task<bool> isClientValid(string client)
        {
            IBank proxy = ServiceProxy.Create<IBank>(new Uri("fabric:/BookstoreTransaction/Bank"), new ServicePartitionKey(1));
            return await proxy.isClientValid(client);
        }


        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
