using SportStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.Models.Repositories
{
    public class SQLDepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbContext _context;

        public SQLDepartmentRepository(AppDbContext context)
        {
            _context = context;
        }
        public bool AddDepartment(string dept)
        {
            if (!DepartmentExists(dept))
            {
                Department department = new Department() { DepartmentName = dept };
                _context.Departments.Add(department);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool DepartmentExists(string dept)
        {
            if (_context.Departments.Select(d => d.DepartmentName).Contains(dept)) return true;
            return false;
        }

        public IEnumerable<Department> GetDepartments()
        {
            return _context.Departments;
        }

        public bool RemoveDepartment(string dept)
        {
            if (DepartmentExists(dept))
            {
                Department department = new Department() { DepartmentName = dept };
                _context.Departments.Remove(department);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
