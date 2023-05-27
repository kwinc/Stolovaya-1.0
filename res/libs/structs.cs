using System;
using System.Text.Json;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using Xceed.Wpf.Toolkit.PropertyGrid.Converters;

namespace Stolovaya_1._0
{
    public struct idParam
    {
        public int id { get; set; }
        public string name { get; set; }
        public string desc { get; set; }
    }
    public struct forDBmanip
    {
        public bool isEdit { get; set; }
        public string title { get; set;}
        public idParam element { get; set;}
        public string fast_db { get; set; }
        public string fast_db_row { get; set; }
    }
    public struct person
    {
        public int id_person { get; set; }
        public string fio { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public bool isAdmin { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public DateTime birthday { get; set; }
        public idParam post { get; set; }
    }
    public struct type_product
    {
        public int id_type_product { get; set; }
        public string name { get; set; }
        public double price { get; set; }
        public idParam id_unit { get; set; }
    }
    public struct product_storage
    {
        public int id_product_storage { get; set; }
        public type_product id_type_product { get; set; }
        public DateTime date_start { get; set; }
        public DateTime date_end { get; set; }
        public double count { get; set; }
        public idParam id_unit { get; set; }
    }
    public struct decommissioned_product
    {
        public int id_decommission { get; set; }


        public type_product id_type_product { get; set; }
        public DateTime date_start { get; set; }
        public DateTime date_end { get; set; }
        public double count { get; set; }
        public idParam id_unit { get; set; }


        public idParam id_reason { get; set; }
        public string description { get; set; }
        public person whoDecommissioned { get; set; }
        public DateTime whenDecommissioned { get; set; }
    }
    public struct dish
    {
        public int id_dish { get; set; }
        public string name_dish { get; set; }
        public double price { get; set; }
        public string products { get; set; }
        public double weight { get; set; }
        public byte[] picture { get; set; }
        public BitmapImage readyPic { get; set; }
    }
    public struct dailyMenuS
    {
        public int id_menu { get; set; }
        public dish[] dishes { get; set; }
        public DateTime date { get; set; }
    }
/*Person tom = new Person("Tom", 37);
  string json = JsonSerializer.Serialize(tom);
  Console.WriteLine(json);
  Person? restoredPerson = JsonSerializer.Deserialize<Person>(json);
  Console.WriteLine(restoredPerson?.Name); // Tom
 
  class Person
  {
    public string Name { get; }
    public int Age { get; set; }
    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }
  }*/
}
