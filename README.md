# Usando MVVM Light con WPF para implementación Model-View-ViewModel

## Paso 1

1. Creamos proyecto `WPF NetFramework` e instalamos el Nuget `MVVM Light`, la instalación añadirá las librerias necesarias además del la carpeta ViewModel con las siguientes dos clases:

   1. `MainViewModel.cs`, esta clase hereda de `ViewModelBase` y nos da el acceso al método *RaisedPropertyChanged*, que es quién nos notificará cuando cambie el valor de una propiedad

   2. ViewModelLocator.cs, contiene las referencias estáticas para todos los ViewModel en la aplicación. Su constructor nos provee de un simple container IOC para registrar y resolver instancias

      Información acerca de  Service Locator: [Link](http://msdn.microsoft.com/en-us/library/ff648968.aspx)

**La instalación nos creará código con ciertos errores, hay que hacer estos ligeros  cambios**

En ViewModelLocator.cs file cambiar 

```
using Microsoft.Practices.ServiceLocation;
```

por

```
using CommonServiceLocator;
```

Si observamos un error como este 

```The name "ViewModelLocator" does not exist in the namespace ```

Click derecho a la solution "Clean" or "Clean solution" y después "Rebuild"

1. Creamos una Base de Datos sencilla, con solo una tabla Employee

![image-20200507145535142](C:\Users\Alberto\AppData\Roaming\Typora\typora-user-images\image-20200507145535142.png)

## Paso 2

Añadir una nueva carpeta `Models`. En esta carpeta añadir un nuevo item `ADO.NET Entity Data Model`, con el nombre que quieras, en mi caso **CompanyAEPC** e incluirle la tabla creada en el **Paso 1**. Cuando esto esté listo veremos la siguiente tabla en nuestro VS,

![image-20200507154505797](C:\Users\Alberto\AppData\Roaming\Typora\typora-user-images\image-20200507154505797.png)

## Paso 3

Añadir nueva carpeta llamada Services, dentro de esta carpeta crear una Interfaz y una clase.

**Interfaz IDataAccessService**

```C#
public interface IDataAccessService
{
	ObservableCollection GetEmployees();
	int CreateEmployee(EmployeeInfo Emp);
}
```

**Class DataAccessService**

```C#
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
```

*El nombre `MVVMLightDbEntities` es el nombre de la clase generada automáticamente cuando creamos en el paso 2 `ADO.NET Entity Data Model`. Esta es la clase de nuestro contexto*

![image-20200507160832324](C:\Users\Alberto\AppData\Roaming\Typora\typora-user-images\image-20200507160832324.png)

**Este código define nuestra interface para acceder a la data de nuestra base de datos usando Entity Framework.**

## Paso 4

Para registrar el **Data Access** service en el IoC, necesitamos registrar la clase `*DataAccessService*` dentro de este. Para esto abrimos la clase `ViewModelLocator` y le añadimos las siguientes lineas:

```C#
SimpleIoc.Default.Register<IDataAccessService, DataAccessService>();
```

Recordemos que *IDataAccessService* y *DataAccessService*, son las clases que hemos creado para acceder a nuestros datos, en estas clases está la lógica que nos lista los empleados y con el que podemos crear empleados, pudiéramos incluir mas. Estas clases instancian nuestro contexto para poder comunicarse con la DB.

Al colocarlo en el ViewModelLocator como vemos arriba, es como que lo estamos inyectando a nuestra app para que podamos usarlo posteriormente en las demas clases dentro del ViewModel, claro en las clases del ViewModel que querramos usarlo deberemos instanciarlo.

**Al registrarlo de esta forma estamos colocando este servicio en el IOC, asi se enviará este servicio a nuestras clases del ViewModel, asi que en estas clases (las del ViewModels) podemos recibirlas en el constructor, si lo deseamos.**

El namespace para la clase DataAccessService debe ser usado en la clase ViewModelLocator ,* es decir, el using, que en nuestro caso es:

```C#
using MVVMLight_CRUD.Services;
```

## Paso 5

1. Implementemos la lógica para leer todos los empleados. En la clase `MainViewModel` añadimos una propiedad y hacemos que esta genere una notificación cuando se produzca algún cambio en ella, para esto invocamos al método `RaisePropertyChanged` en el set de esta propiedad:

```c#
public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
        }

        private ObservableCollection<Employee> _Employees;
        public ObservableCollection<Employee>  Employees
        {
            get { return _Employees; }
            set { 
                _Employees = value;
                RaisePropertyChanged("Employees");
            }
        }

    }
```

De esta manera esta propiedad *_Employee* estará expuesta al **UI**. El *setter* de esta propiedad llama al método *RaisedPropertyChanged* que internamente a su vez invocará el evento *PropertyChanged*, cuando la data de la coleccion cambie.

2. Definimos el objeto *IDataAccessService* a nivel de la clase *ViewModel (**MainViewModel.cs**)*

```C#
IDataAccessService _serviceProxy;
```

En la clase *MainViewModel.cs* creamos el método para traernos todos los datos de nuestra tabla empleado

```C#
void GetEmployees()
{
    Employees.Clear();
    foreach (var item in _serviceProxy.GetEmployees())
    {
        Employees.Add(item);
    }
}
```

Este metodo de arriba llama al metodo GetEmployees() de la clase *DataAccessService* y coloca esa data dentro de un objeto del tipo colección observable, que en este caso se llama Employees.

3. En la clase MainViewModel.cs definimos el objeto *RelayCommand* 

``` C#
public RelayCommand ReadAllCommand { get; set; }
```

Ahora usamos el constructor para recibir este servicio, el cual recibimos ya que lo registramos en el IOC, en el Paso 4. También instanciamos e inicializamos el  *Employees* observable collection and *ReadAllCommand* object:

```C#
public MainViewModel(IDataAccessService serviceProxy)
{
    _serviceProxy = serviceProxy;
    Employees = new ObservableCollection<Employee>();
    ReadAllCommand = new RelayCommand(GetEmployees);
}
```

*ReadAllCommand es básicamente un delegado, pero su configuración y modo de funcionamiento lo hace el MVVM Light por nosotros*