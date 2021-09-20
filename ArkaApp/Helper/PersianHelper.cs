using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ArkaApp.Helper
{
    public static class PersianHelper
    {
        public static int GetPersinYear(this DateTime value)
        {
            System.Globalization.PersianCalendar persianCalendar = new System.Globalization.PersianCalendar();

            return persianCalendar.GetYear(value);
        }
        public static string GetPersinDateTime(this DateTime value, bool isIncludeSecond = true)
        {
            System.Globalization.PersianCalendar persianCalendar = new System.Globalization.PersianCalendar();

            return
                isIncludeSecond ?
                string.Format("{0}/{1}/{2} {3}:{4}:{5}", new object[]{ persianCalendar.GetYear(value), persianCalendar.GetMonth(value).ToString("00"),
                persianCalendar.GetDayOfMonth(value).ToString("00"),
                value.Hour.ToString("00"),value.Minute.ToString("00"),value.Second.ToString("00")}) :
                string.Format("{0}/{1}/{2} {3}:{4}", new object[]{ persianCalendar.GetYear(value), persianCalendar.GetMonth(value).ToString("00"),
                persianCalendar.GetDayOfMonth(value).ToString("00"),
                value.Hour.ToString("00"),value.Minute.ToString("00")});
        }
        public static string GetPersinDate(this DateTime value)
        {
            System.Globalization.PersianCalendar persianCalendar = new System.Globalization.PersianCalendar();

            return
                  string.Format("{0}/{1}/{2}", new object[]{ persianCalendar.GetYear(value), persianCalendar.GetMonth(value).ToString("00"),
                persianCalendar.GetDayOfMonth(value).ToString("00")});
        }

        public static DateTime ToMiladi(string value)
        {
            DateTime dt = DateTime.Parse(value, new CultureInfo("fa-IR"));

            PersianCalendar persianCalendar = new PersianCalendar();
            return new DateTime(persianCalendar.GetYear(dt), persianCalendar.GetMonth(dt), persianCalendar.GetDayOfMonth(dt), persianCalendar);
        }

        public static string ToEnglishNumber(this string input)
        {
            string[] persian = new string[10] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };

            input = input.Replace("/", "");

            for (int j = 0; j < persian.Length; j++)
                input = input.Replace(persian[j], j.ToString());

            return input.Substring(0, 4) + '/' +
                input.Substring(4, 2) + '/' +
                input.Substring(6, 2);
        }

    }
}