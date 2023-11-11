// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAnalysisPlugin.Domain.Entities.SimpleTimeTracker;
using DataAnalysisPlugin.Repositories.Dapper.Core;

namespace DataAnalysisPlugin.Repositories.Dapper
{
    public class SimpleTimeTrackerDbContext : DapperDbContext
    {
        public List<SimpleTimeTrackerCsvEntity> QueryAllCsv()
        {
            using (var con = CreateConnection())
            {
                con.Open();

                var sql = "SELECT * FROM Csv;";

                return con.Query<SimpleTimeTrackerCsvEntity>(sql).ToList();
            }
        }
    }
}
