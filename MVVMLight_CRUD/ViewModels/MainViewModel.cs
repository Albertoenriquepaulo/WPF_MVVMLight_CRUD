using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MVVMLight_CRUD.Models;
using MVVMLight_CRUD.Services;
using System.Collections.ObjectModel;

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
        IDataAccessService _serviceProxy;
        public RelayCommand ReadAllCommand { get; set; }
        public RelayCommand<Employee> SaveCommand { get; set; }
        public MainViewModel(IDataAccessService serviceProxy)
        {
            _serviceProxy = serviceProxy;

            Employees = new ObservableCollection<Employee>();
            ReadAllCommand = new RelayCommand(GetEmployees);

            Employee = new Employee();
            SaveCommand = new RelayCommand<Employee>(SaveEmployee);

            GetEmployees();

        }

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




    }
}