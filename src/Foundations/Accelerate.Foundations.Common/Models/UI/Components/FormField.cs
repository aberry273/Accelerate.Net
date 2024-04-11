using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Accelerate.Foundations.Common.Models.UI.Components
{
    public enum FormFieldTypes
    {
        input, email, textarea, number, password, file
    }
    public class FormField
    {
        public string Label { get; set; }
        public string Name { get; set; }
        public string Type => Enum.GetName(FieldType);
        [JsonIgnore]
        public FormFieldTypes FieldType { get; set; }
        public bool? Disabled { get; set; }
        public bool? Hidden { get; set; }
        public string Placeholder { get; set; }
        public bool? Autocomplete { get; set; }
        public bool? AriaInvalid { get; set; }
        public bool? ClearOnSubmit { get; set; }
        public string? Accept { get; set; }
        public string Helper { get; set; }
        public object Value { get; set; }
    }
}
