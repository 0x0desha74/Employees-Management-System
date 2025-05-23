﻿using AutoMapper;
using Demo.DAL.Models;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
namespace Demo.PL.Mapping_Profiles
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<EmployeeViewModel, Employee>().ReverseMap();
            CreateMap<DepartmentViewModel, Department>().ReverseMap();
            CreateMap<ApplicationUser, UserViewModel>().ReverseMap();
            CreateMap<RoleViewModel, IdentityRole>()
                .ForMember(d => d.Name, O => O.MapFrom(s => s.RoleName))
                .ReverseMap();
        }
    }
}
