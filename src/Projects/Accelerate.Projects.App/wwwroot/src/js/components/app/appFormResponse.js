
export default function (data) {
	return {
    // PROPERTIES
    loading: false,
    fields: [],
    label: 'Submit',
    // INIT
    init() {
      this.label = data.label;
      this.event = data.event;
      this.postbackType = data.postbackType
      this.fields = data.fields
      this.setHtml(data)
    }, 
    
     async submit(fields) {
      this.loading = true;
      const payload = {}
      fields.map(x => {
        payload[x.name] = x.value
        return payload
      })
        let response = this.$fetch.POST(data.postbackUrl, payload);
        
      if(this.event) {
        this.$dispatch(this.event, response)
      }
      this.resetValues(fields);
      this.loading = false;
    },
    resetValues(fields) {
        for (var i = 0; i < fields.length; i++) {
        if (fields[i].clearOnSubmit === true)
            fields[i].value = null;
      }
    },
    format(type) {

    },
    setHtml(data) {
      // make ajax request
      const label = data.label || 'Submit'
      const html = `
        <div>
          <progress x-show="loading"></progress>
          <fieldset x-data="formFields({fields})"></fieldset>
          <footer align="right" style="text-align:right">
          <!--
            <fieldset role="group">
              <input name="Tag" type="text" placeholder="#tags" autocomplete="email" />
              <button class="small" @click="await submit(fields)" :disabled="loading">${label}</button>
            </fieldset>
            -->
            <button class="flat" @click="await submit(fields)" :disabled="loading">${label}</button>
          </footer>
        </div>
      `
      this.$nextTick(() => {
        this.$root.innerHTML = html
      });
    },
  }
}