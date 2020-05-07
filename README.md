# Usando MVVM Light con WPF para implementación Model-View-ViewModel

Creamos proyecto `WPF NetFramework` e instalamos el Nuget `MVVM Light`, la instalación añadirá las librerias necesarias además del la carpeta ViewModel con las siguientes dos clases:

1. `MainViewModel.cs`, esta clase hereda de `ViewModelBase` y nos da el acceso al método *RaisedPropertyChanged*, que es quién nos notificará cuando cambie el valor de una propiedad

2. ViewModelLocator.cs, contiene las referencias estáticas para todos los ViewModel en la aplicación. Su constructor nos provee de un simple container IOC para registrar y resolver instancias

   Información acerca de  Service Locator: [Link](http://msdn.microsoft.com/en-us/library/ff648968.aspx)

   

Creamos una Base de Datos sencilla, con solo una tabla Employee

![image-20200507145535142](C:\Users\Alberto\AppData\Roaming\Typora\typora-user-images\image-20200507145535142.png)

