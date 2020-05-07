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
        public MainViewModel(IDataAccessService serviceProxy)
        {
            _serviceProxy = serviceProxy;
            Employees = new ObservableCollection<Employee>();
            ReadAllCommand = new RelayCommand(GetEmployees);

            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
        }

        private ObservableCollection<Employee> _Employees;
        public ObservableCollection<Employee> Employees
        {
            get { return _Employees; }
            set
            {
                _Employees = value;
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

    }
}