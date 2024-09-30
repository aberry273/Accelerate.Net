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
        //        input, email, textarea, wysiwyg, basicWysiwyg, link, number, password, file, list, quotes, select, chips
        input, email, textarea, number, password, file, Image, Video, select
    }
    public enum FormFieldComponents
    {
        aclFieldInput, aclFieldTextarea, aclFieldContentEditable, aclFieldCodeEditor, aclFieldEditorJs, aclFieldSelect, aclFieldSwitch, aclFieldFile, aclFieldSelectCheckbox
    }
    public class FormField
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Event { get; set; }
        public string Name { get; set; }
        public string Type => Enum.GetName(FieldType);
        public string Component => Enum.GetName(FieldComponent);
        [JsonIgnore]
        public FormFieldComponents FieldComponent { get; set; }
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
        public int? Min { get; set; }
        public int? Max { get; set; }
        public bool? Autocomplete { get; set; }
        public bool? AriaInvalid { get; set; }
        public bool? ClearOnSubmit { get; set; }
        public bool? AreItemsObject { get; set; } = false;
        public string? Accept { get; set; }
        public string Helper { get; set; }
        public object Value { get; set; }
        public List<dynamic> Items { get; set; }
    }
}
