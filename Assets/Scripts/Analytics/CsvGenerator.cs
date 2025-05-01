using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public class CsvGenerator<T>
{
    public List<string> GenerateCsv(IEnumerable<T> data, string filePath)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var header = string.Join(",", properties.Select(p => p.Name));

        var csvLines = new List<string> { header };

        foreach (var item in data)
        {
            var values = new List<string>();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(item);
                values.Add(value != null ? value.ToString() : string.Empty);
            }
            csvLines.Add(string.Join(",", values));
        }
        return csvLines;
    }
}