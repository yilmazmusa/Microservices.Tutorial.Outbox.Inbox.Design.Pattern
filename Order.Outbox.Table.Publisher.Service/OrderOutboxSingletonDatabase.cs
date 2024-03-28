using Dapper;
using System.Data.SqlClient;
using System.Data;
using System.Collections;

namespace Order.Outbox.Table.Publisher.Service
{
    public static class OrderOutboxSingletonDatabase
    {
        static IDbConnection _dbConnection; //Veritabanı işlemleri yapabilmemiz için gerekli.
        static bool _dataReaderState = true; //Veritabanı işlemlerinin uygun olup olmadığını kontrol edicek field.Yani her 5 saniyede bir işlem yapılacakya her işlemin başlaması için önce veritabanıhazır mı kontrol etmemiz gerekiyor. İlk başta true dememizin sebebi ilk başta işlemler başlayabilsin diye.


        static OrderOutboxSingletonDatabase()
              => _dbConnection = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=OrderDB;Trusted_Connection=True;TrustServerCertificate=True");

        public static IDbConnection Connection
        {

            get
            {
                if (_dbConnection.State == ConnectionState.Closed)
                    _dbConnection.Open(); // Yani veritabanı bağlantısını açıp gönderdik.
                return _dbConnection;
            }
        }

        public static async Task<IEnumerable<T>> QueryAsync<T>(string sql) =>
            await _dbConnection.QueryAsync<T>(sql); //Select sorgularımız bu fonk ile çalıştırıcaz.

        public static async Task<int> ExecuteAsync(string sql) =>
            await _dbConnection.ExecuteAsync(sql); //Insert,Update,Delete işlemlerini bu fonk ile  çalıştırıcaz.


        public static void DataReaderReady() => _dataReaderState = true; // ne zaman bu fonk tetiklenir dataReader hazır artık demektir.
        public static void DataReaderBusy() => _dataReaderState = false;
        public static bool DataReaderState => _dataReaderState;

    }
}
