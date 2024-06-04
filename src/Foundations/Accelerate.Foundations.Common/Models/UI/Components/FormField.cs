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
        input, email, textarea, wysiwyg, basicWysiwyg, number, password, file, list, quotes, select, chips
    }
    public class FormField
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Name { get; set; }
        public string Type => Enum.GetName(FieldType);
        [JsonIgnore]
        public FormFieldTypes FieldType { get; set; }
        public bool? Disabled { get; set; }
        public bool? Hidden { get; set; }
        /// <summary>
        ///  If using a FileForm ensure multiple is flagged for multi select components
        /// </summary>
        public bool? Multiple { get; set; }
        public bool? IsArray { get; set; }
        public string? Icon { get; set; }
        public string? Class { get; set; }
        public string Placeholder { get; set; }
        public bool? Autocomplete { get; set; }
        public bool? AriaInvalid { get; set; }
        public bool? ClearOnSubmit { get; set; }
        public string? Accept { get; set; }
        public string Helper { get; set; }
        public object Value { get; set; }
        public List<string> Items { get; set; }
    }
}
