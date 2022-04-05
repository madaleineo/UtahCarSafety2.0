using System;
using System.Linq;

namespace UtahCarSafety.Models.ViewModels
{
    public class CrashViewModel
    {
        public IQueryable<Crash> Crashes { get; set; }
        public IQueryable<City> Cities { get; set; }


        public PageInfo PageInfo { get; set; }
    }
}

