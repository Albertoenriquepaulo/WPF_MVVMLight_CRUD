using MVVMLight_CRUD.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMLight_CRUD.Services
{
    class DataAccessService : IDataAccessService
    {
        MVVMLightDbEntities context;
        public DataAccessService()
        {
            context = new MVVMLightDbEntities();
        }
        public int CreateEmployee(Employee Emp)
        {
            context.Employees.Add(Emp);
            context.SaveChanges();
            return Emp.EmpNo;
        }

        public ObservableCollection<Employee> GetEmployees()
        {
            ObservableCollection<Employee> Employees = new ObservableCollection<Employee>();
            foreach (var item in context.Employees)
            {
                Employees.Add(item);
            }
            return Employees;
        }
    }
}
