using System;
using System.Collections.Generic;
using System.Text;

namespace EncompassApi.xUnit.Extensions
{
    public static class ObjectArrayExtensions
    {
        public static IEnumerable<TObject> ChangeProperty<TObject>(this IEnumerable<TObject> objs, string PropertyName, object newValue)
        {
            foreach (var obj in objs)
            {
                var p = obj.GetType().GetProperty(PropertyName);
                p.SetValue(obj, newValue);

            }

            return objs;
        }
    }
}
