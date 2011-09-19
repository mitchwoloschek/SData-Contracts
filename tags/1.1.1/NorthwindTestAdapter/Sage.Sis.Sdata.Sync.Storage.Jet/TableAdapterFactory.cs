﻿#region Usings

using System;
using System.Collections.Generic;
using System.Text;
using Sage.Sis.Common.Data.OleDb;
using Sage.Sis.Sdata.Sync.Context;
using Sage.Sis.Sdata.Sync.Storage.Jet.TableAdapters;
using Sage.Sis.Sdata.Sync.Storage.Jet.Tables;
using Sage.Sis.Sdata.Sync.Storage.Jet.Syndication;

#endregion

namespace Sage.Sis.Sdata.Sync.Storage.Jet
{
    internal class TableAdapterFactory
    {
        #region Class Variables

        private SdataContext _context;
        private IJetConnectionProvider _connProvider;

        #endregion

        #region Ctor.

        public TableAdapterFactory(SdataContext context, IJetConnectionProvider connProvider)
        {
            _context = context;
            _connProvider = connProvider;
        }

        #endregion

        internal IResourceKindTableAdapter CreateResourceKindTableAdapter()
        {
            IResourceKindTable resourceKindTable = new ResourceKindTable();
            using (IJetTransaction jetTransaction = _connProvider.GetTransaction(false))
            {
                if (!JetHelpers.TableExists(resourceKindTable.TableName, jetTransaction))
                {
                    resourceKindTable.CreateTable(jetTransaction);
                    jetTransaction.Commit();
                }
            }

            IResourceKindTableAdapter resourceKindTableAdapter = new ResourceKindTableAdapter(resourceKindTable, _context);

            return resourceKindTableAdapter;
            //CachedResourceKindTableAdapter cachedResourceKindTableAdapter = new CachedResourceKindTableAdapter(resourceKindTableAdapter);

            //using (IJetTransaction jetTransaction = _connProvider.GetTransaction(false))
            //{
            //    cachedResourceKindTableAdapter.Load(jetTransaction);
            //    jetTransaction.Commit();
            //}
            //return cachedResourceKindTableAdapter;
        }

        internal IEndPointTableAdapter CreateEndPointTableAdapter()
        {
            IEndPointTable EndPointTable = new EndPointTable();
            using (IJetTransaction jetTransaction = _connProvider.GetTransaction(false))
            {
                if (!JetHelpers.TableExists(EndPointTable.TableName, jetTransaction))
                {
                    EndPointTable.CreateTable(jetTransaction);
                    jetTransaction.Commit();
                }
            }

            IEndPointTableAdapter EndPointTableAdapter = new EndPointTableAdapter(EndPointTable, _context);

            using (IJetTransaction jetTransaction = _connProvider.GetTransaction(false))
            {
                EndPointTableAdapter.SetOriginEndPoint(_context.BaseUrl, jetTransaction);
                jetTransaction.Commit();

            }

            return EndPointTableAdapter;
            //CachedEndPointTableAdapter cachedEndPointTableAdapter = new CachedEndPointTableAdapter(EndPointTableAdapter);
            
            //using (IJetTransaction jetTransaction = _connProvider.GetTransaction(false))
            //{
            //    cachedEndPointTableAdapter.Load(jetTransaction);
            //    jetTransaction.Commit();
            //}

            //return cachedEndPointTableAdapter;
        }

        internal ISyncDigestTableAdapter CreateSyncDigestTableAdapter(IResourceKindTableAdapter resourceKindTableAdapter, IEndPointTableAdapter EndPointTableAdapter)
        {
            ISyncDigestTable syncDigestTable = new SyncDigestTable(resourceKindTableAdapter.Table, EndPointTableAdapter.Table);
            using (IJetTransaction jetTransaction = _connProvider.GetTransaction(false))
            {
                // create the table
                if (!JetHelpers.TableExists(syncDigestTable.TableName, jetTransaction))
                {
                    syncDigestTable.CreateTable(jetTransaction);
                    jetTransaction.Commit();
                }
                else
                {
                    // update the table
                    ITableFieldsUpdated tableFieldUpdater = syncDigestTable as ITableFieldsUpdated;
                    if (null != tableFieldUpdater)
                    {
                        tableFieldUpdater.UpdateFields(jetTransaction);
                        jetTransaction.Commit();
                    }
                }
            }

            return new SyncDigestTableAdapter(syncDigestTable, _context);
        }

        internal IAppBookmarkTableAdapter CreateAppBookmarkTableAdapter(IResourceKindTableAdapter resourceKindTableAdapter)
        {
            IAppBookmarkTable appBookmarkTable = new AppBookmarkTable(resourceKindTableAdapter.Table);
            using (IJetTransaction jetTransaction = _connProvider.GetTransaction(false))
            {
                if (!JetHelpers.TableExists(appBookmarkTable.TableName, jetTransaction))
                {
                    appBookmarkTable.CreateTable(jetTransaction);
                    jetTransaction.Commit();
                }
            }

            return new AppBookmarkTableAdapter(appBookmarkTable, _context);
        }

        internal ICorrelatedResSyncTableAdapter CreateCorrelatedResSyncTableAdapter(string resourceKind, IEndPointTableAdapter EndPointTableAdapter, IResourceKindTableAdapter resourceKindTableAdapter)
        {
            ResourceKindInfo resourceKindInfo = null;
            using (IJetTransaction jetTransaction = _connProvider.GetTransaction(false))
            {
                resourceKindInfo = resourceKindTableAdapter.GetOrCreate(resourceKind, jetTransaction);
                jetTransaction.Commit();
            }

            ICorrelatedResSyncTable correlatedResSyncTable = new CorrelatedResSyncTable(resourceKindInfo.Id, resourceKindTableAdapter.Table, EndPointTableAdapter.Table);
            using (IJetTransaction jetTransaction = _connProvider.GetTransaction(false))
            {
                if (!JetHelpers.TableExists(correlatedResSyncTable.TableName, jetTransaction))
                {
                    correlatedResSyncTable.CreateTable(jetTransaction);
                    jetTransaction.Commit();
                }
            }

            return new CorrelatedResSyncTableAdapter(correlatedResSyncTable,_context);
        }

        internal ItickTableAdapter CreatetickTableAdapter(IResourceKindTableAdapter resourceKindTableAdapter)
        {
            ItickTable tickTable = new tickTable(resourceKindTableAdapter.Table);
            using (IJetTransaction jetTransaction = _connProvider.GetTransaction(false))
            {
                if (!JetHelpers.TableExists(tickTable.TableName, jetTransaction))
                {
                    tickTable.CreateTable(jetTransaction);
                    jetTransaction.Commit();
                }
            }

            return new tickTableAdapter(tickTable, _context);
        }

        internal ISyncResultsTableAdapter CreateSyncResultsTableAdapter(string resourceKind, IResourceKindTableAdapter resourceKindTableAdapter)
        {
            ResourceKindInfo resourceKindInfo = null;
            using (IJetTransaction jetTransaction = _connProvider.GetTransaction(false))
            {
                resourceKindInfo = resourceKindTableAdapter.GetOrCreate(resourceKind, jetTransaction);
                jetTransaction.Commit();
            }

            ISyncResultsTable syncResultsTable = new SyncResultsTable(resourceKindInfo.Id, resourceKindTableAdapter.Table);
            using (IJetTransaction jetTransaction = _connProvider.GetTransaction(false))
            {
                if (!JetHelpers.TableExists(syncResultsTable.TableName, jetTransaction))
                {
                    syncResultsTable.CreateTable(jetTransaction);
                    jetTransaction.Commit();
                }
            }

            return new SyncResultsTableAdapter(syncResultsTable, _context);
        }
    }
}
