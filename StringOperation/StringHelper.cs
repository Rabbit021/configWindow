using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace StringOperation
{
    public static class StringHelper
    {
        public static string ReplaceExt(this string originStr, string oldStr, string newStr, bool isRegix = false)
        {
            if (string.IsNullOrEmpty(oldStr) || string.Equals(oldStr, newStr)) return originStr;
            string updatedStr = originStr;
            try
            {
                if (string.IsNullOrEmpty(originStr)) return originStr;
                if (!isRegix)
                    updatedStr = originStr.Replace(oldStr, newStr);
                else
                    updatedStr = Regex.Replace(originStr, oldStr, newStr);
            }
            catch (System.Exception ex)
            {

            }
            return updatedStr;
        }
      
        public static string RemoveFromTo(this string originStr, int from, int to)
        {
            StringBuilder builder = new StringBuilder(originStr);
            try
            {
                int canLength = Math.Min((to - from), builder.Length);
                if (from < originStr.Length)
                    builder.Remove(from, canLength);
            }
            catch (Exception ex)
            {
                Console.WriteLine("RemoveFromTo " + ex.Message);
            }
            return builder.ToString();
        }

        public static string RemoveSide(this string originStr, int length, bool firstOrLast)
        {
            StringBuilder builder = new StringBuilder(originStr);
            try
            {
                int canLength = Math.Min(length, builder.Length);
                if (firstOrLast)
                    builder.Remove(0, canLength);
                else
                    builder.Remove(builder.Length - canLength, canLength);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("RemoveSide " + ex.Message);
            }
            return builder.ToString();
        }

        public static string InsertAt(this string originStr, int index, string appendStr)
        {
            StringBuilder builder = new StringBuilder(originStr);
            try
            {
                int maxIndex = originStr.Length, minIndex = 0;
                index = Math.Min(maxIndex, index);
                index = Math.Max(minIndex, index);
                builder.Insert(index, appendStr);
            }
            catch (System.Exception ex)
            {

            }
            return builder.ToString();
        }
        public static string InsertPreFix(this string originStr, string appendStr)
        {
            return originStr.InsertAt(0, appendStr);
        }
        public static string InsertStuffix(this string originStr, string appendStr)
        {
            return originStr.InsertAt(originStr.Length, appendStr);
        }
    }
}
