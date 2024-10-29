using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MIS.Infrastucture;
using MIS.Models.DB;
using MIS.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MIS.Services
{
    public interface IDoctorService
    {
        Task<object> register(DoctorRegisterModel doctor);

        Task<TokenResponseModel> login(LoginCredentialsModel loginCredentials);

        Task<ResponseModel> logout(string token);

        Task<DoctorModel> getProfile(ClaimsPrincipal doctor);

        Task<ResponseModel> editProfile(DoctorEditModel doctorEdit, ClaimsPrincipal user);
    }
    public class DoctorService : IDoctorService
    {
        private readonly MisDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;
        private readonly ITokenBlackListService _tokenBlackListService;

        public DoctorService(
            MisDbContext context, 
            IPasswordHasher passwordHasher, 
            IJwtProvider jwtProvider, 
            ITokenBlackListService tokenBlackListService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
            _tokenBlackListService = tokenBlackListService;
        }

        // регистрация доктора
        public async Task<object> register(DoctorRegisterModel doctor)
        {
            var hashedPassword = _passwordHasher.Generate(doctor.password);

            // проверка существует ли уже доктор с такой почтой или телефоном
            var exists = await _context.Doctors
                                .AsNoTracking()
                                .AnyAsync(d => d.email == doctor.email || d.phone == doctor.phone);

            if (exists)
            {
                return new ResponseModel { status = "400", message = "this email or phone number already registered" };
            }

            // проверка существует ли такая специализация
            exists = await _context.Specialties
                .AsNoTracking()
                .AnyAsync(i => i.id == doctor.speciality);

            if (!exists)
            {
                return new ResponseModel { status = "400", message = "wrong specialty" };
            }

            DbDoctor newDoctor = new DbDoctor
            {
                birthday = doctor.birthday,
                createTime = DateTime.Now,

                email = doctor.email,
                gender = doctor.gender,
                name = doctor.name,
                phone = doctor.phone,
                passwordHash = hashedPassword,
                specialtyId = doctor.speciality
            };

            await _context.Doctors.AddAsync(newDoctor);
            await _context.SaveChangesAsync();

            return _jwtProvider.GenerateToken(newDoctor);
        }

        public async Task<TokenResponseModel> login(LoginCredentialsModel loginCredentials)
        {
            var doctor = await _context.Doctors
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.email == loginCredentials.email);

            if (doctor == null)
            {
                return null;
            }

            var result = _passwordHasher.Verify(loginCredentials.password, doctor.passwordHash);

            if (!result)
            {
                return null;
            }
            else
            {
                return _jwtProvider.GenerateToken(doctor);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<ResponseModel> logout(string token)
        {
            if (!string.IsNullOrEmpty(token))
            { 
                await _tokenBlackListService.AddTokenToBlackList(token);
                return new ResponseModel { status = "200", message = "Success" };
            }
            else
            {
                return new ResponseModel { status = "404", message = "Logout Failed" };
            }
        }

        public async Task<DoctorModel> getProfile(ClaimsPrincipal user)
        {
            var doctorId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (doctorId == null || !Guid.TryParse(doctorId, out var parsedId))
            {
                return null;
            }

            var doctor = await _context.Doctors
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.id == parsedId);

            if (doctor == null)
            {
                return null;
            }

            return new DoctorModel
            {
                id = doctor.id,
                createTime = doctor.createTime,
                name = doctor.name,
                birthday = doctor.birthday,
                gender = doctor.gender,
                email = doctor.email,
                phone = doctor.phone
            };
        }

        public async Task<ResponseModel> editProfile(DoctorEditModel doctorEdit, ClaimsPrincipal user)
        {
            var doctorId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (doctorId == null || !Guid.TryParse(doctorId, out var parsedId))
            {
                return new ResponseModel { status = "401", message = "Unauthorized"};
            }

            var doctor = await _context.Doctors
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.id == parsedId);

            if (doctor == null)
            {
                return new ResponseModel { status = "404", message = "Not found" }; ;
            }

            // проверка существует ли уже доктор с такой почтой или телефоном
            var exists = await _context.Doctors
                                .AsNoTracking()
                                .AnyAsync(d => d.email == doctor.email || d.phone == doctor.phone);

            if (exists)
            {
                return new ResponseModel { status = "400", message = "this email or phone number already registered" };
            }

            doctor.email = doctorEdit.email;
            doctor.name = doctorEdit.name;
            doctor.birthday = doctorEdit.birthday;
            doctor.gender = doctorEdit.gender;
            doctor.phone = doctorEdit.phone;

            await _context.SaveChangesAsync();
            return new ResponseModel { status = "200", message = "Success" };
        }
    }
}
