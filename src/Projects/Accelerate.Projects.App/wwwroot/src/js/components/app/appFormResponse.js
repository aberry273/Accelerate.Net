
export default function (data) {
	return {
    // PROPERTIES
    loading: false,
    fields: [],
    item: null,
    label: 'Submit',
    // INIT
    init() {
      this.label = data.label;
      this.event = data.event;
      this.item = data.item;
      this.postbackType = data.postbackType
      this.fields = data.fields,
      this.setHtml(data)
    },
    setFields(inputName, inputPlaceholder) {
      return [
          {
            name: inputName || 'Content',
            type: 'textarea',
            placeholder: inputPlaceholder || 'Whats your update?',
            autocomplete: null,
            helper: '',
            clearOnSubmit: true,
          },
          {
            name: 'parentId',
            type: 'input',
            disabled: true,
            hidden: true,
            autocomplete: null,
            helper: '',
            value: this.item.id,
          },
          {
            name: 'status',
            type: 'input',
            disabled: true,
            hidden: true,
            autocomplete: null,
            helper: '',
            value: this.item.status,
          },
          {
            name: 'channelId',
            type: 'input',
            disabled: true,
            hidden: true,
            autocomplete: null,
            helper: '',
            value: this.item.channelId,
          },
          {
            name: 'threadId',
            type: 'input',
            disabled: true,
            hidden: true,
            autocomplete: null,
            helper: '',
            value: this.item.threadId,
          },
          {
            name: 'tags',
            type: 'input',
            disabled: true,
            hidden: true,
            autocomplete: null,
            helper: '',
            value: this.item.tags,
          },
          {
            name: 'category',
            type: 'input',
            disabled: true,
            hidden: true,
            autocomplete: null,
            helper: '',
            value: this.item.category,
          },
          {
            name: 'userId',
            type: 'input',
            disabled: true,
            hidden: true,
            autocomplete: null,
            helper: '',
            value: this.item.userId,
          },
        ]
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
      for(var i = 0; i < fields.length; i++) {
        if(fields[i].clearOnSubmit)
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
            <button class="small" @click="await submit(fields)" :disabled="loading">${label}</button>
          </footer>
        </div>
      `
      this.$nextTick(() => {
        this.$root.innerHTML = html
      });
    },
  }
}