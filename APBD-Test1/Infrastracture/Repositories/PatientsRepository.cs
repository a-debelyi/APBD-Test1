using Microsoft.Data.SqlClient;

namespace APBD_Test1.Infrastracture.Repositories;

public class PatientsRepository : IPatientsRepository
{
    private readonly string _connectionString;

    public PatientsRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<bool> ExistsPatientAsync(int id)
    {
        string sql = "SELECT 1 FROM patient WHERE patient_id = @id";
        await using (SqlConnection conn = new(_connectionString))
        await using (SqlCommand comm = new(sql, conn))
        {
            await conn.OpenAsync();
            comm.Parameters.AddWithValue("@id", id);
            return null != await comm.ExecuteScalarAsync();
        }
    }
}