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

namespace SQLLiteExtensions
{
    // Connection with a register

    public class SQLExtConnectionRegister: SQLExtConnection
    {
        public SQLExtRegister RegisterClass;

        public SQLExtConnectionRegister(string dbfile, bool utctimeindicator, AccessMode mode = AccessMode.ReaderWriter) : base(dbfile,utctimeindicator, mode)
        {
            RegisterClass = new SQLExtRegister(this);
        }

        // return true if created
        public bool CreateRegistry()
        {
            var tables = this.Tables();
            if (!tables.Contains("Register"))
            {
                ExecuteNonQuery("CREATE TABLE Register (ID TEXT PRIMARY KEY NOT NULL, ValueInt INTEGER, ValueDouble DOUBLE, ValueString TEXT, ValueBlob BLOB)");
                return true;
            }
            else
                return false;
        }
    }
}
