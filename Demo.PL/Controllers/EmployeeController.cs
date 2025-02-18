using Demo.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Demo.DAL.Models;
using System;
using Demo.BLL.Repositories;
using Demo.PL.ViewModels;
using AutoMapper;
using System.Collections.Generic;
using Demo.PL.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace Demo.PL.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        //private readonly IEmployeeRepository _employeeRepository;
        //private readonly IDepartmentRepository _departmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(string SearchValue)
        {
            IEnumerable<Employee> employees;

            if (string.IsNullOrEmpty(SearchValue))

                employees = await _unitOfWork.EmployeeRepository.GetAll();
            else
                employees =  _unitOfWork.EmployeeRepository.SearchEmployeesByName(SearchValue); // IQuerable


            var mappedEmps = _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeViewModel>>(employees);
            return View(mappedEmps);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.departments = await _unitOfWork.DepartmentRepository.GetAll();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel employeeVM)
        {
            if (ModelState.IsValid)
            {
                employeeVM.ImageName =await DocumentSettings.UploadFile(employeeVM.Image, "images");

                var mappedEmp = _mapper.Map<EmployeeViewModel, Employee>(employeeVM);
               await _unitOfWork.EmployeeRepository.Add(mappedEmp);
                int count =await _unitOfWork.Complete();

                if (count > 0)

                    TempData["Message"] = "Employee is Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            else

                return View(employeeVM);

        }

        public async Task<IActionResult> Details(int? id, string ViewName = "Details")
        {
            if (id is null)
                return BadRequest();
            var employee = await _unitOfWork.EmployeeRepository.Get(id.Value);
            if (employee is null)
                return NotFound();
            
            var mappedEmp = _mapper.Map<Employee, EmployeeViewModel>(employee);
            return View(ViewName, mappedEmp);

        }

        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.departments = await _unitOfWork.DepartmentRepository.GetAll();



            return await Details(id, "Edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, EmployeeViewModel employeeVM)
        {
            if (id != employeeVM.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    employeeVM.ImageName =await DocumentSettings.UploadFile(employeeVM.Image, "images");
                    var mappedEmp = _mapper.Map<EmployeeViewModel, Employee>(employeeVM);
                    _unitOfWork.EmployeeRepository.Update(mappedEmp);
                    await _unitOfWork.Complete();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(employeeVM);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            return await Details(id, "Delete");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Delete([FromRoute] int id, EmployeeViewModel employeeVM)
        {
            if (id != employeeVM.Id)
                return BadRequest();

            try
            {
                var mappedEmp = _mapper.Map<EmployeeViewModel, Employee>(employeeVM);

                _unitOfWork.EmployeeRepository.Delete(mappedEmp);
              int count  = await _unitOfWork.Complete();
                if (count > 0)
                    DocumentSettings.DeleteFile(employeeVM.ImageName, "images");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.TryAddModelError(string.Empty, ex.Message);
            }

            return View(employeeVM);
        }

    }
}
