// See https://aka.ms/new-console-template for more information

/* Reflection in C# allows you to inspect and manipulate metadata of types (classes, interfaces, enums, etc.), methods, properties, and other members of a .NET application at runtime. 
It provides the ability to query type information, invoke methods, and access fields dynamically.  */

using System;
using System.Reflection;

public class MyClass
{
    public int MyProperty { get; set; }
    public string? MyField;

    public static void MyMethod()
    {
        Console.WriteLine("MyMethod called");
    }
}

class Program
{
    static void Main()
    {
        // Get the type information of MyClass using reflection
        Type myClassType = typeof(MyClass);

        // Display the name of the class
        Console.WriteLine("Class Name: " + myClassType.Name);

        // Get all properties of MyClass
        PropertyInfo[] properties = myClassType.GetProperties();

        // Display information about each property
        Console.WriteLine("\nProperties:");
        foreach (PropertyInfo prop in properties)
        {
            Console.WriteLine($"Name: {prop.Name}, Type: {prop.PropertyType}");
        }

        // Get all fields of MyClass
        FieldInfo[] fields = myClassType.GetFields();

        // Display information about each field
        Console.WriteLine("\nFields:");
        foreach (FieldInfo field in fields)
        {
            Console.WriteLine($"Name: {field.Name}, Type: {field.FieldType}");
        }

        // Get all methods of MyClass
        MethodInfo[] methods = myClassType.GetMethods();

        // Display information about each method
        Console.WriteLine("\nMethods:");
        foreach (MethodInfo method in methods)
        {
            Console.WriteLine($"Name: {method.Name}, Return Type: {method.ReturnType}");
        }
    }
}

/* In this example, the program defines a class MyClass with a property, a field, and a method. The Main method then uses reflection to get information about this class by obtaining 
its type (Type) and accessing its properties, fields, and methods using GetProperties(), GetFields(), and GetMethods() respectively. Finally, it prints out the information obtained for 
each property, field, and method.

This code demonstrates the basics of using reflection to inspect and retrieve information about types and their members at runtime in a C# program.*/