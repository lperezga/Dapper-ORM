using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Dapper_ORM
{
    public class Repository
    {
        const string database = "data source=SVQ-BGYGZM2;Initial Catalog = example_database; Integrated Security = True;";

        public List<Models.Customer> ReadAllCustomers()
        {
            using (IDbConnection db = new SqlConnection(database))
            {
                return db.Query<Models.Customer>("SELECT * FROM Customer").ToList();
            }
        }

        public List<Models.Customer> ReadAllCustomersUsingSP()
        {
            using (IDbConnection db = new SqlConnection(database))
            {
                return db.Query<Models.Customer>("GetAllCustomers").ToList();
            }
        }

        public Models.Customer GetCustomerById(int id)
        {
            using (IDbConnection db = new SqlConnection(database))
            {
                return db.QuerySingleOrDefault<Models.Customer>("SELECT * FROM Customer WHERE Id = @Id", new { Id = id });
                //return db.Query<Models.Customer>("SELECT * FROM Customer WHERE Id = @Id", new { Id = id }).SingleOrDefault();
            }
        }

        public Models.Customer GetCustomerByIdUsingSP(int id)
        {
            using (IDbConnection db = new SqlConnection(database))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                return db.QueryFirst<Models.Customer>("GetCustomerById", param: parameters, commandType: CommandType.StoredProcedure);
                //return db.Query<Models.Customer>("GetCustomerById", param: parameters, commandType: CommandType.StoredProcedure).First();
            }
        }

        public int UpdateCustomerUsingSP(Models.Customer customer)
        {
            using (IDbConnection db = new SqlConnection(database))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", customer.Id);
                parameters.Add("@City", customer.City);
                parameters.Add("@NumRowsAffected", dbType: DbType.Int32, direction: ParameterDirection.Output); // Obligatorio declarar tipo y direction

                db.Execute("UpdateCustomerNameById", parameters, commandType: CommandType.StoredProcedure);

                return parameters.Get<int>("@NumRowsAffected");
            }
        }

        public int BulkInsertManyCustomers()
        {
            using (var connection = new SqlConnection(database))
            {
                string sql = "INSERT INTO Customer (FirstName,LastName,City,Country,Phone) values (@firstName, @lastName, @city, @country, @phone);";

                return connection.Execute(sql,
                    new[]{
                        new { firstName = "Dapper", lastName = "Test", city = "Sevilla", country = "Spain", phone = "987654321" },
                        new { firstName = "Dapper 2", lastName = "Test 2", city = "Sevilla", country = "Spain", phone = "987654321" }
                        }
                );
            }
        }

        public List<Models.Customer> GetCustomerWithOrders()
        {
            var query = @"select * from Customer
                            INNER JOIN [Order] o ON
                            o.CustomerId = Customer.Id";

            using (IDbConnection db = new SqlConnection(database))
            {
                var customerDictionary = new Dictionary<int, Models.Customer>();

                return db.Query<Models.Customer, Models.Order, Models.Customer>(query, (customer, order) =>
                {
                    Models.Customer result;

                    if (!customerDictionary.TryGetValue(customer.Id, out result))
                    {
                        result = customer;
                        result.Orders = new List<Models.Order>();
                        customerDictionary.Add(result.Id, result);
                    }

                    result.Orders.Add(order);
                    return result;
                }, splitOn: "Id").ToList();
            }
        }

        public List<Models.Customer> GetCustomerWithOrWithoutOrders()
        {
            var query = @"select Customer.Id as Id, 
                            FirstName,
                            LastName,
                            City,
                            Country,
                            Phone,
                            ''[split],
                            o.Id as Id,
                            OrderDate,
                            OrderNumber,
                            TotalAmount
                            from Customer
                            LEFT JOIN [Order] o ON
                            o.CustomerId = Customer.Id";

            using (IDbConnection db = new SqlConnection(database))
            {
                var customerDictionary = new Dictionary<int, Models.Customer>();

                return db.Query<Models.Customer, Models.Order, Models.Customer>(query, (customer, order) =>
                {
                    Models.Customer result;

                    if (!customerDictionary.TryGetValue(customer.Id, out result))
                    {
                        result = customer;
                        result.Orders = new List<Models.Order>();
                        customerDictionary.Add(result.Id, result);
                    }

                    if (order.Id != 0)
                    {
                        result.Orders.Add(order);
                    }

                    return result;
                }, splitOn: "split").ToList();
            }
        }

        public Models.Customer CustomerMultipleQuery(int id)
        {
            var query = "SELECT * FROM Customer WHERE Id = @Id; SELECT * FROM [Order] WHERE CustomerId = @Id";

            using (IDbConnection db = new SqlConnection(database))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                var results = db.QueryMultiple(query, parameters);
                var customers = results.ReadSingle<Models.Customer>();
                customers.Orders = results.Read<Models.Order>().ToList();

                return customers;
            }
        }

        public void MoreThan7Objects()
        {
            var query = "SELECT * FROM Customer,[Order]";

            using (IDbConnection db = new SqlConnection(database))
            {
                db.Query(query, new[] { typeof(Models.Customer), typeof(Models.Order) }, obj =>
                {
                    var customer = obj[0] as Models.Customer;
                    var order = obj[1] as Models.Order;
                    return customer;
                }, splitOn: "Id");
            }
        }
    }
}