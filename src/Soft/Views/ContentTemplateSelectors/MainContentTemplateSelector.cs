using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Soft.MVVM
{
    internal class MainContentTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ProjectStructureViewModel)
            {
                return (DataTemplate)(container as FrameworkElement)!.FindResource("ProjectStructureTemplate");
            }

            return new DataTemplate();
        }
    }
}
