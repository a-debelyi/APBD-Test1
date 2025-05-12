namespace APBD_Test1.Infrastracture.Repositories;

public interface IDoctorsRepository
{
    Task<int?> GetDoctorIdAsync(string pwz);
}