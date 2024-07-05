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
                 <article class="quote" style="padding: 4px;">
                    <i x-show="field.value.image" class="click small  material-icons " @click="field.value.image = null">clear</i>

                    <figure x-show="field.value.image" style="text-align:center;">
                        <img
                            style="max-height: 200px; border-radius: 8px"
                            :src="field.value.image"
                            :alt="field.value.title"
                        />
                    </figure> 
                    <footer class="padless">
                        <a class="secondary" href="field.value.url target="_blank"><sup x-text="field.value.url"></sup></a>
                        <div>
                            <div x-show="field.value.title">
                                <i class="click small material-icons " @click="field.value.title = null">clear</i>
                                <b x-text="field.value.title"></b>
                            </div>
                            <div x-show="field.value.description">
                                <i  class="click small  material-icons " @click="field.value.description = null">clear</i>
                                <p  x-text="field.value.description"></p>
                            </div>
                        </div>
                        <button class="small primary flat" @click="field.value = null">Remove</button>
                    </footer>
                 </article>
            </template>
        
        <small 
            x-show="field.helper != null && field.helper.length > 0"
            :id="field.id || field.name+i" x-text="field.helper"></small>
        `
}