using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TourismSmartTransportation.Data.Interfaces;

namespace TourismSmartTransportation.Data.EntityChangeNotifier
{
    public class EntityChangeNotifier<TEntity>
        : IDisposable
        where TEntity : class
    {
        private Expression<Func<TEntity, bool>> _query;
        private string _connectionString;
        private string _queryString;

        public EntityChangeNotifier(Expression<Func<TEntity, bool>> query)
        {
            _query = query;
            _queryString = "Select LicensePlates From dbo.[Vehicle]";
            _connectionString = "Server=tcp:se32.database.windows.net;Initial Catalog=tourism-smart-transportation;Persist Security Info=False;User ID=se32;Password=Password@32;MultipleActiveResultSets=True";
            // _connectionString = "Data Source=se32.database.windows.net;Initial Catalog=tourism-smart-transportation;User ID=se32;Password=Password@32;MultipleActiveResultSets=True";
            SqlDependency.Start(_connectionString);
            RegisterNotification();
        }

        private void RegisterNotification()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = GetCommand(_queryString, connection))
                {
                    connection.Open();


                    // var sqlDependency = new SqlDependency(command);
                    // sqlDependency.OnChange += new OnChangeEventHandler(_sqlDependency_OnChange);

                    // NOTE: You have to execute the command, or the notification will never fire.
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                    }
                }
            }
        }

        private SqlCommand GetCommand(string queryString, SqlConnection connection)
        {
            return new SqlCommand(queryString, connection);
        }

        private void _sqlDependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            Console.WriteLine("Info: {0}; Source: {1}; Type: {2}", e.Info, e.Source, e.Type);
            RegisterNotification();
        }

        public void Dispose()
        {
            SqlDependency.Stop(_connectionString);
        }
    }
}