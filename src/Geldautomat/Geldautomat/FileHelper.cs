using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Geldautomat.DataObjects;

namespace Geldautomat
{
    /// <summary>
    /// Provides several helper functions for the interaction with a file
    /// </summary>
    internal static class FileHelper
    {
        /// <summary>
        /// Loads all available bank accounts
        /// </summary>
        /// <returns>The list with the bank accounts</returns>
        public static List<BankAccount> LoadBankAccounts()
        {
            // Get the path
            var path = GetFilePath();

            // Check if the file exists, if not, return an empty list
            if (!File.Exists(path))
                return new List<BankAccount>();

            // Load the content of the file
            var content = File.ReadAllLines(path);

            var result = new List<BankAccount>();
            // Iterate through every line and convert it into the needed data
            foreach (var line in content)
            {
                if (line.StartsWith("Id"))
                    continue;

                var lineContent = line.Split(";", StringSplitOptions.RemoveEmptyEntries);

                if (lineContent.Length != 5)
                    continue;

                var idx = 0;
                result.Add(new BankAccount
                {
                    Id = lineContent[idx++].ToUInt(),
                    Number = lineContent[idx++],
                    Pin = lineContent[idx++].ToUInt(),
                    Name = lineContent[idx++],
                    Amount = lineContent[idx].ToDecimal()
                });
            }

            // Return the result
            return result;
        }

        /// <summary>
        /// Saves the bank data
        /// </summary>
        /// <param name="data">The data (the list with the bank accounts)</param>
        /// <returns>true when successful, otherwise false</returns>
        public static bool SaveData(List<BankAccount> data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var result = new List<string>
            {
                "Id;Number;Pin;Name;Amount"
            };

            foreach (var entry in data)
            {
                result.Add($"{entry.Id};{entry.Number};{entry.Pin};{entry.Name};{entry.Amount}");
            }

            var path = GetFilePath();
            File.WriteAllLines(path, result);

            return File.Exists(path);
        }

        /// <summary>
        /// Returns the path of the bank data
        /// </summary>
        /// <returns>The path of the bank data</returns>
        private static string GetFilePath()
        {
            var mainPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(mainPath))
                throw new ArgumentException("Can't determine the base path.");

            return Path.Combine(mainPath, "BankData.csv");
        }

        /// <summary>
        /// Converts a string into an int
        /// </summary>
        /// <param name="value">The string value</param>
        /// <returns>The int value</returns>
        public static uint ToUInt(this string value)
        {
            return uint.TryParse(value, out var result) ? result : 0;
        }

        /// <summary>
        /// Converts a string into a decimal
        /// </summary>
        /// <param name="value">The string value</param>
        /// <returns>The decimal value</returns>
        public static decimal ToDecimal(this string value)
        {
            return decimal.TryParse(value, out var result) ? result : 0;
        }
    }
}
