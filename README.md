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

![](https://github.com/Albertoenriquepaulo/WPF_MVVMLight_CRUD/blob/master/MVVMLight_CRUD/assets/image-20200507145535142.png=raw=true)

## Paso 2

Añadir una nueva carpeta `Models`. En esta carpeta añadir un nuevo item `ADO.NET Entity Data Model`, con el nombre que quieras, en mi caso **CompanyAEPC** e incluirle la tabla creada en el **Paso 1**. Cuando esto esté listo veremos la siguiente tabla en nuestro VS,

![image-20200507154505797](https://github.com/Albertoenriquepaulo/WPF_MVVMLight_CRUD/blob/master/MVVMLight_CRUD/assets/image-20200507154505797.png?raw=true)


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

![image-20200507160832324](https://github.com/Albertoenriquepaulo/WPF_MVVMLight_CRUD/blob/master/MVVMLight_CRUD/assets/image-20200507160832324.png?raw=true)

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

## Paso 6

Creamos una carpeta llamada Views y añadimos un item del tipo *User Control (WPF),* a esta `View` le coloqué el nombre de *EmployeeInfoView.xaml*

![image-20200507174456635](https://github.com/Albertoenriquepaulo/WPF_MVVMLight_CRUD/blob/master/MVVMLight_CRUD/assets/image-20200507174456635.png?raw=true)

## Paso 7

1. En esta vista añadimos un TextBlock, DataGrid y Button:

![image-20200507174618371](https://github.com/Albertoenriquepaulo/WPF_MVVMLight_CRUD/blob/master/MVVMLight_CRUD/assets/image-20200507174618371.png?raw=true)

2. En el XAML -> *EmployeeInfoView.xaml*, seteamos/igualamos/configuramos la propiedad *DataContext* del *UserControl* a la propiedad Main expuesta por la clase ViewModelLocator:

   ```C#
   DataContext="{Binding Main, Source={StaticResource Locator}}"
   ```

El *Locator* es declarado en los recursos del *App.xaml*. Main es la propiedad pública expuesta por el *ViewModelLocator* y esta ViewModelLocator retorna un objeto del tipo MainViewModel. 

***La expresión anterior significa que MainViewModel ahora está vinculado con UserControl. Esto significa que todas las declaraciones públicas (propiedades y comandos de notificación obligatoria) pueden vincularse con los elementos XAML en la vista EmployeeInfoView.xaml.***

3. Enlazamos (Bind) el command *ReadAllCommand* y la colección *Employees* del *MainViewModel* a el *Button* y el *DataGrid* respectivamente, esto porsupuesto lo hacemos en *EmployeeInfoView.xaml*

   ```xaml
   <DataGrid Grid.Row="3" Grid.Column="1" 
                     FontSize="16"
                     ColumnWidth="*"
                     IsReadOnly="True"
                     x:Name="dgemp"
                     ItemsSource="{Binding Employees}"
                     />
           <Button Grid.Row="5" Grid.Column="1" 
                   Content="Get List" 
                   FontWeight="Bold" FontSize="30"
                   x:Name="btnloadallemployees"
                   Command="{Binding ReadAllCommand}"/>
   ```

## Paso 8

Ejecutamos el proyecto y el DataGrid debe mostrar la data presente en employees cuando hagamos Click sobre el boton *"Get List"*

![image-20200507183551441](https://github.com/Albertoenriquepaulo/WPF_MVVMLight_CRUD/blob/master/MVVMLight_CRUD/assets/image-20200507183551441.png?raw=true)



# Pasando Parámetros desde el View a el ViewModel (Section 2)

En los pasos anteriores, vimos como crear el ViewModel, definimos Notificaciones de cambio de las propiedades y tambien el RelayCommand.

En esta sección veremos como enviar Data desde el View al ViewModel y escribir en nuestra Base de Datos.

## Paso 1

1. Declarar las siguientes propiedas en el *MainViewModel*

```c#
private Employee _employee;
public Employee Employee
{
    get { return _employee; }
    set { _employee = value; RaisePropertyChanged("Employee"); }
}
```

Este objeto Employee será usado para agregar los nuevos registros Employee

2. Ahora creamos el metodo que accederá a la BD *SaveEmployee* y guardará el empleado, todo esto en nuestra clase *MainViewModel*

```C#
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
        RaisePropertyChanged("Employee"); //Creo que esto está demás
    }
}
```

El metodo *CreateEmployee(emp)* retorna el numero de empleado, por lo que si este numero es diferente de cero, agragamos el objeto Employee a nuestro *Employees observable collection*.

Nos queda asociar el boton de Save o Guardar empleado a un Commando, para esto instanciamos un nuevo objeto RelayCommand in la clase ViewModel (*MainViewModel*)

```C#
public RelayCommand<employeeinfo> SaveCommand { get; set; }	
```

Declaramos La propiedad de tipo genérico *RelayCommand*, tomando en cuenta que ***T*** representa el parámetro de entrada; en nuestro caso, ***T*** es del tipo *Employee*.

Asi debe ir quedando nuestra clase ***MainViewModel***

```C#
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
                RaisePropertyChanged("Employee"); //Creo que esto está demás
            }
        }
    }
```

*RelayCommand* es pasado junto con el método *SaveEmployee()*. Esto es posible porque el método SaveEmployee acepta el objeto *Employee* como su parámetro de entrada. Este es el mismo objeto definido en la declaración de la propiedad genérica *RelayCommand*

## Paso 2

Añadiremos especie de estilos globales, para esto Abrimos *App.Xaml* y en resources, añadimos estilos a *TextBlock* y *TextBoxes*

```xaml
<Application x:Class="MVVMLight_CRUD.App" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" xmlns:local="clr-namespace:MVVMLight_CRUD" StartupUri="MainWindow.xaml" xmlns:d="http://schemas.microsoft.com/expression/blend/2008" d1p1:Ignorable="d" xmlns:d1p1="http://schemas.openxmlformats.org/markup-compatibility/2006">
    <Application.Resources>
        <vm:ViewModelLocator x:Key="Locator" d:IsDataSource="True" xmlns:vm="clr-namespace:MVVMLight_CRUD.ViewModel" />
        <Style TargetType="{x:Type TextBlock}">
            <Setter Property="FontSize" Value="20"></Setter>
            <Setter Property="FontWeight" Value="Bold"></Setter>
        </Style>

        <Style TargetType="{x:Type TextBox}">
            <Setter Property="FontSize" Value="20"></Setter>
            <Setter Property="FontWeight" Value="Bold"></Setter>
        </Style>
    </Application.Resources>
</Application>
```

Debido a que estos estilos están definidos en este ***XAML***, se aplicará estos estilos a todos los *TextBlocks* y *TextBoxes* en la aplicación, sin necesidad de usar Keys.

Vimos como hacer uso del Command generico para pasar parametros desde ***UI*** al *ViewModel*



# Definición de comandos en elementos de la interfaz de usuario que no tienen la propiedad Command (Section 3)

Existen elementos que basicamente su comportamiento o uso natural se presta mejor para el uso del *Command*, como lo son los Buttons, RadioButtons, OptionButtons, CheckBox, etc. Pero que sucederia si quisieramos enlazar un *command* a un TextBox, de manera que este se ejecute cada vez que escribimos en el. Esto lo ilustra bastante bien el tipico buscador que va sugiriendo o haciendo búsqueda a medida que vamos escribiendo. Entonces en este caso tendríamos que cambiar el comportamiento del TextBox para que soporte *command*.

La libreria **MVVM Light** nos ofrece una dll para ello. Cuando la instalamos está añade la DLL ***System.Windows.Interactivity.dll***  assembly, que nos permite definir comportamientos en los elementos de la UI.

**La libreria *MVVM Light*  nos ofrece una clase *EventToCommand*  bajo el *GalaSoft.MvvmLight.Command namespace*. Esta clase nos permite enlazar (bind) cualquier evento de cualquier *FrameworkElement* a un *ICommand*. En nuestro caso, utilizaremos *EventToCommand* para ejecutar un método en la clase MainViewModel definiendo el comando en el evento *TextChanged* del *TextBox*.**

## Paso 1

1. Debemos crear la propiedad a la cual le asignaremos el avlor del TextBox y que cambiara su valor a média que este TextBox cambie. Añadimos lo siguiente al *MainViewModel* 

```C#
private string _employeeName;
public string EmployeeName
{
    get { return _employeeName; }
    set { _employeeName = value; RaisePropertyChanged(nameof(EmployeeName)); }
}
```

Esta propiedad tipo string se unirá al TextBox del view. Esta propiedad será seteada cada vez que hagamos cambio o insertemos texto en el TextBox element del View.

2. Creamos un método para la búsqueda, filtrará los employees del Collection Employees basado en EmployeeName.

```C#
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
```

3. Ahora definiremos el *Objeto Command* en el mismo *MainViewModel* (*RelayCommand object*)

```c#
public RelayCommand SearchCommand { get; set; }
```

4. Y en el constructor del *MainViewModel* inicializamos el *Command Object* pasándole el metodo *SearchEmployee*, no hace falta pero también inicializo la variable *EmployeeName*

```C#
EmployeeName = string.Empty;
SearchCommand = new RelayCommand(SearchEmployee);
```

## Paso 2

Como último paso debemos hacer los ***Binds*** en las vista, es decir registrar la interactividad y configurar el *EventToObject* en el XAML. 

Debemos añadir el ***TextBlock*** y el ***TextBox*** correspondiente en la vista ***EmployeeInfoView.xaml***

```XAML
<TextBlock Grid.Row="3" VerticalAlignment="Center"
                   Grid.Column="1" Text="Search" FontSize="16"/>
        <TextBox Grid.Row="3" Grid.Column="1" 
                 VerticalAlignment="Center" 
                 Width="200" Height="30"
                 Margin="30,0,0,0"
                 Text="{Binding EmployeeName, UpdateSourceTrigger=PropertyChanged}"
                 >
            <i:Interaction.Triggers>
                <i:EventTrigger EventName="TextChanged">
                    <mvvm:EventToCommand 
                    Command="{Binding SearchCommand, Mode=OneWay}"
                     />
                </i:EventTrigger>
            </i:Interaction.Triggers>
        </TextBox>
```

Para que el código de arriba funcione debemos añadir las lineas de código siguiente (directivas) dentro de la etiqueta ***UserControl***

```xaml
xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity" 
xmlns:mvvm="http://www.galasoft.ch/mvvmlight"
```

Quedando esta etiqueta de la siguiente manera

```XAML
<UserControl x:Class="MVVMLight_CRUD.Views.EmployeeInfoView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" 
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008" 
             xmlns:local="clr-namespace:MVVMLight_CRUD.Views" 
             xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity" 
             xmlns:mvvm="http://www.galasoft.ch/mvvmlight"
             mc:Ignorable="d" 
             d:DesignHeight="450" d:DesignWidth="800"
             DataContext="{Binding Main, Source={StaticResource Locator}}">
    .
    .
    .
</UserControl>
```

**Usando el *EventToCommand*, podemos fácilmente enlazar un *Command* a un *FrameworkElement*.**