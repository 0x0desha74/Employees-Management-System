using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.DAL.Models;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Demo.PL.Controllers
{
    public class DepartmentController : Controller
    {
        //private readonly IDepartmentRepository _departmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private IActionResult wait;

        public DepartmentController(IUnitOfWork unitOfWork, IMapper mapper) //Ask CLR for creating object from class implementing Interface [IDepartmentRepository]
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(string SearchValue)
        {
            IEnumerable<Department> departments;

            if (string.IsNullOrEmpty(SearchValue))
                departments = await _unitOfWork.DepartmentRepository.GetAll();
            else
                departments = _unitOfWork.DepartmentRepository.SearchDepartmentByName(SearchValue);


            var mappedDepts = _mapper.Map<IEnumerable<Department>, IEnumerable<DepartmentViewModel>>(departments);
            return View(mappedDepts);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(DepartmentViewModel departmentVM)
        {
            if (ModelState.IsValid)
            {

                var mappedDept = _mapper.Map<DepartmentViewModel, Department>(departmentVM);
                await _unitOfWork.DepartmentRepository.Add(mappedDept);
                int count = await _unitOfWork.Complete();

                if (count > 0)
                    TempData["Message"] = "Department is Created Successfully";

                return RedirectToAction(nameof(Index));
            }
            else
                return View(departmentVM);

        }

        public async Task<IActionResult> Details(int? id, string ViewName = "Details")
        {
            if (id is null)
                return BadRequest();
            var department = await _unitOfWork.DepartmentRepository.Get(id.Value);
            if (department is null)
                return NotFound();
            var mappedDept = _mapper.Map<Department, DepartmentViewModel>(department);
            return View(ViewName, mappedDept);

        }

        public async Task<IActionResult> Edit(int? id)
        {
            ///if(id is null)
            ///    return BadRequest();
            ///var department = _departmentRepository.Get(id.Value);
            ///if( department is null)
            ///    return NotFound();
            ///return View(department);
            ///

            return await Details(id, "Edit");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, DepartmentViewModel departmentVm)
        {
            if (id != departmentVm.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                try
                {
                    var mappedDept = _mapper.Map<DepartmentViewModel, Department>(departmentVm);
                    _unitOfWork.DepartmentRepository.Update(mappedDept);
                    await _unitOfWork.Complete();

                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    //1.Log Exception
                    //2.Friendly Message   => not today

                    ModelState.AddModelError(string.Empty, ex.Message); // Development
                }
            }
            return View(departmentVm);
        }


        public async Task<IActionResult> Delete(int? id)
            => await  Details(id, "Delete");

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int id, DepartmentViewModel departmentVM)
        {
            if (id != departmentVM.Id)
                return BadRequest();

            try
            {
                var mappedDept = _mapper.Map<DepartmentViewModel, Department>(departmentVM);
                _unitOfWork.DepartmentRepository.Delete(mappedDept);
               await _unitOfWork.Complete();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.TryAddModelError(string.Empty, ex.Message);
            }

            return View(departmentVM);
        }
    }
}
