using Microsoft.Data.SqlClient;

namespace APBD_Test1.Infrastracture.Repositories;

public class DoctorsRepository : IDoctorsRepository
{
    private readonly string _connectionString;

    public DoctorsRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<int?> GetDoctorIdAsync(string pwz)
    {
        string sql = @"
                SELECT TOP 1 doctor_id 
                FROM doctor 
                WHERE pwz = @pwz
                ORDER BY doctor_id DESC
        ";

        await using (SqlConnection conn = new(_connectionString))
        await using (SqlCommand comm = new(sql, conn))
        {
            await conn.OpenAsync();
            comm.Parameters.AddWithValue("@pwz", pwz);
            var doctorId = await comm.ExecuteScalarAsync();
            return doctorId != null ? Convert.ToInt32(doctorId) : null;
        }
    }
}