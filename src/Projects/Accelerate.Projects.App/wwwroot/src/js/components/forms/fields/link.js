export default function (data) {
    return ` 
        <!--Hidden input-->
        <input
            :type="field.type"
            :name="field.name"
            :value="field.value"
            x-model="field.value"
            :hidden="true"
            ></input>

            <template x-if="field.value">
                 <article class="quote" style="padding: 0px;">
                    <figure style="text-align:center;">
                      <img
                        style="max-height: 200px; border-radius: 8px"
                        :src="field.value.image"
                        :alt="field.value.title"
                      />
                    </figure>
                    <footer class="padless">
                        <sup x-text="field.value.url"></sup>
                        <div>
                            <b x-text="field.value.title"></b>
                            <p x-text="field.value.description"></p>
                        </div>
                        <button class="small secondary material-icons flat" @click="field.value = null">clear</button>
                    </footer>
                 </article>
            </template>
        
        <small 
            x-show="field.helper != null && field.helper.length > 0"
            :id="field.id || field.name+i" x-text="field.helper"></small>
        `
}