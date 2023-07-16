using BilgeShop.Business.Dtos;
using BilgeShop.Business.Services;
using BilgeShop.Business.Types;
using BilgeShop.Data.Entities;
using BilgeShop.Data.Repositories;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilgeShop.Business.Managers
{
    public class UserManager : IUserServices
    {
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IDataProtector _dataProtector;
        public UserManager(IRepository<UserEntity> userRepository, IDataProtectionProvider dataProtectionProvider)
        {
            _userRepository = userRepository;
            _dataProtector = dataProtectionProvider.CreateProtector("Security");
        }
        public ServiceMessage AddUser(AddUserDto addUserDto)
        {
            var hasMail=_userRepository.GetAll(x=>x.Email.ToLower()==addUserDto.Email.ToLower()).ToList();
            if(hasMail.Any() )
            {
                return new ServiceMessage
                {
                    IsSucced = false,
                    Message = "Bu e-mail adresi kullanımda"
                };
            }
            var entity = new UserEntity()
            {
                Email = addUserDto.Email,
                FirstName = addUserDto.FirstName,
                LastName = addUserDto.LastName,
                Password = _dataProtector.Protect(addUserDto.Password),
                UserType = Data.Enums.UserTypeEnum.User,
            };

            _userRepository.Add(entity);

            return new ServiceMessage
            {
                IsSucced = true,
                Message = "Kayıt Başarılı"
            };
        }

        public UserInfoDto LoginUser(LoginDto loginDto)
        {
            var userEntity =_userRepository.Get(x=>x.Email==loginDto.Email);

            if(userEntity is null) 
            {
                return null;
            }

            var rawPassword=_dataProtector.Unprotect(userEntity.Password);

            if(loginDto.Password==rawPassword)
            {
                return new UserInfoDto()
                {
                    Id = userEntity.Id,
                    Email = userEntity.Email,
                    FirstName = userEntity.FirstName,
                    LastName = userEntity.LastName,
                    UserType = userEntity.UserType,
                };
            }
            else
            {
                return null;
            }
        }

    }
}
