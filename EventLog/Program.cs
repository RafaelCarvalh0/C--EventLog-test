using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading;


class MySample
{

    public static void Main()
    {
        //PARA ESCREVER LOGS PERSONALIZADOS NO WINDOWS
        int idLog = 0; //Gera um ID para o Log
        string MySource = "Exception 10"; //Escreve a mensagem do Log
        string Log = "Teste"; //Escreve no tipo de log (Application, Setup, etc... ** OU CRIO ALGUM NOVO** )
        string Event = "Erro de retorno do banco de dados"; //Escreve uma descrição para o log inserido

        try
        {
            if (!EventLog.SourceExists(MySource))
            {
                EventLog.CreateEventSource(MySource, Log);

                EventLog.WriteEntry(MySource, Event, EventLogEntryType.Information, idLog);

                Console.WriteLine("CreatedEventSource --> " + Event);
                Console.WriteLine("Exiting, execute the application a second time to use the source.");
                // The source is created.  Exit the application to allow it to be registered.
                return;
                //}

                // Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog();

                myLog.Source = MySource;

                // Write an informational entry to the event log.
                myLog.WriteEntry(Event);          

            }

            DisplayEventLogProperties();
   
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
        
    }

    static void DisplayEventLogProperties()
    {
        // Iterate through the current set of event log files,
        // displaying the property settings for each file.

        EventLog[] eventLogs = EventLog.GetEventLogs();
        foreach (EventLog e in eventLogs)
        {
            Console.WriteLine($"Nome do registro: {e.Log}"); 
            Console.WriteLine($"Nome do registro: {e.Source}");
            if(e.LogDisplayName == "Application")
            {
         
            Int64 sizeKB = 0;

            Console.WriteLine();
            Console.WriteLine("{0}:", e.LogDisplayName);
            Console.WriteLine("  Log name = \t\t {0}", e.Log);

            Console.WriteLine("  Number of event log entries = {0}", e.Entries.Count.ToString());

            // Determine if there is an event log file for this event log.
            RegistryKey regEventLog = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Services\\EventLog\\" + e.Log);
            if (regEventLog != null)
            {
                Object temp = regEventLog.GetValue("File");
                if (temp != null)
                {
                    Console.WriteLine("  Log file path = \t {0}", temp.ToString());
                    FileInfo file = new FileInfo(temp.ToString());

                    // Get the current size of the event log file.
                    if (file.Exists)
                    {
                        sizeKB = file.Length / 1024;
                        if ((file.Length % 1024) != 0)
                        {
                            sizeKB++;
                        }
                        Console.WriteLine("  Current size = \t {0} kilobytes", sizeKB.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("  Log file path = \t <not set>");
                }
            }

            // Display the maximum size and overflow settings.

            sizeKB = e.MaximumKilobytes;
            Console.WriteLine("  Maximum size = \t {0} kilobytes", sizeKB.ToString());
            Console.WriteLine("  Overflow setting = \t {0}", e.OverflowAction.ToString());

            switch (e.OverflowAction)
            {
                case OverflowAction.OverwriteOlder:
                    Console.WriteLine("\t Entries are retained a minimum of {0} days.",
                        e.MinimumRetentionDays);
                    break;
                case OverflowAction.DoNotOverwrite:
                    Console.WriteLine("\t Older entries are not overwritten.");
                    break;
                case OverflowAction.OverwriteAsNeeded:
                    Console.WriteLine("\t If number of entries equals max size limit, a new event log entry overwrites the oldest entry.");
                    break;
                default:
                    break;
            }

            }
        }
    }
}

