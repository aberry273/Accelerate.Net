export default function (data) {
    return {
        // PROPERTIES
        mxForm_data: {},
        mxForm_fields: [],
        mxForm_method: 'POST',
        mxForm_postbackUrl: '/api',
        mxForm_label: 'Submit',
        mxForm_class: '',
        mxForm_submit: null,
        init() {
        },
        // GETTERS
        get mxForm_submitClass() { return 'inline-block w-full px-5 py-4 text-lg font-medium text-center text-white transition duration-200 bg-blue-600 rounded-lg hover:bg-blue-700 ease' },
        get mxForm_submitInvalidClass() { return 'group-invalid:pointer-events-none group-invalid:opacity-30' },
        get mxForm_formClass() { return 'flex flex-col items-start justify-start w-full h-full p-10 lg:p-16 xl:p-24' },
        get mxForm_formPadlessClass() { return 'flex flex-col items-start justify-start w-full h-full p-4' },
        get mxForm_FileFormHeaders() {
          return {
            'Accept': '*/*',
          }
        },
        // METHODS
        _mxForm_SetFieldValue(fields, field) {
            for(let i = 0; i < fields.length; i++)
            {
                if (fields[i].name == field.name){
                    fields[i].value = field.value;
                }
            } 
        },
        async _mxForm_SubmitAjaxRequest(action, postbackUrl, payload, config, isJson) {
            let response = null;
            switch (action) {
              case 'POST':
                response = await this.$fetch.POST(postbackUrl, payload, config, isJson);
                break;
              case 'PUT':
                response = await this.$fetch.PUT(postbackUrl, payload, config, isJson);
                break;
              case 'GET':
                response = await this.$fetch.GET(postbackUrl, payload, config, isJson);
                break;
              case 'DELETE':
                response = await this.$fetch.DELETE(postbackUrl);
                break;
              default:
                throw new Error(`_mxForm_SubmitAjaxRequest expected {action} with type: POST, PUT, GET, DELETE`);
            }
        },
        toCamelCase(str) {
          return str.toLowerCase().replace(/[^a-zA-Z0-9]+(.)/g, (m, chr) => chr.toUpperCase());
        },
        _mxForm_GetFormData(form, ignoreNull = true, flattenPayload = false) {
          if (!form) return {};
          const self = this;
          const payload = {};
          form.fields.map((x) => {
            const name = this.toCamelCase(x.name); 
            if(ignoreNull && !x.value) return;
            payload[name] = x.value;
          });
          if (!form.sections) {
            return payload;
          }
          // form section fields
          // If flattenPayload is set, then set all data to the root object
          // Else created objects per section within the payload
          if (flattenPayload) {
            form.sections.map((x) => {
              x.fields.map((y) => {
                const fieldName = y.name.replace(/\s/g, '');
                if(ignoreNull && !y.value) return;
                payload[fieldName] = y.value;
              });
            });
          } else {
            form.sections.map((x) => {
              const sectionName = x.title.replace(/\s/g, '');
              payload[sectionName] = {};

              x.fields.map((y) => {
                const fieldName = y.name.replace(/\s/g, '');    
                if(ignoreNull && !y.value) return;
                payload[sectionName][fieldName] = y.value;
              });
            });
          }
          return payload;
        },
        // Does not allow for sectioned formData
        _mxForm_GetFileFormData(form) {
          if (!form) return new FormData();
          const self = this;
          const formData = new FormData();
          if (form.fields) { 
            for (var i = 0; i < form.fields.length; i++) {
              var x = form.fields[i];
              if (x.value == null) continue;
              const name = x.name.replace(/\s/g, '');
              if (x.multiple) {
                for (var j = 0; j < x.value.length; j++) {
                  const value = x.value[j];
                  if (this._mxForm_isFieldValueObject(value)) {
                    //Serialize object if JSON
                    formData.append(name, JSON.stringify(value));
                  }
                  else {
                    formData.append(name, value);
                  }
                }
                continue;
              }
              else {
                const value = x.value;
                if (this._mxForm_isFieldValueObject(value)) {
                  //Serialize object if JSON
                  formData.append(name, JSON.stringify(value));
                }
                else {
                  formData.append(name, value);
                }
              }
            }
          }
          
          return formData;
        },
        _mxForm_isFieldValueObject(value) {
          const type = this._mxForm_CheckDataType(value);
          return (type == 'object');
        },
        _mxForm_CheckDataType(data) {
          var objectConstructor = ({}).constructor;
          if (data.constructor === objectConstructor) {
            return "object";
          }
          return "string";
        },
    }
}