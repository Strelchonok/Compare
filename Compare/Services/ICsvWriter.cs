using Compare.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Compare.Services
{
    public interface ICsvWriter
    {
        string Write<T>(IList<T> list, bool includeHeader = true);
        string Write<T>(IList<T> list, string fileName, bool includeHeader = true);
        bool WriteFile(string fileName, string csv);
        string[] ReadCSVFile(string fileName, string path);
        List<CsvModel> FromCsvToCSVModel(string[] CsvStrings);
    }

    public class CsvWriter : ICsvWriter
    {
        private const string DELIMITER = ",";

        public string Write<T>(IList<T> list, bool includeHeader = true)
        {
            StringBuilder sb = new StringBuilder();

            Type type = typeof(T);

            PropertyInfo[] properties = type.GetProperties();

            if (includeHeader)
            {
                sb.AppendLine(this.CreateCsvHeaderLine(properties));
            }

            foreach (var item in list)
            {
                sb.AppendLine(this.CreateCsvLine(item, properties));
            }

            return sb.ToString();
        }

        public string Write<T>(IList<T> list, string fileName, bool includeHeader = true)
        {
            string csv = this.Write(list, includeHeader);

            this.WriteFile(fileName, csv);

            return csv;
        }

        private string CreateCsvHeaderLine(PropertyInfo[] properties)
        {
            List<string> propertyValues = new List<string>();

            foreach (var prop in properties)
            {
                string stringformatString = string.Empty;
                string value = prop.Name;

                var attribute = prop.GetCustomAttribute(typeof(DisplayAttribute));
                if (attribute != null)
                {
                    value = (attribute as DisplayAttribute).Name;
                }

                this.CreateCsvStringItem(propertyValues, value);
            }

            return this.CreateCsvLine(propertyValues);
        }

        private string CreateCsvLine<T>(T item, PropertyInfo[] properties)
        {
            List<string> propertyValues = new List<string>();

            foreach (var prop in properties)
            {
                string stringformatString = string.Empty;
                object value = prop.GetValue(item, null);

                if (prop.PropertyType == typeof(string))
                {
                    this.CreateCsvStringItem(propertyValues, value);
                }
                else if (prop.PropertyType == typeof(string[]))
                {
                    this.CreateCsvStringArrayItem(propertyValues, value);
                }
                else if (prop.PropertyType == typeof(List<string>))
                {
                    this.CreateCsvStringListItem(propertyValues, value);
                }
                else
                {
                    this.CreateCsvItem(propertyValues, value);
                }
            }

            return this.CreateCsvLine(propertyValues);
        }

        private string CreateCsvLine(IList<string> list)
        {
            return string.Join(DELIMITER, list);
        }

        private void CreateCsvItem(List<string> propertyValues, object value)
        {
            if (value != null)
            {
                propertyValues.Add(value.ToString());
            }
            else
            {
                propertyValues.Add(string.Empty);
            }
        }

        private void CreateCsvStringListItem(List<string> propertyValues, object value)
        {
            string formatString = "\"{0}\"";
            if (value != null)
            {
                value = this.CreateCsvLine((List<string>)value);
                propertyValues.Add(string.Format(formatString, this.ProcessStringEscapeSequence(value)));
            }
            else
            {
                propertyValues.Add(string.Empty);
            }
        }

        private void CreateCsvStringArrayItem(List<string> propertyValues, object value)
        {
            string formatString = "\"{0}\"";
            if (value != null)
            {
                value = this.CreateCsvLine(((string[])value).ToList());
                propertyValues.Add(string.Format(formatString, this.ProcessStringEscapeSequence(value)));
            }
            else
            {
                propertyValues.Add(string.Empty);
            }
        }

        private void CreateCsvStringItem(List<string> propertyValues, object value)
        {
            string formatString = "\"{0}\"";
            if (value != null)
            {
                propertyValues.Add(string.Format(formatString, this.ProcessStringEscapeSequence(value)));
            }
            else
            {
                propertyValues.Add(string.Empty);
            }
        }

        private string ProcessStringEscapeSequence(object value)
        {
            return value.ToString().Replace("\"", "\"\"");
        }

        public bool WriteFile(string fileName, string csv)
        {
            bool fileCreated = false;

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                File.WriteAllText(fileName, csv);

                fileCreated = true;
            }

            return fileCreated;
        }

        public string[] ReadCSVFile(string fileName, string path)
        {
            var values = File.ReadAllLines(path + fileName);

            return values;
        }

        public List<CsvModel> FromCsvToCSVModel(string[] CsvStrings)
        {
            List<CsvModel> Items = new List<CsvModel>();
            foreach (string item in CsvStrings)
            {
                string[] values = item.Split(',');

                    CsvModel @object = new CsvModel
                {
                    VendorName = values[0].Trim(new char[] { '"', '\\' }),
                    ProductName = values[1].Trim(new char[] { '"', '\\' }),
                    Price = Convert.ToDouble(values[2])
                };
                Items.Add(@object);
            }
            return Items;
        }
    }
}
