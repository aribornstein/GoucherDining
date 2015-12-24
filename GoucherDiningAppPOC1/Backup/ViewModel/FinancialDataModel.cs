using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Globalization;
using System.Windows.Resources;

namespace StockDemo
{
    public class FinancialDataModel
    {
        private static FinancialDataModel instance = new FinancialDataModel();

        private List<OhlcModel> dailyData;
        private List<OhlcModel> weeklyData;
        private List<OhlcModel> monthlyData;

        private FinancialDataModel()
        {
        }

        public static FinancialDataModel Instance
        {
            get
            {
                return instance;
            }
        }

        public List<OhlcModel> DailyData
        {
            get
            {
                if (dailyData == null)
                {
                    dailyData = new List<OhlcModel>();
                    LoadData(dailyData, "Daily");
                }

                return dailyData;
            }
        }

        public List<OhlcModel> WeeklyData
        {
            get
            {
                if (weeklyData == null)
                {
                    weeklyData = new List<OhlcModel>();
                    LoadData(weeklyData, "Weekly");
                }

                return weeklyData;
            }
        }

        public List<OhlcModel> MonthlyData
        {
            get
            {
                if (monthlyData == null)
                {
                    monthlyData = new List<OhlcModel>();
                    LoadData(monthlyData, "Monthly");
                }

                return monthlyData;
            }
        }

private static void LoadData(List<OhlcModel> list, string fileName)
{
    StreamResourceInfo resource = Application.GetResourceStream(new Uri("ViewModel/" + fileName + ".txt", UriKind.RelativeOrAbsolute));
    using (StreamReader reader = new StreamReader(resource.Stream))
    {
        string line = string.Empty;
        while (!string.IsNullOrEmpty(line = reader.ReadLine()))
        {
            string[] values = line.Split('\t');
            double[] args = new double[values.Length - 1];

            // first argument is the Date, start from the second splitted value
            for(int i = 1; i < values.Length; i++)
            {
                args[i - 1] = double.Parse(values[i], CultureInfo.InvariantCulture);
            }

            OhlcModel model = new OhlcModel(args);
            model.Date = DateTime.Parse(values[0], CultureInfo.InvariantCulture);

            list.Add(model);
        }
    }
}
    }
}
