import input from './fields/input.js'
import textarea from './fields/textarea.js'

export default function (data) {
	return {
    // PROPERTIES
    loading: false,
    fields: [],
    label: 'Submit',
    loading: false,
    event: null,
    postbackType: 'POST',
    localEvent: '__formAjax:completed',
    // INIT
    init() {
      this.label = data.label;
      this.event = data.event;
      this.postbackType = data.postbackType
    this.setHtml(data)
    this.localEvent += new Date().toISOString()

    this.$events.on(this.localEvent, (data) => {
        console.log(data)
        if (data.statusCode == 200) {
            //w
        }
        if (data.statusCode >= 400 && data.statusCode <= 500) {
            //this.resetValues(fields);
        }
        if (data.statusCode >= 500 && data.statusCode <= 600) {
            //this.resetValues(fields);
        }
    })
    },
    // METHODS
    renderField(field) {
      if(field.type == 'textarea') return textarea(field)
      
      return input(field)
    },
    async submit(fields) {
      this.loading = true;
      const payload = {}
      fields.map(x => {
        payload[x.name] = x.value
        return payload
      })
      let response = null;
      switch (this.postbackType) {
        case 'POST':
          response = await this.$fetch.POST(data.postbackUrl, payload);
          break;
        case 'PUT':
          response = await this.$fetch.PUT(data.postbackUrl, payload);
          // Expected output: "Mangoes and papayas are $2.79 a pound."
          break;
        case 'GET':
          response = await this.$fetch.GET(data.postbackUrl, payload);
          // Expected output: "Mangoes and papayas are $2.79 a pound."
          break;
        case 'DELETE':
          response = await this.$fetch.DELETE(data.postbackUrl, payload);
          // Expected output: "Mangoes and papayas are $2.79 a pound."
          break;
      }

      if(this.event) {
        this.$dispatch(this.event, response)
      }
      this.$dispatch(this.localEvent, response)
      this.loading = false;
    },
    resetValues(fields) {
      for(var i = 0; i < fields.length; i++) {
        fields[i].value = null;
      }
    },
    setHtml(data) {
      // make ajax request
      const label = data.label || 'Submit'
      this.fields = data.fields || []
      const html =  `
        <div>
          <progress x-show="loading"></progress>
          <fieldset x-data="formFields({fields})"></fieldset>
          <footer align="right">
            <button class="small" @click="await submit(fields)" :disabled="loading">${label}</button>
          </footer>
        </div>
        `
        this.$nextTick(() => {
            this.$root.innerHTML = html;
        })
    },
  }
}