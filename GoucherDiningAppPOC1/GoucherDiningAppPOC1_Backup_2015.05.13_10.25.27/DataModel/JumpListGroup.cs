using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoucherDiningAppPOC1
{
    public class JumpListGroup<T> : List<object>
    {
        /// <summary>
        /// Key that represents the group of objects and used as group header.
        /// </summary>
        public object Key { get; set; }

        public new IEnumerator<object> GetEnumerator()
        {
            return (System.Collections.Generic.IEnumerator<object>)base.GetEnumerator();
        }
    }
}
