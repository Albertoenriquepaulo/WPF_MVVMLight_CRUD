   M S B A M L   `   `   `  ����I  EMVVMLight_CRUD, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:MVVMLight_CRUD.ViewsMVVMLight_CRUD.Views  5      +    %MVVMLight_CRUD.Views.SaveEmployeeView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      4local"clr-namespace:MVVMLight_CRUD.Views     5        � DataContext  5	      �� *Main+   Locator �� Source# [  .��5
      �  � RowDefinitions 5   
   �� 5      $��20 �6   �� 5      $
��auto �6   �� 5      $��30 �6   �� 5      $
��auto �6   �� 5      $��10 �6   �� 5      $
��auto �6   �� 5      $��10 �6   �� 5      $
��auto �6   �� 5      $��10 �6   �� 5      $
��auto �6   �� 5      $��10 �6   �� 5      $
��auto �6   �� 5      $��80 �6   �� 5      $��60 �6   �� 5      $��* �6   �� 5      $��20 �6    � ColumnDefinitions 5   
   �� 5      $��20 �6   �� 5      $��2* �6   �� 5       $��4* �6   �� 5!      $��20 �6   .+�5$   
   �� $��1��6   $��Employee Data��6!   $��4��5%      $��Center=�5&      $��Center=�6/   �� 5)   
   $��3��6   $��1��6!   $��Number��61   �� 5*   
   $��3��6   $��2��6   ��5+      �� *Employee.EmpNo+�� 5-   
   $��5��6   $��1��6!   $
��Name��61   �� 5.   
   $��5��6   $��2��6   ��5/      �� *Employee.EmpName+�� 51   
   $��7��6   $��1��6!   $��Salary��61   �� 52   
   $��7��6   $��2��6   ��53      �� *Employee.Salary+�� 55   
   $��9��6   $��1��6!   $��
Department��61   �� 56   
   $��9��6   $��2��6   ��57      �� *Employee.DeptName+�� 59   
   $��11��6   $��1��6"   $��Designation:��62   �� 5:   
   $��11��6   $��2��6    ��5;      �� *Employee.Designation+�� 5=   
   $��13��6   $��1��6   $��2��5>      $
��Save��5?      ��5@      �� *SaveCommand+��61   �� *
Employee+5B      5C      