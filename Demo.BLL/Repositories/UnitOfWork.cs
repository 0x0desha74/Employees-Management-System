using Demo.BLL.Interfaces;
using Demo.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IEmployeeRepository EmployeeRepository { get; set; }
        public IDepartmentRepository DepartmentRepository { get; set; }
        private readonly MVCAppDbContext _dbContext;

        public UnitOfWork(MVCAppDbContext dbContext) //Ask CLR for Creating Object from DbContext
        {
            EmployeeRepository = new EmployeeRepository(dbContext);
            DepartmentRepository = new DepartmentRepository(dbContext);
            _dbContext = dbContext;
        }

        public async Task<int> Complete()
          => await _dbContext.SaveChangesAsync();

        public void Dispose() // This Method id Run Automaticlly by CLR
            => _dbContext.Dispose();
    }
}
