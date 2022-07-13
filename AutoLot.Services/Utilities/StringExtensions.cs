using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoLot.Services.Utilities
{
    public static class StringExtensions
    {
        public static string CutController(this string original)
            => original.Replace("Controller", "", StringComparison.OrdinalIgnoreCase);
    }
}
