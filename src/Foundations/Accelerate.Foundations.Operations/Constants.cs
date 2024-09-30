using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Operations
{
    public struct Constants
    {
        public struct Defaults
        {
        }
        public struct Config
        {
            public const string Environment = "Environment";
            public const string ConfigName = "OperationsConfiguration";
            public const string LocalDatabaseKey = "LocalOperationsContext";
            public const string DatabaseKey = "OperationsContext";
        }
        public struct Settings
        {
            public enum ActionTypes
            {
                Postback, Email, BulkEmail, CsvImport, RssRead
            }
            public static string[] Actions => new string[]
            {
                ActionTypes.BulkEmail.ToString(),
                ActionTypes.Email.ToString(),
                ActionTypes.Postback.ToString(),
                ActionTypes.CsvImport.ToString(),
                ActionTypes.RssRead.ToString(),
            };
            public static KeyValuePair<string, string>[] Schedules => new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>(nameof(ScheduleFormats.EveryMinuteTestSchedule), ScheduleFormats.EveryMinuteTestSchedule),
				// Daily
				new KeyValuePair<string, string>(nameof(ScheduleFormats.DailyFormats.DailyAt9PM), ScheduleFormats.DailyFormats.DailyAt9PM),
                new KeyValuePair<string, string>(nameof(ScheduleFormats.DailyFormats.DailyAt6PM), ScheduleFormats.DailyFormats.DailyAt6PM),
                new KeyValuePair<string, string>(nameof(ScheduleFormats.DailyFormats.DailyAt9AM), ScheduleFormats.DailyFormats.DailyAt9AM),
                new KeyValuePair<string, string>(nameof(ScheduleFormats.DailyFormats.DailyAt6AM), ScheduleFormats.DailyFormats.DailyAt6AM),
                new KeyValuePair<string, string>(nameof(ScheduleFormats.DailyFormats.WeekdaysAt9PM), ScheduleFormats.DailyFormats.WeekdaysAt9PM),
                new KeyValuePair<string, string>(nameof(ScheduleFormats.DailyFormats.WeekdaysAt6PM), ScheduleFormats.DailyFormats.WeekdaysAt6PM),
                new KeyValuePair<string, string>(nameof(ScheduleFormats.DailyFormats.WeekdaysAt9AM), ScheduleFormats.DailyFormats.WeekdaysAt9AM),
                new KeyValuePair<string, string>(nameof(ScheduleFormats.DailyFormats.WeekdaysAt6AM), ScheduleFormats.DailyFormats.WeekdaysAt6AM),
				// Weekly
				new KeyValuePair<string, string>(nameof(ScheduleFormats.WeeklyFormats.WeeklySunday6AM), ScheduleFormats.WeeklyFormats.WeeklySunday6AM),
                new KeyValuePair<string, string>(nameof(ScheduleFormats.WeeklyFormats.WeeklySunday9AM), ScheduleFormats.WeeklyFormats.WeeklySunday9AM),
                new KeyValuePair<string, string>(nameof(ScheduleFormats.WeeklyFormats.WeeklySunday6PM), ScheduleFormats.WeeklyFormats.WeeklySunday6PM),
                new KeyValuePair<string, string>(nameof(ScheduleFormats.WeeklyFormats.WeeklySunday9PM), ScheduleFormats.WeeklyFormats.WeeklySunday9PM),
            };
            /*
             * http://www.quartz-scheduler.org/documentation/quartz-2.3.0/tutorials/crontrigger.html
            */
            public struct ScheduleFormats
            {
                public const string EveryMinuteTestSchedule = "0 0/1 * * * ?";
                public struct DailyFormats
                {
                    public const string DailyAt9PM = "0 0 21 * * ?";
                    public const string DailyAt6PM = "0 0 17 * * ?";
                    public const string DailyAt9AM = "0 0 9 * * ?";
                    public const string DailyAt6AM = "0 0 6 * * ?";
                    public const string WeekdaysAt9PM = "0 0 21 ? * ?";
                    public const string WeekdaysAt6PM = "0 0 17 ? * ?";
                    public const string WeekdaysAt9AM = "0 0 9 ? * ?";
                    public const string WeekdaysAt6AM = "0 0 9 ? * ?";
                }
                public struct WeeklyFormats
                {
                    public const string WeeklySunday6AM = "0 0 9 * 1 ?";
                    public const string WeeklySunday9AM = "0 0 6 * 1 ?";
                    public const string WeeklySunday6PM = "0 0 17 * 1 ?";
                    public const string WeeklySunday9PM = "0 0 21 * 1 ?";
                }
            }
        }
    }
}
