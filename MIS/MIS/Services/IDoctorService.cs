using MIS.Models;

namespace MIS.Services
{
    public interface IDoctorService
    {
        bool register(DoctorRegisterModel doctor);

        bool login(LoginCredentialsModel loginCredentials);

        bool logout();

        DoctorModel getProfile();

        bool editProfile(DoctorEditModel doctorEdit);
    }
    public class DoctorService : IDoctorService
    {
        public bool register(DoctorRegisterModel doctor)
        {
            return true;
        }

        public bool login(LoginCredentialsModel loginCredentials)
        {
            return true;
        }

        public bool logout()
        {
            return true;
        }

        public DoctorModel getProfile()
        {
            return null;
        }

        public bool editProfile(DoctorEditModel doctorEdit)
        {
            return true;
        }
    }
}
