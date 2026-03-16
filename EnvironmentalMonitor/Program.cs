using EnvironmentalMonitor.Services;
using System.Security.Cryptography.X509Certificates;

namespace EnvironmentalMonitor
{
    internal class Program
    {
        /// <summary>
        /// Serves as the entry point for the Environmental Monitor System application, providing a console interface
        /// for starting the server, generating a summary report, or exiting the application.
        /// </summary>
        /// <remarks>This method presents an interactive menu that allows users to start the environmental
        /// server, generate a summary report, or exit the application. The server can only be started once per session;
        /// subsequent attempts will notify the user if the server is already running. The method runs continuously
        /// until the user chooses to exit.</remarks>
        /// <returns></returns>
        static async Task Main()
        {
            EnvironmentalServer? server = null;
            Task? serverTask = null;

            /// The main loop of the application, which continuously displays a menu and processes user input until the user chooses to exit.
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Environmental Monitor System.");
                Console.WriteLine("1 - Start Server");
                Console.WriteLine("2 - Generate Summary Report");
                Console.WriteLine("3 - Exit");
                Console.WriteLine("Select option: [1-3]");

                string? choice = Console.ReadLine();

                // Process the user's menu selection and execute the corresponding action.
                switch (choice)
                {
                    case "1":
                        if (server == null)
                        {
                            server = new EnvironmentalServer(11000);
                            serverTask = server.Start();
                            Console.WriteLine("Server started.");

                            await serverTask;
                        }
                        else
                        {
                            Console.WriteLine("Server is already running.");
                        }
                        break;

                    case "2":
                        GenerateSummary();
                        break;

                    case "3":
                        Console.WriteLine("Exiting...");
                        return;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        /// <summary>
        /// Generates a summary report for a user-specified date range by prompting for start and end dates.
        /// </summary>
        /// <remarks>The method requests the start and end dates from the user in the 'yyyy-MM-dd' format.
        /// If the input is not a valid date, an error message is displayed and the report is not generated. The report
        /// is created using the SummaryReportService and covers the specified date range.</remarks>
        public static void GenerateSummary()
        {
            Console.WriteLine("Enter start date (yyyy-MM-dd): ");
            string startInput = Console.ReadLine();

            Console.WriteLine("Enter end date (yyyy-MM-dd): ");
            string endInput = Console.ReadLine();

            if (!DateTime.TryParse(startInput, out DateTime start) ||
                !DateTime.TryParse(endInput, out DateTime end))
            {
                Console.WriteLine("Invalid date format. Please use yyyy-MM-dd.");
                return;
            }

            end = end.AddDays(1); // include the full end date

            SummaryReportService summaryService = new SummaryReportService();
            summaryService.GenerateSummary(start, end);

            Console.WriteLine("Summary report generated (if data exists).");
        }
    }
}
