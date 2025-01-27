﻿/*
 * Copyright © 2019-2021 EDDiscovery development team
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 * 
 * EDDiscovery is not affiliated with Frontier Developments plc.
 */

using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Threading;

namespace SQLLiteExtensions
{
    // A connection

    public class SQLExtConnection : IDisposable             
    {
        public enum AccessMode { Reader, Writer, ReaderWriter };
        public string DBFile { get; private set; }    // the File name

        protected SQLExtConnection(string dbfile, bool utctimeindicator, AccessMode mode = AccessMode.ReaderWriter )
        {
            try
            {
                DBFile = dbfile;
                connection = SQLDbProvider.DbProvider().CreateConnection();

                // Use the database selected by maindb as the 'main' database
                connection.ConnectionString = "Data Source=" + DBFile.Replace("\\", "\\\\") + ";Pooling=true;";

                if (utctimeindicator)   // indicate treat dates as UTC.
                    connection.ConnectionString += "DateTimeKind=Utc;";

                if (mode == AccessMode.Reader)
                {
                    connection.ConnectionString += "Read Only=True;";
                }

                System.Diagnostics.Debug.WriteLine($"SQLExtConnection created connection {connection.ConnectionString} on {Thread.CurrentThread.Name}");

                connection.Open();

                owningThread = Thread.CurrentThread;
            }
            catch
            {
                throw;
            }
        }

        public virtual DbCommand CreateCommand(string query, DbTransaction tn = null)
        {
            AssertThreadOwner();
            DbCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 30;
            cmd.CommandText = query;
            return cmd;
        }

        public virtual DbTransaction BeginTransaction()
        {
            AssertThreadOwner();
            return connection.BeginTransaction();
        }

        public virtual DbTransaction BeginTransaction(IsolationLevel isolevel)
        {
            AssertThreadOwner();
            return connection.BeginTransaction(isolevel);
        }
        public virtual void Dispose()
        {
            Dispose(true);
        }
        
        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (connection != null)
                {
                    System.Diagnostics.Debug.WriteLine($"SQLExtConnection closed connection {connection.ConnectionString} on {Thread.CurrentThread.Name}");
                    connection.Close();
                    connection.Dispose();       // note, SQL pools connections, so it may still be physically open. Use System.Data.SQLite.SQLiteConnection.ClearAllPools() to force dropping all connections
                    connection = null;
#if DEBUG
                    hasbeendisposed = true;
#endif
                }
            }
        }

        // Query operators

        public void ExecuteNonQueries(params string[] queries)
        {
            foreach (var query in queries)
            {
                ExecuteNonQuery(query);
            }
        }

        public void ExecuteNonQuery(string query)
        {
            using (DbCommand command = CreateCommand(query))
                command.ExecuteNonQuery();
        }

        public Type GetConnectionType()
        {
            return connection.GetType();
        }

#if DEBUG
        //since finalisers impose a penalty, we shall check only in debug mode
        private bool hasbeendisposed = false;
        ~SQLExtConnection()
        {
            if (!hasbeendisposed)       // finalisation may come very late.. not immediately as its done on garbage collection.  Warn by message and assert.
            {
                System.Windows.Forms.MessageBox.Show("SQLConnection missing dispose for connection " + DBFile);
                System.Diagnostics.Debug.Assert(hasbeendisposed, "Missing dispose for connection" + DBFile);       // must have been disposed
            }
        }
#endif

        protected void AssertThreadOwner()
        {
            if (Thread.CurrentThread != owningThread)
            {
                throw new InvalidOperationException($"DB connection was passed between threads.  Owning thread: {owningThread.Name} ({owningThread.ManagedThreadId}); this thread: {Thread.CurrentThread.Name} ({Thread.CurrentThread.ManagedThreadId})");
            }
        }

        protected DbConnection connection;      // the connection, available to inheritors

        private Thread owningThread;          // tracing who owns the thread to prevent cross thread ops
    }
}
