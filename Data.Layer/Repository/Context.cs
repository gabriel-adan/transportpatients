using Bussiness.Layer.Model;
using Logger.Layer.Log.Service;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Data.Layer.Repository
{
    public class Context
    {
        private static SQLiteConnection connection;
        public static bool IsConnected { get; private set; }

        public static bool OpenConnection(string connectionString)
        {
            try
            {
                connection = new SQLiteConnection(connectionString);
                connection.Open();
                IsConnected = true;
                return true;
            }
            catch (SQLiteException e)
            {
                LoggerManager.HandleException(e);
                IsConnected = false;
                return false;
            }
            catch (ArgumentException e)
            {
                LoggerManager.HandleException(e);
                IsConnected = false;
                return false;
            }
        }

        public static void CloseConnection()
        {
            if (connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
                IsConnected = false;
            }
        }

        public static IList<Driver> DriversList(DaysOfWeek dayOfWeek)
        {
            IList<Driver> resultList = new List<Driver>();
            try
            {
                using (SQLiteCommand command = new SQLiteCommand("SELECT Id, Name FROM Driver", connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Driver driver = new Driver() { Id = reader.GetInt16(0), Name = reader.GetString(1) };
                            using (SQLiteCommand cmd = new SQLiteCommand("SELECT c.Id, UPPER(c.Name) AS Name, h.Place, h.EntryTime, h.ExitTime, IFNULL(t.EntryDriverId, -1) AS EntryDriverId, IFNULL(t.ExitDriverId, -1) AS ExitDriverId, t.IsCanceled FROM Transport t INNER JOIN Customer c ON c.Id = t.CustomerId LEFT JOIN Hour h ON h.CustomerId = c.Id AND h.DayOfWeekId = t.DayOfWeekId AND h.EntryTime = t.EntryTime AND h.ExitTime = t.ExitTime WHERE h.DayOfWeekId = " + ((int)dayOfWeek) + " AND t.EntryDriverId = " + driver.Id + " UNION SELECT c.Id, UPPER(c.Name) AS Name, h.Place, h.EntryTime, h.ExitTime, IFNULL(t.EntryDriverId, -1) AS EntryDriverId, IFNULL(t.ExitDriverId, -1) AS ExitDriverId, t.IsCanceled FROM Transport t INNER JOIN Customer c ON c.Id = t.CustomerId LEFT JOIN Hour h ON h.CustomerId = c.Id AND h.DayOfWeekId = t.DayOfWeekId AND h.EntryTime = t.EntryTime AND h.ExitTime = t.ExitTime WHERE h.DayOfWeekId = " + ((int)dayOfWeek) + " AND t.ExitDriverId = " + driver.Id + " ORDER BY h.EntryTime;", connection))
                            {
                                using (SQLiteDataReader rd = cmd.ExecuteReader())
                                {
                                    while (rd.Read())
                                    {
                                        if (!rd.GetBoolean(7))
                                        {
                                            if (rd.GetInt16(5) == rd.GetInt16(6))
                                            {
                                                Transport transport = new Transport() { Customer = new Customer() { Id = rd.GetInt32(0), Name = rd.GetString(1), Hour = new Hour() { DayOfWeek = dayOfWeek, Place = rd.GetString(2), EntryTime = rd.GetString(3), ExitTime = rd.GetString(4) } }, IsCanceled = rd.GetBoolean(7), EntryDriver = driver, ExitDriver = driver };
                                                driver.Transports.Add(transport);
                                            }
                                            else
                                            {
                                                Transport transport = new Transport() { Customer = new Customer() { Id = rd.GetInt32(0), Name = rd.GetString(1), Hour = new Hour() { DayOfWeek = dayOfWeek, Place = rd.GetString(2), EntryTime = rd.GetString(3), ExitTime = rd.GetString(4) } }, IsCanceled = rd.GetBoolean(7) };
                                                if (rd.GetInt16(5) == driver.Id)
                                                {
                                                    transport.EntryDriver = driver;
                                                }
                                                else
                                                {
                                                    transport.EntryDriver = null;
                                                }
                                                if (rd.GetInt16(6) == driver.Id)
                                                {
                                                    transport.ExitDriver = driver;
                                                }
                                                else
                                                {
                                                    transport.ExitDriver = null;
                                                }
                                                driver.Transports.Add(transport);
                                            }
                                        }
                                    }
                                }
                            }
                            resultList.Add(driver);
                        }
                    }
                }
            }
            catch (SQLiteException e)
            {
                LoggerManager.HandleException(e);
            }
            catch (InvalidOperationException e)
            {
                LoggerManager.HandleException(e);
            }
            return resultList;
        }

        public static IList<Transport> GetCustomersTransportsByDayOfWeek(DaysOfWeek dayOfWeek)
        {
            IList<Transport> resultList = new List<Transport>();
            try
            {
                using (SQLiteCommand command = new SQLiteCommand("SELECT c.Id, UPPER(c.Name) as Name, h.Place, h.EntryTime, h.ExitTime, IFNULL(t.EntryDriverId, -1) AS EntryDriverId, IFNULL(t.ExitDriverId, -1) AS ExitDriverId, t.IsCanceled FROM Transport t INNER JOIN Customer c ON c.Id = t.CustomerId LEFT JOIN Hour h ON h.CustomerId = c.Id AND h.DayOfWeekId = t.DayOfWeekId AND h.EntryTime = t.EntryTime AND h.ExitTime = t.ExitTime WHERE h.DayOfWeekId = " + ((int)dayOfWeek) + " ORDER BY t.EntryTime;", connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Hour hour = new Hour() { Place = reader.GetString(2), EntryTime = reader.GetString(3), ExitTime = reader.GetString(4), DayOfWeek = dayOfWeek };
                            Customer customer = new Customer() { Id = reader.GetInt32(0), Name = reader.GetString(1), Hour = hour };
                            Transport transport = new Transport() { Customer = customer, IsCanceled = reader.GetBoolean(7), EntryDriver = null, ExitDriver = null };
                            int id = reader.GetInt32(5);
                            if (id > 0)
                            {
                                SQLiteCommand cmd = new SQLiteCommand("SELECT Id, UPPER(Name) as Name FROM Driver WHERE Id = " + id + ";", connection);
                                SQLiteDataReader dr = cmd.ExecuteReader();
                                if (dr.Read())
                                {
                                    Driver driver = new Driver() { Id = dr.GetInt32(0), Name = dr.GetString(1) };
                                    driver.Transports.Add(transport);
                                    transport.EntryDriver = driver;
                                }
                            }
                            id = reader.GetInt32(6);
                            if (id > 0)
                            {
                                SQLiteCommand cmd = new SQLiteCommand("SELECT Id, UPPER(Name) as Name FROM Driver WHERE Id = " + id + ";", connection);
                                SQLiteDataReader dr = cmd.ExecuteReader();
                                if (dr.Read())
                                {
                                    Driver driver = new Driver() { Id = dr.GetInt32(0), Name = dr.GetString(1) };
                                    driver.Transports.Add(transport);
                                    transport.ExitDriver = driver;
                                }
                            }
                            resultList.Add(transport);
                        }
                    }
                }
            }
            catch (SQLiteException e)
            {
                LoggerManager.HandleException(e);
            }
            catch (InvalidOperationException e)
            {
                LoggerManager.HandleException(e);
            }
            return resultList;
        }

        public static bool UpdateTransport(Transport transport)
        {
            if (transport == null) return false;
            string query = "UPDATE Transport SET EntryDriverId = " + ((transport.EntryDriver == null) ? "NULL" : transport.EntryDriver.Id.ToString()) + ", ExitDriverId = " + ((transport.ExitDriver == null) ? "NULL" : transport.ExitDriver.Id.ToString()) + ", IsCanceled = " + (transport.IsCanceled ? 1 : 0) + " WHERE CustomerId = " + transport.Customer.Id + " AND DayOfWeekId = " + ((int)transport.Customer.Hour.DayOfWeek) + " AND EntryTime = '" + transport.Customer.Hour.EntryTime + "' AND ExitTime = '" + transport.Customer.Hour.ExitTime + "';";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                try
                {
                    return command.ExecuteNonQuery() > 0;
                }
                catch (SQLiteException e)
                {
                    LoggerManager.HandleException(e);
                    return false;
                }
                catch (ArgumentNullException e)
                {
                    LoggerManager.HandleException(e);
                    return false;
                }
                catch (FormatException e)
                {
                    LoggerManager.HandleException(e);
                    return false;
                }
                catch (OverflowException e)
                {
                    LoggerManager.HandleException(e);
                    return false;
                }
            }
        }

        public static int AddCustomer(string name)
        {
            try
            {
                using (SQLiteCommand command = new SQLiteCommand("INSERT INTO Customer(Name) VALUES ('" + name + "');", connection))
                {
                    if (command.ExecuteNonQuery() > 0)
                    {
                        command.CommandText = "SELECT LAST_INSERT_ROWID() AS Id;";
                        return int.Parse(command.ExecuteScalar().ToString());
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (SQLiteException e)
            {
                LoggerManager.HandleException(e);
                return 0;
            }
        }

        public static int AddDriver(string name)
        {
            try
            {
                using (SQLiteCommand command = new SQLiteCommand("INSERT INTO Driver(Name) VALUES ('" + name + "');", connection))
                {
                    if (command.ExecuteNonQuery() > 0)
                    {
                        command.CommandText = "SELECT LAST_INSERT_ROWID() AS Id;";
                        return int.Parse(command.ExecuteScalar().ToString());
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (SQLiteException e)
            {
                LoggerManager.HandleException(e);
                return 0;
            }
        }

        public static int AddHour(Hour hour, int customerId)
        {
            try
            {
                using (SQLiteCommand command = new SQLiteCommand("INSERT INTO Hour(Place, EntryTime, ExitTime, DayOfWeekId, CustomerId) VALUES ('" + hour.Place + "', '" + hour.EntryTime + "', '" + hour.ExitTime + "', " + ((int)hour.DayOfWeek) + ", " + customerId + ");", connection))
                {
                    return command.ExecuteNonQuery();
                }
            }
            catch (SQLiteException e)
            {
                LoggerManager.HandleException(e);
                return 0;
            }
        }

        public static IList<Customer> GetCustomerList()
        {
            IList<Customer> list = new List<Customer>();
            try
            {
                using (SQLiteCommand command = new SQLiteCommand("SELECT c.Id, UPPER(c.Name) as Name, IFNULL(h.Place, '') AS Place, IFNULL(h.EntryTime, '') AS EntryTime, IFNULL(h.ExitTime, '') AS ExitTime, IFNULL(h.DayOfWeekId, 0) AS DayOfWeekId FROM Customer c LEFT JOIN Hour h ON c.Id = h.CustomerId WHERE h.DayOfWeekId > 0 OR h.DayOfWeekId IS NULL ORDER BY c.Name;", connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        int customerId = 0;
                        Customer customer = null;
                        while (reader.Read())
                        {
                            if (customerId != reader.GetInt32(0))
                            {
                                customerId = reader.GetInt32(0);
                                customer = new Customer() { Id = customerId, Name = reader.GetString(1) };
                                list.Add(customer);
                            }
                            if (!string.IsNullOrEmpty(reader.GetString(2)))
                            {
                                Hour hour = new Hour() { Place = reader.GetString(2), EntryTime = reader.GetString(3), ExitTime = reader.GetString(4) };
                                switch (reader.GetInt32(5))
                                {
                                    case 1:
                                        hour.DayOfWeek = DaysOfWeek.LUNES;
                                        break;
                                    case 2:
                                        hour.DayOfWeek = DaysOfWeek.MARTES;
                                        break;
                                    case 3:
                                        hour.DayOfWeek = DaysOfWeek.MIERCOLES;
                                        break;
                                    case 4:
                                        hour.DayOfWeek = DaysOfWeek.JUEVES;
                                        break;
                                    case 5:
                                        hour.DayOfWeek = DaysOfWeek.VIERNES;
                                        break;
                                }
                                customer.Hours.Add(hour);
                            }
                        }
                    }
                }
            }
            catch (SQLiteException e)
            {
                LoggerManager.HandleException(e);
            }
            catch (InvalidOperationException e)
            {
                LoggerManager.HandleException(e);
            }
            return list;
        }

        public static IList<Driver> DriversList()
        {
            IList<Driver> resultList = new List<Driver>();
            try
            {
                using (SQLiteCommand command = new SQLiteCommand("SELECT Id, UPPER(Name) as Name FROM Driver;", connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Driver driver = new Driver() { Id = reader.GetInt16(0), Name = reader.GetString(1) };
                            resultList.Add(driver);
                        }
                    }
                }
            }
            catch (SQLiteException e)
            {
                LoggerManager.HandleException(e);
            }
            catch (InvalidOperationException e)
            {
                LoggerManager.HandleException(e);
            }
            return resultList;
        }

        public static int UpdateHour(Hour oldHour, Hour newHour, int customerId)
        {
            if (oldHour == null || newHour == null)
                return 0;
            try
            {
                using (SQLiteCommand command = new SQLiteCommand("UPDATE Hour SET Place = '" + newHour.Place + "', EntryTime = '" + newHour.EntryTime + "', ExitTime = '" + newHour.ExitTime + "', DayOfWeekId = " + ((int)newHour.DayOfWeek) + " WHERE EntryTime = '" + oldHour.EntryTime + "' AND ExitTime = '" + oldHour.ExitTime + "' AND DayOfWeekId = " + ((int)oldHour.DayOfWeek) + " AND CustomerId = " + customerId + ";", connection))
                {
                    return command.ExecuteNonQuery();
                }
            }
            catch (SQLiteException e)
            {
                LoggerManager.HandleException(e);
                return 0;
            }
        }

        public static bool DeleteHour(Hour hour, int customerId)
        {
            if (hour == null)
                return false;
            try
            {
                using (SQLiteCommand command = new SQLiteCommand("DELETE FROM Hour WHERE EntryTime = '" + hour.EntryTime + "' AND ExitTime = '" + hour.ExitTime + "' AND DayOfWeekId = " + ((int)hour.DayOfWeek) + " AND CustomerId = " + customerId + ";", connection))
                {
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (SQLiteException e)
            {
                LoggerManager.HandleException(e);
                return false;
            }
        }

        public static bool DeleteCustomer(Customer customer)
        {
            if (customer == null)
                return false;
            try
            {
                using (SQLiteCommand command = new SQLiteCommand("DELETE FROM Customer WHERE Id = " + customer.Id + ";", connection))
                {
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (SQLiteException e)
            {
                LoggerManager.HandleException(e);
                return false;
            }
        }

        public static bool UpdateCustomer(Customer customer)
        {
            if (customer == null)
                return false;
            try
            {
                using (SQLiteCommand command = new SQLiteCommand("UPDATE Customer SET Name = '" + customer.Name + "' WHERE Id = " + customer.Id + ";", connection))
                {
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (SQLiteException e)
            {
                LoggerManager.HandleException(e);
                return false;
            }
        }

        public static bool UpdateDriver(Driver driver)
        {
            if (driver == null)
                return false;
            try
            {
                using (SQLiteCommand command = new SQLiteCommand("UPDATE Driver SET Name = '" + driver.Name + "' WHERE Id = " + driver.Id + ";", connection))
                {
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (SQLiteException e)
            {
                LoggerManager.HandleException(e);
                return false;
            }
        }

        public static bool DeleteDriver(Driver driver)
        {
            if (driver == null)
                return false;
            try
            {
                using (SQLiteCommand command = new SQLiteCommand("DELETE FROM Driver WHERE Id = " + driver.Id + ";", connection))
                {
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (SQLiteException e)
            {
                LoggerManager.HandleException(e);
                return false;
            }
        }
    }
}