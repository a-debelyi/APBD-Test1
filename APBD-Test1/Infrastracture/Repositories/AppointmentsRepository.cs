using APBD_8.Exceptions;
using APBD_Test1.Core.Models.DTOs.Request;
using APBD_Test1.Core.Models.DTOs.Response;
using Microsoft.Data.SqlClient;

namespace APBD_Test1.Infrastracture.Repositories;

public class AppointmentsRepository : IAppointmentsRepository
{
    private readonly string _connectionString;

    public AppointmentsRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<bool> ExistsAppointmentAsync(int id)
    {
        string sql = "SELECT 1 FROM appointment WHERE appointment_id = @id";
        await using (SqlConnection conn = new(_connectionString))
        await using (SqlCommand comm = new(sql, conn))
        {
            await conn.OpenAsync();
            comm.Parameters.AddWithValue("@id", id);
            return null != await comm.ExecuteScalarAsync();
        }
    }

    public async Task<AppointmentResponseDto> GetAppointmentAsync(int id)
    {
        string sql = @"
                SELECT a.date, 
                       p.first_name, 
                       p.last_name, 
                       p.date_of_birth, 
                       d.doctor_id,
                       d.pwz,
                       s.name,
                       aps.service_fee
                FROM appointment a
                JOIN patient p ON p.patient_id = a.patient_id
                JOIN doctor d ON d.doctor_id = a.doctor_id
                JOIN appointment_service aps ON aps.appointment_id = a.appointment_id
                JOIN service s ON s.service_id = aps.service_id
                WHERE a.appointment_id = @id;
        ";

        AppointmentResponseDto? appointment = null;

        await using (var conn = new SqlConnection(_connectionString))
        await using (var cmd = new SqlCommand(sql, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("@id", id);

            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    if (appointment is null)
                    {
                        appointment = new AppointmentResponseDto()
                        {
                            Date = reader.GetDateTime(0),
                            Patient = new PatientResponseDto()
                            {
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                DateOfBirth = reader.GetDateTime(3),
                            },
                            Doctor = new DoctorResponseDto
                            {
                                DoctorId = reader.GetInt32(4),
                                Pwz = reader.GetString(5)
                            },
                            AppointmentServices = new List<AppointmentServiceResponseDto>()
                        };
                    }

                    appointment.AppointmentServices.Add(new AppointmentServiceResponseDto
                    {
                        Name = reader.GetString(6),
                        ServiceFee = reader.GetDecimal(7)
                    });
                }
            }
        }

        return appointment;
    }

    public async Task<int> CreateAppointmentAsync(AppointmentRequestDto appointment, int doctorId)
    {
        await using (SqlConnection conn = new(_connectionString))
        {
            await conn.OpenAsync();
            SqlTransaction transaction = conn.BeginTransaction();
            try
            {
                string insertAppointmentSql = @"
                            INSERT INTO appointment (appointment_id, patient_id, doctor_id, date)
                            VALUES (@appointment_id, @patient_id, @doctor_id, @date);
                ";
                await using (SqlCommand comm = new(insertAppointmentSql, conn, transaction))
                {
                    comm.Parameters.AddWithValue("@appointment_id", appointment.AppointmentId);
                    comm.Parameters.AddWithValue("@patient_id", appointment.PatientId);
                    comm.Parameters.AddWithValue("@doctor_id", doctorId);
                    comm.Parameters.AddWithValue("@date", DateTime.Now);
                    await comm.ExecuteScalarAsync();
                }

                foreach (var service in appointment.Services)
                {
                    int serviceId;
                    string getServiceIdSql = @"
                                SELECT TOP 1 service_id 
                                FROM service 
                                WHERE name = @name
                                ORDER BY service_id DESC;
                    ";
                    await using (SqlCommand comm = new(getServiceIdSql, conn, transaction))
                    {
                        comm.Parameters.AddWithValue("@name", service.ServiceName);
                        var serviceIdObj = await comm.ExecuteScalarAsync();
                        if (serviceIdObj is null)
                            throw new NotFoundException($"Service with name {service.ServiceName} does not exist");
                        serviceId = Convert.ToInt32(serviceIdObj);
                    }

                    string insertAppointmentServiceSql = @"
                                INSERT INTO appointment_service (appointment_id, service_id, service_fee)
                                VALUES (@appointment_id, @service_id, @service_fee);
                    ";
                    await using (SqlCommand comm = new(insertAppointmentServiceSql, conn, transaction))
                    {
                        comm.Parameters.AddWithValue("@appointment_id", appointment.AppointmentId);
                        comm.Parameters.AddWithValue("@service_id", serviceId);
                        comm.Parameters.AddWithValue("@service_fee", service.ServiceFee);
                        await comm.ExecuteScalarAsync();
                    }
                }

                transaction.Commit();
                return appointment.AppointmentId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}