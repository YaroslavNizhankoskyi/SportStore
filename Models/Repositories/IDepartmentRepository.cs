using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.Models.Repositories
{
    public interface IDepartmentRepository
    {
        bool AddDepartment(string dept);

        bool RemoveDepartment(string dept);

        bool DepartmentExists(string dept);

        IEnumerable<Department> GetDepartments();
    }
}
