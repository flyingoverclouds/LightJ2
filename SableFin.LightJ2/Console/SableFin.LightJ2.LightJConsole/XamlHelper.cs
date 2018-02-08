using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace SableFin.LightJ2.LightJConsole
{
    static class XamlHelper
    {
        public static object FindResource(this FrameworkElement fe,object resourceName)
        {
            if (fe.Resources.ContainsKey(resourceName))
                return fe.Resources[resourceName];
            foreach (var md in fe.Resources.MergedDictionaries)
            {
                if (md.ContainsKey(resourceName))
                    return md[resourceName];
            }
            return null;
        }
    }
}
