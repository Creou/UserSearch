using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace UserSearcher
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null)
            {
                OutputHelp();
                return;
            }

            if (args.Length < 1)
            {
                OutputHelp();
                return;
            }

            String searchData = args[0];

            if (searchData.StartsWith(@"/?"))
            {
                OutputHelp();
                return;
            }

            bool returnAllUsers = false;
            if (searchData.ToUpper().StartsWith(@"/A"))
            {
                returnAllUsers = true;
            }

            if (String.IsNullOrEmpty(searchData) && !returnAllUsers)
            {
                OutputHelp();
                return;
            }

            bool searchName = true;
            bool searchFullName = true;
            bool searchSid = true;
            bool searchDescription = true;

            if (args.Length >= 2)
            {
                String switches = args[1];
                switches = switches.ToUpper();
                if (switches.StartsWith(@"/C:"))
                {
                    searchName = false;
                    searchFullName = false;
                    searchSid = false;
                    searchDescription = false;

                    switches = switches.Replace(@"/C:", String.Empty);

                    if (switches.Contains("N"))
                    {
                        searchName = true;
                    }
                    if (switches.Contains("F"))
                    {
                        searchFullName = true;
                    }
                    if (switches.Contains("D"))
                    {
                        searchDescription = true;
                    }
                    if (switches.Contains("S"))
                    {
                        searchSid = true;
                    }
                }
                else
                {
                    OutputHelp();
                    return;
                }
            }

            try
            {
                if (returnAllUsers || searchName || searchFullName || searchDescription || searchSid)
                {
                    Console.WriteLine("Loading data...");

                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_UserAccount");
                    ManagementObjectCollection results = searcher.Get();

                    Console.WriteLine("Searching...");
                    if (returnAllUsers)
                    {
                        Console.WriteLine("    Display all");
                    }
                    else
                    {
                        Console.WriteLine("    " + searchData);

                        StringBuilder searchingInfo = new StringBuilder();

                        searchingInfo.Append("    Categories - ");
                        if (searchName)
                        {
                            searchingInfo.Append("Name ");
                        }
                        if (searchFullName)
                        {
                            searchingInfo.Append("Fullname ");
                        }
                        if (searchDescription)
                        {
                            searchingInfo.Append("Description ");
                        }
                        if (searchSid)
                        {
                            searchingInfo.Append("SID ");
                        }

                        Console.WriteLine(searchingInfo);
                    }

                    Console.WriteLine();

                    searchData = searchData.ToUpper();

                    bool found = false;

                    foreach (ManagementObject queryObj in results)
                    {
                        bool nameMatch = ((string)queryObj["Name"]).ToUpper().Contains(searchData);
                        bool fullNameMatch = ((string)queryObj["FullName"]).ToUpper().Contains(searchData);
                        bool sidMatch = ((string)queryObj["SID"]).ToUpper().Contains(searchData);
                        bool descriptionMatch = ((string)queryObj["Description"]).ToUpper().Contains(searchData);

                        if (
                            returnAllUsers || 
                                (
                                    (nameMatch && searchName) ||
                                    (fullNameMatch && searchFullName) ||
                                    (descriptionMatch && searchDescription) ||
                                    (sidMatch && searchSid)
                                )
                           )
                        {
                            found = true;
                            Console.WriteLine("-----------------------------------");
                            Console.WriteLine("Win32_UserAccount instance");
                            Console.WriteLine("-----------------------------------");
                            Console.WriteLine("AccountType:         {0}", queryObj["AccountType"]);
                            Console.WriteLine("Caption:             {0}", queryObj["Caption"]);
                            Console.WriteLine("Description:         {0}", queryObj["Description"]);
                            Console.WriteLine("Disabled:            {0}", queryObj["Disabled"]);
                            Console.WriteLine("Domain:              {0}", queryObj["Domain"]);
                            Console.WriteLine("FullName:            {0}", queryObj["FullName"]);
                            Console.WriteLine("InstallDate:         {0}", queryObj["InstallDate"]);
                            Console.WriteLine("LocalAccount:        {0}", queryObj["LocalAccount"]);
                            Console.WriteLine("Lockout:             {0}", queryObj["Lockout"]);
                            Console.WriteLine("Name:                {0}", queryObj["Name"]);
                            Console.WriteLine("PasswordChangeable:  {0}", queryObj["PasswordChangeable"]);
                            Console.WriteLine("PasswordExpires:     {0}", queryObj["PasswordExpires"]);
                            Console.WriteLine("PasswordRequired:    {0}", queryObj["PasswordRequired"]);
                            Console.WriteLine("SID:                 {0}", queryObj["SID"]);
                            Console.WriteLine("SIDType:             {0}", queryObj["SIDType"]);
                            Console.WriteLine("Status:              {0}", queryObj["Status"]);
                            Console.WriteLine();
                        }
                    }

                    Console.WriteLine("Search complete.");

                    if (!found)
                    {
                        Console.WriteLine();
                        Console.WriteLine("No matches found.");
                    }
                }
                else
                {
                    Console.WriteLine("You must specify at least one category to search.");
                    Console.WriteLine();
                    OutputHelp();
                }
            }
            catch (ManagementException e)
            {
                Console.WriteLine("An error occurred while querying for WMI data: " + e.Message);
                Console.WriteLine();
                OutputHelp();
            }

#if DEBUG
            Console.ReadLine();
#endif
        }

        public static void OutputHelp()
        {
            Console.WriteLine(@"User Data Searcher");
            Console.WriteLine(@"==================");
            Console.WriteLine();
            Console.WriteLine(@"Usage:");
            Console.WriteLine(@"    usersearch [/? | /A | searchdata] [/C:][N][F][D][S]");
            Console.WriteLine();
            Console.WriteLine(@"    /A - Return all users");
            Console.WriteLine();
            Console.WriteLine(@"    /? - Display this help");
            Console.WriteLine();
            Console.WriteLine(@"    searchdata  - The data to search users for (case insensitive)");
            Console.WriteLine();
            Console.WriteLine(@"    /C - Specify the cateogies to search (omit to search all categories)");
            Console.WriteLine();
            Console.WriteLine(@"         N = Name");
            Console.WriteLine(@"         F = Full Name");
            Console.WriteLine(@"         D = Description");
            Console.WriteLine(@"         S = SID");
            Console.WriteLine();
            Console.WriteLine(@"Examples:");
            Console.WriteLine();
            Console.WriteLine(@"    Search all categories for ""smith""");
            Console.WriteLine(@"     usersearch smith");
            Console.WriteLine();
            Console.WriteLine(@"    Search full name for ""bob""");
            Console.WriteLine(@"     usersearch bob /C:F");
            Console.WriteLine();
            Console.WriteLine(@"    Search full name and description for ""jim""");
            Console.WriteLine(@"     usersearch jim /C:FD");
            Console.WriteLine();
            Console.WriteLine(@"    Display all users");
            Console.WriteLine(@"     usersearch /A");
            Console.WriteLine();
#if DEBUG
            Console.ReadLine();
#endif
        }
    }


}
