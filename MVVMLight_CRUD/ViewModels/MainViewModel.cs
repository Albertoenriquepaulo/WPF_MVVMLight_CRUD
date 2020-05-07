using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MVVMLight_CRUD.Models;
using MVVMLight_CRUD.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace MVVMLight_CRUD.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        #region DataAccesServiceInyection
        IDataAccessService _serviceProxy;
        #endregion

        #region CommandForGetEmployeesOperation
        public RelayCommand ReadAllCommand { get; set; }
        #endregion

        #region CommandForGetSaveEmployeeOperation
        public RelayCommand<Employee> SaveCommand { get; set; }
        #endregion

        #region CommandForGetSearchEmployeeOperation
        public RelayCommand SearchCommand { get; set; }
        #endregion

        #region MainViewModelConstructor
        public MainViewModel(IDataAccessService serviceProxy)
        {
            _serviceProxy = serviceProxy;

            Employees = new ObservableCollection<Employee>();
            ReadAllCommand = new RelayCommand(GetEmployees);

            Employee = new Employee();
            SaveCommand = new RelayCommand<Employee>(SaveEmployee);

            EmployeeName = string.Empty;
            SearchCommand = new RelayCommand(SearchEmployee);

            GetEmployees();

        }
        #endregion

        #region GetEmployeesOperation
        private ObservableCollection<Employee> _employees;
        public ObservableCollection<Employee> Employees
        {
            get { return _employees; }
            set
            {
                _employees = value;
                RaisePropertyChanged("Employees");
            }
        }
        void GetEmployees()
        {
            Employees.Clear();
            foreach (var item in _serviceProxy.GetEmployees())
            {
                Employees.Add(item);
            }
        }
        #endregion

        #region SaveEmployeeOperation
        private Employee _employee;
        public Employee Employee
        {
            get { return _employee; }
            set { _employee = value; RaisePropertyChanged("Employee"); }
        }
        void SaveEmployee(Employee emp)
        {
            Employee.EmpNo = _serviceProxy.CreateEmployee(emp);
            if (Employee.EmpNo != 0)
            {
                Employees.Add(Employee);
                GetEmployees();
                //RaisePropertyChanged("Employee"); //Creo que esto está demás
            }
        }
        #endregion

        #region SearchEmployeeOperation
        private string _employeeName;
        public string EmployeeName
        {
            get { return _employeeName; }
            set { _employeeName = value; RaisePropertyChanged(nameof(EmployeeName)); }
        }

        void SearchEmployee()
        {
            Employees.Clear();
            var Res = from e in _serviceProxy.GetEmployees()
                      where e.EmpName.StartsWith(EmployeeName)
                      select e;
            foreach (var item in Res)
            {
                Employees.Add(item);
            }
        }
        #endregion


    }
}