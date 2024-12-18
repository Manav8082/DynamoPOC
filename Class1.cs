using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace DynomaoPOC
{
    //public class Class1
    //{
        
    //}
    
    public class Class1
    {
        public static Dictionary<string, Dictionary<string, int>> AggregateData(List<ExchangeData> data)
        {
            // Group by ModelName, then by ExchangeName and sum the Time for each combination
            var result = data
                .Where(d => d.Operation == "UpdateExchangeAsync" || d.Operation == "UpdateExchangeAsync:GenerateViewableAsync")
                .GroupBy(d => d.ModelName)
                .ToDictionary(
                    modelGroup => modelGroup.Key,
                    modelGroup => modelGroup
                        .GroupBy(d => d.ExchangeName)
                        .ToDictionary(
                            exchangeGroup => exchangeGroup.Key,
                            exchangeGroup => exchangeGroup.Sum(d => d.Time)
                        )
                );

            return result;
        }
        public static List<ExchangeData> LoadExchangeDataFromCSV(string filePath)
        {
            var dataList = new List<ExchangeData>();

            try
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines.Skip(1)) // Skip the header row
                {
                    var columns = line.Split(',');

                    var operation = columns[0].Trim();
                    var time = int.Parse(columns[1].Trim());
                    var events = int.Parse(columns[2].Trim());
                    var exchangeName = columns[3].Trim();
                    var modelName = columns[4].Trim();
                    var executionTime = string.IsNullOrEmpty(columns[5].Trim()) ? 0 : int.Parse(columns[5].Trim());

                    var exchangeData = new ExchangeData(operation, time, events, exchangeName, modelName, executionTime);
                    dataList.Add(exchangeData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
            }

            return dataList;
        }
    }
    public class ExchangeData
    {
        public string Operation { get; set; }
        public int Time { get; set; }
        public int Events { get; set; }
        public string ExchangeName { get; set; }
        public string ModelName { get; set; }
        public int ExecutionTime { get; set; }

        public ExchangeData(string operation, int time, int events, string exchangeName, string modelName, int executionTime)
        {
            Operation = operation;
            Time = time;
            Events = events;
            ExchangeName = exchangeName;
            ModelName = modelName;
            ExecutionTime = executionTime;
        }
    }
}
