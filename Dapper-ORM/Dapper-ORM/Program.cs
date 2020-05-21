using System;

namespace Dapper_ORM
{
    class Program
    {
        static void Main(string[] args)
        {
            Repository repository = new Repository();

            #region Query

            // Obtiene todos customers
            var allCustomers = repository.ReadAllCustomers();
            Console.WriteLine(allCustomers.Count);

            // Obtiene todos los customers a través de un procedimiento almacenado
            var allCustomersSP = repository.ReadAllCustomersUsingSP();
            Console.WriteLine(allCustomersSP.Count);

            // Obtiene un customer por id
            var customer = repository.GetCustomerById(1);
            Console.WriteLine($"Id: {customer.Id} - {customer.FirstName} {customer.LastName}");

            // Obtiene un customer por id a través de un procedimiento almacenado
            var customerSP = repository.GetCustomerByIdUsingSP(1);
            Console.WriteLine($"Id: {customerSP.Id} - {customerSP.FirstName} {customerSP.LastName}");

            // (INNER 1-N) Añade orders a los customers
            var customersWithOrders = repository.GetCustomerWithOrders();
            Console.WriteLine(customersWithOrders.Count);

            // (LEFT 1-N) Añade orders a los customers en caso de que tengan
            var customersWithOrWithoutOrders = repository.GetCustomerWithOrWithoutOrders();
            Console.WriteLine(customersWithOrWithoutOrders.Count);

            // (Multiple-query 1-N) Añade orders a los customers
            var multipleQueryCustomers = repository.CustomerMultipleQuery(20);
            Console.WriteLine(multipleQueryCustomers.Orders.Count);

            // Consulta sobre más de 7 objetos
            repository.MoreThan7Objects();

            #endregion

            #region Execute

            // Inserta varios customers a la vez
            var affectedRows = repository.BulkInsertManyCustomers();
            Console.WriteLine(affectedRows);

            // Actualiza un customer.city a través de un procedimiento almacenado
            affectedRows = repository.UpdateCustomerUsingSP(customer);
            Console.WriteLine(affectedRows);

            #endregion

            Console.ReadKey();
        }
    }
}
