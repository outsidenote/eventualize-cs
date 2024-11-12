﻿using EvDb.Adapters.Store.SqlServer;
using EvDb.Core;
using EvDb.StressTestsWebApi.Outbox;

namespace EvDb.StressTestsWebApi;


public static class StoreAdapterHelper
{
    public static IEvDbStorageAdapter CreateStoreAdapter(
        ILogger logger,
        StoreType storeType,
        EvDbTestStorageContext context)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

        string connectionKey = storeType switch
        {
            StoreType.SqlServer => "EvDbSqlServerConnection",
            StoreType.Posgres => "EvDbPosgresConnection",
            _ => throw new NotImplementedException()
        };


        string connectionString = configuration.GetConnectionString(connectionKey) ?? throw new ArgumentNullException(connectionKey);

        //IEvDbStorageAdapter result = storeType switch
        //{
        //    StoreType.SqlServer =>
        //        EvDbSqlServerStorageAdapterFactory.Create(logger, connectionString, context),
        //    //StoreType.Posgres => ,
        //    //    PosgresStorageAdapterFactory.Create(_logger, connectionString, context),
        //    _ => throw new NotImplementedException()
        //};
        //return result;
        throw new NotImplementedException();
    }

    public static IEvDbStorageMigration CreateStoreMigration(
        ILogger logger,
        StoreType storeType,
        EvDbTestStorageContext? context = null)
    {
        context = context ?? new EvDbTestStorageContext();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

        string connectionKey = storeType switch
        {
            StoreType.SqlServer => "EvDbSqlServerConnection",
            StoreType.Posgres => "EvDbPosgresConnection",
            _ => throw new NotImplementedException()
        };


        string connectionString = configuration.GetConnectionString(connectionKey) ?? throw new ArgumentNullException(connectionKey);

        IEvDbStorageMigration result = storeType switch
        {
            StoreType.SqlServer =>
                SqlServerStorageMigrationFactory.Create(logger, connectionString, context, OutboxShards.Table1, OutboxShards.Table2),
            //StoreType.Posgres => ,
            //    PosgresStorageAdapterFactory.Create(_logger, connectionString, context),
            _ => throw new NotImplementedException()
        };
        return result;
    }

}
