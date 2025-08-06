
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SE_GRA_ELE_FPT_Utilities
{
    public static class Ultility
    {
        ////Convert enum to selectList
        //public static SelectList EnumToSelectList<T>(bool all = true, bool isDisplayName = true, bool isIndexed = true)
        //{
        //    var value = Enum.GetValues(typeof(T)).Cast<T>().Select(
        //        e => new SelectListItem
        //        {
        //            Text = isDisplayName ? GetEnumDisplayName(e as Enum) : GetEnumDescription(e as Enum),
        //            Value = isIndexed ? Convert.ToInt32(e).ToString() : e.ToString()
        //        }).ToList();
        //    if (all) value.Insert(0, new SelectListItem { Text = "All", Value = null });
        //    var selectList = new SelectList(value, "Value", "Text");
        //    return selectList;
        //}
        //// Get description

        //// Get description
        //private static string GetEnumDescription(Enum value)
        //{
        //    FieldInfo fi = value.GetType().GetField(value.ToString());
        //    DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        //    if (attributes != null && attributes.Length > 0)
        //        return attributes[0].Description;
        //    else
        //        return value.ToString();
        //}
        //// Get display name
        //public static string GetEnumDisplayName(Enum value)
        //{
        //    FieldInfo fi = value.GetType().GetField(value.ToString());
        //    DisplayAttribute[] attributes = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);
        //    if (attributes != null && attributes.Length > 0)
        //        return attributes[0].Name;
        //    else
        //        return value.ToString();
        //}
    }
}
