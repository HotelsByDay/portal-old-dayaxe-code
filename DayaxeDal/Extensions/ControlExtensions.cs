using System.Linq;
using System.Web.UI;

namespace DayaxeDal.Extensions
{
    public static class ControlExtensions
    {
        public static Control FindControlRecursive(Control container, string name)
        {
            if ((container.ID != null) && (container.ID.Equals(name)))
                return container;

            return (from Control ctrl in container.Controls select FindControlRecursive(ctrl, name)).FirstOrDefault(foundCtrl => foundCtrl != null);
        }
    }
}
