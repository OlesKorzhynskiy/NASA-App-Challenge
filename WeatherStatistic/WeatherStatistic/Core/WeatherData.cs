using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using WeatherStatistic.Enums;

namespace WeatherStatistic.Core
{
    public class WeatherData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Strength { get; set; }
        public virtual int WeatherDataTypeId
        {
            get => (int)WeatherDataType;
            set => WeatherDataType = (WeatherDataTypeEnum)value;
        }
        [NotMapped]
        public WeatherDataTypeEnum WeatherDataType { get; set; }
    }
}