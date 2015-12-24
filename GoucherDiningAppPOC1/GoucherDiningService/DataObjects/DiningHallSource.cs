using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Mobile.Service;

// The data model defined by this file serves as a representative example of a strongly-typed
// model.  The property names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs. If using this model, you might improve app 
// responsiveness by initiating the data loading task in the code behind for App.xaml when the app 
// is first launched.



namespace GoucherDiningService.DataObjects
{
    public class Entre : EntityData
    {

        public string Name { get; private set; }
        public double Calories { get; private set; }
        public string Station { get; private set; }

    }

    public class Menu : EntityData
    {

        public string MealType { get; private set; }
        public string Date { get; private set; }
        public string Open { get; private set; }
        public string Close { get; private set; }
        public List<Entre> Entres { get; private set; }
       
    }

    public class DiningHall : EntityData
    {
     
        public string Name { get; private set; }
        public List<Menu> Menus { get; private set; }        

    }

    
}