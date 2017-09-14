using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Test
{
    public static class UIHelper
    {
        public static void ResreshItems(object itemSource)
        {
            CollectionViewSource.GetDefaultView(itemSource).Refresh();
        }
    }
}
