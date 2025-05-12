namespace APBD_Test1.Infrastracture.Repositories;

public interface IPatientsRepository
{
    Task<bool> ExistsPatientAsync(int id);
}