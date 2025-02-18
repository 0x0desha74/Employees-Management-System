using Demo.BLL.Interfaces;
using Demo.DAL.Context;
using Demo.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(MVCAppDbContext dbContext) : base(dbContext)
        {

        }
        public IQueryable<Employee> GetEmployeesByAddress(string address)
        { 
            _dbContext.Employees.Where(E => E.Address == address).ToList(); 
            return _dbContext.Employees;
        }

        public IQueryable<Employee> SearchEmployeesByName(string name)
           => _dbContext.Employees.Where(E => E.Name.ToLower().Contains(name.ToLower()));
           
        
    }
}
