
export default function (data) {
	return {
    // PROPERTIES
    loading: false,
    fields: [],
    item: null,
    label: 'Submit',
    tagStr: null,
    tags: [],
    showTags: false,
    showText: false,
    showVideo: false,
    showImage: false,
    // INIT
    init() {
      this.tags = [];
      this.label = data.label;
      this.event = data.event;
      this.item = data.item;
      this.postbackType = data.postbackType
      this.fields = data.fields,
      this.setHtml(data)
    },
    // GETTERS
    get typeSelected() { return this.showImage || this.showVideo || this.showText },
    // METHODS
    async submit(fields) {
      this.loading = true;
      const payload = {}
      fields.map(x => {
        payload[x.name] = x.value
        return payload
      })
      // Set tags
      fields.tags = this.tags.join(',')

      let response = await this.$fetch.POST(data.postbackUrl, payload);
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
    cancelTypes() {
      this.showText = false;
      this.showVideo = false;
      this.showImage = false;
    },
    addTag() {
      this.tags.push(this.tagStr);
      this.tagStr = null;
    },
    setHtml(data) {
      // make ajax request
      const label = data.label || 'Submit'
      const html = `
        <div>
          <progress x-show="loading"></progress>
          <fieldset x-data="formFields({fields})"></fieldset>
         
            <div >
              <!-- Tags visible -->
              <fieldset role="search" x-show="showTags">
                <input name="Tag" type="text" x-model="tagStr" placeholder="tag your post.." />
                <button class="small flat secondary material-icons" @click="addTag" :disabled="tagStr == null">add</button>
                <button class="secondary flat material-icons" @click="showTags = !showTags"  :disabled="loading">chevron_right</button>
                <button class="" @click="await submit(fields)" :disabled="loading">${label}</button>
              </fieldset>
            
              <fieldset role="search" x-show="!showTags">
                <!--Types-->
                <button class="small secondary material-icons flat" x-show="!typeSelected" @click="showText = !showText" :disabled="loading">text_format</button>
                <button class="small secondary material-icons flat" x-show="!typeSelected" @click="showVideo = !showVideo" :disabled="loading">videocam</button>
                <button class="small secondary material-icons flat" x-show="!typeSelected" @click="showImage = !showImage" :disabled="loading">image</button>
                <!--Cancel-->
                <button class="small secondary material-icons flat" x-show="typeSelected" @click="cancelTypes" :disabled="loading">cancel</button>

                <!--Type formats-->
                <button class="small secondary material-icons flat small" x-show="showText" @click="showTags = !showTags" :disabled="loading">format_list_bulleted</button>
                <button class="small secondary material-icons flat small" x-show="showText" @click="showTags = !showTags" :disabled="loading">format_list_numbered</button>
                <button class="small secondary material-icons flat small" x-show="showText" @click="showTags = !showTags" :disabled="loading">link</button>
                <button class="small secondary material-icons flat small" x-show="showText" @click="showTags = !showTags" :disabled="loading">format_quote</button>
                <button class="small secondary material-icons flat small" x-show="showText" @click="showTags = !showTags" :disabled="loading">code</button>
                
                <input name="Tag" disabled type="text" placeholder="" />
                
                <button class=" secondary material-icons  flat" @click="showTags = !showTags" :disabled="loading">sell</button>
                
                <button class="" @click="await submit(fields)"  :disabled="loading">${label}</button>

              </fieldset>
            </ul>
         
          <nav x-show="tags.length > 0" x-transition.scale.origin.top>
            <!-- Tags -->
            <div class="grid" role="group" >
              <div class="container">
                <template x-for="(tag, i) in tags">
                  <button class="tag closable outline secondary small" x-text="tag" @click="tags.splice(i, 1)"></button>
                </template>
              </div>
            </div>
          </nav>
        </div>
      `
      this.$nextTick(() => {
        this.$root.innerHTML = html
      });
    },
  }
}