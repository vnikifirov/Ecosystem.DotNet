namespace SwiftCode.Core.Utilities
{
    using System.Collections;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    // ? Trim all string properties in custome objects of project
    public static class TrimStrings
    {
        // ? https://stackoverflow.com/questions/7726714/trim-all-string-properties
        public static TSelf TrimProps<TSelf>(
            TSelf obj,
            BindingFlags flags = (BindingFlags.Public | BindingFlags.Instance))
        {
            // ? Recursively get properties if an obj arg is Collection
            // ? To check if a property can be set, use the CanWrite property.
            if (obj is ICollection objList)
            {
                Parallel.ForEach(objList.Cast<object>(), objItem => TrimProps(objItem));
            }
            else
            {
                var properties = obj.GetType().GetProperties(flags);

                // ! Question for me:) Should i use parallel programing for columns?
                // ! Or enougth only for collections of records
                // ? Parallelize the in memory processing
                // ? Link => https://stackoverflow.com/questions/41561365/running-async-foreach-loop-c-sharp-async-await
                Parallel.ForEach(properties, property =>
                {
                    object propValue = property.GetValue(obj, null);

                    // ? Whenever a property is null, skipe it
                    if (propValue == null) return;

                    // ? Removing extra whitespaces if property is string
                    // ? To check if a property can be set, use the CanWrite property.
                    // ? Link => https://stackoverflow.com/questions/9364092/property-set-method-not-found-error-during-reflection
                    if ((propValue is string prop) && property.CanWrite)
                    {
                        property.SetValue(obj, prop.Trim(), null);
                    }

                    // ? Recursively get properties if property is Collection
                    // ? To check if a property can be set, use the CanWrite property.
                    if (obj is ICollection items)
                    {
                        Parallel.ForEach(items.Cast<object>(), item => TrimProps(item));
                    }

                    // ? Recursively get properties if property is "user-defined" or custome type
                    // ? To check if a property can be set, use the CanWrite property.
                    var isDefiendType = TypeValidator.IsTypeOf
                    (
                        propType: propValue.GetType(),
                        namepace: ProjectAssembly.Get()
                    );
                    if (isDefiendType && property.CanWrite)
                    {
                        TrimProps(propValue);
                    }
                });
            }

            return obj;
        }
    }
}
