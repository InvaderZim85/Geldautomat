using System;
using System.Collections.Generic;
using System.Linq;
using Geldautomat.DataObjects;

namespace Geldautomat
{
    /// <summary>
    /// Provides the main logic of the program
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Contains the value which indicates if the application should be closed
        /// </summary>
        private static bool _closeApplication;

        /// <summary>
        /// Contains the list with the bank accounts
        /// </summary>
        private static List<BankAccount> _bankAccounts;

        /// <summary>
        /// Contains the current selected account
        /// </summary>
        private static BankAccount _currentAccount;

        /// <summary>
        /// The main entry point of the program
        /// </summary>
        private static void Main()
        {
            Console.Title = "TestBank est. 2020";

            // Load the bank data
            try
            {
                _bankAccounts = FileHelper.LoadBankAccounts();

                while (true)
                {
                    Console.Clear();
                    PrintHeader();

                    _currentAccount = GetBankAccount();

                    if (_closeApplication)
                        return;

                    if (_currentAccount != null)
                    {
                        Console.WriteLine();

                        DoOperation();

                        FileHelper.SaveData(_bankAccounts);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occured: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Application was closed. Press enter to continue...");
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Ask the user for an account and returns them if it's correct or the user want's to exit
        /// </summary>
        /// <returns>The desired bank account</returns>
        private static BankAccount GetBankAccount()
        {
            var exitNumber = false;
            var exitPin = false;
            var count = 0;
            BankAccount account = null;

            while (!exitNumber)
            {
                Console.Write("Please enter your account number: ");
                var number = Console.ReadLine();

                if (string.IsNullOrEmpty(number))
                {
                    Console.WriteLine("\r\nError > Invalid input. Please try again. Press enter to continue...");
                    Console.ReadLine();
                    count++;

                    // If the user entered the account number three times wrong, exit
                    if (count >= 3)
                        return null;

                    continue;
                }

                if (number.Equals("exit"))
                {
                    _closeApplication = true;
                    return null;
                }

                account = _bankAccounts.FirstOrDefault(f => f.Number.Equals(number, StringComparison.OrdinalIgnoreCase));

                if (account == null)
                {
                    Console.WriteLine("\r\nError > The entered account number was not found. Please try again. Press enter to continue...");
                    Console.ReadLine();
                    count++;

                    if (count >= 3)
                        return null;
                }
                else
                {
                    exitNumber = true;
                }
            }

            count = 0;
            while (!exitPin)
            {
                Console.Write("Please enter you pin: ");
                var pin = GetPin();

                if (account.Pin != pin)
                {
                    Console.WriteLine("\r\nError > The entered pin was wrong. Please try again. Press enter to continue...");
                    Console.ReadLine();
                    count++;

                    if (count >= 0)
                        return null;
                }
                else
                {
                    exitPin = true;
                }
            }

            return account;
        }

        /// <summary>
        /// Gets the pin
        /// </summary>
        /// <returns>The pin of the user</returns>
        private static uint GetPin()
        {
            var pass = string.Empty;
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    Console.Write("\b \b");
                    pass = pass[..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    pass += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);

            return pass.ToUInt();
        }

        /// <summary>
        /// Asks the user which operation he / she want's to execute
        /// </summary>
        private static void DoOperation()
        {
            var exit = false;

            while (!exit)
            {
                Console.Clear();
                PrintHeader();

                Console.WriteLine($"Account: {_currentAccount.Name}");
                Console.WriteLine();
                Console.WriteLine("Please select your desired operation: " +
                                  "\r\n- 1 = Show current amount" +
                                  "\r\n- 2 = Take out money" +
                                  "\r\n- 3 = Deposit money" +
                                  "\r\n- 9 = Exit");

                Console.Write("Option: ");
                var input = Console.ReadLine();
                var number = input.ToUInt();
                Console.WriteLine();

                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("\r\nError > Wrong input. Please try again. Press enter to continue...");
                    Console.ReadLine();
                    continue;
                }

                if (number == 0)
                {
                    Console.WriteLine("\r\nError > Wrong input. Please try again. Press enter to continue...");
                    Console.ReadLine();
                    continue;
                }

                if (number == 9)
                {
                    exit = true;
                    continue;
                }

                PerformOperation(number);
            }
        }

        /// <summary>
        /// Performs the desired operation
        /// </summary>
        /// <param name="operation">The number of the desired operation</param>
        private static void PerformOperation(uint operation)
        {
            if (operation == 1)
            {
                Console.WriteLine($"Current amount: {_currentAccount.Amount:N2}");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
            else
            {
                var exit = false;
                while (!exit)
                {
                    Console.WriteLine($"Available amount: {_currentAccount.Amount:N2}");
                    Console.Write("Enter the desired amount: ");
                    var input = Console.ReadLine();

                    if (string.IsNullOrEmpty(input))
                    {
                        Console.WriteLine("\r\nError > Invalid input. Please try again. Press enter to continue...");
                        Console.ReadLine();
                        continue;
                    }

                    var value = input.ToDecimal();
                    if (value == 0)
                    {
                        Console.WriteLine("\r\nError > The entered amount is 0 or not valid. Press enter to continue...");
                        Console.ReadLine();
                        continue;
                    }

                    if (operation == 2 && _currentAccount.Amount < value)
                    {
                        Console.WriteLine(
                            $"\r\nError > The entered amount is greater than the available amount. Current amount: {_currentAccount.Amount:N2}. Please try again. Press enter to continue...");
                        Console.ReadLine();
                        continue;
                    }

                    if (operation == 2)
                    {
                        _currentAccount.Amount -= value;
                    }
                    else
                    {
                        _currentAccount.Amount += value;
                    }

                    Console.WriteLine($"New amount: {_currentAccount.Amount:N2}");
                    Console.WriteLine("Press enter to continue...");
                    Console.ReadLine();

                    exit = true;
                }
            }
        }

        /// <summary>
        /// Prints the header
        /// </summary>
        private static void PrintHeader()
        {
            Console.WriteLine("+--------------------+");
            Console.WriteLine("| TestBank est. 2020 |");
            Console.WriteLine("+--------------------+");
            Console.WriteLine();
        }
    }
}
