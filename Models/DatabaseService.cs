using Dealership.Models;
using Npgsql;

namespace Dealership.Models
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // ПОЛУЧИТЬ пользователя по коду
        public async Task<User?> GetUserByDealerCodeAsync(string dealerCode)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(
                "SELECT * FROM users WHERE dealer_code = @dealerCode",
                connection);

            command.Parameters.AddWithValue("@dealerCode", dealerCode);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    DealerCode = reader.GetString(reader.GetOrdinal("dealer_code")),
                    FullName = reader.GetString(reader.GetOrdinal("full_name")),
                    PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
                    IsAdmin = reader.GetBoolean(reader.GetOrdinal("is_admin"))
                };
            }

            return null;
        }

        // СОЗДАТЬ нового пользователя
        public async Task<int> CreateUserAsync(User user)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(
                @"INSERT INTO users (dealer_code, full_name, password_hash, is_admin) 
                  VALUES (@dealerCode, @fullName, @passwordHash, @isAdmin) 
                  RETURNING id",
                connection);

            command.Parameters.AddWithValue("@dealerCode", user.DealerCode);
            command.Parameters.AddWithValue("@fullName", user.FullName);
            command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@isAdmin", user.IsAdmin);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        // ДОБАВИТЬ данные продаж (из dealer_account.html)
        public async Task AddSaleDataAsync(string dealerCode, DateTime month,
                                          int brandId, int modelId,
                                          int carsSold, int cancelledSales, int carsDelivered)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(
                @"INSERT INTO dealer_data 
                  (dealer_code, month, brand_id, model_id, cars_sold, cancelled_sales, cars_delivered) 
                  VALUES (@dealerCode, @month, @brandId, @modelId, @carsSold, @cancelledSales, @carsDelivered)",
                connection);

            command.Parameters.AddWithValue("@dealerCode", dealerCode);
            command.Parameters.AddWithValue("@month", month);
            command.Parameters.AddWithValue("@brandId", brandId);
            command.Parameters.AddWithValue("@modelId", modelId);
            command.Parameters.AddWithValue("@carsSold", carsSold);
            command.Parameters.AddWithValue("@cancelledSales", cancelledSales);
            command.Parameters.AddWithValue("@carsDelivered", carsDelivered);

            await command.ExecuteNonQueryAsync();
        }

        // ПОЛУЧИТЬ данные продаж для дилера
        public async Task<List<SaleData>> GetSalesForDealerAsync(string dealerCode)
        {
            var sales = new List<SaleData>();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(
                @"SELECT dd.*, cb.name as brand_name, cm.name as model_name 
                  FROM dealer_data dd
                  LEFT JOIN car_brands cb ON dd.brand_id = cb.id
                  LEFT JOIN car_models cm ON dd.model_id = cm.id
                  WHERE dd.dealer_code = @dealerCode
                  ORDER BY dd.month DESC",
                connection);

            command.Parameters.AddWithValue("@dealerCode", dealerCode);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                sales.Add(new SaleData
                {
                    Month = reader.GetDateTime(reader.GetOrdinal("month")),
                    BrandName = reader.IsDBNull(reader.GetOrdinal("brand_name"))
                        ? "" : reader.GetString(reader.GetOrdinal("brand_name")),
                    ModelName = reader.IsDBNull(reader.GetOrdinal("model_name"))
                        ? "" : reader.GetString(reader.GetOrdinal("model_name")),
                    CarsSold = reader.GetInt32(reader.GetOrdinal("cars_sold")),
                    CancelledSales = reader.GetInt32(reader.GetOrdinal("cancelled_sales")),
                    CarsDelivered = reader.GetInt32(reader.GetOrdinal("cars_delivered"))
                });
            }

            return sales;
        }
    }

    public class SaleData
    {
        public DateTime Month { get; set; }
        public string BrandName { get; set; } = "";
        public string ModelName { get; set; } = "";
        public int CarsSold { get; set; }
        public int CancelledSales { get; set; }
        public int CarsDelivered { get; set; }
    }
}

