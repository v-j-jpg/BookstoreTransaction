using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommonLibrary.Interface;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using CommonLibrary.Model;
using Newtonsoft.Json;
using CommonLibrary;
using System.Net;

namespace BankService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class BankService : StatefulService, IBank
    {
        public IReliableDictionary<long, Customer>? customerDictionary;

        public BankService(StatefulServiceContext context)
            : base(context)
        { }

        public void EnlistMoneyTransfer(string userID, double amount)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> isClientValid(string user)
        {
            //Customer customer = JsonConvert.DeserializeObject<Customer>(user);

            //try
            //{
            //    using (var transaction = StateManager.CreateTransaction())
            //    {
            //        var client = await customerDictionary!.TryGetValueAsync(transaction, customer!.BankCardNumber);

            //        return true;
            //    }
            //}
            //catch (Exception ex)
            //{

            //    throw new NotImplementedException(ex.Message);
            //}
            return true;
        }

        public async Task<List<string>> ListClients()
        {
            var a = "cccccccccc";

            return null;

            //customerDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<long, Customer>>("customerDictionary");
            //AddClientsToBank();

            //var clients = new List<string>();

            //using (var tx = StateManager.CreateTransaction())
            //{
            //    var enumerator = (await customerDictionary.CreateEnumerableAsync(tx)).GetAsyncEnumerator();

            //    while (await enumerator.MoveNextAsync(CancellationToken.None))
            //    {
            //        var book = enumerator.Current.Value;
            //        clients.Add(JsonConvert.SerializeObject(book));
            //    }
            //}

            //return clients;
        }

        private async void AddClientsToBank()
        {
            //CustomerDatabase database = new CustomerDatabase();
            //    try
            //    {
            //        // Create a new Transaction object for this partition
            //        using (var tx = StateManager.CreateTransaction())
            //        {
            //            // AddAsync takes key's write lock; if >4 secs, TimeoutException
            //            // Key & value put in temp dictionary (read your own writes),
            //            // serialized, redo/undo record is logged & sent to secondary replicas
            //            foreach (Customer customer in database.Customers)
            //                await customerDictionary!.AddAsync(tx, customer.BankCardNumber, customer);

            //            // CommitAsync sends Commit record to log & secondary replicas
            //            // After quorum responds, all locks released
            //            await tx.CommitAsync();
            //        }
            //    }
            //    catch (TimeoutException ex)
            //    {
            //        //file in use/locked, delay the transaction 
            //        await Task.Delay(100);
            //    }
            
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

       public Task<bool> Prepare()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Commit()
        {
            throw new NotImplementedException();
        }

        public Task Rollback()
        {
            throw new NotImplementedException();
        }
    }
}
