using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Health;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CommonLibrary;
using CommonLibrary.Interface;
using CommonLibrary.Model;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Newtonsoft.Json;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookstoreService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class BookstoreService : StatefulService, IBookstore
    {
        public IReliableDictionary<long, Book>? bookDictionary;
        private long bookID;
        private uint count;

        public BookstoreService(StatefulServiceContext context)
            : base(context)
        { }

        public async Task EnlistPurchase(long bookID, uint count)
        {
            this.bookID = bookID;
            this.count = count;
        }

        public Task<double> GetItemPrice(string bookID)
        {
            throw new NotImplementedException();
        }

        public async Task<List<string>> ListAvailableItems()
        {
            bookDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<long, Book>>("bookDictionary");
            AddBooksToBookstore();

            var books = new List<string>();

            using (var transaction = StateManager.CreateTransaction())
            {
                var enumerator = (await bookDictionary.CreateEnumerableAsync(transaction)).GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    var book = enumerator.Current.Value;
                    books.Add(JsonConvert.SerializeObject(book));
                }
            }

            return books;
        }
        public async void AddBooksToBookstore()
        {
            BookDatabase database = new BookDatabase();

            try
            {
                // Create a new Transaction object for this partition
                using (var tx = StateManager.CreateTransaction())
                {
                    // AddAsync takes key's write lock; if >4 secs, TimeoutException
                    // Key & value put in temp dictionary (read your own writes),
                    // serialized, redo/undo record is logged & sent to secondary replicas
                    foreach (Book book in database.Books)
                        await bookDictionary!.AddAsync(tx, book.BookID, book);

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

        public Task<double> GetItemPrice(long bookID)
        {
            throw new NotImplementedException();
        }

        public async Task<string?> GetBook(long bookID)
        {
            try
            {
                using (var transaction = StateManager.CreateTransaction())
                {
                    var book = await bookDictionary!.TryGetValueAsync(transaction, bookID!);
                   
                    return JsonConvert.SerializeObject(book!.Value);
                }
            } catch (Exception ex) { 
            
                throw new NotImplementedException(ex.Message);
            }
           
        }

        public async Task<bool> Prepare()
        {
            var books = await StateManager.GetOrAddAsync<IReliableDictionary<long, Book>>("Books");
            var temp = await StateManager.GetOrAddAsync<IReliableDictionary<long, Book>>("Books");

            using (var tx = StateManager.CreateTransaction())
            {
                Book item = (await books.TryGetValueAsync(tx, bookID)).Value;
                
                if (item != null && item.Quantity >= count)
                {
                    await temp.TryRemoveAsync(tx, 1);
                    await temp.AddAsync(tx, 1, item);
                    await tx.CommitAsync();
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> Commit()
        {
            var books = await StateManager.GetOrAddAsync<IReliableDictionary<long, Book>>("Books");

            using (var tx = StateManager.CreateTransaction())
            {
                Book book = (await books.TryGetValueAsync(tx, bookID)).Value;
                book.Quantity = book.Quantity - count;
                await tx.CommitAsync();
            }
            return true;
        }

        public async Task<bool> Rollback()
        {
            var books = await StateManager.GetOrAddAsync<IReliableDictionary<long, Book>>("Books");
            var temp = await StateManager.GetOrAddAsync<IReliableDictionary<long, Book>>("Books2");

            using (var tx = StateManager.CreateTransaction())
            {
                Book prep = (await temp.TryGetValueAsync(tx, 1)).Value;
                Book book = (await books.TryGetValueAsync(tx, prep.BookID)).Value;
                book.Quantity = prep.Quantity;
                await tx.CommitAsync();
            }

            return true;
        }
    }
}
