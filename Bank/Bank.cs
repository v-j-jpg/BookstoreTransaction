using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using CommonLibrary;
using CommonLibrary.Interface;
using CommonLibrary.Model;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Newtonsoft.Json;

namespace Bank
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class Bank : StatefulService, IBank
    {
        public IReliableDictionary<long, Customer>? customerDictionary;
        private long clientID = 0;
        private double amount = 0;

        public Bank(StatefulServiceContext context)
            : base(context)
        {
            //AddClientsToBank();
        }

        public async Task<bool> Commit()
        {
            var customerDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<long, Customer>>("customerDictionary");

            using (var tx = StateManager.CreateTransaction())
            {
                //Customer client = (await clients.TryGetValueAsync(tx, clientID)).Value;
                var client = await customerDictionary!.TryGetValueAsync(tx, clientID);

                //(Customer)client = client!.Money - amount;
               
                await tx.CommitAsync();
            }
            return true;
        }

        public async Task EnlistMoneyTransfer(long clientID, double amount)
        {
            this.clientID = clientID;
            this.amount = amount;
        }

        public async Task<string> GetValidClient(string user)
        {
            //customer form info
            Customer? customer = JsonConvert.DeserializeObject<Customer>(user);

            customerDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<long, Customer>>("customerDictionary");

            try
            {
                using (var tx = StateManager.CreateTransaction())
                {
                    var client = await customerDictionary!.TryGetValueAsync(tx, customer!.BankCardNumber);

                    //if client has an acc in the bank, the object will be returned
                    return JsonConvert.SerializeObject(client.Value);
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }

        public async Task<List<string>> ListClients()
        {
            var clients = new List<string>();
            customerDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<long, Customer>>("customerDictionary");

            using (var tx = StateManager.CreateTransaction())
            {
                var enumerator = (await customerDictionary.CreateEnumerableAsync(tx)).GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    var customer = enumerator.Current.Value;
                    clients.Add(JsonConvert.SerializeObject(customer));
                }
            }

            return clients;
        }
        public async Task AddClientsToBank()
        {
            CustomerDatabase database = new CustomerDatabase();
            customerDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<long, Customer>>("customerDictionary");

            try
            {
                // Create a new Transaction object for this partition
                using (var tx = StateManager.CreateTransaction())
                {
                    // AddAsync takes key's write lock; if >4 secs, TimeoutException
                    // Key & value put in temp dictionary (read your own writes),
                    // serialized, redo/undo record is logged & sent to secondary replicas
                    foreach (Customer customer in database.Customers)
                        await customerDictionary!.AddOrUpdateAsync(tx, customer.BankCardNumber!, customer, (k, v) => v);

                    // CommitAsync sends Commit record to log & secondary replicas
                    // After quorum responds, all locks released
                    await tx.CommitAsync();
                }
            }
            catch (TimeoutException ex)
            {
                //file in use/locked, delay the transaction 
                await Task.Delay(100);
            }

        }
        public async Task<bool> Prepare()
        {
            customerDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<long, Customer>>("customerDictionary");
            //var temp = await StateManager.GetOrAddAsync<IReliableDictionary<long, Customer>>("customerDictionary");

            using (var tx = StateManager.CreateTransaction())
            {
                Customer client = (await customerDictionary.TryGetValueAsync(tx, clientID)).Value;

                if (client != null && client.Money >= amount)
                {
                    //await temp.TryRemoveAsync(tx, item.BankCardNumber);
                    client.Money-=amount;
                    await customerDictionary.AddOrUpdateAsync(tx, clientID, client!, (k, v) => v);
                    //await tx.CommitAsync();
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> Rollback()
        {
            var clients = await StateManager.GetOrAddAsync<IReliableDictionary<long, Customer>>("customerDictionary");

            using (var tx = StateManager.CreateTransaction())
            {
                Customer account = (await clients.TryGetValueAsync(tx, clientID)).Value;
                account.Money += amount;
                await tx.CommitAsync();
            }
            return true;
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
            await AddClientsToBank();

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
