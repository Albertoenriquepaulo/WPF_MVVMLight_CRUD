using MVVMLight_CRUD.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMLight_CRUD.Services
{
    /// <summary>
    /// The Interface defining methods for Create Employee and Read All Employees  
    /// </summary>
    public interface IDataAccessService
    {
        ObservableCollection<Employee> GetEmployees();
        int CreateEmployee(Employee Emp);
    }
}
